// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests
{
    public static class PaddingModeTests
    {
        [Theory]
        [InlineData(PaddingMode.Zeros, 0, "", "")] // no block is added in this case!
        [InlineData(PaddingMode.Zeros, 1,    "46785bde46622b92ff7c8ebb91508a", "1be8aa365a15d11fc7826b3a10602d09" )]
        [InlineData(PaddingMode.Zeros, 13, "e505a2", "0a2e62938b03e5822ee251117a4ce066")]
        [InlineData(PaddingMode.PKCS7, 1,    "46785bde46622b92ff7c8ebb91508a", "db5b7829cce732bfe609140cf45a8843")]
        [InlineData(PaddingMode.PKCS7, 13, "e505a2", "46785bde46622b92ff7c8ebb91508a4d")]
        [InlineData(PaddingMode.PKCS7, 16, "", "d5450767bcc31793fe5065251b96b715")]
        [InlineData(PaddingMode.ANSIX923, 1, "46785bde46622b92ff7c8ebb91508a", "db5b7829cce732bfe609140cf45a8843" )]
        [InlineData(PaddingMode.ANSIX923, 13, "e505a2", "43b27d41a9fde73ca5db22c0fda76cb1")]
        [InlineData(PaddingMode.ANSIX923, 16, "", "a3d32a3a9dca71b6f961f5a8ed7e414f")]
        [InlineData(PaddingMode.ISO10126, 1, "46785bde46622b92ff7c8ebb91508a", null)]
        [InlineData(PaddingMode.ISO10126, 13, "e505a2", null)]
        [InlineData(PaddingMode.ISO10126, 16, "", null)]
        private static void ValidatePaddingMode(PaddingMode paddingMode, int expectedPaddingSize, string plainTextStr, string expectedCipherStr)
        {

            byte[] key = "1ed2f625c187b993256a8b3ccf9dcbfa5b44b4795c731012f70e4e64732efd5d".HexToByteArray();
            byte[] iv = "47d1e060ba3c8643f9f8b65feeda4b30".HexToByteArray();

            byte[] plainText = plainTextStr.HexToByteArray();
            byte[] expectedCipher = expectedCipherStr == null? Array.Empty<byte>() : expectedCipherStr.HexToByteArray();

            using (Aes a = Aes.Create())
            {
                a.Key = key;
                a.IV = iv;
                a.Mode = CipherMode.CBC;
                a.Padding = paddingMode;

                byte[] cipher = a.Encrypt(plainText);

                // we cannot validate the cipher in this padding mode as it consists of random data
                if (paddingMode != PaddingMode.ISO10126)
                {
                    Assert.Equal<byte>(expectedCipher, cipher);
                }

                // decrypt it with PaddingMode.None so that we can inspect the padding manually
                a.Padding = PaddingMode.None;
                byte[] decrypted = a.Decrypt(cipher);
                ValidatePadding(decrypted, paddingMode, expectedPaddingSize);
            }
        }

        private static void ValidatePadding(byte[] buffer, PaddingMode paddingMode, int expectedPaddingSize)
        {
            switch (paddingMode)
            {
                case PaddingMode.PKCS7:
                    ValidatePKCS7Padding(buffer, expectedPaddingSize);
                    break;
                case PaddingMode.ANSIX923:
                    ValidateANSIX923Padding(buffer, expectedPaddingSize);
                    break;
                case PaddingMode.ISO10126:
                    ValidateISO10126Padding(buffer, expectedPaddingSize);
                    break;
                case PaddingMode.Zeros:
                    ValidateZerosPadding(buffer, expectedPaddingSize);
                    break;
                case PaddingMode.None:
                    break;
                default:
                    break;
            }
        }

        private static void ValidateZerosPadding(byte[] buffer, int expectedPaddingSize)
        {
            for (int i = buffer.Length - 1; i > buffer.Length - 1 - expectedPaddingSize; i--)
            {
                Assert.Equal(0, buffer[i]);
            }
        }

        private static void ValidatePKCS7Padding(byte[] buffer, int expectedPaddingSize)
        {
            for (int i = buffer.Length - 1; i > buffer.Length - 1 - expectedPaddingSize; i--)
            {
                Assert.Equal(expectedPaddingSize, buffer[i]);
            }
        }

        private static void ValidateANSIX923Padding(byte[] buffer, int expectedPaddingSize)
        {
            Assert.Equal(buffer[buffer.Length - 1], expectedPaddingSize);

            for (int i = buffer.Length - expectedPaddingSize; i < buffer.Length - 1; i++)
            {
                Assert.Equal(0, buffer[i]);
            }
        }

        private static void ValidateISO10126Padding(byte[] buffer, int expectedPaddingSize)
        {
            // there is nothing else to validate as all the other padding bytes are random.
            Assert.Equal(buffer[buffer.Length - 1], expectedPaddingSize);
        }
    }
}