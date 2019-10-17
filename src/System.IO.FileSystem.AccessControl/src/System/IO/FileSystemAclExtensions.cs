// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Security.AccessControl;
using System;

namespace System.IO
{
    public static class FileSystemAclExtensions
    {
        /// <summary>Creates a new file and returns a <see cref="FileStream" /> instance representing it, ensuring the file is created with all the specified permissions and security properties.</summary>
        /// <param name="fileInfo">The object describing a file that does not exist in disk yet.</param>
        /// <param name="mode">A constant that determines how to open or create the file.</param>
        /// <param name="rights">A constant that determines the access rights to use when creating access and audit rules.</param>
        /// <param name="share">A constant that determines how the file will be shared by processes.</param>
        /// <param name="bufferSize">A positive <see cref="int" /> value greater than 0 indicating the buffer size. The default buffer size is 4096.</param>
        /// <param name="options">A constant that specifies additional file options.</param>
        /// <param name="fileSecurity">An object that determines the access control and audit security for the file.</param>
        /// <returns>A file stream that represents the newly created file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileSecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is negative or zero.
        ///
        /// -or-
        ///
        /// <paramref name="mode" />, <paramref name="rights" />, <paramref name="share" /> or <paramref name="options" /> contain an invalid value.
        /// </exception>
        /// <remarks><format type="text/markdown"><![CDATA[
        /// ## Remarks
        ///
        /// This method can also throw any of the exceptions that <xref:System.IO.FileStream> can throw.
        /// ]]></format></remarks>
        public static FileStream Create(this FileInfo fileInfo, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            string badArg = null;

            if (mode < FileMode.CreateNew || mode > FileMode.Append)
                badArg = nameof(mode);

            if (rights < FileSystemRights.ReadData || rights > FileSystemRights.FullControl)
                badArg = nameof(rights);

            if (share < FileShare.None || share > FileShare.Inheritable)
                badArg = nameof(share);

            if (options < FileOptions.None || options > FileOptions.Asynchronous)
                badArg = nameof(options);

            if (badArg != null)
                throw new ArgumentOutOfRangeException(badArg, SR.ArgumentOutOfRange_Enum);

            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            if (fileSecurity == null)
                throw new ArgumentNullException(nameof(fileSecurity));

            fileInfo.SetAccessControl(fileSecurity);
            return new FileStream(fileInfo.FullName, mode, mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite, share, bufferSize, options);
        }

        /// <summary>Creates a new directory, ensuring it is created with the specified directory security.</summary>
        /// <param name="directoryInfo">The object describing a directory that does not exist in disk yet.</param>
        /// <param name="directorySecurity">An object that determines the access control and audit security for the directory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directorySecurity" /> is <see langword="null" />.</exception>
        public static void Create(this DirectoryInfo directoryInfo, DirectorySecurity directorySecurity)
        {
            if (directorySecurity == null)
                throw new ArgumentNullException(nameof(directorySecurity));

            // FileSystem is an internal class in another assembly. How do I consume it here?
            FileSystem.CreateDirectory(directoryInfo.FullName, directorySecurity.GetSecurityDescriptorBinaryForm());
        }

        public static DirectorySecurity GetAccessControl(this DirectoryInfo directoryInfo)
        {
            return new DirectorySecurity(directoryInfo.FullName, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
        }

        public static DirectorySecurity GetAccessControl(this DirectoryInfo directoryInfo, AccessControlSections includeSections)
        {
            return new DirectorySecurity(directoryInfo.FullName, includeSections);
        }

        public static void SetAccessControl(this DirectoryInfo directoryInfo, DirectorySecurity directorySecurity)
        {
            if (directorySecurity == null)
                throw new ArgumentNullException(nameof(directorySecurity));

            string fullPath = Path.GetFullPath(directoryInfo.FullName);
            directorySecurity.Persist(fullPath);
        }

        public static FileSecurity GetAccessControl(this FileInfo fileInfo)
        {
            return GetAccessControl(fileInfo, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
        }

        public static FileSecurity GetAccessControl(this FileInfo fileInfo, AccessControlSections includeSections)
        {
            return new FileSecurity(fileInfo.FullName, includeSections);
        }

        public static void SetAccessControl(this FileInfo fileInfo, FileSecurity fileSecurity)
        {
            if (fileSecurity == null)
                throw new ArgumentNullException(nameof(fileSecurity));

            string fullPath = Path.GetFullPath(fileInfo.FullName);
            // Appropriate security check should be done for us by FileSecurity.
            fileSecurity.Persist(fullPath);
        }

        public static FileSecurity GetAccessControl(this FileStream fileStream)
        {
            SafeFileHandle handle = fileStream.SafeFileHandle;
            if (handle.IsClosed)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_FileClosed);
            }
            return new FileSecurity(handle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
        }

        public static void SetAccessControl(this FileStream fileStream, FileSecurity fileSecurity)
        {
            SafeFileHandle handle = fileStream.SafeFileHandle;

            if (fileSecurity == null)
                throw new ArgumentNullException(nameof(fileSecurity));

            if (handle.IsClosed)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_FileClosed);
            }

            fileSecurity.Persist(handle, fileStream.Name);
        }
    }
}
