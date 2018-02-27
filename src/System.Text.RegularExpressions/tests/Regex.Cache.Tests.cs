using System.Diagnostics;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    public class Regex_Cache_Tests : RemoteExecutorTestBase
    {
        [Fact]
        public void CacheSize_Get()
        {
            Assert.Equal(15, Regex.CacheSize);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(12)]
        public void CacheSize_Set(int newCacheSize)
        {
            int originalCacheSize = Regex.CacheSize;

            Regex.CacheSize = newCacheSize;
            Assert.Equal(newCacheSize, Regex.CacheSize);

            Regex.CacheSize = originalCacheSize;
        }

        [Fact]
        public void CacheSize_Set_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => Regex.CacheSize = -1);
        }

        [Fact]
        public void Ctor_Cache_Second_drops_first_does_not_throw()
        {
            int originalCacheSize = Regex.CacheSize;

            Regex.CacheSize = 1;
            Regex.IsMatch("1", "1");
            Regex.IsMatch("2", "2"); // previous removed from cache

            Regex.CacheSize = originalCacheSize;
        }

        [Fact]
        public void Ctor_Cache_Shrink_cache_does_not_throw()
        {
            int originalCacheSize = Regex.CacheSize;

            Regex.CacheSize = 2;
            Regex.IsMatch("1", "1");
            Regex.IsMatch("2", "2");
            Regex.CacheSize = 1;
            Regex.CacheSize = 0; // clear

            Regex.CacheSize = originalCacheSize;
        }

        [Fact]
        public void Ctor_Cache_Promote_entries_does_not_throw()
        {
            int originalCacheSize = Regex.CacheSize;

            Regex.CacheSize = 3;
            Regex.IsMatch("1", "1");
            Regex.IsMatch("2", "2"); 
            Regex.IsMatch("3", "3");
            Regex.IsMatch("1", "1"); // should be put first
            Regex.CacheSize = 1;  // only 1 stays

            Regex.CacheSize = originalCacheSize;
        }

    }
}
