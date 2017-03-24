// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void CannotCallGetHashCodeOnSpan()
        {
            Span<int> span = new Span<int>(new int[0]);

            try
            {
#pragma warning disable 0618
                int result = span.GetHashCode();
#pragma warning restore 0618
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(ex is NotSupportedException);
            }
        }
    }
}
