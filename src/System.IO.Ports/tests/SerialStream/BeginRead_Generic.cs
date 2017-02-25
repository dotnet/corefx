// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.PortsTests;
using System.Text;
using System.Threading;
using Legacy.Support;
using Xunit;
using ThreadState = System.Threading.ThreadState;

namespace System.IO.Ports.Tests
{
    public class SerialStream_BeginRead_Generic : PortsTest
    {
        // Set bounds fore random timeout values.
        // If the min is to low read will not timeout accurately and the testcase will fail
        private const int minRandomTimeout = 250;

        // If the max is to large then the testcase will take forever to run
        private const int maxRandomTimeout = 2000;

        // If the percentage difference between the expected timeout and the actual timeout
        // found through Stopwatch is greater then 10% then the timeout value was not correctly
        // to the read method and the testcase fails.
        public const double maxPercentageDifference = .15;

        // The number of random bytes to receive for parity testing
        private const int numRndBytesPairty = 8;

        // The number of characters to read at a time for parity testing
        private const int numBytesReadPairty = 2;

        // The number of random bytes to receive for BytesToRead testing
        private const int numRndBytesToRead = 16;

        // When we test Read and do not care about actually reading anything we must still
        // create an byte array to pass into the method the following is the size of the 
        // byte array used in this situation
        private const int defaultByteArraySize = 1;

        private const int NUM_TRYS = 5;

        private const int MAX_WAIT_THREAD = 1000;

        #region Test Cases

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void ReadAfterClose()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Verifying read method throws exception after a call to Cloes()");

                com.Open();
                Stream serialStream = com.BaseStream;
                com.Close();

                VerifyReadException(serialStream, typeof(ObjectDisposedException));
            }
        }


        [ConditionalFact(nameof(HasOneSerialPort))]
        public void ReadAfterSerialStreamClose()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Verifying read method throws exception after a call to BaseStream.Close()");
                com.Open();
                Stream serialStream = com.BaseStream;
                com.BaseStream.Close();
                VerifyReadException(serialStream, typeof(ObjectDisposedException));
            }
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void Timeout()
        {
            var rndGen = new Random(-55);
            int readTimeout = rndGen.Next(minRandomTimeout, maxRandomTimeout);

            Debug.WriteLine("Verifying ReadTimeout={0}", readTimeout);

            VerifyTimeout(readTimeout);
        }

        private void WriteToCom1()
        {
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var rndGen = new Random(-55);
                var xmitBuffer = new byte[1];
                int sleepPeriod = rndGen.Next(minRandomTimeout, maxRandomTimeout / 2);

                // Sleep some random period with of a maximum duration of half the largest possible timeout value for a read method on COM1
                Thread.Sleep(sleepPeriod);

                com2.Open();
                com2.Write(xmitBuffer, 0, xmitBuffer.Length);

                if (com2.IsOpen)
                    com2.Close();
            }
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void DefaultParityReplaceByte()
        {
            VerifyParityReplaceByte(-1, numRndBytesPairty - 2);
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void NoParityReplaceByte()
        {
            var rndGen = new Random(-55);

            // 		if(!VerifyParityReplaceByte((int)'\0', rndGen.Next(0, numRndBytesPairty - 1), new System.Text.UTF7Encoding())){
            VerifyParityReplaceByte((int)'\0', rndGen.Next(0, numRndBytesPairty - 1), Encoding.Unicode);
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void RNDParityReplaceByte()
        {
            var rndGen = new Random(-55);

            VerifyParityReplaceByte(rndGen.Next(0, 128), 0, new UTF8Encoding());
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void ParityErrorOnLastByte()
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var rndGen = new Random(15);
                var bytesToWrite = new byte[numRndBytesPairty];
                var expectedBytes = new byte[numRndBytesPairty];
                var actualBytes = new byte[numRndBytesPairty + 1];

                IAsyncResult readAsyncResult;

                /* 1 Additional character gets added to the input buffer when the parity error occurs on the last byte of a stream
         We are verifying that besides this everything gets read in correctly. See NDP Whidbey: 24216 for more info on this */
                Debug.WriteLine("Verifying default ParityReplace byte with a parity errro on the last byte");

                // Generate random characters without an parity error
                for (var i = 0; i < bytesToWrite.Length; i++)
                {
                    var randByte = (byte)rndGen.Next(0, 128);

                    bytesToWrite[i] = randByte;
                    expectedBytes[i] = randByte;
                }

                bytesToWrite[bytesToWrite.Length - 1] = (byte)(bytesToWrite[bytesToWrite.Length - 1] | 0x80);
                // Create a parity error on the last byte
                expectedBytes[expectedBytes.Length - 1] = com1.ParityReplace;
                // Set the last expected byte to be the ParityReplace Byte

                com1.Parity = Parity.Space;
                com1.DataBits = 7;
                com1.ReadTimeout = 250;

                com1.Open();
                com2.Open();

                readAsyncResult = com2.BaseStream.BeginWrite(bytesToWrite, 0, bytesToWrite.Length, null, null);
                readAsyncResult.AsyncWaitHandle.WaitOne();

                TCSupport.WaitForReadBufferToLoad(com1, bytesToWrite.Length + 1);

                com1.Read(actualBytes, 0, actualBytes.Length);

                // Compare the chars that were written with the ones we expected to read
                for (var i = 0; i < expectedBytes.Length; i++)
                {
                    if (expectedBytes[i] != actualBytes[i])
                    {
                        Fail("ERROR!!!: Expected to read {0}  actual read  {1}", (int)expectedBytes[i],
                            (int)actualBytes[i]);
                    }
                }

                if (1 < com1.BytesToRead)
                {
                    Fail("ERROR!!!: Expected BytesToRead=0 actual={0}", com1.BytesToRead);
                    Debug.WriteLine("ByteRead={0}, {1}", com1.ReadByte(), bytesToWrite[bytesToWrite.Length - 1]);
                }

                bytesToWrite[bytesToWrite.Length - 1] = (byte)(bytesToWrite[bytesToWrite.Length - 1] & 0x7F);
                // Clear the parity error on the last byte
                expectedBytes[expectedBytes.Length - 1] = bytesToWrite[bytesToWrite.Length - 1];

                VerifyRead(com1, com2, bytesToWrite, expectedBytes, expectedBytes.Length / 2);
            }
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void BytesToRead_RND_Buffer_Size()
        {
            var rndGen = new Random(-55);

            VerifyBytesToRead(rndGen.Next(1, 2 * numRndBytesToRead));
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void BytesToRead_1_Buffer_Size()
        {
            // 		if(!VerifyBytesToRead(1, new System.Text.UTF7Encoding())){
            VerifyBytesToRead(1, Encoding.UTF32);
        }


        [ConditionalFact(nameof(HasNullModem))]
        public void BytesToRead_Equal_Buffer_Size()
        {
            var rndGen = new Random(-55);

            VerifyBytesToRead(numRndBytesToRead, new UTF8Encoding());
        }
        #endregion

        #region Verification for Test Cases
        private void VerifyTimeout(int readTimeout)
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                IAsyncResult readAsyncResult;

                var asyncRead = new AsyncRead(com1);
                var asyncEndRead = new Thread(asyncRead.EndRead);
                int waitTime;
                var asyncCallbackCalled = false;

                com1.Open();
                com2.Open();
                com1.ReadTimeout = readTimeout;

                readAsyncResult = com1.BaseStream.BeginRead(new byte[8], 0, 8,
                    delegate (IAsyncResult ar) { asyncCallbackCalled = true; }, null);
                asyncRead.ReadAsyncResult = readAsyncResult;

                Thread.Sleep(100 > com1.ReadTimeout ? 2 * com1.ReadTimeout : 200);
                // Sleep for 200ms or 2 times the ReadTimeout

                if (readAsyncResult.IsCompleted)
                {
                    // Verify the IAsyncResult has not completed
                    Fail("Err_565088aueiud!!!: Expected read to not have completed");
                }

                asyncEndRead.Start();

                waitTime = 0;
                while (asyncEndRead.ThreadState == ThreadState.Unstarted && waitTime < MAX_WAIT_THREAD)
                {
                    // Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                if (MAX_WAIT_THREAD <= waitTime)
                {
                    Fail("Err_018158ajied!!!: Expected EndRead to have returned");
                }

                Thread.Sleep(100 < com1.ReadTimeout ? 2 * com1.ReadTimeout : 200);
                // Sleep for 200ms or 2 times the ReadTimeout

                if (!asyncEndRead.IsAlive)
                {
                    // Verify EndRead is blocking and is still alive
                    Fail("Err_4085858aiehe!!!: Expected read to not have completed");
                }

                if (asyncCallbackCalled)
                {
                    Fail("Err_750551aiuehd!!!: Expected AsyncCallback not to be called");
                }

                com2.Write(new byte[8], 0, 8);

                waitTime = 0;
                while (asyncEndRead.IsAlive && waitTime < MAX_WAIT_THREAD)
                {
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                if (MAX_WAIT_THREAD <= waitTime)
                {
                    Fail("Err_018158ajied!!!: Expected EndRead to have returned");
                }

                waitTime = 0;
                while (!asyncCallbackCalled && waitTime < 5000)
                {
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                Assert.True(asyncCallbackCalled, "Err_21208aheide!!!: Expected AsyncCallback to be called after some data was written to the port");
            }
        }

        private void VerifyReadException(Stream serialStream, Type expectedException)
        {
            Assert.Throws(expectedException, () =>
            {
                IAsyncResult readAsyncResult = serialStream.BeginRead(new byte[defaultByteArraySize], 0, defaultByteArraySize,
                    null, null);
                readAsyncResult.AsyncWaitHandle.WaitOne();
            });
        }

        private void VerifyParityReplaceByte(int parityReplace, int parityErrorIndex)
        {
            VerifyParityReplaceByte(parityReplace, parityErrorIndex, new ASCIIEncoding());
        }

        private void VerifyParityReplaceByte(int parityReplace, int parityErrorIndex, Encoding encoding)
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var rndGen = new Random(-55);
                var bytesToWrite = new byte[numRndBytesPairty];
                var expectedBytes = new byte[numRndBytesPairty];
                byte expectedByte;

                // Generate random characters without an parity error
                for (var i = 0; i < bytesToWrite.Length; i++)
                {
                    var randByte = (byte)rndGen.Next(0, 128);

                    bytesToWrite[i] = randByte;
                    expectedBytes[i] = randByte;
                }

                if (-1 == parityReplace)
                {
                    // If parityReplace is -1 and we should just use the default value
                    expectedByte = com1.ParityReplace;
                }
                else if ('\0' == parityReplace)
                {
                    // If parityReplace is the null charachater and parity replacement should not occur
                    com1.ParityReplace = (byte)parityReplace;
                    expectedByte = bytesToWrite[parityErrorIndex];
                }
                else
                {
                    // Else parityReplace was set to a value and we should expect this value to be returned on a parity error
                    com1.ParityReplace = (byte)parityReplace;
                    expectedByte = (byte)parityReplace;
                }

                // Create an parity error by setting the highest order bit to true
                bytesToWrite[parityErrorIndex] = (byte)(bytesToWrite[parityErrorIndex] | 0x80);
                expectedBytes[parityErrorIndex] = (byte)expectedByte;

                Debug.WriteLine("Verifying ParityReplace={0} with an ParityError at: {1} ", com1.ParityReplace,
                    parityErrorIndex);

                com1.Parity = Parity.Space;
                com1.DataBits = 7;
                com1.Encoding = encoding;

                com1.Open();
                com2.Open();

                VerifyRead(com1, com2, bytesToWrite, expectedBytes, numBytesReadPairty);
            }
        }


        public void VerifyBytesToRead(int numBytesRead)
        {
            VerifyBytesToRead(numBytesRead, new ASCIIEncoding());
        }


        public void VerifyBytesToRead(int numBytesRead, Encoding encoding)
        {
            using (var com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (var com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                var rndGen = new Random(-55);
                var bytesToWrite = new byte[numRndBytesToRead];

                // Generate random characters 
                for (var i = 0; i < bytesToWrite.Length; i++)
                {
                    var randByte = (byte)rndGen.Next(0, 256);

                    bytesToWrite[i] = randByte;
                }

                Debug.WriteLine("Verifying BytesToRead with a buffer of: {0} ", numBytesRead);

                com1.Encoding = encoding;

                com1.Open();
                com2.Open();

                VerifyRead(com1, com2, bytesToWrite, bytesToWrite, numBytesRead);
            }
        }

        private void VerifyRead(SerialPort com1, SerialPort com2, byte[] bytesToWrite, byte[] expectedBytes, int rcvBufferSize)
        {
            var rcvBuffer = new byte[rcvBufferSize];
            var buffer = new byte[bytesToWrite.Length];
            int totalBytesRead;
            int bytesToRead;

            com2.Write(bytesToWrite, 0, bytesToWrite.Length);
            com1.ReadTimeout = 250;

            TCSupport.WaitForReadBufferToLoad(com1, bytesToWrite.Length);

            totalBytesRead = 0;
            bytesToRead = com1.BytesToRead;

            while (0 != com1.BytesToRead)
            {
                int bytesRead = com1.BaseStream.EndRead(com1.BaseStream.BeginRead(rcvBuffer, 0, rcvBufferSize, null, null));

                // While their are more characters to be read
                if ((bytesToRead > bytesRead && rcvBufferSize != bytesRead) || (bytesToRead <= bytesRead && bytesRead != bytesToRead))
                {
                    // If we have not read all of the characters that we should have
                    Fail("ERROR!!!: Read did not return all of the characters that were in SerialPort buffer");
                }

                if (bytesToWrite.Length < totalBytesRead + bytesRead)
                {
                    // If we have read in more characters then we expect
                    Fail("ERROR!!!: We have received more characters then were sent");
                    break;
                }

                Array.Copy(rcvBuffer, 0, buffer, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;

                if (bytesToWrite.Length - totalBytesRead != com1.BytesToRead)
                {
                    Fail("ERROR!!!: Expected BytesToRead={0} actual={1}", bytesToWrite.Length - totalBytesRead, com1.BytesToRead);
                }

                bytesToRead = com1.BytesToRead;
            }

            // Compare the bytes that were written with the ones we expected to read
            for (var i = 0; i < bytesToWrite.Length; i++)
            {
                if (expectedBytes[i] != buffer[i])
                {
                    Fail("ERROR!!!: Expected to read {0}  actual read  {1}", expectedBytes[i], buffer[i]);
                }
            }
        }

        private class AsyncRead
        {
            private readonly SerialPort _com;
            private IAsyncResult _readAsyncResult;

            public AsyncRead(SerialPort com)
            {
                _com = com;
            }

            public void BeginRead()
            {
                _readAsyncResult = _com.BaseStream.BeginRead(new byte[8], 0, 8, null, null);
            }

            public IAsyncResult ReadAsyncResult
            {
                get
                {
                    return _readAsyncResult;
                }
                set
                {
                    _readAsyncResult = value;
                }
            }

            public void EndRead()
            {
                _com.BaseStream.EndRead(_readAsyncResult);
            }
        }
        #endregion
    }
}
