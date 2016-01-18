// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class libgssapi
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


            public static GssApiException Create(string message)
            {
                return new GssApiException(message);
            }

            public static GssApiException Create(Status majorStatus, Status minorStatus)
            {
                return new GssApiException(majorStatus, minorStatus);
            }

            public static GssApiException Create(string message, Status majorStatus, Status minorStatus)
            {
                return new GssApiException(SR.Format(message, majorStatus, minorStatus), majorStatus, minorStatus);
            }

            public static void AssertOrThrowIfError(string message, Status majorStatus, Status minorStatus)
            {
                if (majorStatus != Status.GSS_S_COMPLETE)
                {
                    GssApiException ex = Create(majorStatus, minorStatus);
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
                Tuple<Status, bool>[] statusArr = { Tuple.Create(majorStatus, false), Tuple.Create(minorStatus, true) };
                int length = statusArr.Length;
                string[] msgStrings = new string[length];

                for (int i = 0; i < length; i++)
                {
                    using (SafeGssBufferHandle msgBuffer = new SafeGssBufferHandle())
                    {
                        Interop.libgssapi.Status minStat;
                        if (Status.GSS_S_COMPLETE != GssDisplayStatus(out minStat, statusArr[i].Item1, statusArr[i].Item2, msgBuffer))
                        {
                            continue;
                        }
                        msgStrings[i] = Marshal.PtrToStringAnsi(msgBuffer.Value);
                    }
                }
                return msgStrings[0] + " (" + msgStrings[1] + ") \nStatus: " + majorStatus.ToString("x8") + " (" + minorStatus.ToString("x8") + ")";
            }
        }
    }
#endif
}
