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
        [DebuggerTypeProxy(typeof(ImmutableArrayFixedSizeBuilderDebuggerProxy<>))]
        [DebuggerDisplay("Capacity = {Capacity}")]
        public sealed class FixedSizeBuilder
        {
            /// <summary>
            /// The backing array for the builder.
            /// </summary>
            private T[] elements;

            /// <summary>
            /// Initializes a new instance of the <see cref="FixedSizeBuilder"/> class.
            /// </summary>
            /// <param name="capacity">The capacity of the internal array.</param>
            internal FixedSizeBuilder(int capacity)
            {
                Reset(capacity);
            }

            /// <summary>
            /// Gets the capacity of the internal array 
            /// </summary>
            public int Capacity 
            {
                get { return this.elements == null ? 0 : this.elements.Length; }
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
                    if (elements == null)
                    {
                        throw new InvalidOperationException();
                    }

                    return this.elements[index];
                }

                set
                {
                    if (elements == null)
                    {
                        throw new InvalidOperationException();
                    }

                    this.elements[index] = value;
                }
            }

            /// <summary>
            /// Initializes the internal array of the builder to the specified capacity
            /// </summary>
            /// <param name="capacity">The capacity of the internal array.</param>
            public void Reset(int capacity)
            {
                Requires.Range(capacity >= 0, "capacity");
                this.elements = new T[capacity];
            }

            /// <summary>
            /// Clears the internal array and returns it as an immutable array instance
            /// </summary>
            /// <returns>An immutable array.</returns>
            public ImmutableArray<T> Freeze()
            {
                var temp = this.elements;
                this.elements = null;
                return new ImmutableArray<T>(temp);
            }
        }
    }

    /// <summary>
    /// A simple view of the builder that the debugger can show to the developer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class ImmutableArrayFixedSizeBuilderDebuggerProxy<T>
    {
        /// <summary>
        /// The builder to be displayed
        /// </summary>
        private readonly ImmutableArray<T>.FixedSizeBuilder builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArrayFixedSizeBuilderDebuggerProxy{T}"/> class.
        /// </summary>
        /// <param name="builder">The builder to display in the debugger</param>
        public ImmutableArrayFixedSizeBuilderDebuggerProxy(ImmutableArray<T>.FixedSizeBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Gets a simple debugger-viewable collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] A
        {
            get
            {
                var array = new T[this.builder.Capacity];
                for (int i = 0; i < this.builder.Capacity; i++)
                {
                    array[i] = this.builder[i];
                }

                return array;
            }
        }
    }
}
