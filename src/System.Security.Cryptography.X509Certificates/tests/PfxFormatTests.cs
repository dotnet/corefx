// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Security.Cryptography.Pkcs;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public abstract class PfxFormatTests
    {
        // Use SHA-1 for Windows 7-8.1 support.
        private static readonly HashAlgorithmName s_digestAlgorithm = HashAlgorithmName.SHA1;

        // The PBE parameters used by Windows 7 for private keys.
        // Needs to stay 3DES/SHA1 for Windows 7 to read it.
        private static readonly PbeParameters s_windowsPbe =
            new PbeParameters(PbeEncryptionAlgorithm.TripleDes3KeyPkcs12, HashAlgorithmName.SHA1, 2000);

        protected static readonly X509KeyStorageFlags s_importFlags =
            Cert.EphemeralIfPossible | X509KeyStorageFlags.UserKeySet;

        protected static readonly X509KeyStorageFlags s_exportableImportFlags =
            s_importFlags | X509KeyStorageFlags.Exportable;

        protected abstract void ReadPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedCert);

        protected abstract void ReadEmptyPfx(byte[] pfxBytes, string correctPassword);
        protected abstract void ReadWrongPassword(byte[] pfxBytes, string wrongPassword);
        protected abstract void ReadUnreadablePfx(byte[] pfxBytes, string bestPassword);

        [Fact]
        public void EmptyPfx_NoMac()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithoutIntegrity();
            ReadEmptyPfx(builder.Encode(), correctPassword: null);
        }

        [Fact]
        public void EmptyPfx_NoMac_ArbitraryPassword()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithoutIntegrity();
            byte[] pfxBytes = builder.Encode();

            ReadEmptyPfx(pfxBytes, "arbitrary password");
            ReadEmptyPfx(pfxBytes, "other arbitrary password");
        }

        [Fact]
        public void EmptyPfx_EmptyPassword()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithMac(string.Empty, s_digestAlgorithm, 1);
            byte[] pfxBytes = builder.Encode();

            ReadEmptyPfx(pfxBytes, correctPassword: null);
            ReadEmptyPfx(pfxBytes, correctPassword: string.Empty);
        }

        [Fact]
        public void EmptyPfx_NullPassword()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithMac(null, s_digestAlgorithm, 1);
            byte[] pfxBytes = builder.Encode();

            ReadEmptyPfx(pfxBytes, correctPassword: null);
            ReadEmptyPfx(pfxBytes, correctPassword: string.Empty);
        }

        [Fact]
        public void EmptyPfx_BadPassword()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithMac("correct password", s_digestAlgorithm, 1);
            ReadWrongPassword(builder.Encode(), "wrong password");
        }

        [Fact]
        public void OneCert_NoKeys_EncryptedNullPassword_NoMac()
        {
            using (X509Certificate2 cert = new X509Certificate2(TestData.MsCertificate))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                certContents.AddCertificate(cert);
                builder.AddSafeContentsEncrypted(certContents, (string)null, s_windowsPbe);
                builder.SealWithoutIntegrity();

                ReadPfx(builder.Encode(), null, cert);
                //ReadPfx(builder.Encode(), string.Empty, cert);
            }
        }

        [Fact]
        public void OneCert_NoKeys_EncryptedEmptyPassword_NoMac()
        {
            using (X509Certificate2 cert = new X509Certificate2(TestData.MsCertificate))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                certContents.AddCertificate(cert);
                builder.AddSafeContentsEncrypted(certContents, string.Empty, s_windowsPbe);
                builder.SealWithoutIntegrity();

                //ReadPfx(builder.Encode(), null, cert);
                ReadPfx(builder.Encode(), string.Empty, cert);
            }
        }

        [Fact]
        public void OneCert_EncryptedEmptyPassword_OneKey_EncryptedNullPassword_NoMac()
        {
            using (var cert = new X509Certificate2(TestData.PfxData, TestData.PfxDataPassword, s_exportableImportFlags))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();
                keyContents.AddShroudedKey(cert.GetRSAPrivateKey(), (string)null, s_windowsPbe);
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                certContents.AddCertificate(cert);

                builder.AddSafeContentsUnencrypted(keyContents);
                builder.AddSafeContentsEncrypted(certContents, string.Empty, s_windowsPbe);
                builder.SealWithoutIntegrity();
                byte[] pfxBytes = builder.Encode();

                ReadUnreadablePfx(pfxBytes, null);
                ReadUnreadablePfx(pfxBytes, string.Empty);
            }
        }

        protected static void AssertCertEquals(X509Certificate2 expectedCert, X509Certificate2 actual)
        {
            if (expectedCert.HasPrivateKey)
            {
                Assert.True(actual.HasPrivateKey, "actual.HasPrivateKey");
            }
            else
            {
                Assert.False(actual.HasPrivateKey, "actual.HasPrivateKey");
            }

            Assert.Equal(expectedCert.RawData.ByteArrayToHex(), actual.RawData.ByteArrayToHex());
        }

        protected static void AssertMessageContains(string expectedSubstring, Exception ex)
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                Assert.Contains(expectedSubstring, ex.Message);
            }
        }
    }
}
