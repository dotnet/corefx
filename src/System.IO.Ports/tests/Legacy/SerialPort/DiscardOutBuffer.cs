// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;
using System.Diagnostics;
using System.IO.PortsTests;
using Legacy.Support;
using Xunit;

public class DiscardOutBuffer : PortsTest
{
    //The string used with Write(str) to fill the input buffer
    public static readonly string DEFAULT_STRING = "Hello World";

    //The buffer lenght used whe filling the ouput buffer
    public static readonly int DEFAULT_BUFFER_LENGTH = 8;

    //Delegate to start asynchronous write on the SerialPort com with byte[] of size bufferLength
    public delegate void AsyncWriteDelegate(SerialPort com, int bufferLength);

    #region Test Cases

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void OutBufferFilled_Discard_Once()
    {
        IAsyncResult asyncResult;
        using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        {
            AsyncWriteDelegate write = new AsyncWriteDelegate(WriteRndByteArray);


            Debug.WriteLine("Verifying Discard method after input buffer has been filled");
            com1.Open();
            com1.WriteTimeout = 500;
            com1.Handshake = Handshake.RequestToSend;

            asyncResult = write.BeginInvoke(com1, DEFAULT_BUFFER_LENGTH, null, null);

            while (DEFAULT_BUFFER_LENGTH > com1.BytesToWrite)
                System.Threading.Thread.Sleep(50);

            VerifyDiscard(com1);

        //Wait for write method to timeout
        while (!asyncResult.IsCompleted)
            System.Threading.Thread.Sleep(100);
        }

    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void OutBufferFilled_Discard_Multiple()
    {
        using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        {
            AsyncWriteDelegate write = new AsyncWriteDelegate(WriteRndByteArray);
            IAsyncResult asyncResult;

            Debug.WriteLine("Verifying call Discard method several times after input buffer has been filled");

            com1.Open();
            com1.WriteTimeout = 500;
            com1.Handshake = Handshake.RequestToSend;

            asyncResult = write.BeginInvoke(com1, DEFAULT_BUFFER_LENGTH, null, null);

            while (DEFAULT_BUFFER_LENGTH > com1.BytesToWrite)
                System.Threading.Thread.Sleep(50);

            VerifyDiscard(com1);
            VerifyDiscard(com1);
            VerifyDiscard(com1);

            //Wait for write method to timeout
            while (!asyncResult.IsCompleted)
                System.Threading.Thread.Sleep(100);
        }
    }

    [ConditionalFact(nameof(HasOneSerialPort))]
    public void OutBufferFilled_Discard_Cycle()
    {
        IAsyncResult asyncResult;
        using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        {
            AsyncWriteDelegate write = new AsyncWriteDelegate(WriteRndByteArray);

            Debug.WriteLine("Verifying call Discard method after input buffer has been filled discarded and filled again");

            com1.Open();
            com1.WriteTimeout = 500;
            com1.Handshake = Handshake.RequestToSend;

            asyncResult = write.BeginInvoke(com1, DEFAULT_BUFFER_LENGTH, null, null);

            while (DEFAULT_BUFFER_LENGTH > com1.BytesToWrite)
                System.Threading.Thread.Sleep(50);

            VerifyDiscard(com1);

            //Wait for write method to timeout
            while (!asyncResult.IsCompleted)
                System.Threading.Thread.Sleep(100);

            asyncResult = write.BeginInvoke(com1, DEFAULT_BUFFER_LENGTH, null, null);

            while (DEFAULT_BUFFER_LENGTH > com1.BytesToWrite)
                System.Threading.Thread.Sleep(50);

            VerifyDiscard(com1);

        //Wait for write method to timeout
        while (!asyncResult.IsCompleted)
            System.Threading.Thread.Sleep(100);

        }
    }

    [ConditionalFact(nameof(HasNullModem))]
    public void InAndOutBufferFilled_Discard()
    {
        using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
        using (SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
        {
            AsyncWriteDelegate write = new AsyncWriteDelegate(WriteRndByteArray);
            IAsyncResult asyncResult;
            int origBytesToRead;

            Debug.WriteLine("Verifying Discard method after input buffer has been filled");

            com1.Open();
            com2.Open();
            com1.WriteTimeout = 500;

            com1.Handshake = Handshake.RequestToSend;
            com2.Write(DEFAULT_STRING);

            while (DEFAULT_STRING.Length > com1.BytesToRead)
            {
                System.Threading.Thread.Sleep(50);
            }

            asyncResult = write.BeginInvoke(com1, DEFAULT_BUFFER_LENGTH, null, null);
            origBytesToRead = com1.BytesToRead;

            while (DEFAULT_BUFFER_LENGTH > com1.BytesToWrite)
            {
                System.Threading.Thread.Sleep(50);
            }

            VerifyDiscard(com1);

            Assert.Equal(origBytesToRead,com1.BytesToRead);

            //Wait for write method to timeout
            while (!asyncResult.IsCompleted)
                System.Threading.Thread.Sleep(100);
        }
    }


    private void WriteRndByteArray(SerialPort com, int byteLength)
    {
        byte[] buffer = new byte[byteLength];
        Random rndGen = new Random();

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)rndGen.Next(0, 256);
        }

        try
        {
            com.Write(buffer, 0, buffer.Length);
        }
        catch (TimeoutException)
        {
        }
    }
    #endregion

    #region Verification for Test Cases
    private void VerifyDiscard(SerialPort com)
    {
        com.DiscardOutBuffer();
        Assert.Equal(0, com.BytesToWrite);
    }
    #endregion
}