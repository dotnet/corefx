// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
#pragma warning disable CS0649 // never assigned to warning
        internal struct SECURITY_ATTRIBUTES
        {
            internal uint nLength;
            internal IntPtr lpSecurityDescriptor;
            internal BOOL bInheritHandle;
        }
#pragma warning restore CS0649
    }
}
