using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class SkipTests
    {
        [Fact]
        public void SkipSome()
        {
            Assert.Equal(Enumerable.Range(10, 10), Enumerable.Range(0, 20).Skip(10));
        }

        [Fact]
        public void SkipExcessive()
        {
            Assert.Equal(Enumerable.Empty<int>(), Enumerable.Range(0, 20).Skip(42));
        }

        [Fact]
        public void SkipAllExactly()
        {
            Assert.False(Enumerable.Range(0, 20).Skip(20).Any());
        }

        [Fact]
        public void SkipThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<DateTime>)null).Skip(3));
        }

        [Fact]
        public void SkipNegative()
        {
            Assert.Equal(Enumerable.Range(0, 20), Enumerable.Range(0, 20).Skip(-42));
        }

        [Fact]
        public void SkipWhileAllTrue()
        {
            Assert.Equal(Enumerable.Empty<int>(), Enumerable.Range(0, 20).SkipWhile(i => i < 40));
            Assert.Equal(Enumerable.Empty<int>(), Enumerable.Range(0, 20).SkipWhile((i, idx) => i == idx));
        }

        [Fact]
        public void SkipWhileAllFalse()
        {
            Assert.Equal(Enumerable.Range(0, 20), Enumerable.Range(0, 20).SkipWhile(i => i != 0));
            Assert.Equal(Enumerable.Range(0, 20), Enumerable.Range(0, 20).SkipWhile((i, idx) => i != idx));
        }

        [Fact]
        public void SkipWhileThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).SkipWhile(i => i < 40));
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).SkipWhile((i, idx) => i == idx));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 20).SkipWhile((Func<int, int, bool>)null));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 20).SkipWhile((Func<int, bool>)null));
        }

        [Fact]
        public void SkipWhileHalf()
        {
            Assert.Equal(Enumerable.Range(10, 10), Enumerable.Range(0, 20).SkipWhile(i => i < 10));
            Assert.Equal(Enumerable.Range(10, 10), Enumerable.Range(0, 20).SkipWhile((i, idx) => idx < 10));
        }
    }
}
