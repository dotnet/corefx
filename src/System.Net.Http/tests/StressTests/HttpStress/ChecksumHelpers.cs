// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using Crc32 = Force.Crc32.Crc32Algorithm;

namespace HttpStress
{
    public static class ChecksumHelpers
    {
        public static void Append(byte[] bytes, ref uint accumulator)
        {
            accumulator = Crc32.Append(accumulator, bytes);
        }

        public static void Append(string text, ref uint accumulator, Encoding encoding = null)
        {
            if (encoding == null) 
            {
                encoding = Encoding.ASCII;
            }
            
            accumulator = Crc32.Append(accumulator, encoding.GetBytes(text));
        }

        public static uint Compute(byte[] bytes)
        {
            return Crc32.Compute(bytes);
        }

        public static uint ComputeHeaderChecksum<T>(IEnumerable<(string name, T)> headers) where T : IEnumerable<string>
        {
            uint checksum = 0;

            foreach ((string name, IEnumerable<string> values) in headers)
            {
                Append(name, ref checksum);
                foreach (string value in values) 
                {
                    Append(value, ref checksum);
                }
            }

            return checksum;
        }
    }
}