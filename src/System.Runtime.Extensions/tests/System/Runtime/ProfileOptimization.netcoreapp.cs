// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using Xunit;

namespace System.Runtime.Tests
{
    public class ProfileOptimizationTest : RemoteExecutorTestBase
    {
        [Fact]
        public void ProfileOptimization_CheckFileExists()
        {
            string tmpProfileFilePath = GetTestFileName();

            RemoteInvoke(profileFilePath =>
            {
                string directoryName = Path.GetDirectoryName(profileFilePath);

                // after test fail tracked by https://github.com/dotnet/corefx/issues/31792
                // we suspect that the reason is something related to write permission to the location
                // to prove that we added a simple write to file in same location of profile file directory path
                // ProfileOptimization/Multi-Core JIT could fail silently
                File.WriteAllText(Path.Combine(directoryName, Path.GetRandomFileName()), "42");

                ProfileOptimization.SetProfileRoot(directoryName);
                ProfileOptimization.StartProfile(Path.GetFileName(profileFilePath));
                return 42;
            }, tmpProfileFilePath).Dispose();

            FileInfo fileInfo = new FileInfo(tmpProfileFilePath);
            Assert.True(fileInfo.Exists);
            Assert.True(fileInfo.Length > 0);
        }
    }
}
