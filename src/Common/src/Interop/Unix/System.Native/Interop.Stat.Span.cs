// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Sys
    {
        // Unix max paths are typically 1K or 4K, 256 should handle the majority of paths
        // without putting too much pressure on the stack.
        private const int StackBufferSize = 256;

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Stat2", SetLastError = true)]
        internal unsafe static extern int Stat_Byte(ref byte path, out FileStatus output);

        internal unsafe static int Stat_Span(ReadOnlySpan<char> path, out FileStatus output)
        {
            byte* buffer = stackalloc byte[StackBufferSize];
            var converter = new ValueUtf8Converter(new Span<byte>(buffer, StackBufferSize));
            int result = Stat_Byte(ref MemoryMarshal.GetReference(converter.ConvertString(path)), out output);
            converter.Clear();
            return result;
        }

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_LStat2", SetLastError = true)]
        internal static extern int LStat_Byte(ref byte path, out FileStatus output);

        internal unsafe static int LStat_Span(ReadOnlySpan<char> path, out FileStatus output)
        {
            byte* buffer = stackalloc byte[StackBufferSize];
            var converter = new ValueUtf8Converter(new Span<byte>(buffer, StackBufferSize));
            int result = LStat_Byte(ref MemoryMarshal.GetReference(converter.ConvertString(path)), out output);
            converter.Clear();
            return result;
        }
    }
}
