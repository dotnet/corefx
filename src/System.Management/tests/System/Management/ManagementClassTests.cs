// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using Xunit;

namespace System.Management.Tests
{
    public class ManagementClassTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void CodeTypeDeclaration_For_Win32_LogicalDisk(bool includeSystemClassInClassDef, bool systemPropertyClass)
        {
            var managementClass = new ManagementClass(null, "Win32_LogicalDisk", null);
            CodeTypeDeclaration classDom = managementClass.GetStronglyTypedClassCode(includeSystemClassInClassDef, systemPropertyClass);
            Assert.Equal(systemPropertyClass ? "ManagementSystemProperties" : "LogicalDisk", classDom.Name);
        }

        [Fact]
        public void ClassMembers_For_Win32_LogicalDisk()
        {
            var managementClass = new ManagementClass(new ManagementPath("Win32_LogicalDisk"));

            MethodDataCollection methods = managementClass.Methods;
            Assert.True(methods.Count > 0);
            foreach (MethodData method in methods)
                Assert.False(string.IsNullOrWhiteSpace(method.Name));

            PropertyDataCollection properties = managementClass.Properties;
            Assert.True(properties.Count > 0);
            foreach (PropertyData property in properties)
                Assert.False(string.IsNullOrWhiteSpace(property.Name));

            QualifierDataCollection qualifiers = managementClass.Qualifiers;
            foreach (QualifierData qualifier in qualifiers)
                Assert.False(string.IsNullOrWhiteSpace(qualifier.Name));
        }
    }
}
