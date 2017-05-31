// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Drawing.PrimitivesTests
{
    public partial class SizeTests
    {
        #region Size * int tests

        private void MultSizeIntTester(int width, int height, int multiplier)
        {
            Size sz = new Size(width, height);
            Size expected;

            unchecked
            {
                expected = new Size(width * multiplier, height * multiplier);
            }
            Assert.Equal(expected, sz * multiplier);
            Assert.Equal(expected, multiplier * sz);
        }

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
        public void MultTestSizeInt(int value1, int value2)
        {
            MultSizeIntTester(value1, 1, value2);
            MultSizeIntTester(value2, 1, value1);

            MultSizeIntTester(1, value1, value2);
            MultSizeIntTester(1, value2, value1);
        }

        #endregion

        #region Size * float tests

        private void MultSizeFloatTester(int width, int height, float multiplier)
        {
            Size sz = new Size(width, height);
            SizeF expected;

            unchecked
            {
                expected = new SizeF(width * multiplier, height * multiplier);
            }
            Assert.Equal(expected, sz * multiplier);
            Assert.Equal(expected, multiplier * sz);
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
            MultSizeFloatTester(value1, 1, value2);
            MultSizeFloatTester(1, value1, value2);
        }

        #endregion

        #region Divide by Zero

        [Fact]
        public void DivideByZeroChecks()
        {
            Size size = new Size(100, 100);
            Assert.Throws<DivideByZeroException>(() => size / 0);

            SizeF expectedSizeF = new SizeF(float.PositiveInfinity, float.PositiveInfinity);
            Assert.Equal(expectedSizeF, size / 0.0f);
        }

        #endregion

        #region Size / int tests

        private void DivideSizeIntTester(int width, int height, int divisor)
        {
            Size size = new Size(width, height);
            Size expected;

            expected = new Size(width / divisor, height / divisor);

            Assert.Equal(expected, size / divisor);
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
            DivideSizeIntTester(value1, 1, value2);
            DivideSizeIntTester(1, value1, value2);
        }

        #endregion

        #region Size / float tests

        private void DivideSizeFloatTester(int width, int height, float divisor)
        {
            Size size = new Size(width, height);
            SizeF expected;

            expected = new SizeF(width / divisor, height / divisor);
            Assert.Equal(expected, size / divisor);
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
            DivideSizeFloatTester(value1, 1, value2);
            DivideSizeFloatTester(1, value1, value2);
        }

        #endregion
    }
}
