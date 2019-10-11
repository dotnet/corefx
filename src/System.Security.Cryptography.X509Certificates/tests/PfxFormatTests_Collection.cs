// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public sealed class PfxFormatTests_Collection : PfxFormatTests
    {
        protected override void ReadPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedCert)
        {
            using (ImportedCollection imported = Cert.Import(pfxBytes, correctPassword, s_importFlags))
            {
                X509Certificate2Collection coll = imported.Collection;
                Assert.Equal(1, coll.Count);

                AssertCertEquals(expectedCert, coll[0]);
            }
        }

        protected override void ReadEmptyPfx(byte[] pfxBytes, string correctPassword)
        {
            X509Certificate2Collection coll = new X509Certificate2Collection();
            coll.Import(pfxBytes, correctPassword, s_importFlags);
            Assert.Equal(0, coll.Count);
        }

        protected override void ReadWrongPassword(byte[] pfxBytes, string wrongPassword)
        {
            X509Certificate2Collection coll = new X509Certificate2Collection();

            CryptographicException ex = Assert.ThrowsAny<CryptographicException>(
                () => coll.Import(pfxBytes, wrongPassword, s_importFlags));

            AssertMessageContains("password", ex);
        }

        protected override void ReadUnreadablePfx(byte[] pfxBytes, string bestPassword)
        {
            X509Certificate2Collection coll = new X509Certificate2Collection();

            CryptographicException ex = Assert.ThrowsAny<CryptographicException>(
                () => coll.Import(pfxBytes, bestPassword, s_importFlags));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // NTE_FAIL
                Assert.Equal(-2146893792, ex.HResult);
            }
            else
            {
                Assert.NotNull(ex.InnerException);
            }
        }
    }
}
