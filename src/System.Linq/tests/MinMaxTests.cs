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

        [Fact]
        public void MinNullableInt()
        {
            var one = Enumerable.Range(1, 10).Select(i => (int?)i).ToArray();
            var minusTen = new[] { default(int?), -1, -10, 10, 200, 1000 };
            var hundred = new[] { default(int?), 3000, 100, 200, 1000 };
            Assert.Equal(1, one.Min());
            Assert.Equal(-10, minusTen.Min());
            Assert.Equal(100, hundred.Min());
        }

        [Fact]
        public void MaxNullableInt()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (int?)i).ToArray();
            var minusTen = new[] { default(int?), -100, -15, -50, -10 };
            var thousand = new[] { default(int?), -16, 0, 50, 100, 1000 };
            Assert.Equal(10, ten.Max());
            Assert.Equal(-10, minusTen.Max());
            Assert.Equal(1000, thousand.Max());
        }

        [Fact]
        public void MinNullableLong()
        {
            var one = Enumerable.Range(1, 10).Select(i => (long?)i).ToArray();
            var minusTen = new[] { default(long?), -1L, -10, 10, 200, 1000 };
            var hundred = new[] { default(long?), 3000L, 100, 200, 1000 };
            Assert.Equal(1, one.Min());
            Assert.Equal(-10, minusTen.Min());
            Assert.Equal(100, hundred.Min());
        }

        [Fact]
        public void MaxNullableLong()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (long?)i).ToArray();
            var minusTen = new[] { default(long?), -100L, -15, -50, -10 };
            var thousand = new[] { default(long?), -16L, 0, 50, 100, 1000 };
            Assert.Equal(10, ten.Max());
            Assert.Equal(-10, minusTen.Max());
            Assert.Equal(1000, thousand.Max());
        }

        [Fact]
        public void MinNullableSingle()
        {
            var one = Enumerable.Range(1, 10).Select(i => (float?)i).ToArray();
            var minusTen = new[] { default(float?), -1F, -10, 10, 200, 1000 };
            var hundred = new[] { default(float?), 3000F, 100, 200, 1000 };
            Assert.Equal(1L, one.Min());
            Assert.Equal(-10L, minusTen.Min());
            Assert.Equal(100L, hundred.Min());
        }

        [Fact]
        public void MaxNullableSingle()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (float?)i).ToArray();
            var minusTen = new[] { default(float?), -100F, -15, -50, -10 };
            var thousand = new[] { default(float?), -16F, 0, 50, 100, 1000 };
            Assert.Equal(10F, ten.Max());
            Assert.Equal(-10F, minusTen.Max());
            Assert.Equal(1000F, thousand.Max());
        }

        [Fact]
        public void MinNullableDouble()
        {
            var one = Enumerable.Range(1, 10).Select(i => (double?)i).ToArray();
            var minusTen = new[] { default(double?), -1D, -10, 10, 200, 1000 };
            var hundred = new[] { default(double?), 3000D, 100, 200, 1000 };
            Assert.Equal(1D, one.Min());
            Assert.Equal(-10D, minusTen.Min());
            Assert.Equal(100D, hundred.Min());
        }

        [Fact]
        public void MaxNullableDouble()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (double?)i).ToArray();
            var minusTen = new[] { default(double?), -100D, -15, -50, -10 };
            var thousand = new[] { default(double?), -16D, 0, 50, 100, 1000 };
            Assert.Equal(10D, ten.Max());
            Assert.Equal(-10D, minusTen.Max());
            Assert.Equal(1000D, thousand.Max());
        }

        [Fact]
        public void MinNullableDecimal()
        {
            var one = Enumerable.Range(1, 10).Select(i => (decimal?)i).ToArray();
            var minusTen = new[] { default(decimal?), -1M, -10, 10, 200, 1000 };
            var hundred = new[] { default(decimal?), 3000M, 100, 200, 1000 };
            Assert.Equal(1M, one.Min());
            Assert.Equal(-10M, minusTen.Min());
            Assert.Equal(100M, hundred.Min());
        }

        [Fact]
        public void MaxNullableDecimal()
        {
            var ten = Enumerable.Range(1, 10).Select(i => (decimal?)i).ToArray();
            var minusTen = new[] { default(decimal?), -100M, -15, -50, -10 };
            var thousand = new[] { default(decimal?), -16M, 0, 50, 100, 1000 };
            Assert.Equal(10M, ten.Max());
            Assert.Equal(-10M, minusTen.Max());
            Assert.Equal(1000M, thousand.Max());
        }
    }
}
