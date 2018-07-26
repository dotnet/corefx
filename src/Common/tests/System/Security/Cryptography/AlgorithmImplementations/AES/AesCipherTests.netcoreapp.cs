// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Encryption.Aes.Tests
{
    using Aes = System.Security.Cryptography.Aes;

    public partial class AesCipherTests
    {
        [Theory]
        [MemberData(nameof(GetNistGcmTestCases))]
        public static void AesGcmNistTests(AEADTest testCase)
        {
            using (AesGcm aesGcm = new AesGcm(testCase.Key))
            {
                byte[] ciphertext = new byte[testCase.Plaintext.Length];
                byte[] tag = new byte[testCase.Tag.Length];
                aesGcm.Encrypt(testCase.Nonce, testCase.Plaintext, ciphertext, tag, testCase.AssociatedData);
                Assert.Equal(testCase.Ciphertext, ciphertext);
                Assert.Equal(testCase.Tag, tag);

                byte[] plaintext = new byte[testCase.Plaintext.Length];
                aesGcm.Decrypt(testCase.Nonce, ciphertext, tag, plaintext, testCase.AssociatedData);
                Assert.Equal(testCase.Plaintext, plaintext);
            }
        }

        [Theory]
        [MemberData(nameof(GetNistGcmTestCases))]
        public static void AesGcmNistTestsTamperTag(AEADTest testCase)
        {
            Random r = new Random();
            using (AesGcm aesGcm = new AesGcm(testCase.Key))
            {
                byte[] ciphertext = new byte[testCase.Plaintext.Length];
                byte[] tag = new byte[testCase.Tag.Length];
                aesGcm.Encrypt(testCase.Nonce, testCase.Plaintext, ciphertext, tag, testCase.AssociatedData);
                Assert.Equal(testCase.Ciphertext, ciphertext);
                Assert.Equal(testCase.Tag, tag);

                tag[r.Next() % tag.Length] ^= 1;

                byte[] plaintext = new byte[testCase.Plaintext.Length];
                Assert.Throws<CryptographicException>(() => aesGcm.Decrypt(testCase.Nonce, ciphertext, tag, plaintext, testCase.AssociatedData));
            }
        }

        [Theory]
        [MemberData(nameof(GetNistCcmTestCases))]
        public static void AesCcmNistTests(AEADTest testCase)
        {
            using (AesCcm aesCcm = new AesCcm(testCase.Key))
            {
                byte[] ciphertext = new byte[testCase.Plaintext.Length];
                byte[] tag = new byte[testCase.Tag.Length];
                aesCcm.Encrypt(testCase.Nonce, testCase.Plaintext, ciphertext, tag, testCase.AssociatedData);
                Assert.Equal(testCase.Ciphertext, ciphertext);
                Assert.Equal(testCase.Tag, tag);

                byte[] plaintext = new byte[testCase.Plaintext.Length];
                aesCcm.Decrypt(testCase.Nonce, ciphertext, tag, plaintext, testCase.AssociatedData);
                Assert.Equal(testCase.Plaintext, plaintext);
            }
        }

        [Theory]
        [MemberData(nameof(GetNistCcmTestCases))]
        public static void AesCcmNistTestsTamperTag(AEADTest testCase)
        {
            Random r = new Random();
            using (AesCcm aesCcm = new AesCcm(testCase.Key))
            {
                byte[] ciphertext = new byte[testCase.Plaintext.Length];
                byte[] tag = new byte[testCase.Tag.Length];
                aesCcm.Encrypt(testCase.Nonce, testCase.Plaintext, ciphertext, tag, testCase.AssociatedData);
                Assert.Equal(testCase.Ciphertext, ciphertext);
                Assert.Equal(testCase.Tag, tag);

                tag[r.Next() % tag.Length] ^= 1;

                byte[] plaintext = new byte[testCase.Plaintext.Length];
                Assert.Throws<CryptographicException>(() => aesCcm.Decrypt(testCase.Nonce, ciphertext, tag, plaintext, testCase.AssociatedData));
            }
        }
    }
}
