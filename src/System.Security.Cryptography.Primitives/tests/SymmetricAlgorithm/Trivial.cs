// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Security.Cryptography.Encryption.Tests.Symmetric
{
    public static class TrivialTests
    {
        [Fact]
        public static void TestAutomaticKey()
        {
            using (Trivial t = new Trivial())
            {
                byte[] generatedKey = t.Key;
                Assert.Equal(generatedKey, Trivial.GeneratedKey);
                Assert.NotSame(generatedKey, Trivial.GeneratedKey);
            }
        }

        [Fact]
        public static void TestKey()
        {
            using (Trivial s = new Trivial())
            {
                Assert.Equal(0, s.KeySize);

                Assert.Throws<ArgumentNullException>(() => s.Key = null);

                // Testing KeySize and Key setter.
                int[] validKeySizes = { 40, 104, 152, 808, 816, 824, 832 };
                for (int keySize = -10; keySize < 200 * 8; keySize++)
                {
                    if (validKeySizes.Contains(keySize))
                    {
                        s.KeySize = keySize;
                        Assert.Equal(keySize, s.KeySize);
                    }
                    else
                    {
                        Assert.Throws<CryptographicException>(() => s.KeySize = keySize);
                    }

                    if (keySize >= 0)
                    {
                        int keySizeInBytes = keySize / 8;
                        byte[] key = GenerateRandom(keySizeInBytes);
                        if (validKeySizes.Contains(keySizeInBytes * 8))
                        {
                            s.SetKeySize(-1);
                            s.Key = key;
                            byte[] copyOfKey = s.Key;
                            Assert.Equal(key, copyOfKey);
                            Assert.Equal(key.Length * 8, s.KeySize);
                            Assert.NotSame(key, copyOfKey);
                        }
                        else
                        {
                            Assert.Throws<CryptographicException>(() => s.Key = key);
                        }
                    }
                }

                // Test overflow
                try
                {
                    byte[] hugeKey = new byte[536870917]; // value chosen so that when multiplied by 8 (bits) it overflows to the value 40
                    Assert.Throws<CryptographicException>(() => s.Key = hugeKey);
                }
                catch (OutOfMemoryException) { } // in case there isn't enough memory at test-time to allocate the large array
            }
        }

        [Fact]
        public static void KeySize_Key_LooseCoupling()
        {
            using (Trivial t = new Trivial())
            {
                t.GenerateKey();

                // Set the KeySizeValue field, then confirm that get_Key doesn't check it at all.
                const int UnusualKeySize = 51;
                t.SetKeySize(UnusualKeySize);
                Assert.Equal(UnusualKeySize, t.KeySize);

                byte[] key = t.Key;
                // It doesn't equal it in bytes
                Assert.NotEqual(UnusualKeySize, key.Length);
                // It doesn't equal it in bits
                Assert.NotEqual(UnusualKeySize, key.Length * 8);
            }
        }

        [Fact]
        public static void KeySize_CurrentValue_NotGrandfathered()
        {
            using (Trivial t = new Trivial())
            {
                t.SetKeySize(525600);
                Assert.Throws<CryptographicException>(() => t.KeySize = t.KeySize);
            }
        }

        [Fact]
        public static void TestAutomaticIv()
        {
            using (Trivial t = new Trivial())
            {
                t.BlockSize = 5 * 8;
                byte[] generatedIv = t.IV;
                Assert.Equal(generatedIv, Trivial.GeneratedIV);
                Assert.NotSame(generatedIv, Trivial.GeneratedIV);
            }
        }

        [Fact]
        public static void TestIv()
        {
            using (Trivial s = new Trivial())
            {
                Assert.Throws<ArgumentNullException>(() => s.IV = null);

                // Testing IV property setter
                {
                    s.BlockSize = 5 * 8;
                    {
                        byte[] iv = GenerateRandom(5);
                        s.IV = iv;
                        byte[] copyOfIv = s.IV;
                        Assert.Equal(iv, copyOfIv);
                        Assert.False(Object.ReferenceEquals(iv, copyOfIv));
                    }

                    {
                        byte[] iv = GenerateRandom(6);
                        Assert.Throws<CryptographicException>(() => s.IV = iv);
                    }
                }
            }
        }

        [Fact]
        public static void TestBlockSize()
        {
            using (Trivial s = new Trivial())
            {
                Assert.Equal(0, s.BlockSize);

                // Testing BlockSizeSetter.
                int[] validBlockSizes = { 40, 104, 152, 808, 816, 824, 832 };
                for (int blockSize = -10; blockSize < 200 * 8; blockSize++)
                {
                    if (validBlockSizes.Contains(blockSize))
                    {
                        s.BlockSize = blockSize;
                        Assert.Equal(blockSize, s.BlockSize);
                    }
                    else
                    {
                        Assert.Throws<CryptographicException>(() => s.BlockSize = blockSize);
                    }
                }
            }
            return;
        }

        [Fact]
        public static void GetCipherMode_NoValidation()
        {
            using (Trivial t = new Trivial())
            {
                t.SetCipherMode(24601);
                Assert.Equal(24601, (int)t.Mode);
            }
        }

        [Fact]
        public static void GetPaddingMode_NoValidation()
        {
            using (Trivial t = new Trivial())
            {
                t.SetPaddingMode(24601);
                Assert.Equal(24601, (int)t.Padding);
            }
        }

        [Fact]
        public static void LegalBlockSizes_CopiesData()
        {
            using (Trivial t = new Trivial())
            {
                KeySizes[] a = t.LegalBlockSizes;
                KeySizes[] b = t.LegalBlockSizes;

                Assert.NotSame(a, b);
            }
        }

        [Fact]
        public static void LegalKeySizes_CopiesData()
        {
            using (Trivial t = new Trivial())
            {
                KeySizes[] a = t.LegalKeySizes;
                KeySizes[] b = t.LegalKeySizes;

                Assert.NotSame(a, b);
            }
        }

        [Fact]
        public static void SetKey_Uses_LegalKeySizesProperty()
        {
            using (SymmetricAlgorithm s = new DoesNotSetKeySizesFields())
            {
                Assert.Throws<CryptographicException>(() => s.Key = Array.Empty<byte>());
                s.Key = new byte[16];
            }
        }

        [Fact]
        public static void SetKeySize_Uses_LegalKeySizesProperty()
        {
            using (SymmetricAlgorithm s = new DoesNotSetKeySizesFields())
            {
                Assert.Throws<CryptographicException>(() => s.KeySize = 0);
                s.KeySize = 128;
            }
        }

        [Fact]
        public static void SetBlockSize_Uses_LegalBlockSizesProperty()
        {
            using (SymmetricAlgorithm s = new DoesNotSetKeySizesFields())
            {
                Assert.Throws<CryptographicException>(() => s.BlockSize = 0);
                s.BlockSize = 8;
            }
        }
        
        private static byte[] GenerateRandom(int size)
        {
            byte[] data = new byte[size];
            Random r = new Random();
            for (int i = 0; i < size; i++)
            {
                data[i] = unchecked((byte)(r.Next()));
            }
            return data;
        }

        private class Trivial : SymmetricAlgorithm
        {
            public Trivial()
            {
                // Desktop's SymmetricAlgorithm reads from the field overly aggressively,
                // but in Core it always reads from the property. By setting the field
                // we're still happy on Desktop tests, and we can validate the default
                // behavior of the LegalBlockSizes property.
                LegalBlockSizesValue = new KeySizes[]
                    {
                        new KeySizes(5*8, -99*8, 0*8),
                        new KeySizes(13*8, 22*8, 6*8),
                        new KeySizes(101*8, 104*8, 1*8),
                    };

                // Desktop's SymmetricAlgorithm reads from the property correctly, but
                // we'll set the field here, anyways, to validate the default behavior
                // of the LegalKeySizes property.
                LegalKeySizesValue = new KeySizes[]
                    {
                        new KeySizes(5*8, -99*8, 0*8),
                        new KeySizes(13*8, 22*8, 6*8),
                        new KeySizes(101*8, 104*8, 1*8),
                    };
            }

            public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
            {
                throw new CreateDecryptorNotImplementedException();
            }

            public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
            {
                throw new CreateEncryptorNotImplementedException();
            }

            public override void GenerateIV()
            {
                IV = GeneratedIV;
            }

            public override void GenerateKey()
            {
                Key = GeneratedKey;
            }

            public void SetBlockSize(int blockSize)
            {
                BlockSizeValue = blockSize;
            }

            public void SetKeySize(int keySize)
            {
                KeySizeValue = keySize;
            }

            public void SetCipherMode(int anyValue)
            {
                ModeValue = (CipherMode)anyValue;
            }

            public void SetPaddingMode(int anyValue)
            {
                PaddingValue = (PaddingMode)anyValue;
            }

            public static readonly byte[] GeneratedKey = GenerateRandom(13);
            public static readonly byte[] GeneratedIV = GenerateRandom(5);
        }

        private class DoesNotSetKeySizesFields : SymmetricAlgorithm
        {
            public DoesNotSetKeySizesFields()
            {
                // Ensure the default values for the fields.
                Assert.Null(KeyValue);
                Assert.Null(IVValue);
                Assert.Null(LegalKeySizesValue);
                Assert.Null(LegalBlockSizesValue);
                Assert.Equal(0, KeySizeValue);
                Assert.Equal(0, BlockSizeValue);
                Assert.Equal(CipherMode.CBC, ModeValue);
                Assert.Equal(PaddingMode.PKCS7, PaddingValue);
            }

            public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
            {
                throw new CreateDecryptorNotImplementedException();
            }

            public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
            {
                throw new CreateEncryptorNotImplementedException();
            }

            public override void GenerateIV()
            {
                throw new GenerateIvNotImplementedException();
            }

            public override void GenerateKey()
            {
                throw new GenerateKeyNotImplementedException();
            }

            public override KeySizes[] LegalBlockSizes
            {
                get { return new[] { new KeySizes(8, 64, 8) }; }
            }

            public override KeySizes[] LegalKeySizes
            {
                get { return new[] { new KeySizes(64, 128, 8) }; }
            }
        }

        private class GenerateIvNotImplementedException : Exception { }
        private class GenerateKeyNotImplementedException : Exception { }
        private class CreateDecryptorNotImplementedException : Exception { }
        private class CreateEncryptorNotImplementedException : Exception { }
    }
}
