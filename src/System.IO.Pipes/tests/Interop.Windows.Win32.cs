// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes.Tests
{
    /// <summary>
    /// The class contains interop declarations and helpers methods for them.
    /// </summary>
    internal static partial class Interop
    {

        private static bool TryHandleGetImpersonationUserNameError(SafePipeHandle handle, int error, StringBuilder builder, out string userName)
        {
            if ((error == ERROR_SUCCESS || error == ERROR_CANNOT_IMPERSONATE) && Environment.Is64BitProcess)
            {
                LoadLibraryEx("sspicli.dll", IntPtr.Zero, LOAD_LIBRARY_SEARCH_SYSTEM32).SetHandleAsInvalid();

                if (GetNamedPipeHandleStateW(handle, IntPtr.Zero, out _, IntPtr.Zero, IntPtr.Zero, builder, builder.Capacity))
                {
                    userName = builder.ToString();
                    return true;
                }
            }

            userName = string.Empty;
            return false;
        }
    }
}
