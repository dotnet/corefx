// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel AVX hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Avx : Sse42
    {
        internal Avx() { }

        public new static bool IsSupported { get { return false; } }

        /// <summary>
        /// __m256 _mm256_add_ps (__m256 a, __m256 b)
        ///   VADDPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Add(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_add_pd (__m256d a, __m256d b)
        ///   VADDPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Add(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_addsub_ps (__m256 a, __m256 b)
        ///   VADDSUBPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> AddSubtract(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_addsub_pd (__m256d a, __m256d b)
        ///   VADDSUBPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> AddSubtract(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_and_ps (__m256 a, __m256 b)
        ///   VANDPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> And(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_and_pd (__m256d a, __m256d b)
        ///   VANDPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> And(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_andnot_ps (__m256 a, __m256 b)
        ///   VANDNPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> AndNot(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_andnot_pd (__m256d a, __m256d b)
        ///   VANDNPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> AndNot(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_blend_ps (__m256 a, __m256 b, const int imm8)
        ///   VBLENDPS ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<float> Blend(Vector256<float> left, Vector256<float> right, byte control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_blend_pd (__m256d a, __m256d b, const int imm8)
        ///   VBLENDPD ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<double> Blend(Vector256<double> left, Vector256<double> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_blendv_ps (__m256 a, __m256 b, __m256 mask)
        ///   VBLENDVPS ymm, ymm, ymm/m256, ymm
        /// </summary>
        public static Vector256<float> BlendVariable(Vector256<float> left, Vector256<float> right, Vector256<float> mask) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_blendv_pd (__m256d a, __m256d b, __m256d mask)
        ///   VBLENDVPD ymm, ymm, ymm/m256, ymm
        /// </summary>
        public static Vector256<double> BlendVariable(Vector256<double> left, Vector256<double> right, Vector256<double> mask) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_broadcast_ss (float const * mem_addr)
        ///   VBROADCASTSS xmm, m32
        /// </summary>
        public static unsafe Vector128<float> BroadcastScalarToVector128(float* source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_broadcast_ss (float const * mem_addr)
        ///   VBROADCASTSS ymm, m32
        /// </summary>
        public static unsafe Vector256<float> BroadcastScalarToVector256(float* source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_broadcast_sd (double const * mem_addr)
        ///   VBROADCASTSD ymm, m64
        /// </summary>
        public static unsafe Vector256<double> BroadcastScalarToVector256(double* source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_broadcast_ps (__m128 const * mem_addr)
        ///   VBROADCASTF128, ymm, m128
        /// </summary>
        public static unsafe Vector256<float> BroadcastVector128ToVector256(float* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_broadcast_pd (__m128d const * mem_addr)
        ///   VBROADCASTF128, ymm, m128
        /// </summary>
        public static unsafe Vector256<double> BroadcastVector128ToVector256(double* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_ceil_ps (__m256 a)
        ///   VROUNDPS ymm, ymm/m256, imm8(10)
        /// </summary>
        public static Vector256<float> Ceiling(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_ceil_pd (__m256d a)
        ///   VROUNDPD ymm, ymm/m256, imm8(10)
        /// </summary>
        public static Vector256<double> Ceiling(Vector256<double> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_cmp_ps (__m128 a, __m128 b, const int imm8)
        ///   VCMPPS xmm, xmm, xmm/m128, imm8
        /// </summary>
        public static Vector128<float> Compare(Vector128<float> left, Vector128<float> right, FloatComparisonMode mode) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_cmp_pd (__m128d a, __m128d b, const int imm8)
        ///   VCMPPD xmm, xmm, xmm/m128, imm8
        /// </summary>
        public static Vector128<double> Compare(Vector128<double> left, Vector128<double> right, FloatComparisonMode mode) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_cmp_ps (__m256 a, __m256 b, const int imm8)
        ///   VCMPPS ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<float> Compare(Vector256<float> left, Vector256<float> right, FloatComparisonMode mode) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_cmp_pd (__m256d a, __m256d b, const int imm8)
        ///   VCMPPD ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<double> Compare(Vector256<double> left, Vector256<double> right, FloatComparisonMode mode) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm_cmp_sd (__m128d a, __m128d b, const int imm8)
        ///   VCMPSS xmm, xmm, xmm/m32, imm8
        /// </summary>
        public static Vector128<double> CompareScalar(Vector128<double> left, Vector128<double> right, FloatComparisonMode mode) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128 _mm_cmp_ss (__m128 a, __m128 b, const int imm8)
        ///   VCMPSD xmm, xmm, xmm/m64, imm8
        /// </summary>
        public static Vector128<float> CompareScalar(Vector128<float> left, Vector128<float> right, FloatComparisonMode mode) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// float _mm256_cvtss_f32 (__m256 a)
        ///   HELPER: VMOVSS
        /// </summary>
        public static float ConvertToSingle(Vector256<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_cvtpd_epi32 (__m256d a)
        ///   VCVTPD2DQ xmm, ymm/m256
        /// </summary>
        public static Vector128<int> ConvertToVector128Int32(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128 _mm256_cvtpd_ps (__m256d a)
        ///   VCVTPD2PS xmm, ymm/m256
        /// </summary>
        public static Vector128<float> ConvertToVector128Single(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_cvtps_epi32 (__m256 a)
        ///   VCVTPS2DQ ymm, ymm/m256
        /// </summary>
        public static Vector256<int> ConvertToVector256Int32(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_cvtepi32_ps (__m256i a)
        ///   VCVTDQ2PS ymm, ymm/m256
        /// </summary>
        public static Vector256<float> ConvertToVector256Single(Vector256<int> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_cvtps_pd (__m128 a)
        ///   VCVTPS2PD ymm, xmm/m128
        /// </summary>
        public static Vector256<double> ConvertToVector256Double(Vector128<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_cvtepi32_pd (__m128i a)
        ///   VCVTDQ2PD ymm, xmm/m128
        /// </summary>
        public static Vector256<double> ConvertToVector256Double(Vector128<int> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_cvttpd_epi32 (__m256d a)
        ///   VCVTTPD2DQ xmm, ymm/m256
        /// </summary>
        public static Vector128<int> ConvertToVector128Int32WithTruncation(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_cvttps_epi32 (__m256 a)
        ///   VCVTTPS2DQ ymm, ymm/m256
        /// </summary>
        public static Vector256<int> ConvertToVector256Int32WithTruncation(Vector256<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_div_ps (__m256 a, __m256 b)
        ///   VDIVPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Divide(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_div_pd (__m256d a, __m256d b)
        ///   VDIVPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Divide(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_dp_ps (__m256 a, __m256 b, const int imm8)
        ///   VDPPS ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<float> DotProduct(Vector256<float> left, Vector256<float> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_moveldup_ps (__m256 a)
        ///   VMOVSLDUP ymm, ymm/m256
        /// </summary>
        public static Vector256<float> DuplicateEvenIndexed(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_movedup_pd (__m256d a)
        ///   VMOVDDUP ymm, ymm/m256
        /// </summary>
        public static Vector256<double> DuplicateEvenIndexed(Vector256<double> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_movehdup_ps (__m256 a)
        ///   VMOVSHDUP ymm, ymm/m256
        /// </summary>
        public static Vector256<float> DuplicateOddIndexed(Vector256<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __int8 _mm256_extract_epi8 (__m256i a, const int index)
        ///   HELPER
        /// </summary>
        public static byte Extract(Vector256<byte> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __int16 _mm256_extract_epi16 (__m256i a, const int index)
        ///   HELPER
        /// </summary>
        public static ushort Extract(Vector256<ushort> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __int32 _mm256_extract_epi32 (__m256i a, const int index)
        ///   HELPER
        /// </summary>
        public static int Extract(Vector256<int> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __int32 _mm256_extract_epi32 (__m256i a, const int index)
        ///   HELPER
        /// </summary>
        public static uint Extract(Vector256<uint> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __int64 _mm256_extract_epi64 (__m256i a, const int index)
        ///   HELPER
        /// </summary>
        public static long Extract(Vector256<long> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __int64 _mm256_extract_epi64 (__m256i a, const int index)
        ///   HELPER
        /// </summary>
        public static ulong Extract(Vector256<ulong> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<byte> ExtractVector128(Vector256<byte> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<sbyte> ExtractVector128(Vector256<sbyte> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<short> ExtractVector128(Vector256<short> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<ushort> ExtractVector128(Vector256<ushort> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<int> ExtractVector128(Vector256<int> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<uint> ExtractVector128(Vector256<uint> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<long> ExtractVector128(Vector256<long> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<ulong> ExtractVector128(Vector256<ulong> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm256_extractf128_ps (__m256 a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<float> ExtractVector128(Vector256<float> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm256_extractf128_pd (__m256d a, const int imm8)
        ///   VEXTRACTF128 xmm/m128, ymm, imm8
        /// </summary>
        public static Vector128<double> ExtractVector128(Vector256<double> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(byte* address, Vector256<byte> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(sbyte* address, Vector256<sbyte> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(short* address, Vector256<short> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(ushort* address, Vector256<ushort> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(int* address, Vector256<int> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(uint* address, Vector256<uint> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(long* address, Vector256<long> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128i _mm256_extractf128_si256 (__m256i a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(ulong* address, Vector256<ulong> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128 _mm256_extractf128_ps (__m256 a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(float* address, Vector256<float> value, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm256_extractf128_pd (__m256d a, const int imm8)
        ///   VEXTRACTF128 m128, ymm, imm8
        /// </summary>
        public static unsafe void ExtractVector128(double* address, Vector256<double> value, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256d _mm256_castpd128_pd256 (__m128d a)
        ///   HELPER - No Codegen
        /// __m256 _mm256_castps128_ps256 (__m128 a)
        ///   HELPER - No Codegen
        /// __m256i _mm256_castsi128_si256 (__m128i a)
        ///   HELPER - No Codegen
        /// </summary>
        public static Vector256<T> ExtendToVector256<T>(Vector128<T> value) where T : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_floor_ps (__m256 a)
        ///   VROUNDPS ymm, ymm/m256, imm8(9)
        /// </summary>
        public static Vector256<float> Floor(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_floor_pd (__m256d a)
        ///   VROUNDPS ymm, ymm/m256, imm8(9)
        /// </summary>
        public static Vector256<double> Floor(Vector256<double> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128d _mm256_castpd256_pd128 (__m256d a)
        ///   HELPER - No Codegen
        /// __m128 _mm256_castps256_ps128 (__m256 a)
        ///   HELPER - No Codegen
        /// __m128i _mm256_castsi256_si128 (__m256i a)
        ///   HELPER - No Codegen
        /// </summary>
        public static Vector128<T> GetLowerHalf<T>(Vector256<T> value) where T : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_hadd_ps (__m256 a, __m256 b)
        ///   VHADDPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> HorizontalAdd(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_hadd_pd (__m256d a, __m256d b)
        ///   VHADDPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> HorizontalAdd(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_hsub_ps (__m256 a, __m256 b)
        ///   VHSUBPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> HorizontalSubtract(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_hsub_pd (__m256d a, __m256d b)
        ///   VHSUBPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> HorizontalSubtract(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insert_epi8 (__m256i a, __int8 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<sbyte> Insert(Vector256<sbyte> value, sbyte data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi8 (__m256i a, __int8 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<byte> Insert(Vector256<byte> value, byte data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi16 (__m256i a, __int16 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<short> Insert(Vector256<short> value, short data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi16 (__m256i a, __int16 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<ushort> Insert(Vector256<ushort> value, ushort data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi32 (__m256i a, __int32 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<int> Insert(Vector256<int> value, int data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi32 (__m256i a, __int32 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<uint> Insert(Vector256<uint> value, uint data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi64 (__m256i a, __int64 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<long> Insert(Vector256<long> value, long data, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insert_epi64 (__m256i a, __int64 i, const int index)
        ///   HELPER
        /// </summary>
        public static Vector256<ulong> Insert(Vector256<ulong> value, ulong data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<byte> InsertVector128(Vector256<byte> value, Vector128<byte> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<sbyte> InsertVector128(Vector256<sbyte> value, Vector128<sbyte> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<short> InsertVector128(Vector256<short> value, Vector128<short> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<ushort> InsertVector128(Vector256<ushort> value, Vector128<ushort> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<int> InsertVector128(Vector256<int> value, Vector128<int> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<uint> InsertVector128(Vector256<uint> value, Vector128<uint> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<long> InsertVector128(Vector256<long> value, Vector128<long> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<ulong> InsertVector128(Vector256<ulong> value, Vector128<ulong> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_insertf128_ps (__m256 a, __m128 b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<float> InsertVector128(Vector256<float> value, Vector128<float> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256d _mm256_insertf128_pd (__m256d a, __m128d b, int imm8)
        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
        /// </summary>
        public static Vector256<double> InsertVector128(Vector256<double> value, Vector128<double> data, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<sbyte> InsertVector128(Vector256<sbyte> value, sbyte* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<byte> InsertVector128(Vector256<byte> value, byte* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<short> InsertVector128(Vector256<short> value, short* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<ushort> InsertVector128(Vector256<ushort> value, ushort* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<int> InsertVector128(Vector256<int> value, int* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<uint> InsertVector128(Vector256<uint> value, uint* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<long> InsertVector128(Vector256<long> value, long* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_insertf128_si256 (__m256i a, __m128i b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<ulong> InsertVector128(Vector256<ulong> value, ulong* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_insertf128_ps (__m256 a, __m128 b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<float> InsertVector128(Vector256<float> value, float* address, byte index) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_insertf128_pd (__m256d a, __m128d b, int imm8)
        ///   VINSERTF128 ymm, ymm, m128, imm8
        /// </summary>
        public static unsafe Vector256<double> InsertVector128(Vector256<double> value, double* address, byte index) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<sbyte> LoadVector256(sbyte* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<byte> LoadVector256(byte* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<short> LoadVector256(short* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<ushort> LoadVector256(ushort* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<int> LoadVector256(int* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<uint> LoadVector256(uint* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<long> LoadVector256(long* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_loadu_si256 (__m256i const * mem_addr)
        ///   VMOVDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<ulong> LoadVector256(ulong* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_loadu_ps (float const * mem_addr)
        ///   VMOVUPS ymm, ymm/m256
        /// </summary>
        public static unsafe Vector256<float> LoadVector256(float* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_loadu_pd (double const * mem_addr)
        ///   VMOVUPD ymm, ymm/m256
        /// </summary>
        public static unsafe Vector256<double> LoadVector256(double* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<sbyte> LoadAlignedVector256(sbyte* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<byte> LoadAlignedVector256(byte* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<short> LoadAlignedVector256(short* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<ushort> LoadAlignedVector256(ushort* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<int> LoadAlignedVector256(int* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<uint> LoadAlignedVector256(uint* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<long> LoadAlignedVector256(long* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_load_si256 (__m256i const * mem_addr)
        ///   VMOVDQA ymm, m256
        /// </summary>
        public static unsafe Vector256<ulong> LoadAlignedVector256(ulong* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_load_ps (float const * mem_addr)
        ///   VMOVAPS ymm, ymm/m256
        /// </summary>
        public static unsafe Vector256<float> LoadAlignedVector256(float* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_load_pd (double const * mem_addr)
        ///   VMOVAPD ymm, ymm/m256
        /// </summary>
        public static unsafe Vector256<double> LoadAlignedVector256(double* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<sbyte> LoadDquVector256(sbyte* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<byte> LoadDquVector256(byte* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<short> LoadDquVector256(short* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<ushort> LoadDquVector256(ushort* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<int> LoadDquVector256(int* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<uint> LoadDquVector256(uint* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<long> LoadDquVector256(long* address) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_lddqu_si256 (__m256i const * mem_addr)
        ///   VLDDQU ymm, m256
        /// </summary>
        public static unsafe Vector256<ulong> LoadDquVector256(ulong* address) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_maskload_ps (float const * mem_addr, __m128i mask)
        ///   VMASKMOVPS xmm, xmm, m128
        /// </summary>
        public static unsafe Vector128<float> MaskLoad(float* address, Vector128<float> mask) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_maskload_pd (double const * mem_addr, __m128i mask)
        ///   VMASKMOVPD xmm, xmm, m128
        /// </summary>
        public static unsafe Vector128<double> MaskLoad(double* address, Vector128<double> mask) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_maskload_ps (float const * mem_addr, __m256i mask)
        ///   VMASKMOVPS ymm, ymm, m256
        /// </summary>
        public static unsafe Vector256<float> MaskLoad(float* address, Vector256<float> mask) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_maskload_pd (double const * mem_addr, __m256i mask)
        ///   VMASKMOVPD ymm, ymm, m256
        /// </summary>
        public static unsafe Vector256<double> MaskLoad(double* address, Vector256<double> mask) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm_maskstore_ps (float * mem_addr, __m128i mask, __m128 a)
        ///   VMASKMOVPS m128, xmm, xmm
        /// </summary>
        public static unsafe void MaskStore(float* address, Vector128<float> mask, Vector128<float> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm_maskstore_pd (double * mem_addr, __m128i mask, __m128d a)
        ///   VMASKMOVPD m128, xmm, xmm
        /// </summary>
        public static unsafe void MaskStore(double* address, Vector128<double> mask, Vector128<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm256_maskstore_ps (float * mem_addr, __m256i mask, __m256 a)
        ///   VMASKMOVPS m256, ymm, ymm
        /// </summary>
        public static unsafe void MaskStore(float* address, Vector256<float> mask, Vector256<float> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_maskstore_pd (double * mem_addr, __m256i mask, __m256d a)
        ///   VMASKMOVPD m256, ymm, ymm
        /// </summary>
        public static unsafe void MaskStore(double* address, Vector256<double> mask, Vector256<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_max_ps (__m256 a, __m256 b)
        ///   VMAXPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Max(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_max_pd (__m256d a, __m256d b)
        ///   VMAXPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Max(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_min_ps (__m256 a, __m256 b)
        ///   VMINPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Min(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_min_pd (__m256d a, __m256d b)
        ///   VMINPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Min(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_movemask_ps (__m256 a)
        ///   VMOVMSKPS reg, ymm
        /// </summary>
        public static int MoveMask(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// int _mm256_movemask_pd (__m256d a)
        ///   VMOVMSKPD reg, ymm
        /// </summary>
        public static int MoveMask(Vector256<double> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_mul_ps (__m256 a, __m256 b)
        ///   VMULPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Multiply(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_mul_pd (__m256d a, __m256d b)
        ///   VMULPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Multiply(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_or_ps (__m256 a, __m256 b)
        ///   VORPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Or(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_or_pd (__m256d a, __m256d b)
        ///   VORPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Or(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_permute_ps (__m128 a, int imm8)
        ///   VPERMILPS xmm, xmm, imm8
        /// </summary>
        public static Vector128<float> Permute(Vector128<float> value, byte control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_permute_pd (__m128d a, int imm8)
        ///   VPERMILPD xmm, xmm, imm8
        /// </summary>
        public static Vector128<double> Permute(Vector128<double> value, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_permute_ps (__m256 a, int imm8)
        ///   VPERMILPS ymm, ymm, imm8
        /// </summary>
        public static Vector256<float> Permute(Vector256<float> value, byte control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_permute_pd (__m256d a, int imm8)
        ///   VPERMILPD ymm, ymm, imm8
        /// </summary>
        public static Vector256<double> Permute(Vector256<double> value, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<byte> Permute2x128(Vector256<byte> left, Vector256<byte> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<sbyte> Permute2x128(Vector256<sbyte> left, Vector256<sbyte> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<short> Permute2x128(Vector256<short> left, Vector256<short> right, byte control) { throw new PlatformNotSupportedException(); }
        
        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<ushort> Permute2x128(Vector256<ushort> left, Vector256<ushort> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<int> Permute2x128(Vector256<int> left, Vector256<int> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<uint> Permute2x128(Vector256<uint> left, Vector256<uint> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<long> Permute2x128(Vector256<long> left, Vector256<long> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_permute2f128_si256 (__m256i a, __m256i b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<ulong> Permute2x128(Vector256<ulong> left, Vector256<ulong> right, byte control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_permute2f128_ps (__m256 a, __m256 b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<float> Permute2x128(Vector256<float> left, Vector256<float> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256d _mm256_permute2f128_pd (__m256d a, __m256d b, int imm8)
        ///   VPERM2F128 ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<double> Permute2x128(Vector256<double> left, Vector256<double> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128 _mm_permutevar_ps (__m128 a, __m128i b)
        ///   VPERMILPS xmm, xmm, xmm/m128
        /// </summary>
        public static Vector128<float> PermuteVar(Vector128<float> left, Vector128<int> control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m128d _mm_permutevar_pd (__m128d a, __m128i b)
        ///   VPERMILPD xmm, xmm, xmm/m128
        /// </summary>
        public static Vector128<double> PermuteVar(Vector128<double> left, Vector128<long> control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_permutevar_ps (__m256 a, __m256i b)
        ///   VPERMILPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> PermuteVar(Vector256<float> left, Vector256<int> control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_permutevar_pd (__m256d a, __m256i b)
        ///   VPERMILPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> PermuteVar(Vector256<double> left, Vector256<long> control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_rcp_ps (__m256 a)
        ///   VRCPPS ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Reciprocal(Vector256<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_rsqrt_ps (__m256 a)
        ///   VRSQRTPS ymm, ymm/m256
        /// </summary>
        public static Vector256<float> ReciprocalSqrt(Vector256<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_round_ps (__m256 a, _MM_FROUND_TO_NEAREST_INT | _MM_FROUND_NO_EXC)
        ///   VROUNDPS ymm, ymm/m256, imm8(8)
        /// </summary>
        public static Vector256<float> RoundToNearestInteger(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_round_ps (__m256 a, _MM_FROUND_TO_NEG_INF | _MM_FROUND_NO_EXC)
        ///   VROUNDPS ymm, ymm/m256, imm8(9)
        /// </summary>
        public static Vector256<float> RoundToNegativeInfinity(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_round_ps (__m256 a, _MM_FROUND_TO_POS_INF | _MM_FROUND_NO_EXC)
        ///   VROUNDPS ymm, ymm/m256, imm8(10)
        /// </summary>
        public static Vector256<float> RoundToPositiveInfinity(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_round_ps (__m256 a, _MM_FROUND_TO_ZERO | _MM_FROUND_NO_EXC)
        ///   VROUNDPS ymm, ymm/m256, imm8(11)
        /// </summary>
        public static Vector256<float> RoundToZero(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_round_ps (__m256 a, _MM_FROUND_CUR_DIRECTION)
        ///   VROUNDPS ymm, ymm/m256, imm8(4)
        /// </summary>
        public static Vector256<float> RoundCurrentDirection(Vector256<float> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256d _mm256_round_pd (__m256d a, _MM_FROUND_TO_NEAREST_INT | _MM_FROUND_NO_EXC)
        ///   VROUNDPD ymm, ymm/m256, imm8(8)
        /// </summary>
        public static Vector256<double> RoundToNearestInteger(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_round_pd (__m256d a, _MM_FROUND_TO_NEG_INF | _MM_FROUND_NO_EXC)
        ///   VROUNDPD ymm, ymm/m256, imm8(9)
        /// </summary>
        public static Vector256<double> RoundToNegativeInfinity(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_round_pd (__m256d a, _MM_FROUND_TO_POS_INF | _MM_FROUND_NO_EXC)
        ///   VROUNDPD ymm, ymm/m256, imm8(10)
        /// </summary>
        public static Vector256<double> RoundToPositiveInfinity(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_round_pd (__m256d a, _MM_FROUND_TO_ZERO | _MM_FROUND_NO_EXC)
        ///   VROUNDPD ymm, ymm/m256, imm8(11)
        /// </summary>
        public static Vector256<double> RoundToZero(Vector256<double> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_round_pd (__m256d a, _MM_FROUND_CUR_DIRECTION)
        ///   VROUNDPD ymm, ymm/m256, imm8(4)
        /// </summary>
        public static Vector256<double> RoundCurrentDirection(Vector256<double> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_set_epi8 (char e31, char e30, char e29, char e28, char e27, char e26, char e25, char e24, char e23, char e22, char e21, char e20, char e19, char e18, char e17, char e16, char e15, char e14, char e13, char e12, char e11, char e10, char e9, char e8, char e7, char e6, char e5, char e4, char e3, char e2, char e1, char e0)
        ///   HELPER
        /// </summary>
        public static Vector256<sbyte> SetVector256(sbyte e31, sbyte e30, sbyte e29, sbyte e28, sbyte e27, sbyte e26, sbyte e25, sbyte e24, sbyte e23, sbyte e22, sbyte e21, sbyte e20, sbyte e19, sbyte e18, sbyte e17, sbyte e16, sbyte e15, sbyte e14, sbyte e13, sbyte e12, sbyte e11, sbyte e10, sbyte e9, sbyte e8, sbyte e7, sbyte e6, sbyte e5, sbyte e4, sbyte e3, sbyte e2, sbyte e1, sbyte e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi8 (char e31, char e30, char e29, char e28, char e27, char e26, char e25, char e24, char e23, char e22, char e21, char e20, char e19, char e18, char e17, char e16, char e15, char e14, char e13, char e12, char e11, char e10, char e9, char e8, char e7, char e6, char e5, char e4, char e3, char e2, char e1, char e0)
        ///   HELPER
        /// </summary>
        public static Vector256<byte> SetVector256(byte e31, byte e30, byte e29, byte e28, byte e27, byte e26, byte e25, byte e24, byte e23, byte e22, byte e21, byte e20, byte e19, byte e18, byte e17, byte e16, byte e15, byte e14, byte e13, byte e12, byte e11, byte e10, byte e9, byte e8, byte e7, byte e6, byte e5, byte e4, byte e3, byte e2, byte e1, byte e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi16 (short e15, short e14, short e13, short e12, short e11, short e10, short e9, short e8, short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0)
        ///   HELPER
        /// </summary>
        public static Vector256<short> SetVector256(short e15, short e14, short e13, short e12, short e11, short e10, short e9, short e8, short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi16 (short e15, short e14, short e13, short e12, short e11, short e10, short e9, short e8, short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0)
        ///   HELPER
        /// </summary>
        public static Vector256<ushort> SetVector256(ushort e15, ushort e14, ushort e13, ushort e12, ushort e11, ushort e10, ushort e9, ushort e8, ushort e7, ushort e6, ushort e5, ushort e4, ushort e3, ushort e2, ushort e1, ushort e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi32 (int e7, int e6, int e5, int e4, int e3, int e2, int e1, int e0)
        ///   HELPER
        /// </summary>
        public static Vector256<int> SetVector256(int e7, int e6, int e5, int e4, int e3, int e2, int e1, int e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi32 (int e7, int e6, int e5, int e4, int e3, int e2, int e1, int e0)
        ///   HELPER
        /// </summary>
        public static Vector256<uint> SetVector256(uint e7, uint e6, uint e5, uint e4, uint e3, uint e2, uint e1, uint e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi64x (__int64 e3, __int64 e2, __int64 e1, __int64 e0)
        ///   HELPER
        /// </summary>
        public static Vector256<long> SetVector256(long e3, long e2, long e1, long e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256i _mm256_set_epi64x (__int64 e3, __int64 e2, __int64 e1, __int64 e0)
        ///   HELPER
        /// </summary>
        public static Vector256<ulong> SetVector256(ulong e3, ulong e2, ulong e1, ulong e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256 _mm256_set_ps (float e7, float e6, float e5, float e4, float e3, float e2, float e1, float e0)
        ///   HELPER
        /// </summary>
        public static Vector256<float> SetVector256(float e7, float e6, float e5, float e4, float e3, float e2, float e1, float e0) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_set_pd (double e3, double e2, double e1, double e0)
        ///   HELPER
        /// </summary>
        public static Vector256<double> SetVector256(double e3, double e2, double e1, double e0) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_set1_epi8 (char a)
        ///   HELPER
        /// __m256i _mm256_set1_epi16 (short a)
        ///   HELPER
        /// __m256i _mm256_set1_epi32 (int a)
        ///   HELPER
        /// __m256i _mm256_set1_epi64x (long long a)
        ///   HELPER
        /// __m256 _mm256_set1_ps (float a)
        ///   HELPER
        /// __m256d _mm256_set1_pd (double a)
        ///   HELPER
        /// </summary>
        public static Vector256<T> SetAllVector256<T>(T value) where T : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_set_m128 (__m128 hi, __m128 lo)
        ///   HELPER
        /// __m256d _mm256_set_m128d (__m128d hi, __m128d lo)
        ///   HELPER
        /// __m256i _mm256_set_m128i (__m128i hi, __m128i lo)
        ///   HELPER
        /// </summary>
        public static Vector256<T> SetHighLow<T>(Vector128<T> hi, Vector128<T> lo) where T : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256i _mm256_setzero_si256 (void)
        ///   HELPER
        /// __m256 _mm256_setzero_ps (void)
        ///   HELPER
        /// __m256d _mm256_setzero_pd (void)
        ///   HELPER
        /// </summary>
        public static Vector256<T> SetZeroVector256<T>() where T : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_shuffle_ps (__m256 a, __m256 b, const int imm8)
        ///   VSHUFPS ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<float> Shuffle(Vector256<float> value, Vector256<float> right, byte control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_shuffle_pd (__m256d a, __m256d b, const int imm8)
        ///   VSHUFPD ymm, ymm, ymm/m256, imm8
        /// </summary>
        public static Vector256<double> Shuffle(Vector256<double> value, Vector256<double> right, byte control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_sqrt_ps (__m256 a)
        ///   VSQRTPS ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Sqrt(Vector256<float> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_sqrt_pd (__m256d a)
        ///   VSQRTPD ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Sqrt(Vector256<double> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_castpd_ps (__m256d a)
        ///   HELPER - No Codegen
        /// __m256i _mm256_castpd_si256 (__m256d a)
        ///   HELPER - No Codegen
        /// __m256d _mm256_castps_pd (__m256 a)
        ///   HELPER - No Codegen
        /// __m256i _mm256_castps_si256 (__m256 a)
        ///   HELPER - No Codegen
        /// __m256d _mm256_castsi256_pd (__m256i a)
        ///   HELPER - No Codegen
        /// __m256 _mm256_castsi256_ps (__m256i a)
        ///   HELPER - No Codegen
        /// </summary>
        public static Vector256<U> StaticCast<T, U>(Vector256<T> value) where T : struct where U : struct { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(sbyte* address, Vector256<sbyte> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(byte* address, Vector256<byte> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(short* address, Vector256<short> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(ushort* address, Vector256<ushort> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(int* address, Vector256<int> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(uint* address, Vector256<uint> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(long* address, Vector256<long> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQA m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(ulong* address, Vector256<ulong> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_ps (float * mem_addr, __m256 a)
        ///   VMOVAPS m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(float* address, Vector256<float> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_store_pd (double * mem_addr, __m256d a)
        ///   VMOVAPD m256, ymm
        /// </summary>
        public static unsafe void StoreAligned(double* address, Vector256<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(sbyte* address, Vector256<sbyte> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(byte* address, Vector256<byte> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(short* address, Vector256<short> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(ushort* address, Vector256<ushort> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(int* address, Vector256<int> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(uint* address, Vector256<uint> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(long* address, Vector256<long> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_si256 (__m256i * mem_addr, __m256i a)
        ///   VMOVNTDQ m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(ulong* address, Vector256<ulong> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_ps (float * mem_addr, __m256 a)
        ///   MOVNTPS m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(float* address, Vector256<float> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_stream_pd (double * mem_addr, __m256d a)
        ///   MOVNTPD m256, ymm
        /// </summary>
        public static unsafe void StoreAlignedNonTemporal(double* address, Vector256<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(sbyte* address, Vector256<sbyte> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(byte* address, Vector256<byte> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(short* address, Vector256<short> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(ushort* address, Vector256<ushort> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(int* address, Vector256<int> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(uint* address, Vector256<uint> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(long* address, Vector256<long> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_si256 (__m256i * mem_addr, __m256i a)
        ///   MOVDQU m256, ymm
        /// </summary>
        public static unsafe void Store(ulong* address, Vector256<ulong> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_ps (float * mem_addr, __m256 a)
        ///   MOVUPS m256, ymm
        /// </summary>
        public static unsafe void Store(float* address, Vector256<float> source) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// void _mm256_storeu_pd (double * mem_addr, __m256d a)
        ///   MOVUPD m256, ymm
        /// </summary>
        public static unsafe void Store(double* address, Vector256<double> source) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_sub_ps (__m256 a, __m256 b)
        ///   VSUBPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Subtract(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_sub_pd (__m256d a, __m256d b)
        ///   VSUBPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Subtract(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm_testc_ps (__m128 a, __m128 b)
        ///   VTESTPS xmm, xmm/m128
        /// </summary>
        public static bool TestC(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// int _mm_testc_pd (__m128d a, __m128d b)
        ///   VTESTPD xmm, xmm/m128
        /// </summary>
        public static bool TestC(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<byte> left, Vector256<byte> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<sbyte> left, Vector256<sbyte> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<short> left, Vector256<short> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<ushort> left, Vector256<ushort> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<int> left, Vector256<int> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<uint> left, Vector256<uint> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<long> left, Vector256<long> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<ulong> left, Vector256<ulong> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_ps (__m256 a, __m256 b)
        ///   VTESTPS ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testc_pd (__m256d a, __m256d b)
        ///   VTESTPS ymm, ymm/m256
        /// </summary>
        public static bool TestC(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm_testnzc_ps (__m128 a, __m128 b)
        ///   VTESTPS xmm, xmm/m128
        /// </summary>
        public static bool TestNotZAndNotC(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// int _mm_testnzc_pd (__m128d a, __m128d b)
        ///   VTESTPD xmm, xmm/m128
        /// </summary>
        public static bool TestNotZAndNotC(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<byte> left, Vector256<byte> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<sbyte> left, Vector256<sbyte> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<short> left, Vector256<short> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<ushort> left, Vector256<ushort> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<int> left, Vector256<int> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<uint> left, Vector256<uint> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<long> left, Vector256<long> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<ulong> left, Vector256<ulong> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_ps (__m256 a, __m256 b)
        ///   VTESTPS ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testnzc_pd (__m256d a, __m256d b)
        ///   VTESTPD ymm, ymm/m256
        /// </summary>
        public static bool TestNotZAndNotC(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm_testz_ps (__m128 a, __m128 b)
        ///   VTESTPS xmm, xmm/m128
        /// </summary>
        public static bool TestZ(Vector128<float> left, Vector128<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// int _mm_testz_pd (__m128d a, __m128d b)
        ///   VTESTPD xmm, xmm/m128
        /// </summary>
        public static bool TestZ(Vector128<double> left, Vector128<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<byte> left, Vector256<byte> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<sbyte> left, Vector256<sbyte> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<short> left, Vector256<short> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<ushort> left, Vector256<ushort> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<int> left, Vector256<int> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<uint> left, Vector256<uint> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<long> left, Vector256<long> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_si256 (__m256i a, __m256i b)
        ///   VPTEST ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<ulong> left, Vector256<ulong> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_ps (__m256 a, __m256 b)
        ///   VTESTPS ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm256_testz_pd (__m256d a, __m256d b)
        ///   VTESTPD ymm, ymm/m256
        /// </summary>
        public static bool TestZ(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_unpackhi_ps (__m256 a, __m256 b)
        ///   VUNPCKHPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> UnpackHigh(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_unpackhi_pd (__m256d a, __m256d b)
        ///   VUNPCKHPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> UnpackHigh(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_unpacklo_ps (__m256 a, __m256 b)
        ///   VUNPCKLPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> UnpackLow(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_unpacklo_pd (__m256d a, __m256d b)
        ///   VUNPCKLPD ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> UnpackLow(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m256 _mm256_xor_ps (__m256 a, __m256 b)
        ///   VXORPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<float> Xor(Vector256<float> left, Vector256<float> right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __m256d _mm256_xor_pd (__m256d a, __m256d b)
        ///   VXORPS ymm, ymm, ymm/m256
        /// </summary>
        public static Vector256<double> Xor(Vector256<double> left, Vector256<double> right) { throw new PlatformNotSupportedException(); }
    }
}
