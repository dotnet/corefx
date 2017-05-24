// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Drawing.PrimitivesTests
{
    public partial class SizeTests
    {
        [Theory]
        [InlineData(1000, 2400)]
        [InlineData(int.MaxValue, 0)]
        [InlineData(int.MaxValue, 1)]
        [InlineData(int.MaxValue, 2)]
        [InlineData(int.MaxValue, -1)]
        [InlineData(int.MaxValue, -2)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(int.MinValue, 0)]
        [InlineData(int.MinValue, 1)]
        [InlineData(int.MinValue, 2)]
        [InlineData(int.MinValue, -1)]
        [InlineData(int.MinValue, -2)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MinValue)]
        public void MultiplicationTestSizeInt(int value1, int value2)
        {
            Size sz1 = new Size(value1, value1);
            Size sz2 = new Size(value2, value2);
            Size mulExpected;

            unchecked
            {
                mulExpected = new Size(value1 * value2, value1 * value2);
            }

            Assert.Equal(mulExpected, sz1 * value2);
            Assert.Equal(mulExpected, value2 * sz1);
            Assert.Equal(mulExpected, sz2 * value1);
            Assert.Equal(mulExpected, value1 * sz2);
        }


        [Theory]
        [InlineData(1000, 2400.933f)]
        [InlineData(int.MaxValue, 0.0f)]
        [InlineData(int.MaxValue, 1.0f)]
        [InlineData(int.MaxValue, -1.0f)]
        [InlineData(int.MaxValue, float.MaxValue)]
        [InlineData(int.MinValue, 0.0f)]
        [InlineData(int.MinValue, 1.0f)]
        [InlineData(int.MinValue, -1.0f)]
        [InlineData(int.MinValue, float.MinValue)]
        [InlineData(int.MaxValue, float.MinValue)]
        [InlineData(int.MinValue, float.MaxValue)]
        public void MultiplicationTestSizeFloat(int value1, float value2)
        {
            Size sz1 = new Size(value1, value1);
            SizeF mulExpected;

            unchecked
            {
                mulExpected = new SizeF(value1 * value2, value1 * value2);
            }

            Assert.Equal(mulExpected, sz1 * value2);
            Assert.Equal(mulExpected, value2 * sz1);
        }

        [Fact]
        public void DivideByZeroChecks()
        {
            Size size = new Size(100, 100);
            Assert.Throws<DivideByZeroException>(() => size / 0);

            float invDiv = 1 / 0.0f;
            SizeF expectedSizeF = new SizeF(float.PositiveInfinity, float.PositiveInfinity);
            Assert.Equal(expectedSizeF, size * invDiv);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MaxValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, 1)]
        [InlineData(int.MinValue, 1)]
        [InlineData(int.MaxValue, -1)]
        public void DivideTestSizeInt(int value1, int value2)
        {
            Size size = new Size(value1, value1);
            Size expected;

            int invDiv = 1 / value2;
            expected = new Size(value1 * invDiv, value1 * invDiv);

            Assert.Equal(expected, size / value2);
        }

        [Theory]
        [InlineData(0, 1.0f)]
        [InlineData(1, 1.0f)]
        [InlineData(-1, 1.0f)]
        [InlineData(1, -1.0f)]
        [InlineData(-1, -1.0f)]
        [InlineData(int.MaxValue, float.MaxValue)]
        [InlineData(int.MaxValue, float.MinValue)]
        [InlineData(int.MinValue, float.MaxValue)]
        [InlineData(int.MinValue, float.MinValue)]
        [InlineData(int.MaxValue, 1.0f)]
        [InlineData(int.MinValue, 1.0f)]
        [InlineData(int.MaxValue, -1.0f)]
        [InlineData(int.MinValue, -1.0f)]
        public void DivideTestSizeFloat(int value1, float value2)
        {
            SizeF size = new SizeF(value1, value1);
            SizeF expected;

            float invDiv = 1.0f / value2;

            expected = new SizeF(value1 * invDiv, value1 * invDiv);
            Assert.Equal(expected, size / value2);
        }

    }
}
