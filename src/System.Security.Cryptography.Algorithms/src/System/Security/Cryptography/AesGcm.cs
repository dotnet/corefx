// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    public sealed partial class AesGcm : IDisposable
    {
        const int NonceSize = 12;
        public static KeySizes NonceByteSizes { get; } = new KeySizes(NonceSize, NonceSize, 1);
        public static KeySizes TagByteSizes { get; } = new KeySizes(12, 16, 1);

        public AesGcm(ReadOnlySpan<byte> key)
        {
            AesAEAD.CheckKeySize(key.Length * 8);
            ImportKey(key);
        }

        public AesGcm(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            AesAEAD.CheckKeySize(key.Length * 8);
            ImportKey(key);
        }

        public void Encrypt(byte[] nonce, byte[] plaintext, byte[] ciphertext, byte[] tag, byte[] associatedData = null)
        {
            AesAEAD.CheckArgumentsForNull(nonce, plaintext, ciphertext, tag);
            Encrypt((ReadOnlySpan<byte>)nonce, plaintext, ciphertext, tag, associatedData);
        }

        public void Encrypt(
            ReadOnlySpan<byte> nonce,
            ReadOnlySpan<byte> plaintext,
            Span<byte> ciphertext,
            Span<byte> tag,
            ReadOnlySpan<byte> associatedData = default)
        {
            CheckParameters(plaintext.Length, ciphertext.Length, nonce.Length, tag.Length);
            EncryptInternal(nonce, plaintext, ciphertext, tag, associatedData);
        }

        public void Decrypt(byte[] nonce, byte[] ciphertext, byte[] tag, byte[] plaintext, byte[] associatedData = null)
        {
            AesAEAD.CheckArgumentsForNull(nonce, plaintext, ciphertext, tag);
            Decrypt((ReadOnlySpan<byte>)nonce, ciphertext, tag, plaintext, associatedData);
        }

        public void Decrypt(
            ReadOnlySpan<byte> nonce,
            ReadOnlySpan<byte> ciphertext,
            ReadOnlySpan<byte> tag,
            Span<byte> plaintext,
            ReadOnlySpan<byte> associatedData = default)
        {
            CheckParameters(plaintext.Length, ciphertext.Length, nonce.Length, tag.Length);
            DecryptInternal(nonce, ciphertext, tag, plaintext, associatedData);
        }

        private static void CheckParameters(int plaintextSize, int ciphertextSize, int nonceSize, int tagSize)
        {
            if (plaintextSize != ciphertextSize)
                throw new ArgumentException(SR.Cryptography_PlaintextCiphertextLengthMismatch);

            if (!AesAEAD.MatchesKeySizes(nonceSize, NonceByteSizes))
                throw new ArgumentException(SR.Cryptography_InvalidNonceLength);

            if (!AesAEAD.MatchesKeySizes(tagSize, TagByteSizes))
                throw new ArgumentException(SR.Cryptography_InvalidTagLength);
        }
    }
}
