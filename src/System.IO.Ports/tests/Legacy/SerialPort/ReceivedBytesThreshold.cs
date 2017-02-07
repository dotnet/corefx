// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.PortsTests;
using Legacy.Support;

public class ReceivedBytesThreshold_Property : PortsTest
{
    //Maximum random value to use for ReceivedBytesThreshold
    public static readonly int MAX_RND_THRESHOLD = 16;

    //Minimum random value to use for ReceivedBytesThreshold
    public static readonly int MIN_RND_THRESHOLD = 2;

    //Maximum time to wait for all of the expected events to be firered
    public static readonly int MAX_TIME_WAIT = 2000;

    private int _numErrors = 0;
    private int _numTestcases = 0;
    private int _exitValue = TCSupport.PassExitCode;


    public bool RunTest()
    {
        bool retValue = true;
        TCSupport tcSupport = new TCSupport();

        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Default), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Rnd_ExactWrite), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Rnd_MultipleWrite), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Above_Exact), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Above_Below), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Above_1), TCSupport.SerialPortRequirements.NullModem);

        //		retValue &= tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Twice), TCSupport.SerialPortRequirements.NullModem); We can not guarantee that just becuase we write twice the 
        //threshold that the event gets called twice
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Int32MinValue), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_Neg1), TCSupport.SerialPortRequirements.NullModem);
        tcSupport.BeginTestcase(new TestDelegate(ReceivedBytesThreshold_0), TCSupport.SerialPortRequirements.NullModem);

        _numErrors += tcSupport.NumErrors;
        _numTestcases = tcSupport.NumTestcases;
        _exitValue = tcSupport.ExitValue;

        return retValue;
    }

    #region Test Cases
    public bool ReceivedBytesThreshold_Default()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;

        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying default ReceivedBytesThreshold");

        com2.Write(new byte[1], 0, 1);

        if (!rcvEventHandler.WaitForEvent(SerialData.Chars, MAX_TIME_WAIT))
        {
            Debug.WriteLine("ERROR!!!: Event never fired");
            retValue = false;
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, 1, 0);

        if (!retValue)
        {
            Debug.WriteLine("Err_001!!! Verifying default ReceivedBytesThreshold FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Rnd_ExactWrite()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;
        Random rndGen = new Random(-55);
        int receivedBytesThreshold = rndGen.Next(MIN_RND_THRESHOLD, MAX_RND_THRESHOLD);

        com1.ReceivedBytesThreshold = receivedBytesThreshold;
        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("ReceivedBytesThreshold", receivedBytesThreshold);
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying writing exactly the number of bytes of ReceivedBytesThreshold");

        com2.Write(new byte[com1.ReceivedBytesThreshold], 0, com1.ReceivedBytesThreshold);

        if (!rcvEventHandler.WaitForEvent(SerialData.Chars, MAX_TIME_WAIT))
        {
            Debug.WriteLine("ERROR!!!: Event never fired");
            retValue = false;
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, com1.ReceivedBytesThreshold, 0);

        if (!retValue)
        {
            Debug.WriteLine("Err_002!!! Verifying writing exactly the number of bytes of ReceivedBytesThreshold FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Rnd_MultipleWrite()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;
        Random rndGen = new Random(-55);
        int receivedBytesThreshold = rndGen.Next(MIN_RND_THRESHOLD, MAX_RND_THRESHOLD);

        com1.ReceivedBytesThreshold = receivedBytesThreshold;
        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("ReceivedBytesThreshold", receivedBytesThreshold);
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying writing the number of bytes of ReceivedBytesThreshold after several write calls");

        com2.Write(new byte[(int)Math.Floor(com1.ReceivedBytesThreshold / 2.0)], 0, (int)Math.Floor(com1.ReceivedBytesThreshold / 2.0));
        com2.Write(new byte[(int)Math.Ceiling(com1.ReceivedBytesThreshold / 2.0)], 0, (int)Math.Ceiling(com1.ReceivedBytesThreshold / 2.0));

        if (!rcvEventHandler.WaitForEvent(SerialData.Chars, MAX_TIME_WAIT))
        {
            Debug.WriteLine("ERROR!!!: Event never fired");
            retValue = false;
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, com1.ReceivedBytesThreshold, 0);

        if (!retValue)
        {
            Debug.WriteLine("Err_003!!! Verifying writing the number of bytes of ReceivedBytesThreshold after several write calls FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Above_Exact()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;
        Random rndGen = new Random(-55);
        int receivedBytesThreshold = rndGen.Next(MIN_RND_THRESHOLD, MAX_RND_THRESHOLD);

        com1.ReceivedBytesThreshold = receivedBytesThreshold + 1;
        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("ReceivedBytesThreshold", receivedBytesThreshold);
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying writing less then number of bytes of ReceivedBytesThreshold then setting " + "ReceivedBytesThreshold to the number of bytes written");

        com2.Write(new byte[receivedBytesThreshold], 0, receivedBytesThreshold);

        while (com1.BytesToRead < receivedBytesThreshold)
            System.Threading.Thread.Sleep(100);

        if (0 != rcvEventHandler.NumEventsHandled)
        {
            Debug.WriteLine("ERROR!!! Unexpected ReceivedEvent was firered NumEventsHandled={0}", rcvEventHandler.NumEventsHandled);
            retValue = false;
        }
        else
        {
            com1.ReceivedBytesThreshold = receivedBytesThreshold;

            if (!rcvEventHandler.WaitForEvent(SerialData.Chars, MAX_TIME_WAIT))
            {
                Debug.WriteLine("ERROR!!!: Event never fired");
                retValue = false;
            }
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, com1.ReceivedBytesThreshold, 0);

        if (!retValue)
        {
            Debug.WriteLine("Err_004!!! Verifying writing less then number of bytes of ReceivedBytesThreshold then " + "setting ReceivedBytesThreshold to the number of bytes written FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Above_Below()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;
        Random rndGen = new Random(-55);
        int receivedBytesThreshold = rndGen.Next(MIN_RND_THRESHOLD, MAX_RND_THRESHOLD);

        com1.ReceivedBytesThreshold = receivedBytesThreshold + 1;
        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("ReceivedBytesThreshold", receivedBytesThreshold - 1);
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying writing less then number of bytes of ReceivedBytesThreshold then setting " + "ReceivedBytesThreshold to less then the number of bytes written");

        com2.Write(new byte[receivedBytesThreshold], 0, receivedBytesThreshold);

        while (com1.BytesToRead < receivedBytesThreshold)
            System.Threading.Thread.Sleep(100);

        if (0 != rcvEventHandler.NumEventsHandled)
        {
            Debug.WriteLine("ERROR!!! Unexpected ReceivedEvent was firered NumEventsHandled={0}", rcvEventHandler.NumEventsHandled);
            retValue = false;
        }
        else
        {
            com1.ReceivedBytesThreshold = receivedBytesThreshold - 1;

            if (!rcvEventHandler.WaitForEvent(SerialData.Chars, MAX_TIME_WAIT))
            {
                Debug.WriteLine("ERROR!!!: Event never fired");
                retValue = false;
            }
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, com1.ReceivedBytesThreshold, 0);

        if (!retValue)
        {
            Debug.WriteLine("Err_005!!! Verifying writing less then number of bytes of ReceivedBytesThreshold then " + "setting ReceivedBytesThreshold to less then the number of bytes written FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Above_1()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;
        Random rndGen = new Random(-55);
        int receivedBytesThreshold = rndGen.Next(MIN_RND_THRESHOLD, MAX_RND_THRESHOLD);

        com1.ReceivedBytesThreshold = receivedBytesThreshold + 1;
        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("ReceivedBytesThreshold", 1);
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying writing less then number of bytes of ReceivedBytesThreshold then " + "setting ReceivedBytesThreshold to 1");

        com2.Write(new byte[receivedBytesThreshold], 0, receivedBytesThreshold);

        while (com1.BytesToRead < receivedBytesThreshold)
            System.Threading.Thread.Sleep(100);

        if (0 != rcvEventHandler.NumEventsHandled)
        {
            Debug.WriteLine("ERROR!!! Unexpected ReceivedEvent was firered NumEventsHandled={0}", rcvEventHandler.NumEventsHandled);
            retValue = false;
        }
        else
        {
            com1.ReceivedBytesThreshold = 1;

            if (!rcvEventHandler.WaitForEvent(SerialData.Chars, MAX_TIME_WAIT))
            {
                Debug.WriteLine("ERROR!!!: Event never fired");
                retValue = false;
            }
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, com1.ReceivedBytesThreshold, 0);

        if (!retValue)
        {
            Debug.WriteLine("Err_006!!! Verifying writing less then number of bytes of ReceivedBytesThreshold then " + "setting ReceivedBytesThreshold to 1 FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Twice()
    {
        SerialPort com1 = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        SerialPort com2 = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);
        ReceivedEventHandler rcvEventHandler = new ReceivedEventHandler(com1);
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;
        Random rndGen = new Random(-55);
        int receivedBytesThreshold = rndGen.Next(MIN_RND_THRESHOLD, MAX_RND_THRESHOLD);

        com1.ReceivedBytesThreshold = receivedBytesThreshold;
        com1.Open();
        com2.Open();
        com1.DataReceived += new SerialDataReceivedEventHandler(rcvEventHandler.HandleEvent);

        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("ReceivedBytesThreshold", receivedBytesThreshold);
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying writing twice the number of bytes of ReceivedBytesThreshold and ReceivedEvent firered twice");

        com2.Write(new byte[com1.ReceivedBytesThreshold * 2], 0, com1.ReceivedBytesThreshold * 2);

        if (!rcvEventHandler.WaitForEvent(SerialData.Chars, 2, MAX_TIME_WAIT))
        {
            Debug.WriteLine("ERROR!!!: Event never fired");
            retValue = false;
        }

        com1.DiscardInBuffer();

        serPortProp.VerifyPropertiesAndPrint(com1);
        rcvEventHandler.Validate(SerialData.Chars, com1.ReceivedBytesThreshold, 0);
        rcvEventHandler.Validate(SerialData.Chars, 2 * com1.ReceivedBytesThreshold, 1);

        if (!retValue)
        {
            Debug.WriteLine("Err_007!!! Verifying writing twice the number of bytes of ReceivedBytesThreshold and ReceivedEvent firered twice FAILED");
        }

        if (com1.IsOpen)
            com1.Close();

        if (com2.IsOpen)
            com2.Close();

        return retValue;
    }


    public bool ReceivedBytesThreshold_Int32MinValue()
    {
        Debug.WriteLine("Verifying Int32.MinValue ReceivedBytesThreshold");
        VerifyException(Int32.MinValue, typeof(ArgumentOutOfRangeException));
    }


    public bool ReceivedBytesThreshold_Neg1()
    {
        Debug.WriteLine("Verifying -1 ReceivedBytesThreshold");
        VerifyException(-1, typeof(ArgumentOutOfRangeException));
    }


    public bool ReceivedBytesThreshold_0()
    {
        Debug.WriteLine("Verifying 0 ReceivedBytesThreshold");
        VerifyException(0, typeof(ArgumentOutOfRangeException));
    }
    #endregion

    #region Verification for Test Cases
    private bool VerifyException(int receivedBytesThreshold, Type expectedException)
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        bool retValue = true;

        VerifyExceptionAtOpen(com, receivedBytesThreshold, expectedException);

        if (com.IsOpen)
            com.Close();

        VerifyExceptionAfterOpen(com, receivedBytesThreshold, expectedException);

        if (com.IsOpen)
            com.Close();

        return retValue;
    }


    private bool VerifyExceptionAtOpen(SerialPort com, int receivedBytesThreshold, Type expectedException)
    {
        int origReceivedBytesThreshold = com.ReceivedBytesThreshold;
        bool retValue = true;
        SerialPortProperties serPortProp = new SerialPortProperties();

        serPortProp.SetAllPropertiesToDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        try
        {
            com.ReceivedBytesThreshold = receivedBytesThreshold;

            if (null != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected Open() to throw {0} and nothing was thrown", expectedException);
                retValue = false;
            }
        }
        catch (Exception e)
        {
            if (null == expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected Open() NOT to throw an exception and {0} was thrown", e.GetType());
                retValue = false;
            }
            else if (e.GetType() != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected Open() throw {0} and {1} was thrown", expectedException, e.GetType());
                retValue = false;
            }
        }

        serPortProp.VerifyPropertiesAndPrint(com);

        com.ReceivedBytesThreshold = origReceivedBytesThreshold;

        return retValue;
    }


    private bool VerifyExceptionAfterOpen(SerialPort com, int receivedBytesThreshold, Type expectedException)
    {
        bool retValue = true;
        SerialPortProperties serPortProp = new SerialPortProperties();

        com.Open();
        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        try
        {
            com.ReceivedBytesThreshold = receivedBytesThreshold;

            if (null != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the ReceivedBytesThreshold after Open() to throw {0} and nothing was thrown", expectedException);
                retValue = false;
            }
        }
        catch (Exception e)
        {
            if (null == expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the ReceivedBytesThreshold after Open() NOT to throw an exception and {0} was thrown", e.GetType());
                retValue = false;
            }
            else if (e.GetType() != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the ReceivedBytesThreshold after Open() throw {0} and {1} was thrown", expectedException, e.GetType());
                retValue = false;
            }
        }
        serPortProp.VerifyPropertiesAndPrint(com);
        return retValue;
    }



    public class ReceivedEventHandler
    {
        public System.Collections.ArrayList EventType;
        public System.Collections.ArrayList Source;
        public System.Collections.ArrayList BytesToRead;
        public int NumEventsHandled;
        private SerialPort _com;


        public ReceivedEventHandler(SerialPort com)
        {
            _com = com;
            NumEventsHandled = 0;

            EventType = new System.Collections.ArrayList();
            Source = new System.Collections.ArrayList();
            BytesToRead = new System.Collections.ArrayList();
        }


        public void HandleEvent(object source, SerialDataReceivedEventArgs e)
        {
            lock (this)
            {
                try
                {
                    BytesToRead.Add(_com.BytesToRead);
                    EventType.Add(e.EventType);
                    Source.Add(source);

                    NumEventsHandled++;
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp);
                    Debug.WriteLine(exp.StackTrace);
                }
                System.Threading.Monitor.Pulse(this);
            }
        }


        public bool Validate(SerialData eventType, int bytesToRead, int eventIndex)
        {
            bool retValue = true;

            lock (this)
            {
                if (eventIndex >= NumEventsHandled)
                {
                    Debug.WriteLine("ERROR!!! Expected EvenIndex={0} is greater then the number of events handled {1}", eventIndex, NumEventsHandled);
                    return false;
                }

                if (eventType != (SerialData)EventType[eventIndex])
                {
                    Debug.WriteLine("ERROR!!! Expected {0} event type actual={1}", eventType, (SerialData)EventType[eventIndex]);
                    retValue = false;
                }

                if (bytesToRead > (int)BytesToRead[eventIndex])
                {
                    Debug.WriteLine("ERROR!!! Expected BytesToRead={0} actual={1}", bytesToRead, (int)BytesToRead[eventIndex]);
                    retValue = false;
                }

                if (_com != (SerialPort)Source[eventIndex])
                {
                    Debug.WriteLine("ERROR!!! Expected {0} source actual={1}", _com.BaseStream, (System.IO.Stream)Source[eventIndex]);
                    retValue = false;
                }
            }

            return retValue;
        }


        public bool WaitForEvent(SerialData eventType, int timeout)
        {
            return WaitForEvent(eventType, 1, timeout);
        }


        public bool WaitForEvent(SerialData eventType, int numEvents, int timeout)
        {
            lock (this)
            {
                if (EventExists(eventType, numEvents))
                    return true;

                Stopwatch sw = new Stopwatch();

                sw.Start();
                do
                {
                    System.Threading.Monitor.Wait(this, (int)(timeout - sw.ElapsedMilliseconds));

                    if (EventExists(eventType, numEvents))
                        return true;
                } while (sw.ElapsedMilliseconds < timeout);

                sw.Stop();
            }

            return false;
        }


        public bool EventExists(SerialData eventType, int numEvents)
        {
            int numOccurences = 0;

            for (int i = 0; i < NumEventsHandled; i++)
            {
                if (eventType == (SerialData)EventType[i])
                {
                    numOccurences++;
                }
            }

            return numOccurences >= numEvents;
        }
    }
    #endregion
}
