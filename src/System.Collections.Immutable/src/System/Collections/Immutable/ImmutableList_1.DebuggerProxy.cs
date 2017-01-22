// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Immutable
{
    /// <summary>
    /// A simple view of the immutable list that the debugger can show to the developer.
    /// </summary>
    internal class ImmutableListDebuggerProxy<T>
    {
        /// <summary>
        /// The collection to be enumerated.
        /// </summary>
        private readonly ImmutableList<T> _list;

        /// <summary>
        /// The simple view of the collection.
        /// </summary>
        private T[] _cachedContents;

        /// <summary>   
        /// Initializes a new instance of the <see cref="ImmutableListDebuggerProxy{T}"/> class.
        /// </summary>
        /// <param name="list">The list to display in the debugger</param>
        public ImmutableListDebuggerProxy(ImmutableList<T> list)
        {
            Requires.NotNull(list, nameof(list));
            _list = list;
        }

        /// <summary>
        /// Gets a simple debugger-viewable list.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Contents
        {
            get
            {
                if (_cachedContents == null)
                {
                    _cachedContents = _list.ToArray(_list.Count);
                }

                return _cachedContents;
            }
        }
    }
}
