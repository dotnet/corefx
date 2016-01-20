// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NetSecurity
    {
        internal sealed class GssApiException : Exception
        {
            private Status _minorStatus;

            public Status MinorStatus
            {
                get { return _minorStatus;  }
            }

            public GssApiException(string message) : base(message)
            {
            }

            public GssApiException(Status majorStatus, Status minorStatus)
                : this(SR.Format(SR.net_gssapi_operation_failed, majorStatus, minorStatus), majorStatus, minorStatus)
            {
            }

            public GssApiException(string message, Status majorStatus, Status minorStatus)
                : this(message)
            {
                HResult = (int)majorStatus;
                _minorStatus = minorStatus;
            }

            public static void AssertOrThrowIfError(string message, Status majorStatus, Status minorStatus)
            {
                if (majorStatus != Status.GSS_S_COMPLETE)
                {
                    GssApiException ex = new GssApiException(majorStatus, minorStatus);
                    Debug.Fail(message + ": " + ex);
                    throw ex;
                }
            }

#if DEBUG
            public override string ToString()
            {
                return Message + "\n GSSAPI status: " + GetGssApiDisplayStatus((Status)HResult, _minorStatus);
            }

            private static string GetGssApiDisplayStatus(Status majorStatus, Status minorStatus)
            {
                KeyValuePair<Status, bool>[] statusArr = { new KeyValuePair<Status, bool>(majorStatus, false), new KeyValuePair<Status, bool>(minorStatus, true) };
                int length = statusArr.Length;
                string[] msgStrings = new string[length];

                for (int i = 0; i < length; i++)
                {
                    SafeGssBufferHandle msgBuffer;
                    Interop.NetSecurity.Status minStat;
                    int statusLength;
                    if (Status.GSS_S_COMPLETE != DisplayStatus(out minStat, statusArr[i].Key, statusArr[i].Value, out msgBuffer, out statusLength))
                    {
                        msgStrings[i] = "Unknown Error";
                        continue;
                    }
                    using (msgBuffer)
                    {
                        byte[] statusBytes = new byte[statusLength];
                        Interop.NetSecurity.CopyBuffer(msgBuffer, statusBytes, 0);
                        var encoding = new System.Text.ASCIIEncoding();
                        msgStrings[i] = encoding.GetString(statusBytes);
                    }
                }

                return msgStrings[0] + " (" + msgStrings[1] + ") \nStatus: " + majorStatus.ToString("x8") + " (" + minorStatus.ToString("x8") + ")";
            }
        }
    }
#endif
}
