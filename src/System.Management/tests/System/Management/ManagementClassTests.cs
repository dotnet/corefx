// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using Xunit;

namespace System.Management.Tests
{
    public class ManagementClassTests
    {
        [Fact]
        public void CodeTypeDeclaration_For_Win32_LogicalDisk()
        {
            var managementClass = new ManagementClass(null, "Win32_LogicalDisk", null);
            CodeTypeDeclaration classDom = managementClass.GetStronglyTypedClassCode(false, false);
            Assert.Equal("LogicalDisk", classDom.Name);
        }
    }
}
