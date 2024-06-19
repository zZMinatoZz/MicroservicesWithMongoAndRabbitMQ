using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Common.Settings
{
    public class RedisSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string ConnectionString => $"{Host}:{Port}";
    }
}