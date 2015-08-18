using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ZipTests
    {
        [Fact]
        public void NullsFail()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).Zip(new [] { 0 }, (x, y) => x + y));
            Assert.Throws<ArgumentNullException>(() => new [] { 0 }.Zip((IEnumerable<int>)null, (x, y) => x + y));
            Assert.Throws<ArgumentNullException>(() => new [] { 0 }.Zip(new [] { 0 }, (Func<int, int, int>)null));
        }
        
        [Fact]
        public void ExpectedResults()
        {
            Assert.Equal(new [] { 2, 4 }, new [] { 1, 2, 3 }.Zip(new []{ 1, 2 }, (x, y) => x + y).Where(x => true));
            Assert.Equal(new [] { 2, 4 }, new [] { 1, 2, 3 }.Zip(new []{ 1, 2 }.Where(x => true), (x, y) => x + y));
        }
    }
}