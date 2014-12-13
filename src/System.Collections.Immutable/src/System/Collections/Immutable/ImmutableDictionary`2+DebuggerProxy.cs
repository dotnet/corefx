// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Validation;

namespace System.Collections.Immutable
{
    /// <summary>
    /// A simple view of the immutable collection that the debugger can show to the developer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ImmutableDictionaryDebuggerProxy<TKey, TValue>
    {
        /// <summary>
        /// The collection to be enumerated.
        /// </summary>
        private readonly ImmutableDictionary<TKey, TValue> map;

        /// <summary>
        /// The simple view of the collection.
        /// </summary>
        private KeyValuePair<TKey, TValue>[] contents;

        /// <summary>   
        /// Initializes a new instance of the <see cref="ImmutableDictionaryDebuggerProxy&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="map">The collection to display in the debugger</param>
        public ImmutableDictionaryDebuggerProxy(ImmutableDictionary<TKey, TValue> map)
        {
            Requires.NotNull(map, "map");
            this.map = map;
        }

        /// <summary>
        /// Gets a simple debugger-viewable collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Contents
        {
            get
            {
                if (this.contents == null)
                {
                    this.contents = this.map.ToArray(this.map.Count);
                }

                return this.contents;
            }
        }
    }
}

