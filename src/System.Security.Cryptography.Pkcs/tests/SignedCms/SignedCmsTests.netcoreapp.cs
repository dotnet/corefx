// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Pkcs.Tests
{
    public static partial class SignedCmsTests
    {
        [Fact]
        public static void SignCmsUsingExplicitRSAKey()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            (byte[] signedData, X509Certificate2 cert) = GetSignedData(content, GetRSACertAndKey, (key) => ((MyRSA)key).SignHashCalled);

            using (cert)
            {
                VerifyCmsCorrectlySigned(content, signedData, cert);
            }
        }

        [Fact]
        public static void SignCmsUsingExplicitDSAKey()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            (byte[] signedData, X509Certificate2 cert) = GetSignedData(content, GetDSACertAndKey, (key) => ((MyDSA)key).CreateSignatureCalled);

            using (cert)
            {
                VerifyCmsCorrectlySigned(content, signedData, cert);
            }
        }

        [Fact]
        public static void SignCmsUsingExplicitECDsaKey()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            (byte[] signedData, X509Certificate2 cert) = GetSignedData(content, GetECDsaCertAndKey, (key) => ((MyECDsa)key).SignHashCalled);

            using (cert)
            {
                VerifyCmsCorrectlySigned(content, signedData, cert);
            }
        }

        [Fact]
        public static void CounterSignCmsUsingExplicitRSAKeyForFirstSignerAndDSAForCounterSignature()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            Func<(X509Certificate2, AsymmetricAlgorithm)> getFirstSignerCertAndKey = GetRSACertAndKey;
            Func<AsymmetricAlgorithm, bool> signHashCalled = (key) => ((MyRSA)key).SignHashCalled;

            Func<(X509Certificate2, AsymmetricAlgorithm)> getCounterSignerCertAndKey = GetDSACertAndKey;
            Func<AsymmetricAlgorithm, bool> counterSignHashCalled = (key) => ((MyDSA)key).CreateSignatureCalled;

            (byte[] signedData, X509Certificate2 signerCert, X509Certificate2 counterSignerCert) = GetCounterSignedData(content, getFirstSignerCertAndKey, getCounterSignerCertAndKey, signHashCalled, counterSignHashCalled);

            using (signerCert)
            using (counterSignerCert)
            {
                VerifyCmsCorrectlySigned(content, signedData, signerCert, counterSignerCert);
            }
        }

        [Fact]
        public static void CounterSignCmsUsingExplicitDSAKeyForFirstSignerAndECDsaForCounterSignature()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            Func<(X509Certificate2, AsymmetricAlgorithm)> getFirstSignerCertAndKey = GetDSACertAndKey;
            Func<AsymmetricAlgorithm, bool> signHashCalled = (key) => ((MyDSA)key).CreateSignatureCalled;

            Func<(X509Certificate2, AsymmetricAlgorithm)> getCounterSignerCertAndKey = GetECDsaCertAndKey;
            Func<AsymmetricAlgorithm, bool> counterSignHashCalled = (key) => ((MyECDsa)key).SignHashCalled;

            (byte[] signedData, X509Certificate2 signerCert, X509Certificate2 counterSignerCert) = GetCounterSignedData(content, getFirstSignerCertAndKey, getCounterSignerCertAndKey, signHashCalled, counterSignHashCalled);

            using (signerCert)
            using (counterSignerCert)
            {
                VerifyCmsCorrectlySigned(content, signedData, signerCert, counterSignerCert);
            }
        }

        [Fact]
        public static void CounterSignCmsUsingExplicitECDsaKeyForFirstSignerAndRSAForCounterSignature()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            Func<(X509Certificate2, AsymmetricAlgorithm)> getFirstSignerCertAndKey = GetECDsaCertAndKey;
            Func<AsymmetricAlgorithm, bool> signHashCalled = (key) => ((MyECDsa)key).SignHashCalled;

            Func<(X509Certificate2, AsymmetricAlgorithm)> getCounterSignerCertAndKey = GetRSACertAndKey;
            Func<AsymmetricAlgorithm, bool> counterSignHashCalled = (key) => ((MyRSA)key).SignHashCalled;

            (byte[] signedData, X509Certificate2 signerCert, X509Certificate2 counterSignerCert) = GetCounterSignedData(content, getFirstSignerCertAndKey, getCounterSignerCertAndKey, signHashCalled, counterSignHashCalled);

            using (signerCert)
            using (counterSignerCert)
            {
                VerifyCmsCorrectlySigned(content, signedData, signerCert, counterSignerCert);
            }
        }

        [Fact]
        public static void SignCmsUsingRSACertAndECDsaKeyThrows()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            using (X509Certificate2 cert = Certificates.RSA2048SignatureOnly.GetCertificate())
            using (ECDsa key = ECDsa.Create())
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                Assert.Throws<CryptographicException>(() => cms.ComputeSignature(signer));
            }
        }

        [Fact]
        public static void SignCmsUsingDSACertAndECDsaKeyThrows()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            using (X509Certificate2 cert = Certificates.Dsa1024.GetCertificate())
            using (ECDsa key = ECDsa.Create())
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                signer.IncludeOption = X509IncludeOption.EndCertOnly;
                signer.DigestAlgorithm = new Oid(Oids.Sha1, Oids.Sha1);
                Assert.Throws<CryptographicException>(() => cms.ComputeSignature(signer));
            }
        }

        [Fact]
        public static void SignCmsUsingEDCSaCertAndRSAaKeyThrows()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            using (X509Certificate2 cert = Certificates.ECDsaP256Win.GetCertificate())
            using (RSA key = RSA.Create())
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                Assert.Throws<CryptographicException>(() => cms.ComputeSignature(signer));
            }
        }

        [Fact]
        public static void SignCmsUsingRSACertWithNotMatchingKeyThrows()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            using (X509Certificate2 cert = Certificates.RSA2048SignatureOnly.GetCertificate())
            using (RSA key = RSA.Create())
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                Assert.Throws<CryptographicException>(() => cms.ComputeSignature(signer));
            }
        }

        [Fact]
        public static void SignCmsUsingDSACertWithNotMatchingKeyThrows()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            using (X509Certificate2 cert = Certificates.Dsa1024.GetCertificate())
            using (DSA key = DSA.Create())
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                signer.IncludeOption = X509IncludeOption.EndCertOnly;
                signer.DigestAlgorithm = new Oid(Oids.Sha1, Oids.Sha1);
                Assert.Throws<CryptographicException>(() => cms.ComputeSignature(signer));
            }
        }

        [Fact]
        public static void SignCmsUsingECDsaCertWithNotMatchingKeyThrows()
        {
            byte[] content = { 9, 8, 7, 6, 5 };

            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            using (X509Certificate2 cert = Certificates.ECDsaP256Win.GetCertificate())
            using (ECDsa key = ECDsa.Create())
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                Assert.Throws<CryptographicException>(() => cms.ComputeSignature(signer));
            }
        }

        private static void VerifyCmsCorrectlySigned(byte[] content, byte[] signedData, X509Certificate2 cert, X509Certificate2 counterSignerCert = null)
        {
            Assert.False(cert.HasPrivateKey);
            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            cms.Decode(signedData);
            cms.CheckSignature(verifySignatureOnly: true);

            Assert.Equal(1, cms.SignerInfos.Count);

            SignerInfo signer = cms.SignerInfos[0];
            Assert.Equal(cert, signer.Certificate);

            if (counterSignerCert != null)
            {
                Assert.Equal(1, signer.CounterSignerInfos.Count);
                Assert.Equal(counterSignerCert, signer.CounterSignerInfos[0].Certificate);
            }
            else
            {
                Assert.Equal(0, signer.CounterSignerInfos.Count);
            }
        }

        private static (byte[] signedData, X509Certificate2 pubCert) GetSignedData(byte[] content, Func<(X509Certificate2, AsymmetricAlgorithm)> getCertAndKey, Func<AsymmetricAlgorithm, bool> checkIfSignHashCalled)
        {
            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            (X509Certificate2 cert, AsymmetricAlgorithm key) = getCertAndKey();
            using (key)
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);
                if (key is DSA)
                {
                    signer.IncludeOption = X509IncludeOption.EndCertOnly;
                    signer.DigestAlgorithm = new Oid(Oids.Sha1, Oids.Sha1);
                }

                cms.ComputeSignature(signer);

                byte[] ret = cms.Encode();
                Assert.True(checkIfSignHashCalled(key));

                Assert.True(ByteArrayContains(ret, content));

                return (ret, cert);
            }
        }

        private static (byte[] counterSignedData, X509Certificate2 firstSignerCert, X509Certificate2 counterSignerCert) GetCounterSignedData(byte[] content, Func<(X509Certificate2, AsymmetricAlgorithm)> getFirstSignerCertAndKey, Func<(X509Certificate2, AsymmetricAlgorithm)> getCounterSignerCertAndKey, Func<AsymmetricAlgorithm, bool> signedHashCalled, Func<AsymmetricAlgorithm, bool> signedHashCalledForCounterSignerKey)
        {
            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            (X509Certificate2 firstSignerCert, AsymmetricAlgorithm firstSignerKey) = getFirstSignerCertAndKey();
            using (firstSignerKey)
            {
                CmsSigner firstSigner = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, firstSignerCert, firstSignerKey);
                if (firstSignerKey is DSA)
                {
                    firstSigner.IncludeOption = X509IncludeOption.EndCertOnly;
                    firstSigner.DigestAlgorithm = new Oid(Oids.Sha1, Oids.Sha1);
                }

                cms.ComputeSignature(firstSigner);
                Assert.True(signedHashCalled(firstSignerKey));
            }

            byte[] signed = cms.Encode();
            Assert.True(ByteArrayContains(signed, content));
            SignerInfo firstSignerInfo = cms.SignerInfos[0];

            (X509Certificate2 counterSignerCert, AsymmetricAlgorithm counterSignerKey) = getCounterSignerCertAndKey();
            using (counterSignerKey)
            {
                CmsSigner counterSigner = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, counterSignerCert, counterSignerKey);
                if (counterSignerKey is DSA)
                {
                    counterSigner.IncludeOption = X509IncludeOption.EndCertOnly;
                    counterSigner.DigestAlgorithm = new Oid(Oids.Sha1, Oids.Sha1);
                }

                firstSignerInfo.ComputeCounterSignature(counterSigner);
                Assert.True(signedHashCalledForCounterSignerKey(counterSignerKey));
            }

            byte[] counterSigned = cms.Encode();
            Assert.True(ByteArrayContains(counterSigned, content));
            Assert.True(counterSigned.Length > signed.Length);

            return (counterSigned, firstSignerCert, counterSignerCert);
        }

        private static bool ByteArrayContains(byte[] array, byte[] subArray)
        {
            int lengthDiff = array.Length - subArray.Length;

            if (lengthDiff < 0)
            {
                return false;
            }

            for (int shift = 0; shift <= lengthDiff; shift++)
            {
                if (subArray.SequenceEqual(array.Skip(shift).Take(subArray.Length)))
                {
                    return true;
                }
            }

            return false;
        }

        private static (X509Certificate2 pubCert, AsymmetricAlgorithm key) GetRSACertAndKey()
        {
            using (X509Certificate2 signerCert = Certificates.RSA2048SignatureOnly.TryGetCertificateWithPrivateKey())
            {
                Assert.True(signerCert.HasPrivateKey);
                X509Certificate2 pubCert = new X509Certificate2(signerCert.Export(X509ContentType.Cert));
                Assert.False(pubCert.HasPrivateKey);

                MyRSA key = new MyRSA(signerCert.GetRSAPrivateKey());
                return (pubCert, key);
            }
        }

        private static (X509Certificate2 pubCert, AsymmetricAlgorithm key) GetDSACertAndKey()
        {
            using (X509Certificate2 signerCert = Certificates.Dsa1024.TryGetCertificateWithPrivateKey())
            {
                Assert.True(signerCert.HasPrivateKey);
                X509Certificate2 pubCert = new X509Certificate2(signerCert.Export(X509ContentType.Cert));
                Assert.False(pubCert.HasPrivateKey);

                MyDSA key = new MyDSA(signerCert.GetDSAPrivateKey());
                return (pubCert, key);
            }
        }

        private static (X509Certificate2 pubCert, AsymmetricAlgorithm key) GetECDsaCertAndKey()
        {
            using (X509Certificate2 signerCert = Certificates.ECDsaP256Win.TryGetCertificateWithPrivateKey())
            {
                Assert.True(signerCert.HasPrivateKey);
                X509Certificate2 pubCert = new X509Certificate2(signerCert.Export(X509ContentType.Cert));
                Assert.False(pubCert.HasPrivateKey);

                MyECDsa key = new MyECDsa(signerCert.GetECDsaPrivateKey());
                return (pubCert, key);
            }
        }

        class MyRSA : RSA
        {
            public bool SignHashCalled { get; set; } = false;
            private RSA _key;

            public MyRSA(RSA key)
            {
                _key = key;
            }

            public override RSAParameters ExportParameters(bool includePrivateParameters)
            {
                if (includePrivateParameters)
                {
                    throw new Exception("Not possible");
                }

                return _key.ExportParameters(false);
            }

            public override void ImportParameters(RSAParameters parameters)
            {
                throw new Exception("Not possible");
            }

            public override bool TrySignHash(ReadOnlySpan<byte> hash, Span<byte> destination, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding, out int bytesWritten)
            {
                SignHashCalled = true;
                return _key.TrySignHash(hash, destination, hashAlgorithm, padding, out bytesWritten);
            }

            public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
            {
                SignHashCalled = true;
                return _key.SignHash(hash, hashAlgorithm, padding);
            }

            protected override void Dispose(bool disposing)
            {
                _key.Dispose();
            }
        }

        class MyDSA : DSA
        {
            public bool CreateSignatureCalled { get; set; } = false;
            private DSA _key;

            public MyDSA(DSA key)
            {
                _key = key;
            }

            public override DSAParameters ExportParameters(bool includePrivateParameters)
            {
                if (includePrivateParameters)
                {
                    throw new Exception("Not possible");
                }

                return _key.ExportParameters(false);
            }

            public override void ImportParameters(DSAParameters parameters)
            {
                throw new Exception("Not possible");
            }

            public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
            {
                throw new Exception("Not possible");
            }

            public override int KeySize
            {
                get => _key.KeySize;
                set => throw new Exception("Not possible");
            }

            public override byte[] CreateSignature(byte[] rgbHash)
            {
                CreateSignatureCalled = true;
                return _key.CreateSignature(rgbHash);
            }

            public override bool TryCreateSignature(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
            {
                CreateSignatureCalled = true;
                return _key.TryCreateSignature(hash, destination, out bytesWritten);
            }

            protected override void Dispose(bool disposing)
            {
                _key.Dispose();
            }
        }

        class MyECDsa : ECDsa
        {
            public bool SignHashCalled { get; set; } = false;
            private ECDsa _key;

            public MyECDsa(ECDsa key)
            {
                _key = key;
            }

            public override bool VerifyHash(byte[] hash, byte[] signature)
            {
                throw new Exception("Not possible");
            }

            public override bool TrySignHash(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
            {
                SignHashCalled = true;
                return _key.TrySignHash(hash, destination, out bytesWritten);
            }

            public override byte[] SignHash(byte[] hash)
            {
                SignHashCalled = true;
                return _key.SignHash(hash);
            }

            protected override void Dispose(bool disposing)
            {
                _key.Dispose();
            }
        }
    }
}
