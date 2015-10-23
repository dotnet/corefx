// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Sys
    {
        /// <summary>
        /// Get the name of the current system. 
        /// </summary>
        /// <param name="machine">The OS architecture value.</param>
        /// <returns>
        /// Returns non-negative value on success, -1 on failure.
        /// </returns>
        [DllImport(Libraries.SystemNative, CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int UName([Out] StringBuilder machine, int capacity);

        internal static int UName(out string machine)
        {
            int maxBufferLength = 1024;
            StringBuilder buffer = new StringBuilder(maxBufferLength);
            int res = UName(buffer, buffer.Capacity);
            machine = buffer.ToString();

            return res;
        }
    }
}
