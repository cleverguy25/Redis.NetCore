// <copyright file="RedisException.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Redis.NetCore
{
    public class RedisException : Exception
    {
        public RedisException(string message)
            : base(message)
        {
        }

        public RedisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
