?// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    internal sealed class ReadOnlyMemoryDebugView<T>
    {
        private readonly ReadOnlyMemory<T> _memory;

        public ReadOnlyMemoryDebugView(Memory<T> memory)
        {
            _memory = memory;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return _memory.ToArray();
            }
        }
    }
}
