// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System
{
    //
    // This is a "magic constant" that defines how much offset to add to Pinnable.Data to get to the first element of an array.
    // This is necessarily non-portable, but it works for the CLR and CoreClr. It does not work for Mono (Mono has an extra Bounds
    // pointer) but Mono wants to do Span intrinsically.
    //
    [StructLayout(LayoutKind.Sequential)]
    internal struct ArrayHeader
    {
        private IntPtr _lengthPlusPadding;
    }
}
