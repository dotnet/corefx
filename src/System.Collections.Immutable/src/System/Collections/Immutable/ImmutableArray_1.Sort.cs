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
        /// Returns a sorted instance of this array.
        /// </summary>
        [Pure]
        public ImmutableArray<T> Sort()
        {
            var self = this;
            return self.Sort(0, self.Length, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="ImmutableArray{T}"/> using
        /// the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <returns>The sorted list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
        [Pure]
        public ImmutableArray<T> Sort(Comparison<T> comparison)
        {
            Requires.NotNull(comparison, nameof(comparison));

            var self = this;
            return self.Sort(Comparer<T>.Create(comparison));
        }

        /// <summary>
        /// Returns a sorted instance of this array.
        /// </summary>
        /// <param name="comparer">The comparer to use in sorting. If <c>null</c>, the default comparer is used.</param>
        [Pure]
        public ImmutableArray<T> Sort(IComparer<T> comparer)
        {
            var self = this;
            return self.Sort(0, self.Length, comparer);
        }

        /// <summary>
        /// Returns a sorted instance of this array.
        /// </summary>
        /// <param name="index">The index of the first element to consider in the sort.</param>
        /// <param name="count">The number of elements to include in the sort.</param>
        /// <param name="comparer">The comparer to use in sorting. If <c>null</c>, the default comparer is used.</param>
        [Pure]
        public ImmutableArray<T> Sort(int index, int count, IComparer<T> comparer)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.Range(index >= 0, nameof(index));
            Requires.Range(count >= 0 && index + count <= self.Length, nameof(count));

            // 0 and 1 element arrays don't need to be sorted.
            if (count > 1)
            {
                if (comparer == null)
                {
                    comparer = Comparer<T>.Default;
                }

                // Avoid copying the entire array when the array is already sorted.
                bool outOfOrder = false;
                for (int i = index + 1; i < index + count; i++)
                {
                    if (comparer.Compare(self.array[i - 1], self.array[i]) > 0)
                    {
                        outOfOrder = true;
                        break;
                    }
                }

                if (outOfOrder)
                {
                    var tmp = new T[self.Length];
                    Array.Copy(self.array, 0, tmp, 0, self.Length);
                    Array.Sort(tmp, index, count, comparer);
                    return new ImmutableArray<T>(tmp);
                }
            }

            return self;
        }
    }
}