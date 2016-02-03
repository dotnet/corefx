// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    internal static class CollectionAsserts
    {
        public static void Equal(ICollection expected, ICollection actual)
        {
            Assert.Equal(expected == null, actual == null);
            if (expected == null)
            {
                return;
            }
            Assert.Equal(expected.Count, actual.Count);
            IEnumerator e = expected.GetEnumerator();
            IEnumerator a = actual.GetEnumerator();
            while (e.MoveNext())
            {
                Assert.True(a.MoveNext(), "actual has fewer elements");
                if (e.Current == null)
                {
                    Assert.Null(a.Current);
                }
                else
                {
                    Assert.IsType(e.Current.GetType(), a.Current);
                    Assert.Equal(e.Current, a.Current);
                }
            }
            Assert.False(a.MoveNext(), "actual has more elements");
        }

        public static void EqualUnordered(ICollection expected, ICollection actual)
        {
            Assert.Equal(expected == null, actual == null);
            if (expected == null)
            {
                return;
            }

            // Lookups are an aggregated collections (enumerable contents), but ordered.
            ILookup<object, object> e = expected.Cast<object>().ToLookup(key => key);
            ILookup<object, object> a = actual.Cast<object>().ToLookup(key => key);

            // Dictionaries can't handle null keys, which is a possibility
            Assert.Equal(e.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()), a.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()));

            // Get count of null keys.  Returns an empty sequence (and thus a 0 count) if no null key
            Assert.Equal(e[null].Count(), a[null].Count());
        }
    }
}
