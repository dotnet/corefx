// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    public static partial class BinaryPrimitives
    {
        /// <summary>
        /// Writes a structure of type T into a span of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMachineEndian<T>(ref Span<byte> buffer, T value)
            where T : struct
        {
            if (BinaryHelpers.IsReferenceOrContainsReferences<T>())
            {
                Environment.FailFast($"Cannot write the non-blittable type <{typeof(T).Name}> into the span.");
            }
            if ((uint)Unsafe.SizeOf<T>() > (uint)buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            Unsafe.WriteUnaligned<T>(ref buffer.DangerousGetPinnableReference(), value);
        }

        /// <summary>
        /// Writes a structure of type T into a span of bytes.
        /// <returns>If the span is too small to contain the type T, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteMachineEndian<T>(ref Span<byte> buffer, T value)
            where T : struct
        {
            if (BinaryHelpers.IsReferenceOrContainsReferences<T>())
            {
                Environment.FailFast($"Cannot write the non-blittable type <{typeof(T).Name}> into the span.");
            }
            if (Unsafe.SizeOf<T>() > (uint)buffer.Length)
            {
                return false;
            }
            Unsafe.WriteUnaligned<T>(ref buffer.DangerousGetPinnableReference(), value);
            return true;
        }
    }
}

