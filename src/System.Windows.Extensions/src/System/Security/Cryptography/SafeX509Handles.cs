// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
    internal sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // 0 is an Invalid Handle
        internal SafeCertContextHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        internal static SafeCertContextHandle InvalidHandle => new SafeCertContextHandle(IntPtr.Zero);

        [DllImport(Interop.Libraries.Crypt32, SetLastError = true)]
        private static extern bool CertFreeCertificateContext(IntPtr pCertContext);

        protected override bool ReleaseHandle() => CertFreeCertificateContext(handle);
    }

    internal sealed class SafeCertStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // 0 is an Invalid Handle
        internal SafeCertStoreHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        internal static SafeCertStoreHandle InvalidHandle => new SafeCertStoreHandle(IntPtr.Zero);

        [DllImport(Interop.Libraries.Crypt32, SetLastError = true)]
        private static extern bool CertCloseStore(IntPtr hCertStore, uint dwFlags);

        protected override bool ReleaseHandle() => CertCloseStore(handle, 0);
    }
}
