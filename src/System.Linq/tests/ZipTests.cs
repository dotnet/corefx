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
        public void ResultsReadonly()
        {
            var zip = (IList<int>)new [] { 1, 2 }.Zip(new [] { 1, 2, 3 }, (x, y) => x + y);

            Assert.True(zip.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => zip.Add(0));
            Assert.Throws<NotSupportedException>(() => zip[0] = 0);
            Assert.Throws<NotSupportedException>(() => zip.Remove(0));
            Assert.Throws<NotSupportedException>(() => zip.Remove(0));
            Assert.Throws<NotSupportedException>(() => zip.Clear());
            Assert.Throws<NotSupportedException>(() => zip.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => zip.Insert(0, 0));
        }
        
        [Fact]
        public void WhenIList()
        {
            var list = (IList<int>)new [] { 1, 2 }.Zip(new [] { 1, 2, 3 }, (x, y) => x + y);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list[0]);
            Assert.Equal(4, list[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[-1].ToString());
            Assert.Throws<ArgumentOutOfRangeException>(() => list[2].ToString());
            var targetArray = new int[4];
            list.CopyTo(targetArray, 1);
            Assert.Equal(new int[]{ 0, 2, 4, 0 }, targetArray);
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(targetArray, -1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(targetArray, 3));
            Assert.Equal(-1, list.IndexOf(3));
            Assert.Equal(1, list.IndexOf(4));
            Assert.False(list.Contains(3));
            Assert.True(list.Contains(4));

            list = (IList<int>)new [] { 1, 2, 3 }.Zip(new [] { 1, 2 }, (x, y) => x + y);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list[0]);
            Assert.Equal(4, list[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[-1].ToString());
            Assert.Throws<ArgumentOutOfRangeException>(() => list[2].ToString());
            targetArray = new int[4];
            list.CopyTo(targetArray, 1);
            Assert.Equal(new int[]{ 0, 2, 4, 0 }, targetArray);
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(targetArray, -1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(targetArray, 3));
            Assert.Equal(-1, list.IndexOf(3));
            Assert.Equal(1, list.IndexOf(4));
            Assert.False(list.Contains(3));
            Assert.True(list.Contains(4));
        }
        
        [Fact]
        public void ExpectedResults()
        {
            Assert.Equal(new [] { 2, 4 }, new [] { 1, 2, 3 }.Zip(new []{ 1, 2 }, (x, y) => x + y).Where(x => true));
            Assert.Equal(new [] { 2, 4 }, new [] { 1, 2, 3 }.Zip(new []{ 1, 2 }.Where(x => true), (x, y) => x + y));
        }
    }
}