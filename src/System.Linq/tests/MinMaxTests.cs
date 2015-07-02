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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().Min());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().Max());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<long>().Min());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<long>().Max());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<float>().Min());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<float>().Max());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<double>().Min());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<double>().Max());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<decimal>().Min());
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
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<decimal>().Max());
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
            Assert.Equal(null, Enumerable.Empty<int?>().Min());
            Assert.Equal(null, Enumerable.Repeat(default(int?), 100).Min());
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
            Assert.Equal(null, Enumerable.Empty<int?>().Max());
            Assert.Equal(null, Enumerable.Repeat(default(int?), 100).Max());
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
            Assert.Equal(null, Enumerable.Empty<long?>().Min());
            Assert.Equal(null, Enumerable.Repeat(default(long?), 100).Min());
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
            Assert.Equal(null, Enumerable.Empty<long?>().Max());
            Assert.Equal(null, Enumerable.Repeat(default(long?), 100).Max());
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
            Assert.Equal(null, Enumerable.Empty<float?>().Min());
            Assert.Equal(null, Enumerable.Repeat(default(float?), 100).Min());
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
            Assert.Equal(null, Enumerable.Empty<float?>().Max());
            Assert.Equal(null, Enumerable.Repeat(default(float?), 100).Max());
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
            Assert.Equal(null, Enumerable.Empty<double?>().Min());
            Assert.Equal(null, Enumerable.Repeat(default(double?), 100).Min());
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
            Assert.Equal(null, Enumerable.Empty<double?>().Max());
            Assert.Equal(null, Enumerable.Repeat(default(double?), 100).Max());
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
            Assert.Equal(null, Enumerable.Empty<decimal?>().Min());
            Assert.Equal(null, Enumerable.Repeat(default(decimal?), 100).Min());
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
            Assert.Equal(null, Enumerable.Empty<decimal?>().Max());
            Assert.Equal(null, Enumerable.Repeat(default(decimal?), 100).Max());
        }

        // Normally NaN < anything is false, as is anything < NaN
        // However, this leads to some irksome outcomes in Min and Max.
        // If we use those semantics then Min(NaN, 5.0) is NaN, but
        // Min(5.0, NaN) is 5.0!  To fix this, we impose a total
        // ordering where NaN is smaller than every value, including
        // negative infinity.
        // This behaviour must be confirmed as happening.
        // Note that further to this, null is taken as not being in a sequence at all
        // (Null will be returned if the only element in a sequence, but this will also
        // happen with an empty sequence).

        [Fact]
        public void NaNFirstSingle()
        {
            var nanThenOne = Enumerable.Range(1, 10).Select(i => (float)i).Concat(Enumerable.Repeat(float.NaN, 1)).ToArray();
            var nanThenMinusTen = new[] { -1F, -10, float.NaN, 10, 200, 1000 };
            var nanThenMinValue = new[] { float.MinValue, 3000F, 100, 200, float.NaN, 1000 };
            Assert.True(float.IsNaN(nanThenOne.Min()));
            Assert.True(float.IsNaN(nanThenMinusTen.Min()));
            Assert.True(float.IsNaN(nanThenMinValue.Min()));
            Assert.False(float.IsNaN(nanThenOne.Max()));
            Assert.False(float.IsNaN(nanThenMinusTen.Max()));
            Assert.False(float.IsNaN(nanThenMinValue.Max()));
            var nanWithNull = new[] { default(float?), float.NaN, default(float?) };
            Assert.True(float.IsNaN(nanWithNull.Min().Value));
            Assert.True(float.IsNaN(nanWithNull.Max().Value));
        }

        [Fact]
        public void NaNFirstDouble()
        {
            var nanThenOne = Enumerable.Range(1, 10).Select(i => (double)i).Concat(Enumerable.Repeat(double.NaN, 1)).ToArray();
            var nanThenMinusTen = new[] { -1F, -10, double.NaN, 10, 200, 1000 };
            var nanThenMinValue = new[] { double.MinValue, 3000F, 100, 200, double.NaN, 1000 };
            Assert.True(double.IsNaN(nanThenOne.Min()));
            Assert.True(double.IsNaN(nanThenMinusTen.Min()));
            Assert.True(double.IsNaN(nanThenMinValue.Min()));
            Assert.False(double.IsNaN(nanThenOne.Max()));
            Assert.False(double.IsNaN(nanThenMinusTen.Max()));
            Assert.False(double.IsNaN(nanThenMinValue.Max()));
            var nanWithNull = new[] { default(double?), double.NaN, default(double?) };
            Assert.True(double.IsNaN(nanWithNull.Min().Value));
            Assert.True(double.IsNaN(nanWithNull.Max().Value));
        }
    }
}