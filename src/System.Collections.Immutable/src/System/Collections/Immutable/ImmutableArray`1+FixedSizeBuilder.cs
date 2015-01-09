// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Validation;

namespace System.Collections.Immutable
{
    public partial struct ImmutableArray<T>
    {
        /// <summary>
        /// A writable array accessor that can be converted into an <see cref="ImmutableArray{T}"/>
        /// instance without allocating memory.
        /// </summary>
        [DebuggerTypeProxy(typeof(ImmutableArrayFixedLengthBuilderDebuggerProxy<>))]
        [DebuggerDisplay("Length = {Length}")]
        public sealed class FixedLengthBuilder : IReadOnlyList<T>
        {
            /// <summary>
            /// The backing array for the builder.
            /// </summary>
            private T[] _elements;

            /// <summary>
            /// Initializes a new instance of the <see cref="FixedLengthBuilder"/> class.
            /// </summary>
            /// <param name="length">The length of the internal array.</param>
            internal FixedLengthBuilder(int length)
            {
                Reset(length);
            }

            /// <summary>
            /// Gets the length of the internal array 
            /// </summary>
            public int Length
            {
                get { return _elements == null ? 0 : _elements.Length; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance has a backing array.  The internal array is 
            /// removed on calls to ToImmutableAndClear or ToArrayAndClear.
            /// </summary>
            public bool IsInitialized
            {
                get { return _elements != null; }
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <exception cref="System.IndexOutOfRangeException" />
            /// <exception cref="System.InvalidOperationException" />
            public T this[int index]
            {
                get
                {
                    CheckIsInitialized();
                    return _elements[index];
                }

                set
                {
                    CheckIsInitialized();
                    _elements[index] = value;
                }
            }

            /// <summary>
            /// Initializes the internal array of the builder to the specified length.  Any existing 
            /// content in the builder will be erased as a part of this method.
            /// </summary>
            /// <param name="length">The length of the internal array.</param>
            public void Reset(int length)
            {
                Requires.Range(length >= 0, "length");
                if (length == 0)
                {
                    _elements = ImmutableArray<T>.Empty.array;
                }
                else if (_elements != null && _elements.Length == length)
                {
                    Array.Clear(_elements, 0, _elements.Length);
                }
                else
                {
                    _elements = new T[length];
                }
            }

            /// <summary>
            /// Returns an immutable array that represents the data in the builder.
            /// </summary>
            /// <returns>An immutable array.</returns>
            /// <remarks>
            /// The builder can be reinitialized with the Reset method.  
            /// 
            /// Be careful not to use this method in parallel with the indexer set method.  Such a race
            /// has the potential to create an ImmutableArray{T} value where the elements can be observed
            /// to change.
            /// </remarks>
            public ImmutableArray<T> ToImmutableAndClear()
            {
                CheckIsInitialized();
                var temp = _elements;
                _elements = null;
                return new ImmutableArray<T>(temp);
            }

            /// <summary>
            /// Returns an array that represents the data in the builder.
            /// </summary>
            /// <returns>An array.</returns>
            /// <remarks>The builder can be reinitialized with the Reset method.</remarks>
            public T[] ToArrayAndClear()
            {
                CheckIsInitialized();
                var temp = _elements;
                _elements = null;
                return temp;
            }

            /// <summary>
            /// Creates a new array with the current contents of the builder.
            /// </summary>
            public T[] ToArray()
            {
                CheckIsInitialized();
                var temp = new T[_elements.Length];
                Array.Copy(_elements, temp, _elements.Length);
                return temp;
            }

            /// <summary>
            /// Returns an enumerator for the contents of the array.
            /// </summary>
            /// <returns>An enumerator.</returns>
            public Enumerator GetEnumerator()
            {
                CheckIsInitialized();
                return new Enumerator(_elements);
            }

            private void CheckIsInitialized()
            {
                if (!IsInitialized)
                {
                    ThrowInvalidOperation();
                }
            }

            private void ThrowInvalidOperation()
            {
                throw new InvalidOperationException(Strings.InvalidOperationOnUninitializedFixedSizeBuilder);
            }

            int IReadOnlyCollection<T>.Count
            {
                get { return Length; }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                CheckIsInitialized();
                return EnumeratorObject.Create(_elements);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                CheckIsInitialized();
                return EnumeratorObject.Create(_elements);
            }
        }
    }

    /// <summary>
    /// A simple view of the builder that the debugger can show to the developer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class ImmutableArrayFixedLengthBuilderDebuggerProxy<T>
    {
        /// <summary>
        /// The builder to be displayed
        /// </summary>
        private readonly ImmutableArray<T>.FixedLengthBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArrayFixedLengthBuilderDebuggerProxy{T}"/> class.
        /// </summary>
        /// <param name="builder">The builder to display in the debugger</param>
        public ImmutableArrayFixedLengthBuilderDebuggerProxy(ImmutableArray<T>.FixedLengthBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Gets a simple debugger-viewable collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] A
        {
            get { return _builder.ToArray(); }
        }
    }
}
