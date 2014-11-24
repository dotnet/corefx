// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.Immutable
{
    [DebuggerDisplay("Count = {stack != null ? stack.Count : 0}")]
    internal static class AllocFreeConcurrentStack<T>
    {
        private const int MaxSize = 35;

        [ThreadStatic]
        private static Stack<RefAsValueType<T>> _stack;

        public static void TryAdd(T item)
        {
            Stack<RefAsValueType<T>> localStack = _stack; // cache in a local to avoid unnecessary TLS hits on repeated accesses
            if (localStack == null)
            {
                _stack = localStack = new Stack<RefAsValueType<T>>(MaxSize);
            }

            // Just in case we're in a scenario where an object is continually requested on one thread
            // and returned on another, avoid unbounded growth of the stack.
            if (localStack.Count < MaxSize)
            {
                localStack.Push(new RefAsValueType<T>(item));
            }
        }

        public static bool TryTake(out T item)
        {
            Stack<RefAsValueType<T>> localStack = _stack; // cache in a local to avoid unnecessary TLS hits on repeated accesses
            if (localStack != null && localStack.Count > 0)
            {
                item = localStack.Pop().Value;
                return true;
            }

            item = default(T);
            return false;
        }
    }
}
