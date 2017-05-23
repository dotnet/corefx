// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        [Trait("AsReadOnlySpan", "True")]
        public static void ArrayAsReadOnlySpan()
        {
            int[] a = { 19, -17 };
            ReadOnlySpan<int> spanInt = a.AsReadOnlySpan();
            spanInt.Validate<int>(19, -17);

            long[] b = { 1, -3, 7, -15, 31 };
            ReadOnlySpan<long> spanLong = b.AsReadOnlySpan();
            spanLong.Validate<long>(1, -3, 7, -15, 31);

            object o1 = new object();
            object o2 = new object();
            object[] c = { o1, o2 };
            ReadOnlySpan<object> spanObject = c.AsReadOnlySpan();
            spanObject.Validate<object>(o1, o2);
        }

        [Fact]
        [Trait("AsReadOnlySpan", "True")]
        public static void NullArrayAsReadOnlySpan()
        {
            int[] a = null;
            ReadOnlySpan<int> span;
            AssertThrows<ArgumentNullException, int>(span, _span => _span = a.AsReadOnlySpan());
        }

        [Fact]
        [Trait("AsReadOnlySpan", "True")]
        public static void EmptyArrayAsReadOnlySpan()
        {
            int[] empty = Array.Empty<int>();
            ReadOnlySpan<int> span = empty.AsReadOnlySpan();
            span.Validate<int>();
        }
    }
}