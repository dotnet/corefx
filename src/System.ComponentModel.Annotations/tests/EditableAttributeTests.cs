// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.DataAnnotations
{
    public class EditableAttributeTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_construct_and_both_AllowEdit_and_AllowInitialValue_are_set(bool value)
        {
            var attribute = new EditableAttribute(value);
            Assert.Equal(value, attribute.AllowEdit);
            Assert.Equal(value, attribute.AllowInitialValue);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Properties_are_independent(bool value)
        {
            var attribute = new EditableAttribute(value);
            Assert.Equal(value, attribute.AllowEdit);
            Assert.Equal(value, attribute.AllowInitialValue);

            attribute.AllowInitialValue = !value;
            Assert.Equal(value, attribute.AllowEdit);
            Assert.Equal(!value, attribute.AllowInitialValue);
        }
    }
}
