// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

namespace System.IO.FileSystem.Tests
{
    public class Directory_SetLastAccessTime_str_dt : SetTime_str_dt
    {
        protected override void SetTime(string path, DateTime lastWriteTime)
        {
            Directory.SetLastAccessTime(path, lastWriteTime);
        }

        protected override DateTime GetTime(string path)
        {
            return Directory.GetLastAccessTime(path);
        }
    }
}