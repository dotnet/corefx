// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.PortsTests;
using System.Text.RegularExpressions;
using Xunit;

public class OpenDevices : PortsTest
{
    [Fact]
    public void OpenDevices01()
    {
        DosDevices dosDevices = new DosDevices();
        Regex comPortNameRegex = new Regex(@"com\d{1,3}", RegexOptions.IgnoreCase);

        foreach (KeyValuePair<string, string> keyValuePair in dosDevices)
        {
            if (!string.IsNullOrEmpty(keyValuePair.Key) && !comPortNameRegex.IsMatch(keyValuePair.Key))
            {
                using (SerialPort com1 = new SerialPort(keyValuePair.Key))
                {
                    Debug.WriteLine($"Checking exception thrown with Key {keyValuePair.Key}");
                    Assert.ThrowsAny<Exception>(() => com1.Open());
                }
            }

            if (!string.IsNullOrEmpty(keyValuePair.Value) && !comPortNameRegex.IsMatch(keyValuePair.Key))
            {
                using (SerialPort com1 = new SerialPort(keyValuePair.Value))
                {
                    Debug.WriteLine($"Checking exception thrown with Value {keyValuePair.Value}");
                    Assert.ThrowsAny<Exception>(() => com1.Open());
                }
            }
        }
    }
}
