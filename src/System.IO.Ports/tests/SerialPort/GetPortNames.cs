// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.PortsTests;
using System.Linq;
using Legacy.Support;
using Xunit;

namespace System.IO.Ports.Tests
{
    public class GetPortNames : PortsTest
    {
        #region Test Cases

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/20588 - GetPortNames() has registry dependency.", TargetFrameworkMonikers.UapAot)]
        private void OpenEveryPortName()
        {
            foreach (string portName in SerialPort.GetPortNames())
            {
                Debug.WriteLine("Opening port " + portName);
                using (SerialPort serialPort = new SerialPort(portName))
                {
                    try
                    {
                        serialPort.Open();
                    }
                    catch (UnauthorizedAccessException) { }
                }
            }
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/20588 - GetPortNames() has registry dependency.", TargetFrameworkMonikers.UapAot)]
        private void AllHelperPortsAreInGetPortNames()
        {
            string[] serialPortNames = SerialPort.GetPortNames();
            foreach (string helperPortName in PortHelper.GetPorts())
            {
                Assert.True(serialPortNames.Contains(helperPortName),
                    $"{helperPortName} is not present in SerialPort.GetPortNames result");
            }
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/20588 - GetPortNames() has registry dependency.", TargetFrameworkMonikers.UapAot)]
        private void AllGetPortNamesAreInHelperPorts()
        {
            string[] helperPortNames = PortHelper.GetPorts();
            foreach (string serialPortName in SerialPort.GetPortNames())
            {
                Assert.True(helperPortNames.Contains(serialPortName),
                    $"{serialPortName} is not present in PortHelper.GetPorts result");
            }
        }

        #endregion

    }
}

