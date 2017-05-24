// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Encryption.TripleDes.Tests
{
    public static class TripleDESCipherTests
    {
        [Fact]
        public static void TripleDESDefaults()
        {
            using (TripleDES des = TripleDESFactory.Create())
            {
                Assert.Equal(192, des.KeySize);
                Assert.Equal(64, des.BlockSize);
            }
        }

        [Fact]
        public static void TripleDESGenerate128Key()
        {
            using (TripleDES des = TripleDESFactory.Create())
            {
                des.KeySize = 128;
                byte[] key = des.Key;
                Assert.Equal(128, key.Length * 8);
            }
        }

        [Fact]
        public static void TripleDESInvalidKeySizes()
        {
            using (TripleDES des = TripleDESFactory.Create())
            {
                Assert.Throws<CryptographicException>(() => des.KeySize = 128 - 1);
                Assert.Throws<CryptographicException>(() => des.KeySize = 192 + 1);
            }
        }

        [Theory]
        [InlineData(192, "c5629363d957054eba793093b83739bb78711db221a82379", "e56f72478c7479d169d54c0548b744af5b53efb1cdd26037")]
        [InlineData(128, "c5629363d957054eba793093b83739bb",                 "1387b981dbb40f34b915c4ed89fd681a740d3b4869c0b575")]
        [InlineData(192, "c5629363d957054eba793093b83739bbc5629363d957054e", "1387b981dbb40f34b915c4ed89fd681a740d3b4869c0b575")]
        public static void TripleDESRoundTripNoneECB(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.Padding = PaddingMode.None;
                alg.Mode = CipherMode.ECB;

                byte[] plainText = "de7d2dddea96b691e979e647dc9d3ca27d7f1ad673ca9570".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);
                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "de7d2dddea96b691e979e647dc9d3ca27d7f1ad673ca9570".HexToByteArray();
                Assert.Equal<byte>(expectedDecrypted, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "b43eaf0260813fb47c87ae073a146006d359ad04061eb0e6", "dea36279600f19c602b6ed9bf3ffdac5ebf25c1c470eb61c")]
        [InlineData(128, "b43eaf0260813fb47c87ae073a146006",                 "a25e55381f0cc45541741b9ce6e96b7799aa1e0db70780f7")]
        [InlineData(192, "b43eaf0260813fb47c87ae073a146006b43eaf0260813fb4", "a25e55381f0cc45541741b9ce6e96b7799aa1e0db70780f7")]
        public static void TripleDESRoundTripNoneCBC(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();
            byte[] iv = "5fbc5bc21b8597d8".HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.IV = iv;
                alg.Padding = PaddingMode.None;
                alg.Mode = CipherMode.CBC;

                byte[] plainText = "79a86903608e133e020e1dc68c9835250c2f17b0ebeed91b".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);
                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "79a86903608e133e020e1dc68c9835250c2f17b0ebeed91b".HexToByteArray();
                Assert.Equal<byte>(expectedDecrypted, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "9da5b265179d65f634dfc95513f25094411e51bb3be877ef", "149ec32f558b27c7e4151e340d8184f18b4e25d2518f69d9")]
        [InlineData(128, "9da5b265179d65f634dfc95513f25094",                 "02ac5db31cfada874f6042c4e92b09175fd08e93a20f936b")]
        [InlineData(192, "9da5b265179d65f634dfc95513f250949da5b265179d65f6", "02ac5db31cfada874f6042c4e92b09175fd08e93a20f936b")]
        public static void TripleDESRoundTripZerosECB(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.Padding = PaddingMode.Zeros;
                alg.Mode = CipherMode.ECB;

                byte[] plainText = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);
                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8000000".HexToByteArray();
                Assert.Equal<byte>(expectedDecrypted, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "9da5b265179d65f634dfc95513f25094411e51bb3be877ef")]
        [InlineData(128, "9da5b265179d65f634dfc95513f25094")]
        [InlineData(192, "9da5b265179d65f634dfc95513f250949da5b265179d65f6")]
        public static void TripleDESRoundTripISO10126ECB(int keySize, string keyHex)
        {
            byte[] key = keyHex.HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.Padding = PaddingMode.ISO10126;
                alg.Mode = CipherMode.ECB;

                byte[] plainText = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);

                // the padding data for ISO10126 is made up of random bytes, so we cannot actually test
                // the full encrypted text. We need to strip the padding and then compare
                byte[] decrypted = alg.Decrypt(cipher);

                Assert.Equal<byte>(plainText, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "9da5b265179d65f634dfc95513f25094411e51bb3be877ef", "149ec32f558b27c7e4151e340d8184f1c90f0a499e20fda9")]
        [InlineData(128, "9da5b265179d65f634dfc95513f25094",                 "02ac5db31cfada874f6042c4e92b091783620e54a1e75957")]
        [InlineData(192, "9da5b265179d65f634dfc95513f250949da5b265179d65f6", "02ac5db31cfada874f6042c4e92b091783620e54a1e75957")]
        public static void TripleDESRoundTripANSIX923ECB(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.Padding = PaddingMode.ANSIX923;
                alg.Mode = CipherMode.ECB;

                byte[] plainText = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);

                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8".HexToByteArray();
                Assert.Equal<byte>(plainText, decrypted);
            }
        }

        [Fact]
        public static void TripleDES_FailureToRoundTrip192Bits_DifferentPadding_ANSIX923_ZerosECB()
        {
            byte[] key = "9da5b265179d65f634dfc95513f25094411e51bb3be877ef".HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                alg.Padding = PaddingMode.ANSIX923;
                alg.Mode = CipherMode.ECB;

                byte[] plainText = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);

                byte[] expectedCipher = "149ec32f558b27c7e4151e340d8184f1c90f0a499e20fda9".HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                alg.Padding = PaddingMode.Zeros;
                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "77a8b2efb45addb38d7ef3aa9e6ab5d71957445ab8".HexToByteArray();

                // They should not decrypt to the same value
                Assert.NotEqual<byte>(plainText, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "5e970c0d2323d53b28fa3de507d6d20f9f0cd97123398b4d", "65f3dc211876a9daad238aa7d0c7ed7a3662296faf77dff9")]
        [InlineData(128, "5e970c0d2323d53b28fa3de507d6d20f",                 "2f55ff6bd8270f1d68dcb342bb674f914d9e1c0e61017a77")]
        [InlineData(192, "5e970c0d2323d53b28fa3de507d6d20f5e970c0d2323d53b", "2f55ff6bd8270f1d68dcb342bb674f914d9e1c0e61017a77")]
        public static void TripleDESRoundTripZerosCBC(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();
            byte[] iv = "95498b5bf570f4c8".HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.IV = iv;
                alg.Padding = PaddingMode.Zeros;
                alg.Mode = CipherMode.CBC;

                byte[] plainText = "f9e9a1385bf3bd056d6a06eac662736891bd3e6837".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);
                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "f9e9a1385bf3bd056d6a06eac662736891bd3e6837000000".HexToByteArray();
                Assert.Equal<byte>(expectedDecrypted, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "155425f12109cd89378795a4ca337b3264689dca497ba2fa", "7b8d982ee0c14821daf1b8cf4e407c2eb328627b696ac36e")]
        [InlineData(128, "155425f12109cd89378795a4ca337b32",                 "ce7daa4723c4f880fb44c2809821fc2183b46f0c32084620")]
        [InlineData(192, "155425f12109cd89378795a4ca337b32155425f12109cd89", "ce7daa4723c4f880fb44c2809821fc2183b46f0c32084620")]
        public static void TripleDESRoundTripPKCS7ECB(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.Padding = PaddingMode.PKCS7;
                alg.Mode = CipherMode.ECB;

                byte[] plainText = "5bd3c4e16a723a17ac60dd0efdb158e269cddfd0fa".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);
                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "5bd3c4e16a723a17ac60dd0efdb158e269cddfd0fa".HexToByteArray();
                Assert.Equal<byte>(expectedDecrypted, decrypted);
            }
        }

        [Theory]
        [InlineData(192, "6b42da08f93e819fbd26fce0785b0eec3d0cb6bfa053c505", "446f57875e107702afde16b57eaf250b87b8110bef29af89")]
        [InlineData(128, "6b42da08f93e819fbd26fce0785b0eec",                 "ebf995606ceceddf5c90a7302521bc1f6d31f330969cb768")]
        [InlineData(192, "6b42da08f93e819fbd26fce0785b0eec6b42da08f93e819f", "ebf995606ceceddf5c90a7302521bc1f6d31f330969cb768")]
        public static void TripleDESRoundTripPKCS7CBC(int keySize, string keyHex, string expectedCipherHex)
        {
            byte[] key = keyHex.HexToByteArray();
            byte[] iv = "8fc67ce5e7f28cde".HexToByteArray();

            using (TripleDES alg = TripleDESFactory.Create())
            {
                alg.Key = key;
                Assert.Equal(keySize, alg.KeySize);

                alg.IV = iv;
                alg.Padding = PaddingMode.PKCS7;
                alg.Mode = CipherMode.CBC;

                byte[] plainText = "e867f915e275eab27d6951165d26dec6dd0acafcfc".HexToByteArray();
                byte[] cipher = alg.Encrypt(plainText);
                byte[] expectedCipher = expectedCipherHex.HexToByteArray();
                Assert.Equal<byte>(expectedCipher, cipher);

                byte[] decrypted = alg.Decrypt(cipher);
                byte[] expectedDecrypted = "e867f915e275eab27d6951165d26dec6dd0acafcfc".HexToByteArray();
                Assert.Equal<byte>(expectedDecrypted, decrypted);
            }
        }
    }
}
