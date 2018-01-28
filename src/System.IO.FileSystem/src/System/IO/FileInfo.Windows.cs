// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.IO
{
    partial class FileInfo
    {
        internal static unsafe FileInfo Create<TState>(string fullPath, string fileName, ref RawFindData<TState> findData)
        {
            Debug.Assert(fileName.Equals(Path.GetFileName(fullPath)));
            FileInfo info = new FileInfo(fullPath, fileName: fileName, isNormalized: true);
            info.Init(findData._info);
            return info;
        }
    }
}
