// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.Versioning;

namespace System.Collections.Immutable
{
    public partial struct ImmutableArray<T>
    {
        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// See <see cref="IImmutableList{T}"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        IImmutableList<T> IImmutableList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
}
