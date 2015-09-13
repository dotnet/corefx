// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Xunit;

namespace System.Security.Cryptography.Encryption.Tests.Asymmetric
{
    public static class CryptographicExceptionTests
    {
        [Fact]
        public static void Ctor()
        {
            string message = "Some Message";
            var inner = new FormatException(message);

            Assert.NotNull(new CryptographicException().Message);
            Assert.Equal(message, new CryptographicException(message).Message);
            Assert.Equal(message + " 12345", new CryptographicException(message + " {0}", "12345").Message);
            Assert.Equal(5, new CryptographicException(5).HResult);
            Assert.Same(inner, new CryptographicException(message, inner).InnerException);
            Assert.Equal(message, new CryptographicException(message, inner).Message);
        }

        [Theory]
        [InlineData(0x00000000, "The operation completed successfully")]
        [InlineData(0x80090013, "Invalid provider specified")]
        [InlineData(0x80090016, "Keyset does not exist")]
        [PlatformSpecific(PlatformID.Windows)]
        public static void MessageFromHR_Windows(uint code, string msg)
        {
            // The strings may or may not be localized, only en-US strings should be considered.
            if (CultureInfo.CurrentCulture.Name != "en-US")
            {
                return;
            }

            CryptographicException ex = new CryptographicException(unchecked((int)code));

            string debugMsg = string.Format("(0x{0:X8}) {1}", code, msg);
            string[] allowedMessages = { msg, debugMsg };

            Assert.Contains(ex.Message, allowedMessages);
        }

        [Theory]
        [InlineData(0x00000000, "error:00000000:lib(0):func(0):reason(0)")]
        [InlineData(0x0B07806F, "error:0B07806F:x509 certificate routines:X509_PUBKEY_set:unsupported algorithm")]
        [InlineData(0x0B07D065, "error:0B07D065:x509 certificate routines:X509_STORE_add_crl:cert already in hash table")]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public static void MessageFromHR_Unix(uint code, string msg)
        {
            // OpenSSL doesn't seem to localize their messages, which means we don't
            // need to do a culture check for the Unix version of this test.  If it's ever a problem
            // then we can add the filter.

            // Likewise, if there's a problem across OpenSSL versions, figure out what
            // part of the string is reliable, like "error:0x{0:X8}:"
            CryptographicException ex = new CryptographicException(unchecked((int)code));

            Assert.Equal(msg, ex.Message);
        }
    }
}
