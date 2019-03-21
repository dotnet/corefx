// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Data.OleDb {

    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable]
    public sealed class OleDbError {
        readonly private string message;
        readonly private string source;
        readonly private string sqlState;
        readonly private int nativeError;

        internal OleDbError(UnsafeNativeMethods.IErrorRecords errorRecords, int index) {
            OleDbHResult hr;
            int lcid = System.Globalization.CultureInfo.CurrentCulture.LCID;

            Bid.Trace("<oledb.IErrorRecords.GetErrorInfo|API|OS>\n");
            UnsafeNativeMethods.IErrorInfo errorInfo = errorRecords.GetErrorInfo(index, lcid);
            if (null != errorInfo) {

                Bid.Trace("<oledb.IErrorInfo.GetDescription|API|OS>\n");
                hr = errorInfo.GetDescription(out this.message);
                Bid.Trace("<oledb.IErrorInfo.GetDescription|API|OS|RET> Message='%ls'\n", this.message);

                if (OleDbHResult.DB_E_NOLOCALE == hr) { // MDAC 87303
                    Bid.Trace("<oledb.ReleaseComObject|API|OS> ErrorInfo\n");
                    Marshal.ReleaseComObject(errorInfo);

                    Bid.Trace("<oledb.Kernel32.GetUserDefaultLCID|API|OS>\n");
                    lcid = SafeNativeMethods.GetUserDefaultLCID();

                    Bid.Trace("<oledb.IErrorRecords.GetErrorInfo|API|OS> LCID=%d\n", lcid);
                    errorInfo = errorRecords.GetErrorInfo(index, lcid);

                    if (null != errorInfo) {

                        Bid.Trace("<oledb.IErrorInfo.GetDescription|API|OS>\n");
                        hr = errorInfo.GetDescription(out this.message);
                        Bid.Trace("<oledb.IErrorInfo.GetDescription|API|OS|RET> Message='%ls'\n", this.message);
                    }
                }
                if ((hr < 0) && ADP.IsEmpty(this.message)) {
                    this.message = ODB.FailedGetDescription(hr);
                }
                if (null != errorInfo) {

                    Bid.Trace("<oledb.IErrorInfo.GetSource|API|OS>\n");
                    hr = errorInfo.GetSource(out this.source);
                    Bid.Trace("<oledb.IErrorInfo.GetSource|API|OS|RET> Source='%ls'\n", this.source);

                    if (OleDbHResult.DB_E_NOLOCALE == hr) { // MDAC 87303
                        Marshal.ReleaseComObject(errorInfo);

                        Bid.Trace("<oledb.Kernel32.GetUserDefaultLCID|API|OS>\n");
                        lcid = SafeNativeMethods.GetUserDefaultLCID();

                        Bid.Trace("<oledb.IErrorRecords.GetErrorInfo|API|OS> LCID=%d\n", lcid);
                        errorInfo = errorRecords.GetErrorInfo(index, lcid);

                        if (null != errorInfo) {

                            Bid.Trace("<oledb.IErrorInfo.GetSource|API|OS>\n");
                            hr = errorInfo.GetSource(out this.source);
                            Bid.Trace("<oledb.IErrorInfo.GetSource|API|OS|RET> Source='%ls'\n", this.source);
                        }
                    }
                    if ((hr < 0) && ADP.IsEmpty(this.source)) {
                        this.source = ODB.FailedGetSource(hr);
                    }
                    Bid.Trace("<oledb.Marshal.ReleaseComObject|API|OS> ErrorInfo\n");
                    Marshal.ReleaseComObject(errorInfo);
                }
            }

            UnsafeNativeMethods.ISQLErrorInfo sqlErrorInfo;

            Bid.Trace("<oledb.IErrorRecords.GetCustomErrorObject|API|OS> IID_ISQLErrorInfo\n");
            hr = errorRecords.GetCustomErrorObject(index, ref ODB.IID_ISQLErrorInfo, out sqlErrorInfo);

            if (null != sqlErrorInfo) {

                Bid.Trace("<oledb.ISQLErrorInfo.GetSQLInfo|API|OS>\n");
                this.nativeError = sqlErrorInfo.GetSQLInfo(out this.sqlState);

                Bid.Trace("<oledb.ReleaseComObject|API|OS> SQLErrorInfo\n");
                Marshal.ReleaseComObject(sqlErrorInfo);
            }
        }

        public string Message {
            get {
                string message = this.message;
                return ((null != message) ? message : ADP.StrEmpty);
            }
        }

        public int NativeError {
            get {
                return this.nativeError;
            }
        }

        public string Source {
            get {
                string source = this.source;
                return ((null != source) ? source : ADP.StrEmpty);
            }
        }

        public string SQLState {
            get {
                string sqlState = this.sqlState;
                return ((null != sqlState) ? sqlState : ADP.StrEmpty);
            }
        }

        override public string ToString() {
            return Message;
        }
    }
}
