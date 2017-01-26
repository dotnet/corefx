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
    public partial struct ImmutableArray<T> : IImmutableList<T>
    {
        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.Clear()
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.Clear();
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.Add(T value)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.Add(value);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.AddRange(items);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.Insert(int index, T element)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.Insert(index, element);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.InsertRange(int index, IEnumerable<T> items)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.InsertRange(index, items);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.Remove(value, equalityComparer);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.RemoveAll(match);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.RemoveRange(items, equalityComparer);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.RemoveRange(index, count);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveAt(int index)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.RemoveAt(index);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.SetItem(int index, T value)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.SetItem(index, value);
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        IImmutableList<T> IImmutableList<T>.Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.Replace(oldValue, newValue, equalityComparer);
        }
        
    }
}
