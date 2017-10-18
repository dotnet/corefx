// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Management.Tests
{
    public class SelectQueryTests
    {
        [Fact]
        public void Select_Win32_LogicalDisk_ClassName()
        {
            var query = new SelectQuery("Win32_LogicalDisk");
            var scope = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2");
            scope.Connect();

            using (var searcher = new ManagementObjectSearcher(scope, query))
            using (var collection = searcher.Get())
            {
                Assert.True(collection.Count > 0);
                foreach (ManagementBaseObject result in collection)
                {
                    Assert.True(result.Properties.Count > 1);
                    Assert.True(!string.IsNullOrEmpty(result.Properties["DeviceID"].Value.ToString()));
                }
            }
        }

        [Fact]
        public void Select_Win32_LogicalDisk_ClassName_Condition()
        {
            var query = new SelectQuery("Win32_LogicalDisk", "DriveType=3");
            var scope = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2");
            scope.Connect();

            using (var searcher = new ManagementObjectSearcher(scope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                Assert.True(collection.Count > 0);
                foreach (ManagementBaseObject result in collection)
                {
                    Assert.True(!string.IsNullOrEmpty(result.GetPropertyValue("DeviceID").ToString()));
                }
            }
        }

        [Fact]
        public void Select_All_Win32_LogicalDisk_Wql()
        {
            var query = new SelectQuery("select * from Win32_LogicalDisk");
            var scope = new ManagementScope(@"root\cimv2");
            scope.Connect();

            using (var searcher = new ManagementObjectSearcher(scope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                Assert.True(collection.Count > 0);
                foreach (ManagementBaseObject result in collection)
                {
                    Assert.True(!string.IsNullOrEmpty(result.GetPropertyValue("DeviceID").ToString()));
                }
            }
        }
    }
}
