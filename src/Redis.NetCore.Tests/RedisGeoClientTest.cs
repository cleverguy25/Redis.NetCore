// <copyright file="RedisKeyClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Xunit;

namespace Redis.NetCore.Tests
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [Trait("Category", "Integration")]
    public class RedisGeoClientTest
    {
        private readonly GeoPosition _geoPosition1 = new GeoPosition(13.361389, 38.115556);
        private readonly GeoPosition _geoPosition2 = new GeoPosition(15.087269, 37.502669);

        [Fact]
        public async Task GeoPositionAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoPositionAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var positions = await client.GeoPositionAsync(key, "Palermo", "Catania", "NonExisting");
                Assert.Equal(_geoPosition1, positions[0]);
                Assert.Equal(_geoPosition2, positions[1]);
                Assert.Null(positions[2]);
            }
        }

        [Fact]
        public async Task GeoHashAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoHashAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var positions = await client.GeoHashAsync(key, "Palermo", "Catania", "NonExisting");
                Assert.Equal("sqc8b49rny0", positions[0]);
                Assert.Equal("sqdtr74hyu0", positions[1]);
                Assert.Null(positions[2]);
            }
        }

        [Fact]
        public async Task GeoDistanceAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoDistanceAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var distance = await client.GeoDistanceAsync(key, "Palermo", "Catania");
                Assert.Equal(166274.1516, distance);

                distance = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Kilometers);
                Assert.Equal(166.2742, distance);

                distance = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Miles);
                Assert.Equal(103.3182, distance);

                distance = await client.GeoDistanceAsync(key, "Foo", "Bar");
                Assert.Null(distance);
            }
        }

        private async Task GeoAddAsync(IRedisGeoClient client, string key)
        {
            var count = await client.GeoAddAsync(key, (_geoPosition1, "Palermo"), (_geoPosition2, "Catania"));
            Assert.Equal(2, count);
        }
    }
}