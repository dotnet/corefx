// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Net.Http
{
    /// <summary>
    /// Used to read header lines of an HTTP response, where each line is separated by "\r\n".
    /// </summary>
    internal struct WinHttpResponseHeaderReader
    {
        private readonly char[] _buffer;
        private readonly int _length;
        private int _position;

        public WinHttpResponseHeaderReader(char[] buffer, int startIndex, int length)
        {
            CharArrayHelpers.DebugAssertArrayInputs(buffer, startIndex, length);

            _buffer = buffer;
            _position = startIndex;
            _length = length;
        }

        /// <summary>
        /// Reads a header line.
        /// Empty header lines are skipped, as are malformed header lines that are missing a colon character.
        /// </summary>
        /// <returns>true if the next header was read successfully, or false if all characters have been read.</returns>
        public bool ReadHeader(out string name, out string value)
        {
            int startIndex;
            int length;
            while (ReadLine(out startIndex, out length))
            {
                // Skip empty lines.
                if (length == 0)
                {
                    continue;
                }

                int colonIndex = Array.IndexOf(_buffer, ':', startIndex, length);

                // Skip malformed header lines that are missing the colon character.
                if (colonIndex == -1)
                {
                    continue;
                }

                int nameLength = colonIndex - startIndex;

                // If it's a known header name, use the known name instead of allocating a new string.
                if (!HttpKnownHeaderNames.TryGetHeaderName(_buffer, startIndex, nameLength, out name))
                {
                    name = new string(_buffer, startIndex, nameLength);
                }

                // Normalize header value by trimming whitespace.
                int valueStartIndex = colonIndex + 1;
                int valueLength = startIndex + length - colonIndex - 1;
                CharArrayHelpers.Trim(_buffer, ref valueStartIndex, ref valueLength);

                value = HttpKnownHeaderNames.GetHeaderValue(name, _buffer, valueStartIndex, valueLength);

                return true;
            }

            name = null;
            value = null;
            return false;
        }

        /// <summary>
        /// Reads lines separated by "\r\n".
        /// </summary>
        /// <returns>true if the next line was read successfully, or false if all characters have been read.</returns>
        public bool ReadLine()
        {
            int startIndex;
            int length;
            return ReadLine(out startIndex, out length);
        }

        /// <summary>
        /// Reads lines separated by "\r\n".
        /// </summary>
        /// <param name="startIndex">The start of the line segment.</param>
        /// <param name="length">The length of the line segment.</param>
        /// <returns>true if the next line was read successfully, or false if all characters have been read.</returns>
        private bool ReadLine(out int startIndex, out int length)
        {
            Debug.Assert(_buffer != null);

            int i = _position;

            while (i < _length)
            {
                char ch = _buffer[i];
                if (ch == '\r')
                {
                    int next = i + 1;
                    if (next < _length && _buffer[next] == '\n')
                    {
                        startIndex = _position;
                        length = i - _position;
                        _position = i + 2;
                        return true;
                    }
                }
                i++;
            }

            if (i > _position)
            {
                startIndex = _position;
                length = i - _position;
                _position = i;
                return true;
            }

            startIndex = 0;
            length = 0;
            return false;
        }
    }
}
