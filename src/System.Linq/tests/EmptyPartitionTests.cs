// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class EmptyPartitionTests
    {
        private static IEnumerable<T> GetEmptyPartition<T>()
        {
            return new T[0].Take(0);
        }

        [Fact]
        public void EmptyPartitionIsEmpty()
        {
            Assert.Empty(GetEmptyPartition<int>());
            Assert.Empty(GetEmptyPartition<string>());
        }

        [Fact]
        public void SingleInstance()
        {
            // .NET Core returns the instance as an optimization.
            // see https://github.com/dotnet/corefx/pull/2401.
            Assert.Equal(!PlatformDetection.IsFullFramework, ReferenceEquals(GetEmptyPartition<int>(), GetEmptyPartition<int>()));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Netcoreapp, ".NET Core returns the instance as an optimization")]
        public void SkipSame()
        {
            IEnumerable<int> empty = GetEmptyPartition<int>();
            Assert.Same(empty, empty.Skip(2));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Netcoreapp, ".NET Core returns the instance as an optimization")]
        public void TakeSame()
        {
            IEnumerable<int> empty = GetEmptyPartition<int>();
            Assert.Same(empty, empty.Take(2));
        }

        [Fact]
        public void ElementAtThrows()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => GetEmptyPartition<int>().ElementAt(0));
        }

        [Fact]
        public void ElementAtOrDefaultIsDefault()
        {
            Assert.Equal(0, GetEmptyPartition<int>().ElementAtOrDefault(0));
            Assert.Equal(null, GetEmptyPartition<string>().ElementAtOrDefault(0));
        }

        [Fact]
        public void FirstThrows()
        {
            Assert.Throws<InvalidOperationException>(() => GetEmptyPartition<int>().First());
        }

        [Fact]
        public void FirstOrDefaultIsDefault()
        {
            Assert.Equal(0, GetEmptyPartition<int>().FirstOrDefault());
            Assert.Equal(null, GetEmptyPartition<string>().FirstOrDefault());
        }

        [Fact]
        public void LastThrows()
        {
            Assert.Throws<InvalidOperationException>(() => GetEmptyPartition<int>().Last());
        }

        [Fact]
        public void LastOrDefaultIsDefault()
        {
            Assert.Equal(0, GetEmptyPartition<int>().LastOrDefault());
            Assert.Equal(null, GetEmptyPartition<string>().LastOrDefault());
        }

        [Fact]
        public void ToArrayEmpty()
        {
            Assert.Empty(GetEmptyPartition<int>().ToArray());
        }

        [Fact]
        public void ToListEmpty()
        {
            Assert.Empty(GetEmptyPartition<int>().ToList());
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "netfx's Take returns a compiler-generated iterator whose Reset throws.")]
        [Fact]
        public void ResetIsNop()
        {
            IEnumerator<int> en = GetEmptyPartition<int>().GetEnumerator();
            en.Reset();
            en.Reset();
            en.Reset();
        }

        [Fact]
        public void ICollection_IsReadOnly()
        {
            var emptyPartition = GetEmptyPartition<int>() as ICollection<int>;

            Assert.Equal(true, emptyPartition.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => emptyPartition.Add(0));
            Assert.Throws<NotSupportedException>(() => emptyPartition.Remove(0));
            Assert.Throws<NotSupportedException>(() => emptyPartition.Clear());
        }

        [Fact]
        public void ICollection_Count()
        {
            var emptyPartition = GetEmptyPartition<int>() as ICollection<int>;

            Assert.Equal(0, emptyPartition.Count);
        }

        [Fact]
        public void ICollection_CopyTo_ProduceCorrectSequence()
        {
            var emptyPartition = GetEmptyPartition<int>() as ICollection<int>;

            var arrayIndex = 5;
            var array = Enumerable.Range(0, 10).ToArray();
            emptyPartition.CopyTo(array, arrayIndex);
            
            for (var index = 0; index < 10; index++)
            {
                Assert.Equal(index, array[index]);
            }
        }

        [Fact]
        public void ICollection_Contains()
        {
            var emptyPartition = GetEmptyPartition<int>() as ICollection<int>;

            Assert.Equal(false, emptyPartition.Contains(int.MinValue));
            Assert.Equal(false, emptyPartition.Contains(0));
            Assert.Equal(false, emptyPartition.Contains(int.MaxValue));
        }

        [Fact]
        public void IList_IsReadOnly()
        {
            var emptyPartition = GetEmptyPartition<int>() as IList<int>;

            Assert.Throws<NotSupportedException>(() => emptyPartition.Insert(0, 0));
            Assert.Throws<NotSupportedException>(() => emptyPartition.RemoveAt(0));
        }

        [Fact]
        public void IList_Indexer_ThrowExceptionOnOutOfRangeIndex()
        {
            var emptyPartition = GetEmptyPartition<int>() as IList<int>;

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => emptyPartition[int.MinValue]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => emptyPartition[0]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => emptyPartition[int.MaxValue]);
        }

        [Fact]
        public void IList_IndexOf()
        {
            var emptyPartition = GetEmptyPartition<int>() as IList<int>;

            Assert.Equal(-1, emptyPartition.IndexOf(int.MinValue));
            Assert.Equal(-1, emptyPartition.IndexOf(0));
            Assert.Equal(-1, emptyPartition.IndexOf(int.MaxValue));
        }    
    }
}
