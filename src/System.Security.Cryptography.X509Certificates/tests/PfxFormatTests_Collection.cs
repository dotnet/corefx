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
            X509Certificate2 expectedCert,
            Action<X509Certificate2> otherWork)
        {
            using (ImportedCollection imported = Cert.Import(pfxBytes, correctPassword, s_importFlags))
            {
                X509Certificate2Collection coll = imported.Collection;
                Assert.Equal(1, coll.Count);

                AssertCertEquals(expectedCert, coll[0]);
                otherWork?.Invoke(coll[0]);
            }
        }

        protected override void ReadMultiPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedSingleCert,
            X509Certificate2[] expectedOrder)
        {
            using (ImportedCollection imported = Cert.Import(pfxBytes, correctPassword, s_importFlags))
            {
                X509Certificate2Collection coll = imported.Collection;
                Assert.Equal(expectedOrder.Length, coll.Count);

                for (int i = 0; i < coll.Count; i++)
                {
                    AssertCertEquals(expectedOrder[i], coll[i]);
                }
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

        protected override void ReadUnreadablePfx(byte[] pfxBytes, string bestPassword, int win32Error)
        {
            X509Certificate2Collection coll = new X509Certificate2Collection();

            CryptographicException ex = Assert.ThrowsAny<CryptographicException>(
                () => coll.Import(pfxBytes, bestPassword, s_importFlags));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Equal(win32Error, ex.HResult);
            }
            else
            {
                Assert.NotNull(ex.InnerException);
            }
        }
    }
}
