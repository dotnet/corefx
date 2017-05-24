// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Drawing.PrimitivesTest
{
    public partial class SizeFTests
    {
        [Theory]
        [InlineData(1000.234f, 2400.933f)]
        [InlineData(float.MaxValue, 0.0f)]
        [InlineData(float.MaxValue, 1.0f)]
        [InlineData(float.MaxValue, -1.0f)]
        [InlineData(float.MaxValue, float.MaxValue)]
        [InlineData(float.MinValue, 0.0f)]
        [InlineData(float.MinValue, 1.0f)]
        [InlineData(float.MinValue, -1.0f)]
        [InlineData(float.MinValue, float.MinValue)]
        [InlineData(float.MaxValue, float.MinValue)]
        [InlineData(float.MinValue, float.MaxValue)]
        public void MultiplicationTest(float value1, float value2)
        {
            SizeF sz1 = new SizeF(value1, value1);
            SizeF sz2 = new SizeF(value2, value2);
            SizeF mulExpected;

            unchecked
            {
                mulExpected = new SizeF(value1 * value2, value1 * value2);
            }

            Assert.Equal(mulExpected, sz1 * value2);
            Assert.Equal(mulExpected, value2 * sz1);
            Assert.Equal(mulExpected, sz2 * value1);
            Assert.Equal(mulExpected, value1 * sz2);
        }

        [Theory]
        [InlineData(0.0f, 1.0f)]
        [InlineData(1.0f, 1.0f)]
        [InlineData(-1.0f, 1.0f)]
        [InlineData(1.0f, -1.0f)]
        [InlineData(-1.0f, -1.0f)]
        [InlineData(float.MaxValue, float.MaxValue)]
        [InlineData(float.MaxValue, float.MinValue)]
        [InlineData(float.MinValue, float.MaxValue)]
        [InlineData(float.MinValue, float.MinValue)]
        [InlineData(float.MaxValue, 1.0f)]
        [InlineData(float.MinValue, 1.0f)]
        [InlineData(float.MaxValue, -1.0f)]
        [InlineData(float.MinValue, -1.0f)]
        [InlineData(float.MinValue, 0.0f)]
        [InlineData(1.0f, float.MinValue)]
        [InlineData(1.0f, float.MinValue)]
        [InlineData(-1.0f, float.MinValue)]
        [InlineData(-1.0f, float.MinValue)]
        public void DivideTestSizeFloat(float value1, float value2)
        {
            SizeF size = new SizeF(value1, value1);
            float invDiv = 1.0f / value2;
            SizeF expected = new SizeF(value1 * invDiv, value1 * invDiv);
            Assert.Equal(expected, size / value2);
        }

    }
}
