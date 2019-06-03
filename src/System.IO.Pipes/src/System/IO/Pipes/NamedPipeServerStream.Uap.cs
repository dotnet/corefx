// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.IO.Pipes
{
    /// <summary>
    /// Named pipe server
    /// </summary>
    public sealed partial class NamedPipeServerStream : PipeStream
    {
        // Gets the username of the connected client.  Note that we will not have access to the client's 
        // username until it has written at least once to the pipe (and has set its impersonationLevel 
        // argument appropriately). 
        public unsafe string GetImpersonationUserName()
        {
            CheckWriteOperations();

            const uint UserNameMaxLength = Interop.Kernel32.CREDUI_MAX_USERNAME_LENGTH + 1;
            char* userName = stackalloc char[(int)UserNameMaxLength]; // ~1K

            if (Interop.Kernel32.GetNamedPipeHandleStateW(InternalHandle, null, null, null, null, userName, UserNameMaxLength))
            {
                return new string(userName);
            }

            int error = Marshal.GetLastWin32Error();
            throw WinIOError(error);
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------
    }
}
