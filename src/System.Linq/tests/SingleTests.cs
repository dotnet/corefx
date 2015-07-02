using System.Linq.Tests.Helpers;
using Xunit;

namespace System.Linq.Tests
{
    public class SingleTests
    {
        [Fact]
        public void FailOnEmpty()
        {
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().Single());
        }

        [Fact]
        public void DefaultOnEmpty()
        {
            Assert.Equal(0, Enumerable.Empty<int>().SingleOrDefault());
        }

        // Warning: At time of writing these two tests, they will incorrectly run for about five millennia.
        // They'll then probably throw OverflowException rather than the correct InvalidOperationException,
        // but I haven't had time to confirm. Recommend to skip if using without fix applied in commit 3f17916

        [Fact]
        public void FailOnManyMatches()
        {
            Assert.Throws<InvalidOperationException>(() => Enumerable.Range(0, int.MaxValue).Single(i => i > 0));
        }

        [Fact]
        public void DefaultFailOnManyMatches()
        {
            Assert.Throws<InvalidOperationException>(() => Enumerable.Range(0, int.MaxValue).SingleOrDefault(i => i > 0));
        }

        [Fact]
        public void FindSingle()
        {
            Assert.Equal(42, Enumerable.Repeat(42, 1).Single());
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(42, 100)]
        public void FindSingleMatch(int target, int range)
        {
            Assert.Equal(target, Enumerable.Range(0, range).Single(i => i == target));
        }
    }
}