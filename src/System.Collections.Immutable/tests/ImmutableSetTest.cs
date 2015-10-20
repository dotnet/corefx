// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Xunit;
using SetTriad = System.Tuple<System.Collections.Generic.IEnumerable<int>, System.Collections.Generic.IEnumerable<int>, bool>;

namespace System.Collections.Immutable.Tests
{
    public abstract class ImmutableSetTest : ImmutablesTestBase
    {
        [Fact]
        public void AddTest()
        {
            this.AddTestHelper(this.Empty<int>(), 3, 5, 4, 3);
        }

        [Fact]
        public void AddDuplicatesTest()
        {
            var arrayWithDuplicates = Enumerable.Range(1, 100).Concat(Enumerable.Range(1, 100)).ToArray();
            this.AddTestHelper(this.Empty<int>(), arrayWithDuplicates);
        }

        [Fact]
        public void RemoveTest()
        {
            this.RemoveTestHelper(this.Empty<int>().Add(3).Add(5), 5, 3);
        }

        [Fact]
        public void AddRemoveLoadTest()
        {
            var data = this.GenerateDummyFillData();
            this.AddRemoveLoadTestHelper(Empty<double>(), data);
        }

        [Fact]
        public void RemoveNonExistingTest()
        {
            this.RemoveNonExistingTest(this.Empty<int>());
        }

        [Fact]
        public void AddBulkFromImmutableToEmpty()
        {
            var set = this.Empty<int>().Add(5);
            var empty2 = this.Empty<int>();
            Assert.Same(set, empty2.Union(set)); // "Filling an empty immutable set with the contents of another immutable set with the exact same comparer should return the other set."
        }

        [Fact]
        public void ExceptTest()
        {
            this.ExceptTestHelper(Empty<int>().Add(1).Add(3).Add(5).Add(7), 3, 7);
        }

        /// <summary>
        /// Verifies that Except *does* enumerate its argument if the collection is empty.
        /// </summary>
        /// <remarks>
        /// While this would seem an implementation detail and simply lack of an optimization,
        /// it turns out that changing this behavior now *could* represent a breaking change
        /// because if the enumerable were to throw an exception, that exception would be
        /// observed previously, but would no longer be thrown if this behavior changed.
        /// So this is a test to lock the behavior in place or be thoughtful if adding the optimization.
        /// </remarks>
        /// <seealso cref="ImmutableListTest.RemoveRangeDoesNotEnumerateSequenceIfThisIsEmpty"/>
        [Fact]
        public void ExceptDoesEnumerateSequenceIfThisIsEmpty()
        {
            bool enumerated = false;
            Empty<int>().Except(Enumerable.Range(1, 1).Select(n => { enumerated = true; return n; }));
            Assert.True(enumerated);
        }

        [Fact]
        public void SymmetricExceptTest()
        {
            this.SymmetricExceptTestHelper(Empty<int>().Add(1).Add(3).Add(5).Add(7), Enumerable.Range(0, 9).ToArray());
            this.SymmetricExceptTestHelper(Empty<int>().Add(1).Add(3).Add(5).Add(7), Enumerable.Range(0, 5).ToArray());
        }

        [Fact]
        public void EnumeratorTest()
        {
            IComparer<double> comparer = null;
            var set = this.Empty<double>();
            var sortedSet = set as ISortKeyCollection<double>;
            if (sortedSet != null)
            {
                comparer = sortedSet.KeyComparer;
            }

            this.EnumeratorTestHelper(set, comparer, 3, 5, 1);
            double[] data = this.GenerateDummyFillData();
            this.EnumeratorTestHelper(set, comparer, data);
        }

        [Fact]
        public void IntersectTest()
        {
            this.IntersectTestHelper(Empty<int>().Union(Enumerable.Range(1, 10)), 8, 3, 5);
        }

        [Fact]
        public void UnionTest()
        {
            this.UnionTestHelper(this.Empty<int>(), new[] { 1, 3, 5, 7 });
            this.UnionTestHelper(this.Empty<int>().Union(new[] { 2, 4, 6 }), new[] { 1, 3, 5, 7 });
            this.UnionTestHelper(this.Empty<int>().Union(new[] { 1, 2, 3 }), new int[0] { });
            this.UnionTestHelper(this.Empty<int>().Union(new[] { 2 }), Enumerable.Range(0, 1000).ToArray());
        }

        [Fact]
        public void SetEqualsTest()
        {
            Assert.True(this.Empty<int>().SetEquals(this.Empty<int>()));
            var nonEmptySet = this.Empty<int>().Add(5);
            Assert.True(nonEmptySet.SetEquals(nonEmptySet));

            this.SetCompareTestHelper(s => s.SetEquals, s => s.SetEquals, this.GetSetEqualsScenarios());
        }

        [Fact]
        public void IsProperSubsetOfTest()
        {
            this.SetCompareTestHelper(s => s.IsProperSubsetOf, s => s.IsProperSubsetOf, this.GetIsProperSubsetOfScenarios());
        }

        [Fact]
        public void IsProperSupersetOfTest()
        {
            this.SetCompareTestHelper(s => s.IsProperSupersetOf, s => s.IsProperSupersetOf, this.GetIsProperSubsetOfScenarios().Select(Flip));
        }

        [Fact]
        public void IsSubsetOfTest()
        {
            this.SetCompareTestHelper(s => s.IsSubsetOf, s => s.IsSubsetOf, this.GetIsSubsetOfScenarios());
        }

        [Fact]
        public void IsSupersetOfTest()
        {
            this.SetCompareTestHelper(s => s.IsSupersetOf, s => s.IsSupersetOf, this.GetIsSubsetOfScenarios().Select(Flip));
        }

        [Fact]
        public void OverlapsTest()
        {
            this.SetCompareTestHelper(s => s.Overlaps, s => s.Overlaps, this.GetOverlapsScenarios());
        }

        [Fact]
        public void EqualsTest()
        {
            Assert.False(Empty<int>().Equals(null));
            Assert.False(Empty<int>().Equals("hi"));
            Assert.True(Empty<int>().Equals(Empty<int>()));
            Assert.False(Empty<int>().Add(3).Equals(Empty<int>().Add(3)));
            Assert.False(Empty<int>().Add(5).Equals(Empty<int>().Add(3)));
            Assert.False(Empty<int>().Add(3).Add(5).Equals(Empty<int>().Add(3)));
            Assert.False(Empty<int>().Add(3).Equals(Empty<int>().Add(3).Add(5)));
        }

        [Fact]
        public void GetHashCodeTest()
        {
            // verify that get hash code is the default address based one.
            Assert.Equal(EqualityComparer<object>.Default.GetHashCode(Empty<int>()), Empty<int>().GetHashCode());
        }

        [Fact]
        public void ClearTest()
        {
            var originalSet = this.Empty<int>();
            var nonEmptySet = originalSet.Add(5);
            var clearedSet = nonEmptySet.Clear();
            Assert.Same(originalSet, clearedSet);
        }

        [Fact]
        public void ISetMutationMethods()
        {
            var set = (ISet<int>)this.Empty<int>();
            Assert.Throws<NotSupportedException>(() => set.Add(0));
            Assert.Throws<NotSupportedException>(() => set.ExceptWith(null));
            Assert.Throws<NotSupportedException>(() => set.UnionWith(null));
            Assert.Throws<NotSupportedException>(() => set.IntersectWith(null));
            Assert.Throws<NotSupportedException>(() => set.SymmetricExceptWith(null));
        }

        [Fact]
        public void ICollectionOfTMembers()
        {
            var set = (ICollection<int>)this.Empty<int>();
            Assert.Throws<NotSupportedException>(() => set.Add(1));
            Assert.Throws<NotSupportedException>(() => set.Clear());
            Assert.Throws<NotSupportedException>(() => set.Remove(1));
            Assert.True(set.IsReadOnly);
        }

        [Fact]
        public void ICollectionMethods()
        {
            ICollection builder = (ICollection)this.Empty<string>();
            string[] array = new string[0];
            builder.CopyTo(array, 0);

            builder = (ICollection)this.Empty<string>().Add("a");
            array = new string[builder.Count + 1];

            builder.CopyTo(array, 1);
            Assert.Equal(new[] { null, "a" }, array);

            Assert.True(builder.IsSynchronized);
            Assert.NotNull(builder.SyncRoot);
            Assert.Same(builder.SyncRoot, builder.SyncRoot);
        }

        protected abstract bool IncludesGetHashCodeDerivative { get; }

        internal static List<T> ToListNonGeneric<T>(System.Collections.IEnumerable sequence)
        {
            Contract.Requires(sequence != null);
            var list = new List<T>();
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add((T)enumerator.Current);
            }

            return list;
        }

        protected abstract IImmutableSet<T> Empty<T>();

        protected abstract ISet<T> EmptyMutable<T>();

        internal abstract IBinaryTree GetRootNode<T>(IImmutableSet<T> set);

        protected void TryGetValueTestHelper(IImmutableSet<string> set)
        {
            Requires.NotNull(set, "set");

            string expected = "egg";
            set = set.Add(expected);
            string actual;
            string lookupValue = expected.ToUpperInvariant();
            Assert.True(set.TryGetValue(lookupValue, out actual));
            Assert.Same(expected, actual);

            Assert.False(set.TryGetValue("foo", out actual));
            Assert.Equal("foo", actual);

            Assert.False(set.Clear().TryGetValue("nonexistent", out actual));
            Assert.Equal("nonexistent", actual);
        }

        protected IImmutableSet<T> SetWith<T>(params T[] items)
        {
            return this.Empty<T>().Union(items);
        }

        protected void CustomSortTestHelper<T>(IImmutableSet<T> emptySet, bool matchOrder, T[] injectedValues, T[] expectedValues)
        {
            Contract.Requires(emptySet != null);
            Contract.Requires(injectedValues != null);
            Contract.Requires(expectedValues != null);

            var set = emptySet;
            foreach (T value in injectedValues)
            {
                set = set.Add(value);
            }

            Assert.Equal(expectedValues.Length, set.Count);
            if (matchOrder)
            {
                Assert.Equal<T>(expectedValues, set.ToList());
            }
            else
            {
                CollectionAssertAreEquivalent(expectedValues, set.ToList());
            }
        }

        /// <summary>
        /// Tests various aspects of a set.  This should be called only from the unordered or sorted overloads of this method.
        /// </summary>
        /// <typeparam name="T">The type of element stored in the set.</typeparam>
        /// <param name="emptySet">The empty set.</param>
        protected void EmptyTestHelper<T>(IImmutableSet<T> emptySet)
        {
            Contract.Requires(emptySet != null);

            Assert.Equal(0, emptySet.Count); //, "Empty set should have a Count of 0");
            Assert.Equal(0, emptySet.Count()); //, "Enumeration of an empty set yielded elements.");
            Assert.Same(emptySet, emptySet.Clear());
        }

        private IEnumerable<SetTriad> GetSetEqualsScenarios()
        {
            return new List<SetTriad>
            {
                new SetTriad(SetWith<int>(), new int[] { }, true),
                new SetTriad(SetWith<int>(5), new int[] { 5 }, true),
                new SetTriad(SetWith<int>(5), new int[] { 5, 5 }, true),
                new SetTriad(SetWith<int>(5, 8), new int[] { 5, 5 }, false),
                new SetTriad(SetWith<int>(5, 8), new int[] { 5, 7 }, false),
                new SetTriad(SetWith<int>(5, 8), new int[] { 5, 8 }, true),
                new SetTriad(SetWith<int>(5), new int[] { }, false),
                new SetTriad(SetWith<int>(), new int[] { 5 }, false),
                new SetTriad(SetWith<int>(5, 8), new int[] { 5 }, false),
                new SetTriad(SetWith<int>(5), new int[] { 5, 8 }, false),
                new SetTriad(SetWith<int>(5, 8), SetWith<int>(5, 8), true),
            };
        }

        private IEnumerable<SetTriad> GetIsProperSubsetOfScenarios()
        {
            return new List<SetTriad>
            {
                new SetTriad(new int[] { }, new int[] { }, false),
                new SetTriad(new int[] { 1 }, new int[] { }, false),
                new SetTriad(new int[] { 1 }, new int[] { 2 }, false),
                new SetTriad(new int[] { 1 }, new int[] { 2, 3 }, false),
                new SetTriad(new int[] { 1 }, new int[] { 1, 2 }, true),
                new SetTriad(new int[] { }, new int[] { 1 }, true),
            };
        }

        private IEnumerable<SetTriad> GetIsSubsetOfScenarios()
        {
            var results = new List<SetTriad>
            {
                new SetTriad(new int[] { }, new int[] { }, true),
                new SetTriad(new int[] { 1 }, new int[] { 1 }, true),
                new SetTriad(new int[] { 1, 2 }, new int[] { 1, 2 }, true),
                new SetTriad(new int[] { 1 }, new int[] { }, false),
                new SetTriad(new int[] { 1 }, new int[] { 2 }, false),
                new SetTriad(new int[] { 1 }, new int[] { 2, 3 }, false),
            };

            // By definition, any proper subset is also a subset.
            // But because a subset may not be a proper subset, we filter the proper- scenarios.
            results.AddRange(this.GetIsProperSubsetOfScenarios().Where(s => s.Item3));
            return results;
        }

        private IEnumerable<SetTriad> GetOverlapsScenarios()
        {
            return new List<SetTriad>
            {
                new SetTriad(new int[] { }, new int[] { }, false),
                new SetTriad(new int[] { }, new int[] { 1 }, false),
                new SetTriad(new int[] { 1 }, new int[] { 2 }, false),
                new SetTriad(new int[] { 1 }, new int[] { 2, 3 }, false),
                new SetTriad(new int[] { 1, 2 }, new int[] { 3 }, false),
                new SetTriad(new int[] { 1 }, new int[] { 1, 2 }, true),
                new SetTriad(new int[] { 1, 2 }, new int[] { 1 }, true),
                new SetTriad(new int[] { 1 }, new int[] { 1 }, true),
                new SetTriad(new int[] { 1, 2 }, new int[] { 2, 3, 4 }, true),
            };
        }

        private void SetCompareTestHelper<T>(Func<IImmutableSet<T>, Func<IEnumerable<T>, bool>> operation, Func<ISet<T>, Func<IEnumerable<T>, bool>> baselineOperation, IEnumerable<Tuple<IEnumerable<T>, IEnumerable<T>, bool>> scenarios)
        {
            //const string message = "Scenario #{0}: Set 1: {1}, Set 2: {2}";

            int iteration = 0;
            foreach (var scenario in scenarios)
            {
                iteration++;

                // Figure out the response expected based on the BCL mutable collections.
                var baselineSet = this.EmptyMutable<T>();
                baselineSet.UnionWith(scenario.Item1);
                var expectedFunc = baselineOperation(baselineSet);
                bool expected = expectedFunc(scenario.Item2);
                Assert.Equal(expected, scenario.Item3); //, "Test scenario has an expected result that is inconsistent with BCL mutable collection behavior.");

                var actualFunc = operation(this.SetWith(scenario.Item1.ToArray()));
                var args = new object[] { iteration, ToStringDeferred(scenario.Item1), ToStringDeferred(scenario.Item2) };
                Assert.Equal(scenario.Item3, actualFunc(this.SetWith(scenario.Item2.ToArray()))); //, message, args);
                Assert.Equal(scenario.Item3, actualFunc(scenario.Item2)); //, message, args);
            }
        }

        private static Tuple<IEnumerable<T>, IEnumerable<T>, bool> Flip<T>(Tuple<IEnumerable<T>, IEnumerable<T>, bool> scenario)
        {
            return new Tuple<IEnumerable<T>, IEnumerable<T>, bool>(scenario.Item2, scenario.Item1, scenario.Item3);
        }

        private void RemoveTestHelper<T>(IImmutableSet<T> set, params T[] values)
        {
            Contract.Requires(set != null);
            Contract.Requires(values != null);

            Assert.Same(set, set.Except(Enumerable.Empty<T>()));

            int initialCount = set.Count;
            int removedCount = 0;
            foreach (T value in values)
            {
                var nextSet = set.Remove(value);
                Assert.NotSame(set, nextSet);
                Assert.Equal(initialCount - removedCount, set.Count);
                Assert.Equal(initialCount - removedCount - 1, nextSet.Count);

                Assert.Same(nextSet, nextSet.Remove(value)); //, "Removing a non-existing element should not change the set reference.");
                removedCount++;
                set = nextSet;
            }

            Assert.Equal(initialCount - removedCount, set.Count);
        }

        private void RemoveNonExistingTest(IImmutableSet<int> emptySet)
        {
            Assert.Same(emptySet, emptySet.Remove(5));

            // Also fill up a set with many elements to build up the tree, then remove from various places in the tree.
            const int Size = 200;
            var set = emptySet;
            for (int i = 0; i < Size; i += 2)
            { // only even numbers!
                set = set.Add(i);
            }

            // Verify that removing odd numbers doesn't change anything.
            for (int i = 1; i < Size; i += 2)
            {
                var setAfterRemoval = set.Remove(i);
                Assert.Same(set, setAfterRemoval);
            }
        }

        private void AddRemoveLoadTestHelper<T>(IImmutableSet<T> set, T[] data)
        {
            Contract.Requires(set != null);
            Contract.Requires(data != null);

            foreach (T value in data)
            {
                var newSet = set.Add(value);
                Assert.NotSame(set, newSet);
                set = newSet;
            }

            foreach (T value in data)
            {
                Assert.True(set.Contains(value));
            }

            foreach (T value in data)
            {
                var newSet = set.Remove(value);
                Assert.NotSame(set, newSet);
                set = newSet;
            }
        }

        protected void EnumeratorTestHelper<T>(IImmutableSet<T> emptySet, IComparer<T> comparer, params T[] values)
        {
            var set = emptySet;
            foreach (T value in values)
            {
                set = set.Add(value);
            }

            var nonGenericEnumerableList = ToListNonGeneric<T>(set);
            CollectionAssertAreEquivalent(nonGenericEnumerableList, values);

            var list = set.ToList();
            CollectionAssertAreEquivalent(list, values);

            if (comparer != null)
            {
                Array.Sort(values, comparer);
                Assert.Equal<T>(values, list);
            }

            // Apply some less common uses to the enumerator to test its metal.
            IEnumerator<T> enumerator;
            using (enumerator = set.GetEnumerator())
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
                enumerator.Reset(); // reset isn't usually called before MoveNext
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
                ManuallyEnumerateTest(list, enumerator);
                Assert.False(enumerator.MoveNext()); // call it again to make sure it still returns false

                enumerator.Reset();
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
                ManuallyEnumerateTest(list, enumerator);
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // this time only partially enumerate
                enumerator.Reset();
                enumerator.MoveNext();
                enumerator.Reset();
                ManuallyEnumerateTest(list, enumerator);
            }

            Assert.Throws<ObjectDisposedException>(() => enumerator.Reset());
            Assert.Throws<ObjectDisposedException>(() => enumerator.MoveNext());
            Assert.Throws<ObjectDisposedException>(() => enumerator.Current);
        }

        private void ExceptTestHelper<T>(IImmutableSet<T> set, params T[] valuesToRemove)
        {
            Contract.Requires(set != null);
            Contract.Requires(valuesToRemove != null);

            var expectedSet = new HashSet<T>(set);
            expectedSet.ExceptWith(valuesToRemove);

            var actualSet = set.Except(valuesToRemove);
            CollectionAssertAreEquivalent(expectedSet.ToList(), actualSet.ToList());

            this.VerifyAvlTreeState(actualSet);
        }

        private void SymmetricExceptTestHelper<T>(IImmutableSet<T> set, params T[] otherCollection)
        {
            Contract.Requires(set != null);
            Contract.Requires(otherCollection != null);

            var expectedSet = new HashSet<T>(set);
            expectedSet.SymmetricExceptWith(otherCollection);

            var actualSet = set.SymmetricExcept(otherCollection);
            CollectionAssertAreEquivalent(expectedSet.ToList(), actualSet.ToList());

            this.VerifyAvlTreeState(actualSet);
        }

        private void IntersectTestHelper<T>(IImmutableSet<T> set, params T[] values)
        {
            Contract.Requires(set != null);
            Contract.Requires(values != null);

            Assert.True(set.Intersect(Enumerable.Empty<T>()).Count == 0);

            var expected = new HashSet<T>(set);
            expected.IntersectWith(values);

            var actual = set.Intersect(values);
            CollectionAssertAreEquivalent(expected.ToList(), actual.ToList());

            this.VerifyAvlTreeState(actual);
        }

        private void UnionTestHelper<T>(IImmutableSet<T> set, params T[] values)
        {
            Contract.Requires(set != null);
            Contract.Requires(values != null);

            var expected = new HashSet<T>(set);
            expected.UnionWith(values);

            var actual = set.Union(values);
            CollectionAssertAreEquivalent(expected.ToList(), actual.ToList());

            this.VerifyAvlTreeState(actual);
        }

        private void AddTestHelper<T>(IImmutableSet<T> set, params T[] values)
        {
            Contract.Requires(set != null);
            Contract.Requires(values != null);

            Assert.Same(set, set.Union(Enumerable.Empty<T>()));

            int initialCount = set.Count;

            var uniqueValues = new HashSet<T>(values);
            var enumerateAddSet = set.Union(values);
            Assert.Equal(initialCount + uniqueValues.Count, enumerateAddSet.Count);
            foreach (T value in values)
            {
                Assert.True(enumerateAddSet.Contains(value));
            }

            int addedCount = 0;
            foreach (T value in values)
            {
                bool duplicate = set.Contains(value);
                var nextSet = set.Add(value);
                Assert.True(nextSet.Count > 0);
                Assert.Equal(initialCount + addedCount, set.Count);
                int expectedCount = initialCount + addedCount;
                if (!duplicate)
                {
                    expectedCount++;
                }
                Assert.Equal(expectedCount, nextSet.Count);
                Assert.Equal(duplicate, set.Contains(value));
                Assert.True(nextSet.Contains(value));
                if (!duplicate)
                {
                    addedCount++;
                }

                // Next assert temporarily disabled because Roslyn's set doesn't follow this rule.
                Assert.Same(nextSet, nextSet.Add(value)); //, "Adding duplicate value {0} should keep the original reference.", value);
                set = nextSet;
            }
        }

        private void VerifyAvlTreeState<T>(IImmutableSet<T> set)
        {
            var rootNode = this.GetRootNode(set);
            rootNode.VerifyBalanced();
            rootNode.VerifyHeightIsWithinTolerance(set.Count);
        }
    }
}
