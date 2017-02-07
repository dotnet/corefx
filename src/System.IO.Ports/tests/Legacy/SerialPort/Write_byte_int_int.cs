// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;
using System.Diagnostics;
using System.IO.PortsTests;
using Legacy.Support;

public class Write_byte_int_int : PortsTest
{
     //The string size used when veryifying encoding 
    public static readonly int ENCODING_BUFFER_SIZE = 4;

    //The string size used for large byte array testing
    public static readonly int LARGE_BUFFER_SIZE = 2048;

    //When we test Write and do not care about actually writing anything we must still
    //create an byte array to pass into the method the following is the size of the 
    //byte array used in this situation
    public static readonly int DEFAULT_BUFFER_SIZE = 1;
    public static readonly int DEFAULT_BUFFER_OFFSET = 0;
    public static readonly int DEFAULT_BUFFER_COUNT = 1;

    //The maximum buffer size when a exception occurs
    public static readonly int MAX_BUFFER_SIZE_FOR_EXCEPTION = 255;

    //The maximum buffer size when a exception is not expected
    public static readonly int MAX_BUFFER_SIZE = 8;

    //The default number of times the write method is called when verifying write
    public static readonly int DEFAULT_NUM_WRITES = 3;

    //Delegate to start asynchronous write on the SerialPort com with string of size strSize
    public delegate void AsyncWriteDelegate(SerialPort com, int strSize);

    private int _numErrors = 0;
    private int _numTestcases = 0;
    private int _exitValue = TCSupport.PassExitCode;


    public bool RunTest()
    {
        bool retValue = true;
        TCSupport tcSupport = new TCSupport();

        tcSupport.BeginTestcase(new TestDelegate(Buffer_Null), TCSupport.SerialPortRequirements.OneSerialPort);

        tcSupport.BeginTestcase(new TestDelegate(Offset_NEG1), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(Offset_NEGRND), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(Offset_MinInt), TCSupport.SerialPortRequirements.OneSerialPort);

        tcSupport.BeginTestcase(new TestDelegate(Count_NEG1), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(Count_NEGRND), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(Count_MinInt), TCSupport.SerialPortRequirements.OneSerialPort);

        tcSupport.BeginTestcase(new TestDelegate(OffsetCount_EQ_Length_Plus_1), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(OffsetCount_GT_Length), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(Offset_GT_Length), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(Count_GT_Length), TCSupport.SerialPortRequirements.OneSerialPort);

        tcSupport.BeginTestcase(new TestDelegate(OffsetCount_EQ_Length), TCSupport.SerialPortRequirements.LoopbackOrNullModem);
        tcSupport.BeginTestcase(new TestDelegate(Offset_EQ_Length_Minus_1), TCSupport.SerialPortRequirements.LoopbackOrNullModem);

        tcSupport.BeginTestcase(new TestDelegate(Count_EQ_Length), TCSupport.SerialPortRequirements.LoopbackOrNullModem);
        tcSupport.BeginTestcase(new TestDelegate(Count_EQ_Zero), TCSupport.SerialPortRequirements.LoopbackOrNullModem);

        tcSupport.BeginTestcase(new TestDelegate(ASCIIEncoding), TCSupport.SerialPortRequirements.LoopbackOrNullModem);
        //		retValue &= tcSupport.BeginTestcase(new TestDelegate(UTF7Encoding), TCSupport.SerialPortRequirements.LoopbackOrNullModem);
        tcSupport.BeginTestcase(new TestDelegate(UTF8Encoding), TCSupport.SerialPortRequirements.LoopbackOrNullModem);
        tcSupport.BeginTestcase(new TestDelegate(UTF32Encoding), TCSupport.SerialPortRequirements.LoopbackOrNullModem);
        tcSupport.BeginTestcase(new TestDelegate(UnicodeEncoding), TCSupport.SerialPortRequirements.LoopbackOrNullModem);

        tcSupport.BeginTestcase(new TestDelegate(LargeBuffer), TCSupport.SerialPortRequirements.LoopbackOrNullModem);

        _numErrors += tcSupport.NumErrors;
        _numTestcases = tcSupport.NumTestcases;
        _exitValue = tcSupport.ExitValue;

        return retValue;
    }

    #region Test Cases
    public bool Buffer_Null()
    {
        VerifyWriteException(null, 0, 1, typeof(ArgumentNullException));
    }


    public bool Offset_NEG1()
    {
        VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], -1, DEFAULT_BUFFER_COUNT, typeof(ArgumentOutOfRangeException));
    }


    public bool Offset_NEGRND()
    {
        Random rndGen = new Random(-55);

        VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], rndGen.Next(Int32.MinValue, 0), DEFAULT_BUFFER_COUNT, typeof(ArgumentOutOfRangeException));
    }


    public bool Offset_MinInt()
    {
        VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], Int32.MinValue, DEFAULT_BUFFER_COUNT, typeof(ArgumentOutOfRangeException));
    }


    public bool Count_NEG1()
    {
        VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], DEFAULT_BUFFER_OFFSET, -1, typeof(ArgumentOutOfRangeException));
    }


    public bool Count_NEGRND()
    {
        Random rndGen = new Random(-55);

        VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], DEFAULT_BUFFER_OFFSET, rndGen.Next(Int32.MinValue, 0), typeof(ArgumentOutOfRangeException));
    }


    public bool Count_MinInt()
    {
        VerifyWriteException(new byte[DEFAULT_BUFFER_SIZE], DEFAULT_BUFFER_OFFSET, Int32.MinValue, typeof(ArgumentOutOfRangeException));
    }


    public bool OffsetCount_EQ_Length_Plus_1()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
        int offset = rndGen.Next(0, bufferLength);
        int count = bufferLength + 1 - offset;
        Type expectedException = typeof(ArgumentException);

        VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
    }


    public bool OffsetCount_GT_Length()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
        int offset = rndGen.Next(0, bufferLength);
        int count = rndGen.Next(bufferLength + 1 - offset, Int32.MaxValue);
        Type expectedException = typeof(ArgumentException);

        VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
    }


    public bool Offset_GT_Length()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
        int offset = rndGen.Next(bufferLength, Int32.MaxValue);
        int count = DEFAULT_BUFFER_COUNT;
        Type expectedException = typeof(ArgumentException);

        VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
    }


    public bool Count_GT_Length()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE_FOR_EXCEPTION);
        int offset = DEFAULT_BUFFER_OFFSET;
        int count = rndGen.Next(bufferLength + 1, Int32.MaxValue);
        Type expectedException = typeof(ArgumentException);

        VerifyWriteException(new byte[bufferLength], offset, count, expectedException);
    }


    public bool OffsetCount_EQ_Length()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = rndGen.Next(0, bufferLength - 1);
        int count = bufferLength - offset;
        Type expectedException = typeof(ArgumentException);

        VerifyWrite(new byte[bufferLength], offset, count);
    }


    public bool Offset_EQ_Length_Minus_1()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = bufferLength - 1;
        int count = 1;
        Type expectedException = typeof(ArgumentException);

        VerifyWrite(new byte[bufferLength], offset, count);
    }


    public bool Count_EQ_Length()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = 0;
        int count = bufferLength;
        Type expectedException = typeof(ArgumentException);

        VerifyWrite(new byte[bufferLength], offset, count);
    }


    public bool Count_EQ_Zero()
    {
        int bufferLength = 0;
        int offset = 0;
        int count = bufferLength;

        VerifyWrite(new byte[bufferLength], offset, count);
    }


    public bool ASCIIEncoding()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = rndGen.Next(0, bufferLength - 1);
        int count = rndGen.Next(1, bufferLength - offset);

        VerifyWrite(new byte[bufferLength], offset, count, new System.Text.ASCIIEncoding());
    }


    public bool UTF7Encoding()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = rndGen.Next(0, bufferLength - 1);
        int count = rndGen.Next(1, bufferLength - offset);

        VerifyWrite(new byte[bufferLength], offset, count, new System.Text.UTF7Encoding());
    }


    public bool UTF8Encoding()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = rndGen.Next(0, bufferLength - 1);
        int count = rndGen.Next(1, bufferLength - offset);

        VerifyWrite(new byte[bufferLength], offset, count, new System.Text.UTF8Encoding());
    }


    public bool UTF32Encoding()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = rndGen.Next(0, bufferLength - 1);
        int count = rndGen.Next(1, bufferLength - offset);

        VerifyWrite(new byte[bufferLength], offset, count, new System.Text.UTF32Encoding());
    }


    public bool UnicodeEncoding()
    {
        Random rndGen = new Random(-55);
        int bufferLength = rndGen.Next(1, MAX_BUFFER_SIZE);
        int offset = rndGen.Next(0, bufferLength - 1);
        int count = rndGen.Next(1, bufferLength - offset);

        VerifyWrite(new byte[bufferLength], offset, count, new System.Text.UnicodeEncoding());
    }


    public bool LargeBuffer()
    {
        int bufferLength = LARGE_BUFFER_SIZE;
        int offset = 0;
        int count = bufferLength;

        VerifyWrite(new byte[bufferLength], offset, count, 1);
    }
    #endregion

    #region Verification for Test Cases
    public bool VerifyWriteException(byte[] buffer, int offset, int count, Type expectedException)
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        bool retValue = true;
        int bufferLength = null == buffer ? 0 : buffer.Length;

        Debug.WriteLine("Verifying write method throws {0} buffer.Lenght={1}, offset={2}, count={3}", expectedException, bufferLength, offset, count);
        com.Open();

        try
        {
            com.Write(buffer, offset, count);
            Debug.WriteLine("ERROR!!!: No Excpetion was thrown");
            retValue = false;
        }
        catch (Exception e)
        {
            if (e.GetType() != expectedException)
            {
                Debug.WriteLine("ERROR!!!: {0} exception was thrown expected {1}", e.GetType(), expectedException);
                retValue = false;
            }
        }
        if (com.IsOpen)
            com.Close();

        return retValue;
    }


    public bool VerifyWrite(byte[] buffer, int offset, int count)
    {
        return VerifyWrite(buffer, offset, count, new System.Text.ASCIIEncoding());
    }


    public bool VerifyWrite(byte[] buffer, int offset, int count, int numWrites)
    {
        return VerifyWrite(buffer, offset, count, new System.Text.ASCIIEncoding(), numWrites);
    }


    public bool VerifyWrite(byte[] buffer, int offset, int count, System.Text.Encoding encoding)
    {
        return VerifyWrite(buffer, offset, count, encoding, DEFAULT_NUM_WRITES);
    }


    public bool VerifyWrite(byte[] buffer, int offset, int count, System.Text.Encoding encoding, int numWrites)
    {
        SerialPort com1 = TCSupport.InitFirstSerialPort();
        SerialPort com2 = TCSupport.InitSecondSerialPort(com1);
        Random rndGen = new Random(-55);

        Debug.WriteLine("Verifying write method buffer.Lenght={0}, offset={1}, count={2}, endocing={3}", buffer.Length, offset, count, encoding.EncodingName);

        com1.Encoding = encoding;
        com2.Encoding = encoding;

        com1.Open();

        if (!com2.IsOpen) //This is necessary since com1 and com2 might be the same port if we are using a loopback
            com2.Open();

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)rndGen.Next(0, 256);
        }

        return VerifyWriteByteArray(buffer, offset, count, com1, com2, numWrites);
    }


    public bool VerifyWriteByteArray(byte[] buffer, int offset, int count, SerialPort com1, SerialPort com2)
    {
        return VerifyWriteByteArray(buffer, offset, count, com1, com2, DEFAULT_NUM_WRITES);
    }


    public bool VerifyWriteByteArray(byte[] buffer, int offset, int count, SerialPort com1, SerialPort com2, int numWrites)
    {
        bool retValue = true;
        byte[] oldBuffer, expectedBytes, actualBytes;
        int byteRead;
        int index = 0;

        oldBuffer = (byte[])buffer.Clone();
        expectedBytes = new byte[count];
        actualBytes = new byte[expectedBytes.Length * numWrites];

        for (int i = 0; i < count; i++)
        {
            expectedBytes[i] = buffer[i + offset];
        }

        for (int i = 0; i < numWrites; i++)
        {
            com1.Write(buffer, offset, count);
        }

        com2.ReadTimeout = 500;
        System.Threading.Thread.Sleep((int)(((expectedBytes.Length * numWrites * 10.0) / com1.BaudRate) * 1000) + 250);

        //Make sure buffer was not altered during the write call
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] != oldBuffer[i])
            {
                System.Debug.WriteLine("ERROR!!!: The contents of the buffer were changed from {0} to {1} at {2}", oldBuffer[i], buffer[i], i);
                retValue = false;
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
                Debug.WriteLine("ERROR!!!: We have received more bytes then were sent");
                retValue = false;
                break;
            }

            actualBytes[index] = (byte)byteRead;
            index++;
            if (actualBytes.Length - index != com2.BytesToRead)
            {
                System.Debug.WriteLine("ERROR!!!: Expected BytesToRead={0} actual={1}", actualBytes.Length - index, com2.BytesToRead);
                retValue = false;
            }
        }

        //Compare the bytes that were read with the ones we expected to read
        for (int j = 0; j < numWrites; j++)
        {
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                if (expectedBytes[i] != actualBytes[i + expectedBytes.Length * j])
                {
                    System.Debug.WriteLine("ERROR!!!: Expected to read byte {0}  actual read {1} at {2}", (int)expectedBytes[i], (int)actualBytes[i + expectedBytes.Length * j], i);
                    retValue = false;
                }
            }
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }
    #endregion
}
