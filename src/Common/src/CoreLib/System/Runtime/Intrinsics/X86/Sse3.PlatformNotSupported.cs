// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel SSE3 hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Sse3 : Sse2
    {
        internal Sse3() { }

        public new static bool IsSupported { get { return false; } }

        /// <summary>
        /// __m128 _mm_addsub_ps (__m128 a, __m128 b)
        ///   ADDSUBPS xmm, xmm/m128
        /// </summary>
        public static Vector128<float> AddSubtract(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_addsub_pd (__m128d a, __m128d b)
        ///   ADDSUBPD xmm, xmm/m128
        /// </summary>
        public static Vector128<double> AddSubtract(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_hadd_ps (__m128 a, __m128 b)
        ///   HADDPS xmm, xmm/m128
        /// </summary>
        public static Vector128<float> HorizontalAdd(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_hadd_pd (__m128d a, __m128d b)
        ///   HADDPD xmm, xmm/m128
        /// </summary>
        public static Vector128<double> HorizontalAdd(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_hsub_ps (__m128 a, __m128 b)
        ///   HSUBPS xmm, xmm/m128
        /// </summary>
        public static Vector128<float> HorizontalSubtract(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_hsub_pd (__m128d a, __m128d b)
        ///   HSUBPD xmm, xmm/m128
        /// </summary>
        public static Vector128<double> HorizontalSubtract(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_loaddup_pd (double const* mem_addr)
        /// MOVDDUP xmm, m64
        /// </summary>
        public static unsafe Vector128<double> LoadAndDuplicateToVector128(double* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm_lddqu_si128 (__m128i const* mem_addr)
        ///   LDDQU xmm, m128
        /// </summary>
        public static unsafe Vector128<sbyte> LoadDquVector128(sbyte* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<byte> LoadDquVector128(byte* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<short> LoadDquVector128(short* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<ushort> LoadDquVector128(ushort* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<int> LoadDquVector128(int* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<uint> LoadDquVector128(uint* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<long> LoadDquVector128(long* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<ulong> LoadDquVector128(ulong* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_movedup_pd (__m128d a)
        ///   MOVDDUP xmm, xmm/m64
        /// </summary>
        public static Vector128<double> MoveAndDuplicate(Vector128<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_movehdup_ps (__m128 a)
        ///   MOVSHDUP xmm, xmm/m128
        /// </summary>
        public static Vector128<float> MoveHighAndDuplicate(Vector128<float> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_moveldup_ps (__m128 a)
        ///   MOVSLDUP xmm, xmm/m128
        /// </summary>
        public static Vector128<float> MoveLowAndDuplicate(Vector128<float> source) { throw new PlatformNotSupportedException(); }

    }
}
