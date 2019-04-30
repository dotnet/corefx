// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
            }

            if (count == 0)
            {
                return Empty<int>();
            }

            return new RangeIterator(start, count);
        }

        /// <summary>
        /// An iterator that yields a range of consecutive integers.
        /// </summary>
        private sealed partial class RangeIterator : Iterator<int>, IList<int>, IReadOnlyList<int>
        {
            private readonly int _start;
            private readonly int _end;

            public RangeIterator(int start, int count)
            {
                Debug.Assert(count > 0);
                _start = start;
                _end = unchecked(start + count);
            }

            public int Count => unchecked(_end - _start);

            public bool IsReadOnly => true;

            public int this[int index]
            {
                get
                {
                    if ((uint)index >= (uint)Count)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
                    }
                    return unchecked(index + _start);
                }
                set => ThrowHelper.ThrowNotSupportedException();
            }

            public void CopyTo(int[] array, int arrayIndex)
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
                    for (int index = arrayIndex, value = _start; index < end; index++, value++)
                    {
                        array[index] = value;
                    }
                }
            }

            public bool Contains(int item) => item >= _start && item < _end;            

            public int IndexOf(int item)
            {
                if (item < _start || item >= _end)
                {
                    return -1;
                }
                return unchecked(item - _start);
            }            

            void ICollection<int>.Add(int item) => ThrowHelper.ThrowNotSupportedException();
            bool ICollection<int>.Remove(int item) => ThrowHelper.ThrowNotSupportedException<bool>(); 
            void ICollection<int>.Clear() => ThrowHelper.ThrowNotSupportedException();
            void IList<int>.Insert(int index, int item) => ThrowHelper.ThrowNotSupportedException();
            void IList<int>.RemoveAt(int index) => ThrowHelper.ThrowNotSupportedException();

            public override Iterator<int> Clone() => new RangeIterator(_start, _end - _start);

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        Debug.Assert(_start != _end);
                        _current = _start;
                        _state = 2;
                        return true;
                    case 2:
                        if (unchecked(++_current) == _end)
                        {
                            break;
                        }

                        return true;
                }

                _state = -1;
                return false;
            }

            public override void Dispose()
            {
                _state = -1; // Don't reset current
            }
        }
    }
}
