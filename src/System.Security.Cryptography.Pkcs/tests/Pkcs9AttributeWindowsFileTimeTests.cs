// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

using Test.Cryptography;

namespace System.Security.Cryptography.Pkcs.Tests
{
    public static class Pkcs9AttributeWindowsFileTimeTests
    {
        [Fact]
        public static void InputDateTimeAsWindowsFileTimeBefore1601()
        {
            DateTime dt = new DateTime (1600, 12, 31, 11, 59, 59, DateTimeKind.Utc);
            Assert.ThrowsAny<CryptographicException>(() => new Pkcs9SigningTime(dt));
        }

        [Fact]
        public static void InputDateTimeAsWindowsFileTimeMinValue()
        {
            Assert.ThrowsAny<CryptographicException>(() => new Pkcs9SigningTime(DateTime.MinValue));
        }
    }
}

