// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Configuration
{
    internal static class FileUtil
    {
        private const int HRESULT_WIN32_FILE_NOT_FOUND = unchecked((int)0x80070002);
        private const int HRESULT_WIN32_PATH_NOT_FOUND = unchecked((int)0x80070003);

        //
        // Use to avoid the perf hit of a Demand when the Demand is not necessary for security.
        // 
        // If trueOnError is set, then return true if we cannot confirm that the file does NOT exist.
        //
        internal static bool FileExists(string filename, bool trueOnError)
        {
            UnsafeNativeMethods.WIN32_FILE_ATTRIBUTE_DATA data;
            bool ok = UnsafeNativeMethods.GetFileAttributesEx(filename, UnsafeNativeMethods.GetFileExInfoStandard,
                out data);
            if (ok)
            {
                // The path exists. Return true if it is a file, false if a directory.
                return (data.fileAttributes & (int)FileAttributes.Directory) != (int)FileAttributes.Directory;
            }
            else
            {
                if (!trueOnError) return false;
                else
                {
                    // Return true if we cannot confirm that the file does NOT exist.
                    int hr = Marshal.GetHRForLastWin32Error();
                    if ((hr == HRESULT_WIN32_FILE_NOT_FOUND) || (hr == HRESULT_WIN32_PATH_NOT_FOUND)) return false;
                    else return true;
                }
            }
        }
    }
}