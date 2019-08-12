// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using Force.Crc32;

namespace HttpStress
{
    public static class Crc32Helpers
    {
        public static void Append(byte[] bytes, ref uint accumulator)
        {
            accumulator = Crc32Algorithm.Append(accumulator, bytes);
        }

        public static void Append(string text, ref uint accumulator, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.ASCII;
            accumulator = Crc32Algorithm.Append(accumulator, encoding.GetBytes(text));
        }

        public static uint CalculateHeaderChecksum<T>(IEnumerable<(string name, T)> headers) where T : IEnumerable<string>
        {
            uint checksum = 0;

            foreach ((string name, IEnumerable<string> values) in headers)
            {
                Append(name, ref checksum);
                foreach (string value in values) Append(value, ref checksum);
            }

            return checksum;
        }
    }
}