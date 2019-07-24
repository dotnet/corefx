// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum CombineMode : int
        {
            RGN_AND = 1,
            RGN_XOR = 3,
            RGN_DIFF = 4,
        }

        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern RegionType CombineRgn(IntPtr hRgn, IntPtr hRgn1, IntPtr hRgn2, CombineMode nCombineMode);

        public static RegionType CombineRgn(HandleRef hRgn, HandleRef hRgn1, HandleRef hRgn2, CombineMode nCombineMode)
        {
            RegionType result = CombineRgn(hRgn.Handle, hRgn1.Handle, hRgn2.Handle, nCombineMode);
            GC.KeepAlive(hRgn.Wrapper);
            GC.KeepAlive(hRgn1.Wrapper);
            GC.KeepAlive(hRgn2.Wrapper);
            return result;
        }
    }
}
