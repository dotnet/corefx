// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq.Parallel.Tests
{
    internal static class Sources
    {
        // For outerloop, it's more important to saturate the processors/consumers and fill up buffers.
        public static readonly int OuterLoopCount = 64 * 1024 * Environment.ProcessorCount;

        private static readonly IEnumerable<int> OuterLoopCounts = new[] { OuterLoopCount };

        public static IEnumerable<object[]> Ranges(int start, IEnumerable<int> counts)
        {
            foreach (object[] parms in UnorderedSources.Ranges(start, counts))
            {
                yield return new object[] { ((Labeled<ParallelQuery<int>>)parms[0]).Order(), parms[1] };
            }
        }

        // Wrapper for attribute calls
        public static IEnumerable<object[]> OuterloopRanges()
        {
            foreach (object[] parms in Ranges(OuterLoopCounts)) yield return parms;
        }

        public static IEnumerable<object[]> Ranges(int[] counts)
        {
            foreach (object[] parms in Ranges(counts.Cast<int>())) yield return parms;
        }

        public static IEnumerable<object[]> Ranges(IEnumerable<int> counts)
        {
            foreach (object[] parms in Ranges(0, counts)) yield return parms;
        }

        public static IEnumerable<object[]> Ranges<T>(IEnumerable<int> counts, params Func<int, IEnumerable<T>>[] modifiers)
        {
            foreach (object[] parms in UnorderedSources.Ranges(counts, modifiers))
            {
                parms[0] = ((Labeled<ParallelQuery<int>>)parms[0]).Order();
                yield return parms;
            }
        }
    }
}
