// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;
using System.Diagnostics;
using System.IO.PortsTests;
using Legacy.Support;

public class SerialPortRegressions : PortsTest
{
    public static readonly String s_strDtTmVer = "MsftEmpl, 2006/10/10 15:37 MsftEmpl";
    public static readonly String s_strClassMethod = "SerialPortRegressions()";
    public static readonly String s_strTFName = "SerialPortRegressions.cs";
    public static readonly String s_strTFAbbrev = s_strTFName.Substring(0, 6);
    public static readonly String s_strTFPath = Environment.CurrentDirectory;

    private int _numErrors = 0;
    private int _numTestcases = 0;
    private int _exitValue = TCSupport.PassExitCode;
    private static string s_receivedstr = "";
    private static SerialPort s_com1;
    private static SerialPort s_com2;
    private static bool s_readComplete = false;


    public static void Main(string[] args)
    {
        SerialPortRegressions objTest = new SerialPortRegressions();
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(objTest.AppDomainUnhandledException_EventHandler);

        Debug.WriteLine(s_strTFPath + " " + s_strTFName + " , for " + s_strClassMethod + " , Source ver : " + s_strDtTmVer);

        try
        {
            objTest.RunTest();
        }
        catch (Exception e)
        {
            Debug.WriteLine(s_strTFAbbrev + " : FAIL The following exception was thorwn in RunTest(): \n" + e.ToString());
            objTest._numErrors++;
            objTest._exitValue = TCSupport.FailExitCode;
        }

        ////	Finish Diagnostics
        if (objTest._numErrors == 0)
        {
            Debug.WriteLine("PASS.	 " + s_strTFPath + " " + s_strTFName + " ,numTestcases==" + objTest._numTestcases);
        }
        else
        {
            Debug.WriteLine("FAIL!	 " + s_strTFPath + " " + s_strTFName + " ,numErrors==" + objTest._numErrors);

            if (TCSupport.PassExitCode == objTest._exitValue)
                objTest._exitValue = TCSupport.FailExitCode;
        }

        Environment.ExitCode = objTest._exitValue;
    }

    


    //This test is a regression test for DevDivBugs 14181: SerialPort: Data corruption occurs if 
    //DataReceived event is used to receive Unicode characters sent across serial ports
    public bool RunTest()
    {
        bool retValue = true;
        TCSupport tcSupport = new TCSupport();

        retValue &= tcSupport.BeginTestcase(new TestDelegate(UTF8Encoding), TCSupport.SerialPortRequirements.LoopbackOrNullModem);

        _numErrors += tcSupport.NumErrors;
        _numTestcases = tcSupport.NumTestcases;
        _exitValue = tcSupport.ExitValue;

        return retValue;
    }

    public bool UTF8Encoding()
    {
        if (!VerifyReadExisting(new System.Text.UTF8Encoding()))
        {
            Debug.WriteLine("Err_018!!! Verifying readexisting");
            return false;
        }

        return true;
    }


    public bool VerifyReadExisting(System.Text.Encoding encoding)
    {
        string text = "????????????4??????????????????,?11????????????????????????????????????????????????,????????????,??????????";
        s_com1 = TCSupport.InitFirstSerialPort();
        s_com2 = TCSupport.InitSecondSerialPort(s_com1);
        s_com1.ReadTimeout = 500;
        s_com1.Encoding = encoding;
        s_com2.Encoding = encoding;

        s_com1.Open();

        if (!s_com2.IsOpen) //This is necessary since com1 and com2 might be the same port if we are using a loopback
            s_com2.Open();

        s_com2.DataReceived += com2_DataReceived;
        s_com1.Write(text);

        //3 seconds is more than enough time to write a few bytes to the other port	
        TCSupport.WaitForPredicate(delegate () { return s_readComplete == true; }, 3000, "ReadExisting did not complete in a timely fashion.  Timeout");

        if (String.Compare(s_receivedstr, text) == 0)
        {
            Debug.WriteLine("Received and Sent strings are the same");
            return true;
        }
        else
        {
            Debug.WriteLine("Received and Sent strings are different");
            Debug.WriteLine("Sent string:" + text);
            Debug.WriteLine("Received string:" + s_receivedstr);
            return false;
        }
    }


    private static void com2_DataReceived(object o, SerialDataReceivedEventArgs e)
    {
        s_receivedstr += s_com2.ReadExisting();
        s_readComplete = true;
    }
}
