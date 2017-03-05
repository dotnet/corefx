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
    public class Write_char_int_int_generic : PortsTest
    {
        //Set bounds fore random timeout values.
        //If the min is to low write will not timeout accurately and the testcase will fail
        private const int minRandomTimeout = 250;

        //If the max is to large then the testcase will take forever to run
        private const int maxRandomTimeout = 2000;

        //If the percentage difference between the expected timeout and the actual timeout
        //found through Stopwatch is greater then 10% then the timeout value was not correctly
        //to the write method and the testcase fails.
        private static double s_maxPercentageDifference = .15;

        //The char size used when veryifying exceptions that write will throw 
        private const int CHAR_SIZE_EXCEPTION = 4;

        //The char size used when veryifying timeout 
        private const int CHAR_SIZE_TIMEOUT = 4;

        //The char size used when veryifying BytesToWrite 
        private const int CHAR_SIZE_BYTES_TO_WRITE = 4;

        //The char size used when veryifying Handshake 
        private const int CHAR_SIZE_HANDSHAKE = 8;
        private const int NUM_TRYS = 5;

        #region Test Cases

        [Fact]
        public void WriteWithoutOpen()
        {
            using (SerialPort com = new SerialPort())
            {
                Debug.WriteLine("Verifying write method throws exception without a call to Open()");
                VerifyWriteException(com, typeof(InvalidOperationException));
            }
        }

        [ConditionalFact(nameof(HasOneSerialPort))]
        public void WriteAfterFailedOpen()
        {
            using (SerialPort com = new SerialPort("BAD_PORT_NAME"))
            {
                Debug.WriteLine("Verifying write method throws exception with a failed call to Open()");

                //Since the PortName is set to a bad port name Open will thrown an exception
                //however we don't care what it is since we are verifying a write method
                Assert.ThrowsAny<Exception>(() => com.Open());

                VerifyWriteException(com, typeof(InvalidOperationException));
            }
        }


        [ConditionalFact(nameof(HasOneSerialPort))]
        public void WriteAfterClose()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Debug.WriteLine("Verifying write method throws exception after a call to Cloes()");
                com.Open();
                com.Close();

                VerifyWriteException(com, typeof(InvalidOperationException));
            }
        }

        [OuterLoop("Slow test")]
        [ConditionalFact(nameof(HasNullModem))]
        public void Timeout()
        {
            using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                Random rndGen = new Random(-55);
                byte[] XOffBuffer = new byte[1];

                XOffBuffer[0] = 19;

                com1.WriteTimeout = rndGen.Next(minRandomTimeout, maxRandomTimeout);
                com1.Handshake = Handshake.XOnXOff;

                Debug.WriteLine("Verifying WriteTimeout={0}", com1.WriteTimeout);

                com1.Open();
                com2.Open();

                com2.Write(XOffBuffer, 0, 1);
                Thread.Sleep(250);

                com2.Close();

                VerifyTimeout(com1);
            }
        }

        [OuterLoop("Slow test")]
        [ConditionalFact(nameof(HasOneSerialPort), nameof(HasHardwareFlowControl))]
        public void SuccessiveReadTimeout()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Random rndGen = new Random(-55);


                com.WriteTimeout = rndGen.Next(minRandomTimeout, maxRandomTimeout);
                com.Handshake = Handshake.RequestToSendXOnXOff;
                //		com.Encoding = new System.Text.UTF7Encoding();
                com.Encoding = Encoding.Unicode;

                Debug.WriteLine("Verifying WriteTimeout={0} with successive call to write method", com.WriteTimeout);

                com.Open();

                try
                {
                    com.Write(new char[CHAR_SIZE_TIMEOUT], 0, CHAR_SIZE_TIMEOUT);
                }
                catch (TimeoutException)
                {
                }

                VerifyTimeout(com);
            }
        }

        [ConditionalFact(nameof(HasNullModem), nameof(HasHardwareFlowControl))]
        public void SuccessiveReadTimeoutWithWriteSucceeding()
        {
            using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                Random rndGen = new Random(-55);
                AsyncEnableRts asyncEnableRts = new AsyncEnableRts();
                Thread t = new Thread(asyncEnableRts.EnableRTS);

                int waitTime = 0;

                com1.WriteTimeout = rndGen.Next(minRandomTimeout, maxRandomTimeout);
                com1.Handshake = Handshake.RequestToSend;
                com1.Encoding = new UTF8Encoding();

                Debug.WriteLine("Verifying WriteTimeout={0} with successive call to write method with the write succeeding sometime before its timeout", com1.WriteTimeout);
                com1.Open();

                //Call EnableRTS asynchronously this will enable RTS in the middle of the following write call allowing it to succeed 
                //before the timeout is reached
                t.Start();
                waitTime = 0;

                while (t.ThreadState == ThreadState.Unstarted && waitTime < 2000)
                {
                    //Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                try
                {
                    com1.Write(new char[CHAR_SIZE_TIMEOUT], 0, CHAR_SIZE_TIMEOUT);
                }
                catch (TimeoutException)
                {
                }

                asyncEnableRts.Stop();

                while (t.IsAlive)
                    Thread.Sleep(100);

                VerifyTimeout(com1);
            }
        }

        [ConditionalFact(nameof(HasNullModem), nameof(HasHardwareFlowControl))]
        public void BytesToWrite()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                AsyncWriteRndCharArray asyncWriteRndCharArray = new AsyncWriteRndCharArray(com, CHAR_SIZE_BYTES_TO_WRITE);
                Thread t = new Thread(asyncWriteRndCharArray.WriteRndCharArray);

                int waitTime = 0;

                Debug.WriteLine("Verifying BytesToWrite with one call to Write");

                com.Handshake = Handshake.RequestToSend;
                com.Open();
                com.WriteTimeout = 500;

                //Write a random char[] asynchronously so we can verify some things while the write call is blocking
                t.Start();
                waitTime = 0;

                while (t.ThreadState == ThreadState.Unstarted && waitTime < 2000)
                { //Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                TCSupport.WaitForExactWriteBufferLoad(com, CHAR_SIZE_BYTES_TO_WRITE);

                //Wait for write method to timeout
                while (t.IsAlive)
                    Thread.Sleep(100);
            }
        }

        [ConditionalFact(nameof(HasNullModem), nameof(HasHardwareFlowControl))]
        public void BytesToWriteSuccessive()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                AsyncWriteRndCharArray asyncWriteRndCharArray = new AsyncWriteRndCharArray(com, CHAR_SIZE_BYTES_TO_WRITE);
                Thread t1 = new Thread(asyncWriteRndCharArray.WriteRndCharArray);
                Thread t2 = new Thread(asyncWriteRndCharArray.WriteRndCharArray);

                int waitTime = 0;

                Debug.WriteLine("Verifying BytesToWrite with successive calls to Write");

                com.Handshake = Handshake.RequestToSend;
                com.Open();
                com.WriteTimeout = 4000;

                //Write a random char[] asynchronously so we can verify some things while the write call is blocking
                t1.Start();
                waitTime = 0;

                while (t1.ThreadState == ThreadState.Unstarted && waitTime < 2000)
                { //Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                TCSupport.WaitForExactWriteBufferLoad(com, CHAR_SIZE_BYTES_TO_WRITE);

                //Write a random char[] asynchronously so we can verify some things while the write call is blocking
                t2.Start();
                waitTime = 0;

                while (t2.ThreadState == ThreadState.Unstarted && waitTime < 2000)
                { //Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                TCSupport.WaitForExactWriteBufferLoad(com, CHAR_SIZE_BYTES_TO_WRITE * 2);

                //Wait for both write methods to timeout
                while (t1.IsAlive || t2.IsAlive)
                    Thread.Sleep(100);
            }
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Handshake_None()
        {
            using (SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            {
                AsyncWriteRndCharArray asyncWriteRndCharArray = new AsyncWriteRndCharArray(com, CHAR_SIZE_HANDSHAKE);
                Thread t = new Thread(asyncWriteRndCharArray.WriteRndCharArray);

                int waitTime;

                //Write a random char[] asynchronously so we can verify some things while the write call is blocking
                Debug.WriteLine("Verifying Handshake=None");

                com.Open();

                t.Start();
                waitTime = 0;

                while (t.ThreadState == ThreadState.Unstarted && waitTime < 2000)
                { //Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                //Wait for both write methods to timeout
                while (t.IsAlive)
                    Thread.Sleep(100);

                Assert.Equal(0, com.BytesToWrite);
            }
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Handshake_RequestToSend()
        {
            Verify_Handshake(Handshake.RequestToSend);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Handshake_XOnXOff()
        {
            Verify_Handshake(Handshake.XOnXOff);
        }

        [ConditionalFact(nameof(HasNullModem))]
        public void Handshake_RequestToSendXOnXOff()
        {
            Verify_Handshake(Handshake.RequestToSendXOnXOff);
        }

        public class AsyncEnableRts
        {
            private bool _stop;

            public void EnableRTS()
            {
                lock (this)
                {
                    using (SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
                    {
                        Random rndGen = new Random(-55);
                        int sleepPeriod = rndGen.Next(minRandomTimeout, maxRandomTimeout / 2);

                        //Sleep some random period with of a maximum duration of half the largest possible timeout value for a write method on COM1
                        Thread.Sleep(sleepPeriod);

                        com2.Open();
                        com2.RtsEnable = true;

                        while (!_stop)
                            Monitor.Wait(this);

                        com2.RtsEnable = false;
                    }
                }
            }


            public void Stop()
            {
                lock (this)
                {
                    _stop = true;
                    Monitor.Pulse(this);
                }
            }
        }



        public class AsyncWriteRndCharArray
        {
            private readonly SerialPort _com;
            private readonly int _charLength;


            public AsyncWriteRndCharArray(SerialPort com, int charLength)
            {
                _com = com;
                _charLength = charLength;
            }


            public void WriteRndCharArray()
            {
                char[] buffer = TCSupport.GetRandomChars(_charLength, TCSupport.CharacterOptions.Surrogates);

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
        public static void VerifyWriteException(SerialPort com, Type expectedException)
        {
            Assert.Throws(expectedException, () => com.Write(new char[CHAR_SIZE_EXCEPTION], 0, CHAR_SIZE_EXCEPTION));
        }

        private void VerifyTimeout(SerialPort com)
        {
            Stopwatch timer = new Stopwatch();
            int expectedTime = com.WriteTimeout;
            int actualTime = 0;
            double percentageDifference;

            try
            {
                com.Write(new char[CHAR_SIZE_TIMEOUT], 0, CHAR_SIZE_TIMEOUT); //Warm up write method
            }
            catch (TimeoutException) { }

            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            for (int i = 0; i < NUM_TRYS; i++)
            {
                timer.Start();

                try
                {
                    com.Write(new char[CHAR_SIZE_TIMEOUT], 0, CHAR_SIZE_TIMEOUT);
                }
                catch (TimeoutException) { }

                timer.Stop();
                actualTime += (int)timer.ElapsedMilliseconds;
                timer.Reset();
            }

            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            actualTime /= NUM_TRYS;
            percentageDifference = Math.Abs((expectedTime - actualTime) / (double)expectedTime);

            //Verify that the percentage difference between the expected and actual timeout is less then maxPercentageDifference
            if (s_maxPercentageDifference < percentageDifference)
            {
                Fail("ERROR!!!: The write method timedout in {0} expected {1} percentage difference: {2}", actualTime, expectedTime, percentageDifference);
            }
        }

        private void Verify_Handshake(Handshake handshake)
        {
            using (SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName))
            using (SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName))
            {
                AsyncWriteRndCharArray asyncWriteRndCharArray = new AsyncWriteRndCharArray(com1, CHAR_SIZE_HANDSHAKE);
                Thread t =
                    new Thread(asyncWriteRndCharArray.WriteRndCharArray);

                byte[] XOffBuffer = new byte[1];
                byte[] XOnBuffer = new byte[1];
                int waitTime = 0;

                XOffBuffer[0] = 19;
                XOnBuffer[0] = 17;

                Debug.WriteLine("Verifying Handshake={0}", handshake);

                com1.Handshake = handshake;
                com1.Open();
                com2.Open();

                //Setup to ensure write will bock with type of handshake method being used
                if (Handshake.RequestToSend == handshake || Handshake.RequestToSendXOnXOff == handshake)
                {
                    com2.RtsEnable = false;
                }

                if (Handshake.XOnXOff == handshake || Handshake.RequestToSendXOnXOff == handshake)
                {
                    com2.Write(XOffBuffer, 0, 1);
                    Thread.Sleep(250);
                }

                //Write a random char array asynchronously so we can verify some things while the write call is blocking
                t.Start();
                waitTime = 0;

                while (t.ThreadState == ThreadState.Unstarted && waitTime < 2000)
                {
                    //Wait for the thread to start
                    Thread.Sleep(50);
                    waitTime += 50;
                }

                TCSupport.WaitForExactWriteBufferLoad(com1, CHAR_SIZE_HANDSHAKE);

                //Verify that CtsHolding is false if the RequestToSend or RequestToSendXOnXOff handshake method is used
                if ((Handshake.RequestToSend == handshake || Handshake.RequestToSendXOnXOff == handshake) && com1.CtsHolding)
                {
                    Fail("ERROR!!! Expcted CtsHolding={0} actual {1}", false, com1.CtsHolding);
                }

                //Setup to ensure write will succeed
                if (Handshake.RequestToSend == handshake || Handshake.RequestToSendXOnXOff == handshake)
                {
                    com2.RtsEnable = true;
                }

                if (Handshake.XOnXOff == handshake || Handshake.RequestToSendXOnXOff == handshake)
                {
                    com2.Write(XOnBuffer, 0, 1);
                }

                //Wait till write finishes
                while (t.IsAlive)
                    Thread.Sleep(100);

                //Verify that the correct number of bytes are in the buffer
                Assert.Equal(0, com1.BytesToWrite);

                //Verify that CtsHolding is true if the RequestToSend or RequestToSendXOnXOff handshake method is used
                if ((Handshake.RequestToSend == handshake || Handshake.RequestToSendXOnXOff == handshake) &&
                    !com1.CtsHolding)
                {
                    Fail("ERROR!!! Expcted CtsHolding={0} actual {1}", true, com1.CtsHolding);
                }
            }
        }

        #endregion
    }
}
