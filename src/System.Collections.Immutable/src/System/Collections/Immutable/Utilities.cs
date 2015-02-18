// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    internal static class Utilities
    {
        /// <summary>
        /// Gets the default comparer to use for a given type, guarding against a race in lazy initialization of the value.
        /// </summary>
        /// <typeparam name="T">The type of value to be compared.</typeparam>
        /// <returns>The comparer instance.</returns>
        internal static Comparer<T> GetDefaultComparer<T>()
        {
            // Protect against a race when .NET initializes its Comparer<T>.Default instance
            // that can lead to us getting a different instance than everyone else will get later.
            // This is important because if our Empty static collection instance is based on the 
            // odd instance of the default comparer, then forever after anyone asking for an instance
            // of the collection with the default comparer will experience another allocation.
            var dummy = Comparer<T>.Default;
            return Comparer<T>.Default;
        }

        /// <summary>
        /// Gets the default comparer to use for a given type, guarding against a race in lazy initialization of the value.
        /// </summary>
        /// <typeparam name="T">The type of value to be compared.</typeparam>
        /// <returns>The comparer instance.</returns>
        internal static EqualityComparer<T> GetDefaultEqualityComparer<T>()
        {
            // Protect against a race when .NET initializes its EqualityComparer<T>.Default instance
            // that can lead to us getting a different instance than everyone else will get later.
            // This is important because if our Empty static collection instance is based on the 
            // odd instance of the default comparer, then forever after anyone asking for an instance
            // of the collection with the default comparer will experience another allocation.
            var dummy = EqualityComparer<T>.Default;
            return EqualityComparer<T>.Default;
        }
    }
}
