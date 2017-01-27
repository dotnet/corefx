// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;

namespace System.Collections.Immutable
{
    public partial struct ImmutableArray<T>
    {
        /// <summary>
        /// Returns an array with the first occurrence of the specified element removed from the array.
        /// If no match is found, the current array is returned.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>The new array.</returns>
        [Pure]
        public ImmutableArray<T> Remove(T item)
        {
            return this.Remove(item, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Returns an array with the first occurrence of the specified element removed from the array.
        /// If no match is found, the current array is returned.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use in the search.
        /// If <c>null</c>, <see cref="EqualityComparer{T}.Default"/> is used.
        /// </param>
        /// <returns>The new array.</returns>
        [Pure]
        public ImmutableArray<T> Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            int index = self.IndexOf(item, 0, self.Length, equalityComparer);
            return index < 0
                ? self
                : self.RemoveAt(index);
        }

        /// <summary>
        /// Returns an array with the element at the specified position removed.
        /// </summary>
        /// <param name="index">The 0-based index into the array for the element to omit from the returned array.</param>
        /// <returns>The new array.</returns>
        [Pure]
        public ImmutableArray<T> RemoveAt(int index)
        {
            return this.RemoveRange(index, 1);
        }

        /// <summary>
        /// Returns an array with the elements at the specified position removed.
        /// </summary>
        /// <param name="index">The 0-based index into the array for the element to omit from the returned array.</param>
        /// <param name="length">The number of elements to remove.</param>
        /// <returns>The new array.</returns>
        [Pure]
        public ImmutableArray<T> RemoveRange(int index, int length)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.Range(index >= 0 && index <= self.Length, nameof(index));
            Requires.Range(length >= 0 && index + length <= self.Length, nameof(length));

            if (length == 0)
            {
                return self;
            }

            T[] tmp = new T[self.Length - length];
            Array.Copy(self.array, 0, tmp, 0, index);
            Array.Copy(self.array, index + length, tmp, index, self.Length - index - length);
            return new ImmutableArray<T>(tmp);
        }

        /// <summary>
        /// Removes the specified values from this list.
        /// </summary>
        /// <param name="items">The items to remove if matches are found in this list.</param>
        /// <returns>
        /// A new list with the elements removed.
        /// </returns>
        [Pure]
        public ImmutableArray<T> RemoveRange(IEnumerable<T> items)
        {
            return this.RemoveRange(items, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Removes the specified values from this list.
        /// </summary>
        /// <param name="items">The items to remove if matches are found in this list.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use in the search.
        /// If <c>null</c>, <see cref="EqualityComparer{T}.Default"/> is used.
        /// </param>
        /// <returns>
        /// A new list with the elements removed.
        /// </returns>
        [Pure]
        public ImmutableArray<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.NotNull(items, nameof(items));

            var indicesToRemove = new SortedSet<int>();
            foreach (var item in items)
            {
                int index = self.IndexOf(item, 0, self.Length, equalityComparer);
                while (index >= 0 && !indicesToRemove.Add(index) && index + 1 < self.Length)
                {
                    // This is a duplicate of one we've found. Try hard to find another instance in the list to remove.
                    index = self.IndexOf(item, index + 1, equalityComparer);
                }
            }

            return self.RemoveAtRange(indicesToRemove);
        }

        /// <summary>
        /// Removes the specified values from this list.
        /// </summary>
        /// <param name="items">The items to remove if matches are found in this list.</param>
        /// <returns>
        /// A new list with the elements removed.
        /// </returns>
        [Pure]
        public ImmutableArray<T> RemoveRange(ImmutableArray<T> items)
        {
            return this.RemoveRange(items, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Removes the specified values from this list.
        /// </summary>
        /// <param name="items">The items to remove if matches are found in this list.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use in the search.
        /// </param>
        /// <returns>
        /// A new list with the elements removed.
        /// </returns>
        [Pure]
        public ImmutableArray<T> RemoveRange(ImmutableArray<T> items, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            Requires.NotNull(items.array, nameof(items));

            if (items.IsEmpty)
            {
                self.ThrowNullRefIfNotInitialized();
                return self;
            }
            else if (items.Length == 1)
            {
                return self.Remove(items[0], equalityComparer);
            }
            else
            {
                return self.RemoveRange(items.array, equalityComparer);
            }
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified
        /// predicate.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements
        /// to remove.
        /// </param>
        /// <returns>
        /// The new list.
        /// </returns>
        [Pure]
        public ImmutableArray<T> RemoveAll(Predicate<T> match)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.NotNull(match, nameof(match));

            if (self.IsEmpty)
            {
                return self;
            }

            List<int> removeIndices = null;
            for (int i = 0; i < self.array.Length; i++)
            {
                if (match(self.array[i]))
                {
                    if (removeIndices == null)
                    {
                        removeIndices = new List<int>();
                    }

                    removeIndices.Add(i);
                }
            }

            return removeIndices != null ?
                self.RemoveAtRange(removeIndices) :
                self;
        }

        /// <summary>
        /// Returns an empty array.
        /// </summary>
        [Pure]
        public ImmutableArray<T> Clear()
        {
            return Empty;
        }

        /// <summary>
        /// Returns an array with items at the specified indices removed.
        /// </summary>
        /// <param name="indicesToRemove">A **sorted set** of indices to elements that should be omitted from the returned array.</param>
        /// <returns>The new array.</returns>
        private ImmutableArray<T> RemoveAtRange(ICollection<int> indicesToRemove)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.NotNull(indicesToRemove, nameof(indicesToRemove));

            if (indicesToRemove.Count == 0)
            {
                // Be sure to return a !IsDefault instance.
                return self;
            }

            var newArray = new T[self.Length - indicesToRemove.Count];
            int copied = 0;
            int removed = 0;
            int lastIndexRemoved = -1;
            foreach (var indexToRemove in indicesToRemove)
            {
                int copyLength = lastIndexRemoved == -1 ? indexToRemove : (indexToRemove - lastIndexRemoved - 1);
                Debug.Assert(indexToRemove > lastIndexRemoved); // We require that the input be a sorted set.
                Array.Copy(self.array, copied + removed, newArray, copied, copyLength);
                removed++;
                copied += copyLength;
                lastIndexRemoved = indexToRemove;
            }

            Array.Copy(self.array, copied + removed, newArray, copied, self.Length - (copied + removed));

            return new ImmutableArray<T>(newArray);
        }
    }
}
