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
            if ((error == Interop.Errors.ERROR_SUCCESS || error == Interop.Errors.ERROR_CANNOT_IMPERSONATE) && Environment.Is64BitProcess)
            {
                // There is a known problem in Windows where if sspicli is not loaded, this function fails unexpectedly with ERROR_SUCCESS, so we need to load it and reattempt
                // If sspicli is already loaded, this is a no-op. 
                // We set the handle as invalid so the library won't get unloaded.
                // ERROR_CANNOT_IMPERSONATE is thrown in Windows 7.
                Interop.Kernel32.LoadLibraryEx("sspicli.dll", IntPtr.Zero, Interop.Kernel32.LOAD_LIBRARY_SEARCH_SYSTEM32).SetHandleAsInvalid();

                if (Interop.Kernel32.GetNamedPipeHandleStateW(InternalHandle, null, null, null, null, userName, UserNameMaxLength))
                {
                    return new string(userName);
                }
                error = Marshal.GetLastWin32Error();
            }
            throw WinIOError(error);
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------
    }
}
