// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class Kernel32
    {
        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        private const int FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
        private const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
        private const int InitialBufferSize = 256;
        private const int BufferSizeIncreaseFactor = 4;
        private const int MaxAllowedBufferSize = 65 * 1024;

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true, BestFitMapping = true)]
        private static extern int FormatMessage(
            int dwFlags,
            IntPtr lpSource,
            uint dwMessageId,
            int dwLanguageId,
            [Out] StringBuilder lpBuffer,
            int nSize,
            IntPtr[] arguments);

        // Windows API FormatMessage lets you format a message string given an errorcode. 
        internal static string GetMessage(int errorCode)
        {
            return GetMessage(IntPtr.Zero, errorCode);
        }

        internal static string GetMessage(IntPtr moduleHandle, int errorCode)
        {
            var sb = new StringBuilder(InitialBufferSize);
            do
            {
                string errorMsg;
                if (TryGetErrorMessage(moduleHandle, errorCode, sb, out errorMsg))
                {
                    return errorMsg;
                }
                else
                {
                    // increase the capacity of the StringBuilder.
                    sb.Capacity *= BufferSizeIncreaseFactor;
                }
            }
            while (sb.Capacity < MaxAllowedBufferSize);

            // If you come here then a size as large as 65K is also not sufficient and so we give the generic errorMsg.
            return string.Format("Unknown error (0x{0:x})", errorCode);
        }

        private static bool TryGetErrorMessage(IntPtr moduleHandle, int errorCode, StringBuilder sb, out string errorMsg)
        {
            errorMsg = "";

            int flags = FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ARGUMENT_ARRAY;
            if (moduleHandle != IntPtr.Zero)
            {
                flags |= FORMAT_MESSAGE_FROM_HMODULE;
            }

            int result = FormatMessage(flags, moduleHandle, unchecked((uint)errorCode), 0, sb, sb.Capacity, null);
            if (result != 0)
            {
                int i = sb.Length;
                while (i > 0)
                {
                    char ch = sb[i - 1];
                    if (ch > 32 && ch != '.')
                        break;
                    i--;
                }
                errorMsg = sb.ToString(0, i);
            }
            else if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
            {
                return false;
            }
            else
            {
                errorMsg = string.Format("Unknown error (0x{0:x})", errorCode);
            }

            return true;
        }
    }
}