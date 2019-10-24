// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography.Pal.AnyOS
{
    internal sealed partial class ManagedPkcsPal : PkcsPal
    {
        internal sealed class ManagedKeyTransPal : KeyTransRecipientInfoPal
        {
            private readonly KeyTransRecipientInfoAsn _asn;

            internal ManagedKeyTransPal(KeyTransRecipientInfoAsn asn)
            {
                _asn = asn;
            }

            public override byte[] EncryptedKey =>
                _asn.EncryptedKey.ToArray();

            public override AlgorithmIdentifier KeyEncryptionAlgorithm =>
                _asn.KeyEncryptionAlgorithm.ToPresentationObject();

            public override SubjectIdentifier RecipientIdentifier =>
                new SubjectIdentifier(_asn.Rid.IssuerAndSerialNumber, _asn.Rid.SubjectKeyIdentifier);

            public override int Version => _asn.Version;

            internal byte[] DecryptCek(X509Certificate2 cert, RSA privateKey, out Exception exception)
            {
                ReadOnlyMemory<byte>? parameters = _asn.KeyEncryptionAlgorithm.Parameters;
                string keyEncryptionAlgorithm = _asn.KeyEncryptionAlgorithm.Algorithm.Value;

                switch (keyEncryptionAlgorithm)
                {
                    case Oids.Rsa:
                        if (parameters != null &&
                            !parameters.Value.Span.SequenceEqual(s_rsaPkcsParameters))
                        {
                            exception = new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                            return null;
                        }
                        break;
                    case Oids.RsaOaep:
                        break;
                    default:
                        exception = new CryptographicException(
                            SR.Cryptography_Cms_UnknownAlgorithm,
                            _asn.KeyEncryptionAlgorithm.Algorithm.Value);

                        return null;
                }

                return DecryptCekCore(cert, privateKey, _asn.EncryptedKey.Span, keyEncryptionAlgorithm, parameters, out exception);
            }

            internal static byte[] DecryptCekCore(
                X509Certificate2 cert,
                RSA privateKey,
                ReadOnlySpan<byte> encryptedKey,
                string keyEncryptionAlgorithm,
                ReadOnlyMemory<byte>? algorithmParameters,
                out Exception exception)
            {
                RSAEncryptionPadding encryptionPadding;

                switch (keyEncryptionAlgorithm)
                {
                    case Oids.Rsa:
                        encryptionPadding = RSAEncryptionPadding.Pkcs1;
                        break;
                    case Oids.RsaOaep:
                        if (!PkcsHelpers.TryGetRsaOaepEncryptionPadding(algorithmParameters, out encryptionPadding, out exception))
                        {
                            return null;
                        }
                        break;
                    default:
                        exception = new CryptographicException(
                            SR.Cryptography_Cms_UnknownAlgorithm,
                            keyEncryptionAlgorithm);

                        return null;
                }

                if (privateKey != null)
                {
                    return DecryptKey(privateKey, encryptionPadding, encryptedKey, out exception);
                }
                else
                {
                    using (RSA rsa = cert.GetRSAPrivateKey())
                    {
                        return DecryptKey(rsa, encryptionPadding, encryptedKey, out exception);
                    }
                }
            }
        }

        private KeyTransRecipientInfoAsn MakeKtri(
            byte[] cek,
            CmsRecipient recipient,
            out bool v0Recipient)
        {
            KeyTransRecipientInfoAsn ktri = new KeyTransRecipientInfoAsn();

            if (recipient.RecipientIdentifierType == SubjectIdentifierType.SubjectKeyIdentifier)
            {
                ktri.Version = 2;
                ktri.Rid.SubjectKeyIdentifier = GetSubjectKeyIdentifier(recipient.Certificate);
            }
            else if (recipient.RecipientIdentifierType == SubjectIdentifierType.IssuerAndSerialNumber)
            {
                byte[] serial = recipient.Certificate.GetSerialNumber();
                Array.Reverse(serial);

                IssuerAndSerialNumberAsn iasn = new IssuerAndSerialNumberAsn
                {
                    Issuer = recipient.Certificate.IssuerName.RawData,
                    SerialNumber = serial,
                };

                ktri.Rid.IssuerAndSerialNumber = iasn;
            }
            else
            {
                throw new CryptographicException(
                    SR.Cryptography_Cms_Invalid_Subject_Identifier_Type,
                    recipient.RecipientIdentifierType.ToString());
            }

            RSAEncryptionPadding padding = recipient.RSAEncryptionPadding ?? RSAEncryptionPadding.Pkcs1;

            if (padding == RSAEncryptionPadding.Pkcs1)
            {
                ktri.KeyEncryptionAlgorithm.Algorithm = new Oid(Oids.Rsa, Oids.Rsa);
                ktri.KeyEncryptionAlgorithm.Parameters = s_rsaPkcsParameters;
            }
            else if (padding == RSAEncryptionPadding.OaepSHA1)
            {
                ktri.KeyEncryptionAlgorithm.Algorithm = new Oid(Oids.RsaOaep, Oids.RsaOaep);
                ktri.KeyEncryptionAlgorithm.Parameters = s_rsaOaepSha1Parameters;
            }
            else if (padding == RSAEncryptionPadding.OaepSHA256)
            {
                ktri.KeyEncryptionAlgorithm.Algorithm = new Oid(Oids.RsaOaep, Oids.RsaOaep);
                ktri.KeyEncryptionAlgorithm.Parameters = s_rsaOaepSha256Parameters;
            }
            else if (padding == RSAEncryptionPadding.OaepSHA384)
            {
                ktri.KeyEncryptionAlgorithm.Algorithm = new Oid(Oids.RsaOaep, Oids.RsaOaep);
                ktri.KeyEncryptionAlgorithm.Parameters = s_rsaOaepSha384Parameters;
            }
            else if (padding == RSAEncryptionPadding.OaepSHA512)
            {
                ktri.KeyEncryptionAlgorithm.Algorithm = new Oid(Oids.RsaOaep, Oids.RsaOaep);
                ktri.KeyEncryptionAlgorithm.Parameters = s_rsaOaepSha512Parameters;
            }
            else
            {
                throw new CryptographicException(SR.Cryptography_Cms_UnknownAlgorithm);
            }

            using (RSA rsa = recipient.Certificate.GetRSAPublicKey())
            {
                ktri.EncryptedKey = rsa.Encrypt(cek, padding);
            }

            v0Recipient = (ktri.Version == 0);
            return ktri;
        }

        private static byte[] DecryptKey(
            RSA privateKey,
            RSAEncryptionPadding encryptionPadding,
            ReadOnlySpan<byte> encryptedKey,
            out Exception exception)
        {
            if (privateKey == null)
            {
                exception = new CryptographicException(SR.Cryptography_Cms_Signing_RequiresPrivateKey);
                return null;
            }

#if NETCOREAPP || NETSTANDARD2_1
            byte[] cek = null;
            int cekLength = 0;

            try
            {
                cek = CryptoPool.Rent(privateKey.KeySize / 8);

                if (!privateKey.TryDecrypt(encryptedKey, cek, encryptionPadding, out cekLength))
                {
                    Debug.Fail("TryDecrypt wanted more space than the key size");
                    exception = new CryptographicException();
                    return null;
                }

                exception = null;
                return new Span<byte>(cek, 0, cekLength).ToArray();
            }
            catch (CryptographicException e)
            {
                exception = e;
                return null;
            }
            finally
            {
                if (cek != null)
                {
                    CryptoPool.Return(cek, cekLength);
                }
            }
#else
            try
            {
                exception = null;
                return privateKey.Decrypt(encryptedKey.ToArray(), encryptionPadding);
            }
            catch (CryptographicException e)
            {
                exception = e;
                return null;
            }
#endif
        }
    }
}
