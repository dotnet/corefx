// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;

namespace System.Security.Cryptography.Tests
{
    public class AsymmetricSignatureFormatterTests
    {
        public static readonly byte[] HelloBytes = new ASCIIEncoding().GetBytes("Hello");

        public static void FormatterArguments(AsymmetricSignatureFormatter formatter)
        {
            Assert.Throws<ArgumentNullException>(() => formatter.SetKey(null));
            Assert.Throws<ArgumentNullException>(() => formatter.CreateSignature((byte[])null));
            Assert.Throws<CryptographicUnexpectedOperationException>(() => formatter.CreateSignature(new byte[] { 0, 1, 2, 3 }));
        }

        public static void DeformatterArguments(AsymmetricSignatureDeformatter deformatter)
        {
            Assert.Throws<ArgumentNullException>(() => deformatter.SetKey(null));
            Assert.Throws<ArgumentNullException>(() => deformatter.VerifySignature((byte[])null, new byte[] { 0, 1, 2 }));
            Assert.Throws<CryptographicUnexpectedOperationException>(() => deformatter.VerifySignature(new byte[] { 0, 1, 2 }, new byte[] { 0, 1, 2 }));
        }

        public static void VerifySignature(AsymmetricSignatureFormatter formatter, AsymmetricSignatureDeformatter deformatter, HashAlgorithm hashAlgorithm, HashAlgorithmName hashAlgorithmName)
        {
            formatter.SetHashAlgorithm(hashAlgorithmName.Name);
            deformatter.SetHashAlgorithm(hashAlgorithmName.Name);

            byte[] hash = hashAlgorithm.ComputeHash(HelloBytes);

            VerifySignature_ComputeHash(formatter, deformatter, hash);

#if netstandard17
            // Check that the hash is preserved from the ComputeHash above
            VerifySignature_HashAlgorithm(formatter, deformatter, hashAlgorithm);
#endif
        }

        private static void VerifySignature_ComputeHash(AsymmetricSignatureFormatter formatter, AsymmetricSignatureDeformatter deformatter, byte[] hash)
        {
            byte[] signature = formatter.CreateSignature(hash);
            Assert.True(deformatter.VerifySignature(hash, signature));

            unchecked { signature[signature.Length - 1]--; }
            Assert.False(deformatter.VerifySignature(hash, signature));
        }

#if netstandard17
        private static void VerifySignature_HashAlgorithm(AsymmetricSignatureFormatter formatter, AsymmetricSignatureDeformatter deformatter, HashAlgorithm hashAlgorithm)
        {
            byte[] signature = formatter.CreateSignature(hashAlgorithm);
            Assert.True(deformatter.VerifySignature(hashAlgorithm, signature));

            unchecked { signature[signature.Length - 1]--; }
            Assert.False(deformatter.VerifySignature(hashAlgorithm, signature));
        }
#endif
    }
}
