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
        public static FileStream Create(this FileInfo fileInfo, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            if (fileSecurity == null)
                throw new ArgumentNullException(nameof(fileSecurity));

            return null;
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
