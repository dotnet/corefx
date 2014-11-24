// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <content>
    /// Contains the inner MutationInput class.
    /// </content>
    public partial class ImmutableDictionary<TKey, TValue>
    {
        /// <summary>
        /// Description of the current data structure as input into a
        /// mutating or query method.
        /// </summary>
        private struct MutationInput
        {
            /// <summary>
            /// The root of the data structure for the collection.
            /// </summary>
            private readonly ImmutableSortedDictionary<int, HashBucket>.Node _root;

            /// <summary>
            /// The comparer used when comparing hash buckets.
            /// </summary>
            private readonly Comparers _comparers;

            /// <summary>
            /// The current number of elements in the collection.
            /// </summary>
            private readonly int _count;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmutableDictionary&lt;TKey, TValue&gt;.MutationInput"/> struct.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="comparers">The comparers.</param>
            /// <param name="count">The current number of elements in the collection.</param>
            internal MutationInput(
                ImmutableSortedDictionary<int, HashBucket>.Node root,
                Comparers comparers,
                int count)
            {
                this._root = root;
                this._comparers = comparers;
                this._count = count;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmutableDictionary&lt;TKey, TValue&gt;.MutationInput"/> struct.
            /// </summary>
            /// <param name="map">The map.</param>
            internal MutationInput(ImmutableDictionary<TKey, TValue> map)
            {
                this._root = map._root;
                this._comparers = map._comparers;
                this._count = map._count;
            }

            /// <summary>
            /// Gets the root of the data structure for the collection.
            /// </summary>
            internal ImmutableSortedDictionary<int, HashBucket>.Node Root
            {
                get { return this._root; }
            }

            /// <summary>
            /// Gets the key comparer.
            /// </summary>
            internal IEqualityComparer<TKey> KeyComparer
            {
                get { return this._comparers.KeyComparer; }
            }

            /// <summary>
            /// Gets the key only comparer.
            /// </summary>
            internal IEqualityComparer<KeyValuePair<TKey, TValue>> KeyOnlyComparer
            {
                get { return this._comparers.KeyOnlyComparer; }
            }

            /// <summary>
            /// Gets the value comparer.
            /// </summary>
            internal IEqualityComparer<TValue> ValueComparer
            {
                get { return this._comparers.ValueComparer; }
            }

            /// <summary>
            /// Gets the comparers.
            /// </summary>
            internal IEqualityComparer<HashBucket> HashBucketComparer
            {
                get { return this._comparers.HashBucketEqualityComparer; }
            }

            /// <summary>
            /// Gets the current number of elements in the collection.
            /// </summary>
            internal int Count
            {
                get { return this._count; }
            }
        }
    }
}
