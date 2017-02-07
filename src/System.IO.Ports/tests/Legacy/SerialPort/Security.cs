// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.IO;
using System.IO.Ports;
using Legacy.Support;

public class Security_TestCase : PortsTest
{
    public bool RunTest()
    {
        bool retValue = true;
        TCSupport tcSupport = new TCSupport();

        retValue &= tcSupport.BeginTestcase(new TestDelegate(PermitOnly_UnmanagedCode), TCSupport.SerialPortRequirements.OneSerialPort);
        retValue &= tcSupport.BeginTestcase(new TestDelegate(Deny_UnmanagedCode), TCSupport.SerialPortRequirements.OneSerialPort);

        _numErrors += tcSupport.NumErrors;
        _numTestcases = tcSupport.NumTestcases;
        _exitValue = tcSupport.ExitValue;

        return retValue;
    }

    public bool PermitOnly_UnmanagedCode()
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

        retValue &= CallEvents(com1);
        retValue &= CallProperties(com1);
        retValue &= CallMethods(com1);

        if (!retValue)
        {
            Debug.WriteLine("Err_001!!! PermitOnly UnmanagedCode FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        return retValue;
    }

    public bool Deny_UnmanagedCode()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        bool retValue = true;

        Debug.WriteLine("Deny UnmanagedCode");
        (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Deny();

        try
        {
            com1.Open();
            retValue = false;
            Debug.WriteLine("Expected ctor to throw SecurityException");
        }
        catch (System.Security.SecurityException) { }

        if (com1.IsOpen)
            com1.Close();

        return retValue;
    }

    private bool CallEvents(SerialPort com)
    {
        bool retValue = true;
        SerialErrorReceivedEventHandler serialErrorReceivedEventHandler = SerialErrorReceivedEventHandler;
        SerialPinChangedEventHandler serialPinChangedEventHandler = SerialPinChangedEventHandler;
        SerialDataReceivedEventHandler serialDataReceivedEventHandler = SerialDataReceivedEventHandler;

        //[] ErrorReceived Add 
        try
        {
            com.ErrorReceived += serialErrorReceivedEventHandler;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ErrorReceived Add threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ErrorReceived Remove
        try
        {
            com.ErrorReceived -= serialErrorReceivedEventHandler;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ErrorReceived Remove threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] PinChanged Add
        try
        {
            com.PinChanged += serialPinChangedEventHandler;
        }
        catch (Exception e)
        {
            Debug.WriteLine("PinChanged Add threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] PinChanged Remove
        try
        {
            com.PinChanged -= serialPinChangedEventHandler;
        }
        catch (Exception e)
        {
            Debug.WriteLine("PinChanged Remove threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DataReceived Add
        try
        {
            com.DataReceived += serialDataReceivedEventHandler;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DataReceived Add threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DataReceived Remove
        try
        {
            com.DataReceived -= serialDataReceivedEventHandler;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DataReceived Remove threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        return retValue;
    }

    private bool CallProperties(SerialPort com)
    {
        bool retValue = true;

        //[] BaseStream get
        try
        {
            Stream stream = com.BaseStream;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BaseStream get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] BaudRate get 
        try
        {
            int baudRate = com.BaudRate;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BaudRate get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] BaudRate set
        try
        {
            com.BaudRate = 14400;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BaudRate set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] BreakState get
        try
        {
            bool breakState = com.BreakState;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BreakState get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] BreakState set 
        try
        {
            com.BreakState = true;
            com.BreakState = false;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BaudRBreakStateate set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] BytesToWrite get
        try
        {
            int bytesToWrite = com.BytesToWrite;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BytesToWrite get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] BytesToRead get
        try
        {
            int bytesToRead = com.BytesToRead;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BytesToRead get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] CDHolding get
        try
        {
            bool cDHolding = com.CDHolding;
        }
        catch (Exception e)
        {
            Debug.WriteLine("CDHolding get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] CtsHolding get
        try
        {
            bool ctsHolding = com.CtsHolding;
        }
        catch (Exception e)
        {
            Debug.WriteLine("CtsHolding get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DataBits get
        try
        {
            int DataBits = com.DataBits;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DataBits get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DataBits set
        try
        {
            com.DataBits = 7;
            com.DataBits = 8;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DataBits set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DiscardNull get
        try
        {
            bool discardNull = com.DiscardNull;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DiscardNull get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DiscarNull set
        try
        {
            com.DiscardNull = true;
            com.DiscardNull = false;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DiscardNull set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DsrHolding get
        try
        {
            bool DsrHolding = com.DsrHolding;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DsrHolding get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DtrEnable get
        try
        {
            bool dtrEnable = com.DtrEnable;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DtrEnable get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DtrEnable set
        try
        {
            com.DtrEnable = true;
            com.DtrEnable = false;
        }
        catch (Exception e)
        {
            Debug.WriteLine("DtrEnable set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Encoding get
        try
        {
            System.Text.Encoding encoding = com.Encoding;
        }
        catch (Exception e)
        {
            Debug.WriteLine("Encoding get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Encoding set
        try
        {
            com.Encoding = System.Text.Encoding.UTF8;
            com.Encoding = System.Text.Encoding.ASCII;
        }
        catch (Exception e)
        {
            Debug.WriteLine("Encoding set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Handshake get
        try
        {
            Handshake handshake = com.Handshake;
        }
        catch (Exception e)
        {
            Debug.WriteLine("Handshake get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Handshake set
        try
        {
            com.Handshake = Handshake.RequestToSend;
            com.Handshake = Handshake.None;
        }
        catch (Exception e)
        {
            Debug.WriteLine("Handshake set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] IsOpen get
        try
        {
            bool isOpen = com.IsOpen;
        }
        catch (Exception e)
        {
            Debug.WriteLine("IsOpen get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] NewLine get
        try
        {
            string newLine = com.NewLine;
        }
        catch (Exception e)
        {
            Debug.WriteLine("NewLine get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] NewLine set
        try
        {
            com.NewLine = "foo";
            com.NewLine = "\n";
        }
        catch (Exception e)
        {
            Debug.WriteLine("NewLine set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Parity get
        try
        {
            Parity parity = com.Parity;
        }
        catch (Exception e)
        {
            Debug.WriteLine("Parity get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Parity set
        try
        {
            com.Parity = Parity.Even;
            com.Parity = Parity.None;
        }
        catch (Exception e)
        {
            Debug.WriteLine("Parity set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ParityReplace get
        try
        {
            byte parityReplace = com.ParityReplace;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ParityReplace get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ParityReplace set
        try
        {
            com.ParityReplace = 32;
            com.ParityReplace = 63;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ParityReplace set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] PortName get
        try
        {
            string portName = com.PortName;
        }
        catch (Exception e)
        {
            Debug.WriteLine("PortName get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] PortName set
        try
        {
            com.Close();
            com.PortName = "Com255";
            com.PortName = TCSupport.LocalMachineSerialInfo.FirstAvailablePortName;
            com.Open();
        }
        catch (Exception e)
        {
            Debug.WriteLine("PortName set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadBufferSize get
        try
        {
            int readBufferSize = com.ReadBufferSize;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ReadBufferSize get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadBufferSize set
        try
        {
            com.Close();
            com.ReadBufferSize = 8192;
            com.ReadBufferSize = 4096;
            com.Open();
        }
        catch (Exception e)
        {
            Debug.WriteLine("ReadBufferSize set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadTimeout get
        try
        {
            int readTimeout = com.ReadTimeout;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ReadTimeout get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadTimeout set
        try
        {
            com.ReadTimeout = 1000;
            com.ReadTimeout = -1;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ReadTimeout set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReceivedBytesThreshold get
        try
        {
            int receivedBytesThreshold = com.ReceivedBytesThreshold;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ReceivedBytesThreshold get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReceivedBytesThreshold set
        try
        {
            com.ReceivedBytesThreshold = 1;
            com.ReceivedBytesThreshold = 8;
        }
        catch (Exception e)
        {
            Debug.WriteLine("ReceivedBytesThreshold set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] RtsEnable get
        try
        {
            bool rtsEnable = com.RtsEnable;
        }
        catch (Exception e)
        {
            Debug.WriteLine("RtsEnable get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] RtsEnable set
        try
        {
            com.RtsEnable = true;
            com.RtsEnable = false;
        }
        catch (Exception e)
        {
            Debug.WriteLine("RtsEnable set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] StopBits get
        try
        {
            StopBits stopBits = com.StopBits;
        }
        catch (Exception e)
        {
            Debug.WriteLine("StopBits get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] StopBits set
        try
        {
            com.StopBits = StopBits.Two;
            com.StopBits = StopBits.One;
        }
        catch (Exception e)
        {
            Debug.WriteLine("StopBits set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] WriteBufferSize get
        try
        {
            int writeBufferSize = com.WriteBufferSize;
        }
        catch (Exception e)
        {
            Debug.WriteLine("WriteBufferSize get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] WriteBufferSize set
        try
        {
            com.Close();
            com.WriteBufferSize = 8192;
            com.WriteBufferSize = 4096;
            com.Open();
        }
        catch (Exception e)
        {
            Debug.WriteLine("WriteBufferSize set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] WriteTimeout get
        try
        {
            int writeTimeout = com.WriteTimeout;
        }
        catch (Exception e)
        {
            Debug.WriteLine("WriteTimeout get threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] WriteTimeout set
        try
        {
            com.WriteTimeout = 1000;
            com.WriteTimeout = -1;
        }
        catch (Exception e)
        {
            Debug.WriteLine("WriteTimeout set threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        return retValue;
    }

    private bool CallMethods(SerialPort com)
    {
        bool retValue = true;

        com.ReadTimeout = 0;

        //[] DiscardInBuffer
        try
        {
            com.DiscardInBuffer();
        }
        catch (Exception e)
        {
            Debug.WriteLine("DiscardInBuffer threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] DiscardOutBuffer
        try
        {
            com.DiscardOutBuffer();
        }
        catch (Exception e)
        {
            Debug.WriteLine("DiscardOutBuffer threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] GetPortNames
        try
        {
            SerialPort.GetPortNames();
        }
        catch (Exception e)
        {
            Debug.WriteLine("GetPortNames threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Read(byte[], int, int)
        try
        {
            com.Read(new byte[1], 0, 1);
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("Read(byte[], int, int) threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadChar
        try
        {
            com.ReadChar();
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("ReadChar threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Read(char[], int, int)
        try
        {
            com.Read(new char[1], 0, 1);
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("Read(char[], int, int) threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadByte
        try
        {
            com.ReadByte();
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("ReadByte threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadExisting
        try
        {
            com.ReadByte();
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("ReadExisting threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadLine
        try
        {
            com.ReadLine();
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("ReadLine threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] ReadTo
        try
        {
            com.ReadTo("<END>");
        }
        catch (TimeoutException) { }
        catch (Exception e)
        {
            Debug.WriteLine("ReadTo threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Write(string)
        try
        {
            com.Write("foo");
        }
        catch (Exception e)
        {
            Debug.WriteLine("Write(string) threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Write(char[], int, int)
        try
        {
            com.Write(new char[1], 0, 1);
        }
        catch (Exception e)
        {
            Debug.WriteLine("Write(char[], int, int) threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] Write(byte[], int, int)
        try
        {
            com.Write(new byte[1], 0, 1);
        }
        catch (Exception e)
        {
            Debug.WriteLine("Write(byte[], int, int) threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        //[] WriteLine
        try
        {
            com.WriteLine("foo");
        }
        catch (Exception e)
        {
            Debug.WriteLine("WriteLine threw the following unexpected exception:");
            Debug.WriteLine(e);
            retValue = false;
        }

        return retValue;
    }

    private void SerialErrorReceivedEventHandler(Object sender, SerialErrorReceivedEventArgs e) { }
    private void SerialPinChangedEventHandler(Object sender, SerialPinChangedEventArgs e) { }
    private void SerialDataReceivedEventHandler(Object sender, SerialDataReceivedEventArgs e) { }
}
