// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    internal static partial class JsonReaderHelper
    {
        // Reject any invalid UTF-8 data rather than silently replacing.
        public static readonly UTF8Encoding s_utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private const int StackallocThreshold = 256;

        // TODO: Similar to escaping, replace the unescaping logic with publicly shipping APIs from https://github.com/dotnet/corefx/issues/33509
        public static string GetUnescapedString(ReadOnlySpan<byte> utf8Source, int idx)
        {
            byte[] unescapedArray = null;
            string utf8String;

            Span<byte> unescaped = utf8Source.Length <= StackallocThreshold ?
                stackalloc byte[utf8Source.Length] :
                (unescapedArray = ArrayPool<byte>.Shared.Rent(utf8Source.Length));

            Unescape(utf8Source, unescaped, idx, out int written);
            utf8String = s_utf8Encoding.GetString(unescaped.Slice(0, written));

            if (unescapedArray != null)
            {
                ArrayPool<byte>.Shared.Return(unescapedArray);
            }

            return utf8String;
        }

        private static void Unescape(ReadOnlySpan<byte> source, Span<byte> destination, int idx, out int written)
        {
            Debug.Assert(idx >= 0 && idx < source.Length);
            Debug.Assert(source[idx] == JsonConstants.BackSlash);
            Debug.Assert(destination.Length >= source.Length);

            source.Slice(0, idx).CopyTo(destination);
            written = idx;

            for (; idx < source.Length; idx++)
            {
                byte currentByte = source[idx];
                if (currentByte == JsonConstants.BackSlash)
                {
                    idx++;
                    currentByte = source[idx];

                    if (currentByte == JsonConstants.Quote)
                    {
                        destination[written++] = JsonConstants.Quote;
                    }
                    else if (currentByte == 'n')
                    {
                        destination[written++] = JsonConstants.LineFeed;
                    }
                    else if (currentByte == 'r')
                    {
                        destination[written++] = JsonConstants.CarriageReturn;
                    }
                    else if (currentByte == JsonConstants.BackSlash)
                    {
                        destination[written++] = JsonConstants.BackSlash;
                    }
                    else if (currentByte == JsonConstants.Slash)
                    {
                        destination[written++] = JsonConstants.Slash;
                    }
                    else if (currentByte == 't')
                    {
                        destination[written++] = JsonConstants.Tab;
                    }
                    else if (currentByte == 'b')
                    {
                        destination[written++] = JsonConstants.BackSpace;
                    }
                    else if (currentByte == 'f')
                    {
                        destination[written++] = JsonConstants.FormFeed;
                    }
                    else if (currentByte == 'u')
                    {
                        Debug.Assert(source.Length >= idx + 5);

                        bool result = Utf8Parser.TryParse(source.Slice(idx + 1, 4), out int scalar, out int bytesConsumed, 'x');
                        Debug.Assert(result);
                        Debug.Assert(bytesConsumed == 4);
                        idx += bytesConsumed;     // The loop iteration will increment idx past the last hex digit

                        if (IsInRangeInclusive((uint)scalar, JsonConstants.HighSurrogateStartValue, JsonConstants.LowSurrogateEndValue))
                        {
                            // The first hex value cannot be a low surrogate
                            if (scalar >= JsonConstants.LowSurrogateStartValue)
                            {
                                ThrowHelper.ThrowInvalidOperationException_ReadInvalidUTF16(scalar);
                            }

                            // If the first hex value is a high surrogate, the next one must be a low surrogate
                            Debug.Assert(IsInRangeInclusive((uint)scalar, JsonConstants.HighSurrogateStartValue, JsonConstants.HighSurrogateEndValue));

                            idx += 3;   // Skip the last hex digit and \u

                            if (source.Length < idx + 4 || source[idx - 2] != '\\' || source[idx - 1] != 'u')
                            {
                                ThrowHelper.ThrowInvalidOperationException_ReadInvalidUTF16();
                            }

                            result = Utf8Parser.TryParse(source.Slice(idx, 4), out int lowSurrogate, out bytesConsumed, 'x');
                            Debug.Assert(result);
                            Debug.Assert(bytesConsumed == 4);

                            if (!IsInRangeInclusive((uint)lowSurrogate, JsonConstants.LowSurrogateStartValue, JsonConstants.LowSurrogateEndValue))
                            {
                                ThrowHelper.ThrowInvalidOperationException_ReadInvalidUTF16(lowSurrogate);
                            }

                            idx += bytesConsumed - 1;  // The loop iteration will increment idx past the last hex digit

                            // To find the unicode scalar:
                            // (0x400 * (High surrogate - 0xD800)) + Low surrogate - 0xDC00 + 0x10000
                            scalar = (JsonConstants.BitShiftBy10 * (scalar - JsonConstants.HighSurrogateStartValue))
                                + (lowSurrogate - JsonConstants.LowSurrogateStartValue)
                                + JsonConstants.UnicodePlane01StartValue;
                        }

                        EncodeToUtf8Bytes((uint)scalar, destination.Slice(written), out int bytesWritten);
                        Debug.Assert(bytesWritten <= 4);
                        written += bytesWritten;
                    }
                }
                else
                {
                    destination[written++] = currentByte;
                }
            }
        }

        /// <summary>
        /// Copies the UTF-8 code unit representation of this scalar to an output buffer.
        /// The buffer must be large enough to hold the required number of <see cref="byte"/>s.
        /// </summary>
        private static void EncodeToUtf8Bytes(uint scalar, Span<byte> utf8Destination, out int bytesWritten)
        {
            Debug.Assert(IsValidUnicodeScalar(scalar));
            Debug.Assert(utf8Destination.Length >= 4);

            if (scalar < 0x80U)
            {
                // Single UTF-8 code unit
                utf8Destination[0] = (byte)scalar;
                bytesWritten = 1;
            }
            else if (scalar < 0x800U)
            {
                // Two UTF-8 code units
                utf8Destination[0] = (byte)(0xC0U | (scalar >> 6));
                utf8Destination[1] = (byte)(0x80U | (scalar & 0x3FU));
                bytesWritten = 2;
            }
            else if (scalar < 0x10000U)
            {
                // Three UTF-8 code units
                utf8Destination[0] = (byte)(0xE0U | (scalar >> 12));
                utf8Destination[1] = (byte)(0x80U | ((scalar >> 6) & 0x3FU));
                utf8Destination[2] = (byte)(0x80U | (scalar & 0x3FU));
                bytesWritten = 3;
            }
            else
            {
                // Four UTF-8 code units
                utf8Destination[0] = (byte)(0xF0U | (scalar >> 18));
                utf8Destination[1] = (byte)(0x80U | ((scalar >> 12) & 0x3FU));
                utf8Destination[2] = (byte)(0x80U | ((scalar >> 6) & 0x3FU));
                utf8Destination[3] = (byte)(0x80U | (scalar & 0x3FU));
                bytesWritten = 4;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> iff <paramref name="value"/> is a valid Unicode scalar
        /// value, i.e., is in [ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidUnicodeScalar(uint value)
        {
            // By XORing the incoming value with 0xD800, surrogate code points
            // are moved to the range [ U+0000..U+07FF ], and all valid scalar
            // values are clustered into the single range [ U+0800..U+10FFFF ],
            // which allows performing a single fast range check.

            return IsInRangeInclusive(value ^ 0xD800U, 0x800U, 0x10FFFFU);
        }

        /// <summary>
        /// Returns <see langword="true"/> iff <paramref name="value"/> is between
        /// <paramref name="lowerBound"/> and <paramref name="upperBound"/>, inclusive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound)
            => (value - lowerBound) <= (upperBound - lowerBound);
    }
}
