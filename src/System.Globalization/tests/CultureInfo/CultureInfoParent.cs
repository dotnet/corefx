// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.Tests
{
    public class CultureInfoParent
    {
        [Fact]
        public void PosTest1()
        {
            CultureInfo myExpectParentCulture = new CultureInfo("en");
            CultureInfo myTestCulture = new CultureInfo("en-us");
            Assert.Equal(myTestCulture.Parent, myExpectParentCulture);
        }

        [Fact]
        public void PosTest2()
        {
            CultureInfo myTestCulture = new CultureInfo("en");
            CultureInfo myExpectParent = CultureInfo.InvariantCulture;
            Assert.Equal(myTestCulture.Parent, myExpectParent);
        }

        [Fact]
        public void PosTest3()
        {
            CultureInfo myExpectParent1 = new CultureInfo("");
            CultureInfo myTestCulture = CultureInfo.InvariantCulture;
            Assert.Equal(myTestCulture.Parent, myExpectParent1);
        }

        [Fact]
        public void PosTest4()
        {
            CultureInfo myExpectParentCulture = new CultureInfo("uz-Cyrl-UZ");
            Assert.Equal("uz-Cyrl", myExpectParentCulture.Parent.Name);
            Assert.Equal("uz", myExpectParentCulture.Parent.Parent.Name);
            Assert.Equal("", myExpectParentCulture.Parent.Parent.Parent.Name);
        }
    }
}
