using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace System.Linq.Tests
{
    public class SkipTests
    {
        private readonly ITestOutputHelper _output;

        public SkipTests(ITestOutputHelper output)
        {
            _output = output;
        }
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

        [Fact]
        public void SkipErrorWhenSourceErrors()
        {
            var source = Enumerable.Range(-2, 5).Select(i => (decimal)i).Select(m => 1 / m).Skip(4);
            using(var en = source.GetEnumerator())
            {
                Assert.True(
                    Assert.Throws<DivideByZeroException>(() => en.MoveNext())
                    // Check stack-trace still references the WhereSelectEnumerableIterator`2.MoveNext() call
                    // where it originated. Check will have to be changed if the details of `Select()`
                    // implementation change accordingly.
                    .StackTrace.Contains("at System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()"));
            }
        }
    }
}
