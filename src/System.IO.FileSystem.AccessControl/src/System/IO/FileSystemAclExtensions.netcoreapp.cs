// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
    public static partial class FileSystemAclExtensions
    {
        #region Private members

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = 0x40000000;

        #endregion

        #region Public methods

        /// <summary>Creates a new directory, ensuring it is created with the specified directory security. If the directory already exists, nothing is done.</summary>
        /// <param name="directoryInfo">The object describing a directory that does not exist in disk yet.</param>
        /// <param name="directorySecurity">An object that determines the access control and audit security for the directory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directoryInfo" /> or <paramref name="directorySecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="DirectoryNotFoundException">Could not find a part of the path.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the path is denied.</exception>
        /// <remarks><format type="text/markdown"><![CDATA[
        ///## Remarks
        ///This extension method was added to .NET Core to bring the functionality that was provided by the <xref:System.IO.DirectoryInfo.Create(System.Security.AccessControl.DirectorySecurity)> .NET Framework method.
        /// ]]></format></remarks>
        public static void Create(this DirectoryInfo directoryInfo, DirectorySecurity directorySecurity)
        {
            if (directoryInfo == null)
                throw new ArgumentNullException(nameof(directoryInfo));

            if (directorySecurity == null)
                throw new ArgumentNullException(nameof(directorySecurity));

            FileSystem.CreateDirectory(directoryInfo.FullName, directorySecurity.GetSecurityDescriptorBinaryForm());
        }

        /// <summary>
        /// Creates a new file stream, ensuring it is created with the specified properties and security settings.
        /// </summary>
        /// <param name="fileInfo">The current instance describing a file that does not exist in disk yet.</param>
        /// <param name="mode">One of the enumeration values that specifies how the operating system should open a file.</param>
        /// <param name="rights">One of the enumeration values that defines the access rights to use when creating access and audit rules.</param>
        /// <param name="share">One of the enumeration values for controlling the kind of access other FileStream objects can have to the same file.</param>
        /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
        /// <param name="options">One of the enumeration values that describes how to create or overwrite the file.</param>
        /// <param name="fileSecurity">An object that determines the access control and audit security for the file.</param>
        /// <returns>A file stream for the newly created file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileInfo" /> or <paramref name="fileSecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode" /> or <paramref name="share" /> are out of their legal enum range.
        ///-or-
        /// <paramref name="options" /> is not a supported enum value.
        ///-or-
        /// <paramref name="bufferSize" /> is not a positive number.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the path is denied.</exception>
        /// <exception cref="ArgumentException">Invalid handle.</exception>
        /// <exception cref="ObjectDisposedException">Cannot access a closed file.</exception>
        /// <remarks><format type="text/markdown"><![CDATA[
        ///## Remarks
        ///This extension method was added to .NET Core to bring the functionality that was provided by the <xref:System.IO.FileStream.#ctor(System.String,System.IO.FileMode,System.Security.AccessControl.FileSystemRights,System.IO.FileShare,System.Int32,System.IO.FileOptions,System.Security.AccessControl.FileSecurity)> .NET Framework constructor.
        /// ]]></format></remarks>
        public static FileStream Create(this FileInfo fileInfo, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (fileSecurity == null)
            {
                throw new ArgumentNullException(nameof(fileSecurity));
            }

            // don't include inheritable in our bounds check for share
            FileShare tempshare = share & ~FileShare.Inheritable;

            string badArg = null;
            if (mode < FileMode.CreateNew || mode > FileMode.Append)
            {
                badArg = nameof(mode);
            }
            else if (tempshare < FileShare.None || tempshare > (FileShare.ReadWrite | FileShare.Delete))
            {
                badArg = nameof(share);
            }
            if (badArg != null)
            {
                throw new ArgumentOutOfRangeException(badArg, SR.ArgumentOutOfRange_Enum);
            }

            // We need to check only the supported attributes
            // NOTE: any change to FileOptions enum needs to be matched here in the error validation
            if (options != FileOptions.None &&
                (options & ~(FileOptions.WriteThrough | FileOptions.Asynchronous | FileOptions.RandomAccess | FileOptions.DeleteOnClose | FileOptions.SequentialScan | FileOptions.Encrypted | (FileOptions)0x20000000 /* NoBuffering */)) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options), SR.ArgumentOutOfRange_Enum);
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), SR.ArgumentOutOfRange_NeedPosNum);
            }

            SafeFileHandle handle = null;
            try
            {
                handle = CreateFileOpenHandle(fileInfo.FullName, mode, rights, share, options, fileSecurity);
                return new FileStream(handle, GetFileStreamFileAccess(rights), bufferSize, (options & FileOptions.Asynchronous) != 0);
            }
            catch
            {
                // If anything goes wrong while setting up the stream, make sure we deterministically dispose of the opened handle.
                if (handle != null)
                {
                    handle.Dispose();
                    handle = null;
                }

                throw;
            }
        }

        #endregion

        #region Private methods

        // In the context of a FileStream, the only ACCESS_MASK ACE rights we care about are reading/writing data and the generic read/write rights.
        // See: https://docs.microsoft.com/en-us/windows/win32/secauthz/access-mask
        private static FileAccess GetFileStreamFileAccess(FileSystemRights rights)
        {
            FileAccess access = 0;
            if ((rights & FileSystemRights.ReadData) != 0 || ((int)rights & GENERIC_READ) != 0)
            {
                access = FileAccess.Read;
            }
            if ((rights & FileSystemRights.WriteData) != 0 || ((int)rights & GENERIC_WRITE) != 0)
            {
                access = access == FileAccess.Read ? FileAccess.ReadWrite : FileAccess.Write;
            }
            return access;
        }

        private static unsafe SafeFileHandle CreateFileOpenHandle(string fullPath, FileMode mode, FileSystemRights rights, FileShare share, FileOptions options, FileSecurity security)
        {
            Debug.Assert(fullPath != null);

            // Must use a valid Win32 constant
            mode = (mode == FileMode.Append) ? FileMode.OpenOrCreate : mode;

            // For mitigating local elevation of privilege attack through named pipes
            // make sure we always call CreateFile with SECURITY_ANONYMOUS so that the
            // named pipe server can't impersonate a high privileged client security context
            // (note that this is the effective default on CreateFile2)
            int flagsAndAttributes = (int)options | Interop.Kernel32.SecurityOptions.SECURITY_SQOS_PRESENT | Interop.Kernel32.SecurityOptions.SECURITY_ANONYMOUS;

            SafeFileHandle handle;

            fixed (byte* pSecurityDescriptor = security.GetSecurityDescriptorBinaryForm())
            {
                Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = secAttrs = new Interop.Kernel32.SECURITY_ATTRIBUTES
                {
                    nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES),
                    bInheritHandle = ((share & FileShare.Inheritable) != 0) ? Interop.BOOL.TRUE : Interop.BOOL.FALSE,
                    lpSecurityDescriptor = (IntPtr)pSecurityDescriptor
                };

                using (DisableMediaInsertionPrompt.Create())
                {
                    handle = Interop.Kernel32.CreateFile(fullPath, (int)rights, share, ref secAttrs, mode, flagsAndAttributes, IntPtr.Zero);
                }
            }

            ValidateFileHandle(handle, fullPath);

            return handle;
        }

        private static void ValidateFileHandle(SafeFileHandle handle, string fullPath)
        {
            if (handle.IsInvalid)
            {
                // Return a meaningful exception with the full path.

                // NT5 oddity - when trying to open "C:\" as a Win32FileStream,
                // we usually get ERROR_PATH_NOT_FOUND from the OS.  We should
                // probably be consistent w/ every other directory.
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode == Interop.Errors.ERROR_PATH_NOT_FOUND && fullPath.Length == PathInternal.GetRootLength(fullPath))
                {
                    errorCode = Interop.Errors.ERROR_ACCESS_DENIED;
                }

                throw Win32Marshal.GetExceptionForWin32Error(errorCode, fullPath);
            }
        }

        #endregion
    }
}
