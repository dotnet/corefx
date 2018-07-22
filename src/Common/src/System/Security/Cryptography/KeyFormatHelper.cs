// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography
{
    internal static class KeyFormatHelper
    {
        internal delegate void KeyReader<TRet, TParsed>(in TParsed key, in AlgorithmIdentifierAsn algId, out TRet ret);

        internal static unsafe void ReadSubjectPublicKeyInfo<TRet, TParsed>(
            string[] validOids,
            ReadOnlySpan<byte> source,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    ReadSubjectPublicKeyInfo(validOids, manager.Memory, keyReader, out bytesRead, out ret);
                }
            }
        }

        internal static ReadOnlyMemory<byte> ReadSubjectPublicKeyInfo(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            // X.509 SubjectPublicKeyInfo is described as DER.
            SubjectPublicKeyInfoAsn spki =
                AsnSerializer.Deserialize<SubjectPublicKeyInfoAsn>(source, AsnEncodingRules.DER, out int read);

            if (Array.IndexOf(validOids, spki.Algorithm.Algorithm.Value) < 0)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            bytesRead = read;
            return spki.SubjectPublicKey;
        }

        private static void ReadSubjectPublicKeyInfo<TRet, TParsed>(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            // X.509 SubjectPublicKeyInfo is described as DER.
            SubjectPublicKeyInfoAsn spki =
                AsnSerializer.Deserialize<SubjectPublicKeyInfoAsn>(source, AsnEncodingRules.DER, out int read);

            if (Array.IndexOf(validOids, spki.Algorithm.Algorithm.Value) < 0)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            TParsed parsed;

            if (typeof(TParsed) == typeof(ReadOnlyMemory<byte>))
            {
                ReadOnlyMemory<byte> tmp = spki.SubjectPublicKey;
                parsed = (TParsed)(object)tmp;
            }
            else
            {
                // Fails if there are unconsumed bytes.
                parsed = AsnSerializer.Deserialize<TParsed>(
                    spki.SubjectPublicKey,
                    AsnEncodingRules.DER);
            }

            keyReader(parsed, spki.Algorithm, out ret);
            bytesRead = read;
        }

        internal static unsafe void ReadPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlySpan<byte> source,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    ReadPkcs8(validOids, manager.Memory, keyReader, out bytesRead, out ret);
                }
            }
        }

        internal static ReadOnlyMemory<byte> ReadPkcs8(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            PrivateKeyInfoAsn privateKeyInfo =
                AsnSerializer.Deserialize<PrivateKeyInfoAsn>(source, AsnEncodingRules.BER, out int read);

            if (Array.IndexOf(validOids, privateKeyInfo.PrivateKeyAlgorithm.Algorithm.Value) < 0)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            bytesRead = read;
            return privateKeyInfo.PrivateKey;
        }

        private static void ReadPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            PrivateKeyInfoAsn privateKeyInfo =
                AsnSerializer.Deserialize<PrivateKeyInfoAsn>(source, AsnEncodingRules.BER, out int read);

            if (Array.IndexOf(validOids, privateKeyInfo.PrivateKeyAlgorithm.Algorithm.Value) < 0)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            // Fails if there are unconsumed bytes.
            TParsed parsed = AsnSerializer.Deserialize<TParsed>(
                privateKeyInfo.PrivateKey,
                AsnEncodingRules.BER);

            keyReader(parsed, privateKeyInfo.PrivateKeyAlgorithm, out ret);
            bytesRead = read;
        }

        internal static unsafe void ReadEncryptedPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlySpan<byte> source,
            ReadOnlySpan<char> password,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    ReadEncryptedPkcs8(validOids, manager.Memory, password, keyReader, out bytesRead, out ret);
                }
            }
        }

        internal static unsafe void ReadEncryptedPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlySpan<byte> source,
            ReadOnlySpan<byte> passwordBytes,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    ReadEncryptedPkcs8(validOids, manager.Memory, passwordBytes, keyReader, out bytesRead, out ret);
                }
            }
        }

        private static void ReadEncryptedPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            ReadOnlySpan<char> password,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            ReadEncryptedPkcs8(
                validOids,
                source.Span,
                password,
                ReadOnlySpan<byte>.Empty,
                keyReader,
                out bytesRead,
                out ret);
        }

        private static void ReadEncryptedPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            ReadOnlySpan<byte> passwordBytes,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            ReadEncryptedPkcs8(
                validOids,
                source,
                ReadOnlySpan<char>.Empty,
                passwordBytes,
                keyReader,
                out bytesRead,
                out ret);
        }

        private static unsafe void ReadEncryptedPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlySpan<byte> source,
            ReadOnlySpan<char> password,
            ReadOnlySpan<byte> passwordBytes,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    ReadEncryptedPkcs8(
                        validOids,
                        manager.Memory,
                        password,
                        passwordBytes,
                        keyReader,
                        out bytesRead,
                        out ret);
                }
            }
        }

        private static void ReadEncryptedPkcs8<TRet, TParsed>(
            string[] validOids,
            ReadOnlyMemory<byte> source,
            ReadOnlySpan<char> password,
            ReadOnlySpan<byte> passwordBytes,
            KeyReader<TRet, TParsed> keyReader,
            out int bytesRead,
            out TRet ret)
        {
            EncryptedPrivateKeyInfoAsn epki =
                AsnSerializer.Deserialize<EncryptedPrivateKeyInfoAsn>(source, AsnEncodingRules.BER, out int read);

            // No supported encryption algorithms produce more bytes of decryption output than there
            // were of decryption input.
            byte[] decrypted = ArrayPool<byte>.Shared.Rent(epki.EncryptedData.Length);
            Memory<byte> decryptedMemory = decrypted;

            try
            {
                int decryptedBytes = PasswordBasedEncryption.Decrypt(
                    epki.EncryptionAlgorithm,
                    password,
                    passwordBytes,
                    epki.EncryptedData.Span,
                    decrypted);

                decryptedMemory = decryptedMemory.Slice(0, decryptedBytes);

                ReadPkcs8(
                    validOids,
                    decryptedMemory,
                    keyReader,
                    out int innerRead,
                    out ret);

                if (innerRead != decryptedMemory.Length)
                {
                    ret = default;
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                bytesRead = read;
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(decryptedMemory.Span);
                ArrayPool<byte>.Shared.Return(decrypted);
            }
        }

        internal static AsnWriter WritePkcs8(AsnWriter algorithmIdentifierWriter, AsnWriter privateKeyWriter)
        {
            // Ensure both input writers are balanced.
            ReadOnlySpan<byte> algorithmIdentifier = algorithmIdentifierWriter.EncodeAsSpan();
            ReadOnlySpan<byte> privateKey = privateKeyWriter.EncodeAsSpan();

            Debug.Assert(algorithmIdentifier.Length > 0, "algorithmIdentifier was empty");
            Debug.Assert(algorithmIdentifier[0] == 0x30, "algorithmIdentifier is not a constructed sequence");
            Debug.Assert(privateKey.Length > 0, "privateKey was empty");

            // https://tools.ietf.org/html/rfc5208#section-5
            //
            // PrivateKeyInfo ::= SEQUENCE {
            //   version                   Version,
            //   privateKeyAlgorithm       PrivateKeyAlgorithmIdentifier,
            //   privateKey                PrivateKey,
            //   attributes           [0]  IMPLICIT Attributes OPTIONAL }
            // 
            // Version ::= INTEGER
            // PrivateKeyAlgorithmIdentifier ::= AlgorithmIdentifier
            // PrivateKey ::= OCTET STRING
            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);

            // PrivateKeyInfo
            writer.PushSequence();

            // https://tools.ietf.org/html/rfc5208#section-5 says the current version is 0.
            writer.WriteInteger(0);

            // PKI.Algorithm (AlgorithmIdentifier)
            WriteEncodedSpan(writer, algorithmIdentifier);
            
            // PKI.privateKey
            writer.WriteOctetString(privateKey);

            // We don't currently accept attributes, so... done.
            writer.PopSequence();
            return writer;
        }

        internal static unsafe AsnWriter WriteEncryptedPkcs8(
            ReadOnlySpan<char> password,
            AsnWriter pkcs8Writer,
            PbeParameters pbeParameters)
        {
            return WriteEncryptedPkcs8(
                password,
                ReadOnlySpan<byte>.Empty,
                pkcs8Writer,
                pbeParameters);
        }

        internal static AsnWriter WriteEncryptedPkcs8(
            ReadOnlySpan<byte> passwordBytes,
            AsnWriter pkcs8Writer,
            PbeParameters pbeParameters)
        {
            return WriteEncryptedPkcs8(
                ReadOnlySpan<char>.Empty,
                passwordBytes,
                pkcs8Writer,
                pbeParameters);
        }

        private static unsafe AsnWriter WriteEncryptedPkcs8(
            ReadOnlySpan<char> password,
            ReadOnlySpan<byte> passwordBytes,
            AsnWriter pkcs8Writer,
            PbeParameters pbeParameters)
        {
            ReadOnlySpan<byte> pkcs8Span = pkcs8Writer.EncodeAsSpan();

            PasswordBasedEncryption.InitiateEncryption(
                pbeParameters,
                out SymmetricAlgorithm cipher,
                out string hmacOid,
                out string encryptionAlgorithmOid,
                out bool isPkcs12);

            byte[] encryptedRent = null;
            Span<byte> encryptedSpan = default;
            AsnWriter writer = null;

            try
            {
                Debug.Assert(cipher.BlockSize <= 128, $"Encountered unexpected block size: {cipher.BlockSize}");
                Span<byte> iv = stackalloc byte[cipher.BlockSize / 8];
                Span<byte> salt = stackalloc byte[16];

                // We need at least one block size beyond the input data size.
                encryptedRent = ArrayPool<byte>.Shared.Rent(
                    checked(pkcs8Span.Length + (cipher.BlockSize / 8)));

                RandomNumberGenerator.Fill(salt);

                int written = PasswordBasedEncryption.Encrypt(
                    password,
                    passwordBytes,
                    cipher,
                    isPkcs12,
                    pkcs8Span,
                    pbeParameters,
                    salt,
                    encryptedRent,
                    iv);

                encryptedSpan = encryptedRent.AsSpan(0, written);

                writer = new AsnWriter(AsnEncodingRules.DER);

                // PKCS8 EncryptedPrivateKeyInfo
                writer.PushSequence();

                // EncryptedPrivateKeyInfo.encryptionAlgorithm
                PasswordBasedEncryption.WritePbeAlgorithmIdentifier(
                    writer,
                    isPkcs12,
                    encryptionAlgorithmOid,
                    salt,
                    pbeParameters.IterationCount,
                    hmacOid,
                    iv);

                // encryptedData
                writer.WriteOctetString(encryptedSpan);
                writer.PopSequence();

                AsnWriter ret = writer;
                // Don't dispose writer on the way out.
                writer = null;
                return ret;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(encryptedSpan);
                ArrayPool<byte>.Shared.Return(encryptedRent);

                writer?.Dispose();
                cipher.Dispose();
            }
        }

        private static unsafe void WriteEncodedSpan(AsnWriter writer, ReadOnlySpan<byte> encodedValue)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(encodedValue))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, encodedValue.Length))
                {
                    writer.WriteEncodedValue(manager.Memory);
                }
            }
        }

        internal static ArraySegment<byte> DecryptPkcs8(
            ReadOnlySpan<char> inputPassword,
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            return DecryptPkcs8(
                inputPassword,
                ReadOnlySpan<byte>.Empty,
                source,
                out bytesRead);
        }

        internal static ArraySegment<byte> DecryptPkcs8(
            ReadOnlySpan<byte> inputPasswordBytes,
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            return DecryptPkcs8(
                ReadOnlySpan<char>.Empty,
                inputPasswordBytes,
                source,
                out bytesRead);
        }

        private static ArraySegment<byte> DecryptPkcs8(
            ReadOnlySpan<char> inputPassword,
            ReadOnlySpan<byte> inputPasswordBytes,
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            EncryptedPrivateKeyInfoAsn epki =
                AsnSerializer.Deserialize<EncryptedPrivateKeyInfoAsn>(source, AsnEncodingRules.BER, out bytesRead);

            // No supported encryption algorithms produce more bytes of decryption output than there
            // were of decryption input.
            byte[] decrypted = ArrayPool<byte>.Shared.Rent(epki.EncryptedData.Length);

            try
            {
                int decryptedBytes = PasswordBasedEncryption.Decrypt(
                    epki.EncryptionAlgorithm,
                    inputPassword,
                    inputPasswordBytes,
                    epki.EncryptedData.Span,
                    decrypted);

                return new ArraySegment<byte>(decrypted, 0, decryptedBytes);
            }
            catch (CryptographicException e)
            {
                ArrayPool<byte>.Shared.Return(decrypted);
                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
            }
        }

        internal static AsnWriter ReencryptPkcs8(
            ReadOnlySpan<char> inputPassword,
            ReadOnlyMemory<byte> current,
            ReadOnlySpan<char> newPassword,
            PbeParameters pbeParameters)
        {
            ArraySegment<byte> decrypted = DecryptPkcs8(
                inputPassword,
                current,
                out int bytesRead);

            Memory<byte> decryptedMemory = decrypted;

            try
            {
                if (bytesRead != current.Length)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                using (AsnWriter pkcs8Writer = new AsnWriter(AsnEncodingRules.BER))
                {
                    pkcs8Writer.WriteEncodedValue(decryptedMemory);

                    return WriteEncryptedPkcs8(
                        newPassword,
                        pkcs8Writer,
                        pbeParameters);
                }
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(decryptedMemory.Span);
                ArrayPool<byte>.Shared.Return(decrypted.Array);
            }
        }

        internal static AsnWriter ReencryptPkcs8(
            ReadOnlySpan<char> inputPassword,
            ReadOnlyMemory<byte> current,
            ReadOnlySpan<byte> newPasswordBytes,
            PbeParameters pbeParameters)
        {
            ArraySegment<byte> decrypted = DecryptPkcs8(
                inputPassword,
                current,
                out int bytesRead);

            Memory<byte> decryptedMemory = decrypted;

            try
            {
                if (bytesRead != current.Length)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                using (AsnWriter pkcs8Writer = new AsnWriter(AsnEncodingRules.BER))
                {
                    pkcs8Writer.WriteEncodedValue(decryptedMemory);

                    return WriteEncryptedPkcs8(
                        newPasswordBytes,
                        pkcs8Writer,
                        pbeParameters);
                }
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(decryptedMemory.Span);
                ArrayPool<byte>.Shared.Return(decrypted.Array);
            }
        }
    }
}
