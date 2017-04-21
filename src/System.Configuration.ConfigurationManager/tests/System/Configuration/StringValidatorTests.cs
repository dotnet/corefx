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
            // This should complete with no errors.
            StringValidator validator = new StringValidator(5, 10);
        }

        [Fact]
        public void Constructor_MinValueMaxValueAndInvalidChars()
        {
            // this should complete with no errors.
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

        [Fact]
        public void Validate_PassInNonString()
        {
            StringValidator validator = new StringValidator(5);
            ArgumentException thrownException = Assert.Throws<ArgumentException>(() => validator.Validate(5));
        }

        [Fact]
        public void Validate_StringTooSmall()
        {
            StringValidator validator = new StringValidator(5);
            ArgumentException thrownException = Assert.Throws<ArgumentException>(() => validator.Validate("Hi"));
        }

        [Fact]
        public void Validate_StringTooBig()
        {
            StringValidator validator = new StringValidator(5, 10);
            ArgumentException thrownException = Assert.Throws<ArgumentException>(() => validator.Validate("This is more than ten"));
        }

        [Fact]
        public void Validate_EmptyString()
        {
            // this should complete with no errors.
            StringValidator validator = new StringValidator(0);
            validator.Validate(string.Empty);
        }

        [Theory]
        [InlineData("Hello-")]
        [InlineData("Hello_")]
        [InlineData("-Hello")]
        [InlineData("_Hello")]
        [InlineData("He-llo")]
        [InlineData("He_llo")]
        [InlineData("H-e-l-l-o")]
        [InlineData("H_e_l_l_o_")]
        [InlineData("H_e-l_l-o_")]
        [InlineData("He__llo")]
        [InlineData("He--;o")]
        [InlineData("H_-el- -o")]
        [InlineData("_")]
        [InlineData("-")]
        public void Validate_UsinginvalidCharacters(string stringToValidate)
        {
            StringValidator validator = new StringValidator(1, 20, "_-");
            ArgumentException result = Assert.Throws<ArgumentException>(() => validator.Validate(stringToValidate));
        }

        [Fact]
        public void Validate_NoINvalidCharactersSpecified()
        {
            StringValidator validator = new StringValidator(5);
            validator.Validate("Hello");
        }
    }
}