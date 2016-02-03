// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Xunit;

namespace System.Net.Security.Tests
{
    internal static class TestConfiguration
    {
        public const int PassingTestTimeoutMilliseconds = 15 * 1000;
        public const int FailingTestTimeoutMiliseconds = 250;

        private const string CertificatePassword = "testcertificate";
        private const string TestDataFolder = "TestData";

        public static X509Certificate2 GetServerCertificate()
        {
            X509Certificate2Collection certCollection = TestConfiguration.GetServerCertificateCollection();
            return GetCertWithPrivateKey(certCollection);
        }

        public static X509Certificate2Collection GetServerCertificateCollection()
        {
            return GetCertificateCollection("contoso.com.pfx");
        }

        public static X509Certificate2 GetClientCertificate()
        {
            X509Certificate2Collection certCollection = TestConfiguration.GetClientCertificateCollection();
            return GetCertWithPrivateKey(certCollection);
        }

        public static X509Certificate2Collection GetClientCertificateCollection()
        {
            return GetCertificateCollection("testclient1_at_contoso.com.pfx");
        }

        private static X509Certificate2Collection GetCertificateCollection(string certificateFileName)
        {
            var certCollection = new X509Certificate2Collection();
            certCollection.Import(
                Path.Combine(TestDataFolder, certificateFileName),
                CertificatePassword,
                X509KeyStorageFlags.DefaultKeySet);

            return certCollection;
        }

        private static X509Certificate2 GetCertWithPrivateKey(X509Certificate2Collection certCollection)
        {
            X509Certificate2 certificate = null;

            foreach (X509Certificate2 c in certCollection)
            {
                if (c.HasPrivateKey)
                {
                    certificate = c;
                    break;
                }
            }

            Assert.NotNull(certificate);
            return certificate;
        }
    }
}
