﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    public static partial class Base64
    {
        private static TVector ReadVector<TVector>(ReadOnlySpan<sbyte> data)
        {
            ref sbyte tmp = ref MemoryMarshal.GetReference(data);
            return Unsafe.As<sbyte, TVector>(ref tmp);
        }

        [Conditional("DEBUG")]
        private static unsafe void AssertRead<TVector>(byte* src, byte* srcStart, int srcLength)
        {
            int vectorElements = Unsafe.SizeOf<TVector>();
            byte* readEnd = src + vectorElements;
            byte* srcEnd = srcStart + srcLength;

            if (readEnd > srcEnd)
            {
                int srcIndex = (int)(src - srcStart);
                Debug.Fail($"Read for {typeof(TVector)} is not within safe bounds. srcIndex: {srcIndex}, srcLength: {srcLength}");
            }
        }

        [Conditional("DEBUG")]
        private static unsafe void AssertWrite<TVector>(byte* dest, byte* destStart, int destLength)
        {
            int vectorElements = Unsafe.SizeOf<TVector>();
            byte* writeEnd = dest + vectorElements;
            byte* destEnd = destStart + destLength;

            if (writeEnd > destEnd)
            {
                int destIndex = (int)(dest - destStart);
                Debug.Fail($"Write for {typeof(TVector)} is not within safe bounds. destIndex: {destIndex}, destLength: {destLength}");
            }
        }
    }
}
