// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    partial class DirectoryInfo
    {
        internal static unsafe DirectoryInfo Create(string fullPath, string fileName, FileStatus fileStatus)
        {
            DirectoryInfo info = new DirectoryInfo(fullPath, fileName: fileName, isNormalized: true);
            info.Init(fileStatus);
            return info;
        }
    }
}
