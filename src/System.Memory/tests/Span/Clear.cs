// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void Clear()
        {
            int[] actual = { 1, 2, 3 };
            int[] expected = { 0, 0, 0 };

            Span<int> span = new Span<int>(actual);
            span.Clear();
            Assert.Equal<int>(expected, actual);
        }
    }
}
