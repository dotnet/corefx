// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Tests
{
    public class GetEnvironmentVariable
    {
        [Fact]
        public void InvalidArguments_ThrowsExceptions()
        {
            Assert.Throws<ArgumentNullException>("variable", () => Environment.GetEnvironmentVariable(null));
            Assert.Throws<ArgumentNullException>("variable", () => Environment.SetEnvironmentVariable(null, "test"));
            Assert.Throws<ArgumentException>("variable", () => Environment.SetEnvironmentVariable("", "test"));
            Assert.Throws<ArgumentException>("value", () => Environment.SetEnvironmentVariable("test", new string('s', 65 * 1024)));

            Assert.Throws<ArgumentException>("variable", () => Environment.SetEnvironmentVariable("", "test", EnvironmentVariableTarget.Machine));
            Assert.Throws<ArgumentNullException>("variable", () => Environment.SetEnvironmentVariable(null, "test", EnvironmentVariableTarget.User));
            Assert.Throws<ArgumentNullException>("variable", () => Environment.GetEnvironmentVariable(null, EnvironmentVariableTarget.Process));
            Assert.Throws<ArgumentOutOfRangeException>("target", () => Environment.GetEnvironmentVariable("test", (EnvironmentVariableTarget)42));
            Assert.Throws<ArgumentOutOfRangeException>("target", () => Environment.SetEnvironmentVariable("test", "test", (EnvironmentVariableTarget)(-1)));
            Assert.Throws<ArgumentOutOfRangeException>("target", () => Environment.GetEnvironmentVariables((EnvironmentVariableTarget)(3)));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Throws<ArgumentException>("variable", () => Environment.SetEnvironmentVariable(new string('s', 256), "value", EnvironmentVariableTarget.User));
            }
        }

        [Fact]
        public void EmptyVariableReturnsNull()
        {
            Assert.Null(Environment.GetEnvironmentVariable(String.Empty));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)] // GetEnvironmentVariable by design doesn't respect changes via setenv
        public void RandomLongVariableNameCanRoundTrip()
        {
            // NOTE: The limit of 32766 characters enforced by desktop
            // SetEnvironmentVariable is antiquated. I was
            // able to create ~1GB names and values on my Windows 8.1 box. On
            // desktop, GetEnvironmentVariable throws OOM during its attempt to
            // demand huge EnvironmentPermission well before that. Also, the old
            // test for long name case wasn't very good: it just checked that an
            // arbitrary long name > 32766 characters returned null (not found), but
            // that had nothing to do with the limit, the variable was simply not
            // found!

            string variable = "LongVariable_" + new string('@', 33000);
            const string value = "TestValue";

            try
            {
                SetEnvironmentVariableWithPInvoke(variable, value);

                Assert.Equal(value, Environment.GetEnvironmentVariable(variable));
            }
            finally
            {
                SetEnvironmentVariableWithPInvoke(variable, null);
            }
        }

        [Fact]
        public void RandomVariableThatDoesNotExistReturnsNull()
        {
            string variable = "TestVariable_SurelyThisDoesNotExist";
            Assert.Null(Environment.GetEnvironmentVariable(variable));
        }

        [Fact]
        public void VariableNamesAreCaseInsensitiveAsAppropriate()
        {
            string value = "TestValue";

            try
            {
                Environment.SetEnvironmentVariable("ThisIsATestEnvironmentVariable", value);

                Assert.Equal(value, Environment.GetEnvironmentVariable("ThisIsATestEnvironmentVariable"));

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    value = null;
                }

                Assert.Equal(value, Environment.GetEnvironmentVariable("thisisatestenvironmentvariable"));
                Assert.Equal(value, Environment.GetEnvironmentVariable("THISISATESTENVIRONMENTVARIABLE"));
                Assert.Equal(value, Environment.GetEnvironmentVariable("ThISISATeSTENVIRoNMEnTVaRIABLE"));
            }
            finally
            {
                Environment.SetEnvironmentVariable("ThisIsATestEnvironmentVariable", null);
            }
        }

        [Fact]
        public void CanGetAllVariablesIndividually()
        {
            Random r = new Random();
            string envVar1 = "TestVariable_CanGetVariablesIndividually_" + r.Next().ToString();
            string envVar2 = "TestVariable_CanGetVariablesIndividually_" + r.Next().ToString();

            try
            {
                Environment.SetEnvironmentVariable(envVar1, envVar1);
                Environment.SetEnvironmentVariable(envVar2, envVar2);

                IDictionary envBlock = Environment.GetEnvironmentVariables();

                // Make sure the environment variables we set are part of the dictionary returned.
                Assert.True(envBlock.Contains(envVar1));
                Assert.True(envBlock.Contains(envVar1));

                // Make sure the values match the expected ones.
                Assert.Equal(envVar1, envBlock[envVar1]);
                Assert.Equal(envVar2, envBlock[envVar2]);

                // Make sure we can read the individual variables as well
                Assert.Equal(envVar1, Environment.GetEnvironmentVariable(envVar1));
                Assert.Equal(envVar2, Environment.GetEnvironmentVariable(envVar2));
            }
            finally
            {
                // Clear the variables we just set
                Environment.SetEnvironmentVariable(envVar1, null);
                Environment.SetEnvironmentVariable(envVar2, null);
            }
        }

        [Fact]
        public void EnumerateYieldsDictionaryEntryFromIEnumerable()
        {
            // GetEnvironmentVariables has always yielded DictionaryEntry from IEnumerable
            IDictionary vars = Environment.GetEnvironmentVariables();
            IEnumerator enumerator = ((IEnumerable)vars).GetEnumerator();
            if (enumerator.MoveNext())
            {
                Assert.IsType<DictionaryEntry>(enumerator.Current);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            }
        }

        [Fact]
        public void GetEnumerator_IDictionaryEnumerator_YieldsDictionaryEntries()
        {
            // GetEnvironmentVariables has always yielded DictionaryEntry from IDictionaryEnumerator
            IDictionary vars = Environment.GetEnvironmentVariables();
            IDictionaryEnumerator enumerator = vars.GetEnumerator();
            if (enumerator.MoveNext())
            {
                Assert.IsType<DictionaryEntry>(enumerator.Current);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(EnvironmentVariableTarget.User)]
        [InlineData(EnvironmentVariableTarget.Process)]
        [InlineData(EnvironmentVariableTarget.Machine)]
        public void GetEnumerator_LinqOverDictionaryEntries_Success(EnvironmentVariableTarget? target)
        {
            IDictionary envVars = target != null ?
                Environment.GetEnvironmentVariables(target.Value) :
                Environment.GetEnvironmentVariables();

            Assert.IsType<Hashtable>(envVars);

            foreach (KeyValuePair<string, string> envVar in envVars.Cast<DictionaryEntry>().Select(de => new KeyValuePair<string, string>((string)de.Key, (string)de.Value)))
            {
                Assert.NotNull(envVar.Key);
            }
        }

        public void EnvironmentVariablesAreHashtable()
        {
            // On NetFX, the type returned was always Hashtable
            Assert.IsType<Hashtable>(Environment.GetEnvironmentVariables());
        }

        [Theory]
        [InlineData(EnvironmentVariableTarget.Process)]
        [InlineData(EnvironmentVariableTarget.Machine)]
        [InlineData(EnvironmentVariableTarget.User)]
        public void EnvironmentVariablesAreHashtable(EnvironmentVariableTarget target)
        {
            // On NetFX, the type returned was always Hashtable
            Assert.IsType<Hashtable>(Environment.GetEnvironmentVariables(target));
        }

        [Theory]
        [InlineData(EnvironmentVariableTarget.Process)]
        [InlineData(EnvironmentVariableTarget.Machine)]
        [InlineData(EnvironmentVariableTarget.User)]
        public void EnumerateYieldsDictionaryEntryFromIEnumerable(EnvironmentVariableTarget target)
        {
            // GetEnvironmentVariables has always yielded DictionaryEntry from IEnumerable
            IDictionary vars = Environment.GetEnvironmentVariables(target);
            IEnumerator enumerator = ((IEnumerable)vars).GetEnumerator();
            if (enumerator.MoveNext())
            {
                Assert.IsType<DictionaryEntry>(enumerator.Current);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            }
        }

        [Theory]
        [InlineData(EnvironmentVariableTarget.Process)]
        [InlineData(EnvironmentVariableTarget.Machine)]
        [InlineData(EnvironmentVariableTarget.User)]
        public void EnumerateEnvironmentVariables(EnvironmentVariableTarget target)
        {
            bool lookForSetValue = (target == EnvironmentVariableTarget.Process) || PlatformDetection.IsWindowsAndElevated;

            const string key = "EnumerateEnvironmentVariables";
            string value = Path.GetRandomFileName();

            try
            {
                if (lookForSetValue)
                {
                    Environment.SetEnvironmentVariable(key, value, target);
                    Assert.Equal(value, Environment.GetEnvironmentVariable(key, target));
                }

                IDictionary results = Environment.GetEnvironmentVariables(target);

                // Ensure we can walk through the results
                IDictionaryEnumerator enumerator = results.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Assert.NotNull(enumerator.Entry);
                }

                if (lookForSetValue)
                {
                    // Ensure that we got our flagged value out
                    Assert.Equal(value, results[key]);
                }
            }
            finally
            {
                if (lookForSetValue)
                {
                    Environment.SetEnvironmentVariable(key, null, target);
                    Assert.Null(Environment.GetEnvironmentVariable(key, target));
                }
            }
        }

        private static void SetEnvironmentVariableWithPInvoke(string name, string value)
        {
            bool success =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                    SetEnvironmentVariable(name, value) :
                    (value != null ? setenv(name, value, 1) : unsetenv(name)) == 0;
            Assert.True(success);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        [DllImport("libc")]
        private static extern int setenv(string name, string value, int overwrite);

        [DllImport("libc")]
        private static extern int unsetenv(string name);
    }
}
