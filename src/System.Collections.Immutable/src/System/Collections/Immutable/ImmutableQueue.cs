// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Collections.Immutable
{
    /// <summary>
    /// A set of initialization methods for instances of <see cref="ImmutableQueue{T}"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public static class ImmutableQueue
    {
        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <returns>The immutable collection.</returns>
        [Pure]
        public static ImmutableQueue<T> Create<T>()
        {
            return ImmutableQueue<T>.Empty;
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified item.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="item">The item to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        [Pure]
        public static ImmutableQueue<T> Create<T>(T item)
        {
            return new ImmutableQueue<T>(ImmutableStack.Create(item), ImmutableStack<T>.Empty);
        }

        /// <summary>
        /// Creates a new immutable queue from the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items to store in the queue.</typeparam>
        /// <param name="items">The enumerable to copy items from.</param>
        /// <returns>The new immutable queue.</returns>
        [Pure]
        public static ImmutableQueue<T> CreateRange<T>(IEnumerable<T> items)
        {
            Requires.NotNull(items, nameof(items));

            var ilist = items as IList<T>;
            if (ilist != null)
            {
                return CreateRange(ilist: ilist);
            }

            using (IEnumerator<T> e = items.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return ImmutableQueue<T>.Empty;
                }

                var forwards = ImmutableStack.Create(e.Current);
                var backwards = ImmutableStack<T>.Empty;
                
                while (e.MoveNext())
                {
                    backwards = backwards.Push(e.Current);
                }

                return new ImmutableQueue<T>(forwards: forwards, backwards: backwards);
            }
        }

        /// <summary>
        /// Creates a new immutable queue from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of items to store in the queue.</typeparam>
        /// <param name="list">The list to copy items from.</param>
        /// <returns>The new immutable queue.</returns>
        /// <remarks>
        /// This version is a faster method of creating an immutable queue because it pushes
        /// the items onto the queue's forwards stack in reverse order. This eliminates the need
        /// to have a backwards stack and then expensively reverse that stack when two items are
        /// dequeued. Additionally, we make one fewer virtual method call per item than the
        /// enumerable version.
        /// </remarks>
        private static ImmutableQueue<T> CreateRange<T>(IList<T> ilist)
        {
            Debug.Assert(ilist != null);

            int count = ilist.Count;
            if (count <= 0)
            {
                return ImmutableQueue<T>.Empty;
            }

            var forwards = ImmutableStack<T>.Empty;

            for (int i = count - 1; i >= 0; i--)
            {
                forwards = forwards.Push(ilist[i]);
            }

            return new ImmutableQueue<T>(forwards: forwards, backwards: ImmutableStack<T>.Empty);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="items">The items to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        [Pure]
        public static ImmutableQueue<T> Create<T>(params T[] items)
        {
            Requires.NotNull(items, nameof(items));

            var queue = ImmutableQueue<T>.Empty;
            foreach (var item in items)
            {
                queue = queue.Enqueue(item);
            }

            return queue;
        }

        /// <summary>
        /// Retrieves the item at the head of the queue, and returns a queue with the head element removed.
        /// </summary>
        /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
        /// <param name="queue">The queue to dequeue from.</param>
        /// <param name="value">Receives the value from the head of the queue.</param>
        /// <returns>The new queue with the head element removed.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the stack is empty.</exception>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        [Pure]
        public static IImmutableQueue<T> Dequeue<T>(this IImmutableQueue<T> queue, out T value)
        {
            Requires.NotNull(queue, nameof(queue));

            value = queue.Peek();
            return queue.Dequeue();
        }
    }
}
