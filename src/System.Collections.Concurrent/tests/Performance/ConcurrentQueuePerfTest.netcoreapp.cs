// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Collections.Concurrent.Tests
{
    public class Perf_ConcurrentQueueTests
    {
        /// <summary>
        /// Creates a list containing a number of elements equal to the specified size
        /// </summary>
        [Benchmark]
        [InlineData(8, 1000)]
        [InlineData(50, 1000)]
        public void Enqueue_Dequeue(int threadsCount, int itemsPerThread)
        {
            var q = new ConcurrentQueue<int>();
            Task.WaitAll((from i in Enumerable.Range(0, threadsCount) select Task.Run(() =>
            {
                var random = new Random();
                for (int j = 0; j < itemsPerThread; j++)
                {
                    switch (random.Next(2))
                    {
                        case 0:
                            q.Enqueue(random.Next(int.MaxValue));
                            break;
                        case 1:
                            int d;
                            q.TryDequeue(out d);
                            break;
                    }
                }
            })).ToArray());
        }
    }
}
