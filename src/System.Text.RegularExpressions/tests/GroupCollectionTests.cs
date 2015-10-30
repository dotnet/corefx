// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Test
{
    public class GroupCollectionTests
    {
        [Fact]
        public static void EnumeratorTest()
        {
            GroupCollection collection = CreateCollection();
            IEnumerator enumerator = collection.GetEnumerator();

            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();

                Assert.Equal(enumerator.Current, collection[i]);
            }

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();

            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();

                Assert.Equal(enumerator.Current, collection[i]);
            }

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void Contains()
        {
            ICollection<Group> collection = CreateCollection();
            foreach (Group item in collection)
            {
                Assert.True(collection.Contains(item));
            }

            foreach (Group item in CreateCollection())
            {
                Assert.False(collection.Contains(item));
            }
        }

        [Fact]
        public static void ContainsNonGeneric()
        {
            IList collection = CreateCollection();
            foreach (object item in collection)
            {
                Assert.True(collection.Contains(item));
            }

            foreach (object item in CreateCollection())
            {
                Assert.False(collection.Contains(item));
            }

            Assert.False(collection.Contains(new object()));
        }

        [Fact]
        public static void IndexOf()
        {
            IList<Group> collection = CreateCollection();

            int i = 0;
            foreach (Group item in collection)
            {
                Assert.Equal(i, collection.IndexOf(item));
                i++;
            }

            foreach (Group item in CreateCollection())
            {
                Assert.Equal(-1, collection.IndexOf(item));
            }
        }

        [Fact]
        public static void IndexOfNonGeneric()
        {
            IList collection = CreateCollection();

            int i = 0;
            foreach (object item in collection)
            {
                Assert.Equal(i, collection.IndexOf(item));
                i++;
            }

            foreach (object item in CreateCollection())
            {
                Assert.Equal(-1, collection.IndexOf(item));
            }

            Assert.Equal(-1, collection.IndexOf(new object()));
        }

        [Fact]
        public static void Indexer()
        {
            GroupCollection collection = CreateCollection();
            Assert.Equal("212-555-6666", collection[0].ToString());
            Assert.Equal("212", collection[1].ToString());
            Assert.Equal("555-6666", collection[2].ToString());
        }

        [Fact]
        public static void IndexerIListOfT()
        {
            IList<Group> collection = CreateCollection();
            Assert.Equal("212-555-6666", collection[0].ToString());
            Assert.Equal("212", collection[1].ToString());
            Assert.Equal("555-6666", collection[2].ToString());
        }

        [Fact]
        public static void IndexerIListNonGeneric()
        {
            IList collection = CreateCollection();
            Assert.Equal("212-555-6666", collection[0].ToString());
            Assert.Equal("212", collection[1].ToString());
            Assert.Equal("555-6666", collection[2].ToString());
        }

        [Fact]
        public static void CopyToExceptions()
        {
            GroupCollection collection = CreateCollection();
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new Group[1], -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Group[1], 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Group[1], 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new Group[1], 2));
        }

        [Fact]
        public static void CopyToNonGenericExceptions()
        {
            ICollection collection = CreateCollection();
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Group[10, 10], 0));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Group[1], -1));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Group[1], 0));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Group[1], 1));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Group[1], 2));
            Assert.Throws<InvalidCastException>(() => collection.CopyTo(new int[collection.Count], 0));
        }

        [Fact]
        public static void CopyTo()
        {
            string[] expected = new[] { "212-555-6666", "212", "555-6666" };
            GroupCollection collection = CreateCollection();

            Group[] array = new Group[collection.Count];
            collection.CopyTo(array, 0);

            Assert.Equal(expected, array.Select(c => c.ToString()));
        }

        [Fact]
        public static void CopyToNonGeneric()
        {
            string[] expected = new[] { "212-555-6666", "212", "555-6666" };
            ICollection collection = CreateCollection();

            Group[] array = new Group[collection.Count];
            collection.CopyTo(array, 0);

            Assert.Equal(expected, array.Select(c => c.ToString()));
        }

        [Fact]
        public static void SyncRoot()
        {
            ICollection collection = CreateCollection();
            Assert.NotNull(collection.SyncRoot);
            Assert.Same(collection.SyncRoot, collection.SyncRoot);
        }

        [Fact]
        public static void IsNotSynchronized()
        {
            ICollection collection = CreateCollection();
            Assert.False(collection.IsSynchronized);
        }

        [Fact]
        public void IsReadOnly()
        {
            IList<Group> list = CreateCollection();
            Assert.True(list.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => list.Add(default(Group)));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.Insert(0, default(Group)));
            Assert.Throws<NotSupportedException>(() => list.Remove(default(Group)));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = default(Group));
        }

        [Fact]
        public static void IsReadOnlyNonGeneric()
        {
            IList list = CreateCollection();
            Assert.True(list.IsReadOnly);
            Assert.True(list.IsFixedSize);
            Assert.Throws<NotSupportedException>(() => list.Add(default(Group)));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.Insert(0, default(Group)));
            Assert.Throws<NotSupportedException>(() => list.Remove(default(Group)));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = default(Group));
        }

        [Fact]
        public static void DebuggerAttributeTests()
        {
            DebuggerAttributes.ValidateDebuggerDisplayReferences(CreateCollection());
            DebuggerAttributes.ValidateDebuggerTypeProxyProperties(CreateCollection());
        }

        private static GroupCollection CreateCollection()
        {
            Regex regex = new Regex(@"(\d{3})-(\d{3}-\d{4})");
            Match match = regex.Match("212-555-6666");
            return match.Groups;
        }
    }
}
