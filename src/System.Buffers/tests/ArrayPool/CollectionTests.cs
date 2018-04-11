// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Xunit;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Buffers.ArrayPool.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
    public class CollectionTests : RemoteExecutorTestBase
    {
        private const string TrimSwitchName = "DOTNET_SYSTEM_BUFFERS_ARRAYPOOL_TRIMSHARED";

        [Theory,
            InlineData(true),
            InlineData(false)]
        public void BuffersAreCollectedWhenStale(bool trim)
        {
            RemoteInvokeOptions options = new RemoteInvokeOptions
            {
                // This test has to wait for the buffers to go stale (give it three minutes)
                TimeOut = 3 * 60 * 1000
            };

            options.StartInfo.UseShellExecute = false;
            options.StartInfo.EnvironmentVariables.Add(TrimSwitchName, trim.ToString());

            RemoteInvoke((trimString) =>
            {
                // Check that our environment is as we expect
                Assert.Equal(trimString, Environment.GetEnvironmentVariable(TrimSwitchName));

                const int BufferCount = 8;
                const int BufferSize = 1025;

                // Get the pool and check our trim setting
                var pool = ArrayPool<int>.Shared;
                Assert.StartsWith("TlsOverPerCoreLockedStacksArrayPool", pool.GetType().Name);
                bool parsedTrim = bool.Parse(trimString);
                var trimField = pool.GetType().GetField("s_trimBuffers", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.Equal(parsedTrim, (bool)trimField.GetValue(null));

                List<int[]> rentedBuffers = new List<int[]>();

                // Rent and return a set of buffers
                for (int i = 0; i < BufferCount; i++)
                {
                    rentedBuffers.Add(pool.Rent(BufferSize));
                }
                for (int i = 0; i < BufferCount; i++)
                {
                    pool.Return(rentedBuffers[i]);
                }

                // Rent what we returned and ensure they are the same
                for (int i = 0; i < BufferCount; i++)
                {
                    var buffer = pool.Rent(BufferSize);
                    Assert.Contains(rentedBuffers, item => ReferenceEquals(item, buffer));
                }
                for (int i = 0; i < BufferCount; i++)
                {
                    pool.Return(rentedBuffers[i]);
                }

                // Now wait a little over a minute and force a GC to get some buffers returned
                Console.WriteLine("Waiting a minute for buffers to go stale...");
                Thread.Sleep(61 * 1000);
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
                bool foundNewBuffer = false;
                for (int i = 0; i < BufferCount; i++)
                {
                    var buffer = pool.Rent(BufferSize);
                    if (!rentedBuffers.Any(item => ReferenceEquals(item, buffer)))
                    {
                        foundNewBuffer = true;
                    }
                }

                if (parsedTrim)
                {
                    Assert.True(foundNewBuffer, "should have found a newly created buffer");
                }
                else
                {
                    Assert.False(foundNewBuffer, "should not have found a newly created buffer");
                }
                return SuccessExitCode;
            }, trim.ToString(), options).Dispose();
        }

        [Theory,
            InlineData(true),
            InlineData(false)]
        public unsafe void ThreadLocalIsCollectedUnderHighPressure(bool trim)
        {
            RemoteInvokeOptions options = new RemoteInvokeOptions
            {
                // This test has to fill memory, give it a minute to do so
                TimeOut = 1 * 60 * 1000
            };

            options.StartInfo.UseShellExecute = false;
            options.StartInfo.EnvironmentVariables.Add(TrimSwitchName, trim.ToString());

            RemoteInvoke((trimString) =>
            {
                // Check that our environment is as we expect
                Assert.Equal(trimString, Environment.GetEnvironmentVariable(TrimSwitchName));

                // Get the pool and check our trim setting
                var pool = ArrayPool<byte>.Shared;
                Assert.StartsWith("TlsOverPerCoreLockedStacksArrayPool", pool.GetType().Name);
                bool parsedTrim = bool.Parse(trimString);
                var trimField = pool.GetType().GetField("s_trimBuffers", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.Equal(parsedTrim, (bool)trimField.GetValue(null));

                // Create our buffer, return it, re-rent it and ensure we have the same one
                const int BufferSize = 4097;
                var buffer = pool.Rent(BufferSize);
                pool.Return(buffer);
                Assert.Same(buffer, pool.Rent(BufferSize));

                // Return it and put memory pressure on to get it cleared
                pool.Return(buffer);

                const int AllocSize = 1024 * 1024 * 64;
                int PageSize = Environment.SystemPageSize;
                var pressureMethod = pool.GetType().GetMethod("GetMemoryPressure", BindingFlags.Static | BindingFlags.NonPublic);
                do
                {
                    Span<byte> native = new Span<byte>(Marshal.AllocHGlobal(AllocSize).ToPointer(), AllocSize);

                    // Touch the pages to bring them into physical memory
                    for (int i = 0; i < native.Length; i += PageSize)
                    {
                        native[i] = 0xEF;
                    }

                    GC.Collect(2);
                } while ((int)pressureMethod.Invoke(null, null) != 2);

                GC.WaitForPendingFinalizers();
                if (parsedTrim)
                {
                    // Should have a new buffer now
                    Assert.NotSame(buffer, pool.Rent(BufferSize));
                }
                else
                {
                    // Disabled, should not have trimmed buffer
                    Assert.Same(buffer, pool.Rent(BufferSize));
                }

                return SuccessExitCode;
            }, trim.ToString(), options).Dispose();
        }
    }
}
