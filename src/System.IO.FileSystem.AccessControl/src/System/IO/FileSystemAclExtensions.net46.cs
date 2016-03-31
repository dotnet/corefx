// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.AccessControl;

namespace System.IO
{
    public static class FileSystemAclExtensions
    {
        public static DirectorySecurity GetAccessControl(DirectoryInfo directoryInfo)
        {
            return directoryInfo.GetAccessControl();
        }

        public static DirectorySecurity GetAccessControl(DirectoryInfo directoryInfo, AccessControlSections includeSections)
        {
            return directoryInfo.GetAccessControl(includeSections);
        }

        public static void SetAccessControl(DirectoryInfo directoryInfo, DirectorySecurity directorySecurity)
        {
            directoryInfo.SetAccessControl(directorySecurity);
        }

        public static FileSecurity GetAccessControl(this FileInfo fileInfo)
        {
            return fileInfo.GetAccessControl();
        }

        public static FileSecurity GetAccessControl(FileInfo fileInfo, AccessControlSections includeSections)
        {
            return fileInfo.GetAccessControl(includeSections);
        }

        public static void SetAccessControl(FileInfo fileInfo, FileSecurity fileSecurity)
        {
            fileInfo.SetAccessControl(fileSecurity);
        }

        public static FileSecurity GetAccessControl(FileStream fileStream)
        {
            return fileStream.GetAccessControl();
        }

        public static void SetAccessControl(FileStream fileStream, FileSecurity fileSecurity)
        {
            fileStream.SetAccessControl(fileSecurity);
        }
    }
}
