// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO.PortsTests;
using System.Text.RegularExpressions;
using Xunit;

namespace System.IO.Ports.Tests
{
    public class OpenDevices : PortsTest
    {
        private const string GuidDevInterfaceComPort = "86e0d1e0-8089-11d0-9ce4-08003e301f73"; // SerialPort GUID Class ID

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))] // ActiveIssue: https://github.com/dotnet/corefx/issues/29756
        [ActiveIssue("https://github.com/dotnet/corefx/issues/23294", TargetFrameworkMonikers.Uap)]
        public void OpenDevices01()
        {
            DosDevices dosDevices = new DosDevices();
            Regex comPortNameRegex = new Regex(@"com\d{1,3}", RegexOptions.IgnoreCase);
         
            // This test enumerates all DosDevices and attempts to open them, expecting an exception.
            // However, some non true Serial devices can successfully open!
            // Added check in SerialPort.Open() to throw exception if not a valid PortName & Also added condition check for valid ports below
            foreach (KeyValuePair<string, string> keyValuePair in dosDevices)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Key) && !comPortNameRegex.IsMatch(keyValuePair.Key) && !keyValuePair.Key.Contains(GuidDevInterfaceComPort))
                {
                    using (SerialPort testPort = new SerialPort(keyValuePair.Key))
                    {
                        Debug.WriteLine($"Checking exception thrown with Key {keyValuePair.Key}");
                        Assert.ThrowsAny<Exception>(() => testPort.Open());
                    }
                }

                if (!string.IsNullOrEmpty(keyValuePair.Value) && !comPortNameRegex.IsMatch(keyValuePair.Key) && !keyValuePair.Key.Contains(GuidDevInterfaceComPort))
                {
                    using (SerialPort testPort = new SerialPort(keyValuePair.Value))
                    {
                        Debug.WriteLine($"Checking exception thrown with Value {keyValuePair.Value}");
                        Assert.ThrowsAny<Exception>(() => testPort.Open());
                    }
                }
            }
        }
    }
}
