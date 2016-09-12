// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    public class CaptureCollectionTests
    {
        [Fact]
        public static void GetEnumerator()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");

            CaptureCollection captures = match.Captures;
            IEnumerator enumerator = captures.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(captures[counter], enumerator.Current);
                    counter++;
                }
                Assert.False(enumerator.MoveNext());
                Assert.Equal(captures.Count, counter);
                enumerator.Reset();
            }
        }

        [Fact]
        public static void GetEnumerator_Invalid()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");
            IEnumerator enumerator = match.Captures.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            while (enumerator.MoveNext()) ;
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void Item_Get_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            CaptureCollection captures = regex.Match("aaabbccccccccccaaaabc").Captures;

            Assert.Throws<ArgumentOutOfRangeException>("i", () => captures[-1]);
            Assert.Throws<ArgumentOutOfRangeException>("i", () => captures[captures.Count]);
        }

        [Fact]
        public void ICollection_Properties()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            CaptureCollection captures = regex.Match("aaabbccccccccccaaaabc").Captures;
            ICollection collection = captures;

            Assert.False(collection.IsSynchronized);
            Assert.Same(collection.SyncRoot, collection.SyncRoot);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void ICollection_CopyTo(int index)
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            CaptureCollection captures = regex.Match("aaabbccccccccccaaaabc").Captures;
            ICollection collection = captures;

            RegularExpressions.Capture[] copy = new RegularExpressions.Capture[collection.Count + index];
            collection.CopyTo(copy, index);

            for (int i = 0; i < index; i++)
            {
                Assert.Null(copy[i]);
            }
            for (int i = index; i < copy.Length; i++)
            {
                Assert.Same(captures[i - index], copy[i]);
            }
        }

        [Fact]
        public void ICollection_CopyTo_Invalid()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            ICollection collection = regex.Match("aaabbccccccccccaaaabc").Captures;

            // Array is null
            Assert.Throws<ArgumentNullException>("array", () => collection.CopyTo(null, 0));

            // Array is multidimensional
            Assert.Throws<ArgumentException>(null, () => collection.CopyTo(new object[10, 10], 0));

            // Array has a non-zero lower bound
            Array o = Array.CreateInstance(typeof(object), new int[] { 10 }, new int[] { 10 });
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(o, 0));

            // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new object[collection.Count], -1));

            // Invalid index + length
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new object[collection.Count], 1));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new object[collection.Count + 1], 2));
        }
    }
}
