// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private sealed partial class RepeatIterator<TResult> : IPartition<TResult>
        {
            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new SelectIPartitionIterator<TResult, TResult2>(this, selector);

            public TResult[] ToArray()
            {
                TResult[] array = new TResult[_count];
                if (_current is object) // not null
                {
                    Array.Fill(array, _current);
                }

                return array;
            }

            public List<TResult> ToList() => new List<TResult>(new ToListCollection(this));

            public int GetCount(bool onlyIfCheap) => _count;

            public IPartition<TResult> Skip(int count)
            {
                if (count >= _count)
                {
                    return EmptyPartition<TResult>.Instance;
                }

                return new RepeatIterator<TResult>(_current, _count - count);
            }

            public IPartition<TResult> Take(int count)
            {
                if (count >= _count)
                {
                    return this;
                }

                return new RepeatIterator<TResult>(_current, count);
            }

            public TResult TryGetElementAt(int index, out bool found)
            {
                if ((uint)index < (uint)_count)
                {
                    found = true;
                    return _current;
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetFirst(out bool found)
            {
                found = true;
                return _current;
            }

            public TResult TryGetLast(out bool found)
            {
                found = true;
                return _current;
            }

            private class ToListCollection : ICollection<TResult>
            {
                readonly TResult _current;
                readonly int _count;

                public ToListCollection(RepeatIterator<TResult> source)
                {
                    _current = source._current;
                    _count = source._count;
                }

                public int Count => _count;

                public bool IsReadOnly => true;

                public void CopyTo(TResult[] array, int _)
                {
                    if (_current is object) // not null
                    {
                        unchecked
                        {
                            for(int index = 0; index < _count; index++)
                            {
                                array[index] = _current;
                            }
                        }
                    }
                }

                IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
                IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => throw new NotSupportedException();
                void ICollection<TResult>.Add(TResult item) => throw new NotSupportedException();
                bool ICollection<TResult>.Remove(TResult item) => throw new NotSupportedException();
                void ICollection<TResult>.Clear() => throw new NotSupportedException();
                bool ICollection<TResult>.Contains(TResult item) => throw new NotSupportedException();
            }

        }
    }
}
