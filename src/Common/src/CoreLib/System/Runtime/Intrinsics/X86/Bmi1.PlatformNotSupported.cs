// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel BMI1 hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Bmi1
    {
        internal Bmi1() { }

        public static bool IsSupported { get { return false; } }

        /// <summary>
        /// unsigned int _andn_u32 (unsigned int a, unsigned int b)
        ///   ANDN r32a, r32b, reg/m32
        /// </summary>
        public static uint AndNot(uint left, uint right) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _andn_u64 (unsigned __int64 a, unsigned __int64 b)
        ///   ANDN r64a, r64b, reg/m64
        /// </summary>
        public static ulong AndNot(ulong left, ulong right) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// unsigned int _bextr_u32 (unsigned int a, unsigned int start, unsigned int len)
        ///   BEXTR r32a, reg/m32, r32b
        /// </summary>
        public static uint BitFieldExtract(uint value, byte start, byte length) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _bextr_u64 (unsigned __int64 a, unsigned int start, unsigned int len)
        ///   BEXTR r64a, reg/m64, r64b
        /// </summary>
        public static ulong BitFieldExtract(ulong value, byte start, byte length) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned int _bextr2_u32 (unsigned int a, unsigned int control)
        ///   BEXTR r32a, reg/m32, r32b
        /// </summary>
        public static uint BitFieldExtract(uint value, ushort control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _bextr2_u64 (unsigned __int64 a, unsigned __int64 control)
        ///   BEXTR r64a, reg/m64, r64b
        /// </summary>
        public static ulong BitFieldExtract(ulong value, ushort control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// unsigned int _blsi_u32 (unsigned int a)
        ///   BLSI reg, reg/m32
        /// </summary>
        public static uint ExtractLowestSetBit(uint value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _blsi_u64 (unsigned __int64 a)
        ///   BLSI reg, reg/m64
        /// </summary>
        public static ulong ExtractLowestSetBit(ulong value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// unsigned int _blsmsk_u32 (unsigned int a)
        ///   BLSMSK reg, reg/m32
        /// </summary>
        public static uint GetMaskUpToLowestSetBit(uint value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _blsmsk_u64 (unsigned __int64 a)
        ///   BLSMSK reg, reg/m64
        /// </summary>
        public static ulong GetMaskUpToLowestSetBit(ulong value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// unsigned int _blsr_u32 (unsigned int a)
        ///   BLSR reg, reg/m32
        /// </summary>
        public static uint ResetLowestSetBit(uint value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _blsr_u64 (unsigned __int64 a)
        ///   BLSR reg, reg/m64
        /// </summary>
        public static ulong ResetLowestSetBit(ulong value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// int _mm_tzcnt_32 (unsigned int a)
        ///   TZCNT reg, reg/m32
        /// </summary>
        public static uint TrailingZeroCount(uint value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// __int64 _mm_tzcnt_64 (unsigned __int64 a)
        ///   TZCNT reg, reg/m64
        /// </summary>
        public static ulong TrailingZeroCount(ulong value) { throw new PlatformNotSupportedException(); }
    }
}
