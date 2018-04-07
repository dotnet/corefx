﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers
{
    /// <summary>
    /// Extension methods for <see cref="ReadOnlySequence{T}"/>
    /// </summary>
    public static class BuffersExtensions
    {
        /// <summary>
        /// Returns position of first occurrence of item in the <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        public static SequencePosition? PositionOf<T>(in this ReadOnlySequence<T> source, T value) where T : IEquatable<T>
        {
            if (source.IsSingleSegment)
            {
                int index = source.First.Span.IndexOf(value);
                if (index != -1)
                {
                    return source.GetPosition(index);
                }

                return null;
            }
            else
            {
                return PositionOfMultiSegement(source, value);
            }
        }

        private static SequencePosition? PositionOfMultiSegement<T>(in ReadOnlySequence<T> source, T value) where T : IEquatable<T>
        {
            SequencePosition position = source.Start;
            while (source.TryGetTrusted(position, out ReadOnlyMemory<T> memory, out SequencePosition next))
            {
                int index = memory.Span.IndexOf(value);
                if (index != -1)
                {
                    return source.GetPosition(index, position);
                }
                position = next;
            }

            return null;
        }

        /// <summary>
        /// Copy the <see cref="ReadOnlySequence{T}"/> to the specified <see cref="Span{Byte}"/>.
        /// </summary>
        /// <param name="source">The source <see cref="ReadOnlySequence{T}"/>.</param>
        /// <param name="destination">The destination <see cref="Span{Byte}"/>.</param>
        public static void CopyTo<T>(in this ReadOnlySequence<T> source, Span<T> destination)
        {
            if (source.Length > destination.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination);

            if (source.IsSingleSegment)
            {
                source.First.Span.CopyTo(destination);
            }
            else
            {
                CopyToMultiSegement(source, destination);
            }
        }

        private static void CopyToMultiSegement<T>(in ReadOnlySequence<T> sequence, Span<T> destination)
        {
            SequencePosition position = sequence.Start;
            while (sequence.TryGetTrusted(position, out ReadOnlyMemory<T> memory, out SequencePosition next))
            {
                ReadOnlySpan<T> span = memory.Span;
                span.CopyTo(destination);
                position = next;
                if (position.GetObject() != null)
                {
                    destination = destination.Slice(span.Length);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Converts the <see cref="ReadOnlySequence{T}"/> to an array
        /// </summary>
        public static T[] ToArray<T>(in this ReadOnlySequence<T> sequence)
        {
            var array = new T[sequence.Length];
            sequence.CopyTo(array);
            return array;
        }

        /// <summary>
        /// Writes contents of <paramref name="value"/> to <paramref name="writer"/>
        /// </summary>
        public static void Write<T>(this IBufferWriter<T> writer, ReadOnlySpan<T> value)
        {
            Span<T> destination = writer.GetSpan();

            // Fast path, try copying to the available memory directly
            if (value.Length <= destination.Length)
            {
                value.CopyTo(destination);
                writer.Advance(value.Length);
            }
            else
            {
                WriteMultiSegment(writer, value, destination);
            }
        }

        private static void WriteMultiSegment<T>(IBufferWriter<T> writer, in ReadOnlySpan<T> source, Span<T> destination)
        {
            ReadOnlySpan<T> input = source;
            while (true)
            {
                int writeSize = Math.Min(destination.Length, input.Length);
                input.Slice(0, writeSize).CopyTo(destination);
                writer.Advance(writeSize);
                input = input.Slice(writeSize);
                if (input.Length > 0)
                {
                    destination = writer.GetSpan(input.Length);
                    continue;
                }

                return;
            }
        }
    }
}
