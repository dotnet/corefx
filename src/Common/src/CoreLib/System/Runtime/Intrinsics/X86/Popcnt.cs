// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel POPCNT hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Popcnt : Sse42
    {
        internal Popcnt() { }

        public new static bool IsSupported { get => IsSupported; }

        /// <summary>
        /// int _mm_popcnt_u32 (unsigned int a)
        ///   POPCNT reg, reg/m32
        /// </summary>
        public static int PopCount(uint value) => PopCount(value);
        /// <summary>
        /// __int64 _mm_popcnt_u64 (unsigned __int64 a)
        ///   POPCNT reg, reg/m64
        /// </summary>
        public static long PopCount(ulong value) => PopCount(value);
    }
}
