using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddRateLimiter(opt =>
    {
        opt.AddFixedWindowLimiter("fixedWindowPolicy", option =>
        {
            // 3 requests each 10 seconds
            option.Window = TimeSpan.FromSeconds(10);
            option.PermitLimit = 3;
        });
    })
    .AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapReverseProxy();
app.UseRateLimiter();

app.Run();
