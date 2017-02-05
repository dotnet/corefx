// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Tests;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public abstract partial class ReadOnlyCollectionBuilder_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        #region IList<T> Helper Methods

        protected override IList<T> GenericIListFactory()
        {
            return GenericListFactory();
        }

        protected override IList<T> GenericIListFactory(int count)
        {
            return GenericListFactory(count);
        }

        #endregion

        #region ReadOnlyCollectionBuilder<T> Helper Methods

        protected virtual ReadOnlyCollectionBuilder<T> GenericListFactory()
        {
            return new ReadOnlyCollectionBuilder<T>();
        }

        protected virtual ReadOnlyCollectionBuilder<T> GenericListFactory(int count)
        {
            IEnumerable<T> toCreateFrom = CreateEnumerable(EnumerableType.List, null, count, 0, 0);
            return new ReadOnlyCollectionBuilder<T>(toCreateFrom);
        }

        private static void AssertEmpty(ReadOnlyCollectionBuilder<T> builder)
        {
            Assert.Equal(0, builder.Count);

            Assert.False(builder.Contains(default(T)));
            Assert.False(builder.Remove(default(T)));
            Assert.InRange(builder.IndexOf(default(T)), int.MinValue, -1);

            using (IEnumerator<T> e = builder.GetEnumerator())
            {
                Assert.False(e.MoveNext());
            }
        }

        protected void AddRange(IList<T> dest, IList<T> src)
        {
            for (int i = 0, count = src.Count; i < count; i++)
                dest.Add(src[i]);
        }

        #endregion

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void CopyTo_ArgumentValidity(int count)
        {
            ReadOnlyCollectionBuilder<T> list = GenericListFactory(count);
            Assert.Throws<ArgumentException>(() => list.CopyTo(new T[0], count + 1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(new T[0], 1));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ToReadOnlyCollection(int count)
        {
            ReadOnlyCollectionBuilder<T> list = GenericListFactory(count);
            var items = list.ToArray();

            ReadOnlyCollection<T> collection = list.ToReadOnlyCollection();

            AssertEmpty(list); // ToReadOnlyCollection behavior is to empty the builder
            Assert.Equal(0, list.Capacity);

            Assert.Equal(items.Length, collection.Count);
            Assert.True(items.SequenceEqual(collection));   
        }
    }

    public class ReadOnlyCollectionBuilder_Generic_Tests_string : ReadOnlyCollectionBuilder_Generic_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public class ReadOnlyCollectionBuilder_Generic_Tests_int : ReadOnlyCollectionBuilder_Generic_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }
    }
}
