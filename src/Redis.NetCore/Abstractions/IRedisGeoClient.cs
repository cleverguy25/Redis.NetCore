// <copyright file="IRedisGeoClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public interface IRedisGeoClient
    {
        Task<int> GeoAddAsync(string geoKey, params (GeoPosition position, string member)[] items);

        Task<GeoPosition[]> GeoPositionAsync(string geoKey, params string[] members);

        Task<string[]> GeoHashAsync(string geoKey, params string[] members);

        Task<double?> GeoDistanceAsync(string geoKey, string member1, string member2, GeoUnit unit);

        Task<double?> GeoDistanceAsync(string geoKey, string member1, string member2);

        RedisGeoRadius GeoRadius(string geoKey, double longitude, double latitude, int radius, GeoUnit unit);

        RedisGeoRadius GeoRadius(string geoKey, string member, int radius, GeoUnit unit);
    }
}
