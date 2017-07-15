// <copyright file="RedisGeoClientTest.cs" company="PayScale">
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

        [Fact]
        public async Task GeoRadiusAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersAsync();
                Array.Sort(members);
                Assert.Equal(new[] { "Catania", "Palermo" }, members);
            }
        }

        [Fact]
        public async Task GeoRadiusByMemberAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusByMemberAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                const string member = "Agrigento";
                await client.GeoAddAsync(key, (new GeoPosition(13.583333, 37.316667), member));
                var members = await client.GeoRadius(key, member, 100, GeoUnit.Kilometers)
                    .GetMembersAsync();
                Array.Sort(members);
                Assert.Equal(new[] { member, "Palermo" }, members);
            }
        }

        [Fact]
        public async Task GeoRadiusAscendingAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusAscendingAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .Ascending().GetMembersAsync();
                Assert.Equal(new[] { "Catania", "Palermo" }, members);
            }
        }

        [Fact]
        public async Task GeoRadiusCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusCountAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .Count(1).Descending().GetMembersAsync();
                Assert.Equal(new[] { "Palermo" }, members);
            }
        }

        [Fact]
        public async Task GeoRadiusWithDistanceAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithDistanceAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersWithDistanceAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal(56.4413, orderedMembers[0].distance);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal(190.4424, orderedMembers[1].distance);
            }
        }

        [Fact]
        public async Task GeoRadiusWithCoordinatesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithCoordinatesAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersWithCoordinatesAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal(_geoPosition2, orderedMembers[0].position);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal(_geoPosition1, orderedMembers[1].position);
            }
        }

        [Fact]
        public async Task GeoRadiusWithDistanceAndCoordinatesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithDistanceAndCoordinatesAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersWithDistanceAndCordinatesAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal(56.4413, orderedMembers[0].distance);
                Assert.Equal(_geoPosition2, orderedMembers[0].position);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal(190.4424, orderedMembers[1].distance);
                Assert.Equal(_geoPosition1, orderedMembers[1].position);
            }
        }

        [Fact]
        public async Task GeoRadiusWithDistanceAndHashAndCoordinatesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithDistanceAndHashAndCoordinatesAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersWithDistanceAndHashAndCordinatesAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal(56.4413, orderedMembers[0].distance);
                Assert.Equal("3479447370796909", orderedMembers[0].hash);
                Assert.Equal(_geoPosition2, orderedMembers[0].position);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal(190.4424, orderedMembers[1].distance);
                Assert.Equal("3479099956230698", orderedMembers[1].hash);
                Assert.Equal(_geoPosition1, orderedMembers[1].position);
            }
        }

        [Fact]
        public async Task GeoRadiusWithDistanceAndHashAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithDistanceAndHashAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersWithDistanceAndHashAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal(56.4413, orderedMembers[0].distance);
                Assert.Equal("3479447370796909", orderedMembers[0].hash);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal(190.4424, orderedMembers[1].distance);
                Assert.Equal("3479099956230698", orderedMembers[1].hash);
            }
        }

        [Fact]
        public async Task GeoRadiusStoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusStoreAsync);
                const string storeKey = key + "Store";
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .StoreAsync(storeKey);
                var members = await client.SortedSetGetRangeStringAsync(storeKey, 0, -1);
                Array.Sort(members);
                Assert.Equal("Catania", members[0]);
                Assert.Equal("Palermo", members[1]);
            }
        }

        [Fact]
        public async Task GeoRadiusStoreDistanceAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusStoreAsync);
                const string storeKey = key + "Store";
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .StoreDistanceAsync(storeKey);
                var members = await client.SortedSetGetRangeWithScoresStringAsync(storeKey, 0, -1);
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal("56.4412578701582", orderedMembers[0].weight.ToString());
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal("190.442429847758", orderedMembers[1].weight.ToString());
            }
        }

        [Fact]
        public async Task GeoRadiusWithHashAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithHashAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers).GetMembersWithHashAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal("3479447370796909", orderedMembers[0].hash);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal("3479099956230698", orderedMembers[1].hash);
            }
        }

        [Fact]
        public async Task GeoRadiusWithHashAndCoordinatesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GeoRadiusWithHashAndCoordinatesAsync);
                await client.DeleteKeyAsync(key);
                await GeoAddAsync(client, key);
                var members = await client.GeoRadius(key, 15, 37, 200, GeoUnit.Kilometers)
                    .GetMembersWithHashAndCoordinatesAsync();
                var orderedMembers = members.OrderBy(member => member.member).ToArray();
                Assert.Equal(2, orderedMembers.Length);
                Assert.Equal("Catania", orderedMembers[0].member);
                Assert.Equal("3479447370796909", orderedMembers[0].hash);
                Assert.Equal(_geoPosition2, orderedMembers[0].position);
                Assert.Equal("Palermo", orderedMembers[1].member);
                Assert.Equal("3479099956230698", orderedMembers[1].hash);
                Assert.Equal(_geoPosition1, orderedMembers[1].position);
            }
        }

        private async Task GeoAddAsync(IRedisGeoClient client, string key)
        {
            var count = await client.GeoAddAsync(key, (_geoPosition1, "Palermo"), (_geoPosition2, "Catania"));
            Assert.Equal(2, count);
        }
    }
}