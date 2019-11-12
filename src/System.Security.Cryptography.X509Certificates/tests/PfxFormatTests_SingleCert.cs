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
            X509Certificate2 expectedCert,
            Action<X509Certificate2> otherWork)
        {
            ReadPfx(pfxBytes, correctPassword, expectedCert, otherWork, s_importFlags);
            ReadPfx(pfxBytes, correctPassword, expectedCert, otherWork, s_exportableImportFlags);
        }

        protected override void ReadMultiPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedSingleCert,
            X509Certificate2[] expectedOrder,
            Action<X509Certificate2> perCertOtherWork)
        {
            ReadPfx(pfxBytes, correctPassword, expectedSingleCert, perCertOtherWork, s_importFlags);
            ReadPfx(pfxBytes, correctPassword, expectedSingleCert, perCertOtherWork, s_exportableImportFlags);
        }

        private void ReadPfx(
            byte[] pfxBytes,
            string correctPassword,
            X509Certificate2 expectedCert,
            Action<X509Certificate2> otherWork,
            X509KeyStorageFlags flags)
        {
            using (X509Certificate2 cert = new X509Certificate2(pfxBytes, correctPassword, flags))
            {
                AssertCertEquals(expectedCert, cert);
                otherWork?.Invoke(cert);
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

        protected override void ReadUnreadablePfx(
            byte[] pfxBytes,
            string bestPassword,
            int win32Error,
            int altWin32Error)
        {
            CryptographicException ex = Assert.ThrowsAny<CryptographicException>(
                () => new X509Certificate2(pfxBytes, bestPassword, s_importFlags));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (altWin32Error != 0 && ex.HResult != altWin32Error)
                {
                    Assert.Equal(win32Error, ex.HResult);
                }
            }
            else
            {
                Assert.NotNull(ex.InnerException);
            }
        }
    }
}
