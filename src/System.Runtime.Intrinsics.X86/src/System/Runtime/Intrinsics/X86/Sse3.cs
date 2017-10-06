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
    public static class Sse3
    {
        public static bool IsSupported { get { return false; } }
        
        /// <summary>
        /// __m128 _mm_addsub_ps (__m128 a, __m128 b)
        /// </summary>
        public static Vector128<float> AddSubtract(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_addsub_pd (__m128d a, __m128d b)
        /// </summary>
        public static Vector128<double> AddSubtract(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_hadd_ps (__m128 a, __m128 b)
        /// </summary>
        public static Vector128<float> HorizontalAdd(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_hadd_pd (__m128d a, __m128d b)
        /// </summary>
        public static Vector128<double> HorizontalAdd(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_hsub_ps (__m128 a, __m128 b)
        /// </summary>
        public static Vector128<float> HorizontalSubtract(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_hsub_pd (__m128d a, __m128d b)
        /// </summary>
        public static Vector128<double> HorizontalSubtract(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_loaddup_pd (double const* mem_addr)
        /// </summary>
        public static unsafe Vector128<double> LoadAndDuplicate(double* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm_lddqu_si128 (__m128i const* mem_addr)
        /// </summary>
        public static unsafe Vector128<sbyte> LoadDqu(sbyte* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<byte> LoadDqu(byte* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<short> LoadDqu(short* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<ushort> LoadDqu(ushort* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<int> LoadDqu(int* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<uint> LoadDqu(uint* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<long> LoadDqu(long* address) { throw new PlatformNotSupportedException(); }
        public static unsafe Vector128<ulong> LoadDqu(ulong* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_movedup_pd (__m128d a)
        /// </summary>
        public static Vector128<double> MoveAndDuplicate(Vector128<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_movehdup_ps (__m128 a)
        /// </summary>
        public static Vector128<float> MoveHighAndDuplicate(Vector128<float> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_moveldup_ps (__m128 a)
        /// </summary>
        public static Vector128<float> MoveLowAndDuplicate(Vector128<float> source) { throw new PlatformNotSupportedException(); }

    }
}
