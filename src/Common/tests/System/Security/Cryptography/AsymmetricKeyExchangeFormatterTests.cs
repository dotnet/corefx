// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;

namespace System.Security.Cryptography.Tests
{
    public class AsymmetricKeyExchangeFormatterTests
    {
        public static readonly byte[] HelloBytes = new ASCIIEncoding().GetBytes("Hello");

        public static void FormatterArguments(AsymmetricKeyExchangeFormatter formatter)
        {
            Assert.Throws<ArgumentNullException>(() => formatter.SetKey(null));
            Assert.Throws<CryptographicUnexpectedOperationException>(() => formatter.CreateKeyExchange(new byte[] { 0, 1, 2, 3 }));
        }

        public static void DeformatterArguments(AsymmetricKeyExchangeDeformatter deformatter)
        {
            Assert.Throws<ArgumentNullException>(() => deformatter.SetKey(null));
            Assert.Throws<CryptographicUnexpectedOperationException>(() => deformatter.DecryptKeyExchange(new byte[] { 0, 1, 2 }));
        }

        public static void VerifyDecryptKeyExchange(AsymmetricKeyExchangeFormatter formatter, AsymmetricKeyExchangeDeformatter deformatter)
        {
            byte[] encrypted = formatter.CreateKeyExchange(HelloBytes);
            byte[] decrypted = deformatter.DecryptKeyExchange(encrypted);
            Assert.Equal(HelloBytes, decrypted);

            unchecked { encrypted[encrypted.Length - 1]--; }
            Assert.ThrowsAny<CryptographicException>(() => deformatter.DecryptKeyExchange(encrypted));
        }
    }
}
