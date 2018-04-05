// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Buffers
{
    internal sealed class ReadOnlySequenceDebugView<T>
    {
        private readonly T[] _array;

        private readonly ReadOnlySequenceDebugViewSegments _segments;
        
        public ReadOnlySequenceDebugView(ReadOnlySequence<T> sequence)
        {
            _array = sequence.ToArray();
            var segments = new List<ReadOnlyMemory<T>>();
            foreach (var readOnlyMemory in sequence)
            {
                segments.Add(readOnlyMemory);
            }
            _segments = new ReadOnlySequenceDebugViewSegments()
            {
                Segments = segments.ToArray()
            };
        }
        
        public ReadOnlySequenceDebugViewSegments BufferSegments => _segments;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _array;

        [DebuggerDisplay("Count: {Segments.Length}", Name = "Segments")]
        public struct ReadOnlySequenceDebugViewSegments
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        
            public ReadOnlyMemory<T>[] Segments { get; set; }
        }
    }
}
