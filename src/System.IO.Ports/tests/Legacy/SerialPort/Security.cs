// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.IO;
using System.IO.Ports;
using System.IO.PortsTests;
using System.Security;
using Legacy.Support;
using Xunit;

public class Security_TestCase : PortsTest
{
    [ConditionalFact(nameof(HasOneSerialPort))]
    public void PermitOnly_UnmanagedCode()
    {
        SerialPort com1;
        bool retValue = true;

        Debug.WriteLine("PermitOnly UnmanagedCode");
        (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).PermitOnly();

        com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        //[] Open
        try
        {
            com1.Open();
        }
        catch (Exception e)
        {
            Debug.WriteLine("Open threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        CallEvents(com1);
        CallProperties(com1);
        CallMethods(com1);

        if (!retValue)
        {
            Debug.WriteLine("Err_001!!! PermitOnly UnmanagedCode FAILED");
        }
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void Deny_UnmanagedCode()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Deny UnmanagedCode");
        (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Deny();

        Assert.Throws<SecurityException>(() => com1.Open());
    }

    private void CallEvents(SerialPort com)
    {
        SerialErrorReceivedEventHandler serialErrorReceivedEventHandler = SerialErrorReceivedEventHandler;
        SerialPinChangedEventHandler serialPinChangedEventHandler = SerialPinChangedEventHandler;
        SerialDataReceivedEventHandler serialDataReceivedEventHandler = SerialDataReceivedEventHandler;

        //[] ErrorReceived Add 
        com.ErrorReceived += serialErrorReceivedEventHandler;

        //[] ErrorReceived Remove
        com.ErrorReceived -= serialErrorReceivedEventHandler;
        
        //[] PinChanged Add
        com.PinChanged += serialPinChangedEventHandler;
        
        //[] PinChanged Remove
        com.PinChanged -= serialPinChangedEventHandler;
        
        //[] DataReceived Add
        com.DataReceived += serialDataReceivedEventHandler;
        
        //[] DataReceived Remove
        com.DataReceived -= serialDataReceivedEventHandler;
    }

    private void CallProperties(SerialPort com)
    {
        //[] BaseStream get
        Stream stream = com.BaseStream;

        //[] BaudRate get 
        int baudRate = com.BaudRate;

        //[] BaudRate set
        com.BaudRate = 14400;

        //[] BreakState get
        bool breakState = com.BreakState;

        //[] BreakState set 
        com.BreakState = true;
        com.BreakState = false;
    
        //[] BytesToWrite get
        int bytesToWrite = com.BytesToWrite;
        
        //[] BytesToRead get
        int bytesToRead = com.BytesToRead;
        
        //[] CDHolding get
        bool cDHolding = com.CDHolding;
        
        //[] CtsHolding get
        bool ctsHolding = com.CtsHolding;
        
        //[] DataBits get
        int DataBits = com.DataBits;
        
        //[] DataBits set
        com.DataBits = 7;
        com.DataBits = 8;
        
        //[] DiscardNull get
        bool discardNull = com.DiscardNull;

        //[] DiscarNull set
        com.DiscardNull = true;
        com.DiscardNull = false;
        
        //[] DsrHolding get
        bool DsrHolding = com.DsrHolding;
        
        //[] DtrEnable get
        bool dtrEnable = com.DtrEnable;
        
        //[] DtrEnable set
        com.DtrEnable = true;
        com.DtrEnable = false;
        
        //[] Encoding get
        System.Text.Encoding encoding = com.Encoding;
        
        //[] Encoding set
        com.Encoding = System.Text.Encoding.UTF8;
        com.Encoding = System.Text.Encoding.ASCII;

        //[] Handshake get
        Handshake handshake = com.Handshake;

        //[] Handshake set
        com.Handshake = Handshake.RequestToSend;
        com.Handshake = Handshake.None;
        
        //[] IsOpen get
        bool isOpen = com.IsOpen;
        
        //[] NewLine get
        string newLine = com.NewLine;
        
        //[] NewLine set
        com.NewLine = "foo";
        com.NewLine = "\n";
        
        //[] Parity get
        Parity parity = com.Parity;
        
        //[] Parity set
        com.Parity = Parity.Even;
        com.Parity = Parity.None;
    
        //[] ParityReplace get
        byte parityReplace = com.ParityReplace;
        
        //[] ParityReplace set
        com.ParityReplace = 32;
        com.ParityReplace = 63;
        
        //[] PortName get
        string portName = com.PortName;

        //[] PortName set
        com.Close();
        com.PortName = "Com255";
        com.PortName = TCSupport.LocalMachineSerialInfo.FirstAvailablePortName;
        com.Open();

        //[] ReadBufferSize get
        int readBufferSize = com.ReadBufferSize;
        
        //[] ReadBufferSize set
        com.Close();
        com.ReadBufferSize = 8192;
        com.ReadBufferSize = 4096;
        com.Open();
        
        //[] ReadTimeout get
        int readTimeout = com.ReadTimeout;
        
        //[] ReadTimeout set
        com.ReadTimeout = 1000;
        com.ReadTimeout = -1;
        
        //[] ReceivedBytesThreshold get
        int receivedBytesThreshold = com.ReceivedBytesThreshold;
        
        //[] ReceivedBytesThreshold set
        com.ReceivedBytesThreshold = 1;
        com.ReceivedBytesThreshold = 8;
        
        //[] RtsEnable get
        bool rtsEnable = com.RtsEnable;
        
        //[] RtsEnable set
        com.RtsEnable = true;
        com.RtsEnable = false;
        
        //[] StopBits get
        StopBits stopBits = com.StopBits;
        
        //[] StopBits set
        com.StopBits = StopBits.Two;
        com.StopBits = StopBits.One;
        
        //[] WriteBufferSize get
        int writeBufferSize = com.WriteBufferSize;
        
        //[] WriteBufferSize set
        com.Close();
        com.WriteBufferSize = 8192;
        com.WriteBufferSize = 4096;
        com.Open();
        
        //[] WriteTimeout get
        int writeTimeout = com.WriteTimeout;
        
        //[] WriteTimeout set
        com.WriteTimeout = 1000;
        com.WriteTimeout = -1;
    }

    private void CallMethods(SerialPort com)
    {
        com.ReadTimeout = 0;

        //[] DiscardInBuffer
        com.DiscardInBuffer();

        //[] DiscardOutBuffer
        com.DiscardOutBuffer();

        //[] GetPortNames
        SerialPort.GetPortNames();

        //[] Read(byte[], int, int)
        try
        {
            com.Read(new byte[1], 0, 1);
        }
        catch (TimeoutException)
        {
        }

        //[] ReadChar
        try
        {
            com.ReadChar();
        }
        catch (TimeoutException)
        {
        }

        //[] Read(char[], int, int)
        try
        {
            com.Read(new char[1], 0, 1);
        }
        catch (TimeoutException)
        {
        }

        //[] ReadByte
        try
        {
            com.ReadByte();
        }
        catch (TimeoutException)
        {
        }

        //[] ReadExisting
        try
        {
            com.ReadByte();
        }
        catch (TimeoutException)
        {
        }

        //[] ReadLine
        try
        {
            com.ReadLine();
        }
        catch (TimeoutException)
        {
        }

        //[] ReadTo
        try
        {
            com.ReadTo("<END>");
        }
        catch (TimeoutException)
        {
        }

        //[] Write(string)
        com.Write("foo");

        //[] Write(char[], int, int)
        com.Write(new char[1], 0, 1);

        //[] Write(byte[], int, int)
        com.Write(new byte[1], 0, 1);

        //[] WriteLine
        com.WriteLine("foo");
    }

    private void SerialErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e) { }
    private void SerialPinChangedEventHandler(object sender, SerialPinChangedEventArgs e) { }
    private void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e) { }
}
