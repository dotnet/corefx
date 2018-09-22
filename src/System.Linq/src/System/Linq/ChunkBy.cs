using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        /// <summary>
        /// Chunks the elements of a sequence according to a amount and creates a result value from each group and its max capacity.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to chunk.</param>
        /// <param name="size">The max capacity for each chunk.</param>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> with chunks.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="size"/> is below 1.</exception>
        public static IEnumerable<IEnumerable<TSource>> ChunkBy<TSource>(this IEnumerable<TSource> source, int size)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (size < 1)
            {
                throw new ArgumentException($"Parameter: '{nameof(size)}' ({size}) can not be below 1.");
            }

            return CreateChunks(source, size);
        }

        private static IEnumerable<IEnumerable<TSource>> CreateChunks<TSource>(IEnumerable<TSource> source, int size)
        {
            var index = 0;
            var elements = new TSource[size];
            var count = 0;

            foreach (TSource element in source)
            {
                elements[count] = element;
                checked
                {
                    index++;
                }

                count = index % size;
                if (count == 0)
                {
                    yield return elements;
                    elements = new TSource[size];
                }
            }

            // Avoids returning empty chunks.
            if (count > 0)
            {
                yield return new ArraySegment<TSource>(elements, 0, count);
            }
        }
    }
}
