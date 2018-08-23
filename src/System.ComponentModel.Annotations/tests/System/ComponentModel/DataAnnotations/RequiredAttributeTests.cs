// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.DataAnnotations.Tests
{
    public class RequiredAttributeTests : ValidationAttributeTestBase
    {
        protected override IEnumerable<TestCase> ValidValues()
        {
            yield return new TestCase(new RequiredAttribute(), "SomeString");
            yield return new TestCase(new RequiredAttribute() { AllowEmptyStrings = true }, string.Empty);
            yield return new TestCase(new RequiredAttribute() { AllowEmptyStrings = true }, " \t \r \n ");
            yield return new TestCase(new RequiredAttribute(), new object());
            yield return new TestCase(new RequiredAttribute(), new List<string>() { "SomeString" });
            yield return new TestCase(new RequiredAttribute() { AllowEmptyCollections = true }, new List<string>());
        }

        protected override IEnumerable<TestCase> InvalidValues()
        {
            yield return new TestCase(new RequiredAttribute(), null);
            yield return new TestCase(new RequiredAttribute() { AllowEmptyStrings = false }, string.Empty);
            yield return new TestCase(new RequiredAttribute() { AllowEmptyStrings = false }, " \t \r \n ");
            yield return new TestCase(new RequiredAttribute() { AllowEmptyCollections = false }, new List<string>());
            yield return new TestCase(new RequiredAttribute(), new List<string>());
        }

        [Fact]
        public static void AllowEmptyStrings_GetSet_ReturnsExpectected()
        {
            var attribute = new RequiredAttribute();
            Assert.False(attribute.AllowEmptyStrings);
            Assert.False(attribute.AllowEmptyCollections);

            attribute.AllowEmptyStrings = true;
            Assert.True(attribute.AllowEmptyStrings);
            Assert.False(attribute.AllowEmptyCollections);

            attribute.AllowEmptyStrings = false;
            Assert.False(attribute.AllowEmptyStrings);
            Assert.False(attribute.AllowEmptyCollections);

            attribute.AllowEmptyCollections = true;
            Assert.False(attribute.AllowEmptyStrings);
            Assert.True(attribute.AllowEmptyCollections);

            attribute.AllowEmptyCollections = false;
            Assert.False(attribute.AllowEmptyStrings);
            Assert.False(attribute.AllowEmptyCollections);

            attribute.AllowEmptyCollections = true;
            attribute.AllowEmptyStrings = true;
            Assert.True(attribute.AllowEmptyStrings);
            Assert.True(attribute.AllowEmptyCollections);
        }
    }
}
