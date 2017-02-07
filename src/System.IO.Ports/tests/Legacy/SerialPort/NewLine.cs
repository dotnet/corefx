// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;

public class NewLine_Property : PortsTest
{
    public bool RunTest()
    {
        bool retValue = true;
        TCSupport tcSupport = new TCSupport();

        //See ReadLine,ReadTo,WriteLine for further testing
        retValue &= tcSupport.BeginTestcase(new TestDelegate(NewLine_Default), TCSupport.SerialPortRequirements.OneSerialPort);
        retValue &= tcSupport.BeginTestcase(new TestDelegate(NewLine_null), TCSupport.SerialPortRequirements.OneSerialPort);
        retValue &= tcSupport.BeginTestcase(new TestDelegate(NewLine_empty_string), TCSupport.SerialPortRequirements.OneSerialPort);

        _numErrors += tcSupport.NumErrors;
        _numTestcases = tcSupport.NumTestcases;
        _exitValue = tcSupport.ExitValue;

        return retValue;
    }

    #region Test Cases
    public bool NewLine_Default()
    {
        SerialPort com1 = new SerialPort();
        SerialPortProperties serPortProp = new SerialPortProperties();
        bool retValue = true;

        Debug.WriteLine("Verifying default NewLine");
        serPortProp.SetAllPropertiesToDefaults();
        retValue &= serPortProp.VerifyPropertiesAndPrint(com1);

        if (com1.IsOpen)
            com1.Close();

        return retValue;
    }


    public bool NewLine_null()
    {
        Debug.WriteLine("Verifying null NewLine");
        if (!VerifyException(null, typeof(System.ArgumentNullException)))
        {
            Debug.WriteLine("Err_002!!! Verifying null NewLine FAILED");
            return false;
        }

        return true;
    }


    public bool NewLine_empty_string()
    {
        Debug.WriteLine("Verifying empty string NewLine");
        if (!VerifyException("", typeof(System.ArgumentException)))
        {
            Debug.WriteLine("Err_003!!! Verifying empty string NewLine FAILED");
            return false;
        }

        return true;
    }
    #endregion

    #region Verification for Test Cases
    private bool VerifyException(string newLine, System.Type expectedException)
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        bool retValue = true;

        retValue &= VerifyExceptionAtOpen(com, newLine, expectedException);

        if (com.IsOpen)
            com.Close();

        retValue &= VerifyExceptionAfterOpen(com, newLine, expectedException);

        if (com.IsOpen)
            com.Close();

        return retValue;
    }


    private bool VerifyExceptionAtOpen(SerialPort com, string newLine, System.Type expectedException)
    {
        string origNewLine = com.NewLine;
        bool retValue = true;
        SerialPortProperties serPortProp = new SerialPortProperties();

        serPortProp.SetAllPropertiesToDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        try
        {
            com.NewLine = newLine;

            if (null != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected Open() to throw {0} and nothing was thrown", expectedException);
                retValue = false;
            }
        }
        catch (System.Exception e)
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

        retValue &= serPortProp.VerifyPropertiesAndPrint(com);
        com.NewLine = origNewLine;

        return retValue;
    }


    private bool VerifyExceptionAfterOpen(SerialPort com, string newLine, System.Type expectedException)
    {
        bool retValue = true;
        SerialPortProperties serPortProp = new SerialPortProperties();

        com.Open();
        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        try
        {
            com.NewLine = newLine;

            if (null != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the NewLine after Open() to throw {0} and nothing was thrown", expectedException);
                retValue = false;
            }
        }
        catch (System.Exception e)
        {
            if (null == expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the NewLine after Open() NOT to throw an exception and {0} was thrown", e.GetType());
                retValue = false;
            }
            else if (e.GetType() != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the NewLine after Open() throw {0} and {1} was thrown", expectedException, e.GetType());
                retValue = false;
            }
        }

        retValue &= serPortProp.VerifyPropertiesAndPrint(com);

        return retValue;
    }
    #endregion
}
