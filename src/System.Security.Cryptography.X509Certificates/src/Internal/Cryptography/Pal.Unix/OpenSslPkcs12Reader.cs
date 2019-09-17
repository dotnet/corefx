// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Asn1.Pkcs12;
using System.Security.Cryptography.Asn1.Pkcs7;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Threading;

namespace Internal.Cryptography.Pal
{
    internal sealed class OpenSslPkcs12Reader : IDisposable
    {
        private const string DecryptedSentinel = nameof(OpenSslPkcs12Reader);

        private PfxAsn _pfxAsn;
        private ContentInfoAsn[] _safeContentsValues;
        private OpenSslX509CertificateReader[] _certs;

        private OpenSslPkcs12Reader(AsnReader reader)
        {
            PfxAsn.Decode(reader, out _pfxAsn);

            if (_pfxAsn.AuthSafe.ContentType != Oids.Pkcs7Data)
            {
                throw new CryptographicException("Unsupported PFX.");
            }
        }

        public static bool TryRead(byte[] data, out OpenSslPkcs12Reader pkcs12Reader) =>
            TryRead(data, out pkcs12Reader, out _, captureException: false);

        public static bool TryRead(byte[] data, out OpenSslPkcs12Reader pkcs12Reader, out Exception openSslException) =>
            TryRead(data, out pkcs12Reader, out openSslException, captureException: true);

        public static bool TryRead(SafeBioHandle fileBio, out OpenSslPkcs12Reader pkcs12Reader, out Exception openSslException) =>
            TryRead(fileBio, out pkcs12Reader, out openSslException, captureException: true);

        public void Dispose()
        {
            ContentInfoAsn[] rentedContents = Interlocked.Exchange(ref _safeContentsValues, null);
            OpenSslX509CertificateReader[] rentedCerts = Interlocked.Exchange(ref _certs, null);

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
                ArrayPool<OpenSslX509CertificateReader>.Shared.Return(rentedCerts, clearArray: true);
            }
        }

        public unsafe void Decrypt(SafePasswordHandle password)
        {
            ReadOnlyMemory<byte> authSafeSpan =
                Helpers.DecodeOctetStringAsMemory(_pfxAsn.AuthSafe.Content);

            bool hasRef = false;
            password.DangerousAddRef(ref hasRef);

            try
            {
                ReadOnlySpan<char> passwordChars =
                    new ReadOnlySpan<char>((char*)password.DangerousGetHandle(), password.Length);

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

                _certs = ArrayPool<OpenSslX509CertificateReader>.Shared.Rent(certBagIdx);
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

                    if (!OpenSslX509CertificateReader.TryReadX509Der(x509Data.Span, out ICertificatePal pal))
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    _certs[certBagIdx] = (OpenSslX509CertificateReader)pal;

                    if (matchingKeyIdx != -1)
                    {
                        _certs[certBagIdx].SetPrivateKey(GetPrivateKey(keys[matchingKeyIdx]));
                    }
                }

                Array.Reverse(_certs, 0, certsCount);
            }
            catch
            {
                if (_certs != null)
                {
                    foreach (OpenSslX509CertificateReader cert in _certs)
                    {
                        cert?.Dispose();
                    }
                }

                throw;
            }
            finally
            {
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

        private static AsymmetricAlgorithm LoadKey(SafeBagAsn safeBag, ReadOnlySpan<char> password)
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

        private static AsymmetricAlgorithm LoadKey(in ReadOnlyMemory<byte> pkcs8)
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

        private static SafeEvpPKeyHandle GetPrivateKey(AsymmetricAlgorithm key)
        {
            if (key is RSAOpenSsl rsa)
                return rsa.DuplicateKeyHandle();

            if (key is DSAOpenSsl dsa)
                return dsa.DuplicateKeyHandle();

            return ((ECDiffieHellmanOpenSsl)key).DuplicateKeyHandle();
        }

        public ArraySegment<OpenSslX509CertificateReader> ReadCertificates()
        {
            Debug.Assert(_certs != null);

            int nullIdx = Array.IndexOf(_certs, null);

            if (nullIdx >= 0)
            {
                return new ArraySegment<OpenSslX509CertificateReader>(
                    _certs,
                    0,
                    nullIdx);
            }

            return _certs;
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
                AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
                ReadOnlyMemory<byte> encodedData = reader.PeekEncodedValue();

                if (encodedData.Length != data.Length)
                {
                    reader = new AsnReader(encodedData, AsnEncodingRules.BER);
                }

                pkcs12Reader = new OpenSslPkcs12Reader(reader);
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

        private static bool TryRead(
            SafeBioHandle fileBio,
            out OpenSslPkcs12Reader pkcs12Reader,
            out Exception openSslException,
            bool captureException)
        {
            _ = fileBio;
            pkcs12Reader = null;
            openSslException = captureException ? new NotImplementedException() : null;
            return false;
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
    }
}
