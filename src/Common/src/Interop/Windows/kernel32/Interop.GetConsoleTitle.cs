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

        // 1. We first try to call the GetConsoleTitle with an InitialBufferSizeInChars of 256.
        // 2. Then based on the return value either increase the capacity or return failure.
        internal static int GetConsoleTitle(out string title, out int titleLength)
        {
            int lastError = 0;

            // See note below for why we're pessimistically setting the capacity as if it was bytes, even though it's in chars.
            StringBuilder sb = new StringBuilder(InitialBufferSizeInChars * sizeof(char));

            while (true)
            {
                // If capacity is insufficient, sometimes this function returns length,
                // and sometimes (Windows 10 RS2) it returns 0 with either ERROR_INSUFFICIENT_BUFFER or ERROR_SUCCESS.
                // In either of those latter cases, try again hopefully with a larger buffer.
                //
                // In some cases (Windows 7, perhaps depending on codepage) it interprets the second parameter as bytes. 
                // Give it characters, pssimistically, but make sure that the number of characters we offer is twice what it claims it needs.
                //
                // In some cases (Windows 10 RS2), the returned length includes null, in others (Windows 7) it does not. 
                // Pessimistically assume it does not.
                int len = GetConsoleTitle(sb, sb.Capacity);

                if (len <= 0)
                {
                    lastError = Marshal.GetLastWin32Error();

                    if (len < 0 || (lastError != Errors.ERROR_INSUFFICIENT_BUFFER && lastError != Errors.ERROR_SUCCESS))
                    {
                        title = string.Empty;
                        titleLength = title.Length;
                        return lastError;
                    }
                }
                else if (sb.Capacity >= (len + 1) * sizeof(char))
                {
                    // Success
                    title = sb.ToString();
                    titleLength = title.Length;
                    return 0;
                }

                // We need to increase the sb capacity and retry.
                if (sb.Capacity >= MaxAllowedBufferSizeInChars * sizeof(char))
                {
                    // No more room to grow.
                    // Title is greater than the allowed buffer so we do not read the title and only pass the length to the caller.
                    title = string.Empty;
                    titleLength = len;
                    return 0;
                }

                if (len > 0)
                {
                    // Add one for the null terminator in case length doesn't include it
                    // Double the length requested, in case it will interpret sb.Capacity as bytes
                    sb.Capacity = Math.Min((len + 1) * sizeof(char), MaxAllowedBufferSizeInChars * sizeof(char));
                }
                else
                {
                    // Don't know what length is needed: just double
                    sb.Capacity = Math.Min(sb.Capacity * 2, MaxAllowedBufferSizeInChars * sizeof(char));
                }
            }
        }
    }
}
