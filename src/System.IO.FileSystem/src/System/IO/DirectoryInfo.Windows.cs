// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.IO
{
    partial class DirectoryInfo
    {
        internal static unsafe DirectoryInfo Create<TState>(string fullPath, string fileName, ref RawFindData<TState> findData)
        {
            Debug.Assert(fileName.Equals(Path.GetFileName(fullPath)));
            DirectoryInfo info = new DirectoryInfo(fullPath, fileName: fileName, isNormalized: true);
            info.Init(findData._info);
            return info;
        }
    }
}
