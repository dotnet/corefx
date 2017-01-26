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
        /// Returns a new array with the specified value inserted at the specified position.
        /// </summary>
        /// <param name="index">The 0-based index into the array at which the new item should be added.</param>
        /// <param name="item">The item to insert at the start of the array.</param>
        /// <returns>A new array.</returns>
        [Pure]
        public ImmutableArray<T> Insert(int index, T item)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.Range(index >= 0 && index <= self.Length, nameof(index));

            if (self.Length == 0)
            {
                return ImmutableArray.Create(item);
            }

            T[] tmp = new T[self.Length + 1];
            tmp[index] = item;

            if (index != 0)
            {
                Array.Copy(self.array, 0, tmp, 0, index);
            }
            if (index != self.Length)
            {
                Array.Copy(self.array, index, tmp, index + 1, self.Length - index);
            }
            
            return new ImmutableArray<T>(tmp);
        }

        /// <summary>
        /// Inserts the specified values at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the value.</param>
        /// <param name="items">The elements to insert.</param>
        /// <returns>The new immutable collection.</returns>
        [Pure]
        public ImmutableArray<T> InsertRange(int index, IEnumerable<T> items)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.Range(index >= 0 && index <= self.Length, nameof(index));
            Requires.NotNull(items, nameof(items));

            if (self.Length == 0)
            {
                return ImmutableArray.CreateRange(items);
            }

            int count = ImmutableExtensions.GetCount(ref items);
            if (count == 0)
            {
                return self;
            }

            T[] tmp = new T[self.Length + count];
            
            if (index != 0)
            {
                Array.Copy(self.array, 0, tmp, 0, index);
            }
            if (index != self.Length)
            {
                Array.Copy(self.array, index, tmp, index + count, self.Length - index);
            }

            // We want to copy over the items we need to insert.
            // Check first to see if items is a well-known collection we can call CopyTo
            // on to the array, which is an order of magnitude faster than foreach.
            // Otherwise, go to the fallback route where we manually enumerate the sequence
            // and place the items in the array one-by-one.

            if (!items.TryCopyTo(tmp, index))
            {
                int sequenceIndex = index;
                foreach (var item in items)
                {
                    tmp[sequenceIndex++] = item;
                }
            }

            return new ImmutableArray<T>(tmp);
        }

        /// <summary>
        /// Inserts the specified values at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the value.</param>
        /// <param name="items">The elements to insert.</param>
        /// <returns>The new immutable collection.</returns>
        [Pure]
        public ImmutableArray<T> InsertRange(int index, ImmutableArray<T> items)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            ThrowNullRefIfNotInitialized(items);
            Requires.Range(index >= 0 && index <= self.Length, nameof(index));

            if (self.IsEmpty)
            {
                return items;
            }
            else if (items.IsEmpty)
            {
                return self;
            }

            T[] tmp = new T[self.Length + items.Length];
            
            if (index != 0)
            {
                Array.Copy(self.array, 0, tmp, 0, index);
            }
            if (index != self.Length)
            {
                Array.Copy(self.array, index, tmp, index + items.Length, self.Length - index);
            }

            Array.Copy(items.array, 0, tmp, index, items.Length);

            return new ImmutableArray<T>(tmp);
        }

        /// <summary>
        /// Returns a new array with the specified value inserted at the end.
        /// </summary>
        /// <param name="item">The item to insert at the end of the array.</param>
        /// <returns>A new array.</returns>
        [Pure]
        public ImmutableArray<T> Add(T item)
        {
            var self = this;
            if (self.Length == 0)
            {
                return ImmutableArray.Create(item);
            }

            return self.Insert(self.Length, item);
        }

        /// <summary>
        /// Adds the specified values to this list.
        /// </summary>
        /// <param name="items">The values to add.</param>
        /// <returns>A new list with the elements added.</returns>
        [Pure]
        public ImmutableArray<T> AddRange(IEnumerable<T> items)
        {
            var self = this;
            return self.InsertRange(self.Length, items);
        }

        /// <summary>
        /// Adds the specified values to this list.
        /// </summary>
        /// <param name="items">The values to add.</param>
        /// <returns>A new list with the elements added.</returns>
        [Pure]
        public ImmutableArray<T> AddRange(ImmutableArray<T> items)
        {
            var self = this;
            return self.InsertRange(self.Length, items);
        }
    }
}