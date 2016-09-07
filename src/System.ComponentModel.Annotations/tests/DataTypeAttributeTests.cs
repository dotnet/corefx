// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.ComponentModel.DataAnnotations.Tests
{
    public class DataTypeAttributeTests : ValidationAttributeTestBase
    {
        protected override IEnumerable<TestCase> ValidValues()
        {
            foreach (DataType dataType in s_dataTypes)
            {
                if (dataType != DataType.Custom)
                {
                    yield return new TestCase(new DataTypeAttribute(dataType), new object());
                }
            }
            yield return new TestCase(new DataTypeAttribute("CustomDataType"), new object());
            yield return new TestCase(new DataTypeAttribute((DataType)(-1)), new object());
            yield return new TestCase(new DataTypeAttribute(DataType.Upload + 1), new object());
        }

        protected override IEnumerable<TestCase> InvalidValues() => new TestCase[0];

        private static readonly ValidationContext s_testValidationContext = new ValidationContext(new object());

        private static readonly DataType[] s_dataTypes = (DataType[])Enum.GetValues(typeof(DataType));
        public static IEnumerable<object[]> DataTypes_TestData => s_dataTypes.Select(type => new object[] { type });

        [Theory]
        [MemberData(nameof(DataTypes_TestData))]
        [InlineData((DataType)(-1))]
        [InlineData(DataType.Upload + 1)]
        public static void Ctor_DataType(DataType dataType)
        {
            DataTypeAttribute attribute = new DataTypeAttribute(dataType);
            Assert.Equal(dataType, attribute.DataType);
            Assert.Null(attribute.CustomDataType);

            bool expectedNull = dataType != DataType.Date && dataType != DataType.Time && dataType != DataType.Currency;
            Assert.Equal(expectedNull, attribute.DisplayFormat == null);
        }

        [Theory]
        [MemberData(nameof(DataTypes_TestData))]
        public static void GetDataTypeName_ReturnsExpectedName(DataType dataType)
        {
            if (dataType != DataType.Custom)
            {
                DataTypeAttribute attribute = new DataTypeAttribute(dataType);
                Assert.Equal(Enum.GetName(typeof(DataType), dataType), attribute.GetDataTypeName());
            }
        }

        [Theory]
        [InlineData((DataType)(-1))]
        [InlineData(DataType.Upload + 1)]
        public static void GetDataTypeName_InvalidDataType_ThrowsIndexOutOfRangeException(DataType dataType)
        {
            DataTypeAttribute attribute = new DataTypeAttribute(dataType);
            Assert.Throws<IndexOutOfRangeException>(() => attribute.GetDataTypeName());
        }

        [Theory]
        [InlineData("CustomValue")]
        [InlineData("")]
        [InlineData(null)]
        public static void Ctor_String(string customDataType)
        {
            DataTypeAttribute attribute = new DataTypeAttribute(customDataType);
            Assert.Equal(DataType.Custom, attribute.DataType);
            Assert.Equal(customDataType, attribute.CustomDataType);

            if (string.IsNullOrEmpty(customDataType))
            {
                Assert.Throws<InvalidOperationException>(() => attribute.GetDataTypeName());
                Assert.Throws<InvalidOperationException>(() => attribute.Validate(new object(), s_testValidationContext));
            }
            else
            {
                Assert.Equal(customDataType, attribute.GetDataTypeName());
            }
        }

        [Theory]
        [InlineData(DataType.Date, "{0:d}", true)]
        [InlineData(DataType.Time, "{0:t}", true)]
        [InlineData(DataType.Currency, "{0:C}", false)]
        public static void DisplayFormat_ReturnsExpected(DataType dataType, string dataFormatString, bool applyFormatInEditMode)
        {
            DataTypeAttribute attribute = new DataTypeAttribute(dataType);
            Assert.Equal(dataFormatString, attribute.DisplayFormat.DataFormatString);
            Assert.Equal(applyFormatInEditMode, attribute.DisplayFormat.ApplyFormatInEditMode);
        }
    }
}
