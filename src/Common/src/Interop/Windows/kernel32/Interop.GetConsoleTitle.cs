// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern uint GetConsoleTitleW(ref char title, uint nSize);

        internal static string GetConsoleTitle(out int error)
        {
            error = Errors.ERROR_SUCCESS;

            Span<char> initialBuffer = stackalloc char[256];
            ValueStringBuilder builder = new ValueStringBuilder(initialBuffer);

            do
            {
                uint result = GetConsoleTitleW(ref builder.GetPinnableReference(), (uint)builder.Capacity);

                // The documentation asserts that the console's title is stored in a shared 64KB buffer.
                // The magic number that used to exist here (24500), is likely related to that.
                // A full UNICODE_STRING is 32K chars...
                Debug.Assert(result <= short.MaxValue, "shouldn't be possible to grow beyond UNICODE_STRING size");

                if (result == 0)
                {
                    error = Marshal.GetLastWin32Error();
                    if (error == Errors.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // Typically this API truncates, but there was a bug in RS2 so we'll make an attempt to handle
                        builder.EnsureCapacity(builder.Capacity * 2);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else if (result >= builder.Capacity - 1)
                {
                    // Our buffer was full. As this API truncates we need to increase our size and reattempt.
                    builder.EnsureCapacity(builder.Capacity * 2);
                }
                else
                {
                    builder.Length = (int)result;
                    return builder.ToString();
                }
            } while (true);
        }
    }
}
