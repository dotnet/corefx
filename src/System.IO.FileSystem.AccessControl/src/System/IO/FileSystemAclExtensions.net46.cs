// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.AccessControl;

namespace System.IO
{
    public static class FileSystemAclExtensions
    {
        /// <summary>Creates a new directory, ensuring it is created with the specified directory security. If the directory already exists, nothing is done.</summary>
        /// <param name="directoryInfo">The object describing a directory that does not exist in disk yet.</param>
        /// <param name="directorySecurity">An object that determines the access control and audit security for the directory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directoryInfo" /> or <paramref name="directorySecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="DirectoryNotFoundException">Could not find a part of the path.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the path is denied.</exception>
        public static void Create(this DirectoryInfo directoryInfo, DirectorySecurity directorySecurity)
        {
            if (directoryInfo == null)
                throw new ArgumentNullException(nameof(directoryInfo));

            if (directorySecurity == null)
                throw new ArgumentNullException(nameof(directorySecurity));

            directoryInfo.Create(directorySecurity);
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
