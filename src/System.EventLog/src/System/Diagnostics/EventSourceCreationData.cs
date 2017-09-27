// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics
{
    public class EventSourceCreationData
    {
        private string _logName = "Application";
        private string _machineName = ".";
        private string _source;
        private string _messageResourceFile;
        private string _parameterResourceFile;
        private string _categoryResourceFile;
        private int _categoryCount;

        private EventSourceCreationData() { }

        public EventSourceCreationData(string source, string logName)
        {
            _source = source;
            _logName = logName;
        }

        internal EventSourceCreationData(string source, string logName, string machineName)
        {
            _source = source;
            _logName = logName;
            _machineName = machineName;
        }

        private EventSourceCreationData(string source, string logName, string machineName,
                                          string messageResourceFile, string parameterResourceFile,
                                          string categoryResourceFile, short categoryCount)
        {
            _source = source;
            _logName = logName;
            _machineName = machineName;
            _messageResourceFile = messageResourceFile;
            _parameterResourceFile = parameterResourceFile;
            _categoryResourceFile = categoryResourceFile;
            CategoryCount = categoryCount;
        }


        public string LogName
        {
            get { return _logName; }
            set { _logName = value; }
        }

        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public string MessageResourceFile
        {
            get { return _messageResourceFile; }
            set { _messageResourceFile = value; }
        }

        public string ParameterResourceFile
        {
            get { return _parameterResourceFile; }
            set { _parameterResourceFile = value; }
        }

        public string CategoryResourceFile
        {
            get { return _categoryResourceFile; }
            set { _categoryResourceFile = value; }
        }

        public int CategoryCount
        {
            get { return _categoryCount; }
            set
            {
                if (value > UInt16.MaxValue || value < 0)
                    throw new ArgumentOutOfRangeException(nameof(CategoryCount));

                _categoryCount = value;
            }
        }
    }
}


