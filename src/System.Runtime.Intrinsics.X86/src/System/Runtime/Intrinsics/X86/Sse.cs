// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel SSE hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public static class Sse
    {
        public static bool IsSupported { get { return false; } }

        /// <summary>
        /// __m128 _mm_add_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> Add(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_and_ps (__m128 a, __m128 b)
        /// </summary>
        public static Vector128<float> And(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_andnot_ps (__m128 a, __m128 b)
        /// </summary>
        public static Vector128<float> AndNot(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpeq_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareEqual(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpgt_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareGreaterThan(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpge_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareGreaterThanOrEqual(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmplt_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareLessThan(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmple_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareLessThanOrEqual(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpneq_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareNotEqual(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpngt_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareNotGreaterThan(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpnge_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareNotGreaterThanOrEqual(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpnlt_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareNotLessThan(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpnle_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareNotLessThanOrEqual(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpord_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareOrdered(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmpunord_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> CompareUnordered(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }
        
        /// <summary>
        /// __m128 _mm_div_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> Divide(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_loadu_ps (float const* mem_address)
        /// </summary>
        public static unsafe Vector128<float> Load(float* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_load_ps (float const* mem_address)
        /// </summary>
        public static unsafe Vector128<float> LoadAligned(float* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_max_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> Max(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_min_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> Min(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_movehl_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> MoveHighToLow(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_movelh_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> MoveLowToHigh(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_mul_ps (__m128 a, __m128 b)
        /// </summary>
        public static Vector128<float> Multiply(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_or_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> Or(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_rcp_ps (__m128 a)
        /// </summary>
        public static Vector128<float> Reciprocal(Vector128<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_rsqrt_ps (__m128 a)
        /// </summary>
        public static Vector128<float> ReciprocalSquareRoot(Vector128<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_set_ps (float e3, float e2, float e1, float e0)
        /// </summary>
        public static Vector128<float> Set(float e3, float e2, float e1, float e0) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_set1_ps (float a)
        /// </summary>
        public static Vector128<float> Set1(float value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_setzero_ps (void)
        /// </summary>
        public static Vector128<float> SetZero() { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_castpd_ps (__m128d a)
        /// __m128i _mm_castpd_si128 (__m128d a)
        /// __m128d _mm_castps_pd (__m128 a)
        /// __m128i _mm_castps_si128 (__m128 a)
        /// __m128d _mm_castsi128_pd (__m128i a)
        /// __m128 _mm_castsi128_ps (__m128i a)
        /// </summary>
        public static Vector128<U> StaticCast<T, U>(Vector128<T> value) where T : struct where U : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_shuffle_ps (__m128 a,  __m128 b, unsigned int control)
        /// </summary>
        public static Vector128<float> Shuffle(Vector128<float> left, Vector128<float> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_sqrt_ps (__m128 a)
        /// </summary>
        public static Vector128<float> Sqrt(Vector128<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm_store_ps (float* mem_addr, __m128 a)
        /// </summary>
        public static unsafe void StoreAligned(float* address, Vector128<float> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm_stream_ps (float* mem_addr, __m128 a)
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(float* address, Vector128<float> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm_storeu_ps (float* mem_addr, __m128 a)
        /// </summary>
        public static unsafe void Store(float* address, Vector128<float> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_sub_ps (__m128d a, __m128d b)
        /// </summary>
        public static Vector128<float> Subtract(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_unpackhi_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> UnpackHigh(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_unpacklo_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> UnpackLow(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_xor_ps (__m128 a,  __m128 b)
        /// </summary>
        public static Vector128<float> Xor(Vector128<float> left,  Vector128<float> right) { throw new PlatformNotSupportedException(); }
    }
}
