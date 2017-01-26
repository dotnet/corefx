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
        /// Searches the array for the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int IndexOf(T item)
        {
            var self = this;
            return self.IndexOf(item, 0, self.Length, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Searches the array for the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <param name="equalityComparer">The equality comparer to use in the search.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int IndexOf(T item, int startIndex, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            return self.IndexOf(item, startIndex, self.Length - startIndex, equalityComparer);
        }

        /// <summary>
        /// Searches the array for the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int IndexOf(T item, int startIndex)
        {
            var self = this;
            return self.IndexOf(item, startIndex, self.Length - startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Searches the array for the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <param name="count">The number of elements to search.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int IndexOf(T item, int startIndex, int count)
        {
            return this.IndexOf(item, startIndex, count, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Searches the array for the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <param name="count">The number of elements to search.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use in the search.
        /// If <c>null</c>, <see cref="EqualityComparer{T}.Default"/> is used.
        /// </param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int IndexOf(T item, int startIndex, int count, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();

            if (count == 0 && startIndex == 0)
            {
                return -1;
            }

            Requires.Range(startIndex >= 0 && startIndex < self.Length, nameof(startIndex));
            Requires.Range(count >= 0 && startIndex + count <= self.Length, nameof(count));

            equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            if (equalityComparer == EqualityComparer<T>.Default)
            {
                return Array.IndexOf(self.array, item, startIndex, count);
            }
            else
            {
                for (int i = startIndex; i < startIndex + count; i++)
                {
                    if (equalityComparer.Equals(self.array[i], item))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Searches the array for the specified item in reverse.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int LastIndexOf(T item)
        {
            var self = this;
            if (self.Length == 0)
            {
                return -1;
            }

            return self.LastIndexOf(item, self.Length - 1, self.Length, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Searches the array for the specified item in reverse.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int LastIndexOf(T item, int startIndex)
        {
            var self = this;
            if (self.Length == 0 && startIndex == 0)
            {
                return -1;
            }

            return self.LastIndexOf(item, startIndex, startIndex + 1, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Searches the array for the specified item in reverse.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <param name="count">The number of elements to search.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int LastIndexOf(T item, int startIndex, int count)
        {
            return this.LastIndexOf(item, startIndex, count, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Searches the array for the specified item in reverse.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <param name="count">The number of elements to search.</param>
        /// <param name="equalityComparer">The equality comparer to use in the search.</param>
        /// <returns>The 0-based index into the array where the item was found; or -1 if it could not be found.</returns>
        [Pure]
        public int LastIndexOf(T item, int startIndex, int count, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();

            if (startIndex == 0 && count == 0)
            {
                return -1;
            }

            Requires.Range(startIndex >= 0 && startIndex < self.Length, nameof(startIndex));
            Requires.Range(count >= 0 && startIndex - count + 1 >= 0, nameof(count));

            equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            if (equalityComparer == EqualityComparer<T>.Default)
            {
                return Array.LastIndexOf(self.array, item, startIndex, count);
            }
            else
            {
                for (int i = startIndex; i >= startIndex - count + 1; i--)
                {
                    if (equalityComparer.Equals(item, self.array[i]))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Determines whether the specified item exists in the array.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><c>true</c> if an equal value was found in the array; <c>false</c> otherwise.</returns>
        [Pure]
        public bool Contains(T item)
        {
            return this.IndexOf(item) >= 0;
        }
    }
}
