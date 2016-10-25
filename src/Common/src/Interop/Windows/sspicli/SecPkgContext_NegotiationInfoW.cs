// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Net
{
    // sspi.h
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecPkgContext_NegotiationInfoW
    {
        internal IntPtr PackageInfo;
        internal uint NegotiationState;
        internal static readonly int Size = Marshal.SizeOf<SecPkgContext_NegotiationInfoW>();
        internal static readonly int NegotiationStateOffest = (int)Marshal.OffsetOf<SecPkgContext_NegotiationInfoW>("NegotiationState");
    }
}
