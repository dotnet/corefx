using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class MinMaxTests
    {
        [Fact]
        public void MinInt()
        {
            var one = Enumerable.Range(1, 10).ToArray();
            var minusTen = new[] { -1, -10, 10, 200, 1000 };
            var hundred = new[] { 3000, 100, 200, 1000 };
            Assert.Equal(1, one.Min());
            Assert.Equal(-10, minusTen.Min());
            Assert.Equal(100, hundred.Min());
        }

        [Fact]
        public void MaxInt()
        {
            var ten = Enumerable.Range(1, 10).ToArray();
            var minusTen = new[] { -100, -15, -50, -10 };
            var thousand = new[] { -16, 0, 50, 100, 1000 };
            Assert.Equal(10, ten.Max());
            Assert.Equal(-10, minusTen.Max());
            Assert.Equal(1000, thousand.Max());
        }

        [Fact]
        public void MinLong()
        {
            var one = Enumerable.Range(1, 10).Select(i => (long)i).ToArray();
            var minusTen = new[] { -1L, -10, 10, 200, 1000 };
            var hundred = new[] { 3000L, 100, 200, 1000 };
            Assert.Equal(1, one.Min());
            Assert.Equal(-10, minusTen.Min());
            Assert.Equal(100, hundred.Min());
        }

        [Fact]
        public void MaxLong()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (long)i).ToArray();
            var minusTen = new[] { -100L, -15, -50, -10 };
            var thousand = new[] { -16L, 0, 50, 100, 1000 };
            Assert.Equal(10, ten.Max());
            Assert.Equal(-10, minusTen.Max());
            Assert.Equal(1000, thousand.Max());
        }

        [Fact]
        public void MinSingle()
        {
            var one = Enumerable.Range(1, 10).Select(i => (float)i).ToArray();
            var minusTen = new[] { -1F, -10, 10, 200, 1000 };
            var hundred = new[] { 3000F, 100, 200, 1000 };
            Assert.Equal(1L, one.Min());
            Assert.Equal(-10L, minusTen.Min());
            Assert.Equal(100L, hundred.Min());
        }

        [Fact]
        public void MaxSingle()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (float)i).ToArray();
            var minusTen = new[] { -100F, -15, -50, -10 };
            var thousand = new[] { -16F, 0, 50, 100, 1000 };
            Assert.Equal(10F, ten.Max());
            Assert.Equal(-10F, minusTen.Max());
            Assert.Equal(1000F, thousand.Max());
        }

        [Fact]
        public void MinDouble()
        {
            var one = Enumerable.Range(1, 10).Select(i => (double)i).ToArray();
            var minusTen = new[] { -1D, -10, 10, 200, 1000 };
            var hundred = new[] { 3000D, 100, 200, 1000 };
            Assert.Equal(1D, one.Min());
            Assert.Equal(-10D, minusTen.Min());
            Assert.Equal(100D, hundred.Min());
        }

        [Fact]
        public void MaxDouble()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (double)i).ToArray();
            var minusTen = new[] { -100D, -15, -50, -10 };
            var thousand = new[] { -16D, 0, 50, 100, 1000 };
            Assert.Equal(10D, ten.Max());
            Assert.Equal(-10D, minusTen.Max());
            Assert.Equal(1000D, thousand.Max());
        }

        [Fact]
        public void MinDecimal()
        {
            var one = Enumerable.Range(1, 10).Select(i => (decimal)i).ToArray();
            var minusTen = new[] { -1M, -10, 10, 200, 1000 };
            var hundred = new[] { 3000M, 100, 200, 1000 };
            Assert.Equal(1M, one.Min());
            Assert.Equal(-10M, minusTen.Min());
            Assert.Equal(100M, hundred.Min());
        }

        [Fact]
        public void MaxDecimal()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (decimal)i).ToArray();
            var minusTen = new[] { -100M, -15, -50, -10 };
            var thousand = new[] { -16M, 0, 50, 100, 1000 };
            Assert.Equal(10M, ten.Max());
            Assert.Equal(-10M, minusTen.Max());
            Assert.Equal(1000M, thousand.Max());
        }
    }
}
