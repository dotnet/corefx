// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers
{
    /// <summary>
    /// Extension methods for <see cref="ReadOnlyBuffer{T}"/>
    /// </summary>
    public static class BuffersExtensions
    {
        /// <summary>
        /// Returns position of first occurrence of item in the <see cref="ReadOnlyBuffer{T}"/>
        /// </summary>
        public static SequencePosition? PositionOf<T>(this ReadOnlyBuffer<T> buffer, T value) where T : IEquatable<T>
        {
            SequencePosition position = buffer.Start;
            SequencePosition result = position;
            while (buffer.TryGet(ref position, out var memory))
            {
                var index = memory.Span.IndexOf(value);
                if (index != -1)
                {
                    return buffer.GetPosition(result, index);
                }
                result = position;
            }
            return null;
        }


        /// <summary>
        /// Copy the <see cref="ReadOnlyBuffer{T}"/> to the specified <see cref="Span{Byte}"/>.
        /// </summary>
        /// <param name="buffer">The source <see cref="ReadOnlyBuffer{T}"/>.</param>
        /// <param name="destination">The destination <see cref="Span{Byte}"/>.</param>
        public static void CopyTo<T>(this ReadOnlyBuffer<T> buffer, Span<T> destination)
        {
            if (buffer.Length > destination.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination);

            foreach (var segment in buffer)
            {
                segment.Span.CopyTo(destination);
                destination = destination.Slice(segment.Length);
            }
        }

        /// <summary>
        /// Converts the <see cref="ReadOnlyBuffer{T}"/> to an array
        /// </summary>
        public static T[] ToArray<T>(this ReadOnlyBuffer<T> buffer)
        {
            var array = new T[buffer.Length];
            buffer.CopyTo(array);
            return array;
        }
    }
}
