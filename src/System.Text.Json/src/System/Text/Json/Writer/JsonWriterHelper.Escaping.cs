// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;

#if BUILDING_INBOX_LIBRARY
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace System.Text.Json
{
    // TODO: Replace the escaping logic with publicly shipping APIs from https://github.com/dotnet/corefx/issues/33509
    internal static partial class JsonWriterHelper
    {
        // Only allow ASCII characters between ' ' (0x20) and '~' (0x7E), inclusively,
        // but exclude characters that need to be escaped as hex: '"', '\'', '&', '+', '<', '>', '`'
        // and exclude characters that need to be escaped by adding a backslash: '\n', '\r', '\t', '\\', '\b', '\f'
        //
        // non-zero = allowed, 0 = disallowed
        public const int LastAsciiCharacter = 0x7F;
        private static ReadOnlySpan<byte> AllowList => new byte[byte.MaxValue + 1]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // U+0000..U+000F
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // U+0010..U+001F
            1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, // U+0020..U+002F
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, // U+0030..U+003F
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // U+0040..U+004F
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, // U+0050..U+005F
            0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // U+0060..U+006F
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, // U+0070..U+007F

            // Also include the ranges from U+0080 to U+00FF for performance to avoid UTF8 code from checking boundary.
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // U+00F0..U+00FF
        };

#if BUILDING_INBOX_LIBRARY
        private const string HexFormatString = "X4";
#endif

        private static readonly StandardFormat s_hexStandardFormat = new StandardFormat('X', 4);

        private static bool NeedsEscaping(byte value) => AllowList[value] == 0;

        private static bool NeedsEscapingNoBoundsCheck(char value) => AllowList[value] == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool NeedsEscaping(char value) => value > LastAsciiCharacter || AllowList[value] == 0;

#if BUILDING_INBOX_LIBRARY
        private static readonly Vector128<sbyte> s_mask_SByte_0x20 = Vector128.Create((sbyte)0x20); // Space ' '
        private static readonly Vector128<sbyte> s_mask_SByte_0x22 = Vector128.Create((sbyte)0x22); // Quotation Mark '"'
        private static readonly Vector128<sbyte> s_mask_SByte_0x26 = Vector128.Create((sbyte)0x26); // Ampersand '&'
        private static readonly Vector128<sbyte> s_mask_SByte_0x27 = Vector128.Create((sbyte)0x27); // Apostrophe '''
        private static readonly Vector128<sbyte> s_mask_SByte_0x2B = Vector128.Create((sbyte)0x2B); // Plus sign '+'
        private static readonly Vector128<sbyte> s_mask_SByte_0x3C = Vector128.Create((sbyte)0x3C); // Less Than Sign '<'
        private static readonly Vector128<sbyte> s_mask_SByte_0x3E = Vector128.Create((sbyte)0x3E); // Greater Than Sign '>'
        private static readonly Vector128<sbyte> s_mask_SByte_0x5C = Vector128.Create((sbyte)0x5C); // Reverse Solidus '\'
        private static readonly Vector128<sbyte> s_mask_SByte_0x60 = Vector128.Create((sbyte)0x60); // Grave Access '`'

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<sbyte> CreateEscapingMaskSse2(Vector128<sbyte> sourceValue)
        {
            Debug.Assert(Sse2.IsSupported);

            Vector128<sbyte> mask = Sse2.CompareLessThan(sourceValue, s_mask_SByte_0x20); // Control characters, and anything above 0x7E since sbyte.MaxValue is 0x7E

            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x22)); // Quotation Mark "
            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x26)); // Ampersand &
            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x27)); // Apostrophe '
            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x2B)); // Plus sign +

            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x3C)); // Less Than Sign <
            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x3E)); // Greater Than Sign >
            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x5C)); // Reverse Solidus \
            mask = Sse2.Or(mask, Sse2.CompareEqual(sourceValue, s_mask_SByte_0x60)); // Grave Access `

            return mask;
        }

        // See comment below in method CreateEscapingMaskSsse3 for description of the bit-mask.
        private static ReadOnlySpan<byte> Bitmask => new byte[16]
        {
            0b_01000011,        // low-nibble 0
            0b_00000011,        // low-nibble 1
            0b_00000111,        // low-nibble 2
            0b_00000011,        // low-nibble 3
            0b_00000011,        // low-nibble 4
            0b_00000111,        // low-nibble 5
            0b_00000111,        // low-nibble 6
            0b_00000111,        // low-nibble 7
            0b_00000011,        // low-nibble 8
            0b_00000011,        // low-nibble 9
            0b_00000011,        // low-nibble A
            0b_00000111,        // low-nibble B
            0b_00101011,        // low-nibble C
            0b_00000011,        // low-nibble D
            0b_00001011,        // low-nibble E
            0b_10000011,        // low-nibble F
        };

        // To check if a bit in a bitmask from the Bitmask is set, in a sequential code
        // we would do ((1 << bitIndex) & bitmask) != 0
        // As there is no hardware instrinic for such a shift, we use lookup that
        // stores the shifted bitpositions.
        // So (1 << bitIndex) becomes BitPosLook[bitIndex], which is simd-friendly.
        private static ReadOnlySpan<byte> BitPosLookup => new byte[16]
        {
            0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80,

            // A bitmask from the Bitmask (above) is created only for value 0..7 (one byte)
            // so to avoid a explicit check for values outside 0..7, i.e.
            // high nibbles 8..F, we use a bitpos that always results in escaping.
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
        };

        private static readonly Vector128<sbyte> s_mask_SByte_0xF = Vector128.Create((sbyte)0xF);
        private static readonly Vector128<sbyte> s_bitMask = Unsafe.ReadUnaligned<Vector128<sbyte>>(ref MemoryMarshal.GetReference(Bitmask));
        private static readonly Vector128<sbyte> s_bitPosLookup = Unsafe.ReadUnaligned<Vector128<sbyte>>(ref MemoryMarshal.GetReference(BitPosLookup));
        private static readonly Vector128<sbyte> s_zero128 = Vector128<sbyte>.Zero;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<sbyte> CreateEscapingMaskSsse3(Vector128<sbyte> sourceValue)
        {
            // To check if an input byte needs to be escaped or not, we create bit-mask.
            // Therefore we split the input byte into the low- and high-nibble, which will get
            // the row-/column-index in the bit-mask.
            // The bit-mask-matrix looks like
            //                                     high-nibble
            // low-nibble  0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F
            //         0   1   1   0   0   0   0   1   0   1   1   1   1   1   1   1   1
            //         1   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         2   1   1   1   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         3   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         4   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         5   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         6   1   1   1   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         7   1   1   1   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         8   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         9   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         A   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         B   1   1   1   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         C   1   1   0   1   0   1   0   0   1   1   1   1   1   1   1   1
            //         D   1   1   0   0   0   0   0   0   1   1   1   1   1   1   1   1
            //         E   1   1   0   1   0   0   0   0   1   1   1   1   1   1   1   1
            //         F   1   1   0   0   0   0   0   1   1   1   1   1   1   1   1   1
            //
            // where 1 denotes the neeed for escaping, while 0 means no escaping needed.
            // For high-nibbles in the range 8..F every input needs to be escaped, so we
            // can omit them in the bit-mask, thus only high-nibbles in the range 0..7 need
            // to be considered, hence the entries in the bit-mask can be of type byte.
            //
            // In the Bitmask (see above) for each row (= low-nibble) a bit-mask for the
            // high-nibbles (= columns) is created.

            Debug.Assert(Ssse3.IsSupported);

            Vector128<sbyte> highNibbles = Sse2.And(Sse2.ShiftRightLogical(sourceValue.AsInt32(), 4).AsSByte(), s_mask_SByte_0xF);
            Vector128<sbyte> lowNibbles = Sse2.And(sourceValue, s_mask_SByte_0xF);

            Vector128<sbyte> bitMask = Ssse3.Shuffle(s_bitMask, lowNibbles);
            Vector128<sbyte> bitPositions = Ssse3.Shuffle(s_bitPosLookup, highNibbles);

            Vector128<sbyte> mask = Sse2.And(bitPositions, bitMask);
            mask = Sse2.CompareGreaterThan(mask, s_zero128);

            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NeedsEscapingCore(Vector128<sbyte> sourceValue)
        {
            // Check if any of the 16 bytes need to be escaped.
            Vector128<sbyte> mask = Ssse3.IsSupported
                ? CreateEscapingMaskSsse3(sourceValue)
                : CreateEscapingMaskSse2(sourceValue);

            int index = Sse2.MoveMask(mask.AsByte());

            return index;
        }

        // PERF: don't manually inline or call this method in NeedsEscapingCore
        // as the resulting asm won't be great
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndexOfFirstNeedToEscape(int index)
        {
            // Found at least one byte that needs to be escaped, figure out the index of
            // the first one found that needed to be escaped within the 16 bytes.
            Debug.Assert(index > 0 && index <= 65_535);
            int tzc = BitOperations.TrailingZeroCount(index);
            Debug.Assert(tzc >= 0 && tzc <= 16);

            return tzc;
        }
#endif

        public static unsafe int NeedsEscaping(ReadOnlySpan<byte> value, JavaScriptEncoder encoder)
        {
            fixed (byte* pValue = value)
            {
                int idx = 0;

                if (encoder == null)
                {
                    byte* ptr = pValue;
                    byte* end = ptr + value.Length;

#if BUILDING_INBOX_LIBRARY
                    if (Sse2.IsSupported)
                    {
                        byte* vectorizedEnd = end - Vector128<byte>.Count;

                        if (ptr <= vectorizedEnd)
                        {
                            int index;

                            do
                            {
                                Debug.Assert(pValue <= ptr && ptr <= (pValue + value.Length - Vector128<byte>.Count));
                                // Load the next 16 bytes
                                Vector128<sbyte> sourceValue = Sse2.LoadVector128((sbyte*)ptr);

                                index = NeedsEscapingCore(sourceValue);

                                // If index == 0, that means none of the 16 bytes needed to be escaped.
                                // TrailingZeroCount is relatively expensive, avoid it if possible.
                                if (index != 0)
                                {
                                    goto VectorizedFound;
                                }

                                ptr += Vector128<sbyte>.Count;
                            }
                            while (ptr <= vectorizedEnd);

                            // Process the remaining elements.
                            Debug.Assert(end - ptr < Vector128<byte>.Count);

                            const int thresholdForRemainingVectorized = 4;
                            // Process the remaining elements vectorized, only if the remaining count
                            // is above thresholdForRemainingVectorized, otherwise process them sequential.
                            if (ptr < end - thresholdForRemainingVectorized)
                            {
                                // PERF: duplicate instead of jumping at the beginning of the previous loop
                                // otherwise all the static data (vectors) will be re-assigned to registers,
                                // so they are re-used.

                                Debug.Assert(pValue <= vectorizedEnd && vectorizedEnd <= (pValue + value.Length - Vector128<byte>.Count));

                                // Load the last 16 bytes
                                Vector128<sbyte> sourceValue = Sse2.LoadVector128((sbyte*)vectorizedEnd);

                                index = NeedsEscapingCore(sourceValue);
                                if (index != 0)
                                {
                                    ptr = vectorizedEnd;
                                    goto VectorizedFound;
                                }

                                goto NothingFound;
                            }

                            goto Sequential;

                        VectorizedFound:
                            idx = GetIndexOfFirstNeedToEscape(index);
                            goto EscapeFound;
                        }
                    }
#endif

                Sequential:
                    while (ptr < end)
                    {
                        Debug.Assert(pValue <= ptr && ptr < (pValue + value.Length));

                        if (NeedsEscaping(*ptr))
                        {
                            goto EscapeFound;
                        }

                        ptr++;
                    }

                NothingFound:
                    idx = -1; // all characters allowed
                    goto Return;

                EscapeFound:
                    idx += (int)(ptr - pValue);

                Return:
                    return idx;
                }
                else
                {
                    return encoder.FindFirstCharacterToEncodeUtf8(value);
                }
            }
        }

        public static unsafe int NeedsEscaping(ReadOnlySpan<char> value, JavaScriptEncoder encoder)
        {
            fixed (char* pValue = value)
            {
                int idx = 0;

                // Some implementations of JavascriptEncoder.FindFirstCharacterToEncode may not accept
                // null pointers and gaurd against that. Hence, check up-front and fall down to return -1.
                if (encoder == null || value.IsEmpty)
                {
                    short* ptr = (short*)pValue;
                    short* end = ptr + value.Length;

#if BUILDING_INBOX_LIBRARY
                    if (Sse2.IsSupported && value.Length >= 8)
                    {
                        short* vectorizedEnd = end - 2 * Vector128<short>.Count;
                        int index;

                        while (ptr <= vectorizedEnd)
                        {
                            Debug.Assert(pValue <= ptr && ptr <= (pValue + value.Length - 2 * Vector128<short>.Count));

                            // Load the next 16 characters, combine them to one byte vector
                            Vector128<sbyte> sourceValue = Sse2.PackSignedSaturate(
                                Sse2.LoadVector128(ptr),
                                Sse2.LoadVector128(ptr + Vector128<short>.Count));

                            // Check if any of the 16 characters need to be escaped.
                            index = NeedsEscapingCore(sourceValue);

                            // If index == 0, that means none of the 16 characters needed to be escaped.
                            // TrailingZeroCount is relatively expensive, avoid it if possible.
                            if (index != 0)
                            {
                                goto VectorizedFound;
                            }

                            ptr += 2 * Vector128<short>.Count;
                        }

                        vectorizedEnd = end - Vector128<short>.Count;

                    Vectorized:
                        // PERF: JIT produces better code for do-while as for a while-loop (especially spills)
                        if (ptr <= vectorizedEnd)
                        {
                            do
                            {
                                Debug.Assert(pValue <= ptr && ptr <= (pValue + value.Length - Vector128<short>.Count));

                                // Load the next 8 characters + a dummy known that it must not be escaped.
                                // Put the dummy second, so it's easier for GetIndexOfFirstNeedToEscape.
                                Vector128<sbyte> sourceValue = Sse2.PackSignedSaturate(
                                    Sse2.LoadVector128(ptr),
                                    Vector128.Create((short)'A'));  // max. one "iteration", so no need to cache this vector

                                index = NeedsEscapingCore(sourceValue);

                                // If index == 0, that means none of the 16 bytes needed to be escaped.
                                // TrailingZeroCount is relatively expensive, avoid it if possible.
                                if (index != 0)
                                {
                                    goto VectorizedFound;
                                }

                                ptr += Vector128<short>.Count;
                            }
                            while (ptr <= vectorizedEnd);
                        }

                        // Process the remaining characters.
                        Debug.Assert(end - ptr < Vector128<short>.Count);

                        const int thresholdForRemainingVectorized = 4;
                        // Process the remaining elements vectorized, only if the remaining count
                        // is above thresholdForRemainingVectorized, otherwise process them sequential.
                        if (ptr < end - thresholdForRemainingVectorized)
                        {
                            ptr = vectorizedEnd;
                            goto Vectorized;
                        }

                        goto Sequential;

                    VectorizedFound:
                        idx = GetIndexOfFirstNeedToEscape(index);
                        goto EscapeFound;
                    }
#endif

                Sequential:
                    while (ptr < end)
                    {
                        Debug.Assert(pValue <= ptr && ptr < (pValue + value.Length));

                        if (NeedsEscaping(*(char*)ptr))
                        {
                            goto EscapeFound;
                        }

                        ptr++;
                    }

                    idx = -1; // All characters are allowed.
                    goto Return;

                EscapeFound:
                    // Subtraction with short* results in a idiv, so use byte* and shift
                    idx += (int)(((byte*)ptr - (byte*)pValue) >> 1);
                Return:
                    return idx;
                }
                else
                {
                    return idx = encoder.FindFirstCharacterToEncode(pValue, value.Length);
                }
            }
        }

        public static int GetMaxEscapedLength(int textLength, int firstIndexToEscape)
        {
            Debug.Assert(textLength > 0);
            Debug.Assert(firstIndexToEscape >= 0 && firstIndexToEscape < textLength);
            return firstIndexToEscape + JsonConstants.MaxExpansionFactorWhileEscaping * (textLength - firstIndexToEscape);
        }

        private static void EscapeString(ReadOnlySpan<byte> value, Span<byte> destination, JavaScriptEncoder encoder, ref int written)
        {
            Debug.Assert(encoder != null);

            OperationStatus result = encoder.EncodeUtf8(value, destination, out int encoderBytesConsumed, out int encoderBytesWritten);

            Debug.Assert(result != OperationStatus.DestinationTooSmall);
            Debug.Assert(result != OperationStatus.NeedMoreData);

            if (result != OperationStatus.Done)
            {
                ThrowHelper.ThrowArgumentException_InvalidUTF8(value.Slice(encoderBytesWritten));
            }

            Debug.Assert(encoderBytesConsumed == value.Length);

            written += encoderBytesWritten;
        }

        public static void EscapeString(ReadOnlySpan<byte> value, Span<byte> destination, int indexOfFirstByteToEscape, JavaScriptEncoder encoder, out int written)
        {
            Debug.Assert(indexOfFirstByteToEscape >= 0 && indexOfFirstByteToEscape < value.Length);

            value.Slice(0, indexOfFirstByteToEscape).CopyTo(destination);
            written = indexOfFirstByteToEscape;

            if (encoder != null)
            {
                destination = destination.Slice(indexOfFirstByteToEscape);
                value = value.Slice(indexOfFirstByteToEscape);
                EscapeString(value, destination, encoder, ref written);
            }
            else
            {
                // For performance when no encoder is specified, perform escaping here for Ascii and on the
                // first occurrence of a non-Ascii character, then call into the default encoder.
                while (indexOfFirstByteToEscape < value.Length)
                {
                    byte val = value[indexOfFirstByteToEscape];
                    if (IsAsciiValue(val))
                    {
                        if (NeedsEscaping(val))
                        {
                            EscapeNextBytes(val, destination, ref written);
                            indexOfFirstByteToEscape++;
                        }
                        else
                        {
                            destination[written] = val;
                            written++;
                            indexOfFirstByteToEscape++;
                        }
                    }
                    else
                    {
                        // Fall back to default encoder.
                        destination = destination.Slice(written);
                        value = value.Slice(indexOfFirstByteToEscape);
                        EscapeString(value, destination, JavaScriptEncoder.Default, ref written);
                        break;
                    }
                }
            }
        }

        private static void EscapeNextBytes(byte value, Span<byte> destination, ref int written)
        {
            destination[written++] = (byte)'\\';
            switch (value)
            {
                case JsonConstants.Quote:
                    // Optimize for the common quote case.
                    destination[written++] = (byte)'u';
                    destination[written++] = (byte)'0';
                    destination[written++] = (byte)'0';
                    destination[written++] = (byte)'2';
                    destination[written++] = (byte)'2';
                    break;
                case JsonConstants.LineFeed:
                    destination[written++] = (byte)'n';
                    break;
                case JsonConstants.CarriageReturn:
                    destination[written++] = (byte)'r';
                    break;
                case JsonConstants.Tab:
                    destination[written++] = (byte)'t';
                    break;
                case JsonConstants.BackSlash:
                    destination[written++] = (byte)'\\';
                    break;
                case JsonConstants.BackSpace:
                    destination[written++] = (byte)'b';
                    break;
                case JsonConstants.FormFeed:
                    destination[written++] = (byte)'f';
                    break;
                default:
                    destination[written++] = (byte)'u';

                    bool result = Utf8Formatter.TryFormat(value, destination.Slice(written), out int bytesWritten, format: s_hexStandardFormat);
                    Debug.Assert(result);
                    Debug.Assert(bytesWritten == 4);
                    written += bytesWritten;
                    break;
            }
        }

        private static bool IsAsciiValue(byte value) => value <= LastAsciiCharacter;

        private static bool IsAsciiValue(char value) => value <= LastAsciiCharacter;

        private static void EscapeString(ReadOnlySpan<char> value, Span<char> destination, JavaScriptEncoder encoder, ref int written)
        {
            Debug.Assert(encoder != null);

            OperationStatus result = encoder.Encode(value, destination, out int encoderBytesConsumed, out int encoderCharsWritten);

            Debug.Assert(result != OperationStatus.DestinationTooSmall);
            Debug.Assert(result != OperationStatus.NeedMoreData);

            if (result != OperationStatus.Done)
            {
                ThrowHelper.ThrowArgumentException_InvalidUTF16(value[encoderCharsWritten]);
            }

            Debug.Assert(encoderBytesConsumed == value.Length);

            written += encoderCharsWritten;
        }

        public static void EscapeString(ReadOnlySpan<char> value, Span<char> destination, int indexOfFirstByteToEscape, JavaScriptEncoder encoder, out int written)
        {
            Debug.Assert(indexOfFirstByteToEscape >= 0 && indexOfFirstByteToEscape < value.Length);

            value.Slice(0, indexOfFirstByteToEscape).CopyTo(destination);
            written = indexOfFirstByteToEscape;

            if (encoder != null)
            {
                destination = destination.Slice(indexOfFirstByteToEscape);
                value = value.Slice(indexOfFirstByteToEscape);
                EscapeString(value, destination, encoder, ref written);
            }
            else
            {
                // For performance when no encoder is specified, perform escaping here for Ascii and on the
                // first occurrence of a non-Ascii character, then call into the default encoder.
                while (indexOfFirstByteToEscape < value.Length)
                {
                    char val = value[indexOfFirstByteToEscape];
                    if (IsAsciiValue(val))
                    {
                        if (NeedsEscapingNoBoundsCheck(val))
                        {
                            EscapeNextChars(val, destination, ref written);
                            indexOfFirstByteToEscape++;
                        }
                        else
                        {
                            destination[written] = val;
                            written++;
                            indexOfFirstByteToEscape++;
                        }
                    }
                    else
                    {
                        // Fall back to default encoder.
                        destination = destination.Slice(written);
                        value = value.Slice(indexOfFirstByteToEscape);
                        EscapeString(value, destination, JavaScriptEncoder.Default, ref written);
                        break;
                    }
                }
            }
        }

        private static void EscapeNextChars(char value, Span<char> destination, ref int written)
        {
            Debug.Assert(IsAsciiValue(value));

            destination[written++] = '\\';
            switch ((byte)value)
            {
                case JsonConstants.Quote:
                    // Optimize for the common quote case.
                    destination[written++] = 'u';
                    destination[written++] = '0';
                    destination[written++] = '0';
                    destination[written++] = '2';
                    destination[written++] = '2';
                    break;
                case JsonConstants.LineFeed:
                    destination[written++] = 'n';
                    break;
                case JsonConstants.CarriageReturn:
                    destination[written++] = 'r';
                    break;
                case JsonConstants.Tab:
                    destination[written++] = 't';
                    break;
                case JsonConstants.BackSlash:
                    destination[written++] = '\\';
                    break;
                case JsonConstants.BackSpace:
                    destination[written++] = 'b';
                    break;
                case JsonConstants.FormFeed:
                    destination[written++] = 'f';
                    break;
                default:
                    destination[written++] = 'u';
#if BUILDING_INBOX_LIBRARY
                    int intChar = value;
                    intChar.TryFormat(destination.Slice(written), out int charsWritten, HexFormatString);
                    Debug.Assert(charsWritten == 4);
                    written += charsWritten;
#else
                    written = WriteHex(value, destination, written);
#endif
                    break;
            }
        }

#if !BUILDING_INBOX_LIBRARY
        private static int WriteHex(int value, Span<char> destination, int written)
        {
            destination[written++] = (char)Int32LsbToHexDigit(value >> 12);
            destination[written++] = (char)Int32LsbToHexDigit((int)((value >> 8) & 0xFU));
            destination[written++] = (char)Int32LsbToHexDigit((int)((value >> 4) & 0xFU));
            destination[written++] = (char)Int32LsbToHexDigit((int)(value & 0xFU));
            return written;
        }

        /// <summary>
        /// Converts a number 0 - 15 to its associated hex character '0' - 'F' as byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Int32LsbToHexDigit(int value)
        {
            Debug.Assert(value < 16);
            return (byte)((value < 10) ? ('0' + value) : ('A' + (value - 10)));
        }
#endif
    }
}
