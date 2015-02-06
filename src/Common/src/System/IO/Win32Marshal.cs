// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.IO
{
    /// <summary>
    ///     Provides static methods for converting from Win32 errors codes to exceptions, HRESULTS and error messages.
    /// </summary>
    internal static class Win32Marshal
    {
        /// <summary>
        ///     Converts, resetting it, the last Win32 error into a corresponding <see cref="Exception"/> object.
        /// </summary>
        internal static Exception GetExceptionForLastWin32Error()
        {
            int errorCode = Marshal.GetLastWin32Error();
            return GetExceptionForWin32Error(errorCode, String.Empty);
        }

        /// <summary>
        ///     Converts, resetting it, the last Win32 error into a corresponding <see cref="Exception"/> object, optionally 
        ///     including the specified path in the error message.
        /// </summary>
        internal static Exception GetExceptionForLastWin32Error(string path)
        {
            int errorCode = Marshal.GetLastWin32Error();
            return GetExceptionForWin32Error(errorCode, path);
        }

        /// <summary>
        ///     Converts the specified Win32 error into a corresponding <see cref="Exception"/> object.
        /// </summary>
        internal static Exception GetExceptionForWin32Error(int errorCode)
        {
            return GetExceptionForWin32Error(errorCode, string.Empty);
        }

        /// <summary>
        ///     Converts the specified Win32 error into a corresponding <see cref="Exception"/> object, optionally 
        ///     including the specified path in the error message.
        /// </summary>
        internal static Exception GetExceptionForWin32Error(int errorCode, string path)
        {
            switch (errorCode)
            {
                case Interop.ERROR_FILE_NOT_FOUND:
                    if (path.Length == 0)
                        return new FileNotFoundException(SR.IO_FileNotFound);
                    else
                        return new FileNotFoundException(SR.Format(SR.IO_FileNotFound_FileName, path), path);

                case Interop.ERROR_PATH_NOT_FOUND:
                    if (path.Length == 0)
                        return new DirectoryNotFoundException(SR.IO_PathNotFound_NoPathName);
                    else
                        return new DirectoryNotFoundException(SR.Format(SR.IO_PathNotFound_Path, path));

                case Interop.ERROR_ACCESS_DENIED:
                    if (path.Length == 0)
                        return new UnauthorizedAccessException(SR.UnauthorizedAccess_IODenied_NoPathName);
                    else
                        return new UnauthorizedAccessException(SR.Format(SR.UnauthorizedAccess_IODenied_Path, path));

                case Interop.ERROR_ALREADY_EXISTS:
                    if (path.Length == 0)
                        goto default;

                    return new IOException(SR.Format(SR.IO_AlreadyExists_Name, path), MakeHRFromErrorCode(errorCode));

                case Interop.ERROR_FILENAME_EXCED_RANGE:
                    return new PathTooLongException(SR.IO_PathTooLong);

                case Interop.ERROR_INVALID_PARAMETER:
                    return new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));

                case Interop.ERROR_SHARING_VIOLATION:
                    if (path.Length == 0)
                        return new IOException(SR.IO_SharingViolation_NoFileName, MakeHRFromErrorCode(errorCode));
                    else
                        return new IOException(SR.Format(SR.IO_SharingViolation_File, path), MakeHRFromErrorCode(errorCode));

                case Interop.ERROR_FILE_EXISTS:
                    if (path.Length == 0)
                        goto default;

                    return new IOException(SR.Format(SR.IO_FileExists_Name, path), MakeHRFromErrorCode(errorCode));

                case Interop.ERROR_OPERATION_ABORTED:
                    return new OperationCanceledException();

                default:
                    return new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));
            }
        }

        /// <summary>
        ///     Returns a HRESULT for the specified Win32 error code.
        /// </summary>
        internal static int MakeHRFromErrorCode(int errorCode)
        {
            Debug.Assert((0xFFFF0000 & errorCode) == 0, "This is an HRESULT, not an error code!");

            return unchecked(((int)0x80070000) | errorCode);
        }

        /// <summary>
        ///     Returns a Win32 error code for the specified HRESULT if it came from FACILITY_WIN32
        ///     If not, returns the HRESULT unchanged
        /// </summary>
        internal static int TryMakeWin32ErrorCodeFromHR(int hr)
        {
            if ((0xFFFF0000 & hr) == 0x80070000)
            {
                // Win32 error, Win32Marshal.GetExceptionForWin32Error expects the Win32 format
                hr &= 0x0000FFFF;
            }

            return hr;
        }

        /// <summary>
        ///     Returns a string message for the specified Win32 error code.
        /// </summary>
        internal static string GetMessage(int errorCode)
        {
            string errorMsg;

            var sb = new StringBuilder(256);
            do
            {
                if (TryGetErrorMessage(errorCode, sb, out errorMsg))
                    return errorMsg;
                else
                {
                    // increase the capacity of the StringBuilder by 4.
                    sb.Capacity *= 4;
                }
            }
            while (sb.Capacity < MaxAllowedBufferSize);

            // If you come here then a size as large as 65K is also not sufficient and so we give the generic errorMsg.
            return string.Format("Unknown error (0x{0:x})", errorCode);
        }

        private static bool TryGetErrorMessage(int errorCode, StringBuilder sb, out string errorMsg)
        {
            errorMsg = "";

            int result = Interop.mincore.FormatMessage(
                                        Interop.mincore.FORMAT_MESSAGE_IGNORE_INSERTS |
                                        Interop.mincore.FORMAT_MESSAGE_FROM_SYSTEM |
                                        Interop.mincore.FORMAT_MESSAGE_ARGUMENT_ARRAY,
                                        IntPtr.Zero, (uint)errorCode, 0, sb, sb.Capacity + 1,
                                        null);
            if (result != 0)
            {
                int i = sb.Length;
                while (i > 0)
                {
                    char ch = sb[i - 1];
                    if (ch > 32 && ch != '.') break;
                    i--;
                }
                errorMsg = sb.ToString(0, i);
            }
            else if (Marshal.GetLastWin32Error() == Interop.mincore.ERROR_INSUFFICIENT_BUFFER)
            {
                return false;
            }
            else
            {
                errorMsg = string.Format("Unknown error (0x{0:x})", errorCode);
            }

            return true;
        }

        // Windows API FormatMessage lets you format a message string given an errocode.
        // Unlike other APIs this API does not support a way to query it for the total message size.
        //
        // So the API can only be used in one of these two ways.
        // a. You pass a buffer of appropriate size and get the resource.
        // b. Windows creates a buffer and passes the address back and the onus of releasing the bugffer lies on the caller.
        //
        // Since the error code is coming from the user, it is not possible to know the size in advance.
        // Unfortunately we can't use option b. since the buffer can only be freed using LocalFree and it is a private API on onecore.
        // Also, using option b is ugly for the manged code and could cause memory leak in situations where freeing is unsuccessful.
        // 
        // As a result we use the following approach.
        // We initially call the API with a buffer size of 256 and then gradually increase the size in case of failure until we reach the maxiumum allowed limit of 65K.
        private const int MaxAllowedBufferSize = 65 * 1024;
    }
}