// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        internal struct CREATEFILE2_EXTENDED_PARAMETERS
        {
            internal uint dwSize;
            internal uint dwFileAttributes;
            internal uint dwFileFlags;
            internal uint dwSecurityQosFlags;
            internal IntPtr lpSecurityAttributes;
            internal IntPtr hTemplateFile;
        }
    }
}
