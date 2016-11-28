// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Concurrent.Tests
{
    public partial class ConcurrentQueueTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1000)]
        public void Clear_CountMatch(int count)
        {
            var q = new ConcurrentQueue<int>(Enumerable.Range(1, count));
            q.Clear();
            Assert.Equal(0, q.Count);
            Assert.Equal(true, q.IsEmpty);
            Assert.Equal(Enumerable.Range(1, 0), q);
        }

        [Fact]
        public void Concurrent_Clear()
        {
            Concurrent_Clear(1, 10);
            Concurrent_Clear(3, 100);
            Concurrent_Clear(8, 1000);
        }

        public void Concurrent_Clear(int threadsCount, int itemsPerThread)
        {
            int countFailures = 0;
            int isEmptyFailures = 0;
            int enqueueFailures = 0;
            int toArrayFailures = 0;
            int tryDequeueFailures = 0;
            int tryPeekFailures = 0;
            int clearFailures = 0;
            var q = new ConcurrentQueue<int>();

            Task[] threads = new Task[threadsCount];

            var random = new Random();
            for (int i = 0; i < threadsCount; i++)
            {
                threads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        switch (random.Next(7))
                        {
                            case 0:
                                try { var ñ = q.Count; }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref countFailures);
                                    var ex = e;
                                }
                                break;
                            case 1:
                                try { var e = q.IsEmpty; }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref isEmptyFailures);
                                    var ex = e;
                                }
                                break;
                            case 2:
                                try { q.Enqueue(random.Next(int.MaxValue)); }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref enqueueFailures);
                                    var ex = e;
                                }
                                break;
                            case 3:
                                try { q.ToArray(); }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref toArrayFailures);
                                    var ex = e;
                                }
                                break;
                            case 4:
                                try { int v; q.TryDequeue(out v); }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref tryDequeueFailures);
                                    var ex = e;
                                }
                                break;
                            case 5:
                                try { int v; q.TryPeek(out v); }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref tryPeekFailures);
                                    var ex = e;
                                }
                                break;
                            case 6:
                                try { q.Clear(); }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref clearFailures);
                                    var ex = e;
                                }
                                break;
                        }
                    }
                });
            }
            Task.WaitAll(threads);

            Assert.Equal(0, countFailures);
            Assert.Equal(0, isEmptyFailures);
            Assert.Equal(0, enqueueFailures);
            Assert.Equal(0, toArrayFailures);
            Assert.Equal(0, tryDequeueFailures);
            Assert.Equal(0, tryPeekFailures);
            Assert.Equal(0, clearFailures);
        }
    }
}
