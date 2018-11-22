// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics
{
    public static class Vector256
    {
        internal const int Size = 32;

        /// <summary>Creates a new <see cref="Vector256{byte}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{byte}" /> with all elements initialized to <paramref name="value" />.</returns>
        public static unsafe Vector256<byte> Create(byte value)
        {
            var pResult = stackalloc byte[32]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<byte>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{double}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{double}" /> with all elements initialized to <paramref name="value" />.</returns>
        public static unsafe Vector256<double> Create(double value)
        {
            var pResult = stackalloc double[4]
            {
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<double>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{short}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{short}" /> with all elements initialized to <paramref name="value" />.</returns>
        public static unsafe Vector256<short> Create(short value)
        {
            var pResult = stackalloc short[16]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<short>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{int}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{int}" /> with all elements initialized to <paramref name="value" />.</returns>
        public static unsafe Vector256<int> Create(int value)
        {
            var pResult = stackalloc int[8]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<int>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{long}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{long}" /> with all elements initialized to <paramref name="value" />.</returns>
        public static unsafe Vector256<long> Create(long value)
        {
            var pResult = stackalloc long[4]
            {
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<long>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{sbyte}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{sbyte}" /> with all elements initialized to <paramref name="value" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<sbyte> Create(sbyte value)
        {
            var pResult = stackalloc sbyte[32]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<sbyte>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{float}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{float}" /> with all elements initialized to <paramref name="value" />.</returns>
        public static unsafe Vector256<float> Create(float value)
        {
            var pResult = stackalloc float[8]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<float>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{ushort}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ushort}" /> with all elements initialized to <paramref name="value" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ushort> Create(ushort value)
        {
            var pResult = stackalloc ushort[16]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<ushort>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{uint}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{uint}" /> with all elements initialized to <paramref name="value" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<uint> Create(uint value)
        {
            var pResult = stackalloc uint[8]
            {
                value,
                value,
                value,
                value,
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<uint>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{ulong}" /> instance with all elements initialized to the specified value.</summary>
        /// <param name="value">The value that all elements will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ulong}" /> with all elements initialized to <paramref name="value" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ulong> Create(ulong value)
        {
            var pResult = stackalloc ulong[4]
            {
                value,
                value,
                value,
                value,
            };

            return Unsafe.AsRef<Vector256<ulong>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{byte}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <param name="e8">The value that element 8 will be initialized to.</param>
        /// <param name="e9">The value that element 9 will be initialized to.</param>
        /// <param name="e10">The value that element 10 will be initialized to.</param>
        /// <param name="e11">The value that element 11 will be initialized to.</param>
        /// <param name="e12">The value that element 12 will be initialized to.</param>
        /// <param name="e13">The value that element 13 will be initialized to.</param>
        /// <param name="e14">The value that element 14 will be initialized to.</param>
        /// <param name="e15">The value that element 15 will be initialized to.</param>
        /// <param name="e16">The value that element 16 will be initialized to.</param>
        /// <param name="e17">The value that element 17 will be initialized to.</param>
        /// <param name="e18">The value that element 18 will be initialized to.</param>
        /// <param name="e19">The value that element 19 will be initialized to.</param>
        /// <param name="e20">The value that element 20 will be initialized to.</param>
        /// <param name="e21">The value that element 21 will be initialized to.</param>
        /// <param name="e22">The value that element 22 will be initialized to.</param>
        /// <param name="e23">The value that element 23 will be initialized to.</param>
        /// <param name="e24">The value that element 24 will be initialized to.</param>
        /// <param name="e25">The value that element 25 will be initialized to.</param>
        /// <param name="e26">The value that element 26 will be initialized to.</param>
        /// <param name="e27">The value that element 27 will be initialized to.</param>
        /// <param name="e28">The value that element 28 will be initialized to.</param>
        /// <param name="e29">The value that element 29 will be initialized to.</param>
        /// <param name="e30">The value that element 30 will be initialized to.</param>
        /// <param name="e31">The value that element 31 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{byte}" /> with each element initialized to corresponding specified value.</returns>
        public static unsafe Vector256<byte> Create(byte e0, byte e1, byte e2, byte e3, byte e4, byte e5, byte e6, byte e7, byte e8, byte e9, byte e10, byte e11, byte e12, byte e13, byte e14, byte e15, byte e16, byte e17, byte e18, byte e19, byte e20, byte e21, byte e22, byte e23, byte e24, byte e25, byte e26, byte e27, byte e28, byte e29, byte e30, byte e31)
        {
            var pResult = stackalloc byte[32]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
                e8,
                e9,
                e10,
                e11,
                e12,
                e13,
                e14,
                e15,
                e16,
                e17,
                e18,
                e19,
                e20,
                e21,
                e22,
                e23,
                e24,
                e25,
                e26,
                e27,
                e28,
                e29,
                e30,
                e31,
            };

            return Unsafe.AsRef<Vector256<byte>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{double}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{double}" /> with each element initialized to corresponding specified value.</returns>
        public static unsafe Vector256<double> Create(double e0, double e1, double e2, double e3)
        {
            var pResult = stackalloc double[4]
            {
                e0,
                e1,
                e2,
                e3,
            };

            return Unsafe.AsRef<Vector256<double>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{short}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <param name="e8">The value that element 8 will be initialized to.</param>
        /// <param name="e9">The value that element 9 will be initialized to.</param>
        /// <param name="e10">The value that element 10 will be initialized to.</param>
        /// <param name="e11">The value that element 11 will be initialized to.</param>
        /// <param name="e12">The value that element 12 will be initialized to.</param>
        /// <param name="e13">The value that element 13 will be initialized to.</param>
        /// <param name="e14">The value that element 14 will be initialized to.</param>
        /// <param name="e15">The value that element 15 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{short}" /> with each element initialized to corresponding specified value.</returns>
        public static unsafe Vector256<short> Create(short e0, short e1, short e2, short e3, short e4, short e5, short e6, short e7, short e8, short e9, short e10, short e11, short e12, short e13, short e14, short e15)
        {
            var pResult = stackalloc short[16]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
                e8,
                e9,
                e10,
                e11,
                e12,
                e13,
                e14,
                e15,
            };

            return Unsafe.AsRef<Vector256<short>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{int}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{int}" /> with each element initialized to corresponding specified value.</returns>
        public static unsafe Vector256<int> Create(int e0, int e1, int e2, int e3, int e4, int e5, int e6, int e7)
        {
            var pResult = stackalloc int[8]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
            };

            return Unsafe.AsRef<Vector256<int>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{long}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{long}" /> with each element initialized to corresponding specified value.</returns>
        public static unsafe Vector256<long> Create(long e0, long e1, long e2, long e3)
        {
            var pResult = stackalloc long[4]
            {
                e0,
                e1,
                e2,
                e3,
            };

            return Unsafe.AsRef<Vector256<long>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{sbyte}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <param name="e8">The value that element 8 will be initialized to.</param>
        /// <param name="e9">The value that element 9 will be initialized to.</param>
        /// <param name="e10">The value that element 10 will be initialized to.</param>
        /// <param name="e11">The value that element 11 will be initialized to.</param>
        /// <param name="e12">The value that element 12 will be initialized to.</param>
        /// <param name="e13">The value that element 13 will be initialized to.</param>
        /// <param name="e14">The value that element 14 will be initialized to.</param>
        /// <param name="e15">The value that element 15 will be initialized to.</param>
        /// <param name="e16">The value that element 16 will be initialized to.</param>
        /// <param name="e17">The value that element 17 will be initialized to.</param>
        /// <param name="e18">The value that element 18 will be initialized to.</param>
        /// <param name="e19">The value that element 19 will be initialized to.</param>
        /// <param name="e20">The value that element 20 will be initialized to.</param>
        /// <param name="e21">The value that element 21 will be initialized to.</param>
        /// <param name="e22">The value that element 22 will be initialized to.</param>
        /// <param name="e23">The value that element 23 will be initialized to.</param>
        /// <param name="e24">The value that element 24 will be initialized to.</param>
        /// <param name="e25">The value that element 25 will be initialized to.</param>
        /// <param name="e26">The value that element 26 will be initialized to.</param>
        /// <param name="e27">The value that element 27 will be initialized to.</param>
        /// <param name="e28">The value that element 28 will be initialized to.</param>
        /// <param name="e29">The value that element 29 will be initialized to.</param>
        /// <param name="e30">The value that element 30 will be initialized to.</param>
        /// <param name="e31">The value that element 31 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{sbyte}" /> with each element initialized to corresponding specified value.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<sbyte> Create(sbyte e0, sbyte e1, sbyte e2, sbyte e3, sbyte e4, sbyte e5, sbyte e6, sbyte e7, sbyte e8, sbyte e9, sbyte e10, sbyte e11, sbyte e12, sbyte e13, sbyte e14, sbyte e15, sbyte e16, sbyte e17, sbyte e18, sbyte e19, sbyte e20, sbyte e21, sbyte e22, sbyte e23, sbyte e24, sbyte e25, sbyte e26, sbyte e27, sbyte e28, sbyte e29, sbyte e30, sbyte e31)
        {
            var pResult = stackalloc sbyte[32]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
                e8,
                e9,
                e10,
                e11,
                e12,
                e13,
                e14,
                e15,
                e16,
                e17,
                e18,
                e19,
                e20,
                e21,
                e22,
                e23,
                e24,
                e25,
                e26,
                e27,
                e28,
                e29,
                e30,
                e31,
            };

            return Unsafe.AsRef<Vector256<sbyte>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{float}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{float}" /> with each element initialized to corresponding specified value.</returns>
        public static unsafe Vector256<float> Create(float e0, float e1, float e2, float e3, float e4, float e5, float e6, float e7)
        {
            var pResult = stackalloc float[8]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
            };

            return Unsafe.AsRef<Vector256<float>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{ushort}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <param name="e8">The value that element 8 will be initialized to.</param>
        /// <param name="e9">The value that element 9 will be initialized to.</param>
        /// <param name="e10">The value that element 10 will be initialized to.</param>
        /// <param name="e11">The value that element 11 will be initialized to.</param>
        /// <param name="e12">The value that element 12 will be initialized to.</param>
        /// <param name="e13">The value that element 13 will be initialized to.</param>
        /// <param name="e14">The value that element 14 will be initialized to.</param>
        /// <param name="e15">The value that element 15 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ushort}" /> with each element initialized to corresponding specified value.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ushort> Create(ushort e0, ushort e1, ushort e2, ushort e3, ushort e4, ushort e5, ushort e6, ushort e7, ushort e8, ushort e9, ushort e10, ushort e11, ushort e12, ushort e13, ushort e14, ushort e15)
        {
            var pResult = stackalloc ushort[16]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
                e8,
                e9,
                e10,
                e11,
                e12,
                e13,
                e14,
                e15,
            };

            return Unsafe.AsRef<Vector256<ushort>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{uint}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <param name="e4">The value that element 4 will be initialized to.</param>
        /// <param name="e5">The value that element 5 will be initialized to.</param>
        /// <param name="e6">The value that element 6 will be initialized to.</param>
        /// <param name="e7">The value that element 7 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{uint}" /> with each element initialized to corresponding specified value.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<uint> Create(uint e0, uint e1, uint e2, uint e3, uint e4, uint e5, uint e6, uint e7)
        {
            var pResult = stackalloc uint[8]
            {
                e0,
                e1,
                e2,
                e3,
                e4,
                e5,
                e6,
                e7,
            };

            return Unsafe.AsRef<Vector256<uint>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{ulong}" /> instance with each element initialized to the corresponding specified value.</summary>
        /// <param name="e0">The value that element 0 will be initialized to.</param>
        /// <param name="e1">The value that element 1 will be initialized to.</param>
        /// <param name="e2">The value that element 2 will be initialized to.</param>
        /// <param name="e3">The value that element 3 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ulong}" /> with each element initialized to corresponding specified value.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ulong> Create(ulong e0, ulong e1, ulong e2, ulong e3)
        {
            var pResult = stackalloc ulong[4]
            {
                e0,
                e1,
                e2,
                e3,
            };

            return Unsafe.AsRef<Vector256<ulong>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{byte}" /> instance from two <see cref="Vector128{byte}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{byte}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        public static unsafe Vector256<byte> Create(Vector128<byte> lower, Vector128<byte> upper)
        {
            Vector256<byte> result256 = Vector256<byte>.Zero;

            ref Vector128<byte> result128 = ref Unsafe.As<Vector256<byte>, Vector128<byte>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{double}" /> instance from two <see cref="Vector128{double}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{double}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        public static unsafe Vector256<double> Create(Vector128<double> lower, Vector128<double> upper)
        {
            Vector256<double> result256 = Vector256<double>.Zero;

            ref Vector128<double> result128 = ref Unsafe.As<Vector256<double>, Vector128<double>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{short}" /> instance from two <see cref="Vector128{short}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{short}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        public static unsafe Vector256<short> Create(Vector128<short> lower, Vector128<short> upper)
        {
            Vector256<short> result256 = Vector256<short>.Zero;

            ref Vector128<short> result128 = ref Unsafe.As<Vector256<short>, Vector128<short>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{int}" /> instance from two <see cref="Vector128{int}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{int}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        public static unsafe Vector256<int> Create(Vector128<int> lower, Vector128<int> upper)
        {
            Vector256<int> result256 = Vector256<int>.Zero;

            ref Vector128<int> result128 = ref Unsafe.As<Vector256<int>, Vector128<int>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{long}" /> instance from two <see cref="Vector128{long}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{long}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        public static unsafe Vector256<long> Create(Vector128<long> lower, Vector128<long> upper)
        {
            Vector256<long> result256 = Vector256<long>.Zero;

            ref Vector128<long> result128 = ref Unsafe.As<Vector256<long>, Vector128<long>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{sbyte}" /> instance from two <see cref="Vector128{sbyte}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{sbyte}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<sbyte> Create(Vector128<sbyte> lower, Vector128<sbyte> upper)
        {
            Vector256<sbyte> result256 = Vector256<sbyte>.Zero;

            ref Vector128<sbyte> result128 = ref Unsafe.As<Vector256<sbyte>, Vector128<sbyte>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{float}" /> instance from two <see cref="Vector128{float}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{float}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        public static unsafe Vector256<float> Create(Vector128<float> lower, Vector128<float> upper)
        {
            Vector256<float> result256 = Vector256<float>.Zero;

            ref Vector128<float> result128 = ref Unsafe.As<Vector256<float>, Vector128<float>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{ushort}" /> instance from two <see cref="Vector128{ushort}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ushort}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ushort> Create(Vector128<ushort> lower, Vector128<ushort> upper)
        {
            Vector256<ushort> result256 = Vector256<ushort>.Zero;

            ref Vector128<ushort> result128 = ref Unsafe.As<Vector256<ushort>, Vector128<ushort>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{uint}" /> instance from two <see cref="Vector128{uint}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{uint}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<uint> Create(Vector128<uint> lower, Vector128<uint> upper)
        {
            Vector256<uint> result256 = Vector256<uint>.Zero;

            ref Vector128<uint> result128 = ref Unsafe.As<Vector256<uint>, Vector128<uint>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{ulong}" /> instance from two <see cref="Vector128{ulong}" /> instances.</summary>
        /// <param name="lower">The value that the lower 128-bits will be initialized to.</param>
        /// <param name="upper">The value that the upper 128-bits will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ulong}" /> initialized from <paramref name="lower" /> and <paramref name="upper" />.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ulong> Create(Vector128<ulong> lower, Vector128<ulong> upper)
        {
            Vector256<ulong> result256 = Vector256<ulong>.Zero;

            ref Vector128<ulong> result128 = ref Unsafe.As<Vector256<ulong>, Vector128<ulong>>(ref result256);
            result128 = lower;
            Unsafe.Add(ref result128, 1) = upper;

            return result256;
        }

        /// <summary>Creates a new <see cref="Vector256{byte}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{byte}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        public static unsafe Vector256<byte> CreateScalar(byte value)
        {
            var result = Vector256<byte>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<byte>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{double}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{double}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        public static unsafe Vector256<double> CreateScalar(double value)
        {
            var result = Vector256<double>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<double>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{short}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{short}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        public static unsafe Vector256<short> CreateScalar(short value)
        {
            var result = Vector256<short>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<short>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{int}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{int}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        public static unsafe Vector256<int> CreateScalar(int value)
        {
            var result = Vector256<int>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<int>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{long}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{long}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        public static unsafe Vector256<long> CreateScalar(long value)
        {
            var result = Vector256<long>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<long>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{sbyte}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{sbyte}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<sbyte> CreateScalar(sbyte value)
        {
            var result = Vector256<sbyte>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<sbyte>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{float}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{float}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        public static unsafe Vector256<float> CreateScalar(float value)
        {
            var result = Vector256<float>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<float>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{ushort}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ushort}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ushort> CreateScalar(ushort value)
        {
            var result = Vector256<ushort>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<ushort>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{uint}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{uint}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<uint> CreateScalar(uint value)
        {
            var result = Vector256<uint>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<uint>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{ulong}" /> instance with the first element initialized to the specified value and the remaining elements initialized to zero.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ulong}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements initialized to zero.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ulong> CreateScalar(ulong value)
        {
            var result = Vector256<ulong>.Zero;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<ulong>, byte>(ref result), value);
            return result;
        }

        /// <summary>Creates a new <see cref="Vector256{byte}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{byte}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        public static unsafe Vector256<byte> CreateScalarUnsafe(byte value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc byte[32];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<byte>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{double}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{double}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        public static unsafe Vector256<double> CreateScalarUnsafe(double value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc double[4];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<double>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{short}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{short}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        public static unsafe Vector256<short> CreateScalarUnsafe(short value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc short[16];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<short>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{int}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{int}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        public static unsafe Vector256<int> CreateScalarUnsafe(int value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc int[8];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<int>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{long}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{long}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        public static unsafe Vector256<long> CreateScalarUnsafe(long value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc long[4];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<long>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{sbyte}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{sbyte}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<sbyte> CreateScalarUnsafe(sbyte value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc sbyte[32];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<sbyte>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{float}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{float}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        public static unsafe Vector256<float> CreateScalarUnsafe(float value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc float[8];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<float>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{ushort}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ushort}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ushort> CreateScalarUnsafe(ushort value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc ushort[16];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<ushort>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{uint}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{uint}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<uint> CreateScalarUnsafe(uint value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc uint[8];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<uint>>(pResult);
        }

        /// <summary>Creates a new <see cref="Vector256{ulong}" /> instance with the first element initialized to the specified value and the remaining elements left uninitialized.</summary>
        /// <param name="value">The value that element 0 will be initialized to.</param>
        /// <returns>A new <see cref="Vector256{ulong}" /> instance with the first element initialized to <see cref="value" /> and the remaining elements left uninitialized.</returns>
        [CLSCompliant(false)]
        public static unsafe Vector256<ulong> CreateScalarUnsafe(ulong value)
        {
            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc ulong[4];
            pResult[0] = value;
            return Unsafe.AsRef<Vector256<ulong>>(pResult);
        }
    }
}
