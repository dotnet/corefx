// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Collections.Immutable.Tests
{
    public class ImmutableSortedDictionaryBuilderTest : ImmutableDictionaryBuilderTestBase
    {
        [Fact]
        public void CreateBuilder()
        {
            var builder = ImmutableSortedDictionary.CreateBuilder<string, string>();
            Assert.NotNull(builder);

            builder = ImmutableSortedDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);
            Assert.Same(StringComparer.Ordinal, builder.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, builder.ValueComparer);

            builder = ImmutableSortedDictionary.CreateBuilder<string, string>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase);
            Assert.Same(StringComparer.Ordinal, builder.KeyComparer);
            Assert.Same(StringComparer.OrdinalIgnoreCase, builder.ValueComparer);
        }

        [Fact]
        public void ToBuilder()
        {
            var builder = ImmutableSortedDictionary<int, string>.Empty.ToBuilder();
            builder.Add(3, "3");
            builder.Add(5, "5");
            Assert.Equal(2, builder.Count);
            Assert.True(builder.ContainsKey(3));
            Assert.True(builder.ContainsKey(5));
            Assert.False(builder.ContainsKey(7));

            var set = builder.ToImmutable();
            Assert.Equal(builder.Count, set.Count);
            builder.Add(8, "8");
            Assert.Equal(3, builder.Count);
            Assert.Equal(2, set.Count);
            Assert.True(builder.ContainsKey(8));
            Assert.False(set.ContainsKey(8));
        }

        [Fact]
        public void BuilderFromMap()
        {
            var set = ImmutableSortedDictionary<int, string>.Empty.Add(1, "1");
            var builder = set.ToBuilder();
            Assert.True(builder.ContainsKey(1));
            builder.Add(3, "3");
            builder.Add(5, "5");
            Assert.Equal(3, builder.Count);
            Assert.True(builder.ContainsKey(3));
            Assert.True(builder.ContainsKey(5));
            Assert.False(builder.ContainsKey(7));

            var set2 = builder.ToImmutable();
            Assert.Equal(builder.Count, set2.Count);
            Assert.True(set2.ContainsKey(1));
            builder.Add(8, "8");
            Assert.Equal(4, builder.Count);
            Assert.Equal(3, set2.Count);
            Assert.True(builder.ContainsKey(8));

            Assert.False(set.ContainsKey(8));
            Assert.False(set2.ContainsKey(8));
        }

        [Fact]
        public void SeveralChanges()
        {
            var mutable = ImmutableSortedDictionary<int, string>.Empty.ToBuilder();
            var immutable1 = mutable.ToImmutable();
            Assert.Same(immutable1, mutable.ToImmutable()); //, "The Immutable property getter is creating new objects without any differences.");

            mutable.Add(1, "a");
            var immutable2 = mutable.ToImmutable();
            Assert.NotSame(immutable1, immutable2); //, "Mutating the collection did not reset the Immutable property.");
            Assert.Same(immutable2, mutable.ToImmutable()); //, "The Immutable property getter is creating new objects without any differences.");
            Assert.Equal(1, immutable2.Count);
        }

        [Fact]
        public void AddRange()
        {
            var builder = ImmutableSortedDictionary.Create<string, int>().ToBuilder();
            builder.AddRange(new Dictionary<string, int> { { "a", 1 }, { "b", 2 } });
            Assert.Equal(2, builder.Count);
            Assert.Equal(1, builder["a"]);
            Assert.Equal(2, builder["b"]);
        }

        [Fact]
        public void RemoveRange()
        {
            var builder =
                ImmutableSortedDictionary.Create<string, int>()
                                   .AddRange(new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } })
                                   .ToBuilder();
            Assert.Equal(3, builder.Count);
            builder.RemoveRange(new[] { "a", "b" });
            Assert.Equal(1, builder.Count);
            Assert.Equal(3, builder["c"]);
        }

        [Fact]
        public void EnumerateBuilderWhileMutating()
        {
            var builder = ImmutableSortedDictionary<int, string>.Empty
                .AddRange(Enumerable.Range(1, 10).Select(n => new KeyValuePair<int, string>(n, null)))
                .ToBuilder();
            Assert.Equal(
                Enumerable.Range(1, 10).Select(n => new KeyValuePair<int, string>(n, null)),
                builder);

            var enumerator = builder.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            builder.Add(11, null);

            // Verify that a new enumerator will succeed.
            Assert.Equal(
                Enumerable.Range(1, 11).Select(n => new KeyValuePair<int, string>(n, null)),
                builder);

            // Try enumerating further with the previous enumerable now that we've changed the collection.
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            enumerator.Reset();
            enumerator.MoveNext(); // resetting should fix the problem.

            // Verify that by obtaining a new enumerator, we can enumerate all the contents.
            Assert.Equal(
                Enumerable.Range(1, 11).Select(n => new KeyValuePair<int, string>(n, null)),
                builder);
        }

        [Fact]
        public void BuilderReusesUnchangedImmutableInstances()
        {
            var collection = ImmutableSortedDictionary<int, string>.Empty.Add(1, null);
            var builder = collection.ToBuilder();
            Assert.Same(collection, builder.ToImmutable()); // no changes at all.
            builder.Add(2, null);

            var newImmutable = builder.ToImmutable();
            Assert.NotSame(collection, newImmutable); // first ToImmutable with changes should be a new instance.
            Assert.Same(newImmutable, builder.ToImmutable()); // second ToImmutable without changes should be the same instance.

            collection = collection.Clear(); // now start with an empty collection 
            builder = collection.ToBuilder();
            Assert.Same(collection, builder.ToImmutable()); // again, no changes at all. 
            builder.ValueComparer = StringComparer.OrdinalIgnoreCase; // now, force the builder to clear its cache 

            newImmutable = builder.ToImmutable();
            Assert.NotSame(collection, newImmutable); // first ToImmutable with changes should be a new instance. 
            Assert.Same(newImmutable, builder.ToImmutable()); // second ToImmutable without changes should be the same instance. 
        }

        [Fact]
        public void ContainsValue()
        {
            var map = ImmutableSortedDictionary.Create<string, int>().Add("five", 5);
            var builder = map.ToBuilder();
            Assert.True(builder.ContainsValue(5));
            Assert.False(builder.ContainsValue(4));
        }

        [Fact]
        public void Clear()
        {
            var builder = ImmutableSortedDictionary.Create<string, int>().ToBuilder();
            builder.Add("five", 5);
            Assert.Equal(1, builder.Count);
            builder.Clear();
            Assert.Equal(0, builder.Count);
        }

        [Fact]
        public void KeyComparer()
        {
            var builder = ImmutableSortedDictionary.Create<string, string>()
                .Add("a", "1").Add("B", "1").ToBuilder();
            Assert.Same(Comparer<string>.Default, builder.KeyComparer);
            Assert.True(builder.ContainsKey("a"));
            Assert.False(builder.ContainsKey("A"));

            builder.KeyComparer = StringComparer.OrdinalIgnoreCase;
            Assert.Same(StringComparer.OrdinalIgnoreCase, builder.KeyComparer);
            Assert.Equal(2, builder.Count);
            Assert.True(builder.ContainsKey("a"));
            Assert.True(builder.ContainsKey("A"));
            Assert.True(builder.ContainsKey("b"));

            var set = builder.ToImmutable();
            Assert.Same(StringComparer.OrdinalIgnoreCase, set.KeyComparer);
            Assert.True(set.ContainsKey("a"));
            Assert.True(set.ContainsKey("A"));
            Assert.True(set.ContainsKey("b"));
        }

        [Fact]
        public void KeyComparerCollisions()
        {
            // First check where collisions have matching values.
            var builder = ImmutableSortedDictionary.Create<string, string>()
                .Add("a", "1").Add("A", "1").ToBuilder();
            builder.KeyComparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal(1, builder.Count);
            Assert.True(builder.ContainsKey("a"));

            var set = builder.ToImmutable();
            Assert.Same(StringComparer.OrdinalIgnoreCase, set.KeyComparer);
            Assert.Equal(1, set.Count);
            Assert.True(set.ContainsKey("a"));

            // Now check where collisions have conflicting values.
            builder = ImmutableSortedDictionary.Create<string, string>()
                .Add("a", "1").Add("A", "2").Add("b", "3").ToBuilder();
            AssertExtensions.Throws<ArgumentException>(null, () => builder.KeyComparer = StringComparer.OrdinalIgnoreCase);

            // Force all values to be considered equal.
            builder.ValueComparer = EverythingEqual<string>.Default;
            Assert.Same(EverythingEqual<string>.Default, builder.ValueComparer);
            builder.KeyComparer = StringComparer.OrdinalIgnoreCase; // should not throw because values will be seen as equal.
            Assert.Equal(2, builder.Count);
            Assert.True(builder.ContainsKey("a"));
            Assert.True(builder.ContainsKey("b"));
        }

        [Fact]
        public void KeyComparerEmptyCollection()
        {
            var builder = ImmutableSortedDictionary.Create<string, string>()
                .Add("a", "1").Add("B", "1").ToBuilder();
            Assert.Same(Comparer<string>.Default, builder.KeyComparer);
            builder.KeyComparer = StringComparer.OrdinalIgnoreCase;
            Assert.Same(StringComparer.OrdinalIgnoreCase, builder.KeyComparer);
            var set = builder.ToImmutable();
            Assert.Same(StringComparer.OrdinalIgnoreCase, set.KeyComparer);
        }

        [Fact]
        public void GetValueOrDefaultOfConcreteType()
        {
            var empty = ImmutableSortedDictionary.Create<string, int>().ToBuilder();
            var populated = ImmutableSortedDictionary.Create<string, int>().Add("a", 5).ToBuilder();
            Assert.Equal(0, empty.GetValueOrDefault("a"));
            Assert.Equal(1, empty.GetValueOrDefault("a", 1));
            Assert.Equal(5, populated.GetValueOrDefault("a"));
            Assert.Equal(5, populated.GetValueOrDefault("a", 1));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Cannot do DebuggerAttribute testing on UapAot: requires internal Reflection on framework types.")]
        public void DebuggerAttributesValid()
        {
            DebuggerAttributes.ValidateDebuggerDisplayReferences(ImmutableSortedDictionary.CreateBuilder<string, int>());
            ImmutableSortedDictionary<int, string>.Builder builder = ImmutableSortedDictionary.CreateBuilder<int, string>();
            builder.Add(1, "One");
            builder.Add(2, "Two");
            DebuggerAttributeInfo info = DebuggerAttributes.ValidateDebuggerTypeProxyProperties(builder);
            PropertyInfo itemProperty = info.Properties.Single(pr => pr.GetCustomAttribute<DebuggerBrowsableAttribute>().State == DebuggerBrowsableState.RootHidden);
            KeyValuePair<int, string>[] items = itemProperty.GetValue(info.Instance) as KeyValuePair<int, string>[];
            Assert.Equal(builder, items);
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Cannot do DebuggerAttribute testing on UapAot: requires internal Reflection on framework types.")]
        public static void TestDebuggerAttributes_Null()
        {
            Type proxyType = DebuggerAttributes.GetProxyType(ImmutableSortedDictionary.CreateBuilder<int, string>());
            TargetInvocationException tie = Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(proxyType, (object)null));
            Assert.IsType<ArgumentNullException>(tie.InnerException);
        }

        [Fact]
        public void ValueRef()
        {
            var builder = new Dictionary<string, int>()
            {
                { "a", 1 },
                { "b", 2 }
            }.ToImmutableSortedDictionary().ToBuilder();

            ref readonly var safeRef = ref builder.ValueRef("a");
            ref var unsafeRef = ref Unsafe.AsRef(safeRef);

            Assert.Equal(1, builder.ValueRef("a"));

            unsafeRef = 5;

            Assert.Equal(5, builder.ValueRef("a"));
        }

        [Fact]
        public void ValueRef_NonExistentKey()
        {
            var builder = new Dictionary<string, int>()
            {
                { "a", 1 },
                { "b", 2 }
            }.ToImmutableSortedDictionary().ToBuilder();

            Assert.Throws<KeyNotFoundException>(() => builder.ValueRef("c"));
        }

        [Fact]
        public void ToImmutableSortedDictionary()
        {
            ImmutableSortedDictionary<int, int>.Builder builder = ImmutableSortedDictionary.CreateBuilder<int, int>();
            builder.Add(1, 1);
            builder.Add(2, 2);
            builder.Add(3, 3);

            var dictionary = builder.ToImmutableSortedDictionary();
            Assert.Equal(1, dictionary[1]);
            Assert.Equal(2, dictionary[2]);
            Assert.Equal(3, dictionary[3]);

            builder[2] = 5;
            Assert.Equal(5, builder[2]);
            Assert.Equal(2, dictionary[2]);

            builder.Clear();
            Assert.True(builder.ToImmutableSortedDictionary().IsEmpty);
            Assert.False(dictionary.IsEmpty);

            ImmutableSortedDictionary<int, int>.Builder nullBuilder = null;
            AssertExtensions.Throws<ArgumentNullException>("builder", () => nullBuilder.ToImmutableSortedDictionary());
        }

        protected override IImmutableDictionary<TKey, TValue> GetEmptyImmutableDictionary<TKey, TValue>()
        {
            return ImmutableSortedDictionary.Create<TKey, TValue>();
        }

        protected override IImmutableDictionary<string, TValue> Empty<TValue>(StringComparer comparer)
        {
            return ImmutableSortedDictionary.Create<string, TValue>(comparer);
        }

        protected override bool TryGetKeyHelper<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey equalKey, out TKey actualKey)
        {
            return ((ImmutableSortedDictionary<TKey, TValue>.Builder)dictionary).TryGetKey(equalKey, out actualKey);
        }

        protected override IDictionary<TKey, TValue> GetBuilder<TKey, TValue>(IImmutableDictionary<TKey, TValue> basis)
        {
            return ((ImmutableSortedDictionary<TKey, TValue>)(basis ?? GetEmptyImmutableDictionary<TKey, TValue>())).ToBuilder();
        }
    }
}
