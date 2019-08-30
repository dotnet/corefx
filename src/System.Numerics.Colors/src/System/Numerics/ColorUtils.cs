// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics
{
    /// <summary>
    /// Contains some internal helper methods for usage with Color types.
    /// </summary>
    internal static partial class ColorUtils
    {
        /// <summary>
        /// Parses two hexadecimal characters into a color channel value from 0 to 255.
        /// </summary>
        /// <param name="str">The hexadecimal string to parse.</param>
        /// <param name="offset">Which two characters should be parsed, as an offset from the front.</param>
        /// <returns>The color channel value on the range 0 to 255, or -1 if invalid.</returns>
        internal static int ParseCol8(string str, int offset)
        {
            int ig = 0;

            for (int i = 0; i < 2; i++)
            {
                int c = str[i + offset];
                int v;

                if (c >= '0' && c <= '9')
                {
                    v = c - '0';
                }
                else if (c >= 'a' && c <= 'f')
                {
                    v = c - 'a';
                    v += 10;
                }
                else if (c >= 'A' && c <= 'F')
                {
                    v = c - 'A';
                    v += 10;
                }
                else
                {
                    return -1;
                }

                if (i == 0)
                    ig += v * 16;
                else
                    ig += v;
            }

            return ig;
        }

        /// <summary>
        /// Converts the given Color channel value to a 2-character string representing its 8-bit hexadecimal value.
        /// </summary>
        /// <param name="val">The input value, should be on the range 0 to 1.</param>
        /// <returns>2-character string representing the hexadecimal color channel value.</returns>
        internal static string ToHex8(float val)
        {
            int v = (int)Math.Round(Math.Clamp(val * 255.0f, 0.0f, 255.0f));

            var ret = string.Empty;

            for (int i = 0; i < 2; i++)
            {
                char c;
                int lv = v & 0xF;

                if (lv < 10)
                    c = (char)('0' + lv);
                else
                    c = (char)('a' + lv - 10);

                v >>= 4;
                ret = c + ret;
            }

            return ret;
        }

        /// <summary>
        /// Converts the given Color channel value to a 2-character string representing its 8-bit hexadecimal value.
        /// </summary>
        /// <param name="val">The input value, should be on the range 0 to 255.</param>
        /// <returns>2-character string representing the hexadecimal color channel value.</returns>
        internal static string ToHex8(byte val)
        {
            var ret = string.Empty;

            for (int i = 0; i < 2; i++)
            {
                char c;
                int lv = val & 0xF;

                if (lv < 10)
                    c = (char)('0' + lv);
                else
                    c = (char)('a' + lv - 10);

                val >>= 4;
                ret = c + ret;
            }

            return ret;
        }
    }
}
