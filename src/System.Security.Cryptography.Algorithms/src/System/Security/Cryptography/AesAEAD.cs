// Licensed to the .NET Foundation under one or more agreements.
// ThF:\src\corefx\src\System.Security.Cryptography.Algorithms\src\System\Security\Cryptography\AesAEAD.Windows.cse .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    internal partial class AesAEAD
    {
        public static void CheckKeySize(int keySizeInBits)
        {
            if (keySizeInBits != 128 && keySizeInBits != 192 && keySizeInBits != 256)
            {
                throw new CryptographicException(SR.Cryptography_InvalidKeySize);
            }
        }

        public static bool MatchesKeySizes(int size, KeySizes keySizes)
        {
            if (size < keySizes.MinSize || size > keySizes.MaxSize)
                return false;

            return ((size - keySizes.MinSize) % keySizes.SkipSize) == 0;
        }

        public static void CheckArguments(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag)
        {
            if (nonce == null)
                throw new ArgumentNullException(nameof(nonce));

            if (plaintext == null)
                throw new ArgumentNullException(nameof(plaintext));

            if (ciphertext == null)
                throw new ArgumentNullException(nameof(ciphertext));

            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (plaintext.Length != ciphertext.Length)
                throw new CryptographicException(SR.Cryptography_PlaintextCiphertextLengthMismatch);
        }
    }
}
