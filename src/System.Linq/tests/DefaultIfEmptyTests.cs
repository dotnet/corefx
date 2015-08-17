using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class DefaultIfEmptyTests
    {
        private static IEnumerable<T> UnoptimisedEmpty<T>()
        {
            yield break;
        }

        [Fact]
        public void DefaultForEmpty()
        {
            Assert.Same(null, Enumerable.Empty<string>().DefaultIfEmpty().First());
            Assert.Same(null, UnoptimisedEmpty<string>().DefaultIfEmpty().First());
        }

        [Fact]
        public void NoDefaultForNotEmpty()
        {
            Assert.Same("test", new[]{ "test" }.DefaultIfEmpty().First());
        }
        
        [Fact]
        public void KeepTypeIfCollectionNotEmpty()
        {
            Assert.True(new []{ "test" }.DefaultIfEmpty() is string[]);
        }
        
        [Fact]
        public void DefaultIfEmptyThrowOnNull()
        {
            IEnumerable<int> nullEnum = null;
            Assert.Throws<ArgumentNullException>(() => nullEnum.DefaultIfEmpty());
        }
    }
}
