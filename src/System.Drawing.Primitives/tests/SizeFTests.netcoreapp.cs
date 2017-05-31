// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Drawing.PrimitivesTest
{
    public partial class SizeFTests
    {
        #region SizeF * float tests

        private void MultSizeFFloatTester(float width, float height, float multiplier)
        {
            SizeF sz = new SizeF(width, height);
            SizeF mulExpected;

            mulExpected = new SizeF(width * multiplier, height * multiplier);

            Assert.Equal(mulExpected, sz * multiplier);
            Assert.Equal(mulExpected, multiplier * sz);
        }

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
            MultSizeFFloatTester(value1, 1.0f, value2);
            MultSizeFFloatTester(value2, 1.0f, value1);

            MultSizeFFloatTester(1.0f, value1, value2);
            MultSizeFFloatTester(1.0f, value2, value1);
        }

        #endregion

        #region SizeF / float tests

        private void DivideSizeFFloatTester(float width, float height, float divisor)
        {
            SizeF size = new SizeF(width, height);
            SizeF expected = new SizeF(width / divisor, height / divisor);
            Assert.Equal(expected, size / divisor);
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
        public void DivideTestSizeFFloat(float value1, float value2)
        {
            DivideSizeFFloatTester(value1, 1.0f, value2);
            DivideSizeFFloatTester(1.0f, value1, value2);
        }

        #endregion
    }
}
