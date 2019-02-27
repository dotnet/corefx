// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Internal.Runtime.CompilerServices;

namespace System
{
    public static class Utf8StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> AsBytes(this Utf8String text)
        {
            if (text is null)
            {
                return default;
            }

            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in text.GetPinnableReference()), text.Length);
        }
    }
}
