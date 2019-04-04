// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.IO.Compression;
using Microsoft.Win32.SafeHandles;
using size_t = System.IntPtr;

internal static partial class Interop
{
    internal static partial class Brotli
    {
        [DllImport(Libraries.CompressionNative)]
        internal static extern SafeBrotliEncoderHandle BrotliEncoderCreateInstance(IntPtr allocFunc, IntPtr freeFunc, IntPtr opaque);

        [DllImport(Libraries.CompressionNative)]
        internal static extern bool BrotliEncoderSetParameter(SafeBrotliEncoderHandle state, BrotliEncoderParameter parameter, uint value);

        [DllImport(Libraries.CompressionNative)]
        internal static extern unsafe bool BrotliEncoderCompressStream(
            SafeBrotliEncoderHandle state, BrotliEncoderOperation op, ref size_t availableIn,
            byte** nextIn, ref size_t availableOut, byte** nextOut, out size_t totalOut);

        [DllImport(Libraries.CompressionNative)]
        internal static extern bool BrotliEncoderHasMoreOutput(SafeBrotliEncoderHandle state);

        [DllImport(Libraries.CompressionNative)]
        internal static extern void BrotliEncoderDestroyInstance(IntPtr state);

        [DllImport(Libraries.CompressionNative)]
        internal static extern unsafe bool BrotliEncoderCompress(int quality, int window, int v, size_t availableInput, byte* inBytes, ref size_t availableOutput, byte* outBytes);
    }
}

