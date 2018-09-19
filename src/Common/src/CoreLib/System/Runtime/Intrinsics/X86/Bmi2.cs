// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel BMI2 hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Bmi2
    {
        internal Bmi2() { }

        public static bool IsSupported { get => IsSupported; }

        /// <summary>
        /// unsigned int _bzhi_u32 (unsigned int a, unsigned int index)
        ///   BZHI r32a, reg/m32, r32b
        /// </summary>
        public static uint ZeroHighBits(uint value, uint index) => ZeroHighBits(value, index);
        /// <summary>
        /// unsigned __int64 _bzhi_u64 (unsigned __int64 a, unsigned int index)
        ///   BZHI r64a, reg/m32, r64b
        /// </summary>
        public static ulong ZeroHighBits(ulong value, ulong index) => ZeroHighBits(value, index);

        /// <summary>
        /// unsigned int _mulx_u32 (unsigned int a, unsigned int b, unsigned int* hi)
        ///   MULX r32a, r32b, reg/m32
        /// </summary>
        public static unsafe uint MultiplyNoFlags(uint left, uint right, uint* high) => MultiplyNoFlags(left, right, high);
        /// <summary>
        /// unsigned __int64 _mulx_u64 (unsigned __int64 a, unsigned __int64 b, unsigned __int64* hi)
        ///   MULX r64a, r64b, reg/m64
        /// </summary>
        public static unsafe ulong MultiplyNoFlags(ulong left, ulong right, ulong* high) => MultiplyNoFlags(left, right, high);

        /// <summary>
        /// unsigned int _pdep_u32 (unsigned int a, unsigned int mask)
        ///   PDEP r32a, r32b, reg/m32
        /// </summary>
        public static uint ParallelBitDeposit(uint value, uint mask) => ParallelBitDeposit(value, mask);
        /// <summary>
        /// unsigned __int64 _pdep_u64 (unsigned __int64 a, unsigned __int64 mask)
        ///   PDEP r64a, r64b, reg/m64
        /// </summary>
        public static ulong ParallelBitDeposit(ulong value, ulong mask) => ParallelBitDeposit(value, mask);

        /// <summary>
        /// unsigned int _pext_u32 (unsigned int a, unsigned int mask)
        ///   PEXT r32a, r32b, reg/m32
        /// </summary>
        public static uint ParallelBitExtract(uint value, uint mask) => ParallelBitExtract(value, mask);
        /// <summary>
        /// unsigned __int64 _pext_u64 (unsigned __int64 a, unsigned __int64 mask)
        ///   PEXT r64a, r64b, reg/m64
        /// </summary>
        public static ulong ParallelBitExtract(ulong value, ulong mask) => ParallelBitExtract(value, mask);
    }
}
