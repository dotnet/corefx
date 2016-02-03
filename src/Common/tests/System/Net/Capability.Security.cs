// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net.Test.Common
{
    public static partial class Capability
    {
        // Thumbprint for CN = NDX Test Root CA.
        // The certificate is part of the chain at
        // https://github.com/dotnet/corefx-testdata/blob/master/System.Net.TestData/contoso.com.p7b
        private const string CARootThumbprint = "3B279AD43D6DD459268D3F3A3D72DAAD4BF4D9C6";

        private static Lazy<bool> s_trustedCertificateSupport =
            new Lazy<bool>(InitializeTrustedRootCertificateCapability, LazyThreadSafetyMode.ExecutionAndPublication);

        public static bool IsTrustedRootCertificateInstalled()
        {
            return s_trustedCertificateSupport.Value;
        }

        private static bool InitializeTrustedRootCertificateCapability()
        {
            using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certs =
                    store.Certificates.Find(X509FindType.FindByThumbprint, CARootThumbprint, false);

                if (certs.Count == 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
