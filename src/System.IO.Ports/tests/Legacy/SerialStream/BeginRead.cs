// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.PortsTests;
using System.Threading;
using Legacy.Support;
using Xunit;

namespace Legacy.SerialStream
{
    public class BeginRead : PortsTest
    {
        //The number of random bytes to receive for read method testing
        public static readonly int numRndBytesToRead = 16;

        //The number of random bytes to receive for large input buffer testing
        public static readonly int largeNumRndBytesToRead = 2048;

        //When we test Read and do not care about actually reading anything we must still
        //create an byte array to pass into the method the following is the size of the 
        //byte array used in this situation
        public static readonly int defaultByteArraySize = 1;
        public static readonly int defaultByteOffset = 0;
        public static readonly int defaultByteCount = 1;

        //The maximum buffer size when a exception occurs
        public static readonly int maxBufferSizeForException = 255;

        //The maximum buffer size when a exception is not expected
        public static readonly int maxBufferSize = 8;

        //Maximum time to wait for processing the read command to complete
        private const int MAX_WAIT_READ_COMPLETE = 1000;

        #region Test Cases

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Buffer_Null()
        {
            VerifyReadException(null, 0, 1, typeof(ArgumentNullException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_NEG1()
        {
            VerifyReadException(new byte[defaultByteArraySize], -1, defaultByteCount, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_NEGRND()
        {
            var rndGen = new Random(-55);
            VerifyReadException(new byte[defaultByteArraySize], rndGen.Next(int.MinValue, 0), defaultByteCount, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_MinInt()
        {
            VerifyReadException(new byte[defaultByteArraySize], int.MinValue, defaultByteCount, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_NEG1()
        {
            VerifyReadException(new byte[defaultByteArraySize], defaultByteOffset, -1, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_NEGRND()
        {
            var rndGen = new Random(-55);
            VerifyReadException(new byte[defaultByteArraySize], defaultByteOffset, rndGen.Next(int.MinValue, 0), typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_MinInt()
        {
            VerifyReadException(new byte[defaultByteArraySize], defaultByteOffset, int.MinValue, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OffsetCount_EQ_Length_Plus_1()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSizeForException);
            int offset = rndGen.Next(0, bufferLength);
            int count = bufferLength + 1 - offset;
            Type expectedException = typeof(ArgumentException);

            VerifyReadException(new byte[bufferLength], offset, count, expectedException);
        }


        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OffsetCount_GT_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSizeForException);
            int offset = rndGen.Next(0, bufferLength);
            int count = rndGen.Next(bufferLength + 1 - offset, int.MaxValue);
            Type expectedException = typeof(ArgumentException);

            VerifyReadException(new byte[bufferLength], offset, count, expectedException);
        }


        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_GT_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSizeForException);
            int offset = rndGen.Next(bufferLength, int.MaxValue);
            int count = defaultByteCount;
            Type expectedException = typeof(ArgumentException);

            VerifyReadException(new byte[bufferLength], offset, count, expectedException);
        }


        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_GT_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSizeForException);
            int offset = defaultByteOffset;
            int count = rndGen.Next(bufferLength + 1, int.MaxValue);
            Type expectedException = typeof(ArgumentException);

            VerifyReadException(new byte[bufferLength], offset, count, expectedException);
        }
        
        [ConditionalFact(nameof(HasNullModem))]
        public void OffsetCount_EQ_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSize);
            int offset = rndGen.Next(0, bufferLength - 1);
            int count = bufferLength - offset;
            
            VerifyRead(new byte[bufferLength], offset, count);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Offset_EQ_Length_Minus_1()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSize);
            int offset = bufferLength - 1;
            var count = 1;

            VerifyRead(new byte[bufferLength], offset, count);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Count_EQ_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, maxBufferSize);
            var offset = 0;
            int count = bufferLength;

            VerifyRead(new byte[bufferLength], offset, count);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void LargeInputBuffer()
        {
            int bufferLength = largeNumRndBytesToRead;
            var offset = 0;
            int count = bufferLength;

            VerifyRead(new byte[bufferLength], offset, count, largeNumRndBytesToRead);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Callback()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var callbackHandler = new CallbackHandler();

                int elapsedTime;

                Debug.WriteLine("Verifying BeginRead with a callback specified");

                com1.Open();
                com2.Open();

                IAsyncResult readAsyncResult = com1.BaseStream.BeginRead(new byte[numRndBytesToRead], 0, numRndBytesToRead,
                    callbackHandler.Callback, null);
                callbackHandler.BeginReadAysncResult = readAsyncResult;
                VerifyAsyncResult(readAsyncResult, null, false, false,
                    "of IAsyncResult returned from BeginRead BEFORE Write");

                com2.Write(new byte[numRndBytesToRead], 0, numRndBytesToRead);

                //callbackHandler.ReadAsyncResult  guarantees that the callback has been calledhowever it does not gauarentee that 
                //the code calling the the callback has finished it's processing
                IAsyncResult callbackReadAsyncResult = callbackHandler.ReadAysncResult;

                //No we have to wait for the callbackHandler to complete
                elapsedTime = 0;
                while (!callbackReadAsyncResult.IsCompleted && elapsedTime < MAX_WAIT_READ_COMPLETE)
                {
                    Thread.Sleep(10);
                    elapsedTime += 10;
                }

                VerifyAsyncResult(callbackReadAsyncResult, null, false, true,
                    " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(readAsyncResult, null, false, true,
                    "of IAsyncResult returned from BeginRead AFTER Write");
            }
        }
        
        [ConditionalFact(nameof(HasNullModem))]
        public void Callback_EndReadonCallback()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var callbackHandler = new CallbackHandler(com1);

                int elapsedTime;

                Debug.WriteLine("Verifying BeginRead with a callback that calls EndRead");

                com1.Open();
                com2.Open();

                IAsyncResult readAsyncResult = com1.BaseStream.BeginRead(new byte[numRndBytesToRead], 0, numRndBytesToRead,
                    callbackHandler.Callback, null);
                callbackHandler.BeginReadAysncResult = readAsyncResult;
                VerifyAsyncResult(readAsyncResult, null, false, false,
                    "of IAsyncResult returned from BeginRead BEFORE Write");

                com2.Write(new byte[numRndBytesToRead], 0, numRndBytesToRead);

                //callbackHandler.ReadAsyncResult  guarantees that the callback has been calledhowever it does not gauarentee that 
                //the code calling the the callback has finished it's processing
                IAsyncResult callbackReadAsyncResult = callbackHandler.ReadAysncResult;

                //No we have to wait for the callbackHandler to complete
                elapsedTime = 0;
                while (!callbackReadAsyncResult.IsCompleted && elapsedTime < MAX_WAIT_READ_COMPLETE)
                {
                    Thread.Sleep(10);
                    elapsedTime += 10;
                }

                VerifyAsyncResult(callbackReadAsyncResult, null, false, true, " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(readAsyncResult, null, false, true, "of IAsyncResult returned from BeginRead AFTER Write");
            }
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void Callback_State()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var callbackHandler = new CallbackHandler();

                int elapsedTime;

                Debug.WriteLine("Verifying BeginRead with a callback and state specified");

                com1.Open();
                com2.Open();

                IAsyncResult readAsyncResult = com1.BaseStream.BeginRead(new byte[numRndBytesToRead], 0, numRndBytesToRead,
                    callbackHandler.Callback, this);
                callbackHandler.BeginReadAysncResult = readAsyncResult;
                VerifyAsyncResult(readAsyncResult, this, false, false,
                    "of IAsyncResult returned from BeginRead BEFORE Write");

                com2.Write(new byte[numRndBytesToRead], 0, numRndBytesToRead);

                //callbackHandler.ReadAsyncResult  guarantees that the callback has been calledhowever it does not gauarentee that 
                //the code calling the the callback has finished it's processing
                IAsyncResult callbackReadAsyncResult = callbackHandler.ReadAysncResult;

                //No we have to wait for the callbackHandler to complete
                elapsedTime = 0;
                while (!callbackReadAsyncResult.IsCompleted && elapsedTime < MAX_WAIT_READ_COMPLETE)
                {
                    Thread.Sleep(10);
                    elapsedTime += 10;
                }

                VerifyAsyncResult(callbackReadAsyncResult, this, false, true,
                    " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(readAsyncResult, this, false, true,
                    "of IAsyncResult returned from BeginRead AFTER Write");

                

            }
        }
        #endregion

        #region Verification for Test Cases

        private void VerifyReadException(byte[] buffer, int offset, int count, Type expectedException)
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                int bufferLength = null == buffer ? 0 : buffer.Length;

                Debug.WriteLine("Verifying read method throws {0} buffer.Lenght={1}, offset={2}, count={3}",
                    expectedException, bufferLength, offset, count);

                com.Open();

                Assert.Throws(expectedException, () => com.BaseStream.BeginRead(buffer, offset, count, null, null));
            }
        }

        private void VerifyRead(byte[] buffer, int offset, int count)
        {
            VerifyRead(buffer, offset, count, numRndBytesToRead);
        }

        private void VerifyRead(byte[] buffer, int offset, int count, int numberOfBytesToRead)
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var rndGen = new Random(-55);
                var bytesToWrite = new byte[numberOfBytesToRead];

                //Genrate random bytes
                for (var i = 0; i < bytesToWrite.Length; i++)
                {
                    var randByte = (byte)rndGen.Next(0, 256);

                    bytesToWrite[i] = randByte;
                }

                //Genrate some random bytes in the buffer
                for (var i = 0; i < buffer.Length; i++)
                {
                    var randByte = (byte)rndGen.Next(0, 256);

                    buffer[i] = randByte;
                }

                Debug.WriteLine(
                    "Verifying read method buffer.Lenght={0}, offset={1}, count={2} with {3} random chars",
                    buffer.Length, offset, count, bytesToWrite.Length);

                com1.ReadTimeout = 500;
                com1.Open();
                com2.Open();

                VerifyBytesReadOnCom1FromCom2(com1, com2, bytesToWrite, buffer, offset, count);
            }
        }

        private void VerifyBytesReadOnCom1FromCom2(SerialPort com1, SerialPort com2, byte[] bytesToWrite, byte[] rcvBuffer, int offset, int count)
        {
            var buffer = new byte[bytesToWrite.Length];
            int totalBytesRead;
            int bytesToRead;
            var oldRcvBuffer = (byte[])rcvBuffer.Clone();
            var callbackHandler = new CallbackHandler();

            com2.Write(bytesToWrite, 0, bytesToWrite.Length);
            com1.ReadTimeout = 500;
            Thread.Sleep((int)(((bytesToWrite.Length * 10.0) / com1.BaudRate) * 1000) + 250);
            totalBytesRead = 0;
            bytesToRead = com1.BytesToRead;

            do
            {
                IAsyncResult readAsyncResult = com1.BaseStream.BeginRead(rcvBuffer, offset, count,
                    callbackHandler.Callback, this);
                readAsyncResult.AsyncWaitHandle.WaitOne();
                callbackHandler.BeginReadAysncResult = readAsyncResult;

                int bytesRead = com1.BaseStream.EndRead(readAsyncResult);
                VerifyAsyncResult(callbackHandler.ReadAysncResult, this, false, true,
                    " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(readAsyncResult, this, false, true, " of IAsyncResult returned from BeginRead");

                if ((bytesToRead > bytesRead && count != bytesRead) ||
                    (bytesToRead <= bytesRead && bytesRead != bytesToRead))
                {
                    //If we have not read all of the characters that we should have
                    Fail("ERROR!!!: Read did not return all of the characters that were in SerialPort buffer");
                }

                if (bytesToWrite.Length < totalBytesRead + bytesRead)
                {
                    //If we have read in more characters then we expect
                    Fail("ERROR!!!: We have received more characters then were sent");
                }

                VerifyBuffer(rcvBuffer, oldRcvBuffer, offset, bytesRead);

                Array.Copy(rcvBuffer, offset, buffer, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;

                if (bytesToWrite.Length - totalBytesRead != com1.BytesToRead)
                {
                    Fail("ERROR!!!: Expected BytesToRead={0} actual={1}", bytesToWrite.Length - totalBytesRead, com1.BytesToRead);
                }

                oldRcvBuffer = (byte[])rcvBuffer.Clone();
                bytesToRead = com1.BytesToRead;
            } while (0 != com1.BytesToRead); //While there are more bytes to read

            //Compare the bytes that were written with the ones we read
            for (var i = 0; i < bytesToWrite.Length; i++)
            {
                if (bytesToWrite[i] != buffer[i])
                {
                    Fail("ERROR!!!: Expected to read {0}  actual read  {1}", bytesToWrite[i], buffer[i]);
                }
            }
        }
        
        private void VerifyBuffer(byte[] actualBuffer, byte[] expectedBuffer, int offset, int count)
        {
            //Verify all character before the offset
            for (var i = 0; i < offset; i++)
            {
                if (actualBuffer[i] != expectedBuffer[i])
                {
                    Fail("ERROR!!!: Expected {0} in buffer at {1} actual {2}", (int)expectedBuffer[i], i, (int)actualBuffer[i]);
                }
            }

            //Verify all character after the offset + count
            for (int i = offset + count; i < actualBuffer.Length; i++)
            {
                if (actualBuffer[i] != expectedBuffer[i])
                {
                    Fail("ERROR!!!: Expected {0} in buffer at {1} actual {2}", (int)expectedBuffer[i], i, (int)actualBuffer[i]);
                }
            }
        }

        private class CallbackHandler
        {
            private IAsyncResult _readAysncResult;
            private IAsyncResult _beginReadAysncResult;
            private readonly SerialPort _com;

            public CallbackHandler() : this(null) { }

            public CallbackHandler(SerialPort com)
            {
                _com = com;
            }

            public void Callback(IAsyncResult readAysncResult)
            {
                lock (this)
                {
                    _readAysncResult = readAysncResult;

                    if (!readAysncResult.IsCompleted)
                    {
                        throw new Exception("Err_23984afaea Expected IAsyncResult passed into callback to not be completed");
                    }

                    while (null == _beginReadAysncResult)
                    {
                        Monitor.Wait(this);
                    }

                    if (null != _beginReadAysncResult && !_beginReadAysncResult.IsCompleted)
                    {
                        throw new Exception("Err_7907azpu Expected IAsyncResult returned from begin read to not be completed");
                    }

                    if (null != _com)
                    {
                        _com.BaseStream.EndRead(_beginReadAysncResult);
                        if (!_beginReadAysncResult.IsCompleted)
                        {
                            throw new Exception("Err_6498afead Expected IAsyncResult returned from begin read to not be completed");
                        }

                        if (!readAysncResult.IsCompleted)
                        {
                            throw new Exception("Err_1398ehpo Expected IAsyncResult passed into callback to not be completed");
                        }
                    }

                    Monitor.Pulse(this);
                }
            }


            public IAsyncResult ReadAysncResult
            {
                get
                {
                    lock (this)
                    {
                        while (null == _readAysncResult)
                        {
                            Monitor.Wait(this);
                        }

                        return _readAysncResult;
                    }
                }
            }

            public IAsyncResult BeginReadAysncResult
            {
                get
                {
                    return _beginReadAysncResult;
                }
                set
                {
                    lock (this)
                    {
                        _beginReadAysncResult = value;
                        Monitor.Pulse(this);
                    }
                }
            }
        }

        private void VerifyAsyncResult(IAsyncResult asyncResult, object expectedAsyncState, bool expectedCompletedSynchronously, bool expectedIsCompleted, string suffix)
        {
            Assert.Equal(expectedAsyncState, asyncResult.AsyncState);
            Assert.Equal(expectedCompletedSynchronously, asyncResult.CompletedSynchronously);
            Assert.Equal(expectedIsCompleted, asyncResult.IsCompleted);
        }
        #endregion
    }
}
