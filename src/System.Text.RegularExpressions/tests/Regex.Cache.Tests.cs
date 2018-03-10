using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    public class Regex_Cache_Tests : RemoteExecutorTestBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(12)]
        public void CacheSize_Set(int newCacheSize)
        {
            int originalCacheSize = Regex.CacheSize;

            try
            {
                Regex.CacheSize = newCacheSize;
                Assert.Equal(newCacheSize, Regex.CacheSize);
            }
            finally
            {
                Regex.CacheSize = originalCacheSize;
            }
        }

        [Fact]
        public void CacheSize_Set_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => Regex.CacheSize = -1);
        }

        [Fact]
        public void Ctor_Cache_Second_drops_first()
        {
            RemoteInvoke(() =>
            {
                Regex.CacheSize = 1;
                Assert.True(Regex.IsMatch("1", "1"));
                Assert.True(Regex.IsMatch("2", "2")); // previous removed from cache
                Assert.True(GetCachedItemsNum() == 1);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void Ctor_Cache_Shrink_cache()
        {
            RemoteInvoke(() =>
            {
                Regex.CacheSize = 2;
                Assert.True(Regex.IsMatch("1", "1"));
                Assert.True(Regex.IsMatch("2", "2"));
                Assert.True(GetCachedItemsNum() == 2);
                Regex.CacheSize = 1;
                Assert.True(GetCachedItemsNum() == 1);
                Regex.CacheSize = 0; // clear
                Assert.True(GetCachedItemsNum() == 0);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void Ctor_Cache_Promote_entries()
        {
            RemoteInvoke(() =>
            {
                Regex.CacheSize = 3;
                Assert.True(Regex.IsMatch("1", "1"));
                Assert.True(Regex.IsMatch("2", "2")); 
                Assert.True(Regex.IsMatch("3", "3"));
                Assert.True(GetCachedItemsNum() == 3);
                Assert.True(Regex.IsMatch("1", "1")); // should be put first
                Assert.True(GetCachedItemsNum() == 3);
                Regex.CacheSize = 1;  // only 1 stays
                Assert.True(GetCachedItemsNum() == 1);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void Ctor_Cache_Uses_culture_and_options()
        {
            RemoteInvoke(() =>
            {
                Regex.CacheSize = 0;
                Regex.CacheSize = 3;
                Assert.True(Regex.IsMatch("1", "1", RegexOptions.IgnoreCase));
                Assert.True(Regex.IsMatch("1", "1", RegexOptions.Multiline));
                Assert.True(GetCachedItemsNum() == 2);
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                Assert.True(Regex.IsMatch("1", "1", RegexOptions.Multiline));
                Assert.True(GetCachedItemsNum() == 3);
                return SuccessExitCode;
            }).Dispose();
        }

        private int GetCachedItemsNum()
        {
            Type type = typeof(Regex);
            FieldInfo info = type.GetField("s_livecode", BindingFlags.NonPublic | BindingFlags.Static);
            var dictionary = (ICollection) info.GetValue(null);
            return dictionary.Count;
        }
    }
}
