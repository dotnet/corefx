// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class SecureStringMarshalTests
    {
        [Fact]
        public void SecureStringToCoTaskMemAnsi_ValidString_ReturnsExpected()
        {
            using (var secureString = new SecureString())
            {
                IntPtr result = SecureStringMarshal.SecureStringToCoTaskMemAnsi(secureString);
                Marshal.ZeroFreeCoTaskMemAnsi(result);
            }
        }

        [Fact]
        public void SecureStringToCoTaskMemAnsi_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => SecureStringMarshal.SecureStringToCoTaskMemAnsi(null));
        }

        [Fact]
        public void SecureStringToCoTaskMemAnsi_Disposed_ThrowsObjectDisposedException()
        {
            var secureString = new SecureString();
            secureString.Dispose();

            Assert.Throws<ObjectDisposedException>(() => SecureStringMarshal.SecureStringToCoTaskMemAnsi(secureString));
        }

        [Fact]
        public void SecureStringToCoTaskMemUnicode_ValidString_ReturnsExpected()
        {
            using (var secureString = new SecureString())
            {
                IntPtr result = SecureStringMarshal.SecureStringToCoTaskMemUnicode(secureString);
                Marshal.ZeroFreeCoTaskMemUnicode(result);
            }
        }

        [Fact]
        public void SecureStringToCoTaskMemUnicode_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => SecureStringMarshal.SecureStringToCoTaskMemUnicode(null));
        }

        [Fact]
        public void SecureStringToCoTaskMemUnicode_Disposed_ThrowsObjectDisposedException()
        {
            var secureString = new SecureString();
            secureString.Dispose();

            Assert.Throws<ObjectDisposedException>(() => SecureStringMarshal.SecureStringToCoTaskMemUnicode(secureString));
        }

        [Fact]
        public void SecureStringToGlobalAllocAnsi_ValidString_ReturnsExpected()
        {
            using (var secureString = new SecureString())
            {
                IntPtr result = SecureStringMarshal.SecureStringToGlobalAllocAnsi(secureString);
                Marshal.ZeroFreeGlobalAllocAnsi(result);
            }
        }

        [Fact]
        public void SecureStringToGlobalAllocUnsi_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => SecureStringMarshal.SecureStringToGlobalAllocAnsi(null));
        }

        [Fact]
        public void SecureStringToGlobalAllocAnsi_Disposed_ThrowsObjectDisposedException()
        {
            var secureString = new SecureString();
            secureString.Dispose();

            Assert.Throws<ObjectDisposedException>(() => SecureStringMarshal.SecureStringToGlobalAllocAnsi(secureString));
        }

        [Fact]
        public void SecureStringToGlobalAllocUnicode_ValidString_ReturnsExpected()
        {
            using (var secureString = new SecureString())
            {
                IntPtr result = SecureStringMarshal.SecureStringToGlobalAllocUnicode(secureString);
                Marshal.ZeroFreeGlobalAllocAnsi(result);
            }
        }

        [Fact]
        public void SecureStringToGlobalAllocUnicode_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => SecureStringMarshal.SecureStringToGlobalAllocUnicode(null));
        }

        [Fact]
        public void SecureStringToGlobalAllocUnicode_Disposed_ThrowsObjectDisposedException()
        {
            var secureString = new SecureString();
            secureString.Dispose();

            Assert.Throws<ObjectDisposedException>(() => SecureStringMarshal.SecureStringToGlobalAllocUnicode(secureString));
        }
    }
}
