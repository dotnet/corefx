// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Http.HPack
{
    internal static class IntegerEncoder
    {
        public static bool Encode(int i, int n, Span<byte> buffer, out int length)
        {
            int j = 0;
            length = 0;

            if (buffer.Length == 0)
            {
                return false;
            }

            buffer[j] &= MaskHigh(8 - n);
            if (i < (1 << n) - 1)
            {
                buffer[j++] |= (byte)i;
            }
            else
            {
                buffer[j++] |= (byte)((1 << n) - 1);

                if (j == buffer.Length)
                {
                    return false;
                }

                i = i - ((1 << n) - 1);
                while (i >= 128)
                {
                    buffer[j++] = (byte)(i % 128 + 128);

                    if (j > buffer.Length)
                    {
                        return false;
                    }

                    i = i / 128;
                }
                buffer[j++] = (byte)i;
            }

            length = j;
            return true;
        }

        private static byte MaskHigh(int n)
        {
            return (byte)(sbyte.MinValue >> (n - 1));
        }
    }
}
