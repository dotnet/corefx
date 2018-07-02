// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;
using Internal.NativeCrypto;
using static Interop.BCrypt;

namespace System.Security.Cryptography
{
    partial class AesAEAD
    {
        public static unsafe KeySizes GetTagLengths(SafeAlgorithmHandle algorithm)
        {
            BCRYPT_AUTH_TAG_LENGTHS_STRUCT ret;
            int bytesWritten;
            NTSTATUS ntStatus = Interop.BCrypt.BCryptGetProperty(algorithm, BCryptPropertyStrings.BCRYPT_AUTH_TAG_LENGTH, &ret, sizeof(BCRYPT_AUTH_TAG_LENGTHS_STRUCT), out bytesWritten, 0);

            if (ntStatus != NTSTATUS.STATUS_SUCCESS)
            {
                throw CreateCryptographicException(ntStatus);
            }

            return new KeySizes(ret.MinLength, ret.MaxLength, ret.Increment);
        }

        public static unsafe void Encrypt(SafeAlgorithmHandle algorithm, SafeKeyHandle keyHandle, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> associatedData, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag)
        {
            fixed (byte* plaintextBytes = plaintext)
            fixed (byte* nonceBytes = nonce)
            fixed (byte* ciphertextBytes = ciphertext)
            fixed (byte* tagBytes = tag)
            fixed (byte* additionalDataBytes = associatedData)
            {
                BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO authInfo = BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.Create();
                authInfo.pbNonce = nonceBytes;
                authInfo.cbNonce = nonce.Length;
                authInfo.pbTag = tagBytes;
                authInfo.cbTag = tag.Length;
                if (associatedData.Length != 0)
                {
                    authInfo.pbAuthData = additionalDataBytes;
                    authInfo.cbAuthData = associatedData.Length;
                }

                NTSTATUS ntStatus = Cng.Interop.BCryptEncrypt(keyHandle, plaintextBytes, plaintext.Length, new IntPtr(&authInfo), null, 0, ciphertextBytes, ciphertext.Length, out int ciphertextBytesWritten, 0);

                if (ntStatus != NTSTATUS.STATUS_SUCCESS)
                {
                    throw CreateCryptographicException(ntStatus);
                }
            }
        }

        public static unsafe void Decrypt(SafeAlgorithmHandle algorithm, SafeKeyHandle keyHandle, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> associatedData, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext)
        {
            fixed (byte* plaintextBytes = plaintext)
            fixed (byte* nonceBytes = nonce)
            fixed (byte* ciphertextBytes = ciphertext)
            fixed (byte* tagBytes = tag)
            fixed (byte* additionalDataBytes = associatedData)
            {
                BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO authInfo = BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.Create();
                authInfo.pbNonce = nonceBytes;
                authInfo.cbNonce = nonce.Length;
                authInfo.pbTag = tagBytes;
                authInfo.cbTag = tag.Length;
                if (associatedData.Length != 0)
                {
                    authInfo.pbAuthData = additionalDataBytes;
                    authInfo.cbAuthData = associatedData.Length;
                }

                NTSTATUS ntStatus = Cng.Interop.BCryptDecrypt(keyHandle, ciphertextBytes, ciphertext.Length, new IntPtr(&authInfo), null, 0, plaintextBytes, plaintext.Length, out int plaintextBytesWritten, 0);

                switch (ntStatus)
                {
                    case NTSTATUS.STATUS_SUCCESS:
                        return;
                    case NTSTATUS.STATUS_AUTH_TAG_MISMATCH:
                        throw new CryptographicException(SR.Cryptography_AuthTagMismatch);
                    default:
                        throw CreateCryptographicException(ntStatus);
                }
            }
        }
    }
}
