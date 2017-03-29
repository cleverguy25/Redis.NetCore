// <copyright file="RedisConfiguration.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Redis.NetCore.Configuration
{
    public class RedisConfiguration
    {
        public string[] Endpoints { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; } = false;
    }
}
