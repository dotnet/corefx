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
        private IntPtr CreateHandle(string password)
        {
            return Marshal.StringToHGlobalAnsi(password);
        }

        private IntPtr CreateHandle(SecureString password)
        {
            return Marshal.SecureStringToGlobalAllocAnsi(password);
        }

        private void FreeHandle()
        {
            Marshal.ZeroFreeGlobalAllocAnsi(handle);
        }
    }
}
