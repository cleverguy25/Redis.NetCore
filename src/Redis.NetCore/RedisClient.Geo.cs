// <copyright file="RedisClient.Geo.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public partial class RedisClient : IRedisGeoClient
    {
        public async Task<int> GeoAddAsync(string geoKey, params (GeoPosition position, string member)[] items)
        {
            CheckGeoKey(geoKey);
            var byteArray = ConvertTupleItemsToByteArray(items);

            var request = ComposeRequest(RedisCommands.GeoAdd, geoKey.ToBytes(), byteArray);
            var bytes = await SendCommandAsync(request);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<GeoPosition[]> GeoPositionAsync(string geoKey, params string[] members)
        {
            CheckGeoKey(geoKey);

            var request = ComposeRequest(RedisCommands.GeoPosition, geoKey.ToBytes(), members);
            var bytes = await SendMultipleCommandAsync(request);

            return (from item in bytes
                    select new ScanCursor(0, item) into cursor
                    select cursor.GetStringValues().ToArray() into positionParts
                    select positionParts.Length == 0 ? null : new GeoPosition(double.Parse(positionParts[0]), double.Parse(positionParts[1]))).ToArray();
        }

        public async Task<string[]> GeoHashAsync(string geoKey, params string[] members)
        {
            CheckGeoKey(geoKey);

            var request = ComposeRequest(RedisCommands.GeoHash, geoKey.ToBytes(), members);
            var bytes = await SendMultipleCommandAsync(request);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<double?> GeoDistanceAsync(string geoKey, string member1, string member2, GeoUnit unit)
        {
            var bytes = await SendCommandAsync(RedisCommands.GeoDistance, geoKey.ToBytes(), member1.ToBytes(), member2.ToBytes(), unit.Unit.ToBytes());
            return bytes?.ConvertBytesToDouble();
        }

        public async Task<double?> GeoDistanceAsync(string geoKey, string member1, string member2)
        {
            var bytes = await SendCommandAsync(RedisCommands.GeoDistance, geoKey.ToBytes(), member1.ToBytes(), member2.ToBytes());
            return bytes?.ConvertBytesToDouble();
        }

        public RedisGeoRadius GeoRadius(string geoKey, double longitude, double latitude, int radius, GeoUnit unit)
        {
            var request = ComposeRequest(RedisCommands.GeoRadius, geoKey.ToBytes(), longitude.ToBytes(), latitude.ToBytes(), radius.ToBytes(), unit.Unit.ToBytes());
            return new RedisGeoRadius(this, request);
        }

        public RedisGeoRadius GeoRadius(string geoKey, string member, int radius, GeoUnit unit)
        {
            var request = ComposeRequest(RedisCommands.GeoRadiusByMember, geoKey.ToBytes(), member.ToBytes(), radius.ToBytes(), unit.Unit.ToBytes());
            return new RedisGeoRadius(this, request);
        }

        private static byte[][] ConvertTupleItemsToByteArray(IReadOnlyList<(GeoPosition position, string member)> items)
        {
            var byteArray = new byte[items.Count * 3][];
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var position = i * 3;
                byteArray[position] = item.position.Longitude.ToBytes();
                byteArray[position + 1] = item.position.Latitude.ToBytes();
                byteArray[position + 2] = item.member.ToBytes();
            }

            return byteArray;
        }

        private static void CheckGeoKey(string geoKey)
        {
            if (string.IsNullOrEmpty(geoKey))
            {
                throw new ArgumentNullException(nameof(geoKey), "geoKey cannot be null or empty");
            }
        }
    }
}
