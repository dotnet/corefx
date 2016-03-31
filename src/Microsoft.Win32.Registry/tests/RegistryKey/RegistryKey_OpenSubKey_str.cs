// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Xunit;

namespace Microsoft.Win32.RegistryTests
{
    public class RegistryKey_OpenSubKey_str : RegistryTestsBase
    {
        [Fact]
        public void NegativeTests()
        {
            // Should throw if passed subkey name is null
            Assert.Throws<ArgumentNullException>(() => TestRegistryKey.OpenSubKey(name: null));

            // Should throw if subkey name greater than 255 chars
            Assert.Throws<ArgumentException>(() => TestRegistryKey.OpenSubKey(new string('a', 256)));

            // OpenSubKey should be read only by default
            const string name = "FooBar";
            TestRegistryKey.SetValue(name, 42);
            using (var rk = Registry.CurrentUser.OpenSubKey(TestRegistryKeyName))
            {
                Assert.Throws<UnauthorizedAccessException>(() => rk.CreateSubKey(name));
                Assert.Throws<UnauthorizedAccessException>(() => rk.SetValue(name, "String"));
                Assert.Throws<UnauthorizedAccessException>(() => rk.DeleteValue(name));
                Assert.Throws<UnauthorizedAccessException>(() => rk.DeleteSubKey(name));
                Assert.Throws<UnauthorizedAccessException>(() => rk.DeleteSubKeyTree(name));
            }

            // Should throw if RegistryKey closed
            Assert.Throws<ObjectDisposedException>(() =>
            {
                TestRegistryKey.Dispose();
                TestRegistryKey.OpenSubKey(TestRegistryKeyName);
            });
        }

        [Fact]
        public void OpenSubKeyTest()
        {
            TestRegistryKey.CreateSubKey(TestRegistryKeyName);
            Assert.NotNull(TestRegistryKey.OpenSubKey(TestRegistryKeyName));
            Assert.Equal(expected: 1, actual: TestRegistryKey.SubKeyCount);

            TestRegistryKey.DeleteSubKey(TestRegistryKeyName);
            Assert.Null(TestRegistryKey.OpenSubKey(TestRegistryKeyName));
            Assert.Equal(expected: 0, actual: TestRegistryKey.SubKeyCount);
        }

        [Fact]
        public void OpenSubKeyTest2()
        {
            string[] subKeyNames = Enumerable.Range(1, 9).Select(x => "BLAH_" + x.ToString()).ToArray();
            foreach (var subKeyName in subKeyNames)
            {
                TestRegistryKey.CreateSubKey(subKeyName);
            }
            
            Assert.Equal(subKeyNames.Length, TestRegistryKey.SubKeyCount);
            Assert.Equal(subKeyNames, TestRegistryKey.GetSubKeyNames());
        }
    }
}
