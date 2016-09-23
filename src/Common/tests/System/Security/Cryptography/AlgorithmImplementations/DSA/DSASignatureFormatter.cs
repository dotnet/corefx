// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Tests;
using Xunit;

namespace System.Security.Cryptography.Dsa.Tests
{
    public partial class DSASignatureFormatterTests : AsymmetricSignatureFormatterTests
    {
        [Fact]
        public static void VerifySignature_SHA1()
        {
            using (DSA dsa = DSAFactory.Create())
            {
                var formatter = new DSASignatureFormatter(dsa);
                var deformatter = new DSASignatureDeformatter(dsa);
                VerifySignature(formatter, deformatter, SHA1.Create(), "SHA1");
                VerifySignature(formatter, deformatter, SHA1.Create(), "sha1");
            }
        }

        [ConditionalFact(nameof(SupportsFips186_3))]
        public static void VerifySignature_SHA256()
        {
            using (DSA dsa = DSAFactory.Create())
            {
                var formatter = new DSASignatureFormatter(dsa);
                var deformatter = new DSASignatureDeformatter(dsa);
                VerifySignature(formatter, deformatter, SHA256.Create(), "SHA256");
                VerifySignature(formatter, deformatter, SHA256.Create(), "sha256");
            }
        }

        [Fact]
        public static void InvalidHashAlgorithm()
        {
            using (DSA dsa = DSAFactory.Create())
            {
                var formatter = new DSASignatureFormatter(dsa);
                var deformatter = new DSASignatureDeformatter(dsa);

                // Unlike RSA, DSA will throw during SetHashAlgorithm
                Assert.Throws<CryptographicUnexpectedOperationException>(() =>
                    formatter.SetHashAlgorithm("INVALIDVALUE"));
                Assert.Throws<CryptographicUnexpectedOperationException>(() =>
                    deformatter.SetHashAlgorithm("INVALIDVALUE"));
            }
        }

        [Fact]
        public static void VerifyKnownSignature()
        {
            using (DSA dsa = DSAFactory.Create())
            {
                byte[] hash = SHA1.Create().ComputeHash(DSATestData.GetDSA1024_186_2_Data());
                byte[] signature = DSATestData.GetDSA1024_186_2_Signature();

                DSAParameters dsaParameters = DSATestData.GetDSA1024_186_2_Params();
                dsa.ImportParameters(dsaParameters);
                var deformatter = new DSASignatureDeformatter(dsa);
                deformatter.VerifySignature(hash, signature);

                // Negative case
                signature[signature.Length - 1] ^= 0xff;
                Assert.False(deformatter.VerifySignature(hash, signature));
            }
        }

        public static bool SupportsFips186_3
        {
            get
            {
                return DSAFactory.SupportsFips186_3;
            }
        }
    }
}
