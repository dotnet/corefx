﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Internal.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    // AVX2 version based on https://github.com/aklomp/base64/tree/e516d769a2a432c08404f1981e73b431566057be/lib/arch/avx2
    // SSSE3 version based on https://github.com/aklomp/base64/tree/e516d769a2a432c08404f1981e73b431566057be/lib/arch/ssse3

    public static partial class Base64
    {
        static Base64()
        {
            if (Ssse3.IsSupported)
            {
                s_sseEncodeShuffleVec = Vector128.Create(
                     1, 0, 2, 1,
                     4, 3, 5, 4,
                     7, 6, 8, 7,
                    10, 9, 11, 10
                );

                s_sseEncodeLut = Vector128.Create(
                     65, 71, -4, -4,
                     -4, -4, -4, -4,
                     -4, -4, -4, -4,
                    -19, -16, 0, 0
                );

                s_sseDecodeShuffleVec = Vector128.Create(
                     2, 1, 0, 6,
                     5, 4, 10, 9,
                     8, 14, 13, 12,
                    -1, -1, -1, -1
                );

                s_sseDecodeLutLo = Vector128.Create(
                    0x15, 0x11, 0x11, 0x11,
                    0x11, 0x11, 0x11, 0x11,
                    0x11, 0x11, 0x13, 0x1A,
                    0x1B, 0x1B, 0x1B, 0x1A
                );

                s_sseDecodeLutHi = Vector128.Create(
                    0x10, 0x10, 0x01, 0x02,
                    0x04, 0x08, 0x04, 0x08,
                    0x10, 0x10, 0x10, 0x10,
                    0x10, 0x10, 0x10, 0x10
                );

                s_sseDecodeLutShift = Vector128.Create(
                      0, 16, 19, 4,
                    -65, -65, -71, -71,
                      0, 0, 0, 0,
                      0, 0, 0, 0
                );

                s_sseDecodeMask2F = Vector128.Create((sbyte)0x2F);     // ASCII: /
            }

            if (Avx2.IsSupported)
            {
                s_avxEncodePermuteVec = Vector256.Create(0, 0, 1, 2, 3, 4, 5, 6);

                s_avxEncodeShuffleVec = Vector256.Create(
                     5, 4, 6, 5,
                     8, 7, 9, 8,
                    11, 10, 12, 11,
                    14, 13, 15, 14,
                     1, 0, 2, 1,
                     4, 3, 5, 4,
                     7, 6, 8, 7,
                    10, 9, 11, 10
                );

                s_avxEncodeLut = Vector256.Create(
                     65, 71, -4, -4,
                     -4, -4, -4, -4,
                     -4, -4, -4, -4,
                    -19, -16, 0, 0,
                     65, 71, -4, -4,
                     -4, -4, -4, -4,
                     -4, -4, -4, -4,
                    -19, -16, 0, 0
                );

                s_avxDecodeLutLo = Vector256.Create(
                    0x15, 0x11, 0x11, 0x11,
                    0x11, 0x11, 0x11, 0x11,
                    0x11, 0x11, 0x13, 0x1A,
                    0x1B, 0x1B, 0x1B, 0x1A,
                    0x15, 0x11, 0x11, 0x11,
                    0x11, 0x11, 0x11, 0x11,
                    0x11, 0x11, 0x13, 0x1A,
                    0x1B, 0x1B, 0x1B, 0x1A
                );

                s_avxDecodeLutHi = Vector256.Create(
                    0x10, 0x10, 0x01, 0x02,
                    0x04, 0x08, 0x04, 0x08,
                    0x10, 0x10, 0x10, 0x10,
                    0x10, 0x10, 0x10, 0x10,
                    0x10, 0x10, 0x01, 0x02,
                    0x04, 0x08, 0x04, 0x08,
                    0x10, 0x10, 0x10, 0x10,
                    0x10, 0x10, 0x10, 0x10
                );

                s_avxDecodeLutShift = Vector256.Create(
                     0, 16, 19, 4,
                    -65, -65, -71, -71,
                      0, 0, 0, 0,
                      0, 0, 0, 0,
                      0, 16, 19, 4,
                    -65, -65, -71, -71,
                      0, 0, 0, 0,
                      0, 0, 0, 0
                );

                s_avxDecodeShuffleVec = Vector256.Create(
                    2, 1, 0, 6,
                    5, 4, 10, 9,
                    8, 14, 13, 12,
                   -1, -1, -1, -1,
                    2, 1, 0, 6,
                    5, 4, 10, 9,
                    8, 14, 13, 12,
                   -1, -1, -1, -1
               );

                s_avxDecodePermuteVec = Vector256.Create(0, 1, 2, 4, 5, 6, -1, -1);

                s_avxDecodeMask2F = Vector256.Create((sbyte)0x2F);    // ASCII: /
            }
        }

        [Conditional("DEBUG")]
        private static unsafe void AssertRead<TVector>(byte* src, byte* srcStart, int srcLength)
        {
            int vectorElements = Unsafe.SizeOf<TVector>();
            byte* readEnd = src + vectorElements;
            byte* srcEnd = srcStart + srcLength;

            if (readEnd > srcEnd)
            {
                int srcIndex = (int)(src - srcStart);
                throw new InvalidOperationException($"Read for {typeof(TVector)} is not within safe bounds. srcIndex: {srcIndex}, srcLength: {srcLength}");
            }
        }

        [Conditional("DEBUG")]
        private static unsafe void AssertWrite<TVector>(byte* dest, byte* destStart, int destLength)
        {
            int vectorElements = Unsafe.SizeOf<TVector>();
            byte* writeEnd = dest + vectorElements;
            byte* destEnd = destStart + destLength;

            if (writeEnd > destEnd)
            {
                int destIndex = (int)(dest - destStart);
                throw new InvalidOperationException($"Write for {typeof(TVector)} is not within safe bounds. destIndex: {destIndex}, destLength: {destLength}");
            }
        }
    }
}
