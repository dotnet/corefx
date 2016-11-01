// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Xunit;
using System.Text;

namespace System.Diagnostics.Tests
{
    public partial class ProcessStartInfoTests
    {
        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        public void TestEnvironmentVariablesPropertyWindows()
        {
            Assert.NotEqual(0, new Process().StartInfo.EnvironmentVariables.Count);

            ProcessStartInfo psi = new ProcessStartInfo();

            // Creating a detached ProcessStartInfo will pre-populate the environment
            // with current environmental variables.

            StringDictionary environmentVariables = psi.EnvironmentVariables;

            Assert.NotEqual(environmentVariables.Count, 0);

            int CountItems = environmentVariables.Count;

            environmentVariables.Add("NewKey", "NewValue");
            environmentVariables.Add("NewKey2", "NewValue2");

            Assert.Equal(CountItems + 2, environmentVariables.Count);
            environmentVariables.Remove("NewKey");
            Assert.Equal(CountItems + 1, environmentVariables.Count);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentException>(() => { environmentVariables.Add("NewKey2", "NewValue2"); });

            //Clear
            environmentVariables.Clear();
            Assert.Equal(0, environmentVariables.Count);

            //ContainsKey 
            environmentVariables.Add("NewKey", "NewValue");
            environmentVariables.Add("NewKey2", "NewValue2");
            Assert.True(environmentVariables.ContainsKey("NewKey"));
            Assert.True(environmentVariables.ContainsKey("NewKey2"));
            Assert.False(environmentVariables.ContainsKey("NewKey99"));

            //Contains Value
            Assert.True(environmentVariables.ContainsValue("NewValue"));
            Assert.True(environmentVariables.ContainsValue("NewValue2"));

            //Iterating
            string result = null;
            int index = 0;
            foreach (string e1 in environmentVariables.Values)
            {
                index++;
                result += e1;
            }
            Assert.Equal(2, index);
            Assert.Equal("NewValueNewValue2", result);

            result = null;
            index = 0;
            foreach (string e1 in environmentVariables.Keys)
            {
                index++;
                result += e1;
            }
            Assert.Equal("NewKeyNewKey2", result);
            Assert.Equal(2, index);

            result = null;
            index = 0;
            foreach (KeyValuePair<string, string> e1 in environmentVariables)
            {
                index++;
                result += e1.Key;
            }
            Assert.Equal("NewKeyNewKey2", result);
            Assert.Equal(2, index);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() => environmentVariables.ContainsKey(null));

            environmentVariables.Add("NewKey98","NewValue98");

            //Indexed
            string newIndexItem = environmentVariables["NewKey98"];
            Assert.Equal("NewValue98", newIndexItem);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(environmentVariables.ContainsKey("NeWkEy"));
                Assert.Equal("NewValue", environmentVariables["NeWKeY"]);
                Assert.Throws<ArgumentException>(() => environmentVariables.Add("newkey", "newvalue100"));
            }

            Assert.False(environmentVariables.ContainsKey("NewKey99"));

            //Key not found
            Assert.Throws<KeyNotFoundException>(() =>
            {
                string stringout = environmentVariables["NewKey99"];
            });            

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() =>
            {
                string stringout = environmentVariables[null];
            });

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() => environmentVariables.Add(null, "NewValue2"));

            //Invalid Key to add
            Assert.Throws<ArgumentException>(() => environmentVariables.Add("NewKey2", "NewValue2"));

            //Remove Item
            environmentVariables.Remove("NewKey98");

            //Exception not thrown with null key
            Assert.Throws<ArgumentNullException>(() => { environmentVariables.Remove(null); });

            //"Exception not thrown with null key"
            Assert.Throws<KeyNotFoundException>(() => environmentVariables["1bB"]);

            Assert.True(environmentVariables.ContainsKey("NewKey2"));
            Assert.True(environmentVariables.ContainsValue("NewValue2"));

            Assert.False(environmentVariables.ContainsValue("newvalue2"));

            //Use KeyValuePair Enumerator
            var x = environmentVariables.GetEnumerator() as IEnumerator<KeyValuePair<string, string>>;
            x.MoveNext();
            var y1 = x.Current;
            Assert.Equal("NewKey NewValue", y1.Key + " " + y1.Value);
            x.MoveNext();
            y1 = x.Current;
            Assert.Equal("NewKey2 NewValue2", y1.Key + " " + y1.Value);

            environmentVariables.Add("NewKey3", "NewValue3");
            //CopyTo
            KeyValuePair<string, string>[] kvpa = new KeyValuePair<string, string>[10];
            environmentVariables.CopyTo(kvpa, 0);
            Assert.Equal("NewKey", kvpa[0].Key);
            Assert.Equal("NewKey3", kvpa[2].Key);
            Assert.Equal("NewValue3", kvpa[2].Value);

            string[] kvp = new string[10];
            Assert.Throws<ArgumentException>(() => { environmentVariables.CopyTo(kvp, 6); });
            environmentVariables.CopyTo(kvpa, 6);
            Assert.Equal("NewKey", kvpa[6].Key);
            Assert.Equal("NewValue", kvpa[6].Value);

            Assert.Throws<ArgumentOutOfRangeException>(() => { environmentVariables.CopyTo(kvpa, -1); });

            Assert.Throws<ArgumentException>(() => { environmentVariables.CopyTo(kvpa, 9); });

            Assert.Throws<ArgumentNullException>(() =>
            {
                KeyValuePair<string, string>[] kvpanull = null;
                environmentVariables.CopyTo(kvpanull, 0);
            });
        }

        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        public void TestVerbsProperty()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = $"{Process.GetCurrentProcess().ProcessName}.exe";

            Assert.Contains("open", psi.Verbs, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("runas", psi.Verbs, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("runasuser", psi.Verbs, StringComparer.OrdinalIgnoreCase);
            Assert.DoesNotContain("printto", psi.Verbs, StringComparer.OrdinalIgnoreCase);
            Assert.DoesNotContain("closed", psi.Verbs, StringComparer.OrdinalIgnoreCase);
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]
        [Fact]
        public void TestEnvironmentVariablesPropertyUnix(){
            ProcessStartInfo psi = new ProcessStartInfo();

            // Creating a detached ProcessStartInfo will pre-populate the environment
            // with current environmental variables.

            StringDictionary environmentVariables = psi.EnvironmentVariables;

            Assert.NotEqual(0, environmentVariables.Count);

            int CountItems = environmentVariables.Count;

            environmentVariables.Add("NewKey", "NewValue");
            environmentVariables.Add("NewKey2", "NewValue2");

            Assert.Equal(CountItems + 2, environmentVariables.Count);
            environmentVariables.Remove("NewKey");
            Assert.Equal(CountItems + 1, environmentVariables.Count);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentException>(() => { environmentVariables.Add("NewKey2", "NewValue2"); });
            Assert.False(environmentVariables.ContainsKey("NewKey"));

            environmentVariables.Add("newkey2", "newvalue2");
            Assert.True(environmentVariables.ContainsKey("newkey2"));
            Assert.Equal("newvalue2", environmentVariables["newkey2"]);
            Assert.Equal("NewValue2", environmentVariables["NewKey2"]);

            environmentVariables.Clear();

            Assert.Equal(0, environmentVariables.Count);

            environmentVariables.Add("NewKey", "newvalue");
            environmentVariables.Add("newkey2", "NewValue2");
            Assert.False(environmentVariables.ContainsKey("newkey"));
            Assert.False(environmentVariables.ContainsValue("NewValue"));

            string result = null;
            int index = 0;
            foreach (string e1 in environmentVariables.Values)
            {
                index++;
                result += e1;
            }
            Assert.Equal(2, index);
            Assert.Equal("newvalueNewValue2", result);

            result = null;
            index = 0;
            foreach (string e1 in environmentVariables.Keys)
            {
                index++;
                result += e1;
            }
            Assert.Equal("NewKeynewkey2", result);
            Assert.Equal(2, index);

            result = null;
            index = 0;
            foreach (KeyValuePair<string, string> e1 in environmentVariables)
            {
                index++;
                result += e1.Key;
            }
            Assert.Equal("NewKeynewkey2", result);
            Assert.Equal(2, index);

            //Key not found
            Assert.Throws<KeyNotFoundException>(() =>
            {
                string stringout = environmentVariables["NewKey99"];
            });            

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() =>
            {
                string stringout = environmentVariables[null];
            });

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() => environmentVariables.Add(null, "NewValue2"));

            //Invalid Key to add
            Assert.Throws<ArgumentException>(() => environmentVariables.Add("newkey2", "NewValue2"));

            //Use KeyValuePair Enumerator
            var x = environmentVariables.GetEnumerator() as IEnumerator<KeyValuePair<string, string>>;
            x.MoveNext();
            var y1 = x.Current;
            Assert.Equal("NewKey newvalue", y1.Key + " " + y1.Value);
            x.MoveNext();
            y1 = x.Current;
            Assert.Equal("newkey2 NewValue2", y1.Key + " " + y1.Value);

            environmentVariables.Add("newkey3", "newvalue3");

            KeyValuePair<string, string>[] kvpa = new KeyValuePair<string, string>[10];
            environmentVariables.CopyTo(kvpa, 0);
            Assert.Equal("NewKey", kvpa[0].Key);
            Assert.Equal("newkey3", kvpa[2].Key);
            Assert.Equal("newvalue3", kvpa[2].Value);

            string[] kvp = new string[10];
            Assert.Throws<ArgumentException>(() => { environmentVariables.CopyTo(kvp, 6); });
            environmentVariables.CopyTo(kvpa, 6);
            Assert.Equal("NewKey", kvpa[6].Key);
            Assert.Equal("newvalue", kvpa[6].Value);

            Assert.Throws<ArgumentOutOfRangeException>(() => { environmentVariables.CopyTo(kvpa, -1); });

            Assert.Throws<ArgumentException>(() => { environmentVariables.CopyTo(kvpa, 9); });

            Assert.Throws<ArgumentNullException>(() =>
            {
                KeyValuePair<string, string>[] kvpanull = null;
                environmentVariables.CopyTo(kvpanull, 0);
            });
        }
    }
}
        