// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Xunit
{
    public static class SpanAssert
    {
        public static void Equal<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, IEqualityComparer<T> comparer = null) where T : IEquatable<T>
        {
            if (comparer == null)
            {
                Assert.True(MemoryExtensions.SequenceEqual(a, b));
            }
            else
            {
                Assert.Equal(a.Length, b.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    Assert.Equal(a[i], b[i], comparer);
                }
            }
        }

        public static void Equal<T>(Span<T> a, Span<T> b, IEqualityComparer<T> comparer = null) where T : IEquatable<T>
        {
            Equal<T>((ReadOnlySpan<T>)a, (ReadOnlySpan<T>)b, comparer);
        }
    }
}
