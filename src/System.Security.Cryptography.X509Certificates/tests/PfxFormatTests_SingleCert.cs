// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public sealed class PfxFormatTests_SingleCert : PfxFormatTests
    {
        protected override void ReadPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedCert)
        {
            using (X509Certificate2 cert = new X509Certificate2(pfxBytes, correctPassword, s_importFlags))
            {
                AssertCertEquals(expectedCert, cert);
            }
        }

        protected override void ReadEmptyPfx(byte[] pfxBytes, string correctPassword)
        {
            CryptographicException ex = Assert.Throws<CryptographicException>(
                () => new X509Certificate2(pfxBytes, correctPassword, s_importFlags));

            AssertMessageContains("no certificates", ex);
        }

        protected override void ReadWrongPassword(byte[] pfxBytes, string wrongPassword)
        {
            CryptographicException ex = Assert.ThrowsAny<CryptographicException>(
                () => new X509Certificate2(pfxBytes, wrongPassword, s_importFlags));

            AssertMessageContains("password", ex);
        }

        protected override void ReadUnreadablePfx(byte[] pfxBytes, string bestPassword)
        {
            CryptographicException ex = Assert.ThrowsAny<CryptographicException>(
                () => new X509Certificate2(pfxBytes, bestPassword, s_importFlags));

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
