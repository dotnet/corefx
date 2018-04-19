// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Test.Cryptography;
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

            (X509Certificate2 cert, MyRSAFormatter formatter) = GetCertAndFormatter();
            using (cert)
            {
                CmsSigner signer = new CmsSigner(formatter, cert);

                cms.ComputeSignature(signer);
                certBytes = cert.Export(X509ContentType.Cert);

                return cms.Encode();
            }
        }

        private static (X509Certificate2 pubCert, MyRSAFormatter formatter) GetCertAndFormatter()
        {
            using (X509Certificate2 signerCert = Certificates.RSA2048SignatureOnly.TryGetCertificateWithPrivateKey())
            {
                X509Certificate2 pubCert = new X509Certificate2(signerCert.Export(X509ContentType.Cert));
                Assert.Null(pubCert.PrivateKey);

                MyRSAFormatter formatter = new MyRSAFormatter(signerCert);
                return (pubCert, formatter);
            }
        }
    }

    class MyRSAFormatter : AsymmetricSignatureFormatter
    {
        private RSAPKCS1SignatureFormatter _formatter;
        private RSAParameters _expectedPublicKey;
        private RSA _privateKey;

        public MyRSAFormatter(X509Certificate2 cert)
        {
            _formatter = new RSAPKCS1SignatureFormatter(cert.PrivateKey);
            _expectedPublicKey = ((RSA)cert.PublicKey.Key).ExportParameters(false);
            _privateKey = (RSA)cert.PrivateKey;
        }

        public override byte[] CreateSignature(byte[] rgbHash)
        {
            return _formatter.CreateSignature(rgbHash);
        }

        public override void SetHashAlgorithm(string strName)
        {
            _formatter.SetHashAlgorithm(strName);
        }

        public override void SetKey(AsymmetricAlgorithm key)
        {
            RSAParameters pk = ((RSA)key).ExportParameters(false);
            if (PublicKeyEquals(pk, _expectedPublicKey))
            {
                _formatter.SetKey(_privateKey);
            }
            else
            {
                throw new Exception("Unknown public key");
            }
        }

        private bool PublicKeyEquals(RSAParameters a, RSAParameters b)
        {
            return a.Exponent.SequenceEqual(b.Exponent) && a.Modulus.SequenceEqual(b.Modulus);
        }
    }
}
