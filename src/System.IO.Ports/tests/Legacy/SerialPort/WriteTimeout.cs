// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.PortsTests;
using Legacy.Support;
using Xunit;

public class WriteTimeout_Property : PortsTest
{
    //The default number of chars to write with when testing timeout with Write(char[], int, int)
    public static readonly int DEFAULT_WRITE_CHAR_ARRAY_SIZE = 8;

    //The default number of bytes to write with when testing timeout with Write(byte[], int, int)
    public static readonly int DEFAULT_WRITE_BYTE_ARRAY_SIZE = 8;

    //The ammount of time to wait when expecting an infinite timeout
    public static readonly int DEFAULT_WAIT_INFINITE_TIMEOUT = 250;

    //The maximum acceptable time allowed when a write method should timeout immediately
    public static readonly int MAX_ACCEPTABLE_ZERO_TIMEOUT = 100;

    //The maximum acceptable time allowed when a write method should timeout immediately when it is called for the first time
    public static readonly int MAX_ACCEPTABLE_WARMUP_ZERO_TIMEOUT = 5000;

    //The default string to write with when testing timeout with Write(str)
    public static readonly string DEFAULT_STRING_TO_WRITE = "TEST";
    public static readonly int NUM_TRYS = 5;

    public delegate void WriteMethodDelegate(SerialPort com);

    private enum ThrowAt { Set, Open };

    #region Test Cases
    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Default_Write_byte_int_int()
    {
        Debug.WriteLine("Verifying default WriteTimeout with Write(byte[] buffer, int offset, int count)");
        VerifyInfiniteTimeout(Write_byte_int_int, false);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Default_Write_char_int_int()
    {
        Debug.WriteLine("Verifying default WriteTimeout with Write(char[] buffer, int offset, int count)");
        VerifyInfiniteTimeout(Write_char_int_int, false);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Default_Write_str()
    {
        Debug.WriteLine("Verifying default WriteTimeout with Write(string)");
        VerifyInfiniteTimeout(Write_str, false);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Default_WriteLine()
    {
        Debug.WriteLine("Verifying default WriteTimeout with WriteLine()");
        VerifyInfiniteTimeout(WriteLine, false);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Infinite_Write_byte_int_int()
    {
        Debug.WriteLine("Verifying infinite WriteTimeout with Write(byte[] buffer, int offset, int count)");
        VerifyInfiniteTimeout(Write_byte_int_int, true);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Infinite_Write_char_int_int()
    {
        Debug.WriteLine("Verifying infinite WriteTimeout with Write(char[] buffer, int offset, int count)");
        VerifyInfiniteTimeout(Write_char_int_int, true);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Infinite_Write_str()
    {
        Debug.WriteLine("Verifying infinite WriteTimeout with Write(string)");
        VerifyInfiniteTimeout(Write_str, true);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Infinite_WriteLine()
    {
        Debug.WriteLine("Verifying infinite WriteTimeout with WriteLine()");
        VerifyInfiniteTimeout(WriteLine, true);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_Write_byte_int_int_BeforeOpen()
    {
        Debug.WriteLine("Verifying setting WriteTimeout=1 before Open() with Write(byte[] buffer, int offset, int count)");
        Verify1TimeoutBeforeOpen(Write_byte_int_int);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_Write_char_int_int_BeforeOpen()
    {
        Debug.WriteLine("Verifying setting WriteTimeout=1 before Open() with Write(char[] buffer, int offset, int count)");
        Verify1TimeoutBeforeOpen(Write_char_int_int);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_Write_str_BeforeOpen()
    {
      Debug.WriteLine("Verifying 1 WriteTimeout before Open with Write(string)");
        Verify1TimeoutBeforeOpen(Write_str);
    }


    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_WriteLine_BeforeOpen()
    {
        Debug.WriteLine("Verifying 1 WriteTimeout before Open with WriteLine()");
        Verify1TimeoutBeforeOpen(WriteLine);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_Write_byte_int_int_AfterOpen()
    {
        Debug.WriteLine("Verifying setting WriteTimeout=1 after Open() with Write(byte[] buffer, int offset, int count)");
        Verify1TimeoutAfterOpen(Write_byte_int_int);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_Write_char_int_int_AfterOpen()
    {
        Debug.WriteLine("Verifying setting WriteTimeout=1 after Open() with Write(char[] buffer, int offset, int count)");
        Verify1TimeoutAfterOpen(Write_char_int_int);
   }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_Write_str_AfterOpen()
    {
        Debug.WriteLine("Verifying 1 WriteTimeout after Open with Write(string)");
        Verify1TimeoutAfterOpen(Write_str);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_1_WriteLine_AfterOpen()
    {
        Debug.WriteLine("Verifying 1 WriteTimeout after Open with WriteLine()");
        Verify1TimeoutAfterOpen(WriteLine);
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_Int32MinValue()
    {
        Debug.WriteLine("Verifying Int32.MinValue WriteTimeout");
        VerifyException(int.MinValue, ThrowAt.Set, typeof(ArgumentOutOfRangeException));
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void WriteTimeout_NEG2()
    {
        Debug.WriteLine("Verifying -2 WriteTimeout");
        VerifyException(int.MinValue, ThrowAt.Set, typeof(ArgumentOutOfRangeException));
    }
    #endregion

    #region Verification for Test Cases

    private void VerifyInfiniteTimeout(WriteMethodDelegate readMethod, bool setInfiniteTimeout)
    {
        using (SerialPort com1 = TCSupport.InitFirstSerialPort())
        {
            WriteDelegateThread readThread = new WriteDelegateThread(com1, readMethod);
            System.Threading.Thread t = new System.Threading.Thread(readThread.CallWrite);
            SerialPortProperties serPortProp = new SerialPortProperties();
        
            serPortProp.SetAllPropertiesToOpenDefaults();
            serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

            com1.Handshake = Handshake.RequestToSend;

            serPortProp.SetProperty("ReadTimeout", 10);
            com1.ReadTimeout = 10;

            com1.Open();

            if (setInfiniteTimeout)
            {
                com1.WriteTimeout = 500;
                com1.WriteTimeout = SerialPort.InfiniteTimeout;
            }

            t.Start();
            System.Threading.Thread.Sleep(DEFAULT_WAIT_INFINITE_TIMEOUT);

            Assert.True(t.IsAlive);

            com1.Handshake = Handshake.None;

            while (t.IsAlive)
                System.Threading.Thread.Sleep(10);

            com1.DiscardOutBuffer();
            // If we're looped-back, then there will be data queud on the receive side which we need to discard
            com1.DiscardInBuffer();
            serPortProp.VerifyPropertiesAndPrint(com1);
        }
    }

    private void Verify1TimeoutBeforeOpen(WriteMethodDelegate readMethod)
    {
        using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        {
            com.WriteTimeout = 1;
            com.Open();

            Verify1Timeout(com, readMethod);
        }
    }

    private void Verify1TimeoutAfterOpen(WriteMethodDelegate readMethod)
    {
        using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        {
            com.Open();
            com.WriteTimeout = 1;

            Verify1Timeout(com, readMethod);
        }
    }

    private void Verify1Timeout(SerialPort com, WriteMethodDelegate readMethod)
    {
        SerialPortProperties serPortProp = new SerialPortProperties();
        Stopwatch sw = new Stopwatch();
        
        int actualTime = 0;

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        serPortProp.SetProperty("WriteTimeout", 1);

        serPortProp.SetProperty("Handshake", Handshake.RequestToSend);
        com.Handshake = Handshake.RequestToSend;

        serPortProp.SetProperty("ReadTimeout", 1000);
        com.ReadTimeout = 1000;

        System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
        sw.Start();
        readMethod(com);
        sw.Stop();

        if (MAX_ACCEPTABLE_WARMUP_ZERO_TIMEOUT < sw.ElapsedMilliseconds)
        {
            Fail("Err_2570ajdlkj!!! Write Method {0} timed out in {1}ms expected something less then {2}ms", readMethod.Method.Name, sw.ElapsedMilliseconds, MAX_ACCEPTABLE_WARMUP_ZERO_TIMEOUT);
        }
        sw.Reset();

        for (int i = 0; i < NUM_TRYS; i++)
        {
            sw.Start();
            readMethod(com);
            sw.Stop();

            actualTime += (int)sw.ElapsedMilliseconds;
            sw.Reset();
        }

        System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
        actualTime /= NUM_TRYS;

        if (MAX_ACCEPTABLE_ZERO_TIMEOUT < actualTime)
        {
            Fail("ERROR!!! Write Method {0} timed out in {1}ms expected something less then {2}ms", readMethod.Method.Name, actualTime, MAX_ACCEPTABLE_ZERO_TIMEOUT);
        }

        serPortProp.VerifyPropertiesAndPrint(com);
    }


    private void VerifyException(int writeTimeout, ThrowAt throwAt, Type expectedException)
    {
        using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        {
            VerifyExceptionAtOpen(com, writeTimeout, throwAt, expectedException);

            if (com.IsOpen)
                com.Close();

            VerifyExceptionAfterOpen(com, writeTimeout, expectedException);
        }
    }

    private void VerifyExceptionAtOpen(SerialPort com, int writeTimeout, ThrowAt throwAt, Type expectedException)
    {
        int origWriteTimeout = com.WriteTimeout;
        
        SerialPortProperties serPortProp = new SerialPortProperties();

        serPortProp.SetAllPropertiesToDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        if (ThrowAt.Open == throwAt)
            serPortProp.SetProperty("WriteTimeout", writeTimeout);

        try
        {
            com.WriteTimeout = writeTimeout;

            if (ThrowAt.Open == throwAt)
                com.Open();

            if (null != expectedException)
            {
                Fail("ERROR!!! Expected Open() to throw {0} and nothing was thrown", expectedException);
            }
        }
        catch (Exception e)
        {
            if (null == expectedException)
            {
                Fail("ERROR!!! Expected Open() NOT to throw an exception and {0} was thrown", e.GetType());
            }
            else if (e.GetType() != expectedException)
            {
                Fail("ERROR!!! Expected Open() throw {0} and {1} was thrown", expectedException, e.GetType());
            }
        }

        serPortProp.VerifyPropertiesAndPrint(com);
        com.WriteTimeout = origWriteTimeout;
    }


    private void VerifyExceptionAfterOpen(SerialPort com, int writeTimeout, Type expectedException)
    {
        SerialPortProperties serPortProp = new SerialPortProperties();

        com.Open();
        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        try
        {
            com.WriteTimeout = writeTimeout;

            if (null != expectedException)
            {
                Fail("ERROR!!! Expected setting the WriteTimeout after Open() to throw {0} and nothing was thrown", expectedException);
            }
        }
        catch (Exception e)
        {
            if (null == expectedException)
            {
                Fail("ERROR!!! Expected setting the WriteTimeout after Open() NOT to throw an exception and {0} was thrown", e.GetType());
            }
            else if (e.GetType() != expectedException)
            {
                Fail("ERROR!!! Expected setting the WriteTimeout after Open() throw {0} and {1} was thrown", expectedException, e.GetType());
            }
        }
        serPortProp.VerifyPropertiesAndPrint(com);
    }


    private void Write_byte_int_int(SerialPort com)
    {
        try
        {
            com.Write(new byte[DEFAULT_WRITE_BYTE_ARRAY_SIZE], 0, DEFAULT_WRITE_BYTE_ARRAY_SIZE);
        }
        catch (TimeoutException)
        {
        }
    }


    private void Write_char_int_int(SerialPort com)
    {
        try
        {
            com.Write(new char[DEFAULT_WRITE_CHAR_ARRAY_SIZE], 0, DEFAULT_WRITE_CHAR_ARRAY_SIZE);
        }
        catch (TimeoutException)
        {
        }
    }


    private void Write_str(SerialPort com)
    {
        try
        {
            com.Write(DEFAULT_STRING_TO_WRITE);
        }
        catch (TimeoutException)
        {
        }
    }


    private void WriteLine(SerialPort com)
    {
        try
        {
            com.WriteLine(DEFAULT_STRING_TO_WRITE);
        }
        catch (TimeoutException)
        {
        }
    }

    public class WriteDelegateThread
    {
        public WriteDelegateThread(SerialPort com, WriteMethodDelegate writeMethod)
        {
            _com = com;
            _writeMethod = writeMethod;
        }

        public void CallWrite()
        {
            _writeMethod(_com);
        }

        private readonly WriteMethodDelegate _writeMethod;
        private readonly SerialPort _com;
    }

    #endregion
}
