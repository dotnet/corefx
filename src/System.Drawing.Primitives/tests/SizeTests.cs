// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Xunit;

namespace System.Drawing.PrimitivesTests
{
    public class SizeTests
    {
        [Fact]
        public void DefaultConstructorTest()
        {
            Assert.Equal(Size.Empty, new Size());
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(0, 0)]
        public void NonDefaultConstructorTest(int width, int height)
        {
            Size s1 = new Size(width, height);
            Size s2 = new Size(new Point(width, height));

            Assert.Equal(s1, s2);
        }

        [Fact]
        public void IsEmptyDefaultsTest()
        {
            Assert.True(Size.Empty.IsEmpty);
            Assert.True(new Size().IsEmpty);
            Assert.True(new Size(0, 0).IsEmpty);
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public void IsEmptyRandomTest(int width, int height)
        {
            Assert.False(new Size(width, height).IsEmpty);
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(0, 0)]
        public void DimensionsTest(int width, int height)
        {
            Size p = new Size(width, height);
            Assert.Equal(width, p.Width);
            Assert.Equal(height, p.Height);
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(0, 0)]
        public void PointFConversionTest(int width, int height)
        {
            SizeF sz = new Size(width, height);
            Assert.Equal(new SizeF(width, height), sz);
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(0, 0)]
        public void SizeConversionTest(int width, int height)
        {
            Point sz = (Point)new Size(width, height);
            Assert.Equal(new Point(width, height), sz);
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(0, 0)]
        public void ArithmeticTest(int width, int height)
        {
            Size sz1 = new Size(width, height);
            Size sz2 = new Size(height, width);

            Size addExpected = new Size(width + height, height + width);
            Size subExpected = new Size(width - height, height - width);

            Assert.Equal(addExpected, sz1 + sz2);
            Assert.Equal(subExpected, sz1 - sz2);
            Assert.Equal(addExpected, Size.Add(sz1, sz2));
            Assert.Equal(subExpected, Size.Subtract(sz1, sz2));
        }

        [Theory]
        [InlineData(float.MaxValue, float.MinValue)]
        [InlineData(float.MinValue, float.MinValue)]
        [InlineData(float.MaxValue, float.MaxValue)]
        [InlineData(0, 0)]
        public void PointFMathematicalTest(float width, float height)
        {
            SizeF szF = new SizeF(width, height);
            Size pCeiling = new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height));
            Size pTruncate = new Size((int)width, (int)height);
            Size pRound = new Size((int)Math.Round(width), (int)Math.Round(height));

            Assert.Equal(pCeiling, Size.Ceiling(szF));
            Assert.Equal(pRound, Size.Round(szF));
            Assert.Equal(pTruncate, Size.Truncate(szF));
        }

        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(0, 0)]
        public void EqualityTest(int width, int height)
        {
            Size p1 = new Size(width, height);
            Size p2 = new Size(width - 1, height - 1);
            Size p3 = new Size(width, height);

            Assert.True(p1 == p3);
            Assert.True(p1 != p2);
            Assert.True(p2 != p3);

            Assert.True(p1.Equals(p3));
            Assert.False(p1.Equals(p2));
            Assert.False(p2.Equals(p3));

            Assert.Equal(p1.GetHashCode(), p3.GetHashCode());
        }

        [Fact]
        public void ToStringTest()
        {
            Size sz = new Size(0, 0);
            Assert.Equal(string.Format(CultureInfo.CurrentCulture, "{{Width={0}, Height={1}}}", sz.Width, sz.Height), sz.ToString());
        }
    }
}
