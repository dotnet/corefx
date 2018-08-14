// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
    internal static partial class FileSystem
    {
        public static void Encrypt(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string fullPath = Path.GetFullPath(path);

            if (!Interop.Kernel32.EncryptFile(fullPath))
            {
                ThrowException(fullPath);
            }
        }

        public static void Decrypt(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string fullPath = Path.GetFullPath(path);

            if (!Interop.Kernel32.DecryptFile(fullPath))
            {
                ThrowException(fullPath);
            }
        }

        private static void ThrowException(string fullPath)
        {
            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode == Interop.Errors.ERROR_ACCESS_DENIED)
            {
                // Check to see if the file system support the Encrypted File System (EFS)
                string name = DriveInfoInternal.NormalizeDriveName(Path.GetPathRoot(fullPath));
                StringBuilder volumeName = new StringBuilder(50);
                StringBuilder fileSystemName = new StringBuilder(50);

                bool success = Interop.Kernel32.SetThreadErrorMode(Interop.Kernel32.SEM_FAILCRITICALERRORS, out uint oldMode);
                try
                {
                    if (!Interop.Kernel32.GetVolumeInformation(name, volumeName, volumeName.Length, out int serialNumber, out int maxFileNameLen, out int fileSystemFlags, fileSystemName, fileSystemName.Length))
                    {
                        errorCode = Marshal.GetLastWin32Error();
                        throw Win32Marshal.GetExceptionForWin32Error(errorCode, name);
                    }

                    if ((fileSystemFlags & Interop.Kernel32.FILE_SUPPORTS_ENCRYPTION) == 0)
                    {
                        throw new NotSupportedException(SR.PlatformNotSupported_FileEncryption);
                    }
                }
                finally
                {
                    if (success)
                        Interop.Kernel32.SetThreadErrorMode(oldMode, out oldMode);
                }
            }
            throw Win32Marshal.GetExceptionForWin32Error(errorCode, fullPath);
        }
    }
}
