// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



//------------------------------------------------------------------------------


namespace System.Data.SqlClient
{
    public sealed class SqlError
    {
        private string _source = TdsEnums.SQL_PROVIDER_NAME;
        private int _number;
        private byte _state;
        private byte _errorClass;
        private string _server;
        private string _message;
        private string _procedure;
        private int _lineNumber;
        private int _win32ErrorCode;

        internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber, uint win32ErrorCode)
            : this(infoNumber, errorState, errorClass, server, errorMessage, procedure, lineNumber)
        {
            _win32ErrorCode = (int)win32ErrorCode;
        }

        internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber)
        {
            _number = infoNumber;
            _state = errorState;
            _errorClass = errorClass;
            _server = server;
            _message = errorMessage;
            _procedure = procedure;
            _lineNumber = lineNumber;
            _win32ErrorCode = 0;
        }

        // There is no exception stack included because the correct exception stack is only available 
        // on SqlException, and to obtain that the SqlError would have to have backpointers all the
        // way back to SqlException.  If the user needs a call stack, they can obtain it on SqlException.
        public override string ToString()
        {
            return typeof(SqlError).ToString() + ": " + _message; // since this is sealed so we can change GetType to typeof
        }

        public string Source
        {
            get { return _source; }
        }

        public int Number
        {
            get { return _number; }
        }

        public byte State
        {
            get { return _state; }
        }

        public byte Class
        {
            get { return _errorClass; }
        }

        public string Server
        {
            get { return _server; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string Procedure
        {
            get { return _procedure; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        internal int Win32ErrorCode
        {
            get { return _win32ErrorCode; }
        }
    }
}
