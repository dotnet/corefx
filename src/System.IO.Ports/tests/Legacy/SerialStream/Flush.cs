// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.IO.PortsTests;
using System.Threading;
using Legacy.Support;
using Xunit;

namespace Legacy.SerialStream
{
    public class Flush : PortsTest
    {
        //The string used with Write(str) to fill the input buffer
        public static readonly string DEFAULT_STRING = "Hello World";
        public static readonly int DEFAULT_BUFFER_SIZE = 32;
        public static readonly int MAX_WAIT_TIME = 500;

        //The buffer lenght used whe filling the ouput buffer
        public static readonly int DEFAULT_BUFFER_LENGTH = 8;


        #region Test Cases

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Flush_Open_Close()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                com.Open();
                Stream serialStream = com.BaseStream;
                com.Close();

                Debug.WriteLine("Verifying Flush throws exception After Open() then Close()");

                VerifyException(serialStream, typeof(ObjectDisposedException));
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Flush_Open_BaseStreamClose()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                com.Open();
                Stream serialStream = com.BaseStream;
                com.BaseStream.Close();

                Debug.WriteLine("Verifying Flush throws exception After Open() then BaseStream.Close()");

                VerifyException(serialStream, typeof(ObjectDisposedException));
            }
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void InBufferFilled_Flush_Once()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var elapsedTime = 0;
                var xmitBytes = new byte[DEFAULT_BUFFER_SIZE];

                Debug.WriteLine("Verifying Flush method after input buffer has been filled");
                com1.Open();
                com2.Open();

                for (var i = 0; i < xmitBytes.Length; i++) xmitBytes[i] = (byte)i;

                com2.Write(xmitBytes, 0, xmitBytes.Length);

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);
            }
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void InBufferFilled_Flush_Multiple()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var elapsedTime = 0;
                var xmitBytes = new byte[DEFAULT_BUFFER_SIZE];

                Debug.WriteLine("Verifying call Flush method several times after input buffer has been filled");
                com1.Open();
                com2.Open();

                for (var i = 0; i < xmitBytes.Length; i++) xmitBytes[i] = (byte)i;

                com2.Write(xmitBytes, 0, xmitBytes.Length);

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);
                VerifyFlush(com1);
                VerifyFlush(com1);
            }
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void InBufferFilled_Flush_Cycle()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {

                var elapsedTime = 0;
                var xmitBytes = new byte[DEFAULT_BUFFER_SIZE];

                Debug.WriteLine(
                    "Verifying call Flush method after input buffer has been filled discarded and filled again");

                com1.Open();
                com2.Open();

                for (var i = 0; i < xmitBytes.Length; i++) xmitBytes[i] = (byte)i;

                com2.Write(xmitBytes, 0, xmitBytes.Length);

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);

                com2.Write(xmitBytes, 0, xmitBytes.Length);
                elapsedTime = 0;

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE * 2 && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OutBufferFilled_Flush_Once()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                var asyncWriteRndByteArray = new AsyncWriteRndByteArray(com1, DEFAULT_BUFFER_SIZE);
                var t = new Thread(asyncWriteRndByteArray.WriteRndByteArray);

                var elapsedTime = 0;

                Debug.WriteLine("Verifying Flush method after output buffer has been filled");

                com1.Open();
                com1.WriteTimeout = 500;
                com1.Handshake = Handshake.RequestToSend;

                t.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t.IsAlive)
                    Thread.Sleep(100);
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OutBufferFilled_Flush_Multiple()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                var asyncWriteRndByteArray = new AsyncWriteRndByteArray(com1, DEFAULT_BUFFER_SIZE);
                var t = new Thread(asyncWriteRndByteArray.WriteRndByteArray);

                int elapsedTime;

                Debug.WriteLine("Verifying call Flush method several times after output buffer has been filled");

                com1.Open();
                com1.WriteTimeout = 500;
                com1.Handshake = Handshake.RequestToSend;

                t.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);
                VerifyFlush(com1);
                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t.IsAlive)
                    Thread.Sleep(100);
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OutBufferFilled_Flush_Cycle()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                var asyncWriteRndByteArray = new AsyncWriteRndByteArray(com1, DEFAULT_BUFFER_SIZE);
                var t1 = new Thread(asyncWriteRndByteArray.WriteRndByteArray);
                var t2 = new Thread(asyncWriteRndByteArray.WriteRndByteArray);

                var elapsedTime = 0;

                Debug.WriteLine(
                    "Verifying call Flush method after output buffer has been filled discarded and filled again");

                com1.Open();
                com1.WriteTimeout = 500;
                com1.Handshake = Handshake.RequestToSend;

                t1.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t1.IsAlive)
                    Thread.Sleep(100);

                t2.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);
                
                //Wait for write method to timeout
                while (t2.IsAlive)
                    Thread.Sleep(100);
            }
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void InOutBufferFilled_Flush_Once()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var asyncWriteRndByteArray = new AsyncWriteRndByteArray(com1, DEFAULT_BUFFER_SIZE);
                var t = new Thread(asyncWriteRndByteArray.WriteRndByteArray);

                var elapsedTime = 0;
                var xmitBytes = new byte[DEFAULT_BUFFER_SIZE];

                Debug.WriteLine("Verifying Flush method after input and output buffer has been filled");

                com1.Open();
                com2.Open();
                com1.WriteTimeout = 500;
                com1.Handshake = Handshake.RequestToSend;

                for (var i = 0; i < xmitBytes.Length; i++) xmitBytes[i] = (byte)i;

                com2.Write(xmitBytes, 0, xmitBytes.Length);

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                t.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t.IsAlive)
                    Thread.Sleep(100);
            }


        }

        [ConditionalFact(nameof(HasNullModem))]

        public void InOutBufferFilled_Flush_Multiple()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var asyncWriteRndByteArray = new AsyncWriteRndByteArray(com1, DEFAULT_BUFFER_SIZE);
                var t = new Thread(asyncWriteRndByteArray.WriteRndByteArray);

                var elapsedTime = 0;
                var xmitBytes = new byte[DEFAULT_BUFFER_SIZE];

                Debug.WriteLine(
                    "Verifying call Flush method several times after input and output buffer has been filled");

                com1.Open();
                com2.Open();
                com1.WriteTimeout = 500;
                com1.Handshake = Handshake.RequestToSend;

                for (var i = 0; i < xmitBytes.Length; i++) xmitBytes[i] = (byte)i;

                com2.Write(xmitBytes, 0, xmitBytes.Length);

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                t.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);
                VerifyFlush(com1);
                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t.IsAlive)
                    Thread.Sleep(100);


            }


        }


        [ConditionalFact(nameof(HasNullModem))]
        public void InOutBufferFilled_Flush_Cycle()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var asyncWriteRndByteArray = new AsyncWriteRndByteArray(com1, DEFAULT_BUFFER_SIZE);
                var t1 = new Thread(asyncWriteRndByteArray.WriteRndByteArray);
                var t2 = new Thread(asyncWriteRndByteArray.WriteRndByteArray);

                var elapsedTime = 0;
                var xmitBytes = new byte[DEFAULT_BUFFER_SIZE];

                Debug.WriteLine(
                    "Verifying call Flush method after input and output buffer has been filled discarded and filled again");

                com1.Open();
                com2.Open();
                com1.WriteTimeout = 500;
                com1.Handshake = Handshake.RequestToSend;

                for (var i = 0; i < xmitBytes.Length; i++) xmitBytes[i] = (byte)i;

                com2.Write(xmitBytes, 0, xmitBytes.Length);

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                t1.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t1.IsAlive)
                    Thread.Sleep(100);

                t2.Start();
                elapsedTime = 0;

                while (com1.BytesToWrite < DEFAULT_BUFFER_LENGTH && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                com2.Write(xmitBytes, 0, xmitBytes.Length);
                elapsedTime = 0;

                while (com1.BytesToRead < DEFAULT_BUFFER_SIZE && elapsedTime < MAX_WAIT_TIME)
                {
                    Thread.Sleep(50);
                    elapsedTime += 50;
                }

                VerifyFlush(com1);

                //Wait for write method to timeout
                while (t2.IsAlive)
                    Thread.Sleep(100);

            }


        }



        public class AsyncWriteRndByteArray
        {
            private readonly SerialPort _com;
            private readonly int _byteLength;


            public AsyncWriteRndByteArray(SerialPort com, int byteLength)
            {
                _com = com;
                _byteLength = byteLength;
            }


            public void WriteRndByteArray()
            {
                var buffer = new byte[_byteLength];
                var rndGen = new Random(-55);

                for (var i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)rndGen.Next(0, 256);
                }

                try
                {
                    _com.Write(buffer, 0, buffer.Length);
                }
                catch (TimeoutException)
                {
                }
            }
        }
        #endregion

        #region Verification for Test Cases
        private void VerifyException(Stream serialStream, Type expectedException)
        {


            try
            {
                serialStream.Flush();

                Fail("ERROR!!!: No Excpetion was thrown from Flush()");
            }
            catch (Exception e)
            {
                if (e.GetType() != expectedException)
                {
                    Fail("ERROR!!!: {0} exception was thrown expected {1} from Flush()", e.GetType(), expectedException);
                }
            }
        }

        private void VerifyFlush(SerialPort com)
        {

            int origBytesToRead = com.BytesToRead;
            var i = 0;

            com.BaseStream.Flush();

            if (origBytesToRead != com.BytesToRead)
            {
                Fail("ERROR!!! Expected BytesToRead={0} Actual BytesToRead={1}", origBytesToRead, com.BytesToRead);
            }

            if (0 != com.BytesToWrite)
            {
                Fail("ERROR!!! Expected BytesToWrite=0 Actual BytesToWrite={0}", com.BytesToWrite);
            }

            com.ReadTimeout = 0;

            if (origBytesToRead != 0)
            {
                while (true)
                {
                    int byteRead;
                    try
                    {
                        byteRead = com.ReadByte();
                    }
                    catch (TimeoutException)
                    {
                        break;
                    }

                    if (i != byteRead)
                    {
                        Fail("Err_7083apnh Expecte to read={0} actual={1}", i, byteRead);
                    }

                    i++;
                }

                if (i != DEFAULT_BUFFER_SIZE)
                {
                    Fail("Err_09778asdh Expected to read {0} bytes actually read {1}", DEFAULT_BUFFER_SIZE, i);
                }
            }


        }
        #endregion
    }
}
