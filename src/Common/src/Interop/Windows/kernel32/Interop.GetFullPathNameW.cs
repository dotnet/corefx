// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use GetFullPathName or PathHelper.
        /// </summary>
        [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false, ExactSpelling = true)]
        private static extern uint GetFullPathNameW(string path, uint bufferLength, char[] buffer, IntPtr mustBeZero);

        internal static uint GetFullPathWraper(string path, uint bufferLength, char[] buffer) => GetFullPathNameW(path, bufferLength, buffer, IntPtr.Zero);
    }
}
