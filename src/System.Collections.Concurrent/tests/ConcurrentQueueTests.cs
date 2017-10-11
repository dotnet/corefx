// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Concurrent.Tests
{
    public partial class ConcurrentQueueTests : ProducerConsumerCollectionTests
    {
        protected override IProducerConsumerCollection<T> CreateProducerConsumerCollection<T>() => new ConcurrentQueue<T>();
        protected override IProducerConsumerCollection<int> CreateProducerConsumerCollection(IEnumerable<int> collection) => new ConcurrentQueue<int>(collection);
        protected override bool IsEmpty(IProducerConsumerCollection<int> pcc) => ((ConcurrentQueue<int>)pcc).IsEmpty;
        protected override bool TryPeek<T>(IProducerConsumerCollection<T> pcc, out T result) => ((ConcurrentQueue<T>)pcc).TryPeek(out result);
        protected override bool ResetImplemented => false;
        protected override IProducerConsumerCollection<int> CreateOracle(IEnumerable<int> collection) => new QueueOracle(collection);

        protected override string CopyToNoLengthParamName => null;

        [Fact]
        public void Concurrent_Enqueue_TryDequeue_AllItemsReceived()
        {
            int items = 1000;

            var q = new ConcurrentQueue<int>();

            // Consumer dequeues items until it gets as many as it expects
            Task consumer = Task.Run(() =>
            {
                int lastReceived = 0;
                int item;
                while (true)
                {
                    if (q.TryDequeue(out item))
                    {
                        Assert.Equal(lastReceived + 1, item);
                        lastReceived = item;
                        if (item == items) break;
                    }
                    else
                    {
                        Assert.Equal(0, item);
                    }
                }
            });

            // Producer queues the expected number of items
            Task producer = Task.Run(() =>
            {
                for (int i = 1; i <= items; i++) q.Enqueue(i);
            });

            Task.WaitAll(producer, consumer);
        }

        [Fact]
        public void Concurrent_Enqueue_TryPeek_TryDequeue_AllItemsSeen()
        {
            int items = 1000;

            var q = new ConcurrentQueue<int>();

            // Consumer peeks and then dequeues the expected number of items
            Task consumer = Task.Run(() =>
            {
                int lastReceived = 0;
                int item;
                while (true)
                {
                    if (q.TryPeek(out item))
                    {
                        Assert.Equal(lastReceived + 1, item);
                        Assert.True(q.TryDequeue(out item));
                        Assert.Equal(lastReceived + 1, item);
                        lastReceived = item;
                        if (item == items) break;
                    }
                }
            });

            // Producer queues the expected number of items
            Task producer = Task.Run(() =>
            {
                for (int i = 1; i <= items; i++) q.Enqueue(i);
            });

            Task.WaitAll(producer, consumer);
        }

        [Theory]
        [InlineData(1, 4, 1024)]
        [InlineData(4, 1, 1024)]
        [InlineData(3, 3, 1024)]
        public void MultipleProducerConsumer_AllItemsTransferred(int producers, int consumers, int itemsPerProducer)
        {
            var cq = new ConcurrentQueue<int>();
            var tasks = new List<Task>();

            int totalItems = producers * itemsPerProducer;
            int remainingItems = totalItems;
            int sum = 0;

            for (int i = 0; i < consumers; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    while (Volatile.Read(ref remainingItems) > 0)
                    {
                        int item;
                        if (cq.TryDequeue(out item))
                        {
                            Interlocked.Add(ref sum, item);
                            Interlocked.Decrement(ref remainingItems);
                        }
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));
            }

            for (int i = 0; i < producers; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int item = 1; item <= itemsPerProducer; item++)
                    {
                        cq.Enqueue(item);
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.Equal(producers * (itemsPerProducer * (itemsPerProducer + 1) / 2), sum);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(100, false)]
        [InlineData(512, false)]
        [InlineData(1, true)]
        [InlineData(100, true)]
        [InlineData(512, true)]
        public void CopyTo_AllItemsCopiedAtCorrectLocation(int count, bool viaInterface)
        {
            var q = new ConcurrentQueue<int>(Enumerable.Range(0, count));
            var c = (ICollection)q;
            int[] arr = new int[count];

            if (viaInterface)
            {
                c.CopyTo(arr, 0);
            }
            else
            {
                q.CopyTo(arr, 0);
            }
            Assert.Equal(q, arr);

            if (count > 1)
            {
                int toRemove = count / 2;
                int remaining = count - toRemove;
                for (int i = 0; i < toRemove; i++)
                {
                    int item;
                    Assert.True(q.TryDequeue(out item));
                    Assert.Equal(i, item);
                }

                if (viaInterface)
                {
                    c.CopyTo(arr, 1);
                }
                else
                {
                    q.CopyTo(arr, 1);
                }

                Assert.Equal(0, arr[0]);
                for (int i = 0; i < remaining; i++)
                {
                    Assert.Equal(arr[1 + i], i + toRemove);
                }
            }
        }

        [Fact]
        public void IEnumerable_GetAllExpectedItems()
        {
            IEnumerable<int> enumerable1 = new ConcurrentQueue<int>(Enumerable.Range(1, 5));
            IEnumerable enumerable2 = enumerable1;

            int expectedNext = 1;
            using (IEnumerator<int> enumerator1 = enumerable1.GetEnumerator())
            {
                IEnumerator enumerator2 = enumerable2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Assert.True(enumerator2.MoveNext());

                    Assert.Equal(expectedNext, enumerator1.Current);
                    Assert.Equal(expectedNext, enumerator2.Current);

                    expectedNext++;
                }
                Assert.False(enumerator2.MoveNext());
                Assert.Equal(expectedNext, 6);
            }
        }

        [Fact]
        public void ReferenceTypes_NulledAfterDequeue()
        {
            int iterations = 10; // any number <32 will do
            var mres = new ManualResetEventSlim[iterations];

            // Separated out into another method to ensure that even in debug
            // the JIT doesn't force anything to be kept alive for longer than we need.
            var queue = ((Func<ConcurrentQueue<Finalizable>>)(() =>
            {
                var q = new ConcurrentQueue<Finalizable>();

                for (int i = 0; i < iterations; i++)
                {
                    mres[i] = new ManualResetEventSlim();
                    q.Enqueue(new Finalizable(mres[i]));
                }

                for (int i = 0; i < iterations; i++)
                {
                    Finalizable temp;
                    Assert.True(q.TryDequeue(out temp));
                }

                return q;
            }))();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.True(Array.TrueForAll(mres, e => e.IsSet));

            GC.KeepAlive(queue);
        }

        [Fact]
        public void ManySegments_ConcurrentDequeues_RemainsConsistent()
        {
            var cq = new ConcurrentQueue<int>();
            const int Iters = 10000;

            for (int i = 0; i < Iters; i++)
            {
                cq.Enqueue(i);
                cq.GetEnumerator().Dispose(); // force new segment
            }

            int dequeues = 0;
            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                while (!cq.IsEmpty)
                {
                    int item;
                    if (cq.TryDequeue(out item))
                    {
                        Interlocked.Increment(ref dequeues);
                    }
                }
            });

            Assert.Equal(0, cq.Count);
            Assert.True(cq.IsEmpty);
            Assert.Equal(Iters, dequeues);
        }

        [Fact]
        public void ManySegments_ConcurrentEnqueues_RemainsConsistent()
        {
            var cq = new ConcurrentQueue<int>();
            const int ItemsPerThread = 1000;
            int threads = Environment.ProcessorCount;

            Parallel.For(0, threads, i =>
            {
                for (int item = 0; item < ItemsPerThread; item++)
                {
                    cq.Enqueue(item + (i * ItemsPerThread));
                    cq.GetEnumerator().Dispose();
                }
            });

            Assert.Equal(ItemsPerThread * threads, cq.Count);
            Assert.Equal(Enumerable.Range(0, ItemsPerThread * threads), cq.OrderBy(i => i));
        }

        /// <summary>Sets an event when finalized.</summary>
        private sealed class Finalizable
        {
            private ManualResetEventSlim _mres;

            public Finalizable(ManualResetEventSlim mres) { _mres = mres; }

            ~Finalizable() { _mres.Set(); }
        }

        protected sealed class QueueOracle : IProducerConsumerCollection<int>
        {
            private readonly Queue<int> _queue;
            public QueueOracle(IEnumerable<int> collection) { _queue = new Queue<int>(collection); }
            public int Count => _queue.Count;
            public bool IsSynchronized => false;
            public object SyncRoot => null;
            public void CopyTo(Array array, int index) => ((ICollection)_queue).CopyTo(array, index);
            public void CopyTo(int[] array, int index) => _queue.CopyTo(array, index);
            public IEnumerator<int> GetEnumerator() => _queue.GetEnumerator();
            public int[] ToArray() => _queue.ToArray();
            public bool TryAdd(int item) { _queue.Enqueue(item); return true; }
            public bool TryTake(out int item)
            {
                if (_queue.Count > 0)
                {
                    item = _queue.Dequeue();
                    return true;
                }
                else
                {
                    item = 0;
                    return false;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
