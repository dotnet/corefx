// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System 
{
    internal static partial class PinnableReferenceHelpers
    {
        [StructLayout(LayoutKind.Sequential)]
        private sealed class PlainObjectLayout
        {
            public byte Data;
        }

        // Note: Does not work for Mono as Mono has an extra Bounds pointer.
        [StructLayout(LayoutKind.Sequential)]
        private sealed class ArrayLayout
        {
            private IntPtr _lengthPlusPadding;
            public byte Data;
        }

        [StructLayout(LayoutKind.Sequential)]
        private sealed class StringLayout
        {
            private int _length;
            public byte Data;
        }
    }
}

