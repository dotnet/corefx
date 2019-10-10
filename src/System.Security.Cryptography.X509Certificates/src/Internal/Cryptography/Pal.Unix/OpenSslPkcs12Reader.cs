// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;

namespace Internal.Cryptography.Pal
{
    internal sealed class OpenSslPkcs12Reader : UnixPkcs12Reader
    {
        private OpenSslPkcs12Reader(byte[] data)
        {
            ParsePkcs12(data);
        }

        protected override ICertificatePalCore ReadX509Der(ReadOnlyMemory<byte> data)
        {
            if (OpenSslX509CertificateReader.TryReadX509Der(data.Span, out ICertificatePal ret))
            {
                return ret;
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        public static bool TryRead(byte[] data, out OpenSslPkcs12Reader pkcs12Reader) =>
            TryRead(data, out pkcs12Reader, out _, captureException: false);

        public static bool TryRead(byte[] data, out OpenSslPkcs12Reader pkcs12Reader, out Exception openSslException) =>
            TryRead(data, out pkcs12Reader, out openSslException, captureException: true);

        protected override AsymmetricAlgorithm LoadKey(ReadOnlyMemory<byte> pkcs8)
        {
            PrivateKeyInfoAsn privateKeyInfo = PrivateKeyInfoAsn.Decode(pkcs8, AsnEncodingRules.BER);
            AsymmetricAlgorithm key;

            switch (privateKeyInfo.PrivateKeyAlgorithm.Algorithm.Value)
            {
                case Oids.Rsa:
                    key = new RSAOpenSsl();
                    break;
                case Oids.Dsa:
                    key = new DSAOpenSsl();
                    break;
                case Oids.EcDiffieHellman:
                case Oids.EcPublicKey:
                    key = new ECDiffieHellmanOpenSsl();
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

        internal static SafeEvpPKeyHandle GetPrivateKey(AsymmetricAlgorithm key)
        {
            if (key is RSAOpenSsl rsa)
            {
                return rsa.DuplicateKeyHandle();
            }

            if (key is DSAOpenSsl dsa)
            {
                return dsa.DuplicateKeyHandle();
            }

            return ((ECDiffieHellmanOpenSsl)key).DuplicateKeyHandle();
        }

        private static bool TryRead(
            byte[] data,
            out OpenSslPkcs12Reader pkcs12Reader,
            out Exception openSslException,
            bool captureException)
        {
            openSslException = null;

            try
            {
                pkcs12Reader = new OpenSslPkcs12Reader(data);
                return true;
            }
            catch (CryptographicException e)
            {
                if (captureException)
                {
                    openSslException = e;
                }

                pkcs12Reader = null;
                return false;
            }
        }
    }
}
