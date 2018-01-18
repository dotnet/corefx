// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace System.MemoryTests
{
    public static partial class MemoryTests
    {
        [Fact]
        public static void ToEnumerable()
        {
            int[] a = { 91, 92, 93 };
            var memory = new Memory<int>(a);
            IEnumerable<int> copy = memory.ToEnumerable();
            Assert.Equal<int>(a, copy);
        }

        [Fact]
        public static void ToEnumerableWithIndex()
        {
            int[] a = { 91, 92, 93, 94, 95 };
            var memory = new Memory<int>(a);
            IEnumerable<int> copy = memory.Slice(2).ToEnumerable();

            Assert.Equal<int>(new int[] { 93, 94, 95 }, copy);
        }

        [Fact]
        public static void ToEnumerableWithIndexAndLength()
        {
            int[] a = { 91, 92, 93 };
            var memory = new Memory<int>(a, 1, 1);
            IEnumerable<int> copy = memory.ToEnumerable();
            Assert.Equal<int>(new int[] { 92 }, copy);
        }

        [Fact]
        public static void ToEnumerableEmpty()
        {
            Memory<int> memory = Memory<int>.Empty;
            IEnumerable<int> copy = memory.ToEnumerable();
            Assert.Equal(0, copy.Count());
        }

        [Fact]
        public static void ToEnumerableDefault()
        {
            Memory<int> memory = default;
            IEnumerable<int> copy = memory.ToEnumerable();
            Assert.Equal(0, copy.Count());
        }

        [Fact]
        public static void ToEnumerableForEach()
        {
            int[] a = { 91, 92, 93 };
            var memory = new Memory<int>(a);
            int index = 0;
            foreach (int curr in memory.ToEnumerable())
            {
                Assert.Equal(a[index++], curr);
            }
        }

        [Fact]
        public static void ToEnumerableGivenToExistingConstructor()
        {
            int[] a = { 91, 92, 93 };
            var memory = new Memory<int>(a);
            IEnumerable<int> enumer = memory.ToEnumerable();
            var li = new List<int>(enumer);
            Assert.Equal(a, li);
        }

        [Fact]
        public static void ToEnumerableSameAsIEnumerator()
        {
            int[] a = { 91, 92, 93 };
            var memory = new Memory<int>(a);
            IEnumerable<int> enumer = memory.ToEnumerable();
            IEnumerator<int> enumerat = enumer.GetEnumerator();
            Assert.Same(enumer, enumerat);
        }
    }
}
