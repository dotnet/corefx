// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    /// <summary>
    /// Reads bytes as primitives with specific endianness
    /// </summary>
    /// <remarks>
    /// For native formats, MemoryExtensions.Read{T}; should be used.
    /// Use these helpers when you need to read specific endinanness.
    /// </remarks>
    public static partial class BinaryPrimitives
    {
        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReverseEndianness(ushort value) => BitConverter.ByteSwap(value);

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseEndianness(uint value) => BitConverter.ByteSwap(value);

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseEndianness(ulong value) => BitConverter.ByteSwap(value);
    }
}
