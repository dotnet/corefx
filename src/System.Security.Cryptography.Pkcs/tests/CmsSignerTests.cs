// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Xunit;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs.Tests
{
    public class CmsSignerTests
    {
        [Fact]
        public void SignCmsUsingAsymmetricSignatureFormatter()
        {
            byte[] contentBytes = { 9, 8, 7, 6, 5 };

            byte[] signedData = GetSignedData(contentBytes, out byte[] certBytes);
            Assert.True(ByteArrayContains(signedData, contentBytes));

            using (X509Certificate2 cert = new X509Certificate2(certBytes))
            {
                Assert.Null(cert.PrivateKey);
                ContentInfo contentInfo = new ContentInfo(contentBytes);
                SignedCms cms = new SignedCms(contentInfo, detached: false);

                cms.Decode(signedData);
                cms.CheckSignature(verifySignatureOnly: true);

                Assert.Equal(1, cms.SignerInfos.Count);
                Assert.Equal(cert, cms.SignerInfos[0].Certificate);
            }
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

        private static byte[] GetSignedData(byte[] contentBytes, out byte[] certBytes)
        {
            ContentInfo contentInfo = new ContentInfo(contentBytes);
            SignedCms cms = new SignedCms(contentInfo, detached: false);

            (X509Certificate2 cert, MyRSA key) = GetRSACertAndKey();
            using (key)
            {
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert, key);

                cms.ComputeSignature(signer);
                certBytes = cert.Export(X509ContentType.Cert);

                byte[] ret = cms.Encode();
                Assert.True(key.SignHashCalled);

                return ret;
            }
        }

        private static (X509Certificate2 pubCert, MyRSA key) GetRSACertAndKey()
        {
            using (X509Certificate2 signerCert = Certificates.RSA2048SignatureOnly.TryGetCertificateWithPrivateKey())
            {
                X509Certificate2 pubCert = new X509Certificate2(signerCert.Export(X509ContentType.Cert));
                Assert.Null(pubCert.PrivateKey);

                MyRSA formatter = new MyRSA((RSA)signerCert.PrivateKey);
                return (pubCert, formatter);
            }
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
}
