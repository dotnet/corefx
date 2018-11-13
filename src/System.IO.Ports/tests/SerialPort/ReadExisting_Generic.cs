// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.PortsTests;
using System.Linq;
using System.Text;
using System.Threading;
using Legacy.Support;
using Xunit;

namespace System.IO.Ports.Tests
{
    public class ReadExisting_Generic : PortsTest
    {
        //Set bounds fore random timeout values.
        //If the min is to low read will not timeout accurately and the testcase will fail
        private const int minRandomTimeout = 250;

        //If the max is to large then the testcase will take forever to run
        private const int maxRandomTimeout = 2000;

        //Since ReadExisting should return immediately this is the maximum time in ms that ReadExisting should take
        //before we consider ReadExisting to be blocking and cause the test case to fail
        private const double maxTimeout = 50;

        //The number of random characters to receive
        private const int numRndChar = 8;
        private const int NUM_TRYS = 5;

        #region Test Cases

        [Fact]
        public void ReadWithoutOpen()
        {
            using (SerialPort com = new SerialPort())
            {
                Debug.WriteLine("Verifying read method throws exception without a call to Open()");
                VerifyReadException(com, typeof(InvalidOperationException));
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void ReadAfterFailedOpen()
        {
            using (SerialPort com = new SerialPort("BAD_PORT_NAME"))
            {
                Debug.WriteLine("Verifying read method throws exception with a failed call to Open()");

                //Since the PortName is set to a bad port name Open will thrown an exception
                //however we don't care what it is since we are verifying a read method
                Assert.ThrowsAny<Exception>(() => com.Open());

                VerifyReadException(com, typeof(InvalidOperationException));
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void ReadAfterClose()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Verifying read method throws exception after a call to Cloes()");
                com.Open();
                com.Close();

                VerifyReadException(com, typeof(InvalidOperationException));
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Timeout()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Verifying ReadTimeout={0}", com.ReadTimeout);
                com.Open();

                VerifyTimeout(com);
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void Default_MaxInt()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                com.ReadTimeout = int.MaxValue;
                Debug.WriteLine("Verifying ReadTimeout={0}", com.ReadTimeout);
                com.Open();

                VerifyTimeout(com);
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void SuccessiveReadTimeoutNoData()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Random rndGen = new Random(-55);

                com.ReadTimeout = rndGen.Next(minRandomTimeout, maxRandomTimeout);
                com.Encoding = Encoding.Unicode;

                Debug.WriteLine("Verifying ReadTimeout={0} with successive call to read method and no data", com.ReadTimeout);
                com.Open();

                Assert.Equal("", com.ReadExisting());

                VerifyTimeout(com);
            }
        }

        [KnownFailure]
        [ConditionalFact(nameof(HasNullModem))]
        public void DefaultParityReplaceByte()
        {
            VerifyParityReplaceByte(-1, numRndChar - 2);
        }

        [KnownFailure]
        [ConditionalFact(nameof(HasNullModem))]
        public void NoParityReplaceByte()
        {
            Random rndGen = new Random(-55);
            VerifyParityReplaceByte('\0', rndGen.Next(0, numRndChar - 1));
        }

        [KnownFailure]
        [ConditionalFact(nameof(HasNullModem))]
        public void RNDParityReplaceByte()
        {
            Random rndGen = new Random(-55);
            VerifyParityReplaceByte(rndGen.Next(0, 128), 0);
        }

        [KnownFailure]
        [ConditionalFact(nameof(HasNullModem))]
        public void ParityErrorOnLastByte()
        {
            using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                Random rndGen = new Random(15);
                byte[] bytesToWrite = new byte[numRndChar];
                char[] expectedChars = new char[numRndChar];

                /* 1 Additional character gets added to the input buffer when the parity error occurs on the last byte of a stream
                 We are verifying that besides this everything gets read in correctly. See NDP Whidbey: 24216 for more info on this */
                Debug.WriteLine("Verifying default ParityReplace byte with a parity errro on the last byte");

                //Genrate random characters without an parity error
                for (int i = 0; i < bytesToWrite.Length; i++)
                {
                    byte randByte = (byte)rndGen.Next(0, 128);

                    bytesToWrite[i] = randByte;
                    expectedChars[i] = (char)randByte;
                }

                bytesToWrite[bytesToWrite.Length - 1] = (byte)(bytesToWrite[bytesToWrite.Length - 1] | 0x80);
                //Create a parity error on the last byte
                expectedChars[expectedChars.Length - 1] = (char)com1.ParityReplace;
                // Set the last expected char to be the ParityReplace Byte

                com1.Parity = Parity.Space;
                com1.DataBits = 7;
                com1.ReadTimeout = 250;

                com1.Open();
                com2.Open();

                com2.Write(bytesToWrite, 0, bytesToWrite.Length);

                TCSupport.WaitForReadBufferToLoad(com1, bytesToWrite.Length);

                char[] actualChars = (com1.ReadExisting()).ToCharArray();

                //Compare the chars that were written with the ones we expected to read
                Assert.Equal(expectedChars, actualChars.Take(expectedChars.Length).ToArray());

                if (1 < com1.BytesToRead)
                {
                    Fail("ERROR!!!: Expected BytesToRead=0 actual={0}", com1.BytesToRead);
                    Debug.WriteLine("ByteRead={0}, {1}", com1.ReadByte(), bytesToWrite[bytesToWrite.Length - 1]);
                }

                com1.DiscardInBuffer();

                bytesToWrite[bytesToWrite.Length - 1] = (byte)'\n';
                expectedChars[expectedChars.Length - 1] = (char)bytesToWrite[bytesToWrite.Length - 1];

                VerifyRead(com1, com2, bytesToWrite, expectedChars);
            }
        }
        #endregion

        #region Verification for Test Cases
        private void VerifyTimeout(SerialPort com)
        {
            Stopwatch timer = new Stopwatch();
            int actualTime = 0;

            string strReadReturn = com.ReadExisting();

            Assert.Equal("", strReadReturn);

            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            for (int i = 0; i < NUM_TRYS; i++)
            {
                timer.Start();
                strReadReturn = com.ReadExisting();
                timer.Stop();

                Assert.Equal("", strReadReturn);

                actualTime += (int)timer.ElapsedMilliseconds;
                timer.Reset();
            }

            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            actualTime /= NUM_TRYS;

            //Verify that the percentage difference between the expected and actual timeout is less then maxPercentageDifference
            if (maxTimeout < actualTime)
            {
                Fail("ERROR!!!: The read method timedout in {0} expected {1}", actualTime, 0);
            }
        }

        private void VerifyReadException(SerialPort com, Type expectedException)
        {
            Assert.Throws(expectedException, com.ReadExisting);
        }

        private void VerifyParityReplaceByte(int parityReplace, int parityErrorIndex)
        {
            using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                Random rndGen = new Random(-55);
                byte[] byteBuffer = new byte[numRndChar];
                char[] charBuffer = new char[numRndChar];
                int expectedChar;

                //Genrate random characters without an parity error
                for (int i = 0; i < byteBuffer.Length; i++)
                {
                    int randChar = rndGen.Next(0, 128);

                    byteBuffer[i] = (byte)randChar;
                    charBuffer[i] = (char)randChar;
                }

                if (-1 == parityReplace)
                {
                    //If parityReplace is -1 and we should just use the default value
                    expectedChar = com1.ParityReplace;
                }
                else if ('\0' == parityReplace)
                {
                    //If parityReplace is the null charachater and parity replacement should not occur
                    com1.ParityReplace = (byte)parityReplace;
                    expectedChar = charBuffer[parityErrorIndex];
                }
                else
                {
                    //Else parityReplace was set to a value and we should expect this value to be returned on a parity error
                    com1.ParityReplace = (byte)parityReplace;
                    expectedChar = parityReplace;
                }

                //Create an parity error by setting the highest order bit to true
                byteBuffer[parityErrorIndex] = (byte)(byteBuffer[parityErrorIndex] | 0x80);
                charBuffer[parityErrorIndex] = (char)expectedChar;

                Debug.WriteLine("Verifying ParityReplace={0} with an ParityError at: {1} ", com1.ParityReplace,
                    parityErrorIndex);

                com1.Parity = Parity.Space;
                com1.DataBits = 7;

                com1.Open();
                com2.Open();

                VerifyRead(com1, com2, byteBuffer, charBuffer);
            }
        }

        private void VerifyRead(SerialPort com1, SerialPort com2, byte[] bytesToWrite, char[] expectedChars)
        {
            char[] buffer = new char[expectedChars.Length];
            int totalBytesRead;
            int totalCharsRead;

            com2.Write(bytesToWrite, 0, bytesToWrite.Length);
            com1.ReadTimeout = 250;

            TCSupport.WaitForReadBufferToLoad(com1, bytesToWrite.Length);

            totalBytesRead = 0;
            totalCharsRead = 0;

            Stopwatch sw = Stopwatch.StartNew();

            while (0 != com1.BytesToRead)
            {
                //While their are more characters to be read
                string rcvString = com1.ReadExisting();
                char[] rcvBuffer = rcvString.ToCharArray();

                int charsRead = rcvBuffer.Length;
                int bytesRead = com1.Encoding.GetByteCount(rcvBuffer, 0, charsRead);

                if (expectedChars.Length < totalCharsRead + charsRead)
                {
                    //If we have read in more characters then we expect
                    Fail("ERROR!!!: We have received more characters then were sent");
                }

                Array.Copy(rcvBuffer, 0, buffer, totalCharsRead, charsRead);

                totalBytesRead += bytesRead;
                totalCharsRead += charsRead;

                if (bytesToWrite.Length - totalBytesRead != com1.BytesToRead)
                {
                    Fail("ERROR!!!: Expected BytesToRead={0} actual={1}", bytesToWrite.Length - totalBytesRead,
                        com1.BytesToRead);
                }

                Assert.True(sw.ElapsedMilliseconds < 5000, "Timeout waiting for read data");
            }

            //Compare the chars that were written with the ones we expected to read
            Assert.Equal(expectedChars, buffer);
        }

        #endregion
    }
}
