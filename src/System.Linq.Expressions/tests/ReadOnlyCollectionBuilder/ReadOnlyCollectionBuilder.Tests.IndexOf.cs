// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Tests;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class ReadOnlyCollectionBuilder_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        #region Helpers

        public delegate int IndexOfDelegate(ReadOnlyCollectionBuilder<T> list, T value);
        public enum IndexOfMethod
        {
            IndexOf_T,
        };

        private IndexOfDelegate IndexOfDelegateFromType(IndexOfMethod methodType)
        {
            switch (methodType)
            {
                case (IndexOfMethod.IndexOf_T):
                    return ((ReadOnlyCollectionBuilder<T> list, T value) => { return list.IndexOf(value); });
                default:
                    throw new Exception("Invalid IndexOfMethod");
            }
        }

        /// <summary>
        /// MemberData for a Theory to test the IndexOf methods for List. To avoid high code reuse of tests for the 6 IndexOf
        /// methods in List, delegates are used to cover the basic behavioral cases shared by all IndexOf methods. A bool
        /// is used to specify the ordering (front-to-back or back-to-front (e.g. LastIndexOf)) that the IndexOf method
        /// searches in.
        /// </summary>
        public static IEnumerable<object[]> IndexOfTestData()
        {
            foreach (object[] sizes in ValidCollectionSizes())
            {
                int count = (int)sizes[0];
                yield return new object[] { IndexOfMethod.IndexOf_T, count, true };

                if (count > 0) // 0 is an invalid index for IndexOf when the count is 0.
                {
                }
            }
        }

        #endregion

        #region IndexOf

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_NoDuplicates(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            ReadOnlyCollectionBuilder<T> list = GenericListFactory(count);
            List<T> expectedList = list.ToList();
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);

            Assert.All(Enumerable.Range(0, count), i =>
            {
                Assert.Equal(i, IndexOf(list, expectedList[i]));
            });
        }

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_NonExistingValues(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            ReadOnlyCollectionBuilder<T> list = GenericListFactory(count);
            IEnumerable<T> nonexistentValues = CreateEnumerable(EnumerableType.List, list, count: count, numberOfMatchingElements: 0, numberOfDuplicateElements: 0);
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);

            Assert.All(nonexistentValues, nonexistentValue =>
            {
                Assert.Equal(-1, IndexOf(list, nonexistentValue));
            });
        }

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_DefaultValue(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            T defaultValue = default(T);
            ReadOnlyCollectionBuilder<T> list = GenericListFactory(count);
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);
            while (list.Remove(defaultValue))
                count--;
            list.Add(defaultValue);
            Assert.Equal(count, IndexOf(list, defaultValue));
        }

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_OrderIsCorrect(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            ReadOnlyCollectionBuilder<T> list = GenericListFactory(count);
            List<T> withoutDuplicates = list.ToList();
            AddRange(list, withoutDuplicates);
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);

            Assert.All(Enumerable.Range(0, count), i =>
            {
                if (frontToBackOrder)
                    Assert.Equal(i, IndexOf(list, withoutDuplicates[i]));
                else
                    Assert.Equal(count + i, IndexOf(list, withoutDuplicates[i]));
            });
        }

        #endregion
    }
}
