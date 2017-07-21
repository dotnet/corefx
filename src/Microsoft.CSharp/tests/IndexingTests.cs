// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class IndexingTests
    {
        private class AllTheIntegers
        {
            [IndexerName("Integers")]
            public int this[int x] => x;
        }

        [Fact]
        public void CustomIndexerName()
        {
            dynamic d = new AllTheIntegers();
            int answer = d[42];
            Assert.Equal(42, answer);
        }
    }
}
