// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ToString1()
        {
            int[] a = { 91, 92, 93 };
            ReadOnlySpan<int> span = new ReadOnlySpan<int>(a);
            Assert.Equal("System.String[3]", span.ToString());
        }

        [Fact]
        public static void ToString_Empty()
        {
            ReadOnlySpan<int> span = new ReadOnlySpan<int>();
            Assert.Equal("System.String[0]", span.ToString());
        }
    }
}
