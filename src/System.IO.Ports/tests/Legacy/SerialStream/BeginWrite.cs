// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.PortsTests;
using System.Text;
using System.Threading;
using Legacy.Support;
using Xunit;

namespace Legacy.SerialStream
{
    public class BeginWrite : PortsTest
    {
        //The string size used for large byte array testing
        private static readonly int LARGE_BUFFER_SIZE = 2048;

        //When we test Write and do not care about actually writing anything we must still
        //create an byte array to pass into the method the following is the size of the 
        //byte array used in this situation
        private static readonly int DEFAULT_BUFFER_SIZE = 1;
        private static readonly int DEFAULT_BUFFER_OFFSET = 0;
        private static readonly int DEFAULT_BUFFER_COUNT = 1;

        //The maximum buffer size when a exception occurs
        private static readonly int MAX_BUFFER_SIZE_FOR_EXCEPTION = 255;

        //The maximum buffer size when a exception is not expected
        private static readonly int MAX_BUFFER_SIZE = 8;

        //The default number of times the write method is called when verifying write
        private static readonly int DEFAULT_NUM_WRITES = 3;

        //The default number of bytes to write
        private static readonly int DEFAULT_NUM_BYTES_TO_WRITE = 16;

        //Maximum time to wait for processing the read command to complete
        private const int MAX_WAIT_WRITE_COMPLETE = 1000;

        #region Test Cases
        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Buffer_Null()
        {
            VerifyWriteException(null, 0, 1, typeof(ArgumentNullException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_NEG1()
        {
            VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], -1, DEFAULT_BUFFER_COUNT, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_NEGRND()
        {
            var rndGen = new Random(-55);

            VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], rndGen.Next(int.MinValue, 0), DEFAULT_BUFFER_COUNT, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_MinInt()
        {
            VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], int.MinValue, DEFAULT_BUFFER_COUNT, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_NEG1()
        {
            VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], DEFAULT_BUFFER_OFFSET, -1, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_NEGRND()
        {
            var rndGen = new Random(-55);

            VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], DEFAULT_BUFFER_OFFSET, rndGen.Next(int.MinValue, 0), typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_MinInt()
        {
            VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], DEFAULT_BUFFER_OFFSET, int.MinValue, typeof(ArgumentOutOfRangeException));
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OffsetCount_EQ_Length_Plus_1()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
            int offset = rndGen.Next(0, bufferLength);
            int count = bufferLength + 1 - offset;
            Type expectedException = typeof(ArgumentException);

            VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void OffsetCount_GT_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
            int offset = rndGen.Next(0, bufferLength);
            int count = rndGen.Next(bufferLength + 1 - offset, int.MaxValue);
            Type expectedException = typeof(ArgumentException);

            VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Offset_GT_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
            int offset = rndGen.Next(bufferLength, int.MaxValue);
            int count = DEFAULT_BUFFER_COUNT;
            Type expectedException = typeof(ArgumentException);

            VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Count_GT_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
            int offset = DEFAULT_BUFFER_OFFSET;
            int count = rndGen.Next(bufferLength + 1, int.MaxValue);
            Type expectedException = typeof(ArgumentException);

            VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void OffsetCount_EQ_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            int offset = rndGen.Next(0, bufferLength - 1);
            int count = bufferLength - offset;

            VerifyWrite(new byte[bufferLength], offset, count);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Offset_EQ_Length_Minus_1()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            int offset = bufferLength - 1;
            var count = 1;
            
            VerifyWrite(new byte[bufferLength], offset, count);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Count_EQ_Length()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            var offset = 0;
            int count = bufferLength;
            
            VerifyWrite(new byte[bufferLength], offset, count);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void ASCIIEncoding()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            int offset = rndGen.Next(0, bufferLength - 1);
            int count = rndGen.Next(1, bufferLength - offset);

            VerifyWrite(new byte[bufferLength], offset, count, new ASCIIEncoding());
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void UTF8Encoding()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            int offset = rndGen.Next(0, bufferLength - 1);
            int count = rndGen.Next(1, bufferLength - offset);

            VerifyWrite(new byte[bufferLength], offset, count, new UTF8Encoding());
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void UTF32Encoding()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            int offset = rndGen.Next(0, bufferLength - 1);
            int count = rndGen.Next(1, bufferLength - offset);

            VerifyWrite(new byte[bufferLength], offset, count, new UTF32Encoding());
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void UnicodeEncoding()
        {
            var rndGen = new Random(-55);
            int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
            int offset = rndGen.Next(0, bufferLength - 1);
            int count = rndGen.Next(1, bufferLength - offset);

            VerifyWrite(new byte[bufferLength], offset, count, new UnicodeEncoding());
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void LargeBuffer()
        {
            int bufferLength = LARGE_BUFFER_SIZE;
            var offset = 0;
            int count = bufferLength;

            VerifyWrite(new byte[bufferLength], offset, count, 1);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Callback()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var callbackHandler = new CallbackHandler();

                int elapsedTime;

                Debug.WriteLine("Verifying BeginWrite with a callback specified");

                com1.Handshake = Handshake.RequestToSend;
                com1.Open();
                com2.Open();

                IAsyncResult writeAsyncResult = com1.BaseStream.BeginWrite(new byte[DEFAULT_NUM_BYTES_TO_WRITE], 0,
                    DEFAULT_NUM_BYTES_TO_WRITE, callbackHandler.Callback, null);
                callbackHandler.BeginWriteAysncResult = writeAsyncResult;

                VerifyAsyncResult(writeAsyncResult, null, false, false,
                    "of IAsyncResult returned from BeginWrite BEFORE Write");

                com2.RtsEnable = true;

                //callbackHandler.WriteAysncResult guarantees that the callback has been called however it does not gauarentee that 
                //the code calling the the callback has finished it's processing
                IAsyncResult callbackWriteAsyncResult = callbackHandler.WriteAysncResult;

                //No we have to wait for the callbackHandler to complete
                elapsedTime = 0;
                while (!callbackWriteAsyncResult.IsCompleted && elapsedTime < MAX_WAIT_WRITE_COMPLETE)
                {
                    Thread.Sleep(10);
                    elapsedTime += 10;
                }

                VerifyAsyncResult(callbackWriteAsyncResult, null, false, true,
                    " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(writeAsyncResult, null, false, true,
                    "of IAsyncResult returned from BeginWrite AFTER Write");
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

                Debug.WriteLine("Verifying BeginWrite with a callback and state specified");
                com1.Open();
                com2.Open();

                IAsyncResult writeAsyncResult = com1.BaseStream.BeginWrite(new byte[DEFAULT_NUM_BYTES_TO_WRITE], 0,
                    DEFAULT_NUM_BYTES_TO_WRITE, callbackHandler.Callback, this);
                callbackHandler.BeginWriteAysncResult = writeAsyncResult;
                VerifyAsyncResult(writeAsyncResult, this, false, false,
                    "of IAsyncResult returned from BeginWrite BEFORE Write");

                com2.RtsEnable = true;

                //callbackHandler.WriteAysncResult guarantees that the callback has been called however it does not gauarentee that 
                //the code calling the the callback has finished it's processing
                IAsyncResult callbackWriteAsyncResult = callbackHandler.WriteAysncResult;

                //No we have to wait for the callbackHandler to complete
                elapsedTime = 0;
                while (!callbackWriteAsyncResult.IsCompleted && elapsedTime < MAX_WAIT_WRITE_COMPLETE)
                {
                    Thread.Sleep(10);
                    elapsedTime += 10;
                }

                VerifyAsyncResult(callbackWriteAsyncResult, this, false, true,
                    " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(writeAsyncResult, this, false, true,
                    "of IAsyncResult returned from BeginWrite AFTER Write");
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void InBreak()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Verifying BeginWrite throws InvalidOperationException while in a Break");
                com1.Open();
                com1.BreakState = true;

                Assert.Throws<InvalidOperationException>(() => com1.BaseStream.BeginWrite(new byte[8], 0, 8, null, null));
            }
        }
        #endregion

        #region Verification for Test Cases

        private void VerifyWriteException(byte[] buffer, int offset, int count, Type expectedException)
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                int bufferLength = null == buffer ? 0 : buffer.Length;

                Debug.WriteLine("Verifying write method throws {0} buffer.Lenght={1}, offset={2}, count={3}",
                    expectedException, bufferLength, offset, count);
                com.Open();

                Assert.Throws(expectedException, () => com.BaseStream.BeginWrite(buffer, offset, count, null, null));
            }
        }

        private void VerifyWrite(byte[] buffer, int offset, int count)
        {
            VerifyWrite(buffer, offset, count, new ASCIIEncoding());
        }

        private void VerifyWrite(byte[] buffer, int offset, int count, int numWrites)
        {
            VerifyWrite(buffer, offset, count, new ASCIIEncoding(), numWrites);
        }

        private void VerifyWrite(byte[] buffer, int offset, int count, Encoding encoding)
        {
            VerifyWrite(buffer, offset, count, encoding, DEFAULT_NUM_WRITES);
        }

        private void VerifyWrite(byte[] buffer, int offset, int count, Encoding encoding, int numWrites)
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var rndGen = new Random(-55);

                Debug.WriteLine("Verifying write method buffer.Lenght={0}, offset={1}, count={2}, endocing={3}",
                    buffer.Length, offset, count, encoding.EncodingName);

                com1.Encoding = encoding;
                com2.Encoding = encoding;

                com1.Open();
                com2.Open();

                for (var i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)rndGen.Next(0, 256);
                }

                VerifyWriteByteArray(buffer, offset, count, com1, com2, numWrites);
            }
        }
        
        public void VerifyWriteByteArray(byte[] buffer, int offset, int count, SerialPort com1, SerialPort com2)
        {
            VerifyWriteByteArray(buffer, offset, count, com1, com2, DEFAULT_NUM_WRITES);
        }

        private void VerifyWriteByteArray(byte[] buffer, int offset, int count, SerialPort com1, SerialPort com2, int numWrites)
        {

            byte[] oldBuffer, expectedBytes, actualBytes;
            int byteRead;
            var index = 0;
            IAsyncResult writeAsyncResult;
            var callbackHandler = new CallbackHandler();

            oldBuffer = (byte[])buffer.Clone();
            expectedBytes = new byte[count];
            actualBytes = new byte[expectedBytes.Length * numWrites];

            for (var i = 0; i < count; i++)
            {
                expectedBytes[i] = buffer[i + offset];
            }

            for (var i = 0; i < numWrites; i++)
            {
                writeAsyncResult = com1.BaseStream.BeginWrite(buffer, offset, count, callbackHandler.Callback, this);
                writeAsyncResult.AsyncWaitHandle.WaitOne();
                callbackHandler.BeginWriteAysncResult = writeAsyncResult;

                com1.BaseStream.EndWrite(writeAsyncResult);

                VerifyAsyncResult(callbackHandler.WriteAysncResult, this, false, true, " of IAsyncResult passed into AsyncCallback");
                VerifyAsyncResult(writeAsyncResult, this, false, true, " of IAsyncResult returned from BeginWrite");
            }

            com2.ReadTimeout = 500;
            Thread.Sleep((int)(((expectedBytes.Length * numWrites * 10.0) / com1.BaudRate) * 1000) + 250);

            //Make sure buffer was not altered during the write call
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != oldBuffer[i])
                {
                    Fail("ERROR!!!: The contents of the buffer were changed from {0} to {1} at {2}", oldBuffer[i], buffer[i], i);
                }
            }

            while (true)
            {
                try
                {
                    byteRead = com2.ReadByte();
                }
                catch (TimeoutException)
                {
                    break;
                }

                if (actualBytes.Length <= index)
                {
                    //If we have read in more bytes then we expect
                    Fail("ERROR!!!: We have received more bytes then were sent");
                    break;
                }

                actualBytes[index] = (byte)byteRead;
                index++;
                if (actualBytes.Length - index != com2.BytesToRead)
                {
                    Fail("ERROR!!!: Expected BytesToRead={0} actual={1}", actualBytes.Length - index, com2.BytesToRead);
                }
            }

            //Compare the bytes that were read with the ones we expected to read
            for (var j = 0; j < numWrites; j++)
            {
                for (var i = 0; i < expectedBytes.Length; i++)
                {
                    if (expectedBytes[i] != actualBytes[i + expectedBytes.Length * j])
                    {
                        Fail("ERROR!!!: Expected to read byte {0}  actual read {1} at {2}", (int)expectedBytes[i], (int)actualBytes[i + expectedBytes.Length * j], i);
                    }
                }
            }

        }

        private class CallbackHandler
        {
            private IAsyncResult _writeAysncResult;
            private IAsyncResult _beginWriteAysncResult;
            private readonly SerialPort _com;

            public CallbackHandler() : this(null) { }

            public CallbackHandler(SerialPort com)
            {
                _com = com;
            }

            public void Callback(IAsyncResult writeAysncResult)
            {
                lock (this)
                {
                    _writeAysncResult = writeAysncResult;

                    if (!writeAysncResult.IsCompleted)
                    {
                        throw new Exception("Err_23984afaea Expected IAsyncResult passed into callback to not be completed");
                    }

                    while (null == _beginWriteAysncResult)
                    {
                        Monitor.Wait(this);
                    }

                    if (null != _beginWriteAysncResult && !_beginWriteAysncResult.IsCompleted)
                    {
                        throw new Exception("Err_7907azpu Expected IAsyncResult returned from begin write to not be completed");
                    }

                    if (null != _com)
                    {
                        _com.BaseStream.EndWrite(_beginWriteAysncResult);
                        if (!_beginWriteAysncResult.IsCompleted)
                        {
                            throw new Exception("Err_6498afead Expected IAsyncResult returned from begin write to not be completed");
                        }

                        if (!writeAysncResult.IsCompleted)
                        {
                            throw new Exception("Err_1398ehpo Expected IAsyncResult passed into callback to not be completed");
                        }
                    }

                    Monitor.Pulse(this);
                }
            }


            public IAsyncResult WriteAysncResult
            {
                get
                {
                    lock (this)
                    {
                        while (null == _writeAysncResult)
                        {
                            Monitor.Wait(this);
                        }

                        return _writeAysncResult;
                    }
                }
            }

            public IAsyncResult BeginWriteAysncResult
            {
                get
                {
                    return _beginWriteAysncResult;
                }
                set
                {
                    lock (this)
                    {
                        _beginWriteAysncResult = value;
                        Monitor.Pulse(this);
                    }
                }
            }
        }


        private void VerifyAsyncResult(IAsyncResult asyncResult, object expectedAsyncState, bool expectedCompletedSynchronously, bool expectedIsCompleted, string suffix)
        {
            if (expectedAsyncState != asyncResult.AsyncState)
            {
                Fail("ERROR!!!: Expected writeAysncResult.AsyncState={0} acutual={1} " + suffix, expectedAsyncState, asyncResult.AsyncState);
            }

            if (expectedCompletedSynchronously != asyncResult.CompletedSynchronously)
            {
                Fail("ERROR!!!: Expected writeAysncResult.CompletedSynchronously={0} acutual={1} " + suffix, expectedCompletedSynchronously, asyncResult.CompletedSynchronously);
            }

            if (expectedIsCompleted != asyncResult.IsCompleted)
            {
                Fail("ERROR!!!: Expected writeAysncResult.IsCompleted={0} acutual={1} " + suffix, expectedIsCompleted, asyncResult.IsCompleted);
            }
        }
        #endregion
    }
}
