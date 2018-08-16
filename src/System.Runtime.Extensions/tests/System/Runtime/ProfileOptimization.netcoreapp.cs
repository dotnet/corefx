// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using Xunit;

namespace System.Runtime.Tests
{
    public class ProfileOptimizationTest : RemoteExecutorTestBase
    {
        [Fact]
        public void ProfileOptimization_CheckFileExists()
        {
            bool success = false;
            int retryCount = 0;

            // retry n times for max ~20 seconds in total
            // sometimes ProfileOptimization.StartProfile isn't quick enough
            while (retryCount < 7)
            {
                if (success = StartProfile(1000 * retryCount))
                {
                    break;
                }
                retryCount++;
            }

            Assert.True(success);
        }

        private bool StartProfile(int sleepMilliseconds)
        {
            string tmpProfileFilePath = GetTestFileName();

            RemoteInvoke((profileFilePath, sleepfor) =>
            {
                ProfileOptimization.SetProfileRoot(Path.GetDirectoryName(profileFilePath));
                ProfileOptimization.StartProfile(Path.GetFileName(profileFilePath));
                int sleep = int.Parse(sleepfor);

                // we sleep only in case of first test fail
                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
            }, tmpProfileFilePath, sleepMilliseconds.ToString()).Dispose();

            FileInfo fileInfo = new FileInfo(tmpProfileFilePath);
            return fileInfo.Exists && fileInfo.Length > 0;
        }

    }
}
