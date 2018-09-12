// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Immutable.Tests
{
    public abstract partial class ImmutableDictionaryTestBase : ImmutablesTestBase
    {
        [Fact]
        public void EnumeratorTest()
        {
            this.EnumeratorTestHelper(this.Empty<int, GenericParameterHelper>());
        }

        [Fact]
        public void KeysTest()
        {
            this.KeysTestHelper(Empty<int, bool>(), 5);
        }

        [Fact]
        public void ValuesTest()
        {
            this.ValuesTestHelper(Empty<int, bool>(), 5);
        }

        [Fact]
        public void AddAscendingTest()
        {
            this.AddAscendingTestHelper(Empty<int, GenericParameterHelper>());
        }

        [Fact]
        public void DictionaryRemoveThrowsTest()
        {
            IDictionary<int, int> map = this.Empty<int, int>().Add(5, 3).ToReadOnlyDictionary();
            Assert.Throws<NotSupportedException>(() => map.Remove(5));
        }

        [Fact]
        public void DictionaryAddThrowsTest()
        {
            IDictionary<int, int> map = this.Empty<int, int>().ToReadOnlyDictionary();
            Assert.Throws<NotSupportedException>(() => map.Add(5, 3));
        }

        [Fact]
        public void DictionaryIndexSetThrowsTest()
        {
            IDictionary<int, int> map = this.Empty<int, int>().ToReadOnlyDictionary();
            Assert.Throws<NotSupportedException>(() => map[3] = 5);
        }

        [Fact]
        public void EqualsTest()
        {
            Assert.False(Empty<int, int>().Equals(null));
            Assert.False(Empty<int, int>().Equals("hi"));
            Assert.True(Empty<int, int>().Equals(Empty<int, int>()));
            Assert.False(Empty<int, int>().Add(3, 2).Equals(Empty<int, int>().Add(3, 2)));
            Assert.False(Empty<int, int>().Add(3, 2).Equals(Empty<int, int>().Add(3, 1)));
            Assert.False(Empty<int, int>().Add(5, 1).Equals(Empty<int, int>().Add(3, 1)));
            Assert.False(Empty<int, int>().Add(3, 1).Add(5, 1).Equals(Empty<int, int>().Add(3, 1)));
            Assert.False(Empty<int, int>().Add(3, 1).Equals(Empty<int, int>().Add(3, 1).Add(5, 1)));

            Assert.True(Empty<int, int>().ToReadOnlyDictionary().Equals(Empty<int, int>()));
            Assert.True(Empty<int, int>().Equals(Empty<int, int>().ToReadOnlyDictionary()));
            Assert.True(Empty<int, int>().ToReadOnlyDictionary().Equals(Empty<int, int>().ToReadOnlyDictionary()));
            Assert.False(Empty<int, int>().Add(3, 1).ToReadOnlyDictionary().Equals(Empty<int, int>()));
            Assert.False(Empty<int, int>().Equals(Empty<int, int>().Add(3, 1).ToReadOnlyDictionary()));
            Assert.False(Empty<int, int>().ToReadOnlyDictionary().Equals(Empty<int, int>().Add(3, 1).ToReadOnlyDictionary()));
        }

        [Fact]
        public void AddRangeTest()
        {
            var map = Empty<int, GenericParameterHelper>();
            map = map.AddRange(Enumerable.Range(1, 100).Select(n => new KeyValuePair<int, GenericParameterHelper>(n, new GenericParameterHelper())));
            CollectionAssertAreEquivalent(map.Select(kv => kv.Key).ToList(), Enumerable.Range(1, 100).ToList());
            this.VerifyAvlTreeState(map);
            Assert.Equal(100, map.Count);

            // Test optimization for empty map.
            var map2 = Empty<int, GenericParameterHelper>();
            var jointMap = map2.AddRange(map);
            Assert.Same(map, jointMap);

            jointMap = map2.AddRange(map.ToReadOnlyDictionary());
            Assert.Same(map, jointMap);

            jointMap = map2.AddRange(map.ToBuilder());
            Assert.Same(map, jointMap);
        }

        [Fact]
        public void AddDescendingTest()
        {
            this.AddDescendingTestHelper(Empty<int, GenericParameterHelper>());
        }

        [Fact]
        public void AddRemoveRandomDataTest()
        {
            this.AddRemoveRandomDataTestHelper(Empty<double, GenericParameterHelper>());
        }

        [Fact]
        public void AddRemoveEnumerableTest()
        {
            this.AddRemoveEnumerableTestHelper(Empty<int, int>());
        }

        private IImmutableDictionary<TKey, TValue> AddTestHelper<TKey, TValue>(IImmutableDictionary<TKey, TValue> map, TKey key, TValue value) where TKey : IComparable<TKey>
        {
            Assert.NotNull(map);
            Assert.NotNull(key);

            IImmutableDictionary<TKey, TValue> addedMap = map.Add(key, value);
            Assert.NotSame(map, addedMap);
            ////Assert.Equal(map.Count + 1, addedMap.Count);
            Assert.False(map.ContainsKey(key));
            Assert.True(addedMap.ContainsKey(key));
            AssertAreSame(value, addedMap.GetValueOrDefault(key));

            this.VerifyAvlTreeState(addedMap);

            return addedMap;
        }

        protected void AddAscendingTestHelper(IImmutableDictionary<int, GenericParameterHelper> map)
        {
            Assert.NotNull(map);

            for (int i = 0; i < 10; i++)
            {
                map = this.AddTestHelper(map, i, new GenericParameterHelper(i));
            }

            Assert.Equal(10, map.Count);
            for (int i = 0; i < 10; i++)
            {
                Assert.True(map.ContainsKey(i));
            }
        }

        protected void AddDescendingTestHelper(IImmutableDictionary<int, GenericParameterHelper> map)
        {
            for (int i = 10; i > 0; i--)
            {
                map = this.AddTestHelper(map, i, new GenericParameterHelper(i));
            }

            Assert.Equal(10, map.Count);
            for (int i = 10; i > 0; i--)
            {
                Assert.True(map.ContainsKey(i));
            }
        }

        protected void AddRemoveRandomDataTestHelper(IImmutableDictionary<double, GenericParameterHelper> map)
        {
            Assert.NotNull(map);

            double[] inputs = GenerateDummyFillData();
            for (int i = 0; i < inputs.Length; i++)
            {
                map = this.AddTestHelper(map, inputs[i], new GenericParameterHelper());
            }

            Assert.Equal(inputs.Length, map.Count);
            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.True(map.ContainsKey(inputs[i]));
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                map = map.Remove(inputs[i]);
                this.VerifyAvlTreeState(map);
            }

            Assert.Equal(0, map.Count);
        }

        protected void AddRemoveEnumerableTestHelper(IImmutableDictionary<int, int> empty)
        {
            Assert.NotNull(empty);

            Assert.Same(empty, empty.RemoveRange(Enumerable.Empty<int>()));
            Assert.Same(empty, empty.AddRange(Enumerable.Empty<KeyValuePair<int, int>>()));
            var list = new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(3, 5), new KeyValuePair<int, int>(8, 10) };
            var nonEmpty = empty.AddRange(list);
            this.VerifyAvlTreeState(nonEmpty);
            var halfRemoved = nonEmpty.RemoveRange(Enumerable.Range(1, 5));
            Assert.Equal(1, halfRemoved.Count);
            Assert.True(halfRemoved.ContainsKey(8));
            this.VerifyAvlTreeState(halfRemoved);
        }

        protected void KeysTestHelper<TKey, TValue>(IImmutableDictionary<TKey, TValue> map, TKey key)
        {
            Assert.Equal(0, map.Keys.Count());
            Assert.Equal(0, map.ToReadOnlyDictionary().Keys.Count());

            var nonEmpty = map.Add(key, default(TValue));
            Assert.Equal(1, nonEmpty.Keys.Count());
            Assert.Equal(1, nonEmpty.ToReadOnlyDictionary().Keys.Count());
            KeysOrValuesTestHelper(((IDictionary<TKey, TValue>)nonEmpty).Keys, key);
        }

        protected void ValuesTestHelper<TKey, TValue>(IImmutableDictionary<TKey, TValue> map, TKey key)
        {
            Assert.Equal(0, map.Values.Count());
            Assert.Equal(0, map.ToReadOnlyDictionary().Values.Count());

            var nonEmpty = map.Add(key, default(TValue));
            Assert.Equal(1, nonEmpty.Values.Count());
            Assert.Equal(1, nonEmpty.ToReadOnlyDictionary().Values.Count());
            KeysOrValuesTestHelper(((IDictionary<TKey, TValue>)nonEmpty).Values, default(TValue));
        }

        protected void EnumeratorTestHelper(IImmutableDictionary<int, GenericParameterHelper> map)
        {
            for (int i = 0; i < 10; i++)
            {
                map = this.AddTestHelper(map, i, new GenericParameterHelper(i));
            }

            int j = 0;
            foreach (KeyValuePair<int, GenericParameterHelper> pair in map)
            {
                Assert.Equal(j, pair.Key);
                Assert.Equal(j, pair.Value.Data);
                j++;
            }

            var list = map.ToList();
            Assert.Equal<KeyValuePair<int, GenericParameterHelper>>(list, ImmutableSetTest.ToListNonGeneric<KeyValuePair<int, GenericParameterHelper>>(map));

            // Apply some less common uses to the enumerator to test its metal.
            using (var enumerator = map.GetEnumerator())
            {
                enumerator.Reset(); // reset isn't usually called before MoveNext
                ManuallyEnumerateTest(list, enumerator);
                enumerator.Reset();
                ManuallyEnumerateTest(list, enumerator);

                // this time only partially enumerate
                enumerator.Reset();
                enumerator.MoveNext();
                enumerator.Reset();
                ManuallyEnumerateTest(list, enumerator);
            }

            var manualEnum = map.GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => manualEnum.Current);
            while (manualEnum.MoveNext()) { }
            Assert.False(manualEnum.MoveNext());
            Assert.Throws<InvalidOperationException>(() => manualEnum.Current);
        }

        internal abstract IBinaryTree GetRootNode<TKey, TValue>(IImmutableDictionary<TKey, TValue> dictionary);

        private static void KeysOrValuesTestHelper<T>(ICollection<T> collection, T containedValue)
        {
            Requires.NotNull(collection, nameof(collection));

            Assert.True(collection.Contains(containedValue));
            Assert.Throws<NotSupportedException>(() => collection.Add(default(T)));
            Assert.Throws<NotSupportedException>(() => collection.Clear());

            var nonGeneric = (ICollection)collection;
            Assert.NotNull(nonGeneric.SyncRoot);
            Assert.Same(nonGeneric.SyncRoot, nonGeneric.SyncRoot);
            Assert.True(nonGeneric.IsSynchronized);
            Assert.True(collection.IsReadOnly);

            AssertExtensions.Throws<ArgumentNullException>("array", () => nonGeneric.CopyTo(null, 0));
            var array = new T[collection.Count + 1];
            nonGeneric.CopyTo(array, 1);
            Assert.Equal(default(T), array[0]);
            Assert.Equal(array.Skip(1), nonGeneric.Cast<T>().ToArray());
        }

        private void VerifyAvlTreeState<TKey, TValue>(IImmutableDictionary<TKey, TValue> dictionary)
        {
            var rootNode = this.GetRootNode(dictionary);
            rootNode.VerifyBalanced();
            rootNode.VerifyHeightIsWithinTolerance(dictionary.Count);
        }
    }
}
