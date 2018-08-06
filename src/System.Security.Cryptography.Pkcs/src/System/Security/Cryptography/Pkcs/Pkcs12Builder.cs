// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
    public sealed class Pkcs12Builder
    {
        private ReadOnlyMemory<byte> _sealedData;
        private List<ContentInfoAsn> _contents;

        public bool IsSealed => !_sealedData.IsEmpty;

        public void AddSafeContentsEncrypted(
            Pkcs12SafeContents safeContents,
            byte[] passwordBytes,
            PbeParameters pbeParameters)
        {
            AddSafeContentsEncrypted(
                safeContents,
                // Allows null.
                new ReadOnlySpan<byte>(passwordBytes), 
                pbeParameters);
        }

        public void AddSafeContentsEncrypted(
            Pkcs12SafeContents safeContents,
            ReadOnlySpan<byte> passwordBytes,
            PbeParameters pbeParameters)
        {
            if (safeContents == null)
                throw new ArgumentNullException(nameof(safeContents));
            if (pbeParameters == null)
                throw new ArgumentNullException(nameof(pbeParameters));
            if (pbeParameters.IterationCount < 1)
                throw new ArgumentOutOfRangeException(nameof(pbeParameters.IterationCount));
            if (safeContents.ConfidentialityMode != Pkcs12ConfidentialityMode.None)
                throw new ArgumentException(SR.Cryptography_Pkcs12_CannotProcessEncryptedSafeContents, nameof(safeContents));
            if (IsSealed)
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxIsSealed);

            PasswordBasedEncryption.ValidatePbeParameters(
                pbeParameters,
                ReadOnlySpan<char>.Empty,
                passwordBytes);

            byte[] encrypted = safeContents.Encrypt(ReadOnlySpan<char>.Empty, passwordBytes, pbeParameters);

            if (_contents == null)
            {
                _contents = new List<ContentInfoAsn>();
            }

            _contents.Add(
                new ContentInfoAsn
                {
                    ContentType = Oids.Pkcs7Encrypted,
                    Content = encrypted,
                });
        }

        public void AddSafeContentsEncrypted(
            Pkcs12SafeContents safeContents,
            string password,
            PbeParameters pbeParameters)
        {
            AddSafeContentsEncrypted(
                safeContents,
                // This extension invoke allows null
                password.AsSpan(),
                pbeParameters);
        }

        public void AddSafeContentsEncrypted(
            Pkcs12SafeContents safeContents,
            ReadOnlySpan<char> password,
            PbeParameters pbeParameters)
        {
            if (safeContents == null)
                throw new ArgumentNullException(nameof(safeContents));
            if (pbeParameters == null)
                throw new ArgumentNullException(nameof(pbeParameters));
            if (pbeParameters.IterationCount < 1)
                throw new ArgumentOutOfRangeException(nameof(pbeParameters.IterationCount));
            if (safeContents.ConfidentialityMode != Pkcs12ConfidentialityMode.None)
                throw new ArgumentException(SR.Cryptography_Pkcs12_CannotProcessEncryptedSafeContents, nameof(safeContents));
            if (IsSealed)
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxIsSealed);

            PasswordBasedEncryption.ValidatePbeParameters(
                pbeParameters,
                password,
                ReadOnlySpan<byte>.Empty);

            byte[] encrypted = safeContents.Encrypt(password, ReadOnlySpan<byte>.Empty, pbeParameters);

            if (_contents == null)
            {
                _contents = new List<ContentInfoAsn>();
            }

            _contents.Add(
                new ContentInfoAsn
                {
                    ContentType = Oids.Pkcs7Encrypted,
                    Content = encrypted,
                });
        }

        public void AddSafeContentsUnencrypted(Pkcs12SafeContents safeContents)
        {
            if (safeContents == null)
                throw new ArgumentNullException(nameof(safeContents));
            if (IsSealed)
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxIsSealed);

            if (_contents == null)
            {
                _contents = new List<ContentInfoAsn>();
            }

            _contents.Add(safeContents.EncodeToContentInfo());
        }

        public byte[] Encode()
        {
            if (!IsSealed)
            {
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxMustBeSealed);
            }

            return _sealedData.ToArray();
        }

        public void SealWithMac(
            string password,
            HashAlgorithmName hashAlgorithm,
            int iterationCount)
        {
            SealWithMac(
                // This extension invoke allows null
                password.AsSpan(),
                hashAlgorithm,
                iterationCount);
        }

        public void SealWithMac(
            ReadOnlySpan<char> password,
            HashAlgorithmName hashAlgorithm,
            int iterationCount)
        {
            if (iterationCount < 1)
                throw new ArgumentOutOfRangeException(nameof(iterationCount));
            if (IsSealed)
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxIsSealed);

            ContentInfoAsn[] contents = _contents?.ToArray() ?? Array.Empty<ContentInfoAsn>();

            byte[] rentedAuthSafe = null;
            Span<byte> authSafeSpan = default;
            byte[] rentedMac = null;
            Span<byte> macSpan = default;
            Span<byte> salt = stackalloc byte[0];

            try
            {
                using (AsnWriter writer = AsnSerializer.Serialize(contents, AsnEncodingRules.BER))
                using (IncrementalHash hasher = IncrementalHash.CreateHash(hashAlgorithm))
                {
                    ReadOnlySpan<byte> encodedSpan = writer.EncodeAsSpan();

                    rentedAuthSafe = ArrayPool<byte>.Shared.Rent(encodedSpan.Length);
                    encodedSpan.CopyTo(rentedAuthSafe);
                    authSafeSpan = rentedAuthSafe.AsSpan(0, encodedSpan.Length);

                    // Get an array of the proper size for the hash.
                    byte[] macKey = hasher.GetHashAndReset();
                    rentedMac = ArrayPool<byte>.Shared.Rent(macKey.Length);
                    macSpan = rentedMac.AsSpan(0, macKey.Length);

                    // Since the biggest supported hash is SHA-2-512 (64 bytes), the
                    // 128-byte cap here shouldn't ever come into play.
                    Debug.Assert(macKey.Length <= 128);
                    salt = stackalloc byte[Math.Min(macKey.Length, 128)];
                    RandomNumberGenerator.Fill(salt);

                    Pkcs12Kdf.DeriveMacKey(
                        password,
                        hashAlgorithm,
                        iterationCount,
                        salt,
                        macKey);

                    using (IncrementalHash mac = IncrementalHash.CreateHMAC(hashAlgorithm, macKey))
                    {
                        mac.AppendData(encodedSpan);

                        if (!mac.TryGetHashAndReset(macSpan, out int bytesWritten) || bytesWritten != macSpan.Length)
                        {
                            Debug.Fail($"TryGetHashAndReset wrote {bytesWritten} of {macSpan.Length} bytes");
                            throw new CryptographicException();
                        }
                    }
                }

                // https://tools.ietf.org/html/rfc7292#section-4
                //
                // PFX ::= SEQUENCE {
                //   version    INTEGER {v3(3)}(v3,...),
                //   authSafe   ContentInfo,
                //   macData    MacData OPTIONAL
                // }
                using (AsnWriter writer = new AsnWriter(AsnEncodingRules.BER))
                {
                    writer.PushSequence();

                    writer.WriteInteger(3);

                    writer.PushSequence();
                    {
                        writer.WriteObjectIdentifier(Oids.Pkcs7Data);

                        Asn1Tag contextSpecific0 = new Asn1Tag(TagClass.ContextSpecific, 0);

                        writer.PushSequence(contextSpecific0);
                        {
                            writer.WriteOctetString(authSafeSpan);
                            writer.PopSequence(contextSpecific0);
                        }

                        writer.PopSequence();
                    }

                    // https://tools.ietf.org/html/rfc7292#section-4
                    // 
                    // MacData ::= SEQUENCE {
                    //   mac        DigestInfo,
                    //   macSalt    OCTET STRING,
                    //   iterations INTEGER DEFAULT 1
                    //   -- Note: The default is for historical reasons and its use is
                    //   -- deprecated.
                    // }
                    writer.PushSequence();
                    {
                        writer.PushSequence();
                        {
                            writer.PushSequence();
                            {
                                writer.WriteObjectIdentifier(PkcsHelpers.GetOidFromHashAlgorithm(hashAlgorithm));
                                writer.PopSequence();
                            }

                            writer.WriteOctetString(macSpan);
                            writer.PopSequence();
                        }

                        writer.WriteOctetString(salt);

                        if (iterationCount > 1)
                        {
                            writer.WriteInteger(iterationCount);
                        }

                        writer.PopSequence();
                    }

                    writer.PopSequence();
                    _sealedData = writer.Encode();
                }
            }
            finally
            {
                CryptographicOperations.ZeroMemory(macSpan);
                CryptographicOperations.ZeroMemory(authSafeSpan);

                if (rentedMac != null)
                {
                    ArrayPool<byte>.Shared.Return(rentedMac);
                }

                if (rentedAuthSafe != null)
                {
                    ArrayPool<byte>.Shared.Return(rentedAuthSafe);
                }
            }
        }

        public void SealWithoutIntegrity()
        {
            if (IsSealed)
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxIsSealed);

            ContentInfoAsn[] contents = _contents?.ToArray() ?? Array.Empty<ContentInfoAsn>();

            using (AsnWriter contentsWriter = AsnSerializer.Serialize(contents, AsnEncodingRules.BER))
            using (AsnWriter writer = new AsnWriter(AsnEncodingRules.BER))
            {
                // https://tools.ietf.org/html/rfc7292#section-4
                //
                // PFX ::= SEQUENCE {
                //   version    INTEGER {v3(3)}(v3,...),
                //   authSafe   ContentInfo,
                //   macData    MacData OPTIONAL
                // }
                writer.PushSequence();

                writer.WriteInteger(3);

                writer.PushSequence();
                {
                    writer.WriteObjectIdentifier(Oids.Pkcs7Data);

                    Asn1Tag contextSpecific0 = new Asn1Tag(TagClass.ContextSpecific, 0);

                    writer.PushSequence(contextSpecific0);
                    {
                        writer.WriteOctetString(contentsWriter.EncodeAsSpan());
                        writer.PopSequence(contextSpecific0);
                    }

                    writer.PopSequence();
                }

                writer.PopSequence();
                _sealedData = writer.Encode();
            }
        }

        public bool TryEncode(Span<byte> destination, out int bytesWritten)
        {
            if (!IsSealed)
            {
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_PfxMustBeSealed);
            }

            if (destination.Length < _sealedData.Length)
            {
                bytesWritten = 0;
                return false;
            }

            _sealedData.Span.CopyTo(destination);
            bytesWritten = _sealedData.Length;
            return true;
        }
    }
}
