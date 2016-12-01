// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Concurrent.Tests
{
    public partial class ConcurrentBagTests
    {
        [Fact]
        public static void RTest11_Clear()
        {
            RTest11_Clear(1, 10);
            RTest11_Clear(3, 100);
            RTest11_Clear(8, 1000);
        }

        public static void RTest11_Clear(int threadsCount, int itemsPerThread)
        {
            int addFailures = 0;
            int peekFailures = 0;
            int takeFailures = 0;
            int clearFailures = 0;
            int arrayFailures = 0;
            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            Task[] threads = new Task[threadsCount];

            var random = new Random();
            for (int i = 0; i < threadsCount; i++)
            {
                threads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        switch (random.Next(5))
                        {
                            case 0:
                                try
                                {
                                    bag.Add(j);
                                }
                                catch
                                {
                                    Interlocked.Increment(ref addFailures);
                                }
                                break;
                            case 1:
                                try
                                {
                                    int item;
                                    bag.TryPeek(out item);
                                }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref peekFailures);
                                    var ex = e;
                                }
                                break;
                            case 2:
                                try
                                {
                                    int item;
                                    bag.TryTake(out item);
                                }
                                catch (Exception e)
                                {
                                    Interlocked.Increment(ref takeFailures);
                                    var ex = e;
                                }
                                break;
                            case 3:
                                try
                                {
                                    bag.Clear();
                                }
                                catch
                                {
                                    Interlocked.Increment(ref clearFailures);
                                }
                                break;
                            case 4:
                                try
                                {
                                    bag.ToArray();
                                }
                                catch
                                {
                                    Interlocked.Increment(ref arrayFailures);
                                }
                                break;
                        }
                    }
                });
            }

            Task.WaitAll(threads);

            Assert.Equal(0, addFailures);
            Assert.Equal(0, peekFailures);
            Assert.Equal(0, takeFailures);
            Assert.Equal(0, clearFailures);
            Assert.Equal(0, arrayFailures);
        }

    }
}
