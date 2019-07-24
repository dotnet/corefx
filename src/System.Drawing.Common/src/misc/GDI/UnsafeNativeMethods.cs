// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Drawing.Internal
{
    internal static partial class IntUnsafeNativeMethods
    {
        /// <summary>
        /// NOTE: DeleteDC is to be used to delete the hdc created from CreateCompatibleDC ONLY. All other hdcs should
        /// be deleted with DeleteHDC.
        /// </summary>
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool DeleteDC(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateDC(string lpszDriverName, string lpszDeviceName, string lpszOutput, HandleRef /*DEVMODE*/ lpInitData);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateIC(string lpszDriverName, string lpszDeviceName, string lpszOutput, HandleRef /*DEVMODE*/ lpInitData);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateCompatibleDC(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern int SaveDC(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool RestoreDC(HandleRef hDC, int nSavedDC);

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr WindowFromDC(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "OffsetViewportOrgEx", CharSet = CharSet.Auto)]
        public static extern bool IntOffsetViewportOrgEx(HandleRef hDC, int nXOffset, int nYOffset, [In, Out] IntNativeMethods.POINT point);

        public static bool OffsetViewportOrgEx(HandleRef hDC, int nXOffset, int nYOffset, [In, Out] IntNativeMethods.POINT point)
        {
            bool retVal = IntOffsetViewportOrgEx(hDC, nXOffset, nYOffset, point);
            DbgUtil.AssertWin32(retVal, "OffsetViewportOrgEx([hdc=0x{0:X8}], dx=[{1}], dy=[{2}], [out pPoint]) failed.", hDC.Handle, nXOffset, nYOffset);
            return retVal;
        }
    }
}
