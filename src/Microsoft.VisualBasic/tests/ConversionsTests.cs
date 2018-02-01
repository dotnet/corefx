// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using Xunit;

namespace Microsoft.VisualBasic.Tests
{
    public class ConversionsTests
    {
        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("5", true)]
        [InlineData("0", false)]
        [InlineData("5.5", true)]
        [InlineData("0.0", false)]
        [InlineData("&h5", true)]
        [InlineData("&h0", false)]
        [InlineData("&o5", true)]
        [InlineData("&o0", false)]
        public void ToBoolean_String_ReturnsExpected(string str, bool expected)
        {
            Assert.Equal(expected, Conversions.ToBoolean(str));
        }

        [Theory]
        [InlineData("yes")]
        [InlineData("contoso")]
        public void ToBoolean_String_ThrowsOnInvalidFormat(string str)
        {
            Assert.Throws<InvalidCastException>(() => Conversions.ToBoolean(str));
        }

    }
}
