// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Internal.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    // AVX2 version based on https://github.com/aklomp/base64/tree/e516d769a2a432c08404f1981e73b431566057be/lib/arch/avx2
    // SSSE3 version based on https://github.com/aklomp/base64/tree/e516d769a2a432c08404f1981e73b431566057be/lib/arch/ssse3

    /// <summary>
    /// Convert between binary data and UTF-8 encoded text that is represented in base 64.
    /// </summary>
    public static partial class Base64
    {
        /// <summary>
        /// Encode the span of binary data into UTF-8 encoded text represented as base 64.
        /// </summary> 
        /// <param name="bytes">The input span which contains binary data that needs to be encoded.</param>
        /// <param name="utf8">The output span which contains the result of the operation, i.e. the UTF-8 encoded text in base 64.</param>
        /// <param name="bytesConsumed">The number of input bytes consumed during the operation. This can be used to slice the input for subsequent calls, if necessary.</param>
        /// <param name="bytesWritten">The number of bytes written into the output span. This can be used to slice the output for subsequent calls, if necessary.</param>
        /// <param name="isFinalBlock">True (default) when the input span contains the entire data to encode. 
        /// Set to false only if it is known that the input span contains partial data with more data to follow.</param>
        /// <returns>It returns the OperationStatus enum values:
        /// - Done - on successful processing of the entire input span
        /// - DestinationTooSmall - if there is not enough space in the output span to fit the encoded input
        /// - NeedMoreData - only if isFinalBlock is false, otherwise the output is padded if the input is not a multiple of 3
        /// It does not return InvalidData since that is not possible for base 64 encoding.
        /// </returns>
        public static OperationStatus EncodeToUtf8(ReadOnlySpan<byte> bytes, Span<byte> utf8, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true)
        {
            // PERF: use uint to avoid the sign-extensions
            uint sourceIndex = 0;
            uint destIndex = 0;

            if (bytes.IsEmpty)
                goto DoneExit;

            ref byte srcBytes = ref MemoryMarshal.GetReference(bytes);
            ref byte destBytes = ref MemoryMarshal.GetReference(utf8);

            int srcLength = bytes.Length;
            int destLength = utf8.Length;
            int maxSrcLength = srcLength;

            if (srcLength <= MaximumEncodeLength && destLength >= GetMaxEncodedToUtf8Length(srcLength))
            {
                maxSrcLength = srcLength;
            }
            else
            {
                maxSrcLength = (destLength >> 2) * 3;
            }

            if (srcLength < 16)
                goto Scalar;

            if (Avx2.IsSupported && maxSrcLength >= 32)
            {
                Avx2Encode(ref srcBytes, ref destBytes, maxSrcLength, destLength, ref sourceIndex, ref destIndex);

                if (sourceIndex == srcLength)
                    goto DoneExit;
            }

            if (Ssse3.IsSupported && (maxSrcLength >= (int)sourceIndex + 16))
            {
                Ssse3Encode(ref srcBytes, ref destBytes, maxSrcLength, destLength, ref sourceIndex, ref destIndex);

                if (sourceIndex == srcLength)
                    goto DoneExit;
            }

        Scalar:
            maxSrcLength -= 2;
            uint result = 0;

            ref byte encodingMap = ref s_encodingMap[0];

            // In order to elide the movsxd in the loop
            if (sourceIndex < maxSrcLength)
            {
                do
                {
                    result = Encode(ref Unsafe.Add(ref srcBytes, (IntPtr)sourceIndex), ref encodingMap);
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, (IntPtr)destIndex), result);
                    destIndex += 4;
                    sourceIndex += 3;
                }
                while (sourceIndex < (uint)maxSrcLength);
            }

            if (maxSrcLength != srcLength - 2)
                goto DestinationTooSmallExit;

            if (!isFinalBlock)
                goto NeedMoreDataExit;

            if (sourceIndex == srcLength - 1)
            {
                result = EncodeAndPadTwo(ref Unsafe.Add(ref srcBytes, (IntPtr)sourceIndex), ref encodingMap);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, (IntPtr)destIndex), result);
                destIndex += 4;
                sourceIndex += 1;
            }
            else if (sourceIndex == srcLength - 2)
            {
                result = EncodeAndPadOne(ref Unsafe.Add(ref srcBytes, (IntPtr)sourceIndex), ref encodingMap);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, (IntPtr)destIndex), result);
                destIndex += 4;
                sourceIndex += 2;
            }

        DoneExit:
            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.Done;

        NeedMoreDataExit:
            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.NeedMoreData;

        DestinationTooSmallExit:
            bytesConsumed = (int)sourceIndex;
            bytesWritten = (int)destIndex;
            return OperationStatus.DestinationTooSmall;
        }

        /// <summary>
        /// Returns the maximum length (in bytes) of the result if you were to encode binary data within a byte span of size "length".
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="length"/> is less than 0 or larger than 1610612733 (since encode inflates the data by 4/3).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxEncodedToUtf8Length(int length)
        {
            if ((uint)length > MaximumEncodeLength)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);

            return ((length + 2) / 3) * 4;
        }

        /// <summary>
        /// Encode the span of binary data (in-place) into UTF-8 encoded text represented as base 64. 
        /// The encoded text output is larger than the binary data contained in the input (the operation inflates the data).
        /// </summary>
        /// <param name="buffer">The input span which contains binary data that needs to be encoded. 
        /// It needs to be large enough to fit the result of the operation.</param>
        /// <param name="dataLength">The amount of binary data contained within the buffer that needs to be encoded 
        /// (and needs to be smaller than the buffer length).</param>
        /// <param name="bytesWritten">The number of bytes written into the buffer.</param>
        /// <returns>It returns the OperationStatus enum values:
        /// - Done - on successful processing of the entire buffer
        /// - DestinationTooSmall - if there is not enough space in the buffer beyond dataLength to fit the result of encoding the input
        /// It does not return NeedMoreData since this method tramples the data in the buffer and hence can only be called once with all the data in the buffer.
        /// It does not return InvalidData since that is not possible for base 64 encoding.
        /// </returns>
        public static OperationStatus EncodeToUtf8InPlace(Span<byte> buffer, int dataLength, out int bytesWritten)
        {
            int encodedLength = GetMaxEncodedToUtf8Length(dataLength);
            if (buffer.Length < encodedLength)
                goto FalseExit;

            int leftover = dataLength - (dataLength / 3) * 3; // how many bytes after packs of 3

            // PERF: use uint to avoid the sign-extensions
            uint destinationIndex = (uint)(encodedLength - 4);
            uint sourceIndex = (uint)(dataLength - leftover);
            uint result = 0;

            ref byte encodingMap = ref s_encodingMap[0];
            ref byte bufferBytes = ref MemoryMarshal.GetReference(buffer);

            // encode last pack to avoid conditional in the main loop
            if (leftover != 0)
            {
                if (leftover == 1)
                {
                    result = EncodeAndPadTwo(ref Unsafe.Add(ref bufferBytes, (IntPtr)sourceIndex), ref encodingMap);
                }
                else
                {
                    result = EncodeAndPadOne(ref Unsafe.Add(ref bufferBytes, (IntPtr)sourceIndex), ref encodingMap);
                }

                Unsafe.WriteUnaligned(ref Unsafe.Add(ref bufferBytes, (IntPtr)destinationIndex), result);
                destinationIndex -= 4;
            }

            sourceIndex -= 3;
            while ((int)sourceIndex >= 0)
            {
                result = Encode(ref Unsafe.Add(ref bufferBytes, (IntPtr)sourceIndex), ref encodingMap);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref bufferBytes, (IntPtr)destinationIndex), result);
                destinationIndex -= 4;
                sourceIndex -= 3;
            }

            bytesWritten = encodedLength;
            return OperationStatus.Done;

        FalseExit:
            bytesWritten = 0;
            return OperationStatus.DestinationTooSmall;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Avx2Encode(ref byte src, ref byte dest, int sourceLength, int destLength, ref uint sourceIndex, ref uint destIndex)
        {
            ref byte srcStart = ref src;
            ref byte destStart = ref dest;
            ref byte simdSrcEnd = ref Unsafe.Add(ref src, (IntPtr)((uint)sourceLength - 28));	// 28 = 32 - 4

            // The JIT won't hoist these "constants", so help it
            Vector256<sbyte> shuffleVec = s_avxEncodeShuffleVec;
            Vector256<sbyte> shuffleConstant0 = Vector256.Create(0x0fc0fc00).AsSByte();
            Vector256<sbyte> shuffleConstant2 = Vector256.Create(0x003f03f0).AsSByte();
            Vector256<ushort> shuffleConstant1 = Vector256.Create(0x04000040).AsUInt16();
            Vector256<short> shuffleConstant3 = Vector256.Create(0x01000010).AsInt16();
            Vector256<byte> translationContant0 = Vector256.Create((byte)51);
            Vector256<sbyte> translationContant1 = Vector256.Create((sbyte)25);
            Vector256<sbyte> lut = s_avxEncodeLut;

            // first load is done at c-0 not to get a segfault
            AssertRead<Vector256<sbyte>>(ref src, ref srcStart, sourceLength);
            Vector256<sbyte> str = Unsafe.As<byte, Vector256<sbyte>>(ref src);

            // shift by 4 bytes, as required by enc_reshuffle
            str = Avx2.PermuteVar8x32(str.AsInt32(), s_avxEncodePermuteVec).AsSByte();

            while (true)
            {
                // Reshuffle
                str = Avx2.Shuffle(str, shuffleVec);
                Vector256<sbyte> t0 = Avx2.And(str, shuffleConstant0);
                Vector256<sbyte> t2 = Avx2.And(str, shuffleConstant2);
                Vector256<ushort> t1 = Avx2.MultiplyHigh(t0.AsUInt16(), shuffleConstant1);
                Vector256<short> t3 = Avx2.MultiplyLow(t2.AsInt16(), shuffleConstant3);
                str = Avx2.Or(t1.AsSByte(), t3.AsSByte());

                // Translation
                Vector256<byte> indices = Avx2.SubtractSaturate(str.AsByte(), translationContant0);
                Vector256<sbyte> mask = Avx2.CompareGreaterThan(str, translationContant1);
                Vector256<sbyte> tmp = Avx2.Subtract(indices.AsSByte(), mask);
                str = Avx2.Add(str, Avx2.Shuffle(lut, tmp));

                AssertWrite<Vector256<sbyte>>(ref dest, ref destStart, destLength);
                // As has better CQ than WriteUnaligned
                // https://github.com/dotnet/coreclr/issues/21132
                Unsafe.As<byte, Vector256<sbyte>>(ref dest) = str;

                src = ref Unsafe.Add(ref src, 24);
                dest = ref Unsafe.Add(ref dest, 32);

                if (Unsafe.IsAddressGreaterThan(ref src, ref simdSrcEnd))
                    break;

                // Load at c-4, as required by enc_reshuffle
                AssertRead<Vector256<sbyte>>(ref Unsafe.Add(ref src, -4), ref srcStart, sourceLength);
                str = Unsafe.As<byte, Vector256<sbyte>>(ref Unsafe.Add(ref src, -4));
            }

            // Cast to ulong to avoid the overflow-check. Codegen for x86 is still good.
            sourceIndex = (uint)(ulong)Unsafe.ByteOffset(ref srcStart, ref src);
            destIndex = (uint)(ulong)Unsafe.ByteOffset(ref destStart, ref dest);

            src = ref srcStart;
            dest = ref destStart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Ssse3Encode(ref byte src, ref byte dest, int sourceLength, int destLength, ref uint sourceIndex, ref uint destIndex)
        {
            ref byte srcStart = ref src;
            ref byte destStart = ref dest;
            ref byte simdSrcEnd = ref Unsafe.Add(ref src, (IntPtr)((uint)sourceLength - 16 + 1));

            // Shift to workspace
            src = ref Unsafe.Add(ref src, (IntPtr)sourceIndex);
            dest = ref Unsafe.Add(ref dest, (IntPtr)destIndex);

            // The JIT won't hoist these "constants", so help it
            Vector128<sbyte> shuffleVec = s_sseEncodeShuffleVec;
            Vector128<sbyte> shuffleConstant0 = Vector128.Create(0x0fc0fc00).AsSByte();
            Vector128<sbyte> shuffleConstant2 = Vector128.Create(0x003f03f0).AsSByte();
            Vector128<ushort> shuffleConstant1 = Vector128.Create(0x04000040).AsUInt16();
            Vector128<short> shuffleConstant3 = Vector128.Create(0x01000010).AsInt16();
            Vector128<byte> translationContant0 = Vector128.Create((byte)51);
            Vector128<sbyte> translationContant1 = Vector128.Create((sbyte)25);
            Vector128<sbyte> lut = s_sseEncodeLut;

            //while (remaining >= 16)
            while (Unsafe.IsAddressLessThan(ref src, ref simdSrcEnd))
            {
                AssertRead<Vector128<sbyte>>(ref src, ref srcStart, sourceLength);
                Vector128<sbyte> str = Unsafe.As<byte, Vector128<sbyte>>(ref src);

                // Reshuffle
                str = Ssse3.Shuffle(str, shuffleVec);
                Vector128<sbyte> t0 = Sse2.And(str, shuffleConstant0);
                Vector128<sbyte> t2 = Sse2.And(str, shuffleConstant2);
                Vector128<ushort> t1 = Sse2.MultiplyHigh(t0.AsUInt16(), shuffleConstant1);
                Vector128<short> t3 = Sse2.MultiplyLow(t2.AsInt16(), shuffleConstant3);
                str = Sse2.Or(t1.AsSByte(), t3.AsSByte());

                // Translation
                Vector128<byte> indices = Sse2.SubtractSaturate(str.AsByte(), translationContant0);
                Vector128<sbyte> mask = Sse2.CompareGreaterThan(str, translationContant1);
                Vector128<sbyte> tmp = Sse2.Subtract(indices.AsSByte(), mask);
                str = Sse2.Add(str, Ssse3.Shuffle(lut, tmp));

                AssertWrite<Vector128<sbyte>>(ref dest, ref destStart, destLength);
                // As has better CQ than WriteUnaligned
                // https://github.com/dotnet/coreclr/issues/21132
                Unsafe.As<byte, Vector128<sbyte>>(ref dest) = str;

                src = ref Unsafe.Add(ref src, 12);
                dest = ref Unsafe.Add(ref dest, 16);
            }

            // Cast to ulong to avoid the overflow-check. Codegen for x86 is still good.
            sourceIndex = (uint)(ulong)Unsafe.ByteOffset(ref srcStart, ref src);
            destIndex = (uint)(ulong)Unsafe.ByteOffset(ref destStart, ref dest);

            src = ref srcStart;
            dest = ref destStart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Encode(ref byte threeBytes, ref byte encodingMap)
        {
            uint i = (uint)((threeBytes << 16) | (Unsafe.Add(ref threeBytes, 1) << 8) | Unsafe.Add(ref threeBytes, 2));

            uint i0 = Unsafe.Add(ref encodingMap, (IntPtr)(i >> 18));
            uint i1 = Unsafe.Add(ref encodingMap, (IntPtr)((i >> 12) & 0x3F));
            uint i2 = Unsafe.Add(ref encodingMap, (IntPtr)((i >> 6) & 0x3F));
            uint i3 = Unsafe.Add(ref encodingMap, (IntPtr)(i & 0x3F));

            return i0 | (i1 << 8) | (i2 << 16) | (i3 << 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint EncodeAndPadOne(ref byte twoBytes, ref byte encodingMap)
        {
            uint i = (uint)((twoBytes << 16) | (Unsafe.Add(ref twoBytes, 1) << 8));

            uint i0 = Unsafe.Add(ref encodingMap, (IntPtr)(i >> 18));
            uint i1 = Unsafe.Add(ref encodingMap, (IntPtr)((i >> 12) & 0x3F));
            uint i2 = Unsafe.Add(ref encodingMap, (IntPtr)((i >> 6) & 0x3F));

            return i0 | (i1 << 8) | (i2 << 16) | (EncodingPad << 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint EncodeAndPadTwo(ref byte oneByte, ref byte encodingMap)
        {
            uint i = (uint)(oneByte << 8);

            uint i0 = Unsafe.Add(ref encodingMap, (IntPtr)(i >> 10));
            uint i1 = Unsafe.Add(ref encodingMap, (IntPtr)((i >> 4) & 0x3F));

            return i0 | (i1 << 8) | (EncodingPad << 16) | (EncodingPad << 24);
        }

        // Pre-computing this table using a custom string(s_characters) and GenerateEncodingMapAndVerify (found in tests)
        private static readonly byte[] s_encodingMap = {
            65, 66, 67, 68, 69, 70, 71, 72,         //A..H
            73, 74, 75, 76, 77, 78, 79, 80,         //I..P
            81, 82, 83, 84, 85, 86, 87, 88,         //Q..X
            89, 90, 97, 98, 99, 100, 101, 102,      //Y..Z, a..f
            103, 104, 105, 106, 107, 108, 109, 110, //g..n
            111, 112, 113, 114, 115, 116, 117, 118, //o..v
            119, 120, 121, 122, 48, 49, 50, 51,     //w..z, 0..3
            52, 53, 54, 55, 56, 57, 43, 47          //4..9, +, /
        };

        private static readonly Vector128<sbyte> s_sseEncodeShuffleVec;
        private static readonly Vector128<sbyte> s_sseEncodeLut;

        private static readonly Vector256<int> s_avxEncodePermuteVec;
        private static readonly Vector256<sbyte> s_avxEncodeShuffleVec;
        private static readonly Vector256<sbyte> s_avxEncodeLut;

        private const byte EncodingPad = (byte)'='; // '=', for padding

        private const int MaximumEncodeLength = (int.MaxValue / 4) * 3; // 1610612733
    }
}
