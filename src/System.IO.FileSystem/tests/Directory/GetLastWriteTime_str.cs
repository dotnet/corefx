// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

namespace System.IO.FileSystem.Tests
{
    public class Directory_GetLastWriteTime_str : GetTime_str
    {
        protected override DateTime m_Get(string path)
        {
            return Directory.GetLastWriteTime(path);
        }
    }
}
