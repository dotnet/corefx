// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetHostName", SetLastError = true)]
        private unsafe static extern int GetHostName(byte* name, int nameLength);

        internal static unsafe string GetHostName()
        {
            const int HOST_NAME_MAX = 255;
            const int ArrLength = HOST_NAME_MAX + 1;

            byte* name = stackalloc byte[ArrLength];
            int err = GetHostName(name, ArrLength);
            if (err != 0)
            {
                // This should never happen.  According to the man page,
                // the only possible errno for gethostname is ENAMETOOLONG,
                // which should only happen if the buffer we supply isn't big
                // enough, and we're using a buffer size that the man page
                // says is the max for POSIX (and larger than the max for Linux).
                Debug.Fail("gethostname failed");
                throw new InvalidOperationException(string.Format("gethostname returned {0}", err));
            }

            return Marshal.PtrToStringAnsi((IntPtr)name);
        }
    }
}
