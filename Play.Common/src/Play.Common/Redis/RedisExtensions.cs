using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Interfaces;
using Play.Common.Settings;
using StackExchange.Redis;

namespace Play.Common.Redis
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            // using Microsoft.Extensions.Caching.StackExchangeRedis

            // services.AddStackExchangeRedisCache(redisOptions =>
            // {
            //     var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            //     var redisSettings = configuration?.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
            //     redisOptions.Configuration = redisSettings?.ConnectionString;
            // });

            // using StackExchange.Redis
            services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
                var redisSettings = configuration?.GetSection(nameof(RedisSettings)).Get<RedisSettings>();

                return ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { redisSettings!.ConnectionString }
                });
            });

            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}