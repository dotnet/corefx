// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Advapi32
    {
        [DllImport(Libraries.Advapi32, EntryPoint = "GetServiceKeyNameW", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool GetServiceKeyNamePrivate(SafeServiceHandle SCMHandle, string displayName, char[] shortName, ref int shortNameLength);

        public static string GetServiceKeyName(SafeServiceHandle SCMHandle, string displayName)
        {
            // Get the size of buffer required
            int bufLen = 0;
            bool success = GetServiceKeyNamePrivate(SCMHandle, displayName, null, ref bufLen);
            char[] buffer = null;

            if (!success && Marshal.GetLastWin32Error() == Interop.Errors.ERROR_INSUFFICIENT_BUFFER)
            {
                bufLen++; // Does not include null
                buffer = ArrayPool<char>.Shared.Rent(bufLen);

                try
                {
                    success = GetServiceKeyNamePrivate(SCMHandle, displayName, buffer, ref bufLen);
                    if (success)
                        return new string(buffer, 0, bufLen);
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
            }

            return null;
        }
    }
}
