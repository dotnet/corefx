// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace System.Xml
{
    /// <devdoc>
    ///    <para>Returns detailed information about the last parse error, including the error
    ///       number, line number, character position, and a text description.</para>
    /// </devdoc>
    public class XmlException : Exception
    {
        private string _res;
        private string[] _args; // this field is not used, it's here just V1.1 serialization compatibility
        private int _lineNumber;
        private int _linePosition;

        private string _sourceUri;


        private const int HResults_Xml = unchecked((int)0x80131940);

        //provided to meet the ECMA standards
        public XmlException() : this(null)
        {
        }

        //provided to meet the ECMA standards
        public XmlException(String message) : this(message, ((Exception)null), 0, 0)
        {
        }

        //provided to meet ECMA standards
        public XmlException(String message, Exception innerException) : this(message, innerException, 0, 0)
        {
        }

        //provided to meet ECMA standards
        public XmlException(String message, Exception innerException, int lineNumber, int linePosition) :
            this(message, innerException, lineNumber, linePosition, null)
        {
        }

        internal XmlException(String message, Exception innerException, int lineNumber, int linePosition, string sourceUri) :
            base(FormatUserMessage(message, lineNumber, linePosition), innerException)
        {
            HResult = HResults_Xml;
            _res = (message == null ? SR.Xml_DefaultException : SR.Xml_UserException);
            _args = new string[] { message };
            _sourceUri = sourceUri;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        internal XmlException(string res, string[] args) :
            this(res, args, null, 0, 0, null)
        { }

        internal XmlException(string res, string[] args, string sourceUri) :
            this(res, args, null, 0, 0, sourceUri)
        { }

        internal XmlException(string res, string arg) :
            this(res, new string[] { arg }, null, 0, 0, null)
        { }

        internal XmlException(string res, string arg, string sourceUri) :
            this(res, new string[] { arg }, null, 0, 0, sourceUri)
        { }

        internal XmlException(string res, String arg, IXmlLineInfo lineInfo) :
            this(res, new string[] { arg }, lineInfo, null)
        { }

        internal XmlException(string res, String arg, Exception innerException, IXmlLineInfo lineInfo) :
            this(res, new string[] { arg }, innerException, (lineInfo == null ? 0 : lineInfo.LineNumber), (lineInfo == null ? 0 : lineInfo.LinePosition), null)
        { }

        internal XmlException(string res, String arg, IXmlLineInfo lineInfo, string sourceUri) :
            this(res, new string[] { arg }, lineInfo, sourceUri)
        { }

        internal XmlException(string res, string[] args, IXmlLineInfo lineInfo) :
            this(res, args, lineInfo, null)
        { }

        internal XmlException(string res, string[] args, IXmlLineInfo lineInfo, string sourceUri) :
            this(res, args, null, (lineInfo == null ? 0 : lineInfo.LineNumber), (lineInfo == null ? 0 : lineInfo.LinePosition), sourceUri)
        {
        }

        internal XmlException(string res, int lineNumber, int linePosition) :
            this(res, (string[])null, null, lineNumber, linePosition)
        { }

        internal XmlException(string res, string arg, int lineNumber, int linePosition) :
            this(res, new string[] { arg }, null, lineNumber, linePosition, null)
        { }

        internal XmlException(string res, string arg, int lineNumber, int linePosition, string sourceUri) :
            this(res, new string[] { arg }, null, lineNumber, linePosition, sourceUri)
        { }

        internal XmlException(string res, string[] args, int lineNumber, int linePosition) :
            this(res, args, null, lineNumber, linePosition, null)
        { }

        internal XmlException(string res, string[] args, int lineNumber, int linePosition, string sourceUri) :
            this(res, args, null, lineNumber, linePosition, sourceUri)
        { }

        internal XmlException(string res, string[] args, Exception innerException, int lineNumber, int linePosition) :
            this(res, args, innerException, lineNumber, linePosition, null)
        { }

        internal XmlException(string res, string[] args, Exception innerException, int lineNumber, int linePosition, string sourceUri) :
            base(CreateMessage(res, args, lineNumber, linePosition), innerException)
        {
            HResult = HResults_Xml;
            _res = res;
            _args = args;
            _sourceUri = sourceUri;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        private static string FormatUserMessage(string message, int lineNumber, int linePosition)
        {
            if (message == null)
            {
                return CreateMessage(SR.Xml_DefaultException, null, lineNumber, linePosition);
            }
            else
            {
                if (lineNumber == 0 && linePosition == 0)
                {
                    // do not reformat the message when not needed
                    return message;
                }
                else
                {
                    // add line information
                    return CreateMessage(SR.Xml_UserException, new string[] { message }, lineNumber, linePosition);
                }
            }
        }

        private static string CreateMessage(string res, string[] args, int lineNumber, int linePosition)
        {
            string message;

            // No line information -> get resource string and return
            if (lineNumber == 0)
            {
                message = SR.Format(res, args);
            }
            // Line information is available -> we need to append it to the error message
            else
            {
                string lineNumberStr = lineNumber.ToString(CultureInfo.InvariantCulture);
                string linePositionStr = linePosition.ToString(CultureInfo.InvariantCulture);

                message = SR.Format(res, args);
                message = SR.Format(SR.Xml_MessageWithErrorPosition, message, lineNumberStr, linePositionStr);
            }
            return message;
        }

        internal static string[] BuildCharExceptionArgs(string data, int invCharIndex)
        {
            return BuildCharExceptionArgs(data[invCharIndex], invCharIndex + 1 < data.Length ? data[invCharIndex + 1] : '\0');
        }

        internal static string[] BuildCharExceptionArgs(char[] data, int invCharIndex)
        {
            return BuildCharExceptionArgs(data, data.Length, invCharIndex);
        }

        internal static string[] BuildCharExceptionArgs(char[] data, int length, int invCharIndex)
        {
            Debug.Assert(invCharIndex < data.Length);
            Debug.Assert(invCharIndex < length);
            Debug.Assert(length <= data.Length);

            return BuildCharExceptionArgs(data[invCharIndex], invCharIndex + 1 < length ? data[invCharIndex + 1] : '\0');
        }

        internal static string[] BuildCharExceptionArgs(char invChar, char nextChar)
        {
            string[] aStringList = new string[2];

            // for surrogate characters include both high and low char in the message so that a full character is displayed
            if (XmlCharType.IsHighSurrogate(invChar) && nextChar != 0)
            {
                int combinedChar = XmlCharType.CombineSurrogateChar(nextChar, invChar);
                aStringList[0] = new string(new char[] { invChar, nextChar });
                aStringList[1] = string.Format(CultureInfo.InvariantCulture, "0x{0:X2}", combinedChar);
            }
            else
            {
                // don't include 0 character in the string - in means eof-of-string in native code, where this may bubble up to
                if ((int)invChar == 0)
                {
                    aStringList[0] = ".";
                }
                else
                {
                    aStringList[0] = invChar.ToString();
                }
                aStringList[1] = string.Format(CultureInfo.InvariantCulture, "0x{0:X2}", (int)invChar);
            }
            return aStringList;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public int LinePosition
        {
            get { return _linePosition; }
        }

        internal string ResString
        {
            get
            {
                return _res;
            }
        }
    };
} // namespace System.Xml
