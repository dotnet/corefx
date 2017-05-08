// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace Microsoft.Win32.RegistryTests
{
    public class Registry_Fields
    {
        public static readonly object[][] BaseKeyNameTestData =
        {
            new object[] { Registry.CurrentUser, "HKEY_CURRENT_USER" },
            new object[] { Registry.LocalMachine, "HKEY_LOCAL_MACHINE" },
            new object[] { Registry.ClassesRoot, "HKEY_CLASSES_ROOT" },
            new object[] { Registry.Users, "HKEY_USERS" },
            new object[] { Registry.PerformanceData, "HKEY_PERFORMANCE_DATA" },
            new object[] { Registry.CurrentConfig, "HKEY_CURRENT_CONFIG" },
#pragma warning disable 0618 // obsolete
            new object[] { Registry.DynData, "HKEY_DYN_DATA" }
#pragma warning restore 0618
        };

        [Theory]
        [MemberData(nameof(BaseKeyNameTestData))]
        public void BaseKeyName_ExpectedName(RegistryKey baseKey, string expectedName)
        {
            Assert.Equal(expectedName, baseKey.Name);
        }

        [Fact]
        public void DynData_Obsolete()
        {
#pragma warning disable 0618 // obsolete
            RegistryKey dynData = Registry.DynData;
#pragma warning restore 0618
            Assert.Throws<IOException>(() => dynData.GetSubKeyNames());
            Assert.Throws<IOException>(() => dynData.GetValueNames());
            Assert.Throws<IOException>(() => dynData.ValueCount);
        }
    }
}
