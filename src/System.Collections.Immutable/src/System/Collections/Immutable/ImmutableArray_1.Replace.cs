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
        /// Returns an array with the item at the specified position replaced.
        /// </summary>
        /// <param name="index">The index of the item to replace.</param>
        /// <param name="item">The new item.</param>
        /// <returns>The new array.</returns>
        [Pure]
        public ImmutableArray<T> SetItem(int index, T item)
        {
            var self = this;
            self.ThrowNullRefIfNotInitialized();
            Requires.Range(index >= 0 && index < self.Length, nameof(index));

            T[] tmp = new T[self.Length];
            Array.Copy(self.array, 0, tmp, 0, self.Length);
            tmp[index] = item;
            return new ImmutableArray<T>(tmp);
        }

        /// <summary>
        /// Replaces the first equal element in the list with the specified element.
        /// </summary>
        /// <param name="oldValue">The element to replace.</param>
        /// <param name="newValue">The element to replace the old element with.</param>
        /// <returns>The new list -- even if the value being replaced is equal to the new value for that position.</returns>
        /// <exception cref="ArgumentException">Thrown when the old value does not exist in the list.</exception>
        [Pure]
        public ImmutableArray<T> Replace(T oldValue, T newValue)
        {
            return this.Replace(oldValue, newValue, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Replaces the first equal element in the list with the specified element.
        /// </summary>
        /// <param name="oldValue">The element to replace.</param>
        /// <param name="newValue">The element to replace the old element with.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use in the search.
        /// If <c>null</c>, <see cref="EqualityComparer{T}.Default"/> is used.
        /// </param>
        /// <returns>The new list -- even if the value being replaced is equal to the new value for that position.</returns>
        /// <exception cref="ArgumentException">Thrown when the old value does not exist in the list.</exception>
        [Pure]
        public ImmutableArray<T> Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            int index = self.IndexOf(oldValue, 0, self.Length, equalityComparer);
            if (index < 0)
            {
                throw new ArgumentException(SR.CannotFindOldValue, nameof(oldValue));
            }

            return self.SetItem(index, newValue);
        }
    }
}
