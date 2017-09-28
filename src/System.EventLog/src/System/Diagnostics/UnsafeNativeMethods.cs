﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
namespace Microsoft.Win32
{
    internal static class UnsafeNativeMethods
    {
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        public static extern bool ReportEvent(SafeHandle hEventLog, short type, ushort category,
                                                uint eventID, byte[] userSID, short numStrings, int dataLen, HandleRef strings,
                                                byte[] rawData);
#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors. 
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        public static extern bool ClearEventLog(SafeHandle hEventLog, HandleRef lpctstrBackupFileName);
#pragma warning restore BCL0015
#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors. 
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        public static extern bool GetNumberOfEventLogRecords(SafeHandle hEventLog, out int count);
#pragma warning restore BCL0015
#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors. 
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetOldestEventLogRecord(SafeHandle hEventLog, out int number);
#pragma warning restore BCL0015
#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors. 
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadEventLog(SafeHandle hEventLog, int dwReadFlags,
                                                 int dwRecordOffset, byte[] buffer, int numberOfBytesToRead, out int bytesRead,
                                                 out int minNumOfBytesNeeded);
#pragma warning restore BCL0015
#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors. 
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NotifyChangeEventLog(SafeHandle hEventLog, SafeWaitHandle hEvent);
#pragma warning restore BCL0015
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        public extern static int LookupAccountSid(string systemName, byte[] pSid, StringBuilder szUserName, ref int userNameSize, StringBuilder szDomainName, ref int domainNameSize, ref int eUse);
    }
}
