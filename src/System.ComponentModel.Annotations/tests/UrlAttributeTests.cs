// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.DataAnnotations
{
    public class UrlAttributeTests
    {
        private static readonly ValidationContext s_testValidationContext = new ValidationContext(new object());

        [Fact]
        public static void UrlAttribute_creation_DataType_and_CustomDataType()
        {
            var attribute = new UrlAttribute();
            Assert.Equal(DataType.Url, attribute.DataType);
            Assert.Null(attribute.CustomDataType);
        }

        [Fact]
        public static void Validate_successful_for_valid_values()
        {
            var attribute = new UrlAttribute();

            AssertEx.DoesNotThrow(() => attribute.Validate(null, s_testValidationContext)); // Null is valid
            AssertEx.DoesNotThrow(() => attribute.Validate("http://foo.bar", s_testValidationContext));
            AssertEx.DoesNotThrow(() => attribute.Validate("https://foo.bar", s_testValidationContext));
            AssertEx.DoesNotThrow(() => attribute.Validate("ftp://foo.bar", s_testValidationContext));
        }

        [Fact]
        public static void Validate_throws_for_invalid_values()
        {
            var attribute = new UrlAttribute();

            Assert.Throws<ValidationException>(() => attribute.Validate("file:///foo.bar", s_testValidationContext)); // file scheme
            Assert.Throws<ValidationException>(() => attribute.Validate("foo.png", s_testValidationContext)); // no scheme
        }

        [Fact]
        public static void Validate_throws_InvalidOperationException_if_ErrorMessage_is_null()
        {
            var attribute = new UrlAttribute();
            attribute.ErrorMessage = null; // note: this overrides the default value
            Assert.Throws<InvalidOperationException>(() => attribute.Validate("foo.png", s_testValidationContext));
        }

        [Fact]
        public static void Validate_throws_InvalidOperationException_if_ErrorMessage_and_ErrorMessageResourceName_are_set()
        {
            var attribute = new UrlAttribute();
            attribute.ErrorMessage = "SomeErrorMessage";
            attribute.ErrorMessageResourceName = "SomeErrorMessageResourceName";
            Assert.Throws<InvalidOperationException>(() => attribute.Validate("foo.png", s_testValidationContext));
        }

        [Fact]
        public static void Validate_throws_InvalidOperationException_if_ErrorMessageResourceName_set_but_ErrorMessageResourceType_not_set()
        {
            var attribute = new UrlAttribute();
            attribute.ErrorMessageResourceName = "SomeErrorMessageResourceName";
            attribute.ErrorMessageResourceType = null;
            Assert.Throws<InvalidOperationException>(() => attribute.Validate("foo.png", s_testValidationContext));
        }

        [Fact]
        public static void Validate_throws_InvalidOperationException_if_ErrorMessageResourceType_set_but_ErrorMessageResourceName_not_set()
        {
            var attribute = new UrlAttribute();
            attribute.ErrorMessageResourceName = null;
            attribute.ErrorMessageResourceType = typeof(ErrorMessageResources);
            Assert.Throws<InvalidOperationException>(() => attribute.Validate("foo.png", s_testValidationContext));
        }

        [Fact]
        public static void GetValidationResult_returns_ErrorMessage_if_ErrorMessage_overrides_default()
        {
            var attribute = new UrlAttribute();
            attribute.ErrorMessage = "SomeErrorMessage";
            var toBeTested = new UrlClassToBeTested();
            var validationContext = new ValidationContext(toBeTested);
            validationContext.MemberName = "UrlPropertyToBeTested";
            var validationResult = attribute.GetValidationResult(toBeTested, validationContext);
            Assert.Equal("SomeErrorMessage", validationResult.ErrorMessage);
        }


        [Fact]
        public static void GetValidationResult_returns_DefaultErrorMessage_if_ErrorMessage_is_not_set()
        {
            var attribute = new UrlAttribute();
            var toBeTested = new UrlClassToBeTested();
            var validationContext = new ValidationContext(toBeTested);
            validationContext.MemberName = "UrlPropertyToBeTested";
            AssertEx.DoesNotThrow(() => attribute.GetValidationResult(toBeTested, validationContext));
        }

        [Fact]
        public static void GetValidationResultWithErrorMessageResourceNameAndType()
        {
            var attribute = new UrlAttribute();
            attribute.ErrorMessageResourceName = "InternalErrorMessageTestProperty";
            attribute.ErrorMessageResourceType = typeof(ErrorMessageResources);
            var toBeTested = new UrlClassToBeTested();
            var validationContext = new ValidationContext(toBeTested);
            validationContext.MemberName = "UrlPropertyToBeTested";
            var validationResult = attribute.GetValidationResult(toBeTested, validationContext);
            Assert.Equal(
                "Error Message from ErrorMessageResources.InternalErrorMessageTestProperty",
                validationResult.ErrorMessage);
        }
    }

    public class UrlClassToBeTested
    {
        public string UrlPropertyToBeTested
        {
            get { return "foo.png"; }
        }
    }
}
