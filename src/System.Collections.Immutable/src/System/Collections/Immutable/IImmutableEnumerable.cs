// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <summary>
    /// A marker interface for immutable enumerables.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <remarks>
    /// It is safe to assume that types that implement this interface will never change.
    /// Optimizations such as caching contents into an array for better access performance
    /// are legal.
    /// </remarks>
    internal interface IImmutableEnumerable<out T> : IEnumerable<T>
    {
    }
}
