// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.PortsTests;
using Legacy.Support;

public class PortName_Property : PortsTest
{
    
    
    
    
    

    //Determines how long the randomly generated PortName is
    public static readonly int rndPortNameSize = 255;

    private enum ThrowAt { Set, Open };

    private int _numErrors = 0;
    private int _numTestcases = 0;
    private int _exitValue = TCSupport.PassExitCode;

    private DosDevices _dosDevices;

    public PortName_Property()
    {
        if (IsWinXPOrHigher())
            _dosDevices = new DosDevices();
        else
            _dosDevices = null;
    }

    public bool RunTest()
    {
        bool retValue = true;
        TCSupport tcSupport = new TCSupport();

        //Note Other test cases relevent to PortName are handled in Open_PortName.cs and ctor_str.cs
        tcSupport.BeginTestcase(new TestDelegate(PortName_COM1_After_Open), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_COM2_After_Open), TCSupport.SerialPortRequirements.TwoSerialPorts);

        tcSupport.BeginTestcase(new TestDelegate(PortName_Empty), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_null), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_SlashSlash), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_SlashSlashSlash), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_SlashSlashRND), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_FileName), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_COM257), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_LPT), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_LPT1), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_PHYSICALDRIVE0), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_A), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_C), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_SystemDrive), TCSupport.SerialPortRequirements.OneSerialPort);
        tcSupport.BeginTestcase(new TestDelegate(PortName_RND), TCSupport.SerialPortRequirements.OneSerialPort);

        _numErrors += tcSupport.NumErrors;
        _numTestcases = tcSupport.NumTestcases;
        _exitValue = tcSupport.ExitValue;

        return retValue;
    }

    #region Test Cases
    public bool PortName_COM1_After_Open()
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        Debug.WriteLine("Verifying setting PortName=COM1 after open has been called");

        bool retValue = true;

        VerifyExceptionAfterOpen(com, TCSupport.LocalMachineSerialInfo.FirstAvailablePortName, typeof(InvalidOperationException));
        if (!retValue)
        {
            Debug.WriteLine("Err_001!!! Verifying setting PortName=COM1 after open has been called FAILED");
        }

        if (com.IsOpen)
            com.Close();

        return retValue;
    }


    public bool PortName_COM2_After_Open()
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.SecondAvailablePortName);

        Debug.WriteLine("Verifying setting PortName=COM2 after open has been called");

        bool retValue = true;

        retValue = VerifyExceptionAfterOpen(com, TCSupport.LocalMachineSerialInfo.SecondAvailablePortName, typeof(InvalidOperationException));
        if (!retValue)
        {
            Debug.WriteLine("Err_002!!! Verifying setting PortName=COM2 after open has been called FAILED");
        }

        if (com.IsOpen)
            com.Close();

        return retValue;
    }


    public bool PortName_Empty()
    {
        Debug.WriteLine("Verifying setting PortName=\"\"");
        VerifyException("", ThrowAt.Set, typeof(ArgumentException), typeof(ArgumentException));
    }


    public bool PortName_null()
    {
        Debug.WriteLine("Verifying setting PortName=null");
        VerifyException(null, ThrowAt.Set, typeof(ArgumentNullException), typeof(ArgumentNullException));
    }


    private bool PortName_RND()
    {
        Random rndGen = new Random();
        System.Text.StringBuilder rndStrBuf = new System.Text.StringBuilder();

        for (int i = 0; i < rndPortNameSize; i++)
        {
            rndStrBuf.Append((char)rndGen.Next(0, UInt16.MaxValue));
        }

        Debug.WriteLine("Verifying setting PortName to a random string");
        VerifyException(rndStrBuf.ToString(), ThrowAt.Open, typeof(ArgumentException), typeof(InvalidOperationException));
    }


    private bool PortName_SlashSlash()
    {
        Debug.WriteLine("Verifying setting PortName=\\\\");
        VerifyException("\\\\", ThrowAt.Set, typeof(ArgumentException), typeof(ArgumentException));
    }


    private bool PortName_SlashSlashSlash()
    {
        Debug.WriteLine("Verifying setting PortName=\\\\\\");
        VerifyException("\\\\\\", ThrowAt.Set, typeof(ArgumentException), typeof(ArgumentException));
    }


    private bool PortName_SlashSlashRND()
    {
        Random rndGen = new Random();
        System.Text.StringBuilder rndStrBuf = new System.Text.StringBuilder();

        rndStrBuf.Append("\\\\");
        for (int i = 0; i < rndPortNameSize; i++)
        {
            rndStrBuf.Append((char)rndGen.Next(0, UInt16.MaxValue));
        }

        Debug.WriteLine("Verifying setting PortName=\\\\ + RND_STR");
        VerifyException(rndStrBuf.ToString(), ThrowAt.Set, typeof(ArgumentException), typeof(ArgumentException));
    }


    private bool PortName_FileName()
    {
        string fileName = "PortNameEqualToFileName.txt";
        System.IO.FileStream testFile = System.IO.File.Open(fileName, System.IO.FileMode.Create);
        System.Text.ASCIIEncoding asciiEncd = new System.Text.ASCIIEncoding();
        string testStr = "Hello World";

        testFile.Write(asciiEncd.GetBytes(testStr), 0, asciiEncd.GetByteCount(testStr));

        testFile.Close();
        Debug.WriteLine("Verifying setting PortName={0}", fileName);

        if (!VerifyException(fileName, ThrowAt.Open, typeof(ArgumentException), typeof(InvalidOperationException)))
        {
            Debug.WriteLine("Err_004!!! Verifying setting PortName to a file name FAILED");
            return false;
        }

        Debug.WriteLine("Verifying setting PortName={0}", Environment.CurrentDirectory + fileName);

        if (!VerifyException(Environment.CurrentDirectory + fileName, ThrowAt.Open, typeof(ArgumentException), typeof(InvalidOperationException)))
        {
            Debug.WriteLine("Err_009!!! Verifying setting PortName to a file name FAILED");
            return false;
        }

        System.IO.File.Delete(fileName);
        return true;
    }


    private bool PortName_COM257()
    {
        Debug.WriteLine("Verifying setting PortName=COM257");
        VerifyException("COM257", ThrowAt.Open, typeof(System.IO.IOException), typeof(InvalidOperationException));
    }


    private bool PortName_LPT()
    {
        Type expectedException;

        if (IsWinXPOrHigher())
        {
            expectedException = _dosDevices.CommonNameExists("LPT") ? typeof(ArgumentException) : typeof(ArgumentException);
        }
        else
        {
            //			This is not Reliable on Win2K so for now we will assume that the machine does NOT have this device
            //			expectedException = dosDevices.InternalNameExists(@"\DosDevices\LPT") ? typeof(ArgumentException) :  typeof(System.ArgumentException);

            expectedException = typeof(ArgumentException);
        }

        Debug.WriteLine("Verifying setting PortName=LPT");
        VerifyException("LPT", ThrowAt.Open, expectedException, typeof(InvalidOperationException));
    }


    private bool PortName_LPT1()
    {
        Type expectedException;

        if (IsWinXPOrHigher())
        {
            expectedException = _dosDevices.CommonNameExists("LPT1") ? typeof(ArgumentException) : typeof(ArgumentException);
        }
        else
        {
            //			This is not Reliable on Win2K so for now we will assume that the machine DOES have this device
            //			expectedException = dosDevices.InternalNameExists(@"\DosDevices\LPT1") ? typeof(ArgumentException) :  typeof(System.ArgumentException);

            expectedException = typeof(ArgumentException);
        }

        Debug.WriteLine("Verifying setting PortName=LPT1");
        VerifyException("LPT1", ThrowAt.Open, expectedException, typeof(InvalidOperationException));
    }


    private bool PortName_PHYSICALDRIVE0()
    {
        Type expectedException;

        if (IsWinXPOrHigher())
        {
            expectedException = _dosDevices.CommonNameExists("PHYSICALDRIVE0") ? typeof(ArgumentException) : typeof(ArgumentException);
        }
        else
        {
            //			This is not Reliable on Win2K so for now we will assume that the machine DOES have this device
            expectedException = typeof(ArgumentException);
        }


        Debug.WriteLine("Verifying setting PortName=PHYSICALDRIVE0");
        VerifyException("PHYSICALDRIVE0", ThrowAt.Open, expectedException, typeof(InvalidOperationException));
    }


    private bool PortName_A()
    {
        Type expectedException;

        if (IsWinXPOrHigher())
        {
            expectedException = _dosDevices.CommonNameExists("A:") ? typeof(ArgumentException) : typeof(ArgumentException);
        }
        else
        {
            //			This is not Reliable on Win2K so for now we will assume that the machine DOES have this device			
            //			expectedException = dosDevices.InternalNameContains(@"\Device\Floppy") ? typeof(ArgumentException) :  typeof(System.ArgumentException);
            expectedException = typeof(ArgumentException);
        }

        Debug.WriteLine("Verifying setting PortName=A:");
        VerifyException("A:", ThrowAt.Open, expectedException, typeof(InvalidOperationException));
    }


    private bool PortName_C()
    {
        Debug.WriteLine("Verifying setting PortName=C:");

        VerifyException("C:", ThrowAt.Open, new Type[] {typeof(ArgumentException), typeof(ArgumentException)}, typeof(InvalidOperationException));
    }

    private bool PortName_SystemDrive()
    {
        Debug.WriteLine("Verifying setting PortName=%SYSTEMDRIVE%");
        String portName = Environment.GetEnvironmentVariable("SystemDrive");

        if (!String.IsNullOrEmpty(portName) &&
            !VerifyException(portName, ThrowAt.Open, new Type[] { typeof(ArgumentException) }, typeof(InvalidOperationException)))
        {
            Debug.WriteLine("Err_01548!!! Verifying setting PortName=C: FAILED");
            return false;
        }

        return true;
    }
    #endregion

    #region Verification for Test Cases

    private bool IsWinXPOrHigher()
    {
        OperatingSystem currentVersion = Environment.OSVersion;

        if (currentVersion.Platform == PlatformID.Win32NT)
        {
            if (5 < currentVersion.Version.Major)
                return true;
            if (5 == currentVersion.Version.Major && 1 <= currentVersion.Version.Minor)
                return true;
        }

        return false;
    }

    private bool VerifyException(string portName, ThrowAt throwAt, Type expectedExceptionAtOpen, Type expectedExceptionAfterOpen)
    {
        return VerifyException(portName, throwAt, new Type[] { expectedExceptionAtOpen }, expectedExceptionAfterOpen);
    }

    private bool VerifyException(string portName, ThrowAt throwAt, Type[] expectedExceptionAtOpen, Type expectedExceptionAfterOpen)
    {
        SerialPort com = new SerialPort(TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);
        bool retValue = true;

        VerifyExceptionAtOpen(com, portName, throwAt, expectedExceptionAtOpen);

        if (com.IsOpen)
            com.Close();

        VerifyExceptionAfterOpen(com, portName, expectedExceptionAfterOpen);

        if (com.IsOpen)
            com.Close();

        return retValue;
    }


    private bool VerifyExceptionAtOpen(SerialPort com, string portName, ThrowAt throwAt, Type expectedException)
    {
        return VerifyExceptionAtOpen(com, portName, throwAt, new Type[] { expectedException });
    }

    private bool VerifyExceptionAtOpen(SerialPort com, string portName, ThrowAt throwAt, Type[] expectedExceptions)
    {
        string origPortName = com.PortName;
        bool retValue = true;
        SerialPortProperties serPortProp = new SerialPortProperties();

        if (null != expectedExceptions && 0 < expectedExceptions.Length)
        {
            serPortProp.SetAllPropertiesToDefaults();
        }
        else
        {
            serPortProp.SetAllPropertiesToOpenDefaults();
        }

        if (ThrowAt.Open == throwAt)
            serPortProp.SetProperty("PortName", portName);
        else
            serPortProp.SetProperty("PortName", TCSupport.LocalMachineSerialInfo.FirstAvailablePortName);

        try
        {
            com.PortName = portName;

            if (ThrowAt.Open == throwAt)
                com.Open();

            if (null != expectedExceptions && 0 < expectedExceptions.Length)
            {
                Console.Write("ERROR!!! Expected Open() to throw ");
                for (int i = 0; i < expectedExceptions.Length; ++i) Console.Write(expectedExceptions[i] + " ");
                Debug.WriteLine(" and nothing was thrown");
                retValue = false;
            }
        }
        catch (Exception e)
        {
            if (null == expectedExceptions || 0 == expectedExceptions.Length)
            {
                Debug.WriteLine("ERROR!!! Expected Open() NOT to throw an exception and the following was thrown:\n{0}", e);
                retValue = false;
            }
            else
            {
                bool exceptionFound = false;
                Type actualExceptionType = e.GetType();

                for (int i = 0; i < expectedExceptions.Length; ++i)
                {
                    if (actualExceptionType == expectedExceptions[i])
                    {
                        exceptionFound = true;
                        break;
                    }
                }

                if (exceptionFound)
                {
                    Debug.WriteLine("Caught expected exception:\n{0}", e.GetType());
                }
                else
                {
                    Console.Write("ERROR!!! Expected Open() throw ");
                    for (int i = 0; i < expectedExceptions.Length; ++i) Console.Write(expectedExceptions[i] + " ");
                    Debug.WriteLine(" and  the following was thrown:\n{0}", e);
                    retValue = false;
                }
            }
        }

        serPortProp.VerifyPropertiesAndPrint(com);
        com.PortName = origPortName;

        return retValue;
    }


    private bool VerifyExceptionAfterOpen(SerialPort com, string portName, Type expectedException)
    {
        bool retValue = true;
        SerialPortProperties serPortProp = new SerialPortProperties();

        com.Open();
        serPortProp.SetAllPropertiesToOpenDefaults();
        serPortProp.SetProperty("PortName", com.PortName);

        try
        {
            com.PortName = portName;
            if (null != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the PortName after Open() to throw {0} and nothing was thrown", expectedException);
                retValue = false;
            }
        }
        catch (Exception e)
        {
            if (null == expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the PortName after Open() NOT to throw an exception and {0} was thrown", e.GetType());
                retValue = false;
            }
            else if (e.GetType() != expectedException)
            {
                Debug.WriteLine("ERROR!!! Expected setting the PortName after Open() throw {0} and {1} was thrown", expectedException, e.GetType());
                retValue = false;
            }
        }

        serPortProp.VerifyPropertiesAndPrint(com);

        return retValue;
    }
    #endregion
}
