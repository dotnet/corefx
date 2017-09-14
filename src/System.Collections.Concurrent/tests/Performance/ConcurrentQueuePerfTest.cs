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
        internal struct FastRandom // xorshift prng
        {
            private uint _w, _x, _y, _z;

            public FastRandom(int seed)
            {
                _x = (uint)seed;
                _w = 88675123;
                _y = 362436069;
                _z = 521288629;
            }

            public int Next(int maxValue)
            {
                uint t = _x ^ (_x << 11);
                _x = _y; _y = _z; _z = _w;
                _w = _w ^ (_w >> 19) ^ (t ^ (t >> 8));

                return (int)(_w % (uint)maxValue);
            }
        }

        /// <summary>
        /// Benchmark many thread enqueue/dequeue from ConcurrentQueue
        /// </summary>
        [Benchmark(InnerIterationCount = 10)]
        [InlineData(8,  100000)]
        [InlineData(50, 100000)]
        public void Enqueue_Dequeue(int threadCount, int itemsPerThread)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                var q = new ConcurrentQueue<int>();

                var startTest = new ManualResetEvent(false);

                var tasks = new Task[threadCount];
                for (int i = 0; i < tasks.Length; ++i)
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        var random = new FastRandom(i);

                        // Short Warmup
                        for (int j = 0; j < 10; j++)
                        {
                            int d;
                            q.Enqueue(random.Next(1024));
                            q.TryDequeue(out d);
                        }

                        startTest.WaitOne();

                        for (int j = 0; j < itemsPerThread; j++)
                        {
                            if (random.Next(1024) < 511) // Slight Dequeue bias
                            {
                                q.Enqueue(0);
                            }
                            else
                            {
                                int d;
                                q.TryDequeue(out d);
                            }
                        }
                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }

                using (iteration.StartMeasurement())
                {
                    startTest.Set();

                    Task.WaitAll(tasks);
                }
            }
        }
    }
}
