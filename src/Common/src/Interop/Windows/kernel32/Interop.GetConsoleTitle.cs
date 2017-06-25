// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetConsoleTitleW")]
        private static extern int GetConsoleTitle([Out]StringBuilder title, int nSize);

        private const int InitialBufferSizeInChars = 256;

        // Although msdn states that the max allowed limit is 65K,
        // desktop limits this to 24500 as buffer sizes greater than it
        // throw.
        private const int MaxAllowedBufferSizeInChars = 24500;

        // GetConsoleTitle sometimes interprets the second parameter (nSize) as number of bytes and sometimes as the number of chars.
        // Instead of doing complicated and dangerous logic to determine if this may or may not occur,
        // we simply assume the worst and reserve a bigger buffer. This way we may use a bit more memory,
        // but we will always be safe.
        private static int CharCountToByteCount(int numChars)
        {
            return numChars * 2;
        }

        // 1. We first try to call the GetConsoleTitle with an InitialBufferSizeInChars of 256.
        // 2. Then based on the return value either increase the capacity or return failure.
        internal static int GetConsoleTitle(out string title, out int titleLength)
        {
            int lastError = 0;
            StringBuilder sb = new StringBuilder(CharCountToByteCount(InitialBufferSizeInChars + 1));

            while (true)
            {
                // If capacity is insufficient, sometimes it returns length,
                // and sometimes it returns 0 with an error ERROR_INSUFFICIENT_BUFFER
                int len = GetConsoleTitle(sb, sb.Capacity + 1); // +1 for null which marshaler adds

                if (len <= 0)
                {
                    lastError = Marshal.GetLastWin32Error();

                    if (len < 0 || lastError != Errors.ERROR_INSUFFICIENT_BUFFER)
                    {
                        title = string.Empty;
                        titleLength = title.Length;
                        return lastError;
                    }
                }
                else if (sb.Capacity > MaxAllowedBufferSizeInChars)
                {
                    // Title is greater than the allowed buffer so we do not read the title and only pass the length to the caller.
                    title = string.Empty;
                    titleLength = len;
                    return 0;
                }
                else
                {
                    title = sb.ToString();
                    titleLength = title.Length;
                    return 0;
                }

                // We need to increase the sb capacity and retry.
                sb.Capacity = CharCountToByteCount(len == 0 ? Math.Min(sb.Capacity * 2, MaxAllowedBufferSizeInChars) : len + 1);
            }
        }
    }
}
