// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <content>
    /// Contains the inner Enumerator class.
    /// </content>
    public partial class ImmutableHashSet<T>
    {
        /// <summary>
        /// Enumerates the contents of the collection in an allocation-free manner.
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IDisposable
        {
            /// <summary>
            /// The builder being enumerated, if applicable.
            /// </summary>
            private readonly Builder _builder;

            /// <summary>
            /// The enumerator over the sorted dictionary whose keys are hash values.
            /// </summary>
            private ImmutableSortedDictionary<int, HashBucket>.Enumerator _mapEnumerator;

            /// <summary>
            /// The enumerator in use within an individual HashBucket.
            /// </summary>
            private HashBucket.Enumerator _bucketEnumerator;

            /// <summary>
            /// The version of the builder (when applicable) that is being enumerated.
            /// </summary>
            private int _enumeratingBuilderVersion;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmutableHashSet&lt;T&gt;.Enumerator" /> struct.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="builder">The builder, if applicable.</param>
            internal Enumerator(ImmutableSortedDictionary<int, HashBucket>.Node root, Builder builder = null)
            {
                this._builder = builder;
                this._mapEnumerator = new ImmutableSortedDictionary<int, HashBucket>.Enumerator(root);
                this._bucketEnumerator = default(HashBucket.Enumerator);
                this._enumeratingBuilderVersion = builder != null ? builder.Version : -1;
            }

            /// <summary>
            /// Gets the current element.
            /// </summary>
            public T Current
            {
                get
                {
                    this._mapEnumerator.ThrowIfDisposed();
                    return this._bucketEnumerator.Current;
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

                if (this._bucketEnumerator.MoveNext())
                {
                    return true;
                }

                if (this._mapEnumerator.MoveNext())
                {
                    this._bucketEnumerator = new HashBucket.Enumerator(this._mapEnumerator.Current.Value);
                    return this._bucketEnumerator.MoveNext();
                }

                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                this._enumeratingBuilderVersion = _builder != null ? _builder.Version : -1;
                this._mapEnumerator.Reset();

                // Reseting the bucket enumerator is pointless because we'll start on a new bucket later anyway.
                this._bucketEnumerator.Dispose();
                this._bucketEnumerator = default(HashBucket.Enumerator);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this._mapEnumerator.Dispose();
                this._bucketEnumerator.Dispose();
            }

            /// <summary>
            /// Throws an exception if the underlying builder's contents have been changed since enumeration started.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">Thrown if the collection has changed.</exception>
            private void ThrowIfChanged()
            {
                if (this._builder != null && this._builder.Version != this._enumeratingBuilderVersion)
                {
                    throw new InvalidOperationException(Strings.CollectionModifiedDuringEnumeration);
                }
            }
        }
    }
}
