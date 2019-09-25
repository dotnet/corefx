// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class CollectionsMarshalTests
    {
        [Fact]
        public void ListAsSpan()
        {
            var list = new List<int>();
            foreach (var length in Enumerable.Range(0, 36))
            {
                list.Clear();
                ValidateContentEquality(list, CollectionsMarshal.AsSpan(list));

                for (var i = 0; i < length; i++)
                {
                    list.Add(i);
                }
                ValidateContentEquality(list, CollectionsMarshal.AsSpan(list));

                list.TrimExcess();
                ValidateContentEquality(list, CollectionsMarshal.AsSpan(list));

                list.Add(length + 1);
                ValidateContentEquality(list, CollectionsMarshal.AsSpan(list));
            }
        }

        private static void ValidateContentEquality<T>(List<T> list, Span<T> span)
        {
            Assert.Equal(list.Count, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                Assert.Equal(list[i], span[i]);
            }
        }
    }
}
