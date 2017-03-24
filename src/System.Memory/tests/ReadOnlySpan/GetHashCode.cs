// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void CannotCallGetHashCodeOnSpan()
        {
            ReadOnlySpan<int> span = new ReadOnlySpan<int>(new int[0]);

#pragma warning disable 0618
            Assert.Throws<NotSupportedException>(() => span.GetHashCode());
#pragma warning restore 0618
        }
    }
}
