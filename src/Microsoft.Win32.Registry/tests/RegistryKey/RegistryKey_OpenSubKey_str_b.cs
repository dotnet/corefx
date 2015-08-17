// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace Microsoft.Win32.RegistryTests
{
    public class RegistryKey_OpenSubKey_str_b : RegistryTestsBase
    {
        [Fact]
        public void NegativeTests()
        {
            // Should throw if passed subkey name is null
            Assert.Throws<ArgumentNullException>(() => _testRegistryKey.OpenSubKey(name: null, writable: false));

            // Should throw if subkey name greater than 255 chars
            Assert.Throws<ArgumentException>(() => _testRegistryKey.OpenSubKey(new string('a', 256), true));

            // OpenSubKey should be read only
            const string name = "FooBar";
            _testRegistryKey.SetValue(name, 42);
            _testRegistryKey.CreateSubKey(name);
            using (var rk = Registry.CurrentUser.OpenSubKey(name: _testRegistryKeyName, writable: false))
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
                _testRegistryKey.Dispose();
                _testRegistryKey.OpenSubKey(_testRegistryKeyName, true);
            });
        }

        [Fact]
        public void OpenSubKeyTest()
        {
            // [] Should have write rights when true is passed
            const int testValue = 32;
            using (var rk = _testRegistryKey.OpenSubKey("", true))
            {
                rk.CreateSubKey(_testRegistryKeyName);
                rk.SetValue(_testRegistryKeyName, testValue);

                Assert.NotNull(rk.OpenSubKey(_testRegistryKeyName));
                Assert.Equal(testValue, (int)rk.GetValue(_testRegistryKeyName));
            }
        }
    }
}
