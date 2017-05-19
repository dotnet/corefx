// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.PortsTests;
using System.Threading;
using Legacy.Support;
using Xunit;

namespace System.IO.Ports.Tests
{
    public class Open_stress : PortsTest
    {
        [ConditionalFact(nameof(HasNullModem))]
        public void OpenReceiveData()
        {
            Thread workerThread = new Thread(OpenReceiveData_WorkerThread);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Open and Close port while the port is receiving data");

                workerThread.Start(cts.Token);

                try
                {
                    int iterationCount = TCSupport.RunShortStressTests ? 10 : 1000;
                    for (int i = 0; i < iterationCount; ++i)
                    {
                        com.RtsEnable = true;
                        com.Open();
                        com.Close();
                    }
                }
                finally
                {
                    cts.Cancel();
                }
            }

            workerThread.Join();
        }

        private void OpenReceiveData_WorkerThread(object token)
        {
            CancellationToken ct = (CancellationToken)token;
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                byte[] xmitBytes = new byte[16];

                for (int i = 0; i < xmitBytes.Length; ++i)
                    xmitBytes[i] = (byte)i;

                com.Open();

                while (!ct.IsCancellationRequested)
                {
                    com.Write(xmitBytes, 0, xmitBytes.Length);
                }
            }
        }

        [ConditionalFact(nameof(HasNullModem), nameof(HasHardwareFlowControl))]
        public void OpenReceiveDataAndRTS()
        {
            Thread workerThread = new Thread(OpenReceiveDataAndRTS_WorkerThread);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Open and Close port while the port is recieving data and the RTS pin is changing states");

                workerThread.Start(cts.Token);

                byte[] xmitBytes = new byte[16];

                for (int i = 0; i < xmitBytes.Length; ++i)
                    xmitBytes[i] = (byte)i;

                try
                {
                    int iterationCount = TCSupport.RunShortStressTests ? 10 : 1000;
                    for (int i = 0; i < iterationCount; ++i)
                    {
                        com.Open();
                        com.Handshake = Handshake.RequestToSend;
                        com.Write(xmitBytes, 0, xmitBytes.Length);
                        com.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Thread1 threw the following exception:\n{0}", e);
                }
                finally
                {
                    cts.Cancel();
                }
            }

            workerThread.Join();
        }


        private void OpenReceiveDataAndRTS_WorkerThread(object token)
        {
            CancellationToken ct = (CancellationToken)token;
            try
            {
                using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
                {
                    byte[] xmitBytes = new byte[16];

                    for (int i = 0; i < xmitBytes.Length; ++i)
                        xmitBytes[i] = (byte)i;

                    com.Open();

                    while (!ct.IsCancellationRequested)
                    {
                        com.Write(xmitBytes, 0, xmitBytes.Length);
                        com.RtsEnable = !com.RtsEnable;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Thread1 threw the following exception:\n{0}", e);
            }
        }
    }
}
