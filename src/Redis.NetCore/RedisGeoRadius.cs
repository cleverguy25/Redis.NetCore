using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public class RedisGeoRadius
    {
        private readonly RedisClient _client;
        private readonly List<byte[]> _request = new List<byte[]>();

        public RedisGeoRadius(RedisClient client, IEnumerable<byte[]> request)
        {
            _client = client;
            _request.AddRange(request);
        }

        public RedisGeoRadius Ascending()
        {
            _request.Add(RedisCommands.Ascending);
            return this;
        }

        public RedisGeoRadius Descending()
        {
            _request.Add(RedisCommands.Descending);
            return this;
        }

        public RedisGeoRadius Count(int count)
        {
            _request.Add(RedisCommands.Count);
            _request.Add(count.ToBytes());
            return this;
        }

        public async Task<string[]> GetMembersAsync()
        {
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<(string member, double distance)[]> GetMembersWithDistanceAsync()
        {
            _request.Add(RedisCommands.WithDistance);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetStringValues().ToArray()
                    into parts
                    let member = parts[0]
                    let distance = double.Parse(parts[1])
                    select (parts[0], distance)).ToArray();
        }

        public async Task<(string member, double distance, string hash)[]> GetMembersWithDistanceAndHashAsync()
        {
            _request.Add(RedisCommands.WithDistance);
            _request.Add(RedisCommands.WithHash);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetValues().ToArray()
                    into parts
                    let member = parts[0].ConvertBytesToString()
                    let distance = double.Parse(parts[1].ConvertBytesToString())
                    let hash = parts[2].ConvertBytesToString()
                    select (member, distance, hash)).ToArray();
        }

        public async Task<(string member, double distance, GeoPosition position)[]> GetMembersWithDistanceAndCordinatesAsync()
        {
            _request.Add(RedisCommands.WithDistance);
            _request.Add(RedisCommands.WithCoordinates);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetValues().ToArray()
                    into parts
                    let member = parts[0].ConvertBytesToString()
                    let distance = double.Parse(parts[1].ConvertBytesToString())
                    let position = GetGeoPosition(parts[2])
                    select (member, distance, position)).ToArray();
        }

        public async Task<(string member, double distance, string hash, GeoPosition position)[]> GetMembersWithDistanceAndHashAndCordinatesAsync()
        {
            _request.Add(RedisCommands.WithDistance);
            _request.Add(RedisCommands.WithCoordinates);
            _request.Add(RedisCommands.WithHash);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetValues().ToArray()
                    into parts
                    let member = parts[0].ConvertBytesToString()
                    let distance = double.Parse(parts[1].ConvertBytesToString())
                    let hash = parts[2].ConvertBytesToString()
                    let position = GetGeoPosition(parts[3])
                    select (member, distance, hash, position)).ToArray();
        }

        public async Task<(string member, GeoPosition position)[]> GetMembersWithCoordinatesAsync()
        {
            _request.Add(RedisCommands.WithCoordinates);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetValues().ToArray()
                    into parts
                    let member = parts[0].ConvertBytesToString()
                    let position = GetGeoPosition(parts[1])
                    select (member, position)).ToArray();
        }

        public async Task<(string member, string hash, GeoPosition position)[]> GetMembersWithHashAndCoordinatesAsync()
        {
            _request.Add(RedisCommands.WithHash);
            _request.Add(RedisCommands.WithCoordinates);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetValues().ToArray()
                    into parts
                    let member = parts[0].ConvertBytesToString()
                    let hash = parts[1].ConvertBytesToString()
                    let position = GetGeoPosition(parts[2])
                    select (member, hash, position)).ToArray();
        }

        public async Task<(string member, string hash)[]> GetMembersWithHashAsync()
        {
            _request.Add(RedisCommands.WithHash);
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);

            return (from item in bytes
                    select new ScanCursor(0, item)
                    into cursor
                    select cursor.GetValues().ToArray()
                    into parts
                    let member = parts[0].ConvertBytesToString()
                    let hash = parts[1].ConvertBytesToString()
                    select (member, hash)).ToArray();
        }

        public Task StoreAsync(string key)
        {
            RedisClient.CheckKey(key);

            _request.Add(RedisCommands.Store);
            _request.Add(key.ToBytes());
            return _client.SendMultipleCommandAsync(_request.ToArray());
        }

        public Task StoreDistanceAsync(string key)
        {
            RedisClient.CheckKey(key);

            _request.Add(RedisCommands.StoreDistance);
            _request.Add(key.ToBytes());
            return _client.SendMultipleCommandAsync(_request.ToArray());
        }

        private static GeoPosition GetGeoPosition(byte[] positionPart)
        {
            var positionCursor = new ScanCursor(0, positionPart);
            var positionSubParts = positionCursor.GetStringValues().ToArray();
            var longitude = double.Parse(positionSubParts[0]);
            var latitude = double.Parse(positionSubParts[1]);
            return new GeoPosition(longitude, latitude);
        }
    }
}