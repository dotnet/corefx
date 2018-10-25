// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace System.IO.Ports
{
    internal static partial class InternalResources
    {
        internal static void WinIOError()
        {
            int errorCode = Marshal.GetLastWin32Error();
            WinIOError(errorCode, string.Empty);
        }

        internal static void WinIOError(string str)
        {
            int errorCode = Marshal.GetLastWin32Error();
            WinIOError(errorCode, str);
        }

        // After calling GetLastWin32Error(), it clears the last error field,
        // so you must save the HResult and pass it to this method.  This method
        // will determine the appropriate exception to throw dependent on your
        // error, and depending on the error, insert a string into the message
        // gotten from the ResourceManager.
        internal static void WinIOError(int errorCode, string str)
        {
            switch (errorCode)
            {
                case Interop.Errors.ERROR_FILE_NOT_FOUND:
                case Interop.Errors.ERROR_PATH_NOT_FOUND:
                    if (str.Length == 0)
                        throw new IOException(SR.IO_PortNotFound);
                    else
                        throw new IOException(string.Format(SR.IO_PortNotFoundFileName, str));

                case Interop.Errors.ERROR_ACCESS_DENIED:
                    if (str.Length == 0)
                        throw new UnauthorizedAccessException(SR.UnauthorizedAccess_IODenied_NoPortName);
                    else
                        throw new UnauthorizedAccessException(string.Format(SR.UnauthorizedAccess_IODenied_Port, str));

                case Interop.Errors.ERROR_FILENAME_EXCED_RANGE:
                    if (string.IsNullOrEmpty(str))
                        throw new PathTooLongException(SR.IO_PathTooLong_PortName);
                    else
                        throw new PathTooLongException(SR.Format(SR.IO_PathTooLong_Path_PortName, str));

                case Interop.Errors.ERROR_SHARING_VIOLATION:
                    // error message.
                    if (str.Length == 0)
                        throw new IOException(SR.IO_SharingViolation_NoPortName);
                    else
                        throw new IOException(string.Format(SR.IO_SharingViolation_Port, str));

                default:
                    throw new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));
            }
        }

        // Use this to translate error codes like the above into HRESULTs like
        // 0x80070006 for ERROR_INVALID_HANDLE
        internal static int MakeHRFromErrorCode(int errorCode)
        {
            return unchecked(((int)0x80070000) | errorCode);
        }
    }
}
