// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Xunit;

namespace System.Net.Tests
{
    internal static class CertificateConfiguration
    {
        private const string CertificatePassword = "testcertificate";
        private const string TestDataFolder = "TestData";

        public static X509Certificate2 GetServerCertificate() => GetCertWithPrivateKey(GetServerCertificateCollection());

        public static X509Certificate2 GetClientCertificate() => GetCertWithPrivateKey(GetClientCertificateCollection());

        public static X509Certificate2Collection GetServerCertificateCollection() => GetCertificateCollection("contoso.com.pfx");

        public static X509Certificate2Collection GetClientCertificateCollection() => GetCertificateCollection("testclient1_at_contoso.com.pfx");

        private static X509Certificate2Collection GetCertificateCollection(string certificateFileName)
        {
            var certCollection = new X509Certificate2Collection();
            certCollection.Import(Path.Combine(TestDataFolder, certificateFileName), CertificatePassword, X509KeyStorageFlags.DefaultKeySet);
            return certCollection;
        }

        private static X509Certificate2 GetCertWithPrivateKey(X509Certificate2Collection certCollection)
        {
            X509Certificate2 certificate = null;

            foreach (X509Certificate2 c in certCollection)
            {
                if (certificate == null && c.HasPrivateKey)
                {
                    certificate = c;
                }
                else
                {
                    c.Dispose();
                }
            }

            Assert.NotNull(certificate);
            return certificate;
        }
    }
}
