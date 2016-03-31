// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Win32.RegistryTests
{
    public class RegistryKey_CreateSubKey_str : RegistryTestsBase
    {
        [Fact]
        public void NegativeTests()
        {
            // Should throw if passed subkey name is null
            Assert.Throws<ArgumentNullException>(() => TestRegistryKey.CreateSubKey(null));

            // Should throw if key length above 255 characters
            const int maxValueNameLength = 255;
            Assert.Throws<ArgumentException>(() => TestRegistryKey.CreateSubKey(new string('a', maxValueNameLength + 1)));

            // Max number of parts to the registry key path is 509 (failing once it hits 510). 
            // As TestRegistryKey is already a subkey, that gives us 507 remaining parts before an 
            // exception is thrown.
            const int maxNestedLevel = 507;
            string exceedsNestedSubkeyName = string.Join(@"\", Enumerable.Repeat("a", maxNestedLevel));
            using (RegistryKey k = TestRegistryKey.CreateSubKey(exceedsNestedSubkeyName))
            {
                // Verify TestRegistryKey is already nested, with 508 slashes meaning 509 parts
                Assert.Equal(maxNestedLevel + 1, k.Name.Count(c => c == '\\')); 
            }
            Assert.Throws<IOException>(() => TestRegistryKey.CreateSubKey(exceedsNestedSubkeyName + @"\" + maxNestedLevel));

            // Should throw if RegistryKey is readonly
            const string name = "FooBar";
            TestRegistryKey.SetValue(name, 42);
            using (var rk = Registry.CurrentUser.CreateSubKey(TestRegistryKeyName, writable: false))
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
                TestRegistryKey.CreateSubKey(TestRegistryKeyName);
            });
        }

        [Fact]
        public void CreateSubkeyWithEmptyName()
        {
            string expectedName = TestRegistryKey.Name + @"\";
            var rk = TestRegistryKey.CreateSubKey(string.Empty);
            Assert.NotNull(rk);
            Assert.Equal(expectedName, rk.Name);
        }

        [Fact]
        public void CreateSubKeyAndCheckThatItExists()
        {
            TestRegistryKey.CreateSubKey(TestRegistryKeyName);
            Assert.NotNull(TestRegistryKey.OpenSubKey(TestRegistryKeyName));
            Assert.Equal(expected: 1, actual:TestRegistryKey.SubKeyCount);
        }

        [Fact]
        public void CreateSubKeyShouldOpenExisting()
        {
            // CreateSubKey should open subkey if it already exists
            Assert.NotNull(TestRegistryKey.CreateSubKey(TestRegistryKeyName));
            Assert.NotNull(TestRegistryKey.OpenSubKey(TestRegistryKeyName));
            Assert.NotNull(TestRegistryKey.CreateSubKey(TestRegistryKeyName));
        }

        [Theory]
        [InlineData("Dyalog APL/W 10.0")]
        [InlineData(@"a\b\c\d\e\f\g\h")]
        [InlineData(@"a\b\c\/d\//e\f\g\h\//\\")]
        public void CreateSubKeyWithName(string subkeyName)
        {
            TestRegistryKey.CreateSubKey(subkeyName);
            Assert.NotNull(TestRegistryKey.OpenSubKey(subkeyName));
        }

        [Fact]
        public void DeepTest()
        {
            //[] how deep can we go with this

            string subkeyName = string.Empty;

            // Changed the number of times we repeat str1 from 100 to 30 in response to the Windows OS
            //There is a restriction of 255 characters for the keyname even if it is multikeys. Not worth to pursue as a bug
            // reduced further to allow for WoW64 changes to the string.
            for (int i = 0; i < 25 && subkeyName.Length < 230; i++)
                subkeyName = subkeyName + i.ToString() + @"\";
            
            TestRegistryKey.CreateSubKey(subkeyName);
            Assert.NotNull(TestRegistryKey.OpenSubKey(subkeyName));

            //However, we are interested in ensuring that there are no buffer overflow issues with a deeply nested keys
            RegistryKey rk = TestRegistryKey;
            for (int i = 0; i < 3; i++)
            {
                rk = rk.OpenSubKey(subkeyName, true);
                Assert.NotNull(rk);
                rk.CreateSubKey(subkeyName);
            }
        }
    }
}
