// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private sealed partial class RangeIterator : IPartition<int>
        {
            public override IEnumerable<TResult> Select<TResult>(Func<int, TResult> selector)
            {
                return new SelectIPartitionIterator<int, TResult>(this, selector);
            }

            public int[] ToArray()
            {
                int[] array = new int[_end - _start];
                int cur = _start;
                for (int i = 0; i != array.Length; ++i)
                {
                    array[i] = cur;
                    ++cur;
                }

                return array;
            }

            public List<int> ToList() => new List<int>(new ToListCollection(this));

            public int GetCount(bool onlyIfCheap) => unchecked(_end - _start);

            public IPartition<int> Skip(int count)
            {
                if (count >= _end - _start)
                {
                    return EmptyPartition<int>.Instance;
                }

                return new RangeIterator(_start + count, _end - _start - count);
            }

            public IPartition<int> Take(int count)
            {
                int curCount = _end - _start;
                if (count >= curCount)
                {
                    return this;
                }

                return new RangeIterator(_start, count);
            }

            public int TryGetElementAt(int index, out bool found)
            {
                if (unchecked((uint)index < (uint)(_end - _start)))
                {
                    found = true;
                    return _start + index;
                }

                found = false;
                return 0;
            }

            public int TryGetFirst(out bool found)
            {
                found = true;
                return _start;
            }

            public int TryGetLast(out bool found)
            {
                found = true;
                return _end - 1;
            }

            private class ToListCollection : ICollection<int>
            {
                readonly int _start;
                readonly int _count;

                public ToListCollection(RangeIterator source)
                {
                    _start = source._start;
                    _count = source._end - source._start;
                }

                public int Count => _count;

                public bool IsReadOnly => true;

                public void CopyTo(int[] array, int _)
                {
                    unchecked
                    {
                        for(int index = 0; index < _count; index++)
                        {
                            array[index] = _start + index;
                        }
                    }
                }

                IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
                IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotSupportedException();
                void ICollection<int>.Add(int item) => throw new NotSupportedException();
                bool ICollection<int>.Remove(int item) => throw new NotSupportedException();
                void ICollection<int>.Clear() => throw new NotSupportedException();
                bool ICollection<int>.Contains(int item) => throw new NotSupportedException();
            }

        }
    }
}
