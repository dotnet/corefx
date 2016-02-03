// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe class QUERY_SERVICE_CONFIG
        {
            internal int dwServiceType;
            internal int dwStartType;
            internal int dwErrorControl;
            internal char* lpBinaryPathName;
            internal char* lpLoadOrderGroup;
            internal int dwTagId;
            internal char* lpDependencies;
            internal char* lpServiceStartName;
            internal char* lpDisplayName;
        }
    }
}
