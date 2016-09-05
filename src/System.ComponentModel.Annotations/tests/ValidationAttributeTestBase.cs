// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.ComponentModel.DataAnnotations.Tests
{
    public abstract class ValidationAttributeTestBase
    {
        public abstract IEnumerable<Test> ValidValues();
        public abstract IEnumerable<Test> InvalidValues();

        public virtual bool RespectsErrorMessage => true;
        public Type InvalidErrorMessage_Type => RespectsErrorMessage ? typeof(InvalidOperationException) : typeof(ValidationException);

        [Theory]
        public void Validate()
        {
            foreach (Test testCase in ValidValues())
            {
                Validate(testCase.Attribute, testCase.Value, testCase.ValidationContext, true);
            }
            foreach (Test testCase in InvalidValues())
            {
                Validate(testCase.Attribute, testCase.Value, testCase.ValidationContext, false);
            }
        }

        public void Validate(ValidationAttribute attribute, object value, ValidationContext validationContext, bool isValid)
        {
            if (isValid)
            {
                attribute.Validate(value, validationContext);
                Assert.Equal(ValidationResult.Success, attribute.GetValidationResult(value, validationContext));
            }
            else
            {
                Assert.Throws<ValidationException>(() => attribute.Validate(value, validationContext));
                Assert.NotNull(attribute.GetValidationResult(value, validationContext));
            }
            Assert.Equal(isValid, attribute.IsValid(value));
        }

        [Fact]
        public void ErrorMessage_Invalid_Throws()
        {
            if (InvalidValues().Count() == 0)
            {
                return;
            }
            ErrorMessageNotSet(InvalidValues().First());
            ErrorMessageSet_ErrorMessageResourceNameSet(InvalidValues().First());
            ErrorMessageResourceNameSet_ErrorMessageResourceTypeNotSet(InvalidValues().First());
            ErrorMessageResourceNameNotSet_ErrorMessageResourceTypeSet(InvalidValues().First());
        }

        [Fact]
        public void ErrorMessage_Valid()
        {
            if (!RespectsErrorMessage || InvalidValues().Count() == 0)
            {
                return;
            }
            ErrorMessageSet_ReturnsOverridenValue(InvalidValues().First());
            ErrorMessageNotSet_ReturnsDefaultValue(InvalidValues().First());
            ErrorMessageSetFromResource_ReturnsExpectedValue(InvalidValues().First());
        }
        
        public void ErrorMessageNotSet(Test test)
        {
            test.Attribute.ErrorMessage = null;
            Assert.Throws(InvalidErrorMessage_Type, () => test.Attribute.Validate(test.Value, test.ValidationContext));
        }
        
        public void ErrorMessageSet_ErrorMessageResourceNameSet(Test test)
        {
            test.Attribute.ErrorMessage = "Some";
            test.Attribute.ErrorMessageResourceName = "Some";
            Assert.Throws(InvalidErrorMessage_Type, () => test.Attribute.Validate(test.Value, test.ValidationContext));
        }
        
        public void ErrorMessageResourceNameSet_ErrorMessageResourceTypeNotSet(Test test)
        {
            test.Attribute.ErrorMessageResourceName = "Some";
            test.Attribute.ErrorMessageResourceType = null;
            Assert.Throws(InvalidErrorMessage_Type, () => test.Attribute.Validate(test.Value, test.ValidationContext));
        }
        
        public void ErrorMessageResourceNameNotSet_ErrorMessageResourceTypeSet(Test test)
        {
            test.Attribute.ErrorMessageResourceName = null;
            test.Attribute.ErrorMessageResourceType = typeof(ErrorMessageResources);
            Assert.Throws(InvalidErrorMessage_Type, () => test.Attribute.Validate(test.Value, test.ValidationContext));
        }
        
        public void ErrorMessageSet_ReturnsOverridenValue(Test test)
        {
            test.Attribute.ErrorMessage = "SomeErrorMessage";

            var validationResult = test.Attribute.GetValidationResult(test.Value, test.ValidationContext);
            Assert.Equal("SomeErrorMessage", validationResult.ErrorMessage);
        }
        
        public void ErrorMessageNotSet_ReturnsDefaultValue(Test test)
        {
            test.Attribute.GetValidationResult(test.Value, test.ValidationContext);
        }
        
        public void ErrorMessageSetFromResource_ReturnsExpectedValue(Test test)
        {
            test.Attribute.ErrorMessageResourceName = "InternalErrorMessageTestProperty";
            test.Attribute.ErrorMessageResourceType = typeof(ErrorMessageResources);

            var validationResult = test.Attribute.GetValidationResult(test.Value, test.ValidationContext);
            Assert.Equal("Error Message from ErrorMessageResources.InternalErrorMessageTestProperty", validationResult.ErrorMessage);
        }
    }

    public class Test
    {
        public ValidationAttribute Attribute { get; }
        public object Value { get; }
        public ValidationContext ValidationContext { get; }

        public Test(ValidationAttribute attribute, object value, ValidationContext validationContext = null)
        {
            Attribute = attribute;
            Value = value;
            ValidationContext = validationContext ?? new ValidationContext(new object());
        }
    }
}
