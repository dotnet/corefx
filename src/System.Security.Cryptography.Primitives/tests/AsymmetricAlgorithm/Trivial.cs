// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Security.Cryptography.Encryption.Tests.Asymmetric
{
    public static class TrivialTests
    {
        [Fact]
        public static void TestKeySize()
        {
            using (Trivial s = new Trivial())
            {
                Assert.Equal(0, s.KeySize);

                // Testing KeySize.
                int[] validKeySizes = { 40, 104, 152, 808, 809, 810, 816, 824, 832 };
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
                }
            }
            return;
        }

        [Fact]
        public static void TestInvalidAlgorithm()
        {
            var invalid = new Invalid();
            Assert.Throws<NullReferenceException>(() => invalid.LegalKeySizes);
        }

        private class Invalid : AsymmetricAlgorithm
        {
            // Valid algorithsm must override LegalKeySizes
        }

        private class Trivial : AsymmetricAlgorithm
        {
            public Trivial()
            {
                //
                // Although the desktop CLR allows overriding the LegalKeySizes property, 
                // the BlockSize setter does not invoke the overriding method when validating
                // the blockSize. Instead, it accesses the underlying field (LegalKeySizesValue) directly.
                //
                // We've since removed this field from the public surface area (and fixed the BlockSize property
                // to call LegalKeySizes rather than the underlying field.) To make this test also run on the desktop, however,
                // we will also set the LegalKeySizesValue field if present.
                //
                FieldInfo legalKeySizesValue = typeof(AsymmetricAlgorithm).GetTypeInfo().GetDeclaredField("LegalKeySizesValue");
                if (legalKeySizesValue != null && legalKeySizesValue.IsFamily)
                {
                    legalKeySizesValue.SetValue(this, LegalKeySizes);
                }
            }

            public override KeySizes[] LegalKeySizes
            {
                get
                {
                    return new KeySizes[]
                    {
                        new KeySizes(5*8, -99*8, 0*8),
                        new KeySizes(13*8, 22*8, 6*8),
                        new KeySizes(101*8, 104*8, 1*8),
                        new KeySizes(101*8 + 1, 101*8 + 2, 1),
                    };
                }
            }
        }
    }
}
