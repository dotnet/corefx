// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Management.Tests
{
    public class ManagementObjectTests
    {
        [Fact]
        public void Win32_LogicalDisk()
        {
            var deviceId = Environment.GetEnvironmentVariable("SystemDrive");
            using (ManagementObject obj = new ManagementObject(
                $"Win32_LogicalDisk.DeviceID=\"{deviceId}\""))
            {
                obj.Get();
                Assert.True(obj.Properties.Count > 0);
                Assert.True(ulong.Parse(obj["Size"].ToString()) > 0);
                var classPath = obj.ClassPath.Path;
                Assert.Equal($@"\\{Environment.MachineName}\root\cimv2:Win32_LogicalDisk", classPath);
            }
        }
    }
}
