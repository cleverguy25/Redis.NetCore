using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Redis.NetCore
{
    public static class EndpointResolution
    {
        public static async Task<EndPoint> GetEndpointAsync(string endpointConfiguration)
        {
            var parts = endpointConfiguration.Split(':');
            if (parts.Length == 0)
            {
                throw new ArgumentException($"Could not parse value [{endpointConfiguration}]", nameof(endpointConfiguration));
            }


            var host = parts[0];
            var address = await GetIpAddressAsync(host).ConfigureAwait(false);

            var port = GetPort(endpointConfiguration, parts);

            return new IPEndPoint(address, port);
        }

        private static int GetPort(string endpointConfiguration, string[] parts)
        {
            var port = 6379;
            if (parts.Length != 2)
            {
                return port;
            }

            var portConfiguration = parts[1];
            if (int.TryParse(portConfiguration, out port) == false)
            {
                throw new ArgumentException(
                                            $"Could not parse port [{portConfiguration}] from [{endpointConfiguration}]",
                                            nameof(endpointConfiguration));
            }

            return port;
        }

        private static async Task<IPAddress> GetIpAddressAsync(string host)
        {
            IPAddress address;
            if (IsIpAddress(host))
            {
                address = IPAddress.Parse(host);
            }

            var resolvedAddresses = await Dns.GetHostEntryAsync(host).ConfigureAwait(false);
            address = resolvedAddresses.AddressList.First(resolvedAddress => resolvedAddress.AddressFamily == AddressFamily.InterNetwork);
            return address;
        }

        private static bool IsIpAddress(string host)
        {
            return Regex.IsMatch(host, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        }
    }
}
