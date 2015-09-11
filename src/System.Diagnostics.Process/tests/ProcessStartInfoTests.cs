﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Xunit;
using System.Text;

namespace System.Diagnostics.ProcessTests
{
    public class ProcessStartInfoTests : ProcessTestBase
    {
        [Fact]
        public void TestEnvironmentProperty()
        {
            Assert.NotEqual(0, new Process().StartInfo.Environment.Count);

            ProcessStartInfo psi = new ProcessStartInfo();

            // Creating a detached ProcessStartInfo will pre-populate the environment
            // with current environmental variables.

            IDictionary<string, string> environment = psi.Environment;

            Assert.NotEqual(environment.Count, 0);

            int CountItems = environment.Count;

            environment.Add("NewKey", "NewValue");
            environment.Add("NewKey2", "NewValue2");

            Assert.Equal(CountItems + 2, environment.Count);
            environment.Remove("NewKey");
            Assert.Equal(CountItems + 1, environment.Count);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentException>(() => { environment.Add("NewKey2", "NewValue2"); });

            //Clear
            environment.Clear();
            Assert.Equal(0, environment.Count);

            //ContainsKey 
            environment.Add("NewKey", "NewValue");
            environment.Add("NewKey2", "NewValue2");
            Assert.True(environment.ContainsKey("NewKey"));

            Assert.Equal(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), environment.ContainsKey("newkey"));
            Assert.False(environment.ContainsKey("NewKey99"));

            //Iterating
            string result = null;
            int index = 0;
            foreach (string e1 in environment.Values)
            {
                index++;
                result += e1;
            }
            Assert.Equal(2, index);
            Assert.Equal("NewValueNewValue2", result);

            result = null;
            index = 0;
            foreach (string e1 in environment.Keys)
            {
                index++;
                result += e1;
            }
            Assert.Equal("NewKeyNewKey2", result);
            Assert.Equal(2, index);

            result = null;
            index = 0;
            foreach (KeyValuePair<string, string> e1 in environment)
            {
                index++;
                result += e1.Key;
            }
            Assert.Equal("NewKeyNewKey2", result);
            Assert.Equal(2, index);

            //Contains
            Assert.True(environment.Contains(new KeyValuePair<string, string>("NewKey", "NewValue")));
            Assert.Equal(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), environment.Contains(new KeyValuePair<string, string>("nEwKeY", "NewValue")));
            Assert.False(environment.Contains(new KeyValuePair<string, string>("NewKey99", "NewValue99")));

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() => environment.Contains(new KeyValuePair<string, string>(null, "NewValue99")));

            environment.Add(new KeyValuePair<string, string>("NewKey98", "NewValue98"));

            //Indexed
            string newIndexItem = environment["NewKey98"];
            Assert.Equal("NewValue98", newIndexItem);

            //TryGetValue
            string stringout = null;
            Assert.True(environment.TryGetValue("NewKey", out stringout));
            Assert.Equal("NewValue", stringout);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(environment.TryGetValue("NeWkEy", out stringout));
                Assert.Equal("NewValue", stringout);
            }

            stringout = null;
            Assert.False(environment.TryGetValue("NewKey99", out stringout));
            Assert.Equal(null, stringout);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() =>
            {
                string stringout1 = null;
                environment.TryGetValue(null, out stringout1);
            });

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() => environment.Add(null, "NewValue2"));

            //Invalid Key to add
            Assert.Throws<ArgumentException>(() => environment.Add("NewKey2", "NewValue2"));

            //Remove Item
            environment.Remove("NewKey98");
            environment.Remove("NewKey98");   //2nd occurrence should not assert

            //Exception not thrown with null key
            Assert.Throws<ArgumentNullException>(() => { environment.Remove(null); });

            //"Exception not thrown with null key"
            Assert.Throws<KeyNotFoundException>(() => environment["1bB"]);

            Assert.True(environment.Contains(new KeyValuePair<string, string>("NewKey2", "NewValue2")));
            Assert.Equal(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), environment.Contains(new KeyValuePair<string, string>("NEWKeY2", "NewValue2")));

            Assert.False(environment.Contains(new KeyValuePair<string, string>("NewKey2", "newvalue2")));
            Assert.False(environment.Contains(new KeyValuePair<string, string>("newkey2", "newvalue2")));

            //Use KeyValuePair Enumerator
            var x = environment.GetEnumerator();
            x.MoveNext();
            var y1 = x.Current;
            Assert.Equal("NewKey NewValue", y1.Key + " " + y1.Value);
            x.MoveNext();
            y1 = x.Current;
            Assert.Equal("NewKey2 NewValue2", y1.Key + " " + y1.Value);

            //IsReadonly
            Assert.False(environment.IsReadOnly);

            environment.Add(new KeyValuePair<string, string>("NewKey3", "NewValue3"));
            environment.Add(new KeyValuePair<string, string>("NewKey4", "NewValue4"));


            //CopyTo
            KeyValuePair<string, string>[] kvpa = new KeyValuePair<string, string>[10];
            environment.CopyTo(kvpa, 0);
            Assert.Equal("NewKey", kvpa[0].Key);
            Assert.Equal("NewKey3", kvpa[2].Key);

            environment.CopyTo(kvpa, 6);
            Assert.Equal("NewKey", kvpa[6].Key);

            //Exception not thrown with null key
            Assert.Throws<ArgumentOutOfRangeException>(() => { environment.CopyTo(kvpa, -1); });

            //Exception not thrown with null key
            Assert.Throws<ArgumentException>(() => { environment.CopyTo(kvpa, 9); });

            //Exception not thrown with null key
            Assert.Throws<ArgumentNullException>(() =>
            {
                KeyValuePair<string, string>[] kvpanull = null;
                environment.CopyTo(kvpanull, 0);
            });
        }

        [Fact]
        public void TestEnvironmentOfChildProcess()
        {
            const string UnicodeEnvVar = "TestEnvironmentOfChildProcess";
            Environment.SetEnvironmentVariable(UnicodeEnvVar, "\x1234\x5678"); // ensure some Unicode characters are in the output
            try
            {
                var expectedEnv = new HashSet<string>();
                var actualEnv = new HashSet<string>();
                Process p = CreateProcess(() =>
                {
                    foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
                    {
                        Console.WriteLine(envVar.Key + "=" + envVar.Value);
                    }

                    return SuccessExitCode;
                });
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                p.StartInfo.RedirectStandardOutput = true;
                p.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        expectedEnv.Add(e.Data);
                    }
                };

                p.Start();
                p.BeginOutputReadLine();

                foreach (KeyValuePair<string, string> envVar in p.StartInfo.Environment)
                {
                    actualEnv.Add(envVar.Key + "=" + envVar.Value);
                }

                Assert.True(p.WaitForExit(WaitInMS));
                p.WaitForExit(); // This ensures async event handlers are finished processing.

                // Validate against StartInfo.Environment
                if (!expectedEnv.SetEquals(actualEnv))
                {
                    var expected = string.Join(", ", expectedEnv.Except(actualEnv));
                    var actual = string.Join(", ", actualEnv.Except(expectedEnv));

                    Assert.True(false, string.Format("Expected: {0}{1}Actual: {2}", expected, Environment.NewLine, actual));
                }

                // Validate against current process
                var currentProcEnv = new HashSet<string>();
                foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
                {
                    currentProcEnv.Add(envVar.Key + "=" + envVar.Value);
                }

                // Profilers / code coverage tools can add own environment variables but we start
                // child process without them. Thus the set of variables from child process will
                // compose subset of variables from current process.
                // But in case if tests running directly through the Xunit runner, sets will be equal
                // and Assert.ProperSubset will throw. We add null to avoid this.
                currentProcEnv.Add(null);
                Assert.ProperSubset(currentProcEnv, actualEnv);
            }
            finally
            {
                Environment.SetEnvironmentVariable(UnicodeEnvVar, null);
            }
        }

        [Fact]
        public void TestUseShellExecuteProperty()
        {
            ProcessStartInfo psi = new ProcessStartInfo();

            // Calling the setter
            psi.UseShellExecute = false;
            Assert.Throws<PlatformNotSupportedException>(() => { psi.UseShellExecute = true; });

            // Calling the getter
            Assert.False(psi.UseShellExecute, "UseShellExecute=true is not supported on onecore.");
        }

        [Fact]
        public void TestArgumentsProperty()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            Assert.Equal(string.Empty, psi.Arguments);

            psi = new ProcessStartInfo("filename", "-arg1 -arg2");
            Assert.Equal("-arg1 -arg2", psi.Arguments);

            psi.Arguments = "-arg3 -arg4";
            Assert.Equal("-arg3 -arg4", psi.Arguments);
        }

        [Theory, InlineData(true), InlineData(false)]
        public void TestCreateNoWindowProperty(bool value)
        {
            Process testProcess = CreateProcessInfinite();
            try
            {
                testProcess.StartInfo.CreateNoWindow = value;
                testProcess.Start();

                Assert.Equal(value, testProcess.StartInfo.CreateNoWindow);
            }
            finally
            {
                if (!testProcess.HasExited)
                    testProcess.Kill();

                Assert.True(testProcess.WaitForExit(WaitInMS));
            }
        }

        [Fact, PlatformSpecific(PlatformID.Windows), OuterLoop] // Requires admin privileges
        public void TestUserCredentialsPropertiesOnWindows()
        {
            string username = "test", password = "PassWord123!!";
            try
            {
                Interop.NetUserAdd(username, password);
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine("TestUserCredentialsPropertiesOnWindows: NetUserAdd failed: {0}", exc.Message);
                return; // test is irrelevant if we can't add a user
            }

            Process p = CreateProcessInfinite();

            p.StartInfo.LoadUserProfile = true;
            p.StartInfo.UserName = username;
            p.StartInfo.PasswordInClearText = password;

            SafeProcessHandle handle = null;
            try
            {
                p.Start();
                if (Interop.OpenProcessToken(p.SafeHandle, 0x8u, out handle))
                {
                    SecurityIdentifier sid;
                    if (Interop.ProcessTokenToSid(handle, out sid))
                    {
                        string actualUserName = sid.Translate(typeof(NTAccount)).ToString();
                        int indexOfDomain = actualUserName.IndexOf('\\');
                        if (indexOfDomain != -1)
                            actualUserName = actualUserName.Substring(indexOfDomain + 1);

                        bool isProfileLoaded = GetNamesOfUserProfiles().Any(profile => profile.Equals(username));

                        Assert.Equal(username, actualUserName);
                        Assert.True(isProfileLoaded);
                    }
                }
            }
            finally
            {
                if (handle != null)
                    handle.Dispose();

                if (!p.HasExited)
                    p.Kill();

                Interop.NetUserDel(null, username);
                Assert.True(p.WaitForExit(WaitInMS));
            }
        }

        [Fact, PlatformSpecific(PlatformID.AnyUnix)]
        public void TestUserCredentialsPropertiesOnUnix()
        {
            Assert.Throws<PlatformNotSupportedException>(() => _process.StartInfo.Domain);
            Assert.Throws<PlatformNotSupportedException>(() => _process.StartInfo.UserName);
            Assert.Throws<PlatformNotSupportedException>(() => _process.StartInfo.PasswordInClearText);
            Assert.Throws<PlatformNotSupportedException>(() => _process.StartInfo.LoadUserProfile);
        }

        [Fact]
        public void TestWorkingDirectoryProperty()
        {
            // check defaults
            Assert.Equal(string.Empty, _process.StartInfo.WorkingDirectory);

            Process p = CreateProcessInfinite();
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            try
            {
                p.Start();
                Assert.Equal(Directory.GetCurrentDirectory(), p.StartInfo.WorkingDirectory);
            }
            finally
            {
                if (!p.HasExited)
                    p.Kill();

                Assert.True(p.WaitForExit(WaitInMS));
            }
        }

        private static List<string> GetNamesOfUserProfiles()
        {
            List<string> userNames = new List<string>();

            string[] names = Registry.Users.GetSubKeyNames();
            for (int i = 1; i < names.Length; i++)
            {
                try
                {
                    SecurityIdentifier sid = new SecurityIdentifier(names[i]);
                    string userName = sid.Translate(typeof(NTAccount)).ToString();
                    int indexofDomain = userName.IndexOf('\\');
                    if (indexofDomain != -1)
                    {
                        userName = userName.Substring(indexofDomain + 1);
                        userNames.Add(userName);
                    }
                }
                catch (Exception) { }
            }

            return userNames;
        }
    }
}
