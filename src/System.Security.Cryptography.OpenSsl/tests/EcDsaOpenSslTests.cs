// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.EcDsaOpenSsl.Tests
{
    public static class EcDsaOpenSslTests
    {
        [Fact]
        public static void DefaultCtor()
        {
            using (ECDsaOpenSsl e = new ECDsaOpenSsl())
            {
                int keySize = e.KeySize;
                Assert.Equal(521, keySize);
                e.Exercise();
            }
        }

        [Fact]
        public static void Ctor224()
        {
            int expectedKeySize = 224;
            using (ECDsaOpenSsl e = new ECDsaOpenSsl(expectedKeySize))
            {
                int keySize = e.KeySize;
                Assert.Equal(expectedKeySize, keySize);
                e.Exercise();
            }
        }

        [Fact]
        public static void Ctor384()
        {
            int expectedKeySize = 384;
            using (ECDsaOpenSsl e = new ECDsaOpenSsl(expectedKeySize))
            {
                int keySize = e.KeySize;
                Assert.Equal(expectedKeySize, keySize);
                e.Exercise();
            }
        }

        [Fact]
        public static void Ctor521()
        {
            int expectedKeySize = 521;
            using (ECDsaOpenSsl e = new ECDsaOpenSsl(expectedKeySize))
            {
                int keySize = e.KeySize;
                Assert.Equal(expectedKeySize, keySize);
                e.Exercise();
            }
        }

        [Fact]
        public static void CtorHandle224()
        {
            IntPtr ecKey = EC_KEY_new_by_curve_name(NID_secp224r1);
            Assert.NotEqual(IntPtr.Zero, ecKey);
            int success = EC_KEY_generate_key(ecKey);
            Assert.NotEqual(0, success);

            using (ECDsaOpenSsl e = new ECDsaOpenSsl(ecKey))
            {
                int keySize = e.KeySize;
                Assert.Equal(224, keySize);
                e.Exercise();
            }

            EC_KEY_free(ecKey);
        }

        [Fact]
        public static void CtorHandle384()
        {
            IntPtr ecKey = EC_KEY_new_by_curve_name(NID_secp384r1);
            Assert.NotEqual(IntPtr.Zero, ecKey);
            int success = EC_KEY_generate_key(ecKey);
            Assert.NotEqual(0, success);

            using (ECDsaOpenSsl e = new ECDsaOpenSsl(ecKey))
            {
                int keySize = e.KeySize;
                Assert.Equal(384, keySize);
                e.Exercise();
            }

            EC_KEY_free(ecKey);
        }

        [Fact]
        public static void CtorHandle521()
        {
            IntPtr ecKey = EC_KEY_new_by_curve_name(NID_secp521r1);
            Assert.NotEqual(IntPtr.Zero, ecKey);
            int success = EC_KEY_generate_key(ecKey);
            Assert.NotEqual(0, success);

            using (ECDsaOpenSsl e = new ECDsaOpenSsl(ecKey))
            {
                int keySize = e.KeySize;
                Assert.Equal(521, keySize);
                e.Exercise();
            }

            EC_KEY_free(ecKey);
        }

        [Fact]
        public static void CtorHandleDuplicate()
        {
            IntPtr ecKey = EC_KEY_new_by_curve_name(NID_secp521r1);
            Assert.NotEqual(IntPtr.Zero, ecKey);
            int success = EC_KEY_generate_key(ecKey);
            Assert.NotEqual(0, success);

            using (ECDsaOpenSsl e = new ECDsaOpenSsl(ecKey))
            {
                // Make sure ECDsaOpenSsl did his own ref-count bump.
                EC_KEY_free(ecKey);

                int keySize = e.KeySize;
                Assert.Equal(521, keySize);
                e.Exercise();
            }
        }

        [Fact]
        public static void KeySizeProp()
        {
            using (ECDsaOpenSsl e = new ECDsaOpenSsl())
            {
                e.KeySize = 384;
                Assert.Equal(384, e.KeySize);
                e.Exercise();

                e.KeySize = 224;
                Assert.Equal(224, e.KeySize);
                e.Exercise();
            }
        }

        [Fact]
        public static void VerifyDuplicateKey_ValidHandle()
        {
            byte[] data = ByteUtils.RepeatByte(0x71, 11);

            using (ECDsaOpenSsl first = new ECDsaOpenSsl())
            using (SafeEvpPKeyHandle firstHandle = first.DuplicateKeyHandle())
            {
                using (ECDsa second = new ECDsaOpenSsl(firstHandle))
                {
                    byte[] signed = second.SignData(data, HashAlgorithmName.SHA512);
                    Assert.True(first.VerifyData(data, signed, HashAlgorithmName.SHA512));
                }
            }
        }

        [Fact]
        public static void VerifyDuplicateKey_DistinctHandles()
        {
            using (ECDsaOpenSsl first = new ECDsaOpenSsl())
            using (SafeEvpPKeyHandle firstHandle = first.DuplicateKeyHandle())
            using (SafeEvpPKeyHandle firstHandle2 = first.DuplicateKeyHandle())
            {
                Assert.NotSame(firstHandle, firstHandle2);
            }
        }

        [Fact]
        public static void VerifyDuplicateKey_RefCounts()
        {
            byte[] data = ByteUtils.RepeatByte(0x74, 11);
            byte[] signature;
            ECDsa second;

            using (ECDsaOpenSsl first = new ECDsaOpenSsl())
            using (SafeEvpPKeyHandle firstHandle = first.DuplicateKeyHandle())
            {
                signature = first.SignData(data, HashAlgorithmName.SHA384);
                second = new ECDsaOpenSsl(firstHandle);
            }

            // Now show that second still works, despite first and firstHandle being Disposed.
            using (second)
            {
                Assert.True(second.VerifyData(data, signature, HashAlgorithmName.SHA384));
            }
        }

        [Fact]
        public static void VerifyDuplicateKey_NullHandle()
        {
            SafeEvpPKeyHandle pkey = null;
            Assert.Throws<ArgumentNullException>(() => new RSAOpenSsl(pkey));
        }

        [Fact]
        public static void VerifyDuplicateKey_InvalidHandle()
        {
            using (ECDsaOpenSsl ecdsa = new ECDsaOpenSsl())
            {
                SafeEvpPKeyHandle pkey = ecdsa.DuplicateKeyHandle();

                using (pkey)
                {
                }

                Assert.Throws<ArgumentException>(() => new ECDsaOpenSsl(pkey));
            }
        }

        [Fact]
        public static void VerifyDuplicateKey_NeverValidHandle()
        {
            using (SafeEvpPKeyHandle pkey = new SafeEvpPKeyHandle(IntPtr.Zero, false))
            {
                Assert.Throws<ArgumentException>(() => new ECDsaOpenSsl(pkey));
            }
        }

        [Fact]
        public static void VerifyDuplicateKey_RsaHandle()
        {
            using (RSAOpenSsl rsa = new RSAOpenSsl())
            using (SafeEvpPKeyHandle pkey = rsa.DuplicateKeyHandle())
            {
                Assert.ThrowsAny<CryptographicException>(() => new ECDsaOpenSsl(pkey));
            }
        }

        private static void Exercise(this ECDsaOpenSsl e)
        {
            // Make a few calls on this to ensure we aren't broken due to bad/prematurely released handles.

            int keySize = e.KeySize;

            byte[] data = new byte[0x10];
            byte[] sig = e.SignData(data, 0, data.Length, HashAlgorithmName.SHA1);
            bool verified = e.VerifyData(data, sig, HashAlgorithmName.SHA1);
            Assert.True(verified);
            sig[sig.Length - 1]++;
            verified = e.VerifyData(data, sig, HashAlgorithmName.SHA1);
            Assert.False(verified);
        }

        private const int NID_secp224r1 = 713;
        private const int NID_secp384r1 = 715;
        private const int NID_secp521r1 = 716;

        [DllImport("libcrypto")]
        private static extern IntPtr EC_KEY_new_by_curve_name(int nid);

        [DllImport("libcrypto")]
        private static extern int EC_KEY_generate_key(IntPtr ecKey);

        [DllImport("libcrypto")]
        private static extern void EC_KEY_free(IntPtr r);
    }
}
 
