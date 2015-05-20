// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace Microsoft.Win32.RegistryTests
{
    public class RegistryKey_ToString : TestSubKey
    {
        private const string TestKey = "BCL_TEST_TO_STRING";
        private const string CurrentUserKeyName = "HKEY_CURRENT_USER";

        public RegistryKey_ToString()
            : base(TestKey)
        {
        }

        [Fact]
        public void NegativeTests()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _testRegistryKey.Dispose();
                return _testRegistryKey.ToString();
            });
        }

        [Fact]
        public static void TestBaseKeyToString()
        {
            Assert.Equal(CurrentUserKeyName, Registry.CurrentUser.ToString());
        }

        [Fact]
        public void TestToString()
        {
            string expectedName = string.Format("HKEY_CURRENT_USER\\{0}", TestKey);
            Assert.Equal(expectedName, _testRegistryKey.ToString());
        }
    }
}
