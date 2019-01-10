// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    internal static partial class JsonWriterHelper
    {
        public static readonly byte[] s_newLineUtf8 = Encoding.UTF8.GetBytes(Environment.NewLine);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteIndentation(Span<byte> buffer, int indent, out int bytesWritten)
        {
            Debug.Assert(indent % 2 == 0);

            if (buffer.Length >= indent)
            {
                if (indent < 8)
                {
                    int i = 0;
                    while (i < indent)
                    {
                        buffer[i++] = JsonConstants.Space;
                        buffer[i++] = JsonConstants.Space;
                    }
                }
                else
                {
                    buffer.Slice(0, indent).Fill(JsonConstants.Space);
                }
                bytesWritten = indent;
                return true;
            }
            else
            {
                int i = 0;
                while (i < buffer.Length - 1)
                {
                    buffer[i++] = JsonConstants.Space;
                    buffer[i++] = JsonConstants.Space;
                }
                bytesWritten = i;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateProperty(ref ReadOnlySpan<byte> propertyName)
        {
            if (propertyName.Length > JsonConstants.MaxTokenSize)
                ThrowHelper.ThrowArgumentException_PropertyNameTooLarge(propertyName.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateValue(ref ReadOnlySpan<byte> value)
        {
            if (value.Length > JsonConstants.MaxTokenSize)
                ThrowHelper.ThrowArgumentException_ValueTooLarge(value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateDouble(double value)
        {
            if (!double.IsFinite(value))
                ThrowHelper.ThrowArgumentException_ValueNotSupported();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateSingle(float value)
        {
            if (!float.IsFinite(value))
                ThrowHelper.ThrowArgumentException_ValueNotSupported();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateProperty(ref ReadOnlySpan<char> propertyName)
        {
            if (propertyName.Length > JsonConstants.MaxCharacterTokenSize)
                ThrowHelper.ThrowArgumentException_PropertyNameTooLarge(propertyName.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateValue(ref ReadOnlySpan<char> value)
        {
            if (value.Length > JsonConstants.MaxCharacterTokenSize)
                ThrowHelper.ThrowArgumentException_ValueTooLarge(value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidatePropertyAndValue(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<byte> value)
        {
            if (propertyName.Length > JsonConstants.MaxCharacterTokenSize || value.Length > JsonConstants.MaxTokenSize)
                ThrowHelper.ThrowArgumentException(propertyName, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidatePropertyAndValue(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<char> value)
        {
            if (propertyName.Length > JsonConstants.MaxTokenSize || value.Length > JsonConstants.MaxCharacterTokenSize)
                ThrowHelper.ThrowArgumentException(propertyName, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidatePropertyAndValue(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<byte> value)
        {
            if (propertyName.Length > JsonConstants.MaxTokenSize || value.Length > JsonConstants.MaxTokenSize)
                ThrowHelper.ThrowArgumentException(propertyName, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidatePropertyAndValue(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<char> value)
        {
            if (propertyName.Length > JsonConstants.MaxCharacterTokenSize || value.Length > JsonConstants.MaxCharacterTokenSize)
                ThrowHelper.ThrowArgumentException(propertyName, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountDigits(ulong value)
        {
            int digits = 1;
            uint part;
            if (value >= 10000000)
            {
                if (value >= 100000000000000)
                {
                    part = (uint)(value / 100000000000000);
                    digits += 14;
                }
                else
                {
                    part = (uint)(value / 10000000);
                    digits += 7;
                }
            }
            else
            {
                part = (uint)value;
            }

            if (part < 10)
            {
                // no-op
            }
            else if (part < 100)
            {
                digits += 1;
            }
            else if (part < 1000)
            {
                digits += 2;
            }
            else if (part < 10000)
            {
                digits += 3;
            }
            else if (part < 100000)
            {
                digits += 4;
            }
            else if (part < 1000000)
            {
                digits += 5;
            }
            else
            {
                Debug.Assert(part < 10000000);
                digits += 6;
            }

            return digits;
        }

        // TODO: Replace this with publicly shipping implementation: https://github.com/dotnet/corefx/issues/34094

        /// <summary>
        /// Converts a span containing a sequence of UTF-16 bytes into UTF-8 bytes.
        ///
        /// This method will consume as many of the input bytes as possible.
        ///
        /// On successful exit, the entire input was consumed and encoded successfully. In this case, <paramref name="bytesConsumed"/> will be
        /// equal to the length of the <paramref name="utf16Source"/> and <paramref name="bytesWritten"/> will equal the total number of bytes written to
        /// the <paramref name="utf8Destination"/>.
        /// </summary>
        /// <param name="utf16Source">A span containing a sequence of UTF-16 bytes.</param>
        /// <param name="utf8Destination">A span to write the UTF-8 bytes into.</param>
        /// <param name="bytesConsumed">On exit, contains the number of bytes that were consumed from the <paramref name="utf16Source"/>.</param>
        /// <param name="bytesWritten">On exit, contains the number of bytes written to <paramref name="utf8Destination"/></param>
        /// <returns>A <see cref="OperationStatus"/> value representing the state of the conversion.</returns>
        public unsafe static OperationStatus ToUtf8(ReadOnlySpan<byte> utf16Source, Span<byte> utf8Destination, out int bytesConsumed, out int bytesWritten)
        {
            //
            //
            // KEEP THIS IMPLEMENTATION IN SYNC WITH https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Text/UTF8Encoding.cs#L841
            //
            //
            fixed (byte* chars = &MemoryMarshal.GetReference(utf16Source))
            fixed (byte* bytes = &MemoryMarshal.GetReference(utf8Destination))
            {
                char* pSrc = (char*)chars;
                byte* pTarget = bytes;

                char* pEnd = (char*)(chars + utf16Source.Length);
                byte* pAllocatedBufferEnd = pTarget + utf8Destination.Length;

                // assume that JIT will enregister pSrc, pTarget and ch

                // Entering the fast encoding loop incurs some overhead that does not get amortized for small
                // number of characters, and the slow encoding loop typically ends up running for the last few
                // characters anyway since the fast encoding loop needs 5 characters on input at least.
                // Thus don't use the fast decoding loop at all if we don't have enough characters. The threashold
                // was choosen based on performance testing.
                // Note that if we don't have enough bytes, pStop will prevent us from entering the fast loop.
                while (pEnd - pSrc > 13)
                {
                    // we need at least 1 byte per character, but Convert might allow us to convert
                    // only part of the input, so try as much as we can.  Reduce charCount if necessary
                    int available = Math.Min(PtrDiff(pEnd, pSrc), PtrDiff(pAllocatedBufferEnd, pTarget));

                    // FASTLOOP:
                    // - optimistic range checks
                    // - fallbacks to the slow loop for all special cases, exception throwing, etc.

                    // To compute the upper bound, assume that all characters are ASCII characters at this point,
                    //  the boundary will be decreased for every non-ASCII character we encounter
                    // Also, we need 5 chars reserve for the unrolled ansi decoding loop and for decoding of surrogates
                    // If there aren't enough bytes for the output, then pStop will be <= pSrc and will bypass the loop.
                    char* pStop = pSrc + available - 5;
                    if (pSrc >= pStop)
                        break;

                    do
                    {
                        int ch = *pSrc;
                        pSrc++;

                        if (ch > 0x7F)
                        {
                            goto LongCode;
                        }
                        *pTarget = (byte)ch;
                        pTarget++;

                        // get pSrc aligned
                        if ((unchecked((int)pSrc) & 0x2) != 0)
                        {
                            ch = *pSrc;
                            pSrc++;
                            if (ch > 0x7F)
                            {
                                goto LongCode;
                            }
                            *pTarget = (byte)ch;
                            pTarget++;
                        }

                        // Run 4 characters at a time!
                        while (pSrc < pStop)
                        {
                            ch = *(int*)pSrc;
                            int chc = *(int*)(pSrc + 2);
                            if (((ch | chc) & unchecked((int)0xFF80FF80)) != 0)
                            {
                                goto LongCodeWithMask;
                            }

                            // Unfortunately, this is endianess sensitive
#if BIGENDIAN
                            *pTarget = (byte)(ch >> 16);
                            *(pTarget + 1) = (byte)ch;
                            pSrc += 4;
                            *(pTarget + 2) = (byte)(chc >> 16);
                            *(pTarget + 3) = (byte)chc;
                            pTarget += 4;
#else // BIGENDIAN
                            *pTarget = (byte)ch;
                            *(pTarget + 1) = (byte)(ch >> 16);
                            pSrc += 4;
                            *(pTarget + 2) = (byte)chc;
                            *(pTarget + 3) = (byte)(chc >> 16);
                            pTarget += 4;
#endif // BIGENDIAN
                        }
                        continue;

                    LongCodeWithMask:
#if BIGENDIAN
                        // be careful about the sign extension
                        ch = (int)(((uint)ch) >> 16);
#else // BIGENDIAN
                        ch = (char)ch;
#endif // BIGENDIAN
                        pSrc++;

                        if (ch > 0x7F)
                        {
                            goto LongCode;
                        }
                        *pTarget = (byte)ch;
                        pTarget++;
                        continue;

                    LongCode:
                        // use separate helper variables for slow and fast loop so that the jit optimizations
                        // won't get confused about the variable lifetimes
                        int chd;
                        if (ch <= 0x7FF)
                        {
                            // 2 byte encoding
                            chd = unchecked((sbyte)0xC0) | (ch >> 6);
                        }
                        else
                        {
                            // if (!IsLowSurrogate(ch) && !IsHighSurrogate(ch))
                            if (!InRange(ch, JsonConstants.HighSurrogateStart, JsonConstants.LowSurrogateEnd))
                            {
                                // 3 byte encoding
                                chd = unchecked((sbyte)0xE0) | (ch >> 12);
                            }
                            else
                            {
                                // 4 byte encoding - high surrogate + low surrogate
                                // if (!IsHighSurrogate(ch))
                                if (ch > JsonConstants.HighSurrogateEnd)
                                {
                                    // low without high -> bad
                                    goto InvalidData;
                                }

                                chd = *pSrc;

                                // if (!IsLowSurrogate(chd)) {
                                if (!InRange(chd, JsonConstants.LowSurrogateStart, JsonConstants.LowSurrogateEnd))
                                {
                                    // high not followed by low -> bad
                                    goto InvalidData;
                                }

                                pSrc++;

                                ch = chd + (ch << 10) +
                                    (0x10000
                                    - JsonConstants.LowSurrogateStart
                                    - (JsonConstants.HighSurrogateStart << 10));

                                *pTarget = (byte)(unchecked((sbyte)0xF0) | (ch >> 18));
                                // pStop - this byte is compensated by the second surrogate character
                                // 2 input chars require 4 output bytes.  2 have been anticipated already
                                // and 2 more will be accounted for by the 2 pStop-- calls below.
                                pTarget++;

                                chd = unchecked((sbyte)0x80) | (ch >> 12) & 0x3F;
                            }
                            *pTarget = (byte)chd;
                            pStop--;                    // 3 byte sequence for 1 char, so need pStop-- and the one below too.
                            pTarget++;

                            chd = unchecked((sbyte)0x80) | (ch >> 6) & 0x3F;
                        }
                        *pTarget = (byte)chd;
                        pStop--;                        // 2 byte sequence for 1 char so need pStop--.

                        *(pTarget + 1) = (byte)(unchecked((sbyte)0x80) | ch & 0x3F);
                        // pStop - this byte is already included

                        pTarget += 2;
                    }
                    while (pSrc < pStop);

                    Debug.Assert(pTarget <= pAllocatedBufferEnd, "[UTF8Encoding.GetBytes]pTarget <= pAllocatedBufferEnd");
                }

                while (pSrc < pEnd)
                {
                    // SLOWLOOP: does all range checks, handles all special cases, but it is slow

                    // read next char. The JIT optimization seems to be getting confused when
                    // compiling "ch = *pSrc++;", so rather use "ch = *pSrc; pSrc++;" instead
                    int ch = *pSrc;
                    pSrc++;

                    if (ch <= 0x7F)
                    {
                        if (pAllocatedBufferEnd - pTarget <= 0)
                            goto DestinationFull;

                        *pTarget = (byte)ch;
                        pTarget++;
                        continue;
                    }

                    int chd;
                    if (ch <= 0x7FF)
                    {
                        if (pAllocatedBufferEnd - pTarget <= 1)
                            goto DestinationFull;

                        // 2 byte encoding
                        chd = unchecked((sbyte)0xC0) | (ch >> 6);
                    }
                    else
                    {
                        // if (!IsLowSurrogate(ch) && !IsHighSurrogate(ch))
                        if (!InRange(ch, JsonConstants.HighSurrogateStart, JsonConstants.LowSurrogateEnd))
                        {
                            if (pAllocatedBufferEnd - pTarget <= 2)
                                goto DestinationFull;

                            // 3 byte encoding
                            chd = unchecked((sbyte)0xE0) | (ch >> 12);
                        }
                        else
                        {
                            if (pAllocatedBufferEnd - pTarget <= 3)
                                goto DestinationFull;

                            // 4 byte encoding - high surrogate + low surrogate
                            // if (!IsHighSurrogate(ch))
                            if (ch > JsonConstants.HighSurrogateEnd)
                            {
                                // low without high -> bad
                                goto InvalidData;
                            }

                            if (pSrc >= pEnd)
                                goto NeedMoreData;

                            chd = *pSrc;

                            // if (!IsLowSurrogate(chd)) {
                            if (!InRange(chd, JsonConstants.LowSurrogateStart, JsonConstants.LowSurrogateEnd))
                            {
                                // high not followed by low -> bad
                                goto InvalidData;
                            }

                            pSrc++;

                            ch = chd + (ch << 10) +
                                (0x10000
                                - JsonConstants.LowSurrogateStart
                                - (JsonConstants.HighSurrogateStart << 10));

                            *pTarget = (byte)(unchecked((sbyte)0xF0) | (ch >> 18));
                            pTarget++;

                            chd = unchecked((sbyte)0x80) | (ch >> 12) & 0x3F;
                        }
                        *pTarget = (byte)chd;
                        pTarget++;

                        chd = unchecked((sbyte)0x80) | (ch >> 6) & 0x3F;
                    }

                    *pTarget = (byte)chd;
                    *(pTarget + 1) = (byte)(unchecked((sbyte)0x80) | ch & 0x3F);

                    pTarget += 2;
                }

                bytesConsumed = (int)((byte*)pSrc - chars);
                bytesWritten = (int)(pTarget - bytes);
                return OperationStatus.Done;

            InvalidData:
                bytesConsumed = (int)((byte*)(pSrc - 1) - chars);
                bytesWritten = (int)(pTarget - bytes);
                return OperationStatus.InvalidData;

            DestinationFull:
                bytesConsumed = (int)((byte*)(pSrc - 1) - chars);
                bytesWritten = (int)(pTarget - bytes);
                return OperationStatus.DestinationTooSmall;

            NeedMoreData:
                bytesConsumed = (int)((byte*)(pSrc - 1) - chars);
                bytesWritten = (int)(pTarget - bytes);
                return OperationStatus.NeedMoreData;
            }
        }

        // Borrowed from https://github.com/dotnet/corefx/blob/master/src/System.Memory/src/System/Buffers/Text/Utf8Formatter/Utf8Formatter.Integer.Signed.Default.cs#L16
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFormatInt64Default(long value, Span<byte> destination, out int bytesWritten)
        {
            if ((ulong)value < 10)
            {
                return TryFormatUInt32SingleDigit((uint)value, destination, out bytesWritten);
            }

            return TryFormatInt64MultipleDigits(value, destination, out bytesWritten);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFormatUInt64Default(ulong value, Span<byte> destination, out int bytesWritten)
        {
            if (value < 10)
            {
                return TryFormatUInt32SingleDigit((uint)value, destination, out bytesWritten);
            }

            return TryFormatUInt64MultipleDigits(value, destination, out bytesWritten);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt32SingleDigit(uint value, Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length == 0)
            {
                bytesWritten = 0;
                return false;
            }
            destination[0] = (byte)('0' + value);
            bytesWritten = 1;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt64MultipleDigits(long value, Span<byte> destination, out int bytesWritten)
        {
            if (value < 0)
            {
                value = -value;
                int digitCount = CountDigits((ulong)value);
                // WriteDigits does not do bounds checks
                if (digitCount >= destination.Length)
                {
                    bytesWritten = 0;
                    return false;
                }
                destination[0] = (byte)'-';
                bytesWritten = digitCount + 1;
                WriteDigits((ulong)value, destination.Slice(1, digitCount));
                return true;
            }
            else
            {
                return TryFormatUInt64MultipleDigits((ulong)value, destination, out bytesWritten);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt64MultipleDigits(ulong value, Span<byte> destination, out int bytesWritten)
        {
            int digitCount = CountDigits(value);
            // WriteDigits does not do bounds checks
            if (digitCount > destination.Length)
            {
                bytesWritten = 0;
                return false;
            }
            bytesWritten = digitCount;
            WriteDigits(value, destination.Slice(0, digitCount));
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDigits(ulong value, Span<byte> buffer)
        {
            // We can mutate the 'value' parameter since it's a copy-by-value local.
            // It'll be used to represent the value left over after each division by 10.

            for (int i = buffer.Length - 1; i >= 1; i--)
            {
                ulong temp = '0' + value;
                value /= 10;
                buffer[i] = (byte)(temp - (value * 10));
            }

            Debug.Assert(value < 10);
            buffer[0] = (byte)('0' + value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static int PtrDiff(char* a, char* b)
        {
            return (int)(((uint)((byte*)a - (byte*)b)) >> 1);
        }

        // byte* flavor just for parity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static int PtrDiff(byte* a, byte* b)
        {
            return (int)(a - b);
        }
    }
}
