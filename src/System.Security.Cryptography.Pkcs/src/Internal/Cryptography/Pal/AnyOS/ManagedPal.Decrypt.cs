// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography.Pal.AnyOS
{
    internal sealed partial class ManagedPkcsPal : PkcsPal
    {
        private class ManagedDecryptorPal : DecryptorPal
        {
            private byte[] _dataCopy;
            private EnvelopedDataAsn _envelopedData;

            public ManagedDecryptorPal(
                byte[] dataCopy,
                EnvelopedDataAsn envelopedDataAsn,
                RecipientInfoCollection recipientInfos)
                : base(recipientInfos)
            {
                _dataCopy = dataCopy;
                _envelopedData = envelopedDataAsn;
            }

            public override ContentInfo TryDecrypt(
                RecipientInfo recipientInfo,
                X509Certificate2 cert,
                X509Certificate2Collection originatorCerts,
                X509Certificate2Collection extraStore,
                out Exception exception)
            {
                // When encryptedContent is null Windows seems to decrypt the CEK first,
                // then return a 0 byte answer.

                byte[] cek;

                if (recipientInfo.Pal is ManagedKeyTransPal ktri)
                {
                    cek = ktri.DecryptCek(cert, out exception);
                }
                else
                {
                    exception = new CryptographicException(
                        SR.Cryptography_Cms_RecipientType_NotSupported,
                        recipientInfo.Type.ToString());

                    return null;
                }

                if (exception != null)
                {
                    return null;
                }

                ReadOnlyMemory<byte>? encryptedContent = _envelopedData.EncryptedContentInfo.EncryptedContent;

                if (encryptedContent == null)
                {
                    exception = null;
                    return new ContentInfo(
                        new Oid(_envelopedData.EncryptedContentInfo.ContentType),
                        Array.Empty<byte>());
                }

                int encryptedContentLength = encryptedContent.Value.Length;
                byte[] encryptedContentArray = ArrayPool<byte>.Shared.Rent(encryptedContentLength);
                byte[] decrypted;

                try
                {
                    encryptedContent.Value.CopyTo(encryptedContentArray);

                    AlgorithmIdentifierAsn contentEncryptionAlgorithm =
                        _envelopedData.EncryptedContentInfo.ContentEncryptionAlgorithm;

                    using (SymmetricAlgorithm alg = OpenAlgorithm(contentEncryptionAlgorithm))
                    using (ICryptoTransform decryptor = alg.CreateDecryptor(cek, alg.IV))
                    {
                        decrypted = decryptor.OneShot(
                            encryptedContentArray,
                            0,
                            encryptedContentLength);
                    }
                }
                catch (CryptographicException e)
                {
                    exception = e;
                    return null;
                }
                finally
                {
                    Array.Clear(encryptedContentArray, 0, encryptedContentLength);
                    ArrayPool<byte>.Shared.Return(encryptedContentArray);
                }

                if (_envelopedData.EncryptedContentInfo.ContentType == Oids.Pkcs7Data)
                {
                    byte[] tmp = null;

                    try
                    {
                        AsnReader reader = new AsnReader(decrypted, AsnEncodingRules.BER);

                        if (reader.TryGetPrimitiveOctetStringBytes(out ReadOnlyMemory<byte> contents))
                        {
                            decrypted = contents.ToArray();
                        }
                        else
                        {
                            tmp = ArrayPool<byte>.Shared.Rent(decrypted.Length);

                            if (reader.TryCopyOctetStringBytes(tmp, out int written))
                            {
                                Span<byte> innerContents = new Span<byte>(tmp, 0, written);
                                decrypted = innerContents.ToArray();
                                innerContents.Clear();
                            }
                            else
                            {
                                Debug.Fail("Octet string grew during copy");
                                // If this happens (which requires decrypted was overwritten, which
                                // shouldn't be possible), just leave decrypted alone.
                            }
                        }
                    }
                    catch (CryptographicException)
                    {
                    }
                    finally
                    {
                        if (tmp != null)
                        {
                            // Already cleared
                            ArrayPool<byte>.Shared.Return(tmp);
                        }
                    }
                }

                exception = null;
                return new ContentInfo(
                    new Oid(_envelopedData.EncryptedContentInfo.ContentType),
                    decrypted);
            }

            public override void Dispose()
            {
            }
        }
    }
}
