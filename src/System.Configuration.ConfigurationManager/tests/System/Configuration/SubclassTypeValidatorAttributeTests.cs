// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using Xunit;

namespace System.ConfigurationTests
{
    public class SubclassTypeValidatorAttributeTests
    {
        [Fact]
        public void GetBaseClass_PassValidTypeToConstructor()
        {
            SubclassTypeValidatorAttribute attribute = new SubclassTypeValidatorAttribute(typeof(string));
            Type test = attribute.BaseClass;
            Assert.Equal(test, typeof(string));
        }

        [Fact]
        public void GetBaseClass_PassNullIntoConstructor()
        {
            SubclassTypeValidatorAttribute attribute = new SubclassTypeValidatorAttribute(null);
            Type test = attribute.BaseClass;
            Assert.Null(test);
        }
    }
}