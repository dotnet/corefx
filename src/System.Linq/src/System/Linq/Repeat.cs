﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
            }

            if (count == 0)
            {
                return Empty<TResult>();
            }

            return new RepeatIterator<TResult>(element, count);
        }

        /// <summary>
        /// An iterator that yields the same item multiple times. 
        /// </summary>
        /// <typeparam name="TResult">The type of the item.</typeparam>
        private sealed partial class RepeatIterator<TResult> : Iterator<TResult>, IList<TResult>, IReadOnlyList<TResult>
        {
            private readonly int _count;

            public RepeatIterator(TResult element, int count)
            {
                Debug.Assert(count > 0);
                _current = element;
                _count = count;
            }

           public int Count => _count;

            public bool IsReadOnly => true;

            public TResult this[int index]
            {
                get
                {
                    if ((uint)index >= (uint)_count)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
                    }
                    return _current;
                }
                
                set => ThrowHelper.ThrowNotSupportedException();
            }

            public void CopyTo(TResult[] array, int arrayIndex)
            {
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);

                if ((uint)arrayIndex >= (uint)array.Length)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex);

                unchecked
                {
                    if (array.Length - arrayIndex < Count)
                        ThrowHelper.ThrowArgumentArrayPlusOffTooSmall();

                    int end = arrayIndex + Count;
                    for (int index = arrayIndex; index < end; index++)
                    {
                        array[index] = _current;
                    }
                }
            }

            public bool Contains(TResult item) => EqualityComparer<TResult>.Default.Equals(_current, item);            

            public int IndexOf(TResult item) => EqualityComparer<TResult>.Default.Equals(_current, item) ? 0 : -1;      

            void ICollection<TResult>.Add(TResult item) => ThrowHelper.ThrowNotSupportedException();
            bool ICollection<TResult>.Remove(TResult item) => ThrowHelper.ThrowNotSupportedException<bool>();
            void ICollection<TResult>.Clear() => ThrowHelper.ThrowNotSupportedException();
            void IList<TResult>.Insert(int index, TResult item) => ThrowHelper.ThrowNotSupportedException();
            void IList<TResult>.RemoveAt(int index) => ThrowHelper.ThrowNotSupportedException();

            public override Iterator<TResult> Clone()
            {
                return new RepeatIterator<TResult>(_current, _count);
            }

            public override void Dispose()
            {
                // Don't let base.Dispose wipe Current.
                _state = -1;
            }

            public override bool MoveNext()
            {
                // Having a separate field for the number of sent items would be more readable.
                // However, we save it into _state with a bias to minimize field size of the iterator.
                int sent = _state - 1;

                // We can't have sent a negative number of items, obviously. However, if this iterator
                // was illegally casted to IEnumerator without GetEnumerator being called, or if we've
                // already been disposed, then `sent` will be negative.
                if (sent >= 0 && sent != _count)
                {
                    ++_state;
                    return true;
                }

                Dispose();
                return false;
            }
        }
    }
}
