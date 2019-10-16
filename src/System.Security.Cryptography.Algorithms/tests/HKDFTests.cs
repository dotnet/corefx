// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.XUnitExtensions;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Algorithms.Tests
{
    public class HKDFTests
    {
        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExtractByteArrayTests(Rfc5869TestCase test)
        {
            byte[] prk = HKDF.Extract(test.Hash, test.Ikm, test.Salt);
            Assert.Equal(test.Prk, prk);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExtractByteArrayTamperHashTests(Rfc5869TestCase test)
        {
            byte[] prk = HKDF.Extract(HashAlgorithmName.MD5, test.Ikm, test.Salt);
            Assert.NotEqual(test.Prk, prk);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExtractByteArrayTamperIkmTests(Rfc5869TestCase test)
        {
            test.Ikm[0] ^= 1;
            byte[] prk = HKDF.Extract(test.Hash, test.Ikm, test.Salt);
            Assert.NotEqual(test.Prk, prk);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCasesWithNonEmptySalt))]
        public void Rfc5869ExtractByteArrayTamperSaltTests(Rfc5869TestCase test)
        {
            test.Salt[0] ^= 1;
            byte[] prk = HKDF.Extract(test.Hash, test.Ikm, test.Salt);
            Assert.NotEqual(test.Prk, prk);
        }

        [Fact]
        public void Rfc5869ExtractByteArrayDefaultHash()
        {
            byte[] ikm = new byte[20];
            byte[] salt = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Extract(default(HashAlgorithmName), ikm, salt));
        }

        [Fact]
        public void Rfc5869ExtractByteArrayNonsensicalHash()
        {
            byte[] ikm = new byte[20];
            byte[] salt = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Extract(new HashAlgorithmName("foo"), ikm, salt));
        }

        [Fact]
        public void Rfc5869ExtractByteArrayNullIkm()
        {
            byte[] salt = new byte[20];
            Assert.Throws<ArgumentNullException>(() => HKDF.Extract(HashAlgorithmName.SHA1, null, salt));
        }

        [Fact]
        public void Rfc5869ExtractByteArrayEmptyIkm()
        {
            byte[] salt = new byte[20];
            byte[] ikm = Array.Empty<byte>();

            // Ensure does not throw
            byte[] prk = HKDF.Extract(HashAlgorithmName.SHA1, ikm, salt);
            Assert.Equal("FBDB1D1B18AA6C08324B7D64B71FB76370690E1D".HexToByteArray(), prk);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExtractSpanTests(Rfc5869TestCase test)
        {
            byte[] prk = new byte[test.Prk.Length];
            Assert.Equal(test.Prk.Length, HKDF.Extract(test.Hash, test.Ikm, test.Salt, prk));
            Assert.Equal(test.Prk, prk);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExtractSpanTamperIkmTests(Rfc5869TestCase test)
        {
            test.Ikm[0] ^= 1;
            byte[] prk = new byte[test.Prk.Length];
            HKDF.Extract(test.Hash, test.Ikm, test.Salt, prk);
            Assert.NotEqual(test.Prk, prk);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCasesWithNonEmptySalt))]
        public void Rfc5869ExtractSpanTamperSaltTests(Rfc5869TestCase test)
        {
            test.Salt[0] ^= 1;
            byte[] prk = new byte[test.Prk.Length];
            HKDF.Extract(test.Hash, test.Ikm, test.Salt, prk);
            Assert.NotEqual(test.Prk, prk);
        }

        [Fact]
        public void Rfc5869ExtractSpanDefaultHash()
        {
            byte[] prk = new byte[20];
            byte[] ikm = new byte[20];
            byte[] salt = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Extract(default(HashAlgorithmName), ikm, salt, prk));
        }

        [Fact]
        public void Rfc5869ExtractSpanNonsensicalHash()
        {
            byte[] prk = new byte[20];
            byte[] ikm = new byte[20];
            byte[] salt = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Extract(new HashAlgorithmName("foo"), ikm, salt, prk));
        }

        [Fact]
        public void Rfc5869ExtractSpanEmptyIkm()
        {
            byte[] prk = new byte[20];
            byte[] ikm = Array.Empty<byte>();
            byte[] salt = new byte[20];
            Assert.Equal(20, HKDF.Extract(HashAlgorithmName.SHA1, ikm, salt, prk));
            Assert.Equal("FBDB1D1B18AA6C08324B7D64B71FB76370690E1D".HexToByteArray(), prk);
        }

        [Fact]
        public void Rfc5869ExtractSpanEmptySalt()
        {
            byte[] prk = new byte[20];
            byte[] ikm = new byte[20];
            byte[] salt = Array.Empty<byte>();
            Assert.Equal(20, HKDF.Extract(HashAlgorithmName.SHA1, ikm, salt, prk));
            Assert.Equal("A3CBF4A40F51A53E046F07397E52DF9286AE93A2".HexToByteArray(), prk);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(19)]
        public void Rfc5869ExtractSpanPrkTooShort(int prkSize)
        {
            byte[] prk = new byte[prkSize];
            byte[] ikm = new byte[20];
            byte[] salt = new byte[20];
            Assert.Throws<ArgumentException>(() => HKDF.Extract(HashAlgorithmName.SHA1, ikm, salt, prk));
        }

        [Fact]
        public void Rfc5869ExtractSpanPrkTooLong()
        {
            byte[] prk = new byte[24];

            for (int i = 0; i < 4; i++)
            {
                prk[20 + i] = (byte)(i + 5);
            }

            byte[] ikm = new byte[20];
            byte[] salt = new byte[20];
            Assert.Equal(20, HKDF.Extract(HashAlgorithmName.SHA1, ikm, salt, prk));
            Assert.Equal("A3CBF4A40F51A53E046F07397E52DF9286AE93A2".HexToByteArray(), prk.Take(20).ToArray());

            for (int i = 0; i < 4; i++)
            {
                // ensure we didn't modify anything further
                Assert.Equal((byte)(i + 5), prk[20 + i]);
            }
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExpandByteArrayTests(Rfc5869TestCase test)
        {
            byte[] okm = HKDF.Expand(test.Hash, test.Prk, test.Okm.Length, test.Info);
            Assert.Equal(test.Okm, okm);
        }

        [Fact]
        public void Rfc5869ExpandByteArrayDefaultHash()
        {
            byte[] prk = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Expand(default(HashAlgorithmName), prk, 20, null));
        }

        [Fact]
        public void Rfc5869ExpandByteArrayNonsensicalHash()
        {
            byte[] prk = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Expand(new HashAlgorithmName("foo"), prk, 20, null));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExpandByteArrayTamperPrkTests(Rfc5869TestCase test)
        {
            test.Prk[0] ^= 1;
            byte[] okm = HKDF.Expand(test.Hash, test.Prk, test.Okm.Length, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(19)]
        public void Rfc5869ExpandByteArrayPrkTooSmall(int prkSize)
        {
            byte[] prk = new byte[prkSize];
            Assert.Throws<ArgumentException>(() => HKDF.Expand(HashAlgorithmName.SHA1, prk, 17, Array.Empty<byte>()));
        }

        [Fact]
        public void Rfc5869ExpandByteArrayOkmMaxSize()
        {
            byte[] prk = new byte[20];

            // Does not throw
            byte[] okm = HKDF.Expand(HashAlgorithmName.SHA1, prk, 20 * 255, Array.Empty<byte>());
            Assert.Equal(20 * 255, okm.Length);
        }

        [Fact]
        public void Rfc5869ExpandByteArrayOkmMaxSizePlusOne()
        {
            byte[] prk = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Expand(HashAlgorithmName.SHA1, prk, 20 * 255 + 1, Array.Empty<byte>()));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExpandSpanTests(Rfc5869TestCase test)
        {
            byte[] okm = new byte[test.Okm.Length];
            HKDF.Expand(test.Hash, test.Prk, okm, test.Info);
            Assert.Equal(test.Okm, okm);
        }

        [Fact]
        public void Rfc5869ExpandSpanDefaultHash()
        {
            byte[] prk = new byte[20];
            byte[] okm = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Expand(default(HashAlgorithmName), prk, okm, null));
        }

        [Fact]
        public void Rfc5869ExpandSpanNonsensicalHash()
        {
            byte[] prk = new byte[20];
            byte[] okm = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.Expand(new HashAlgorithmName("foo"), prk, okm, null));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869ExpandSpanTamperPrkTests(Rfc5869TestCase test)
        {
            test.Prk[0] ^= 1;
            byte[] okm = new byte[test.Okm.Length];
            HKDF.Expand(test.Hash, test.Prk, okm, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(19)]
        public void Rfc5869ExpandSpanPrkTooSmall(int prkSize)
        {
            byte[] prk = new byte[prkSize];
            byte[] okm = new byte[17];
            Assert.Throws<ArgumentException>(() => HKDF.Expand(HashAlgorithmName.SHA1, prk, okm, Array.Empty<byte>()));
        }

        [Fact]
        public void Rfc5869ExpandSpanOkmMaxSize()
        {
            byte[] prk = new byte[20];
            byte[] okm = new byte[20 * 255];

            // Does not throw
            HKDF.Expand(HashAlgorithmName.SHA1, prk, okm, Array.Empty<byte>());
        }

        [Fact]
        public void Rfc5869ExpandSpanOkmMaxSizePlusOne()
        {
            byte[] prk = new byte[20];
            byte[] okm = new byte[20 * 255 + 1];
            // Note: We expect ArgumentOutOfRangeException for byte array version since it takes a size
            Assert.Throws<ArgumentException>(() => HKDF.Expand(HashAlgorithmName.SHA1, prk, okm, Array.Empty<byte>()));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869DeriveKeyByteArrayTests(Rfc5869TestCase test)
        {
            byte[] okm = HKDF.DeriveKey(test.Hash, test.Ikm, test.Okm.Length, test.Salt, test.Info);
            Assert.Equal(test.Okm, okm);
        }

        [Fact]
        public void Rfc5869DeriveKeyByteArrayDefaultHash()
        {
            byte[] ikm = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.DeriveKey(default(HashAlgorithmName), ikm, 20, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Fact]
        public void Rfc5869DeriveKeyByteArrayNonSensicalHash()
        {
            byte[] ikm = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.DeriveKey(new HashAlgorithmName("foo"), ikm, 20, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Fact]
        public void Rfc5869DeriveKeyByteArrayNullIkm()
        {
            Assert.Throws<ArgumentNullException>(() => HKDF.DeriveKey(HashAlgorithmName.SHA1, null, 20, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869DeriveKeyByteArrayTamperIkmTests(Rfc5869TestCase test)
        {
            test.Ikm[0] ^= 1;
            byte[] okm = HKDF.DeriveKey(test.Hash, test.Ikm, test.Okm.Length, test.Salt, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Fact]
        public void Rfc5869DeriveKeyByteArrayOkmMaxSizePlusOne()
        {
            byte[] ikm = new byte[20];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.DeriveKey(HashAlgorithmName.SHA1, ikm, 20 * 255 + 1, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCasesWithNonEmptySalt))]
        public void Rfc5869DeriveKeyByteArrayTamperSaltTests(Rfc5869TestCase test)
        {
            test.Salt[0] ^= 1;
            byte[] okm = HKDF.DeriveKey(test.Hash, test.Ikm, test.Okm.Length, test.Salt, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCasesWithNonEmptyInfo))]
        public void Rfc5869DeriveKeyByteArrayTamperInfoTests(Rfc5869TestCase test)
        {
            test.Info[0] ^= 1;
            byte[] okm = HKDF.DeriveKey(test.Hash, test.Ikm, test.Okm.Length, test.Salt, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869DeriveKeySpanTests(Rfc5869TestCase test)
        {
            byte[] okm = new byte[test.Okm.Length];
            HKDF.DeriveKey(test.Hash, test.Ikm, okm, test.Salt, test.Info);
            Assert.Equal(test.Okm, okm);
        }

        [Fact]
        public void Rfc5869DeriveKeySpanDefaultHash()
        {
            byte[] ikm = new byte[20];
            byte[] okm = new byte[17];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.DeriveKey(default(HashAlgorithmName), ikm, okm, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Fact]
        public void Rfc5869DeriveKeySpanNonSensicalHash()
        {
            byte[] ikm = new byte[20];
            byte[] okm = new byte[17];
            Assert.Throws<ArgumentOutOfRangeException>(() => HKDF.DeriveKey(new HashAlgorithmName("foo"), ikm, okm, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCases))]
        public void Rfc5869DeriveKeySpanTamperIkmTests(Rfc5869TestCase test)
        {
            test.Ikm[0] ^= 1;
            byte[] okm = new byte[test.Okm.Length];
            HKDF.DeriveKey(test.Hash, test.Ikm, okm, test.Salt, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Fact]
        public void Rfc5869DeriveKeySpanOkmMaxSizePlusOne()
        {
            byte[] ikm = new byte[20];
            byte[] okm = new byte[20 * 255 + 1];
            Assert.Throws<ArgumentException>(() => HKDF.DeriveKey(HashAlgorithmName.SHA1, ikm, okm, Array.Empty<byte>(), Array.Empty<byte>()));
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCasesWithNonEmptySalt))]
        public void Rfc5869DeriveKeySpanTamperSaltTests(Rfc5869TestCase test)
        {
            test.Salt[0] ^= 1;
            byte[] okm = new byte[test.Okm.Length];
            HKDF.DeriveKey(test.Hash, test.Ikm, okm, test.Salt, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        [Theory]
        [MemberData(nameof(GetRfc5869TestCasesWithNonEmptyInfo))]
        public void Rfc5869DeriveKeySpanTamperInfoTests(Rfc5869TestCase test)
        {
            test.Info[0] ^= 1;
            byte[] okm = new byte[test.Okm.Length];
            HKDF.DeriveKey(test.Hash, test.Ikm, okm, test.Salt, test.Info);
            Assert.NotEqual(test.Okm, okm);
        }

        public static IEnumerable<object[]> GetRfc5869TestCases()
        {
            foreach (Rfc5869TestCase test in Rfc5869TestCases)
            {
                yield return new object[] { test };
            }
        }

        public static IEnumerable<object[]> GetRfc5869TestCasesWithNonEmptySalt()
        {
            foreach (Rfc5869TestCase test in Rfc5869TestCases)
            {
                if (test.Salt != null && test.Salt.Length != 0)
                {
                    yield return new object[] { test };
                }
            }
        }

        public static IEnumerable<object[]> GetRfc5869TestCasesWithNonEmptyInfo()
        {
            foreach (Rfc5869TestCase test in Rfc5869TestCases)
            {
                if (test.Info != null && test.Info.Length != 0)
                {
                    yield return new object[] { test };
                }
            }
        }

        private static Rfc5869TestCase[] Rfc5869TestCases => new Rfc5869TestCase[7]
        {
            new Rfc5869TestCase()
            {
                Name = "Basic test case with SHA-256",
                Hash = HashAlgorithmName.SHA256,
                Ikm = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b".HexToByteArray(),
                Salt = "000102030405060708090a0b0c".HexToByteArray(),
                Info = "f0f1f2f3f4f5f6f7f8f9".HexToByteArray(),
                Prk = (
                    "077709362c2e32df0ddc3f0dc47bba63" +
                    "90b6c73bb50f9c3122ec844ad7c2b3e5").HexToByteArray(),
                Okm = (
                    "3cb25f25faacd57a90434f64d0362f2a" +
                    "2d2d0a90cf1a5a4c5db02d56ecc4c5bf" +
                    "34007208d5b887185865").HexToByteArray(),
            },
            new Rfc5869TestCase()
            {
                Name = "Test with SHA-256 and longer inputs/outputs",
                Hash = HashAlgorithmName.SHA256,
                Ikm = (
                    "000102030405060708090a0b0c0d0e0f" +
                    "101112131415161718191a1b1c1d1e1f" +
                    "202122232425262728292a2b2c2d2e2f" +
                    "303132333435363738393a3b3c3d3e3f" +
                    "404142434445464748494a4b4c4d4e4f").HexToByteArray(),
                Salt = (
                    "606162636465666768696a6b6c6d6e6f" +
                    "707172737475767778797a7b7c7d7e7f" +
                    "808182838485868788898a8b8c8d8e8f" +
                    "909192939495969798999a9b9c9d9e9f" +
                    "a0a1a2a3a4a5a6a7a8a9aaabacadaeaf").HexToByteArray(),
                Info = (
                    "b0b1b2b3b4b5b6b7b8b9babbbcbdbebf" +
                    "c0c1c2c3c4c5c6c7c8c9cacbcccdcecf" +
                    "d0d1d2d3d4d5d6d7d8d9dadbdcdddedf" +
                    "e0e1e2e3e4e5e6e7e8e9eaebecedeeef" +
                    "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff").HexToByteArray(),
                Prk = (
                    "06a6b88c5853361a06104c9ceb35b45c" +
                    "ef760014904671014a193f40c15fc244").HexToByteArray(),
                Okm = (
                    "b11e398dc80327a1c8e7f78c596a4934" +
                    "4f012eda2d4efad8a050cc4c19afa97c" +
                    "59045a99cac7827271cb41c65e590e09" +
                    "da3275600c2f09b8367793a9aca3db71" +
                    "cc30c58179ec3e87c14c01d5c1f3434f" +
                    "1d87").HexToByteArray(),
            },
            new Rfc5869TestCase()
            {
                Name = "Test with SHA-256 and zero-length salt/info",
                Hash = HashAlgorithmName.SHA256,
                Ikm = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b".HexToByteArray(),
                Salt = Array.Empty<byte>(),
                Info = Array.Empty<byte>(),
                Prk = (
                    "19ef24a32c717b167f33a91d6f648bdf" +
                    "96596776afdb6377ac434c1c293ccb04").HexToByteArray(),
                Okm = (
                    "8da4e775a563c18f715f802a063c5a31" +
                    "b8a11f5c5ee1879ec3454e5f3c738d2d" +
                    "9d201395faa4b61a96c8").HexToByteArray(),
            },
            new Rfc5869TestCase()
            {
                Name = "Basic test case with SHA-1",
                Hash = HashAlgorithmName.SHA1,
                Ikm = "0b0b0b0b0b0b0b0b0b0b0b".HexToByteArray(),
                Salt = "000102030405060708090a0b0c".HexToByteArray(),
                Info = "f0f1f2f3f4f5f6f7f8f9".HexToByteArray(),
                Prk = "9b6c18c432a7bf8f0e71c8eb88f4b30baa2ba243".HexToByteArray(),
                Okm = (
                    "085a01ea1b10f36933068b56efa5ad81" +
                    "a4f14b822f5b091568a9cdd4f155fda2" +
                    "c22e422478d305f3f896").HexToByteArray(),
            },
            new Rfc5869TestCase()
            {
                Name = "Test with SHA-1 and longer inputs/outputs",
                Hash = HashAlgorithmName.SHA1,
                Ikm = (
                    "000102030405060708090a0b0c0d0e0f" +
                    "101112131415161718191a1b1c1d1e1f" +
                    "202122232425262728292a2b2c2d2e2f" +
                    "303132333435363738393a3b3c3d3e3f" +
                    "404142434445464748494a4b4c4d4e4f").HexToByteArray(),
                Salt = (
                    "606162636465666768696a6b6c6d6e6f" +
                    "707172737475767778797a7b7c7d7e7f" +
                    "808182838485868788898a8b8c8d8e8f" +
                    "909192939495969798999a9b9c9d9e9f" +
                    "a0a1a2a3a4a5a6a7a8a9aaabacadaeaf").HexToByteArray(),
                Info = (
                    "b0b1b2b3b4b5b6b7b8b9babbbcbdbebf" +
                    "c0c1c2c3c4c5c6c7c8c9cacbcccdcecf" +
                    "d0d1d2d3d4d5d6d7d8d9dadbdcdddedf" +
                    "e0e1e2e3e4e5e6e7e8e9eaebecedeeef" +
                    "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff").HexToByteArray(),
                Prk = "8adae09a2a307059478d309b26c4115a224cfaf6".HexToByteArray(),
                Okm = (
                    "0bd770a74d1160f7c9f12cd5912a06eb" +
                    "ff6adcae899d92191fe4305673ba2ffe" +
                    "8fa3f1a4e5ad79f3f334b3b202b2173c" +
                    "486ea37ce3d397ed034c7f9dfeb15c5e" +
                    "927336d0441f4c4300e2cff0d0900b52" +
                    "d3b4").HexToByteArray(),
            },
            new Rfc5869TestCase()
            {
                Name = "Test with SHA-1 and zero-length salt/info",
                Hash = HashAlgorithmName.SHA1,
                Ikm = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b".HexToByteArray(),
                Salt = Array.Empty<byte>(),
                Info = Array.Empty<byte>(),
                Prk = "da8c8a73c7fa77288ec6f5e7c297786aa0d32d01".HexToByteArray(),
                Okm = (
                    "0ac1af7002b3d761d1e55298da9d0506" +
                    "b9ae52057220a306e07b6b87e8df21d0" +
                    "ea00033de03984d34918").HexToByteArray(),
            },
            new Rfc5869TestCase()
            {
                Name = "Test with SHA-1, salt not provided (defaults to HashLen zero octets), zero-length info",
                Hash = HashAlgorithmName.SHA1,
                Ikm = "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".HexToByteArray(),
                Salt = null,
                Info = Array.Empty<byte>(),
                Prk = "2adccada18779e7c2077ad2eb19d3f3e731385dd".HexToByteArray(),
                Okm = (
                    "2c91117204d745f3500d636a62f64f0a" +
                    "b3bae548aa53d423b0d1f27ebba6f5e5" +
                    "673a081d70cce7acfc48").HexToByteArray(),
            },
        };

        public struct Rfc5869TestCase
        {
            public string Name { get; set; }
            public HashAlgorithmName Hash { get; set; }
            public byte[] Ikm { get; set; }
            public byte[] Salt { get; set; }
            public byte[] Info { get; set; }
            public byte[] Prk { get; set; }
            public byte[] Okm { get; set; }

            public override string ToString() => Name;
        }
    }
}
