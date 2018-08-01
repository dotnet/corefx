// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to Intel LZCNT hardware instructions via intrinsics
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Lzcnt
    {
        internal Lzcnt() { }

        public static bool IsSupported { get { return false; } }

        /// <summary>
        /// unsigned int _lzcnt_u32 (unsigned int a)
        ///   LZCNT reg, reg/m32
        /// </summary>
        public static uint LeadingZeroCount(uint value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        /// unsigned __int64 _lzcnt_u64 (unsigned __int64 a)
        ///   LZCNT reg, reg/m64
        /// </summary>
        public static ulong LeadingZeroCount(ulong value) { throw new PlatformNotSupportedException(); }
    }
}
