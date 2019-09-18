// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
    internal partial class SafePasswordHandle
    {
        internal int Length { get; private set; }

        private IntPtr CreateHandle(string password)
        {
            Length = password?.Length ?? 0;
            return Marshal.StringToHGlobalUni(password);
        }

        private IntPtr CreateHandle(SecureString password)
        {
            Length = password?.Length ?? 0;
            return Marshal.SecureStringToGlobalAllocUnicode(password);
        }

        private void FreeHandle()
        {
            Marshal.ZeroFreeGlobalAllocUnicode(handle);
        }
    }
}
