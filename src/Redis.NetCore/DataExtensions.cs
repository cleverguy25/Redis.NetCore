using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Redis.NetCore
{
    public static class DataExtensions
    {

        public static byte[] ToBytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static byte ToByte(this char value)
        {
            return BitConverter.GetBytes(value)[0];
        }

        public static byte[] ToBytes(this int value)
        {
            return ToBytes(value.ToString(CultureInfo.InvariantCulture));
        }

        public static byte[] CollapseArray(this IEnumerable<byte[]> value)
        {
            return value?.SelectMany(item => item)?.ToArray();
        }
    }
}
