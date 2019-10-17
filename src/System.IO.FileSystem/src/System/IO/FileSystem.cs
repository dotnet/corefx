// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

#if MS_IO_REDIST
namespace Microsoft.IO
#else
namespace System.IO
#endif
{
    internal static partial class FileSystem
    {
        internal static string GetDirectoryRoot(string path)
        {
            if (path == null) return null;
            return path.Substring(0, PathInternal.GetRootLength(path.AsSpan()));
        }
    }
}
