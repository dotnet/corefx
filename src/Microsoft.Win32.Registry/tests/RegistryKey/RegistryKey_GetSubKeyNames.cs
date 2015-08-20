// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Win32.RegistryTests
{
    public class RegistryKey_GetSubKeyNames : RegistryTestsBase
    {
        [Fact]
        public void ShoudThrowIfDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _testRegistryKey.Dispose();
                _testRegistryKey.GetSubKeyNames();
            });
        }

        [Fact]
        public void ShouldThrowIfRegistryKeyDeleted()
        {
            Registry.CurrentUser.DeleteSubKeyTree(_testRegistryKeyName);
            Assert.Throws<IOException>(() => _testRegistryKey.GetSubKeyNames());
        }

        [Fact]
        public void GetSubKeyNamesTest()
        {
            // [] Creating new SubKeys and get the names
            string[] expectedSubKeyNames = Enumerable.Range(1, 9).Select(x => "BLAH_" + x.ToString()).ToArray();
            foreach (var subKeyName in expectedSubKeyNames)
            {
                _testRegistryKey.CreateSubKey(subKeyName);
            }

            Assert.Equal(expectedSubKeyNames, _testRegistryKey.GetSubKeyNames());
        }

        [Fact]
        public void GetSubKeyNamesTest2()
        {
            // [] Check that zero length array is returned for no subkeys
            Assert.Empty(_testRegistryKey.GetSubKeyNames());
        }
    }
}
