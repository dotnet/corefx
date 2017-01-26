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
        /// Filters the elements of this array to those assignable to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to filter the elements of the sequence on.</typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that contains elements from
        /// the input sequence of type <typeparamref name="TResult"/>.
        /// </returns>
        [Pure]
        public IEnumerable<TResult> OfType<TResult>()
        {
            var self = this;
            if (self.array == null || self.array.Length == 0)
            {
                return Enumerable.Empty<TResult>();
            }

            return self.array.OfType<TResult>();
        }
    }
}