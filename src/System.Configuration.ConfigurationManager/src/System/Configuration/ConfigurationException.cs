﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration.Internal;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;

namespace System.Configuration
{
    /// <summary>
    ///     A config exception can contain a filename (of a config file) and a line number (of the location in the file in
    ///     which a
    ///     problem was encountered). Section handlers should throw this exception (or subclasses) together with filename and
    ///     line
    ///     number information where possible.
    /// </summary>
    public class ConfigurationException : SystemException
    {
        private string _filename;
        private int _line;

        // Default ctor is required for serialization.
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Init(info.GetString("filename"), info.GetInt32("line"));
        }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException() : this(null, null, null, 0) { }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException(string message) : this(message, null, null, 0) { }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException(string message, Exception inner) : this(message, inner, null, 0) { }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException(string message, XmlNode node)
            : this(message, null, GetXmlNodeFilename(node), GetXmlNodeLineNumber(node))
        { }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException(string message, Exception inner, XmlNode node)
            : this(message, inner, GetXmlNodeFilename(node), GetXmlNodeLineNumber(node))
        { }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException(string message, string filename, int line) : this(message, null, filename, line) { }

        [Obsolete("This class is obsolete, to create a new exception create a System.Configuration!System.Configuration.ConfigurationErrorsException")]
        public ConfigurationException(string message, Exception inner, string filename, int line) : base(message, inner)
        {
            Init(filename, line);
        }

        // The message includes the file/line number information.
        // To get the message without the extra information, use BareMessage.
        public override string Message
        {
            get
            {
                string file = Filename;
                if (!string.IsNullOrEmpty(file))
                {
                    return Line != 0
                        ? BareMessage + " (" + file + " line " + Line.ToString(CultureInfo.InvariantCulture) + ")"
                        : BareMessage + " (" + file + ")";
                }

                return Line != 0
                    ? BareMessage + " (line " + Line.ToString("G", CultureInfo.InvariantCulture) + ")"
                    : BareMessage;
            }
        }

        public virtual string BareMessage => base.Message;

        public virtual string Filename => _filename;

        public virtual int Line => _line;

        private void Init(string filename, int line)
        {
            HResult = HResults.Configuration;
            _filename = filename;
            _line = line;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("filename", _filename);
            info.AddValue("line", _line);
        }

        [Obsolete("This class is obsolete, use System.Configuration!System.Configuration.ConfigurationErrorsException.GetFilename instead")]
        public static string GetXmlNodeFilename(XmlNode node) => (node as IConfigErrorInfo)?.Filename ?? string.Empty;

        [Obsolete("This class is obsolete, use System.Configuration!System.Configuration.ConfigurationErrorsException.GetFilename instead")]
        public static int GetXmlNodeLineNumber(XmlNode node) => (node as IConfigErrorInfo)?.LineNumber ?? 0;
    }
}