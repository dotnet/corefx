// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System; 
using Xunit;

namespace System.Management.Tests
{
    public class SelectQueryTests
    {
        [Fact]
        public void Select_Win32_LogicalDisk()
        {
            var query = new SelectQuery("Win32_LogicalDisk", "DriveType=3");
            var scope = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2");
            scope.Connect();
            
            var searcher = new ManagementObjectSearcher(scope, query);
            foreach(var result in searcher.Get())
            {
                Assert.True(!string.IsNullOrEmpty(result.GetPropertyValue("DeviceID").ToString()));
            }
        }
    }
}
