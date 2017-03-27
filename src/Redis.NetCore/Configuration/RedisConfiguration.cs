﻿namespace Redis.NetCore.Configuration
{
    public class RedisConfiguration
    {
        public string[] Endpoints { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; } = false;
    }
}
