// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.PortsTests;
using Legacy.Support;
using Xunit;

public class ctor_IContainer : PortsTest
{
    [Fact]
    public void Verify()
    {
        SerialPortProperties serPortProp = new SerialPortProperties();
        System.ComponentModel.Container container = new System.ComponentModel.Container();
        using (SerialPort com = new SerialPort(container))
        {
            Assert.Equal(1, container.Components.Count);
            Assert.Equal(com, container.Components[0]);

            serPortProp.SetAllPropertiesToDefaults();

            Debug.WriteLine("Verifying properties is called");
            serPortProp.VerifyPropertiesAndPrint(com);
        }
    }
}
