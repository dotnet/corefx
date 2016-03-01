// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Globalization.Tests
{
    public class TextInfoIsReadOnly
    {
        [Theory]
        [InlineData("en-US", false)]
        [InlineData("fr-FR", false)]
        public void TestEnUSTextInfo(string name, bool expected)
        {
            TextInfo textInfo = new CultureInfo(name).TextInfo;
            Assert.Equal(expected, textInfo.IsReadOnly);
        }
    }
}
