// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;                        //TextWriter
using System.Text;                      //Encoding

namespace Microsoft.Test.ModuleCore
{
    ////////////////////////////////////////////////////////////////
    // TraceLevel
    //
    ////////////////////////////////////////////////////////////////
    public enum TraceLevel
    {
        None,
        Info,
        Default,
        Debug,
        All,
    }

    ////////////////////////////////////////////////////////////////
    // TestLog
    //
    ////////////////////////////////////////////////////////////////
    public class TestLog
    {
        //Data
        private static ITestLog s_pinternal;
        private static TraceLevel s_plevel = TraceLevel.Default;
        private static TestLogAssertHandler s_passerthandler = null;

        //Accessors
        static public ITestLog Internal
        {
            set
            {
                s_pinternal = value;
            }
            get
            {
                return s_pinternal;
            }
        }

        static public TraceLevel Level
        {
            get { return s_plevel; }
            set { s_plevel = value; }
        }

        static public TestLogAssertHandler AssertHandler
        {
            get { return s_passerthandler; }
        }

        static internal void Dispose()
        {
            //Reset the info.  
            s_pinternal = null;
            s_passerthandler = null;
        }

        //Helpers
        static public string NewLine
        {
            get { return "\n"; }
        }

        static public bool WillTrace(TraceLevel level)
        {
            return (s_plevel >= level);
        }

        static public void Write(object value)
        {
            Write(TestLogFlags.Text, StringEx.ToString(value));
        }

        static public void WriteLine(object value)
        {
            WriteLine(TestLogFlags.Text, StringEx.ToString(value));
        }

        static public void WriteLine()
        {
            WriteLine(TestLogFlags.Text, null);
        }

        static public void Write(string text)
        {
            Write(TestLogFlags.Text, text);
        }

        static public void Write(string text, params object[] args)
        {
            //Delegate
            Write(TestLogFlags.Text, String.Format(text, args));
        }

        static public void WriteLine(string text)
        {
            WriteLine(TestLogFlags.Text, text);
        }

        static public void WriteLine(string text, params object[] args)
        {
            //Delegate
            WriteLine(String.Format(text, args));
        }

        static public void Write(char[] value)
        {
            WriteLine(TestLogFlags.Text, new string(value));
        }

        static public void WriteLine(char[] value)
        {
            WriteLine(TestLogFlags.Text, new string(value));
        }

        static public void WriteXml(string text)
        {
            Write(TestLogFlags.Xml, text);
        }

        static public void WriteRaw(string text)
        {
            Write(TestLogFlags.Raw, text);
        }

        static public void WriteIgnore(string text)
        {
            Write(TestLogFlags.Ignore, text);
        }

        static public void WriteLineIgnore(string text)
        {
            WriteLine(TestLogFlags.Ignore, text);
        }

        static public void Write(TestLogFlags flags, string text)
        {
            if (Internal != null)
                Internal.Write(flags, FixupXml(text));
            else
                Console.Write(text);
        }

        static public void WriteLine(TestLogFlags flags, string text)
        {
            if (Internal != null)
                Internal.WriteLine(flags, FixupXml(text));
            else
                Console.WriteLine(text);
        }

        static public void Trace(String value)
        {
            Trace(TraceLevel.Default, value);
        }

        static public void TraceLine(String value)
        {
            TraceLine(TraceLevel.Default, value);
        }

        static public void TraceLine()
        {
            TraceLine(TraceLevel.Default, null);
        }

        static public void Trace(TraceLevel level, String value)
        {
            if (WillTrace(level))
                Write(TestLogFlags.Trace | TestLogFlags.Ignore, value);
        }

        static public void TraceLine(TraceLevel level, String value)
        {
            if (WillTrace(level))
                Write(TestLogFlags.Trace | TestLogFlags.Ignore, value + TestLog.NewLine);
        }

        static public void TraceLine(TraceLevel level)
        {
            TraceLine(level, null);
        }

        static public bool Compare(bool equal, string message)
        {
            if (equal)
                return true;
            return Compare(false, true, message);
        }

        static public bool Compare(object actual, object expected, string message)
        {
            if (InternalEquals(actual, expected))
                return true;

            //Compare not only compares but throws - so your test stops processing
            //This way processing stops upon the first error, so you don't have to check return
            //values or validate values afterwards.  If you have other items to do, then use the 
            //TestLog.Equals instead of TestLog.Compare
            throw new TestFailedException(message, actual, expected, null);
        }

        static public bool Compare(object actual, object expected1, object expected2, string message)
        {
            if (InternalEquals(actual, expected1) || InternalEquals(actual, expected2))
                return true;

            //Compare not only compares but throws - so your test stops processing
            //This way processing stops upon the first error, so you don't have to check return
            //values or validate values afterwards.  If you have other items to do, then use the 
            //TestLog.Equals instead of TestLog.Compare
            throw new TestFailedException(message, actual, expected2, null);
        }

        static public bool Equals(object actual, object expected, string message)
        {
            //Equals is identical to Compare, except that Equals doesn't throw.
            //i.e. the test wants to record the failure and continue to do other things
            if (InternalEquals(actual, expected))
                return true;

            TestLog.Error(TestResult.Failed, actual, expected, null, message, new Exception().StackTrace, null, 0);

            return false;
        }

        static public bool Warning(bool equal, string message)
        {
            return Warning(equal, true, message, null);
        }

        static public bool Warning(object actual, object expected, string message)
        {
            return Warning(actual, expected, message, null);
        }

        static public bool Warning(object actual, object expected, string message, Exception inner)
        {
            //See if these are equal
            bool equal = InternalEquals(actual, expected);
            if (equal)
                return true;

            try
            {
                throw new TestWarningException(message, actual, expected, inner);
            }
            catch (Exception e)
            {
                //Warning should continue - not halt test progress
                TestLog.HandleException(e);
                return false;
            }
        }

        static public bool Skip(string message)
        {
            //Delegate
            return Skip(true, message);
        }

        static public bool Skip(bool skip, string message)
        {
            if (skip)
                throw new TestSkippedException(message);
            return false;
        }

        static internal bool InternalEquals(object actual, object expected)
        {
            //Handle null comparison
            if (actual == null && expected == null)
                return true;
            else if (actual == null || expected == null)
                return false;

            //Otherwise
            return expected.Equals(actual);
        }

        static public void Error(TestResult result, object actual, object expected, string source, string message, string stack, String filename, int lineno)
        {
            //Log the error
            if (Internal != null)
            {
                Internal.Error(result,
                            TestLogFlags.Text,          	//flags        
                            StringEx.Format(actual),		//actual
                            StringEx.Format(expected),		//expected
                            source,							//source
                            message,						//message
                            stack,	    					//stack
                            filename,		                //filename
                            lineno	                        //line
                        );
            }
            else
            {
                //We call IError::Compare, which logs the error AND increments the error count...
                Console.WriteLine("Message:\t" + message);
                Console.WriteLine("Source:\t\t" + source);
                Console.WriteLine("Expected:\t" + expected);
                Console.WriteLine("Received:\t" + actual);
                Console.WriteLine("Details:" + TestLog.NewLine + stack);
                Console.WriteLine("File:\t\t" + filename);
                Console.WriteLine("Line:\t\t" + lineno);
            }
        }

        public static TestResult HandleException(Exception e)
        {
            TestResult result = TestResult.Failed;
            Exception inner = e;

            if (!(inner is TestException) ||
                       ((inner as TestException).Result != TestResult.Skipped &&
                        (inner as TestException).Result != TestResult.Passed &&
                        (inner as TestException).Result != TestResult.Warning))
                inner = e; //start over so we do not loose the stack trace

            while (inner != null)
            {
                String source = inner.Source;

                //Message
                String message = inner.Message;
                if (inner != e)
                    message = "Inner Exception -> " + message;

                //Expected / Actual
                Object actual = inner.GetType();
                Object expected = null;
                String details = inner.StackTrace;
                String filename = null;
                int line = 0;
                if (inner is TestException)
                {
                    TestException testinner = (TestException)inner;

                    //Setup more meaningful info
                    actual = testinner.Actual;
                    expected = testinner.Expected;
                    result = testinner.Result;
                    switch (result)
                    {
                        case TestResult.Passed:
                        case TestResult.Skipped:
                            WriteLine(message);
                            return result;
                    }
                }

                //Log
                TestLog.Error(result, actual, expected, source, message, details, filename, line);

                //Next
                inner = inner.InnerException;
            }

            return result;
        }

        private static String FixupXml(String value)
        {
            bool escapeXmlStuff = false;
            if (value == null) return null;

            StringBuilder b = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '&':
                        if (escapeXmlStuff) b.Append("&amp;"); else b.Append('&');
                        break;
                    case '<':
                        if (escapeXmlStuff) b.Append("&lt;"); else b.Append('<');
                        break;
                    case '>':
                        if (escapeXmlStuff) b.Append("&gt;"); else b.Append('>');
                        break;
                    case '"':
                        if (escapeXmlStuff) b.Append("&quot;"); else b.Append('"');
                        break;
                    case '\'':
                        if (escapeXmlStuff) b.Append("&apos;"); else b.Append('\'');
                        break;
                    case '\t':
                        b.Append('\t');
                        break;
                    case '\r':
                        b.Append('\r');
                        break;
                    case '\n':
                        b.Append('\n');
                        break;
                    default:
                        if ((value[i] < 0x20) || value[i] > 0x80)
                        {
                            b.Append(PrintUnknownCharacter(value[i]));
                        }
                        else
                        {
                            b.Append(value[i]);
                        }
                        break;
                }
            }
            return b.ToString();
        }

        private static string PrintUnknownCharacter(char ch)
        {
            int number = ch;
            string result = string.Empty;
            if (number == 0)
            {
                result = "0";
            }
            while (number > 0)
            {
                int n = number % 16;
                number = number / 16;
                if (n < 10)
                {
                    result = (char)(n + (int)'0') + result;
                }
                else
                {
                    result = (char)(n - 10 + (int)'A') + result;
                }
            }
            return "px" + result + "p";
        }
    }

    ////////////////////////////////////////////////////////////////
    // TestLogWriter
    //
    ////////////////////////////////////////////////////////////////
    public class TestLogWriter : TextWriter
    {
        //Data
        protected TestLogFlags pflags = TestLogFlags.Text;

        //Constructor
        public TestLogWriter(TestLogFlags flags)
        {
            pflags = flags;
        }

        //Overrides    
        public override void Write(char ch)
        {
            //A subclass must minimally implement the Write(Char) method. 
            Write(ch.ToString());
        }

        public override void Write(string text)
        {
            //Delegate
            TestLog.Write(pflags, text);
        }

        public override void Write(char[] ch)
        {
            //Note: This is a workaround the TextWriter::Write(char[]) that incorrectly 
            //writes 1 char at a time, which means \r\n is written sperately and then gets fixed
            //up to be two carriage returns!
            if (ch != null)
            {
                StringBuilder builder = new StringBuilder(ch.Length);
                builder.Append(ch);
                Write(builder.ToString());
            }
        }

        public override void WriteLine(string strText)
        {
            Write(strText + this.NewLine);
        }

        public override void WriteLine()
        {
            //Writes a line terminator to the text stream. 
            //The default line terminator is a carriage return followed by a line feed ("\r\n"), 
            //but this value can be changed using the NewLine property.
            Write(this.NewLine);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }

    ////////////////////////////////////////////////////////////////
    // TestLogAssertHandler
    //
    ////////////////////////////////////////////////////////////////
    public class TestLogAssertHandler
    {
        //Data
        protected bool pshouldthrow = false;

        //Constructor
        public TestLogAssertHandler()
        {
        }

        //Accessors
        public virtual bool ShouldThrow
        {
            get { return pshouldthrow; }
            set { pshouldthrow = value; }
        }

        //Overloads
        public void Fail(string message, string details)
        {
            //Handle the assert, treat it as an error.
            Exception e = new TestException(TestResult.Assert, message, details, null, null);
            e.Source = "Debug.Assert";

            //Note: We don't throw the exception (by default), since we want to continue on Asserts
            //(as many of them are benign or simply a valid runtime error).
            if (this.ShouldThrow)
                throw e;

            TestLog.Error(TestResult.Assert, details, null, "Debug.Assert", message, new Exception().StackTrace, null, 0);
        }

        public void Write(string strText)
        {
        }

        public void WriteLine(string strText)
        {
        }
    }
}
