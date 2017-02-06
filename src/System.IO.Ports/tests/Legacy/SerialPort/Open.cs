// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Legacy.Support;
using System.IO.Ports;
using System.IO.PortsTests;
using Xunit;

public class Open : PortsTest
{
    [ConditionalFact(nameof(HasOneSerialPort))]
    public void OpenDefault()
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPortProperties serPortProp = new SerialPortProperties();

        com.Open();
        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Console.WriteLine("BytesToWrite={0}", com.BytesToWrite);

        serPortProp.VerifyPropertiesAndPrint(com);

        if (com.IsOpen)
            com.Close();
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void OpenTwice()
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPortProperties serPortProp = new SerialPortProperties();

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Console.WriteLine("Verifying after calling Open() twice");

        com.Open();
        Assert.Throws<InvalidOperationException>(() => com.Open());

        serPortProp.VerifyPropertiesAndPrint(com);

        if (com.IsOpen)
            com.Close();
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void OpenTwoInstances()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPortProperties serPortProp1 = new SerialPortProperties();
        SerialPortProperties serPortProp2 = new SerialPortProperties();

        Console.WriteLine("Verifying calling Open() on two instances of SerialPort");
        serPortProp1.SetAllPropertiesToOpenDefaults();
        serPortProp1.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        serPortProp2.SetAllPropertiesToDefaults();
        serPortProp2.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        com1.Open();

        Assert.Throws<UnauthorizedAccessException>(() => com2.Open());

        serPortProp1.VerifyPropertiesAndPrint(com1);
        serPortProp2.VerifyPropertiesAndPrint(com2);

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();
    }
}
