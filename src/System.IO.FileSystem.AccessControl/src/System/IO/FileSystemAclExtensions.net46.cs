// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.AccessControl;

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

            return new FileStream(fileInfo.FullName, mode, rights, share, bufferSize, options, fileSecurity);
        }

        public static DirectorySecurity GetAccessControl(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.GetAccessControl();
        }

        public static DirectorySecurity GetAccessControl(this DirectoryInfo directoryInfo, AccessControlSections includeSections)
        {
            return directoryInfo.GetAccessControl(includeSections);
        }

        public static void SetAccessControl(this DirectoryInfo directoryInfo, DirectorySecurity directorySecurity)
        {
            directoryInfo.SetAccessControl(directorySecurity);
        }

        public static FileSecurity GetAccessControl(this FileInfo fileInfo)
        {
            return fileInfo.GetAccessControl();
        }

        public static FileSecurity GetAccessControl(this FileInfo fileInfo, AccessControlSections includeSections)
        {
            return fileInfo.GetAccessControl(includeSections);
        }

        public static void SetAccessControl(this FileInfo fileInfo, FileSecurity fileSecurity)
        {
            fileInfo.SetAccessControl(fileSecurity);
        }

        public static FileSecurity GetAccessControl(this FileStream fileStream)
        {
            return fileStream.GetAccessControl();
        }

        public static void SetAccessControl(this FileStream fileStream, FileSecurity fileSecurity)
        {
            fileStream.SetAccessControl(fileSecurity);
        }
    }
}
