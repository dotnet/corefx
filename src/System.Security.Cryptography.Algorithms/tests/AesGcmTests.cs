// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Security.Cryptography.Algorithms.Tests
{
    public class AesGcmTests
    {
        [Fact]
        public static void EncryptDecryptSimpleNoAAD()
        {
            const int datalength = 32;
            byte[] originalData = Enumerable.Range(1, datalength).Select((x) => (byte)x).ToArray();
            byte[] nonce = Convert.FromBase64String("tBMp3WSvLDA2ZhtG");
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");

            byte[] expectedCiphertext = Convert.FromBase64String("8a8fstRIXMU21hhHXVL/s/BgmKVpJ3A59zss6r6LxZc=");
            byte[] expectedTag = Convert.FromBase64String("fGf9pRM+AzKbS0QR");

            byte[] ciphertext = new byte[expectedCiphertext.Length];
            byte[] tag = new byte[expectedTag.Length];

            using (AesGcm aesGcm = new AesGcm(key))
            {
                aesGcm.Encrypt(nonce, originalData, ciphertext, tag);
                Assert.Equal(expectedCiphertext, ciphertext);
                Assert.Equal(expectedTag, tag);

                byte[] plaintext = new byte[originalData.Length];
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
                Assert.Equal(originalData, plaintext);
            }
        }

        [Fact]
        public static void EncryptDecryptSimpleAAD()
        {
            const int datalength = 32;
            byte[] originalData = Enumerable.Range(1, datalength).Select((x) => (byte)x).ToArray();
            byte[] additionalData = Enumerable.Range(30, 65).Select((x) => (byte)x).ToArray();
            byte[] nonce = Convert.FromBase64String("tBMp3WSvLDA2ZhtG");
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");

            byte[] expectedCiphertext = Convert.FromBase64String("8a8fstRIXMU21hhHXVL/s/BgmKVpJ3A59zss6r6LxZc=");
            byte[] expectedTag = Convert.FromBase64String("T2ct1QTic0CLDazX");

            byte[] ciphertext = new byte[expectedCiphertext.Length];
            byte[] tag = new byte[expectedTag.Length];

            using (AesGcm aesGcm = new AesGcm(key))
            {
                aesGcm.Encrypt(nonce, originalData, ciphertext, tag, additionalData);
                Assert.Equal(expectedCiphertext, ciphertext);
                Assert.Equal(expectedTag, tag);

                byte[] plaintext = new byte[originalData.Length];
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintext, additionalData);
                Assert.Equal(originalData, plaintext);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(32)]
        [InlineData(41)]
        [InlineData(48)]
        [InlineData(50)]
        public static void EncryptDecryptRoundtripNoAAD(int dataLength)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] plaintext = Enumerable.Range(1, dataLength).Select((x) => (byte)x).ToArray();
                byte[] ciphertext = new byte[dataLength];
                byte[] key = new byte[16];
                byte[] nonce = new byte[AesGcm.NonceByteSizes.MinSize];
                byte[] tag = new byte[AesGcm.TagByteSizes.MinSize];
                rng.GetBytes(key);
                rng.GetBytes(nonce);

                using (AesGcm aesGcm = new AesGcm(key))
                {
                    aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

                    byte[] decrypted = new byte[dataLength];
                    aesGcm.Decrypt(nonce, ciphertext, tag, decrypted);
                    Assert.Equal(plaintext, decrypted);
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(32)]
        [InlineData(41)]
        [InlineData(48)]
        [InlineData(50)]
        public static void EncryptTamperTagDecryptNoAAD(int dataLength)
        {
            Random r = new Random();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] plaintext = Enumerable.Range(1, dataLength).Select((x) => (byte)x).ToArray();
                byte[] ciphertext = new byte[dataLength];
                byte[] key = new byte[16];
                byte[] nonce = new byte[AesGcm.NonceByteSizes.MinSize];
                byte[] tag = new byte[AesGcm.TagByteSizes.MinSize];
                rng.GetBytes(key);
                rng.GetBytes(nonce);

                using (AesGcm aesGcm = new AesGcm(key))
                {
                    aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

                    int tamperedIdx = r.Next() % tag.Length;
                    tag[tamperedIdx] = (byte)(tag[tamperedIdx] + 1);

                    byte[] decrypted = new byte[dataLength];
                    Assert.Throws<CryptographicException>(() => aesGcm.Decrypt(key, nonce, ciphertext, tag, decrypted));
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(32)]
        [InlineData(41)]
        [InlineData(48)]
        [InlineData(50)]
        public static void EncryptTamperCTDecrypt(int dataLength)
        {
            Random r = new Random();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] plaintext = Enumerable.Range(1, dataLength).Select((x) => (byte)x).ToArray();
                byte[] ciphertext = new byte[dataLength];
                byte[] key = new byte[16];
                byte[] nonce = new byte[AesGcm.NonceByteSizes.MinSize];
                byte[] tag = new byte[AesGcm.TagByteSizes.MinSize];
                rng.GetBytes(key);
                rng.GetBytes(nonce);

                using (AesGcm aesGcm = new AesGcm(key))
                {
                    aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

                    int tamperedIdx = r.Next() % ciphertext.Length;
                    ciphertext[tamperedIdx] = (byte)(ciphertext[tamperedIdx] + 1);

                    byte[] decrypted = new byte[dataLength];
                    Assert.Throws<CryptographicException>(() => aesGcm.Decrypt(key, nonce, ciphertext, tag, decrypted));
                }
            }
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 100)]
        [InlineData(7, 12)]
        [InlineData(16, 16)]
        [InlineData(17, 29)]
        [InlineData(32, 7)]
        [InlineData(41, 25)]
        [InlineData(48, 22)]
        [InlineData(50, 5)]
        public static void EncryptDecryptRoundtripAAD(int dataLength, int additionalDataLength)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] additionalData = new byte[additionalDataLength];
                rng.GetBytes(additionalData);

                byte[] plaintext = Enumerable.Range(1, dataLength).Select((x) => (byte)x).ToArray();
                byte[] ciphertext = new byte[dataLength];
                byte[] key = new byte[16];
                byte[] nonce = new byte[AesGcm.NonceByteSizes.MinSize];
                byte[] tag = new byte[AesGcm.TagByteSizes.MinSize];
                rng.GetBytes(key);
                rng.GetBytes(nonce);

                using (AesGcm aesGcm = new AesGcm(key))
                {
                    aesGcm.Encrypt(nonce, plaintext, ciphertext, tag, additionalData);

                    byte[] decrypted = new byte[dataLength];
                    aesGcm.Decrypt(nonce, ciphertext, tag, decrypted, additionalData);
                    Assert.Equal(plaintext, decrypted);
                }
            }
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 30)]
        [InlineData(1, 1)]
        [InlineData(1, 100)]
        [InlineData(7, 12)]
        [InlineData(16, 16)]
        [InlineData(17, 29)]
        [InlineData(32, 7)]
        [InlineData(41, 25)]
        [InlineData(48, 22)]
        [InlineData(50, 5)]
        public static void EncryptTamperAADDecrypt(int dataLength, int additionalDataLength)
        {
            Random r = new Random();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] additionalData = new byte[additionalDataLength];
                rng.GetBytes(additionalData);

                byte[] plaintext = Enumerable.Range(1, dataLength).Select((x) => (byte)x).ToArray();
                byte[] ciphertext = new byte[dataLength];
                byte[] key = new byte[16];
                byte[] nonce = new byte[AesGcm.NonceByteSizes.MinSize];
                byte[] tag = new byte[AesGcm.TagByteSizes.MinSize];
                rng.GetBytes(key);
                rng.GetBytes(nonce);

                using (AesGcm aesGcm = new AesGcm(key))
                {
                    aesGcm.Encrypt(nonce, plaintext, ciphertext, tag, additionalData);

                    int tamperedIdx = r.Next() % additionalData.Length;
                    additionalData[tamperedIdx] = (byte)(additionalData[tamperedIdx] + 1);

                    byte[] decrypted = new byte[dataLength];
                    Assert.Throws<CryptographicException>(() => aesGcm.Decrypt(nonce, ciphertext, tag, decrypted, additionalData));
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(17)]
        [InlineData(29)]
        [InlineData(33)]
        public static void InvalidKeyLength(int keyLength)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] key = new byte[keyLength];
                rng.GetBytes(key);

                Assert.Throws<CryptographicException>(() => new AesGcm(key));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        public static void InvalidNonceLength(int nonceLength)
        {
            int dataLength = 30;
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] plaintext = Enumerable.Range(1, dataLength).Select((x) => (byte)x).ToArray();
                byte[] ciphertext = new byte[dataLength];
                byte[] key = new byte[16];
                byte[] nonce = new byte[nonceLength];
                byte[] tag = new byte[AesGcm.TagByteSizes.MinSize];
                rng.GetBytes(key);
                rng.GetBytes(nonce);

                using (AesGcm aesGcm = new AesGcm(key))
                {
                    Assert.Throws<CryptographicException>(() => aesGcm.Encrypt(nonce, plaintext, ciphertext, tag));
                }
            }
        }

        [Fact]
        public static void TwoEncryptionsAndDecryptionsUsingOneInstance()
        {
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");
            byte[] originalData1 = Enumerable.Range(1, 15).Select((x) => (byte)x).ToArray();
            byte[] originalData2 = Enumerable.Range(14, 97).Select((x) => (byte)x).ToArray();
            byte[] associatedData2 = Enumerable.Range(100, 109).Select((x) => (byte)x).ToArray();
            byte[] nonce1 = Convert.FromBase64String("tBMp3WSvLDA2ZhtG");
            byte[] nonce2 = Convert.FromBase64String("i6EIkui4fQMRlr+Z");

            byte[] expectedCiphertext1 = Convert.FromBase64String("8a8fstRIXMU21hhHXVL/");
            byte[] expectedTag1 = Convert.FromBase64String("WrZWJMRrgWDzToH1");

            byte[] expectedCiphertext2 = Convert.FromBase64String(
                "IXvtAURtcxo3KiswrH/Nc67XyUbZFxrpwAscWJynO6IcG6x5I12awNDImRhN2Flrhm" +
                "/ZamwaKAg1V7Q6XLtTFQDoz7rYJHxtHetRp8Xf5FgBqNjVGbP6mC9UaqLQLbl42g==");
            byte[] expectedTag2 = Convert.FromBase64String("nHXQBmQP9PtoxgyVSKRc+A==");

            using (var aesGcm = new AesGcm(key))
            {
                byte[] ciphertext1 = new byte[originalData1.Length];
                byte[] tag1 = new byte[expectedTag1.Length];
                aesGcm.Encrypt(nonce1, (byte[])originalData1.Clone(), ciphertext1, tag1);
                Assert.Equal(expectedCiphertext1, ciphertext1);
                Assert.Equal(expectedTag1, tag1);

                byte[] ciphertext2 = new byte[originalData2.Length];
                byte[] tag2 = new byte[expectedTag2.Length];
                aesGcm.Encrypt(nonce2, (byte[])originalData2.Clone(), ciphertext2, tag2, associatedData2);
                Assert.Equal(expectedCiphertext2, ciphertext2);
                Assert.Equal(expectedTag2, tag2);

                byte[] plaintext1 = new byte[originalData1.Length];
                aesGcm.Decrypt(nonce1, ciphertext1, tag1, plaintext1);
                Assert.Equal(originalData1, plaintext1);

                byte[] plaintext2 = new byte[originalData2.Length];
                aesGcm.Decrypt(nonce2, ciphertext2, tag2, plaintext2, associatedData2);
                Assert.Equal(originalData2, plaintext2);
            }
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(3, 4)]
        [InlineData(4, 3)]
        [InlineData(20, 120)]
        [InlineData(120, 20)]
        public static void PlaintextAndCiphertextSizeDiffer(int ptLen, int ctLen)
        {
            byte[] key = new byte[16];
            byte[] nonce = new byte[12];
            byte[] plaintext = new byte[ptLen];
            byte[] ciphertext = new byte[ctLen];
            byte[] tag = new byte[16];

            using (var aesGcm = new AesGcm(key))
            {
                Assert.Throws<CryptographicException>(() => aesGcm.Encrypt(nonce, plaintext, ciphertext, tag));
                Assert.Throws<CryptographicException>(() => aesGcm.Decrypt(nonce, ciphertext, tag, plaintext));
            }
        }

        [Fact]
        public static void NullKey()
        {
            Assert.Throws<ArgumentNullException>(() => new AesGcm((byte[])null));
            Assert.Throws<ArgumentNullException>(() => new AesGcm((ReadOnlySpan<byte>)null));
        }

        [Fact]
        public static void EncryptDecryptNullNonce()
        {
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");
            byte[] plaintext = new byte[0];
            byte[] ciphertext = new byte[0];
            byte[] tag = new byte[16];

            using (var aesGcm = new AesGcm(key))
            {
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt((byte[])null, plaintext, ciphertext, tag));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt((ReadOnlySpan<byte>)null, plaintext, ciphertext, tag));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt((byte[])null, ciphertext, tag, plaintext));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt((ReadOnlySpan<byte>)null, ciphertext, tag, plaintext));
            }
        }

        [Fact]
        public static void EncryptDecryptNullPlaintext()
        {
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");
            byte[] nonce = new byte[12];
            byte[] ciphertext = new byte[0];
            byte[] tag = new byte[16];

            using (var aesGcm = new AesGcm(key))
            {
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt(nonce, (byte[])null, ciphertext, tag));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt(nonce, (ReadOnlySpan<byte>)null, ciphertext, tag));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt(nonce, ciphertext, tag, (byte[])null));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt(nonce, ciphertext, tag, (Span<byte>)null));
            }
        }

        [Fact]
        public static void EncryptDecryptNullCiphertext()
        {
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");
            byte[] nonce = new byte[12];
            byte[] plaintext = new byte[0];
            byte[] tag = new byte[16];

            using (var aesGcm = new AesGcm(key))
            {
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt(nonce, plaintext, (byte[])null, tag));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt(nonce, plaintext, (Span<byte>)null, tag));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt(nonce, (byte[])null, tag, plaintext));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt(nonce, (ReadOnlySpan<byte>)null, tag, plaintext));
            }
        }

        [Fact]
        public static void EncryptDecryptNullTag()
        {
            byte[] key = Convert.FromBase64String("1aGU7ZDP4Iq+zUaRmXzrLA==");
            byte[] nonce = new byte[12];
            byte[] plaintext = new byte[0];
            byte[] ciphertext = new byte[0];

            using (var aesGcm = new AesGcm(key))
            {
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt(nonce, plaintext, ciphertext, (byte[])null));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Encrypt(nonce, plaintext, ciphertext, (Span<byte>)null));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt(nonce, ciphertext, (byte[])null, plaintext));
                Assert.Throws<ArgumentNullException>(() => aesGcm.Decrypt(nonce, ciphertext, (ReadOnlySpan<byte>)null, plaintext));
            }
        }
    }
}
