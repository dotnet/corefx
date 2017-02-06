// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;
using System.IO.PortsTests;
using System.Threading;
using Legacy.Support;
using Xunit;

public class Open_stress : PortsTest
{
    [ConditionalFact(nameof(HasNullModem))]
    public void OpenReceiveData()
    {
        Thread workerThread = new Thread(OpenReceiveData_WorkerThread);
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Console.WriteLine("Open and Close port while the port is receiving data");

        _continueLoop = true;
        workerThread.Start();

        try
        {
            // TODO - Verify number of iterations (original is 1000 which makes test very slow)
            for (int i = 0; i < 10; ++i)
            {
                com.RtsEnable = true;
                com.Open();
                com.Close();
            }
        }
        finally
        {
            _continueLoop = false;

            if (com.IsOpen)
                com.Close();
        }

        workerThread.Join();
    }

    private volatile bool _continueLoop;

    private void OpenReceiveData_WorkerThread()
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        byte[] xmitBytes = new byte[16];

        for (int i = 0; i < xmitBytes.Length; ++i)
            xmitBytes[i] = (byte)i;

        try
        {
            com.Open();

            while (_continueLoop)
            {
                com.Write(xmitBytes, 0, xmitBytes.Length);
            }
        }
        finally
        {
            com.Close();
        }
    }

    [ConditionalFact(nameof(HasNullModem))]
    public void OpenReceiveDataAndRTS()
    {
        Thread workerThread = new Thread(OpenReceiveDataAndRTS_WorkerThread);
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Console.WriteLine("Open and Close port while the port is recieving data and the RTS pin is changing states");

        workerThread.Start();

        _continueLoop = true;

        byte[] xmitBytes = new byte[16];

        for (int i = 0; i < xmitBytes.Length; ++i)
            xmitBytes[i] = (byte)i;


        try
        {
            // TODO - Verify number of iterations (original is 1000 which makes test very slow)
            for (int i = 0; i < 10; ++i)
            {
                com.Open();
                com.Handshake = Handshake.RequestToSend;
                com.Write(xmitBytes, 0, xmitBytes.Length);
                com.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Thread1 threw the following exception:\n{0}", e);
        }
        finally
        {
            _continueLoop = false;

            if (com.IsOpen)
                com.Close();
        }

        workerThread.Join();
    }


    void OpenReceiveDataAndRTS_WorkerThread()
    {
        try
        {
            SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
            byte[] xmitBytes = new byte[16];

            for (int i = 0; i < xmitBytes.Length; ++i)
                xmitBytes[i] = (byte)i;

            com.Open();

            while (_continueLoop)
            {
                com.Write(xmitBytes, 0, xmitBytes.Length);
                com.RtsEnable = !com.RtsEnable;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Thread1 through the following exception:\n{0}", e);
        }
    }
}
