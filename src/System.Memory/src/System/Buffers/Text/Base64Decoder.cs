// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers.Text
{
    // AVX2 version based on https://github.com/aklomp/base64/tree/e516d769a2a432c08404f1981e73b431566057be/lib/arch/avx2
    // SSSE3 version based on https://github.com/aklomp/base64/tree/e516d769a2a432c08404f1981e73b431566057be/lib/arch/ssse3

    public static partial class Base64
    {
        /// <summary>
        /// Decode the span of UTF-8 encoded text represented as base 64 into binary data.
        /// If the input is not a multiple of 4, it will decode as much as it can, to the closest multiple of 4.
        /// </summary>
        /// <param name="utf8">The input span which contains UTF-8 encoded text in base 64 that needs to be decoded.</param>
        /// <param name="bytes">The output span which contains the result of the operation, i.e. the decoded binary data.</param>
        /// <param name="bytesConsumed">The number of input bytes consumed during the operation. This can be used to slice the input for subsequent calls, if necessary.</param>
        /// <param name="bytesWritten">The number of bytes written into the output span. This can be used to slice the output for subsequent calls, if necessary.</param>
        /// <param name="isFinalBlock">True (default) when the input span contains the entire data to decode. 
        /// Set to false only if it is known that the input span contains partial data with more data to follow.</param>
        /// <returns>It returns the OperationStatus enum values:
        /// - Done - on successful processing of the entire input span
        /// - DestinationTooSmall - if there is not enough space in the output span to fit the decoded input
        /// - NeedMoreData - only if isFinalBlock is false and the input is not a multiple of 4, otherwise the partial input would be considered as InvalidData
        /// - InvalidData - if the input contains bytes outside of the expected base 64 range, or if it contains invalid/more than two padding characters,
        ///   or if the input is incomplete (i.e. not a multiple of 4) and isFinalBlock is true.
        /// </returns>
        public static OperationStatus DecodeFromUtf8(ReadOnlySpan<byte> utf8, Span<byte> bytes, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true)
        {
            // PERF: use uint to avoid the sign-extensions
            uint sourceIndex = 0;
            uint destIndex = 0;

            if (utf8.IsEmpty)
                goto DoneExit;

            ref byte srcBytes = ref MemoryMarshal.GetReference(utf8);
            ref byte destBytes = ref MemoryMarshal.GetReference(bytes);

            int srcLength = utf8.Length & ~0x3;  // only decode input up to the closest multiple of 4.
            int destLength = bytes.Length;
            int maxSrcLength = srcLength;
            int decodedLength = GetMaxDecodedFromUtf8Length(srcLength);

            // max. 2 padding chars
            if (destLength + 2 < decodedLength)
            {
                // For overflow see comment below
                maxSrcLength = destLength / 3 * 4;
            }

            if (Avx2.IsSupported && maxSrcLength >= 45)
            {
                Avx2Decode(ref srcBytes, ref destBytes, maxSrcLength, destLength, ref sourceIndex, ref destIndex);

                if (sourceIndex == srcLength)
                    goto DoneExit;
            }
            else if (Ssse3.IsSupported && maxSrcLength >= 24)
            {
                Ssse3Decode(ref srcBytes, ref destBytes, maxSrcLength, destLength, ref sourceIndex, ref destIndex);

                if (sourceIndex == srcLength)
                    goto DoneExit;
            }

            // Last bytes could have padding characters, so process them separately and treat them as valid only if isFinalBlock is true
            // if isFinalBlock is false, padding characters are considered invalid
            int skipLastChunk = isFinalBlock ? 4 : 0;

            if (destLength >= decodedLength)
            {
                maxSrcLength = srcLength - skipLastChunk;
            }
            else
            {
                // This should never overflow since destLength here is less than int.MaxValue / 4 * 3 (i.e. 1610612733)
                // Therefore, (destLength / 3) * 4 will always be less than 2147483641
                maxSrcLength = (destLength / 3) * 4;
            }

            ref sbyte decodingMap = ref s_decodingMap[0];

            // In order to elide the movsxd in the loop
            if (sourceIndex < maxSrcLength)
            {
                do
                {
                    int result = Decode(ref Unsafe.Add(ref srcBytes, (IntPtr)sourceIndex), ref decodingMap);

                    if (result < 0)
                        goto InvalidDataExit;

                    WriteThreeLowOrderBytes(ref Unsafe.Add(ref destBytes, (IntPtr)destIndex), result);
                    destIndex += 3;
                    sourceIndex += 4;
                }
                while (sourceIndex < (uint)maxSrcLength);
            }

            if (maxSrcLength != srcLength - skipLastChunk)
                goto DestinationTooSmallExit;

            // If input is less than 4 bytes, srcLength == sourceIndex == 0
            // If input is not a multiple of 4, sourceIndex == srcLength != 0
            if (sourceIndex == srcLength)
            {
                if (isFinalBlock)
                    goto InvalidDataExit;
                goto NeedMoreDataExit;
            }

            // if isFinalBlock is false, we will never reach this point

            // Handle last four bytes. There are 0, 1, 2 padding chars.
            uint t0, t1, t2, t3;
            t0 = Unsafe.Add(ref srcBytes, (IntPtr)(uint)(srcLength - 4));
            t1 = Unsafe.Add(ref srcBytes, (IntPtr)(uint)(srcLength - 3));
            t2 = Unsafe.Add(ref srcBytes, (IntPtr)(uint)(srcLength - 2));
            t3 = Unsafe.Add(ref srcBytes, (IntPtr)(uint)(srcLength - 1));

            int i0 = Unsafe.Add(ref decodingMap, (IntPtr)t0);
            int i1 = Unsafe.Add(ref decodingMap, (IntPtr)t1);

            i0 <<= 18;
            i1 <<= 12;

            i0 |= i1;

            if (t3 != EncodingPad)
            {
                int i2 = Unsafe.Add(ref decodingMap, (IntPtr)t2);
                int i3 = Unsafe.Add(ref decodingMap, (IntPtr)t3);

                i2 <<= 6;

                i0 |= i3;
                i0 |= i2;

                if (i0 < 0)
                    goto InvalidDataExit;
                if (destIndex > destLength - 3)
                    goto DestinationTooSmallExit;

                WriteThreeLowOrderBytes(ref Unsafe.Add(ref destBytes, (IntPtr)destIndex), i0);
                destIndex += 3;
            }
            else if (t2 != EncodingPad)
            {
                int i2 = Unsafe.Add(ref decodingMap, (IntPtr)t2);

                i2 <<= 6;

                i0 |= i2;

                if (i0 < 0)
                    goto InvalidDataExit;
                if (destIndex > destLength - 2)
                    goto DestinationTooSmallExit;

                Unsafe.Add(ref destBytes, (IntPtr)destIndex) = (byte)(i0 >> 16);
                Unsafe.Add(ref destBytes, (IntPtr)(destIndex + 1)) = (byte)(i0 >> 8);
                destIndex += 2;
            }
            else
            {
                if (i0 < 0)
                    goto InvalidDataExit;
                if (destIndex > destLength - 1)
                    goto DestinationTooSmallExit;

                Unsafe.Add(ref destBytes, (IntPtr)destIndex) = (byte)(i0 >> 16);
                destIndex += 1;
            }

            sourceIndex += 4;

            if (srcLength != utf8.Length)
                goto InvalidDataExit;

        DoneExit:
            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.Done;

        DestinationTooSmallExit:
            if (srcLength != utf8.Length && isFinalBlock)
                goto InvalidDataExit; // if input is not a multiple of 4, and there is no more data, return invalid data instead

            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.DestinationTooSmall;

        NeedMoreDataExit:
            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.NeedMoreData;

        InvalidDataExit:
            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.InvalidData;
        }

        /// <summary>
        /// Returns the maximum length (in bytes) of the result if you were to deocde base 64 encoded text within a byte span of size "length".
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="length"/> is less than 0.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxDecodedFromUtf8Length(int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);

            return (length >> 2) * 3;
        }

        /// <summary>
        /// Decode the span of UTF-8 encoded text in base 64 (in-place) into binary data.
        /// The decoded binary output is smaller than the text data contained in the input (the operation deflates the data).
        /// If the input is not a multiple of 4, it will not decode any.
        /// </summary>
        /// <param name="buffer">The input span which contains the base 64 text data that needs to be decoded.</param>
        /// <param name="bytesWritten">The number of bytes written into the buffer.</param>
        /// <returns>It returns the OperationStatus enum values:
        /// - Done - on successful processing of the entire input span
        /// - InvalidData - if the input contains bytes outside of the expected base 64 range, or if it contains invalid/more than two padding characters, 
        ///   or if the input is incomplete (i.e. not a multiple of 4).
        /// It does not return DestinationTooSmall since that is not possible for base 64 decoding.
        /// It does not return NeedMoreData since this method tramples the data in the buffer and 
        /// hence can only be called once with all the data in the buffer.
        /// </returns>
        public static OperationStatus DecodeFromUtf8InPlace(Span<byte> buffer, out int bytesWritten)
        {
            int bufferLength = buffer.Length;
            uint sourceIndex = 0;
            uint destIndex = 0;

            // only decode input if it is a multiple of 4
            if (bufferLength != ((bufferLength >> 2) * 4))
                goto InvalidExit;
            if (bufferLength == 0)
                goto DoneExit;

            ref byte bufferBytes = ref MemoryMarshal.GetReference(buffer);

            ref sbyte decodingMap = ref s_decodingMap[0];

            while (sourceIndex < bufferLength - 4)
            {
                int result = Decode(ref Unsafe.Add(ref bufferBytes, (IntPtr)sourceIndex), ref decodingMap);
                if (result < 0)
                    goto InvalidExit;
                WriteThreeLowOrderBytes(ref Unsafe.Add(ref bufferBytes, (IntPtr)destIndex), result);
                destIndex += 3;
                sourceIndex += 4;
            }

            uint t0, t1, t2, t3;
            uint n = (uint)(bufferLength - 4);
            t0 = Unsafe.Add(ref bufferBytes, (IntPtr)n);
            t1 = Unsafe.Add(ref bufferBytes, (IntPtr)(n + 1));
            t2 = Unsafe.Add(ref bufferBytes, (IntPtr)(n + 2));
            t3 = Unsafe.Add(ref bufferBytes, (IntPtr)(n + 3));

            int i0 = Unsafe.Add(ref decodingMap, (IntPtr)t0);
            int i1 = Unsafe.Add(ref decodingMap, (IntPtr)t1);

            i0 <<= 18;
            i1 <<= 12;

            i0 |= i1;

            if (t3 != EncodingPad)
            {
                int i2 = Unsafe.Add(ref decodingMap, (IntPtr)t2);
                int i3 = Unsafe.Add(ref decodingMap, (IntPtr)t3);

                i2 <<= 6;

                i0 |= i3;
                i0 |= i2;

                if (i0 < 0)
                    goto InvalidExit;

                WriteThreeLowOrderBytes(ref Unsafe.Add(ref bufferBytes, (IntPtr)destIndex), i0);
                destIndex += 3;
            }
            else if (t2 != EncodingPad)
            {
                int i2 = Unsafe.Add(ref decodingMap, (IntPtr)t2);

                i2 <<= 6;

                i0 |= i2;

                if (i0 < 0)
                    goto InvalidExit;

                Unsafe.Add(ref bufferBytes, (IntPtr)destIndex) = (byte)(i0 >> 16);
                Unsafe.Add(ref bufferBytes, (IntPtr)(destIndex + 1)) = (byte)(i0 >> 8);
                destIndex += 2;
            }
            else
            {
                if (i0 < 0)
                    goto InvalidExit;

                Unsafe.Add(ref bufferBytes, (IntPtr)destIndex) = (byte)(i0 >> 16);
                destIndex += 1;
            }

        DoneExit:
            bytesWritten = (int)destIndex;
            return OperationStatus.Done;

        InvalidExit:
            bytesWritten = (int)destIndex;
            return OperationStatus.InvalidData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Avx2Decode(ref byte src, ref byte destBytes, int sourceLength, int destLength, ref uint sourceIndex, ref uint destIndex)
        {
            ref byte srcStart = ref src;
            ref byte destStart = ref destBytes;
            ref byte simdSrcEnd = ref Unsafe.Add(ref src, (IntPtr)((uint)sourceLength - 45 + 1));

            // The JIT won't hoist these "constants", so help him
            Vector256<sbyte> lutHi = s_avxDecodeLutHi;
            Vector256<sbyte> lutLo = s_avxDecodeLutLo;
            Vector256<sbyte> lutShift = s_avxDecodeLutShift;
            Vector256<sbyte> mask2F = s_avxDecodeMask2F;
            Vector256<sbyte> shuffleConstant0 = Vector256.Create(0x01400140).AsSByte();
            Vector256<short> shuffleConstant1 = Vector256.Create(0x00011000).AsInt16();
            Vector256<sbyte> shuffleVec = s_avxDecodeShuffleVec;
            Vector256<int> permuteVec = s_avxDecodePermuteVec;

            //while (remaining >= 45)
            do
            {
                AssertRead<Vector256<sbyte>>(ref src, ref srcStart, sourceLength);
                Vector256<sbyte> str = Unsafe.As<byte, Vector256<sbyte>>(ref src);

                Vector256<sbyte> hiNibbles = Avx2.And(Avx2.ShiftRightLogical(str.AsInt32(), 4).AsSByte(), mask2F);
                Vector256<sbyte> loNibbles = Avx2.And(str, mask2F);
                Vector256<sbyte> hi = Avx2.Shuffle(lutHi, hiNibbles);
                Vector256<sbyte> lo = Avx2.Shuffle(lutLo, loNibbles);
                Vector256<sbyte> zero = Vector256<sbyte>.Zero;

                // https://github.com/dotnet/coreclr/issues/21247
                if (Avx2.MoveMask(Avx2.CompareGreaterThan(Avx2.And(lo, hi), zero)) != 0)
                    break;

                Vector256<sbyte> eq2F = Avx2.CompareEqual(str, mask2F);
                Vector256<sbyte> shift = Avx2.Shuffle(lutShift, Avx2.Add(eq2F, hiNibbles));
                str = Avx2.Add(str, shift);

                Vector256<short> merge_ab_and_bc = Avx2.MultiplyAddAdjacent(str.AsByte(), shuffleConstant0);
                Vector256<int> @out = Avx2.MultiplyAddAdjacent(merge_ab_and_bc, shuffleConstant1);
                @out = Avx2.Shuffle(@out.AsSByte(), shuffleVec).AsInt32();
                str = Avx2.PermuteVar8x32(@out, permuteVec).AsSByte();

                AssertWrite<Vector256<sbyte>>(ref destBytes, ref destStart, destLength);
                Unsafe.As<byte, Vector256<sbyte>>(ref destBytes) = str;

                src = ref Unsafe.Add(ref src, 32);
                destBytes = ref Unsafe.Add(ref destBytes, 24);
            }
            while (Unsafe.IsAddressLessThan(ref src, ref simdSrcEnd));

            // Cast to ulong to avoid the overflow-check. Codegen for x86 is still good.
            sourceIndex = (uint)(ulong)Unsafe.ByteOffset(ref srcStart, ref src);
            destIndex = (uint)(ulong)Unsafe.ByteOffset(ref destStart, ref destBytes);

            src = ref srcStart;
            destBytes = ref destStart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Ssse3Decode(ref byte src, ref byte destBytes, int sourceLength, int destLength, ref uint sourceIndex, ref uint destIndex)
        {
            ref byte srcStart = ref src;
            ref byte destStart = ref destBytes;
            ref byte simdSrcEnd = ref Unsafe.Add(ref src, (IntPtr)((uint)sourceLength - 24 + 1));

            // The JIT won't hoist these "constants", so help him
            Vector128<sbyte> lutHi = s_sseDecodeLutHi;
            Vector128<sbyte> lutLo = s_sseDecodeLutLo;
            Vector128<sbyte> lutShift = s_sseDecodeLutShift;
            Vector128<sbyte> mask2F = s_sseDecodeMask2F;
            Vector128<sbyte> shuffleConstant0 = Vector128.Create(0x01400140).AsSByte();
            Vector128<short> shuffleConstant1 = Vector128.Create(0x00011000).AsInt16();
            Vector128<sbyte> shuffleVec = s_sseDecodeShuffleVec;

            //while (remaining >= 24)
            do
            {
                AssertRead<Vector128<sbyte>>(ref src, ref srcStart, sourceLength);
                Vector128<sbyte> str = Unsafe.As<byte, Vector128<sbyte>>(ref src);

                Vector128<sbyte> hiNibbles = Sse2.And(Sse2.ShiftRightLogical(str.AsInt32(), 4).AsSByte(), mask2F);
                Vector128<sbyte> loNibbles = Sse2.And(str, mask2F);
                Vector128<sbyte> hi = Ssse3.Shuffle(lutHi, hiNibbles);
                Vector128<sbyte> lo = Ssse3.Shuffle(lutLo, loNibbles);
                Vector128<sbyte> zero = Vector128<sbyte>.Zero;

                if (Sse2.MoveMask(Sse2.CompareGreaterThan(Sse2.And(lo, hi), zero)) != 0)
                    break;

                Vector128<sbyte> eq2F = Sse2.CompareEqual(str, mask2F);
                Vector128<sbyte> shift = Ssse3.Shuffle(lutShift, Sse2.Add(eq2F, hiNibbles));
                str = Sse2.Add(str, shift);

                Vector128<short> merge_ab_and_bc = Ssse3.MultiplyAddAdjacent(str.AsByte(), shuffleConstant0);
                Vector128<int> @out = Sse2.MultiplyAddAdjacent(merge_ab_and_bc, shuffleConstant1);
                str = Ssse3.Shuffle(@out.AsSByte(), shuffleVec);

                AssertWrite<Vector128<sbyte>>(ref destBytes, ref destStart, destLength);
                Unsafe.As<byte, Vector128<sbyte>>(ref destBytes) = str;

                src = ref Unsafe.Add(ref src, 16);
                destBytes = ref Unsafe.Add(ref destBytes, 12);
            }
            while (Unsafe.IsAddressLessThan(ref src, ref simdSrcEnd));

            // Cast to ulong to avoid the overflow-check. Codegen for x86 is still good.
            sourceIndex = (uint)(ulong)Unsafe.ByteOffset(ref srcStart, ref src);
            destIndex = (uint)(ulong)Unsafe.ByteOffset(ref destStart, ref destBytes);

            src = ref srcStart;
            destBytes = ref destStart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Decode(ref byte encodedBytes, ref sbyte decodingMap)
        {
            uint t0, t1, t2, t3;

            t0 = encodedBytes;
            t1 = Unsafe.Add(ref encodedBytes, 1);
            t2 = Unsafe.Add(ref encodedBytes, 2);
            t3 = Unsafe.Add(ref encodedBytes, 3);

            int i0 = Unsafe.Add(ref decodingMap, (IntPtr)t0);
            int i1 = Unsafe.Add(ref decodingMap, (IntPtr)t1);
            int i2 = Unsafe.Add(ref decodingMap, (IntPtr)t2);
            int i3 = Unsafe.Add(ref decodingMap, (IntPtr)t3);

            i0 <<= 18;
            i1 <<= 12;
            i2 <<= 6;

            i0 |= i3;
            i1 |= i2;

            i0 |= i1;
            return i0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteThreeLowOrderBytes(ref byte destination, int value)
        {
            destination = (byte)(value >> 16);
            Unsafe.Add(ref destination, 1) = (byte)(value >> 8);
            Unsafe.Add(ref destination, 2) = (byte)value;
        }

        // Pre-computing this table using a custom string(s_characters) and GenerateDecodingMapAndVerify (found in tests)
        private static readonly sbyte[] s_decodingMap = {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,         //62 is placed at index 43 (for +), 63 at index 47 (for /)
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,         //52-61 are placed at index 48-57 (for 0-9), 64 at index 61 (for =)
            -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,         //0-25 are placed at index 65-90 (for A-Z)
            -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1,         //26-51 are placed at index 97-122 (for a-z)
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,         // Bytes over 122 ('z') are invalid and cannot be decoded
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,         // Hence, padding the map with 255, which indicates invalid input
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        };

        private static readonly Vector128<sbyte> s_sseDecodeShuffleVec;
        private static readonly Vector128<sbyte> s_sseDecodeLutLo;
        private static readonly Vector128<sbyte> s_sseDecodeLutHi;
        private static readonly Vector128<sbyte> s_sseDecodeLutShift;
        private static readonly Vector128<sbyte> s_sseDecodeMask2F;

        private static readonly Vector256<sbyte> s_avxDecodeShuffleVec;
        private static readonly Vector256<int> s_avxDecodePermuteVec;
        private static readonly Vector256<sbyte> s_avxDecodeLutLo;
        private static readonly Vector256<sbyte> s_avxDecodeLutHi;
        private static readonly Vector256<sbyte> s_avxDecodeLutShift;
        private static readonly Vector256<sbyte> s_avxDecodeMask2F;
    }
}
