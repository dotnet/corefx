// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Apple;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed class ApplePkcs12Reader : UnixPkcs12Reader
    {
        internal ApplePkcs12Reader(byte[] data)
        {
            ParsePkcs12(data);
        }

        protected override ICertificatePalCore ReadX509Der(ReadOnlyMemory<byte> data)
        {
            SafeSecCertificateHandle certHandle = Interop.AppleCrypto.X509ImportCertificate(
                data.ToArray(),
                X509ContentType.Cert,
                SafePasswordHandle.InvalidHandle,
                SafeTemporaryKeychainHandle.InvalidHandle,
                exportable: true,
                out SafeSecIdentityHandle identityHandle);

            if (identityHandle.IsInvalid)
            {
                identityHandle.Dispose();
                return new AppleCertificatePal(certHandle);
            }

            Debug.Fail("Non-PKCS12 import produced an identity handle");

            identityHandle.Dispose();
            certHandle.Dispose();
            throw new CryptographicException();
        }

        protected override AsymmetricAlgorithm LoadKey(ReadOnlyMemory<byte> pkcs8)
        {
            PrivateKeyInfoAsn privateKeyInfo = PrivateKeyInfoAsn.Decode(pkcs8, AsnEncodingRules.BER);
            AsymmetricAlgorithm key;

            switch (privateKeyInfo.PrivateKeyAlgorithm.Algorithm.Value)
            {
                case Oids.Rsa:
                    key = new RSAImplementation.RSASecurityTransforms();
                    break;
                case Oids.Dsa:
                    key = new DSAImplementation.DSASecurityTransforms();
                    break;
                case Oids.EcDiffieHellman:
                case Oids.EcPublicKey:
                    key = new ECDsaImplementation.ECDsaSecurityTransforms();
                    break;
                default:
                    throw new CryptographicException(
                        SR.Cryptography_UnknownAlgorithmIdentifier,
                        privateKeyInfo.PrivateKeyAlgorithm.Algorithm.Value);
            }

            key.ImportPkcs8PrivateKey(pkcs8.Span, out int bytesRead);

            if (bytesRead != pkcs8.Length)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            return key;
        }

        internal static SafeSecKeyRefHandle GetPrivateKey(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                return null;
            }

            if (key is RSAImplementation.RSASecurityTransforms rsa)
            {
                return rsa.GetKeys().PrivateKey;
            }

            if (key is DSAImplementation.DSASecurityTransforms dsa)
            {
                return dsa.GetKeys().PrivateKey;
            }

            return ((ECDsaImplementation.ECDsaSecurityTransforms)key).GetKeys().PrivateKey;
        }
    }
}
