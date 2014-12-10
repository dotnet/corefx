// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <content>
    /// Contains the inner Enumerator struct.
    /// </content>
    public partial class ImmutableDictionary<TKey, TValue>
    {
        /// <summary>
        /// Enumerates the contents of the collection in an allocation-free manner.
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable
        {
            /// <summary>
            /// The builder being enumerated, if applicable.
            /// </summary>
            private readonly Builder builder;

            /// <summary>
            /// The enumerator over the sorted dictionary whose keys are hash values.
            /// </summary>
            private SortedInt32KeyNode<HashBucket>.Enumerator mapEnumerator;

            /// <summary>
            /// The enumerator in use within an individual HashBucket.
            /// </summary>
            private HashBucket.Enumerator bucketEnumerator;

            /// <summary>
            /// The version of the builder (when applicable) that is being enumerated.
            /// </summary>
            private int enumeratingBuilderVersion;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmutableDictionary&lt;TKey, TValue&gt;.Enumerator"/> struct.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="builder">The builder, if applicable.</param>
            internal Enumerator(SortedInt32KeyNode<HashBucket> root, Builder builder = null)
            {
                this.builder = builder;
                this.mapEnumerator = new SortedInt32KeyNode<HashBucket>.Enumerator(root);
                this.bucketEnumerator = default(HashBucket.Enumerator);
                this.enumeratingBuilderVersion = builder != null ? builder.Version : -1;
            }

            /// <summary>
            /// Gets the current element.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    this.mapEnumerator.ThrowIfDisposed();
                    return this.bucketEnumerator.Current;
                }
            }

            /// <summary>
            /// Gets the current element.
            /// </summary>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                this.ThrowIfChanged();

                if (this.bucketEnumerator.MoveNext())
                {
                    return true;
                }

                if (this.mapEnumerator.MoveNext())
                {
                    this.bucketEnumerator = new HashBucket.Enumerator(this.mapEnumerator.Current.Value);
                    return this.bucketEnumerator.MoveNext();
                }

                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                this.enumeratingBuilderVersion = builder != null ? builder.Version : -1;
                this.mapEnumerator.Reset();

                // Resetting the bucket enumerator is pointless because we'll start on a new bucket later anyway.
                this.bucketEnumerator.Dispose();
                this.bucketEnumerator = default(HashBucket.Enumerator);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.mapEnumerator.Dispose();
                this.bucketEnumerator.Dispose();
            }

            /// <summary>
            /// Throws an exception if the underlying builder's contents have been changed since enumeration started.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">Thrown if the collection has changed.</exception>
            private void ThrowIfChanged()
            {
                if (this.builder != null && this.builder.Version != this.enumeratingBuilderVersion)
                {
                    throw new InvalidOperationException(Strings.CollectionModifiedDuringEnumeration);
                }
            }
        }
    }
}
