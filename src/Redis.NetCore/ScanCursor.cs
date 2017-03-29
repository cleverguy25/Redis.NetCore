// <copyright file="ScanCursor.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Text;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public class ScanCursor
    {
        private readonly byte[] _keys;
        private int _currentPosition = 0;

        public ScanCursor()
        {
        }

        public ScanCursor(int cursorPosition, byte[] keys)
        {
            CursorPosition = cursorPosition;
            _keys = keys;
        }

        public int CursorPosition { get; set; }

        public IEnumerable<string> GetKeys()
        {
            var firstChar = _keys[_currentPosition];
            _currentPosition++;
            if (firstChar != RedisProtocolContants.Array)
            {
                yield break;
            }

            var length = GetLength();
            for (var i = 0; i < length; i++)
            {
                firstChar = _keys[_currentPosition];
                _currentPosition++;
                if (firstChar != RedisProtocolContants.BulkString)
                {
                    yield break;
                }

                var stringLength = GetLength();
                var stringBytes = new byte[stringLength];
                for (var stringIndex = 0; stringIndex < stringLength; stringIndex++)
                {
                    stringBytes[stringIndex] = _keys[_currentPosition];
                    _currentPosition++;
                }

                _currentPosition += 2;
                yield return Encoding.UTF8.GetString(stringBytes);
            }
        }

        private int GetLength()
        {
            var length = 0;
            var sign = 1;

            var currentChar = _keys[_currentPosition];
            if (currentChar == RedisProtocolContants.Minus)
            {
                sign = -1;
                _currentPosition++;
            }

            currentChar = _keys[_currentPosition];
            while (currentChar != RedisProtocolContants.LineFeed)
            {
                if (currentChar != RedisProtocolContants.CarriageReturn)
                {
                    length = (length * 10) + currentChar - RedisProtocolContants.Zero;
                }

                _currentPosition++;

                currentChar = _keys[_currentPosition];
            }

            _currentPosition++;
            return length * sign;
        }
    }
}