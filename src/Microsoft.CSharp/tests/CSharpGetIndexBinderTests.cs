// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class CSharpGetIndexBinderTests
    {
        [Fact]
        public void ArrayIndexing()
        {
            dynamic d = new[] { 0, 1, 2, 3 };
            for (int i = 0; i != 4; ++i)
            {
                Assert.Equal(i, d[i]);
            }
        }

        [Fact]
        public void ListIndexing()
        {
            dynamic d = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i != 4; ++i)
            {
                Assert.Equal(i, d[i]);
            }
        }

        [Fact]
        public void MultiDimensionalIndexing()
        {
            dynamic d = new[,] { { 0, 1, 2, 3 }, { 1, 2, 3, 4 }, { 2, 3, 4, 5 }, { 3, 4, 5, 6 } };
            for (int i = 0; i != 4; ++i)
            {
                for (int j = 0; j != 4; ++j)
                {
                    Assert.Equal(i + j, d[i, j]);
                }
            }
        }

        [Fact]
        public void NotIndexable()
        {
            dynamic d = 23;
            Assert.Throws<RuntimeBinderException>(() => d[2]);
        }

        [Fact]
        public void TooFewIndices()
        {
            dynamic d = new[,] { { 0, 1, 2, 3 }, { 1, 2, 3, 4 }, { 2, 3, 4, 5 }, { 3, 4, 5, 6 } };
            Assert.Throws<RuntimeBinderException>(() => d[2]);
        }

        [Fact]
        public void TooManyIndices()
        {
            dynamic d = new[] { 0, 1, 2, 3 };
            Assert.Throws<RuntimeBinderException>(() => d[1, 3]);
        }
    }
}
