// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Tests;
using Xunit;

namespace System.Security.Cryptography.Dsa.Tests
{
    public class DSAFormatterTests
    {
        [Fact]
        public static void FormatterArguments()
        {
            AsymmetricSignatureFormatterTests.FormatterArguments(new DSASignatureFormatter());
        }

        [Fact]
        public static void DeformatterArguments()
        {
            AsymmetricSignatureFormatterTests.DeformatterArguments(new DSASignatureDeformatter());
        }

        [Fact]
        public static void VerifySignature_SHA1()
        {
            using (DSA dsa = DSAFactory.Create())
            {

                var formatter = new DSASignatureFormatter(dsa);
                var deformatter = new DSASignatureDeformatter(dsa);
                AsymmetricSignatureFormatterTests.VerifySignature(formatter, deformatter, SHA1.Create(), HashAlgorithmName.SHA1);
            }
        }

        [ConditionalFact(nameof(SupportsFips186_3))]
        public static void VerifySignature_SHA256()
        {
            using (DSA dsa = DSAFactory.Create())
            {
                var formatter = new DSASignatureFormatter(dsa);
                var deformatter = new DSASignatureDeformatter(dsa);
                AsymmetricSignatureFormatterTests.VerifySignature(formatter, deformatter, SHA256.Create(), HashAlgorithmName.SHA256);
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