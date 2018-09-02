// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class Advapi32
    {
        [DllImport(Libraries.Advapi32, EntryPoint = "GetServiceDisplayNameW", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool GetServiceDisplayNamePrivate(SafeServiceHandle SCMHandle, string serviceName, ref char displayName, ref int displayNameLength);

        public static unsafe string GetServiceDisplayName(SafeServiceHandle SCMHandle, string serviceName, bool throwOnError)
        {
            var builder = new ValueStringBuilder(4096);
            int bufLen;
            while (true)
            {
                bufLen = builder.Capacity;
                if (GetServiceDisplayNamePrivate(SCMHandle, serviceName, ref builder.GetPinnableReference(), ref bufLen))
                    break;

                int lastError = Marshal.GetLastWin32Error();
                if (lastError != Interop.Errors.ERROR_INSUFFICIENT_BUFFER)
                {
                    if (throwOnError)
                        throw new Win32Exception(lastError);

                    return null; // Caller may want to try something else
                }

                builder.EnsureCapacity(bufLen + 1); // Does not include null
            }

            builder.Length = bufLen;
            return builder.ToString();
        }
    }
}
