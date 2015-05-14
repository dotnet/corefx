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
    public class CaptureCollectionTests
    {
        [Fact]
        public static void CaptureCollection_GetEnumeratorTest_Negative()
        {
            CaptureCollection collection = CreateCollection();

            IEnumerator enumerator = collection.GetEnumerator();

            Capture current;

            Assert.Throws<InvalidOperationException>(() => current = (Capture)enumerator.Current);


            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();
            }

            enumerator.MoveNext();

            Assert.Throws<InvalidOperationException>(() => current = (Capture)enumerator.Current);
            enumerator.Reset();

            Assert.Throws<InvalidOperationException>(() => current = (Capture)enumerator.Current);
        }

        [Fact]
        public static void CaptureCollection_GetEnumeratorTest()
        {
            CaptureCollection collection = CreateCollection();

            IEnumerator enumerator = collection.GetEnumerator();

            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();

                Assert.Equal(enumerator.Current, collection[i]);
            }

            Assert.False(enumerator.MoveNext());

            enumerator.Reset();

            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();

                Assert.Equal(enumerator.Current, collection[i]);
            }
        }

        [Fact]
        public static void Indexer()
        {
            CaptureCollection collection = CreateCollection();
            Assert.Equal("This ", collection[0].ToString());
            Assert.Equal("is ", collection[1].ToString());
            Assert.Equal("a ", collection[2].ToString());
            Assert.Equal("sentence", collection[3].ToString());
        }

        [Fact]
        public static void IndexerIListOfT()
        {
            IList<Capture> collection = CreateCollection();
            Assert.Equal("This ", collection[0].ToString());
            Assert.Equal("is ", collection[1].ToString());
            Assert.Equal("a ", collection[2].ToString());
            Assert.Equal("sentence", collection[3].ToString());
        }

        [Fact]
        public static void IndexerIListNonGeneric()
        {
            IList collection = CreateCollection();
            Assert.Equal("This ", collection[0].ToString());
            Assert.Equal("is ", collection[1].ToString());
            Assert.Equal("a ", collection[2].ToString());
            Assert.Equal("sentence", collection[3].ToString());
        }

        [Fact]
        public static void Contains()
        {
            ICollection<Capture> collection = CreateCollection();
            foreach (Capture item in collection)
            {
                Assert.True(collection.Contains(item));
            }

            foreach (Capture item in CreateCollection())
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
            IList<Capture> collection = CreateCollection();

            int i = 0;
            foreach (Capture item in collection)
            {
                Assert.Equal(i, collection.IndexOf(item));
                i++;
            }

            foreach (Capture item in CreateCollection())
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
        public static void CopyToExceptions()
        {
            CaptureCollection collection = CreateCollection();
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new Capture[1], -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Capture[1], 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Capture[1], 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new Capture[1], 2));
        }

        [Fact]
        public static void CopyToNonGenericExceptions()
        {
            ICollection collection = CreateCollection();
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Capture[10, 10], 0));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Capture[1], -1));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Capture[1], 0));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Capture[1], 1));
            Assert.Throws<IndexOutOfRangeException>(() => collection.CopyTo(new Capture[1], 2));
            Assert.Throws<InvalidCastException>(() => collection.CopyTo(new int[collection.Count], 0));
        }

        [Fact]
        public static void CopyTo()
        {
            string[] expected = new[] { "This ", "is ", "a ", "sentence" };
            CaptureCollection collection = CreateCollection();

            Capture[] array = new Capture[collection.Count];
            collection.CopyTo(array, 0);

            Assert.Equal(expected, array.Select(c => c.ToString()));
        }

        [Fact]
        public static void CopyToNonGeneric()
        {
            string[] expected = new[] { "This ", "is ", "a ", "sentence" };
            ICollection collection = CreateCollection();

            Capture[] array = new Capture[collection.Count];
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
            IList<Capture> list = CreateCollection();
            Assert.True(list.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => list.Add(default(Capture)));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.Insert(0, default(Capture)));
            Assert.Throws<NotSupportedException>(() => list.Remove(default(Capture)));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = default(Capture));
        }

        [Fact]
        public static void IsReadOnlyNonGeneric()
        {
            IList list = CreateCollection();
            Assert.True(list.IsReadOnly);
            Assert.True(list.IsFixedSize);
            Assert.Throws<NotSupportedException>(() => list.Add(default(Capture)));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.Insert(0, default(Capture)));
            Assert.Throws<NotSupportedException>(() => list.Remove(default(Capture)));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = default(Capture));
        }

        [Fact]
        public static void DebuggerAttributeTests()
        {
            DebuggerAttributes.ValidateDebuggerDisplayReferences(CreateCollection());
            DebuggerAttributes.ValidateDebuggerTypeProxyProperties(CreateCollection());
        }

        private static CaptureCollection CreateCollection()
        {
            Regex regex = new Regex(@"\b(\w+\s*)+\.");
            Match match = regex.Match("This is a sentence.");
            return match.Groups[1].Captures;
        }
    }
}
