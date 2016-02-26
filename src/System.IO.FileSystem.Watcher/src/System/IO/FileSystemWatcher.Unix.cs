// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    public partial class FileSystemWatcher
    {
        protected static void UnlockRootDirectory(Microsoft.Win32.SafeHandles.SafeFileHandle handle)
        {
            Interop.Sys.FLock(handle, Interop.Sys.LockOperations.LOCK_UN);
        }

        protected static void LockRootDirectory(Microsoft.Win32.SafeHandles.SafeFileHandle sfh, string directory)
        {
            // Match Windows and lock the root directory
            // NOTE: this WILL NOT prevent this directory from being deleted from underneath us
            //       due to *nix having advisory-only locks (by default); this means that applications
            //       can choose to ignore locks and operate on the FD anyway.
            if (Interop.Sys.FLock(sfh,
                                  Interop.Sys.LockOperations.LOCK_EX | Interop.Sys.LockOperations.LOCK_NB) < 0)
            {
                // Match Windows and throw FileNotFoundException
                throw new FileNotFoundException(SR.Format(SR.FSW_IOError, directory));
            }
        }
    }
}
