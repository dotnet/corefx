// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Wrapper around a ntlm_type2*
    /// </summary>
    internal sealed class SafeNtlmType2Handle : SafeHandle
    {
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurityNative.HeimNtlmFreeType2(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        private SafeNtlmType2Handle() : base(IntPtr.Zero, true)
        {
        }
    }
}
