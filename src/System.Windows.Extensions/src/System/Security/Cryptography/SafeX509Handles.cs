// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography
{
    internal sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {

        private SafeCertContextHandle() : base(true) { }

        // 0 is an Invalid Handle
        internal SafeCertContextHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        internal static SafeCertContextHandle InvalidHandle
        {
            get { return new SafeCertContextHandle(IntPtr.Zero); }
        }

        [DllImport(CAPI.CRYPT32, SetLastError = true)]
        private static extern bool CertFreeCertificateContext(IntPtr pCertContext);

        override protected bool ReleaseHandle()
        {
            return CertFreeCertificateContext(handle);
        }
    }

    internal sealed class SafeCertStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertStoreHandle() : base(true) { }

        // 0 is an Invalid Handle
        internal SafeCertStoreHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        internal static SafeCertStoreHandle InvalidHandle
        {
            get { return new SafeCertStoreHandle(IntPtr.Zero); }
        }

        [DllImport(CAPI.CRYPT32, SetLastError = true)]
        private static extern bool CertCloseStore(IntPtr hCertStore, uint dwFlags);

        override protected bool ReleaseHandle()
        {
            return CertCloseStore(handle, 0);
        }
    }
}
