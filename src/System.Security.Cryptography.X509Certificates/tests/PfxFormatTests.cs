// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Security.Cryptography.Pkcs;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public abstract partial class PfxFormatTests
    {
        // Use a MAC count of 1 because we're not persisting things, and the password
        // is with the test... just save some CPU cycles.
        private const int MacCount = 1;

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

        protected static readonly Pkcs9LocalKeyId s_keyIdOne = new Pkcs9LocalKeyId(new byte[] { 1 });

        protected abstract void ReadPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedCert,
            Action<X509Certificate2> otherWork = null);

        protected abstract void ReadMultiPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedSingleCert,
            X509Certificate2[] expectedOrder);

        protected abstract void ReadEmptyPfx(byte[] pfxBytes, string correctPassword);
        protected abstract void ReadWrongPassword(byte[] pfxBytes, string wrongPassword);

        protected abstract void ReadUnreadablePfx(
            byte[] pfxBytes,
            string bestPassword,
            // NTE_FAIL
            int win32Error = -2146893792);

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
            builder.SealWithMac(string.Empty, s_digestAlgorithm, MacCount);
            byte[] pfxBytes = builder.Encode();

            ReadEmptyPfx(pfxBytes, correctPassword: null);
            ReadEmptyPfx(pfxBytes, correctPassword: string.Empty);
        }

        [Fact]
        public void EmptyPfx_NullPassword()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithMac(null, s_digestAlgorithm, MacCount);
            byte[] pfxBytes = builder.Encode();

            ReadEmptyPfx(pfxBytes, correctPassword: null);
            ReadEmptyPfx(pfxBytes, correctPassword: string.Empty);
        }

        [Fact]
        public void EmptyPfx_BadPassword()
        {
            Pkcs12Builder builder = new Pkcs12Builder();
            builder.SealWithMac("correct password", s_digestAlgorithm, MacCount);
            ReadWrongPassword(builder.Encode(), "wrong password");
            ReadWrongPassword(builder.Encode(), string.Empty);
            ReadWrongPassword(builder.Encode(), null);
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
                byte[] pfxBytes = builder.Encode();

                ReadPfx(pfxBytes, null, cert);
                ReadPfx(pfxBytes, string.Empty, cert);
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
                byte[] pfxBytes = builder.Encode();

                ReadPfx(pfxBytes, null, cert);
                ReadPfx(pfxBytes, string.Empty, cert);
            }
        }

        [Fact]
        public void OneCert_EncryptedEmptyPassword_OneKey_EncryptedNullPassword_NoMac()
        {
            // This test shows that while a null or empty password will result in both
            // types being tested, the PFX contents have to be the same throughout.
            using (var cert = new X509Certificate2(TestData.PfxData, TestData.PfxDataPassword, s_exportableImportFlags))
            using (AsymmetricAlgorithm key = cert.GetRSAPrivateKey())
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();
                keyContents.AddShroudedKey(key, (string)null, s_windowsPbe);
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

        [Fact]
        public void OneCert_MismatchedKey()
        {
            string pw = nameof(OneCert_MismatchedKey);

            // Build the PFX in the normal Windows style, except the private key doesn't match.
            using (var cert = new X509Certificate2(TestData.PfxData, TestData.PfxDataPassword, s_exportableImportFlags))
            using (RSA realKey = cert.GetRSAPrivateKey())
            using (RSA key = RSA.Create(realKey.KeySize))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();
                Pkcs12SafeBag keyBag = keyContents.AddShroudedKey(key, pw, s_windowsPbe);
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeBag certBag = certContents.AddCertificate(cert);

                keyBag.Attributes.Add(s_keyIdOne);
                certBag.Attributes.Add(s_keyIdOne);

                builder.AddSafeContentsUnencrypted(keyContents);
                builder.AddSafeContentsEncrypted(certContents, pw, s_windowsPbe);
                builder.SealWithoutIntegrity();
                byte[] pfxBytes = builder.Encode();

                ReadPfx(
                    pfxBytes,
                    pw,
                    cert,
                    c =>
                    {
                        using (RSA mismatched = c.GetRSAPrivateKey())
                        using (RSA pubKey = c.GetRSAPublicKey())
                        {
                            Assert.False(
                                pubKey.VerifyData(
                                    pfxBytes,
                                    mismatched.SignData(
                                        pfxBytes,
                                        s_digestAlgorithm,
                                        RSASignaturePadding.Pkcs1),
                                    s_digestAlgorithm,
                                    RSASignaturePadding.Pkcs1),
                                "RSA Signature 'Self-Test' should execute but not match");
                        }
                    });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void OneCert_TwoKeys_FirstWins(bool correctKeyFirst)
        {
            string pw = nameof(OneCert_TwoKeys_FirstWins);

            // Build the PFX in the normal Windows style, except the private key doesn't match.
            using (var cert = new X509Certificate2(TestData.PfxData, TestData.PfxDataPassword, s_exportableImportFlags))
            using (RSA key = cert.GetRSAPrivateKey())
            using (RSA unrelated = RSA.Create(key.KeySize))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeBag keyBag;
                Pkcs12SafeBag keyBag2;
                Pkcs12SafeBag certBag = certContents.AddCertificate(cert);

                if (correctKeyFirst)
                {
                    keyBag = keyContents.AddShroudedKey(key, pw, s_windowsPbe);
                    keyBag2 = keyContents.AddShroudedKey(unrelated, pw, s_windowsPbe);
                }
                else
                {
                    keyBag = keyContents.AddShroudedKey(unrelated, pw, s_windowsPbe);
                    keyBag2 = keyContents.AddShroudedKey(key, pw, s_windowsPbe);
                }

                keyBag.Attributes.Add(s_keyIdOne);
                keyBag2.Attributes.Add(s_keyIdOne);
                certBag.Attributes.Add(s_keyIdOne);

                builder.AddSafeContentsUnencrypted(keyContents);
                builder.AddSafeContentsEncrypted(certContents, pw, s_windowsPbe);
                builder.SealWithoutIntegrity();
                byte[] pfxBytes = builder.Encode();

                ReadPfx(
                    pfxBytes,
                    pw,
                    cert,
                    c =>
                    {
                        using (RSA mismatched = c.GetRSAPrivateKey())
                        using (RSA pubKey = c.GetRSAPublicKey())
                        {
                            // The RSA "self-test" should pass when the correct key is first,
                            // and fail when the unrelated key is first.
                            Assert.Equal(
                                correctKeyFirst,
                                pubKey.VerifyData(
                                    pfxBytes,
                                    mismatched.SignData(
                                        pfxBytes,
                                        s_digestAlgorithm,
                                        RSASignaturePadding.Pkcs1),
                                    s_digestAlgorithm,
                                    RSASignaturePadding.Pkcs1));
                        }
                    });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TwoCerts_OneKey(bool certWithKeyFirst)
        {
            string pw = nameof(TwoCerts_OneKey);

            // Build the PFX in the normal Windows style, except the private key doesn't match.
            using (var cert = new X509Certificate2(TestData.PfxData, TestData.PfxDataPassword, s_exportableImportFlags))
            using (var cert2 = new X509Certificate2(TestData.MsCertificate))
            using (RSA key = cert.GetRSAPrivateKey())
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();

                Pkcs12SafeBag certBag;
                Pkcs12SafeBag keyBag = keyContents.AddShroudedKey(key, pw, s_windowsPbe);
                X509Certificate2[] expectedOrder;

                if (certWithKeyFirst)
                {
                    certBag = certContents.AddCertificate(cert);
                    certContents.AddCertificate(cert2);

                    expectedOrder = new[] { cert2, cert };
                }
                else
                {
                    certContents.AddCertificate(cert2);
                    certBag = certContents.AddCertificate(cert);

                    expectedOrder = new[] { cert, cert2 };
                }

                certBag.Attributes.Add(s_keyIdOne);
                keyBag.Attributes.Add(s_keyIdOne);

                AddContents(keyContents, builder, pw, encrypt: false);
                AddContents(certContents, builder, pw, encrypt: true);

                builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
                ReadMultiPfx(builder.Encode(), pw, cert, expectedOrder);
            }
        }

        [Fact]
        public void OneCert_ExtraKeyWithUnknownAlgorithm()
        {
            string pw = nameof(OneCert_ExtraKeyWithUnknownAlgorithm);

            using (var cert = new X509Certificate2(TestData.MsCertificate))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();

                Pkcs8PrivateKeyInfo pk8 = new Pkcs8PrivateKeyInfo(
                    // The Microsoft organization OID, not an algorithm.
                    new Oid("1.3.6.1.4.1.311", null),
                    null,
                    new byte[] { 0x05, 0x00 });

                // Note that neither the cert nor the key have a LocalKeyId attribute.
                // The existence of this unknown key is enough to abort the load.
                keyContents.AddSafeBag(new Pkcs12ShroudedKeyBag(pk8.Encrypt(pw, s_windowsPbe)));
                certContents.AddCertificate(cert);

                AddContents(keyContents, builder, pw, encrypt: false);
                AddContents(certContents, builder, pw, encrypt: true);

                builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
                byte[] pfxBytes = builder.Encode();

                ReadUnreadablePfx(
                    pfxBytes,
                    pw,
                    //NTE_BAD_ALGID,
                    win32Error: -2146893816);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OneCert_ExtraKeyBadEncoding(bool badTag)
        {
            string pw = nameof(OneCert_ExtraKeyBadEncoding);

            using (var cert = new X509Certificate2(TestData.MsCertificate))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();

                // SEQUENCE { INTEGER(1) } is not a valid RSAPrivateKey, it should be
                // SEQUENCE { INTEGER(N), INTEGER(E), ... (D, P, Q, DP, DQ, QInv) }
                // So the conclusion is "unexpected end of data"
                byte[] badKeyBytes = { 0x30, 0x03, 0x02, 0x01, 0x01 };

                // In "badTag" we make the INTEGER be OCTET STRING, triggering a different
                // "uh, I can't read this..." error.
                if (badTag)
                {
                    badKeyBytes[2] = 0x04;
                }

                Pkcs8PrivateKeyInfo pk8 = new Pkcs8PrivateKeyInfo(
                    // The correct RSA OID.
                    new Oid("1.2.840.113549.1.1.1", null),
                    null,
                    badKeyBytes,
                    skipCopies: true);

                // Note that neither the cert nor the key have a LocalKeyId attribute.
                // The existence of this unreadable key is enough to abort the load.
                keyContents.AddSafeBag(new Pkcs12ShroudedKeyBag(pk8.Encrypt(pw, s_windowsPbe)));
                certContents.AddCertificate(cert);

                AddContents(keyContents, builder, pw, encrypt: false);
                AddContents(certContents, builder, pw, encrypt: true);

                builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
                byte[] pfxBytes = builder.Encode();

                // CRYPT_E_ASN1_BADTAG or CRYPT_E_ASN1_EOD
                int expectedWin32Error = badTag ? -2146881269 : -2146881278;

                ReadUnreadablePfx(
                    pfxBytes,
                    pw,
                    expectedWin32Error);
            }
        }

        [Fact]
        public void OneCert_NoKey_WithLocalKeyId()
        {
            string pw = nameof(OneCert_NoKey_WithLocalKeyId);

            using (var cert = new X509Certificate2(TestData.MsCertificate))
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();

                Pkcs12CertBag certBag = certContents.AddCertificate(cert);
                certBag.Attributes.Add(s_keyIdOne);

                AddContents(certContents, builder, pw, encrypt: true);
                builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
                byte[] pfxBytes = builder.Encode();

                ReadPfx(pfxBytes, pw, cert);
            }
        }

        [Fact]
        public void OneCert_TwentyKeys_NoMatches()
        {
            string pw = nameof(OneCert_NoKey_WithLocalKeyId);

            using (var cert = new X509Certificate2(TestData.MsCertificate))
            using (RSA rsa = RSA.Create())
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();

                Pkcs12CertBag certBag = certContents.AddCertificate(cert);
                certBag.Attributes.Add(s_keyIdOne);

                for (int i = 0; i < 20; i++)
                {
                    Pkcs12SafeBag keyBag = keyContents.AddShroudedKey(rsa, pw, s_windowsPbe);

                    // Even with i=1 this won't match, because { 0x01 } != { 0x01, 0x00, 0x00, 0x00 } and
                    // { 0x01 } != { 0x00, 0x00, 0x00, 0x01 } (binary comparison, not "equivalence" comparison).
                    keyBag.Attributes.Add(new Pkcs9LocalKeyId(BitConverter.GetBytes(i)));
                }

                AddContents(keyContents, builder, pw, encrypt: false);
                AddContents(certContents, builder, pw, encrypt: true);
                builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
                byte[] pfxBytes = builder.Encode();

                ReadPfx(pfxBytes, pw, cert);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TwoCerts_TwentyKeys_NoMatches(bool msCertFirst)
        {
            string pw = nameof(OneCert_NoKey_WithLocalKeyId);

            using (var certWithKey = new X509Certificate2(TestData.PfxData, TestData.PfxDataPassword))
            using (var cert = new X509Certificate2(certWithKey.RawData))
            using (var cert2 = new X509Certificate2(TestData.MsCertificate))
            using (RSA rsa = RSA.Create())
            {
                Pkcs12Builder builder = new Pkcs12Builder();
                Pkcs12SafeContents certContents = new Pkcs12SafeContents();
                Pkcs12SafeContents keyContents = new Pkcs12SafeContents();
                Pkcs12CertBag certBag;

                if (msCertFirst)
                {
                    certBag = certContents.AddCertificate(cert2);
                    certBag.Attributes.Add(s_keyIdOne);
                }

                certBag = certContents.AddCertificate(cert);
                certBag.Attributes.Add(new Pkcs9LocalKeyId(cert.GetCertHash()));

                if (!msCertFirst)
                {
                    certBag = certContents.AddCertificate(cert2);
                    certBag.Attributes.Add(s_keyIdOne);
                }

                for (int i = 0; i < 20; i++)
                {
                    Pkcs12SafeBag keyBag = keyContents.AddShroudedKey(rsa, pw, s_windowsPbe);

                    // Even with i=1 this won't match, because { 0x01 } != { 0x01, 0x00, 0x00, 0x00 } and
                    // { 0x01 } != { 0x00, 0x00, 0x00, 0x01 } (binary comparison, not "equivalence" comparison).
                    keyBag.Attributes.Add(new Pkcs9LocalKeyId(BitConverter.GetBytes(i)));
                }

                AddContents(keyContents, builder, pw, encrypt: false);
                AddContents(certContents, builder, pw, encrypt: true);
                builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
                byte[] pfxBytes = builder.Encode();

                ReadMultiPfx(
                    pfxBytes,
                    pw,
                    msCertFirst ? cert : cert2,
                    msCertFirst ? new[] { cert, cert2 } : new[] { cert2, cert });
            }
        }

        [Fact]
        public void OneCorruptCert()
        {
            string pw = nameof(OneCorruptCert);
            Pkcs12Builder builder = new Pkcs12Builder();
            Pkcs12SafeContents contents = new Pkcs12SafeContents();
            contents.AddSafeBag(new Pkcs12CertBag(new Oid("1.2.840.113549.1.9.22.1"), new byte[] { 0x05, 0x00 }));
            AddContents(contents, builder, pw, encrypt: true);
            builder.SealWithMac(pw, s_digestAlgorithm, MacCount);
            byte[] pfxBytes = builder.Encode();

            ReadUnreadablePfx(
                pfxBytes,
                pw,
                // CRYPT_E_BAD_ENCODE
                -2146885630);
        }

        private static void AddContents(
            Pkcs12SafeContents contents,
            Pkcs12Builder builder,
            string password,
            bool encrypt)
        {
            if (encrypt)
            {
                builder.AddSafeContentsEncrypted(contents, password, s_windowsPbe);
            }
            else
            {
                builder.AddSafeContentsUnencrypted(contents);
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
