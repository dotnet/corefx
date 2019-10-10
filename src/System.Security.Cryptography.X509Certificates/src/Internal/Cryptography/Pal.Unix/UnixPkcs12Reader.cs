// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Asn1.Pkcs12;
using System.Security.Cryptography.Asn1.Pkcs7;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal abstract class UnixPkcs12Reader : IDisposable
    {
        private const string DecryptedSentinel = nameof(UnixPkcs12Reader);

        private PfxAsn _pfxAsn;
        private ContentInfoAsn[] _safeContentsValues;
        private CertAndKey[] _certs;
        private int _certCount;

        protected abstract ICertificatePalCore ReadX509Der(ReadOnlyMemory<byte> data);
        protected abstract AsymmetricAlgorithm LoadKey(ReadOnlyMemory<byte> safeBagBagValue);

        protected void ParsePkcs12(byte[] data)
        {
            // RFC7292 specifies BER instead of DER
            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
            ReadOnlyMemory<byte> encodedData = reader.PeekEncodedValue();

            // Windows compatibility: Ignore trailing data.
            if (encodedData.Length != data.Length)
            {
                reader = new AsnReader(encodedData, AsnEncodingRules.BER);
            }

            PfxAsn.Decode(reader, out _pfxAsn);

            if (_pfxAsn.AuthSafe.ContentType != Oids.Pkcs7Data)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }
        }

        internal CertAndKey GetSingleCert()
        {
            CertAndKey[] certs = _certs;
            Debug.Assert(certs != null);

            if (_certCount < 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            CertAndKey ret;

            for (int i = _certCount - 1; i >= 0; --i)
            {
                if (certs[i].Key != null)
                {
                    ret = certs[i];
                    certs[i] = default;
                    return ret;
                }
            }

            ret = certs[_certCount - 1];
            certs[_certCount - 1] = default;
            return ret;
        }

        internal int GetCertCount()
        {
            return _certCount;
        }

        internal IEnumerable<CertAndKey> EnumerateAll()
        {
            while (_certCount > 0)
            {
                int idx = _certCount - 1;
                CertAndKey ret = _certs[idx];
                _certs[idx] = default;
                _certCount--;
                yield return ret;
            }
        }

        public void Dispose()
        {
            ContentInfoAsn[] rentedContents = Interlocked.Exchange(ref _safeContentsValues, null);
            CertAndKey[] rentedCerts = Interlocked.Exchange(ref _certs, null);

            if (rentedContents != null)
            {
                for (int i = 0; i < rentedContents.Length; i++)
                {
                    string contentType = rentedContents[0].ContentType;

                    if (contentType == null)
                    {
                        break;
                    }

                    if (contentType == DecryptedSentinel)
                    {
                        ReadOnlyMemory<byte> content = rentedContents[0].Content;

                        if (!MemoryMarshal.TryGetArray(content, out ArraySegment<byte> segment))
                        {
                            Debug.Fail("Couldn't unpack decrypted buffer.");
                        }

                        CryptoPool.Return(segment.Array, segment.Count);
                        rentedContents[0].Content = default;
                    }
                }

                ArrayPool<ContentInfoAsn>.Shared.Return(rentedContents, clearArray: true);
            }

            if (rentedCerts != null)
            {
                for (int i = _certCount - 1; i >= 0; --i)
                {
                    rentedCerts[i].Dispose();
                }

                ArrayPool<CertAndKey>.Shared.Return(rentedCerts, clearArray: true);
            }
        }

        public void Decrypt(SafePasswordHandle password)
        {
            ReadOnlyMemory<byte> authSafeSpan =
                Helpers.DecodeOctetStringAsMemory(_pfxAsn.AuthSafe.Content);

            bool hasRef = false;
            password.DangerousAddRef(ref hasRef);

            try
            {
                ReadOnlySpan<char> passwordChars = password.DangerousGetSpan();

                if (_pfxAsn.MacData.HasValue)
                {
                    VerifyAndDecrypt(passwordChars, authSafeSpan);
                }
                else
                {
                    Decrypt(passwordChars, authSafeSpan);
                }
            }
            catch (Exception e)
            {
                throw new CryptographicException("Bad password.", e);
            }
            finally
            {
                password.DangerousRelease();
            }
        }

        private void VerifyAndDecrypt(ReadOnlySpan<char> password, ReadOnlyMemory<byte> authSafeContents)
        {
            Debug.Assert(_pfxAsn.MacData.HasValue);
            ReadOnlySpan<byte> authSafeSpan = authSafeContents.Span;

            if (password.Length == 0)
            {
                if (_pfxAsn.VerifyMac("", authSafeSpan))
                {
                    Decrypt("", authSafeContents);
                    return;
                }

                if (_pfxAsn.VerifyMac(default, authSafeSpan))
                {
                    Decrypt(default, authSafeContents);
                    return;
                }
            }
            else if (_pfxAsn.VerifyMac(password, authSafeSpan))
            {
                Decrypt(password, authSafeContents);
                return;
            }

            throw new CryptographicException("Password did not verify MAC.");
        }

        private void Decrypt(ReadOnlySpan<char> password, ReadOnlyMemory<byte> authSafeContents)
        {
            if (_safeContentsValues == null)
            {
                // The expected number of ContentInfoAsns to read is 2, one encrypted (contains certs),
                // and one plain (contains encrypted keys)
                ContentInfoAsn[] rented = ArrayPool<ContentInfoAsn>.Shared.Rent(10);

                AsnReader outer = new AsnReader(authSafeContents, AsnEncodingRules.BER);
                AsnReader reader = outer.ReadSequence();
                outer.ThrowIfNotEmpty();
                int i = 0;

                while (reader.HasData)
                {
                    GrowIfNeeded(ref rented, i);
                    ContentInfoAsn.Decode(reader, out rented[i]);
                    i++;
                }

                rented.AsSpan(i).Clear();
                _safeContentsValues = rented;
            }

            // The average PFX contains one cert, and one key.
            // The next most common PFX contains 3 certs, and one key.
            //
            // Nothing requires that there be fewer keys than certs,
            // but it's sort of nonsensical when loading this way.
            CertBagAsn[] certBags = ArrayPool<CertBagAsn>.Shared.Rent(10);
            AttributeAsn[][] certBagAttrs = ArrayPool<AttributeAsn[]>.Shared.Rent(10);
            SafeBagAsn[] keyBags = ArrayPool<SafeBagAsn>.Shared.Rent(10);
            AsymmetricAlgorithm[] keys = null;
            int certBagIdx = 0;
            int keyBagIdx = 0;

            try
            {
                for (int i = 0; i < _safeContentsValues.Length; i++)
                {
                    string contentType = _safeContentsValues[i].ContentType;
                    bool process = false;

                    if (contentType == null)
                    {
                        break;
                    }

                    // Should enveloped throw here?
                    if (contentType == Oids.Pkcs7Data)
                    {
                        process = true;
                    }
                    else if (contentType == Oids.Pkcs7Encrypted)
                    {
                        DecryptSafeContents(password, ref _safeContentsValues[i]);
                        process = true;
                    }

                    if (process)
                    {
                        ProcessSafeContents(
                            _safeContentsValues[i],
                            ref certBags,
                            ref certBagAttrs,
                            ref certBagIdx,
                            ref keyBags,
                            ref keyBagIdx);
                    }
                }

                _certs = ArrayPool<CertAndKey>.Shared.Rent(certBagIdx);
                _certs.AsSpan().Clear();

                keys = ArrayPool<AsymmetricAlgorithm>.Shared.Rent(keyBagIdx);
                keys.AsSpan().Clear();

                // Windows Compat: Load all keys before matching.
                // Unloadable key? Unloadable PFX.
                for (int i = keyBagIdx - 1; i >= 0; i--)
                {
                    keys[i] = LoadKey(keyBags[i], password);
                }

                int certsCount = certBagIdx;

                for (certBagIdx--; certBagIdx >= 0; certBagIdx--)
                {
                    int matchingKeyIdx = -1;

                    foreach (AttributeAsn attr in certBagAttrs[certBagIdx] ?? Array.Empty<AttributeAsn>())
                    {
                        if (attr.AttrType.Value == Oids.LocalKeyId && attr.AttrValues.Length > 0)
                        {
                            matchingKeyIdx = FindMatchingKey(
                                keyBags,
                                keyBagIdx,
                                Helpers.DecodeOctetStringAsMemory(attr.AttrValues[0]).Span);

                            // Only try the first one.
                            break;
                        }
                    }

                    ReadOnlyMemory<byte> x509Data =
                        Helpers.DecodeOctetStringAsMemory(certBags[certBagIdx].CertValue);

                    _certs[certBagIdx].Cert = ReadX509Der(x509Data);

                    if (matchingKeyIdx != -1)
                    {
                        _certs[certBagIdx].Key = keys[matchingKeyIdx];
                        keys[matchingKeyIdx] = null;
                    }
                }

                _certCount = certsCount;
            }
            catch
            {
                if (_certs != null)
                {
                    foreach (CertAndKey certAndKey in _certs)
                    {
                        certAndKey.Dispose();
                    }
                }

                throw;
            }
            finally
            {
                if (keys != null)
                {
                    foreach (AsymmetricAlgorithm key in keys)
                    {
                        key?.Dispose();
                    }

                    ArrayPool<AsymmetricAlgorithm>.Shared.Return(keys);
                }

                ArrayPool<CertBagAsn>.Shared.Return(certBags, clearArray: true);
                ArrayPool<AttributeAsn[]>.Shared.Return(certBagAttrs, clearArray: true);
                ArrayPool<SafeBagAsn>.Shared.Return(keyBags, clearArray: true);
            }
        }

        private static int FindMatchingKey(
            SafeBagAsn[] keyBags,
            int keyBagCount,
            ReadOnlySpan<byte> localKeyId)
        {
            for (int i = 0; i < keyBagCount; i++)
            {
                foreach (AttributeAsn attr in keyBags[i].BagAttributes)
                {
                    if (attr.AttrType.Value == Oids.LocalKeyId && attr.AttrValues.Length > 0)
                    {
                        ReadOnlyMemory<byte> curKeyId =
                            Helpers.DecodeOctetStringAsMemory(attr.AttrValues[0]);

                        if (curKeyId.Span.SequenceEqual(localKeyId))
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        private static void DecryptSafeContents(
            ReadOnlySpan<char> password,
            ref ContentInfoAsn safeContentsAsn)
        {
            EncryptedDataAsn encryptedData =
                EncryptedDataAsn.Decode(safeContentsAsn.Content, AsnEncodingRules.BER);

            // https://tools.ietf.org/html/rfc5652#section-8
            if (encryptedData.Version != 0 && encryptedData.Version != 2)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // Since the contents are supposed to be the BER-encoding of an instance of
            // SafeContents (https://tools.ietf.org/html/rfc7292#section-4.1) that implies the
            // content type is simply "data", and that content is present.
            if (encryptedData.EncryptedContentInfo.ContentType != Oids.Pkcs7Data)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (!encryptedData.EncryptedContentInfo.EncryptedContent.HasValue)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            int encryptedValueLength = encryptedData.EncryptedContentInfo.EncryptedContent.Value.Length;

            byte[] destination = CryptoPool.Rent(encryptedValueLength);

            int written = PasswordBasedEncryption.Decrypt(
                encryptedData.EncryptedContentInfo.ContentEncryptionAlgorithm,
                password,
                default,
                encryptedData.EncryptedContentInfo.EncryptedContent.Value.Span,
                destination);

            safeContentsAsn.Content = destination.AsMemory(0, written);
            safeContentsAsn.ContentType = DecryptedSentinel;
        }

        private static void ProcessSafeContents(
            in ContentInfoAsn safeContentsAsn,
            ref CertBagAsn[] certBags,
            ref AttributeAsn[][] certBagAttrs,
            ref int certBagIdx,
            ref SafeBagAsn[] keyBags,
            ref int keyBagIdx)
        {
            ReadOnlyMemory<byte> contentData = safeContentsAsn.Content;

            if (safeContentsAsn.ContentType == Oids.Pkcs7Data)
            {
                contentData = Helpers.DecodeOctetStringAsMemory(contentData);
            }

            AsnReader outer = new AsnReader(contentData, AsnEncodingRules.BER);
            AsnReader reader = outer.ReadSequence();
            outer.ThrowIfNotEmpty();

            while (reader.HasData)
            {
                SafeBagAsn.Decode(reader, out SafeBagAsn bag);

                if (bag.BagId == Oids.Pkcs12CertBag)
                {
                    CertBagAsn certBag = CertBagAsn.Decode(bag.BagValue, AsnEncodingRules.BER);

                    if (certBag.CertId == Oids.Pkcs12X509CertBagType)
                    {
                        GrowIfNeeded(ref certBags, certBagIdx);
                        GrowIfNeeded(ref certBagAttrs, certBagIdx);
                        certBags[certBagIdx] = certBag;
                        certBagAttrs[certBagIdx] = bag.BagAttributes;
                        certBagIdx++;
                    }
                }
                else if (bag.BagId == Oids.Pkcs12KeyBag || bag.BagId == Oids.Pkcs12ShroudedKeyBag)
                {
                    GrowIfNeeded(ref keyBags, keyBagIdx);
                    keyBags[keyBagIdx] = bag;
                    keyBagIdx++;
                }
            }
        }

        private AsymmetricAlgorithm LoadKey(SafeBagAsn safeBag, ReadOnlySpan<char> password)
        {
            if (safeBag.BagId == Oids.Pkcs12ShroudedKeyBag)
            {
                ArraySegment<byte> decrypted = KeyFormatHelper.DecryptPkcs8(
                    password,
                    safeBag.BagValue,
                    out int localRead);

                try
                {
                    if (localRead != safeBag.BagValue.Length)
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    return LoadKey(decrypted.AsMemory());
                }
                finally
                {
                    CryptoPool.Return(decrypted.Array, clearSize: decrypted.Count);
                }
            }

            Debug.Assert(safeBag.BagId == Oids.Pkcs12KeyBag);
            return LoadKey(safeBag.BagValue);
        }

        private static void GrowIfNeeded<T>(ref T[] array, int idx)
        {
            T[] oldRent = array;

            if (idx >= array.Length)
            {
                T[] newRent = ArrayPool<T>.Shared.Rent(array.Length * 2);
                Array.Copy(oldRent, 0, newRent, 0, idx);
                array = newRent;
                ArrayPool<T>.Shared.Return(oldRent, clearArray: true);
            }
        }

        internal struct CertAndKey
        {
            internal ICertificatePalCore Cert;
            internal AsymmetricAlgorithm Key;

            internal void Dispose()
            {
                Cert?.Dispose();
                Key?.Dispose();
            }
        }
    }
}
