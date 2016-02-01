// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class ProcessThreadTests : ProcessTestBase
    {
        [Fact]
        public void TestCommonPriorityAndTimeProperties()
        {
            ProcessThreadCollection threadCollection = _process.Threads;
            Assert.True(threadCollection.Count > 0);
            ProcessThread thread = threadCollection[0];
            try
            {
                if (ThreadState.Terminated != thread.ThreadState)
                {
                    Assert.True(thread.Id >= 0);
                    Assert.Equal(_process.BasePriority, thread.BasePriority);
                    Assert.True(thread.CurrentPriority >= 0);
                    Assert.True(thread.PrivilegedProcessorTime.TotalSeconds >= 0);
                    Assert.True(thread.UserProcessorTime.TotalSeconds >= 0);
                    Assert.True(thread.TotalProcessorTime.TotalSeconds >= 0);
                }
            }
            catch (Win32Exception)
            {
                // Win32Exception is thrown when getting threadinfo fails. 
            }
        }

        [Fact]
        public void TestThreadCount()
        {
            int numOfThreads = 10;
            CountdownEvent counter = new CountdownEvent(numOfThreads);
            ManualResetEventSlim mre = new ManualResetEventSlim();
            for (int i = 0; i < numOfThreads; i++)
            {
                new Thread(() => { counter.Signal(); mre.Wait(); }) { IsBackground = true }.Start();
            }

            counter.Wait();

            try
            {
                Assert.True(Process.GetCurrentProcess().Threads.Count >= numOfThreads);
            }
            finally
            {
                mre.Set();
            }
        }

        [Fact]
        public void TestStartTimeProperty()
        {
            DateTime beforeTime = DateTime.UtcNow;
            Process p = CreateProcessLong();
            try
            {
                p.Start();

                ProcessThreadCollection threadCollection = p.Threads;
                Assert.True(threadCollection.Count > 0);

                ProcessThread thread = threadCollection[0];

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Assert.Throws<PlatformNotSupportedException>(() => thread.StartTime);
                    return;
                }

                if (ThreadState.Initialized != thread.ThreadState)
                {
                    long starttimeTicks = 0;
                    starttimeTicks = thread.StartTime.ToUniversalTime().Ticks;
                    if (starttimeTicks != 0)
                    {
                        // Allow for error for upto a minute.
                        long intervalTicks = TimeSpan.FromMinutes(1).Ticks;
                        long beforeTicks = beforeTime.Ticks - intervalTicks;

                        p.Kill();
                        p.WaitForExit(WaitInMS);

                        long afterTicks = DateTime.UtcNow.Ticks + intervalTicks;
                        Assert.InRange(starttimeTicks, beforeTicks, afterTicks);
                    }
                }
            }
            finally
            {
                if (!p.HasExited)
                    p.Kill();

                Assert.True(p.WaitForExit(WaitInMS));
            }
        }

        [Fact]
        public void TestStartAddressProperty()
        {
            Process p = Process.GetCurrentProcess();
            try
            {
                if (p.Threads.Count != 0)
                {
                    ProcessThread thread = p.Threads[0];
                    Assert.Equal(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), thread.StartAddress == IntPtr.Zero);
                }
            }
            finally
            {
                p.Dispose();
            }
        }

        [Fact]
        public void TestPriorityLevelProperty()
        {
            ProcessThread thread = _process.Threads[0];

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Throws<PlatformNotSupportedException>(() => thread.PriorityLevel);
                Assert.Throws<PlatformNotSupportedException>(() => thread.PriorityLevel = ThreadPriorityLevel.AboveNormal);
                return;
            }

            ThreadPriorityLevel originalPriority = thread.PriorityLevel;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Throws<PlatformNotSupportedException>(() => thread.PriorityLevel = ThreadPriorityLevel.AboveNormal);
                return;
            }

            try
            {
                thread.PriorityLevel = ThreadPriorityLevel.AboveNormal;
                Assert.Equal(ThreadPriorityLevel.AboveNormal, thread.PriorityLevel);
            }
            finally
            {
                thread.PriorityLevel = originalPriority;
                Assert.Equal(originalPriority, thread.PriorityLevel);
            }
        }

        [Fact]
        public void TestThreadStateProperty()
        {
            ProcessThread thread = _process.Threads[0];
            if (ThreadState.Wait != thread.ThreadState)
            {
                Assert.Throws<InvalidOperationException>(() => thread.WaitReason);
            }
        }
    }
}
