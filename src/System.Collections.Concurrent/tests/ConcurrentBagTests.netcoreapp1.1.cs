// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Tests;
using System.Diagnostics;
using System.Linq;
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

            Task[] addThreads = new Task[threadsCount];
            Task[] peekThreads = new Task[threadsCount];
            Task[] takeThreads = new Task[threadsCount];
            Task[] clearThreads = new Task[threadsCount];
            Task[] arrayThreads = new Task[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                addThreads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        try
                        {
                            bag.Add(j);
                        }
                        catch
                        {
                            Interlocked.Increment(ref addFailures);
                        }
                    }
                });
                peekThreads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        try
                        {
                            int item;
                            bag.TryPeek(out item);
                        }
                        catch
                        {
                            Interlocked.Increment(ref peekFailures);
                        }
                    }
                });
                takeThreads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        try
                        {
                            int item;
                            bag.TryTake(out item);
                        }
                        catch
                        {
                            Interlocked.Increment(ref takeFailures);
                        }
                    }
                });
                clearThreads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        try
                        {
                            bag.Clear();
                        }
                        catch
                        {
                            Interlocked.Increment(ref clearFailures);
                        }
                    }
                });
                arrayThreads[i] = Task.Run(() =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        try
                        {
                            bag.ToArray();
                        }
                        catch
                        {
                            Interlocked.Increment(ref arrayFailures);
                        }
                    }
                });
            }

            Task.WaitAll(addThreads);
            Task.WaitAll(peekThreads);
            Task.WaitAll(takeThreads);
            Task.WaitAll(clearThreads);
            Task.WaitAll(arrayThreads);

            Assert.Equal(0, addFailures);
            Assert.Equal(0, peekFailures);
            Assert.Equal(0, takeFailures);
            Assert.Equal(0, clearFailures);
            Assert.Equal(0, arrayFailures);
        }

    }
}
