// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <content>
    /// Contains the inner KeyCollection struct.
    /// </content>
    partial class ImmutableDictionary<TKey, TValue>
    {
        /// <summary>
        /// This structure represents a collection of keys in a dictionary.
        /// </summary>
        public struct KeyCollection : IEnumerable<TKey>
        {
            /// <summary>
            /// The underlying dictionary.
            /// </summary>
            private readonly ImmutableDictionary<TKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> structure.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            internal KeyCollection(ImmutableDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="Enumerator"/> that can be used to iterate through the collection.
            /// </returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary.GetEnumerator());
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
            /// </returns>
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="IEnumerator"/> that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Enumerates the contents of the collection in an allocation-free manner.
            /// </summary>
            public struct Enumerator : IEnumerator<TKey>
            {
                /// <summary>
                /// The enumerator over key/value pairs in the dictionary.
                /// </summary>
                private ImmutableDictionary<TKey, TValue>.Enumerator _dictionaryEnumerator;

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerator"/> structure.
                /// </summary>
                /// <param name="dictionaryEnumerator">The dictionary enumerator.</param>
                internal Enumerator(ImmutableDictionary<TKey, TValue>.Enumerator dictionaryEnumerator)
                    : this()
                {
                    _dictionaryEnumerator = dictionaryEnumerator;
                }

                /// <summary>
                /// Gets the current element.
                /// </summary>
                public TKey Current
                {
                    get
                    {
                        return _dictionaryEnumerator.Current.Key;
                    }
                }

                /// <summary>
                /// Gets the current element.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }

                /// <summary>
                /// Advances the enumerator to the next element of the collection.
                /// </summary>
                /// <returns>
                /// true if the enumerator was successfully advanced to the next element; false if the enumerator has
                /// passed the end of the collection.
                /// </returns>
                /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
                public bool MoveNext()
                {
                    return _dictionaryEnumerator.MoveNext();
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
                public void Reset()
                {
                    _dictionaryEnumerator.Reset();
                }

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    _dictionaryEnumerator.Dispose();
                }
            }
        }
    }
}
