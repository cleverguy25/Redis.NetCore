// <copyright file="TestClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Xunit;
using Redis.NetCore.Sockets;
using NSubstitute;

namespace Redis.NetCore.Tests
{
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public static class TestClient
    {
        public static IRedisClient CreateClient()
        {
            var redisConfiguration = new RedisConfiguration
            {
                Endpoints = new[] { "localhost:32768" }
            };

            DiagnosticListener.AllListeners.Subscribe(listener => SubscribeListener(listener));
            var client = RedisClient.CreateClient(redisConfiguration);
            return client;
        }

        public static async Task SetGetAsync(IRedisStringClient client, string key, string expected)
        {
            await client.SetStringAsync(key, expected);
            var actual = await client.GetStringAsync(key);
            Assert.Equal(expected, actual);
        }

        public static async Task QuickfireAsync(IRedisStringClient client, string label, int count = 1000)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                var task = client.SetStringAsync(label + i, "Value" + i);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            tasks = new List<Task>();
            var getTasksMap = new Dictionary<int, Task<string>>();
            for (var i = 0; i < count; i++)
            {
                var task = client.GetStringAsync(label + i);
                tasks.Add(task);
                getTasksMap[i] = task;
            }

            await Task.WhenAll(tasks);

            foreach (var keyPair in getTasksMap)
            {
                Assert.Equal("Value" + keyPair.Key, keyPair.Value.Result);
            }
        }

        public static Dictionary<string, string> SetupTestHashFields(string fieldName = "field", int count = 20)
        {
            var fields = new Dictionary<string, string>();
            for (var i = 0; i < 20; i++)
            {
                fields[$"{fieldName}{i}"] = $"value{i}";
            }

            return fields;
        }

        public static IEnumerable<(byte[] member, double score)> SetupTestSetItems(string prefix = "match", int count = 20)
        {
            for (var i = 0; i < 20; i++)
            {
                yield return ($"{prefix}{i}".ToBytes(), i);
            }
        }

        public static string[] SetupTestSetMembers(string memberPrefix = "", int count = 20)
        {
            var members = new EditableList<string>();
            for (var i = 0; i < 20; i++)
            {
                members.Add($"{memberPrefix}{i}");
            }

            return members.ToArray();
        }

        public static void SubscribeListener(DiagnosticListener listener)
        {
            if (Debugger.IsAttached == false)
            {
                return;
            }

            listener.Subscribe(@event =>
            {
                if (@event.Value == null)
                {
                    Debug.WriteLine($"From listener {listener.Name} received event {@event.Key}");
                }
                else
                {
                    Debug.WriteLine($"From listener {listener.Name} received event {@event.Key} with payload {@event.Value.ToString()}");
                }
            });
        }

        public static void SetupSocketResponse(IAsyncSocket socket, params string[] dataString)
        {
            SetupConnectAsync(socket);
            SetupSendAsync(socket);

            var awaitable = Substitute.For<ISocketAwaitable<ArraySegment<byte>>>();
            awaitable.GetAwaiter().Returns(awaitable);
            awaitable.IsCompleted.Returns(true);

            var i = 0;
            awaitable.GetResult().Returns(
                                          (context) =>
                                          {
                                              if (i >= dataString.Length)
                                              {
                                                  return new ArraySegment<byte>(new byte[0]);
                                              }

                                              var data = new ArraySegment<byte>(dataString[i].ToBytes());
                                              i++;
                                              return data;
                                          });

            socket.Connected.Returns(true);
            socket.ReceiveAsync(Arg.Any<ArraySegment<byte>>()).Returns(awaitable);
        }

        private static void SetupSendAsync(IAsyncSocket socket)
        {
            var awaitable = Substitute.For<ISocketAwaitable<int>>();
            awaitable.GetAwaiter().Returns(awaitable);
            awaitable.IsCompleted.Returns(true);
            awaitable.GetResult().Returns(1);
            socket.SendAsync(Arg.Any<IList<ArraySegment<byte>>>()).Returns(awaitable);
        }

        private static void SetupConnectAsync(IAsyncSocket socket)
        {
            var awaitable = Substitute.For<ISocketAwaitable<bool>>();
            awaitable.GetAwaiter().Returns(awaitable);
            awaitable.IsCompleted.Returns(true);
            awaitable.GetResult().Returns(true);
            socket.ConnectAsync().Returns(awaitable);
        }
    }
}