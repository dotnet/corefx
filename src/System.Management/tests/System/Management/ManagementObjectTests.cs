// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;

namespace System.Management.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "WMI not supported via UAP")]
    public class ManagementObjectTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Get_Win32_LogicalDisk()
        {
            using (ManagementObject obj = new ManagementObject($"Win32_LogicalDisk.DeviceID=\"{WmiTestHelper.SystemDriveId}\""))
            {
                obj.Get();
                Assert.True(obj.Properties.Count > 0);
                Assert.True(ulong.Parse(obj["Size"].ToString()) > 0);
                var classPath = obj.ClassPath.Path;
                Assert.Equal($@"\\{Environment.MachineName}\root\cimv2:Win32_LogicalDisk", classPath);

                var clone = obj.Clone();
                Assert.False(ReferenceEquals(clone, obj));
                ((ManagementObject)clone).Dispose();
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [OuterLoop]
        public void GetRelated_For_Win32_LogicalDisk()
        {
            using (ManagementObject obj = new ManagementObject($"Win32_LogicalDisk.DeviceID=\"{WmiTestHelper.SystemDriveId}\""))
            using (ManagementObjectCollection relatedCollection = obj.GetRelated())
            {
                Assert.True(relatedCollection.Count > 0);
                foreach (ManagementObject related in relatedCollection)
                    Assert.False(string.IsNullOrWhiteSpace(related.ClassPath.NamespacePath));
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Set_Property_Win32_ComputerSystem()
        {
            using (ManagementObject obj = new ManagementObject($"Win32_ComputerSystem.Name=\"{Environment.MachineName}\""))
            {
                obj.Get();
                obj.SetPropertyValue("Workgroup", "WmiTests");
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [OuterLoop]
        public void Invoke_Instance_And_Static_Method_Win32_Process()
        {
            var processClass = new ManagementClass("Win32_Process");
            object[] methodArgs = { "notepad.exe", null, null, 0 };

            object resultObj = processClass.InvokeMethod("Create", methodArgs);

            Thread.Sleep(1000);
            var resultCode = (uint)resultObj;
            Assert.Equal(0u, resultCode);

            var processId = (uint)methodArgs[3];
            Assert.True(0u != processId, $"Unexpected process ID: {processId}");

            var process = new ManagementObject($"Win32_Process.Handle=\"{processId}\"");
            resultObj = process.InvokeMethod("Terminate", new object[]{ 0 });
            resultCode = (uint)resultObj;
            Assert.Equal(0u, resultCode);

            Thread.Sleep(2000);
            ManagementException managementException = Assert.Throws<ManagementException>(() => process.Get());
            Assert.Equal(ManagementStatus.NotFound, managementException.ErrorCode);
        }
    }
}
