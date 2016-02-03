// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Collections.Immutable.Tests
{
    public class ImmutableDictionaryTest : ImmutableDictionaryTestBase
    {
        [Fact]
        public void AddExistingKeySameValueTest()
        {
            AddExistingKeySameValueTestHelper(Empty(StringComparer.Ordinal, StringComparer.Ordinal), "Company", "Microsoft", "Microsoft");
            AddExistingKeySameValueTestHelper(Empty(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase), "Company", "Microsoft", "MICROSOFT");
        }

        [Fact]
        public void AddExistingKeyDifferentValueTest()
        {
            AddExistingKeyDifferentValueTestHelper(Empty(StringComparer.Ordinal, StringComparer.Ordinal), "Company", "Microsoft", "MICROSOFT");
        }

        [Fact]
        public void UnorderedChangeTest()
        {
            var map = Empty<string, string>(StringComparer.Ordinal)
                .Add("Johnny", "Appleseed")
                .Add("JOHNNY", "Appleseed");
            Assert.Equal(2, map.Count);
            Assert.True(map.ContainsKey("Johnny"));
            Assert.False(map.ContainsKey("johnny"));
            var newMap = map.WithComparers(StringComparer.OrdinalIgnoreCase);
            Assert.Equal(1, newMap.Count);
            Assert.True(newMap.ContainsKey("Johnny"));
            Assert.True(newMap.ContainsKey("johnny")); // because it's case insensitive
        }

        [Fact]
        public void ToSortedTest()
        {
            var map = Empty<string, string>(StringComparer.Ordinal)
                .Add("Johnny", "Appleseed")
                .Add("JOHNNY", "Appleseed");
            var sortedMap = map.ToImmutableSortedDictionary(StringComparer.Ordinal);
            Assert.Equal(sortedMap.Count, map.Count);
            CollectionAssertAreEquivalent<KeyValuePair<string, string>>(sortedMap.ToList(), map.ToList());
        }

        [Fact]
        public void SetItemUpdateEqualKeyTest()
        {
            var map = Empty<string, int>().WithComparers(StringComparer.OrdinalIgnoreCase)
                .SetItem("A", 1);
            map = map.SetItem("a", 2);
            Assert.Equal("a", map.Keys.Single());
        }

        /// <summary>
        /// Verifies that the specified value comparer is applied when
        /// checking for equality.
        /// </summary>
        [Fact]
        public void SetItemUpdateEqualKeyWithValueEqualityByComparer()
        {
            var map = Empty<string, CaseInsensitiveString>().WithComparers(StringComparer.OrdinalIgnoreCase, new MyStringOrdinalComparer());
            string key = "key";
            var value1 = "Hello";
            var value2 = "hello";
            map = map.SetItem(key, new CaseInsensitiveString(value1));
            map = map.SetItem(key, new CaseInsensitiveString(value2));
            Assert.Equal(value2, map[key].Value);

            Assert.Same(map, map.SetItem(key, new CaseInsensitiveString(value2)));
        }

        [Fact]
        public override void EmptyTest()
        {
            base.EmptyTest();
            this.EmptyTestHelperHash(Empty<int, bool>(), 5);
        }

        [Fact]
        public void ContainsValueTest()
        {
            this.ContainsValueTestHelper(ImmutableDictionary<int, GenericParameterHelper>.Empty, 1, new GenericParameterHelper());
        }

        [Fact]
        public void EnumeratorWithHashCollisionsTest()
        {
            var emptyMap = Empty<int, GenericParameterHelper>(new BadHasher<int>());
            this.EnumeratorTestHelper(emptyMap);
        }

        [Fact]
        public void Create()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = new Dictionary<string, string> { { "a", "b" } };
            var keyComparer = StringComparer.OrdinalIgnoreCase;
            var valueComparer = StringComparer.CurrentCulture;

            var dictionary = ImmutableDictionary.Create<string, string>();
            Assert.Equal(0, dictionary.Count);
            Assert.Same(EqualityComparer<string>.Default, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = ImmutableDictionary.Create<string, string>(keyComparer);
            Assert.Equal(0, dictionary.Count);
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = ImmutableDictionary.Create(keyComparer, valueComparer);
            Assert.Equal(0, dictionary.Count);
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(valueComparer, dictionary.ValueComparer);

            dictionary = ImmutableDictionary.CreateRange(pairs);
            Assert.Equal(1, dictionary.Count);
            Assert.Same(EqualityComparer<string>.Default, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = ImmutableDictionary.CreateRange(keyComparer, pairs);
            Assert.Equal(1, dictionary.Count);
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = ImmutableDictionary.CreateRange(keyComparer, valueComparer, pairs);
            Assert.Equal(1, dictionary.Count);
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(valueComparer, dictionary.ValueComparer);
        }

        [Fact]
        public void ToImmutableDictionary()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = new Dictionary<string, string> { { "a", "B" } };
            var keyComparer = StringComparer.OrdinalIgnoreCase;
            var valueComparer = StringComparer.CurrentCulture;

            ImmutableDictionary<string, string> dictionary = pairs.ToImmutableDictionary();
            Assert.Equal(1, dictionary.Count);
            Assert.Same(EqualityComparer<string>.Default, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = pairs.ToImmutableDictionary(keyComparer);
            Assert.Equal(1, dictionary.Count);
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = pairs.ToImmutableDictionary(keyComparer, valueComparer);
            Assert.Equal(1, dictionary.Count);
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(valueComparer, dictionary.ValueComparer);

            dictionary = pairs.ToImmutableDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToLowerInvariant());
            Assert.Equal(1, dictionary.Count);
            Assert.Equal("A", dictionary.Keys.Single());
            Assert.Equal("b", dictionary.Values.Single());
            Assert.Same(EqualityComparer<string>.Default, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = pairs.ToImmutableDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToLowerInvariant(), keyComparer);
            Assert.Equal(1, dictionary.Count);
            Assert.Equal("A", dictionary.Keys.Single());
            Assert.Equal("b", dictionary.Values.Single());
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, dictionary.ValueComparer);

            dictionary = pairs.ToImmutableDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToLowerInvariant(), keyComparer, valueComparer);
            Assert.Equal(1, dictionary.Count);
            Assert.Equal("A", dictionary.Keys.Single());
            Assert.Equal("b", dictionary.Values.Single());
            Assert.Same(keyComparer, dictionary.KeyComparer);
            Assert.Same(valueComparer, dictionary.ValueComparer);

            var list = new int[] { 1, 2 };
            var intDictionary = list.ToImmutableDictionary(n => (double)n);
            Assert.Equal(1, intDictionary[1.0]);
            Assert.Equal(2, intDictionary[2.0]);
            Assert.Equal(2, intDictionary.Count);

            var stringIntDictionary = list.ToImmutableDictionary(n => n.ToString(), StringComparer.OrdinalIgnoreCase);
            Assert.Same(StringComparer.OrdinalIgnoreCase, stringIntDictionary.KeyComparer);
            Assert.Equal(1, stringIntDictionary["1"]);
            Assert.Equal(2, stringIntDictionary["2"]);
            Assert.Equal(2, intDictionary.Count);

            Assert.Throws<ArgumentNullException>(() => list.ToImmutableDictionary<int, int>(null));
            Assert.Throws<ArgumentNullException>(() => list.ToImmutableDictionary<int, int, int>(null, v => v));
            Assert.Throws<ArgumentNullException>(() => list.ToImmutableDictionary<int, int, int>(k => k, null));

            list.ToDictionary(k => k, v => v, null); // verifies BCL behavior is to not throw.
            list.ToImmutableDictionary(k => k, v => v, null, null);
        }

        [Fact]
        public void ToImmutableDictionaryOptimized()
        {
            var dictionary = ImmutableDictionary.Create<string, string>();
            var result = dictionary.ToImmutableDictionary();
            Assert.Same(dictionary, result);

            var cultureComparer = StringComparer.CurrentCulture;
            result = dictionary.WithComparers(cultureComparer, StringComparer.OrdinalIgnoreCase);
            Assert.Same(cultureComparer, result.KeyComparer);
            Assert.Same(StringComparer.OrdinalIgnoreCase, result.ValueComparer);
        }

        [Fact]
        public void WithComparers()
        {
            var map = ImmutableDictionary.Create<string, string>().Add("a", "1").Add("B", "1");
            Assert.Same(EqualityComparer<string>.Default, map.KeyComparer);
            Assert.True(map.ContainsKey("a"));
            Assert.False(map.ContainsKey("A"));

            map = map.WithComparers(StringComparer.OrdinalIgnoreCase);
            Assert.Same(StringComparer.OrdinalIgnoreCase, map.KeyComparer);
            Assert.Equal(2, map.Count);
            Assert.True(map.ContainsKey("a"));
            Assert.True(map.ContainsKey("A"));
            Assert.True(map.ContainsKey("b"));

            var cultureComparer = StringComparer.CurrentCulture;
            map = map.WithComparers(StringComparer.OrdinalIgnoreCase, cultureComparer);
            Assert.Same(StringComparer.OrdinalIgnoreCase, map.KeyComparer);
            Assert.Same(cultureComparer, map.ValueComparer);
            Assert.Equal(2, map.Count);
            Assert.True(map.ContainsKey("a"));
            Assert.True(map.ContainsKey("A"));
            Assert.True(map.ContainsKey("b"));
        }

        [Fact]
        public void WithComparersCollisions()
        {
            // First check where collisions have matching values.
            var map = ImmutableDictionary.Create<string, string>()
                .Add("a", "1").Add("A", "1");
            map = map.WithComparers(StringComparer.OrdinalIgnoreCase);
            Assert.Same(StringComparer.OrdinalIgnoreCase, map.KeyComparer);
            Assert.Equal(1, map.Count);
            Assert.True(map.ContainsKey("a"));
            Assert.Equal("1", map["a"]);

            // Now check where collisions have conflicting values.
            map = ImmutableDictionary.Create<string, string>()
              .Add("a", "1").Add("A", "2").Add("b", "3");
            Assert.Throws<ArgumentException>(() => map.WithComparers(StringComparer.OrdinalIgnoreCase));

            // Force all values to be considered equal.
            map = map.WithComparers(StringComparer.OrdinalIgnoreCase, EverythingEqual<string>.Default);
            Assert.Same(StringComparer.OrdinalIgnoreCase, map.KeyComparer);
            Assert.Same(EverythingEqual<string>.Default, map.ValueComparer);
            Assert.Equal(2, map.Count);
            Assert.True(map.ContainsKey("a"));
            Assert.True(map.ContainsKey("b"));
        }

        [Fact]
        public void CollisionExceptionMessageContainsKey()
        {
            var map = ImmutableDictionary.Create<string, string>()
                .Add("firstKey", "1").Add("secondKey", "2");
            var exception = Assert.Throws<ArgumentException>(() => map.Add("firstKey", "3"));
            Assert.Contains("firstKey", exception.Message);
        }

        [Fact]
        public void WithComparersEmptyCollection()
        {
            var map = ImmutableDictionary.Create<string, string>();
            Assert.Same(EqualityComparer<string>.Default, map.KeyComparer);
            map = map.WithComparers(StringComparer.OrdinalIgnoreCase);
            Assert.Same(StringComparer.OrdinalIgnoreCase, map.KeyComparer);
        }

        [Fact]
        public void GetValueOrDefaultOfIImmutableDictionary()
        {
            IImmutableDictionary<string, int> empty = ImmutableDictionary.Create<string, int>();
            IImmutableDictionary<string, int> populated = ImmutableDictionary.Create<string, int>().Add("a", 5);
            Assert.Equal(0, empty.GetValueOrDefault("a"));
            Assert.Equal(1, empty.GetValueOrDefault("a", 1));
            Assert.Equal(5, populated.GetValueOrDefault("a"));
            Assert.Equal(5, populated.GetValueOrDefault("a", 1));
        }

        [Fact]
        public void GetValueOrDefaultOfConcreteType()
        {
            var empty = ImmutableDictionary.Create<string, int>();
            var populated = ImmutableDictionary.Create<string, int>().Add("a", 5);
            Assert.Equal(0, empty.GetValueOrDefault("a"));
            Assert.Equal(1, empty.GetValueOrDefault("a", 1));
            Assert.Equal(5, populated.GetValueOrDefault("a"));
            Assert.Equal(5, populated.GetValueOrDefault("a", 1));
        }

        [Fact]
        public void EnumeratorRecyclingMisuse()
        {
            var collection = ImmutableDictionary.Create<int, int>().Add(5, 3);
            var enumerator = collection.GetEnumerator();
            var enumeratorCopy = enumerator;
            Assert.True(enumerator.MoveNext());
            Assert.False(enumerator.MoveNext());
            enumerator.Dispose();
            Assert.Throws<ObjectDisposedException>(() => enumerator.MoveNext());
            Assert.Throws<ObjectDisposedException>(() => enumerator.Reset());
            Assert.Throws<ObjectDisposedException>(() => enumerator.Current);
            Assert.Throws<ObjectDisposedException>(() => enumeratorCopy.MoveNext());
            Assert.Throws<ObjectDisposedException>(() => enumeratorCopy.Reset());
            Assert.Throws<ObjectDisposedException>(() => enumeratorCopy.Current);
            enumerator.Dispose(); // double-disposal should not throw
            enumeratorCopy.Dispose();

            // We expect that acquiring a new enumerator will use the same underlying Stack<T> object,
            // but that it will not throw exceptions for the new enumerator.
            enumerator = collection.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            enumerator.Dispose();
        }

        [Fact]
        public void DebuggerAttributesValid()
        {
            DebuggerAttributes.ValidateDebuggerDisplayReferences(ImmutableDictionary.Create<int, int>());
            DebuggerAttributes.ValidateDebuggerTypeProxyProperties(ImmutableDictionary.Create<string, int>());

            object rootNode = DebuggerAttributes.GetFieldValue(ImmutableDictionary.Create<string, string>(), "_root");
            DebuggerAttributes.ValidateDebuggerDisplayReferences(rootNode);
        }

        protected override IImmutableDictionary<TKey, TValue> Empty<TKey, TValue>()
        {
            return ImmutableDictionaryTest.Empty<TKey, TValue>();
        }

        protected override IImmutableDictionary<string, TValue> Empty<TValue>(StringComparer comparer)
        {
            return ImmutableDictionary.Create<string, TValue>(comparer);
        }

        protected override IEqualityComparer<TValue> GetValueComparer<TKey, TValue>(IImmutableDictionary<TKey, TValue> dictionary)
        {
            return ((ImmutableDictionary<TKey, TValue>)dictionary).ValueComparer;
        }

        internal override IBinaryTree GetRootNode<TKey, TValue>(IImmutableDictionary<TKey, TValue> dictionary)
        {
            return ((ImmutableDictionary<TKey, TValue>)dictionary).Root;
        }

        protected void ContainsValueTestHelper<TKey, TValue>(ImmutableDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            Assert.False(map.ContainsValue(value));
            Assert.True(map.Add(key, value).ContainsValue(value));
        }

        private static ImmutableDictionary<TKey, TValue> Empty<TKey, TValue>(IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null)
        {
            return ImmutableDictionary<TKey, TValue>.Empty.WithComparers(keyComparer, valueComparer);
        }

        private void EmptyTestHelperHash<TKey, TValue>(IImmutableDictionary<TKey, TValue> empty, TKey someKey)
        {
            Assert.Same(EqualityComparer<TKey>.Default, ((IHashKeyCollection<TKey>)empty).KeyComparer);
        }

        /// <summary>
        /// An ordinal comparer for case-insensitive strings.
        /// </summary>
        private class MyStringOrdinalComparer : EqualityComparer<CaseInsensitiveString>
        {
            public override bool Equals(CaseInsensitiveString x, CaseInsensitiveString y)
            {
                return StringComparer.Ordinal.Equals(x.Value, y.Value);
            }

            public override int GetHashCode(CaseInsensitiveString obj)
            {
                return StringComparer.Ordinal.GetHashCode(obj.Value);
            }
        }

        /// <summary>
        /// A string-wrapper that considers equality based on case insensitivity.
        /// </summary>
        private class CaseInsensitiveString
        {
            public CaseInsensitiveString(string value)
            {
                Value = value;
            }

            public string Value { get; private set; }
            public override int GetHashCode()
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
            }
            public override bool Equals(object obj)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(this.Value, ((CaseInsensitiveString)obj).Value);
            }
        }
    }
}
