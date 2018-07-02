// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    public partial class AesCcm : IDisposable
    {
        public static KeySizes NonceByteSizes { get; } = new KeySizes(12, 12, 1);

        public AesCcm(byte[] key) : this((ReadOnlySpan<byte>)key)
        {
        }

        public void Encrypt(byte[] nonce, byte[] plaintext, byte[] ciphertext, byte[] tag, byte[] associatedData = null)
        {
            Encrypt((ReadOnlySpan<byte>)nonce, plaintext, ciphertext, tag, associatedData);
        }

        public void Decrypt(byte[] nonce, byte[] ciphertext, byte[] tag, byte[] plaintext, byte[] associatedData = default)
        {
            Decrypt((ReadOnlySpan<byte>)nonce, ciphertext, tag, plaintext, associatedData);
        }

        private static void CheckParameters(int nonceLengthInBytes, int tagSizeInBytes)
        {
            if (!AesAEAD.MatchesKeySizes(nonceLengthInBytes, NonceByteSizes))
            {
                throw new CryptographicException(SR.Cryptography_InvalidNonceLength);
            }

            if (!AesAEAD.MatchesKeySizes(tagSizeInBytes, TagByteSizes))
            {
                throw new CryptographicException(SR.Cryptography_InvalidTagLength);
            }
        }
    }
}
