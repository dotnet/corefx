﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > Int32.MaxValue) throw Error.ArgumentOutOfRange("count");
            if (count == 0) return new EmptyPartition<int>();
            return new RangeIterator(start, count);
        }

        private sealed class RangeIterator : Iterator<int>, IPartition<int>
        {
            private readonly int _start;
            private readonly int _end;

            public RangeIterator(int start, int count)
            {
                Debug.Assert(count > 0);
                _start = start;
                _end = start + count;
            }

            public override Iterator<int> Clone()
            {
                return new RangeIterator(_start, _end - _start);
            }

            public override bool MoveNext()
            {
                switch (state)
                {
                    case 1:
                        Debug.Assert(_start != _end);
                        current = _start;
                        state = 2;
                        return true;
                    case 2:
                        if (++current == _end) break;
                        return true;
                }
                state = -1;
                return false;
            }

            public override void Dispose()
            {
                state = -1; // Don't reset current
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

            public List<int> ToList()
            {
                List<int> list = new List<int>(_end - _start);
                for (int cur = _start; cur != _end; cur++)
                {
                    list.Add(cur);
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                return _end - _start;
            }

            public IPartition<int> Skip(int count)
            {
                if (count >= _end - _start) return new EmptyPartition<int>();
                return new RangeIterator(_start + count, _end - _start - count);
            }

            public IPartition<int> Take(int count)
            {
                int curCount = _end - _start;
                if (count > curCount) count = curCount;
                return new RangeIterator(_start, count);
            }

            public int ElementAt(int index)
            {
                if ((uint)index >= (uint)(_end - _start)) throw Error.ArgumentOutOfRange("index");
                return _start + index;
            }

            public int ElementAtOrDefault(int index)
            {
                return (uint)index >= (uint)(_end - _start) ? 0 : _start + index;
            }

            public int First()
            {
                return _start;
            }

            public int FirstOrDefault()
            {
                return _start;
            }

            public int Last()
            {
                return _end - 1;
            }

            public int LastOrDefault()
            {
                return _end - 1;
            }
        }
    }
}
