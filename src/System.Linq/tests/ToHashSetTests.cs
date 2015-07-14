using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Linq.Tests
{
    public class ToHashSetTests
    {
        private class CustomComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) { return EqualityComparer<T>.Default.Equals(x, y); }
            public int GetHashCode(T obj) { return EqualityComparer<T>.Default.GetHashCode(obj); }
            public override bool Equals(object obj)
            {
                return obj is CustomComparer<T>;
            }
            public override int GetHashCode()
            {
                return -465576218; // Equals all instances of same class, so randomly-produced constant.
            }
        }
        [Fact]
        public void NoExplicitComparer()
        {
            var hs = Enumerable.Range(0, 50).ToHashSet();
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Equal(EqualityComparer<int>.Default, hs.Comparer);
        }
        [Fact]
        public void ExplicitComparer()
        {
            var hs = Enumerable.Range(0, 50).ToHashSet(new CustomComparer<int>());
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Equal(new CustomComparer<int>(), hs.Comparer);
        }
        [Fact]
        public void TolerateNullElements()
        {
            // Unlike the keys of a dictionary, HashSet tolerates null items.
            new string[] { "abc", null, "def" }.ToHashSet();
        }
        [Fact]
        public void TolerateDuplicates()
        {
            // ToDictionary throws on duplicates, because that is the normal behaviour
            // of Dictionary<TKey, TValue>.Add().
            // By the same token, since the normal behaviour of HashSet<T>.Add()
            // is to signal duplicates without an exception ToHashSet should
            // tolerate duplicates.
            var hs = Enumerable.Range(0, 50).Select(i => i / 5).ToHashSet();
            // The OrderBy isn't strictly necessary, but that depends upon an
            // implementation detail of HashSet, so explicitly force ordering.
            Assert.Equal(Enumerable.Range(0, 10), hs.OrderBy(i => i));
        }
        [Fact]
        public void ThrowOnNullSource()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<object>)null).ToHashSet());
        }
    }
}