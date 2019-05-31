// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace System.Net
{
    internal static unsafe partial class UnmanagedCertificateContext
    {
        internal static X509Certificate2Collection GetRemoteCertificatesFromStoreContext(IntPtr certContext)
        {
            X509Certificate2Collection result = new X509Certificate2Collection();

            if (certContext == IntPtr.Zero)
            {
                return result;
            }

            Interop.Crypt32.CERT_CONTEXT context;
            unsafe
            {
                context = *(Interop.Crypt32.CERT_CONTEXT*)certContext;
            }

            if (context.hCertStore != IntPtr.Zero)
            {
                Interop.Crypt32.CERT_CONTEXT* last = null;

                while (true)
                {
                    Interop.Crypt32.CERT_CONTEXT* next =
                        Interop.Crypt32.CertEnumCertificatesInStore(context.hCertStore, last);

                    if (next == null)
                    {
                        break;
                    }

                    var cert = new X509Certificate2(new IntPtr(next));
                    if (NetEventSource.IsEnabled) NetEventSource.Info(certContext, $"Adding remote certificate:{cert}");

                    result.Add(cert);
                    last = next;
                }
            }

            return result;
        }        
    }
}
