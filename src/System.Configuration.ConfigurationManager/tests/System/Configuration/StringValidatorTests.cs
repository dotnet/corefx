// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using Xunit;

namespace System.ConfigurationTests
{
    public class StringValidatorTests
    {
        [Fact]
        public void Constructor_MinLength()
        {
            //We don't expect this to fail
            StringValidator validator = new StringValidator(5);
        }

        [Fact]
        public void Constructor_MinValueAndMaxValue()
        {
            //Won't fail
            StringValidator validator = new StringValidator(5, 10);
        }

        [Fact]
        public void Constructor_MinValueMaxValueAndInvalidChars()
        {
            //Won't fail.
            StringValidator validator = new StringValidator(5, 10, "abcde");
        }

        [Fact]
        public void CanValidate_PassInStringType()
        {
            StringValidator validator = new StringValidator(5);
            bool result = validator.CanValidate(typeof(string));
            Assert.True(result);
        }

        [Fact]
        public void CanValidate_PassInNotStringType()
        {
            StringValidator validator = new StringValidator(5);
            bool result = validator.CanValidate(typeof(Int32));
            Assert.False(result);
        }
    }
}