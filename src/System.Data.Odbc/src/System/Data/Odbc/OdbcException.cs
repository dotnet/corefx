// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;

namespace System.Data.Odbc
{
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class OdbcException : System.Data.Common.DbException
    {
        private OdbcErrorCollection _odbcErrors = new OdbcErrorCollection();

        internal static OdbcException CreateException(OdbcErrorCollection errors, ODBC32.RetCode retcode)
        {
            StringBuilder builder = new StringBuilder();
            foreach (OdbcError error in errors)
            {
                if (builder.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                }

                builder.Append(SR.GetString(SR.Odbc_ExceptionMessage, ODBC32.RetcodeToString(retcode), error.SQLState, error.Message)); // MDAC 68337
            }
            OdbcException exception = new OdbcException(builder.ToString(), errors);
            return exception;
        }

        internal OdbcException(string message, OdbcErrorCollection errors) : base(message)
        {
            _odbcErrors = errors;
            HResult = HResults.OdbcException;
        }

        private OdbcException(SerializationInfo si, StreamingContext sc) : base(si, sc)
        {
            // Ignoring not deserializable input
            HResult = HResults.OdbcException;
        }

        public OdbcErrorCollection Errors
        {
            get
            {
                return _odbcErrors;
            }
        }

        public override void GetObjectData(SerializationInfo si, StreamingContext context)
        {
            base.GetObjectData(si, context);
            si.AddValue("odbcRetcode", (ODBC32.RETCODE)100, typeof(ODBC32.RETCODE)); // NO DATA
            si.AddValue("odbcErrors", null); // Not specifying type to enable serialization of null value of non-serializable type
        }

        // mdac bug 62559 - if we don't have it return nothing (empty string)
        public override string Source
        {
            get
            {
                if (Errors != null && 0 < Errors.Count)
                {
                    string source = Errors[0].Source;
                    return string.IsNullOrEmpty(source) ? "" : source; // base.Source;
                }
                return ""; // base.Source;
            }
        }
    }
}
