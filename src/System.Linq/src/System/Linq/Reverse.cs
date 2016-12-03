﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return new ReverseIterator<TSource>(source);
        }

        private sealed class ReverseIterator<TSource> : Iterator<TSource>, IPartition<TSource>
        {
            private readonly IEnumerable<TSource> _source;
            private TSource[] _buffer;
            private int _index;

            public ReverseIterator(IEnumerable<TSource> source)
            {
                Debug.Assert(source != null);
                _source = source;
            }

            public override Iterator<TSource> Clone()
            {
                return new ReverseIterator<TSource>(_source);
            }

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        Buffer<TSource> buffer = new Buffer<TSource>(_source);
                        _buffer = buffer._items;
                        _index = buffer._count - 1;
                        _state = 2;
                        goto case 2;
                    case 2:
                        if (_index != -1)
                        {
                            _current = _buffer[_index];
                            --_index;
                            return true;
                        }

                        break;
                }

                Dispose();
                return false;
            }

            public override void Dispose()
            {
                _buffer = null; // Just in case this ends up being long-lived, allow the memory to be reclaimed.
                base.Dispose();
            }

            public TSource[] ToArray()
            {
                TSource[] array = _source.ToArray();

                // Array.Reverse() involves boxing for non-primitive value types, but
                // checking that has its own cost, so just use this approach for all types.
                for (int i = 0, j = array.Length - 1; i < j; ++i, --j)
                {
                    TSource temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }

                return array;
            }

            public List<TSource> ToList()
            {
                List<TSource> list = _source.ToList();
                list.Reverse();
                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    IIListProvider<TSource> listProv = _source as IIListProvider<TSource>;
                    if (listProv != null)
                    {
                        return listProv.GetCount(onlyIfCheap: true);
                    }

                    if (!(_source is ICollection<TSource>) && !(_source is ICollection))
                    {
                        return -1;
                    }
                }

                return _source.Count();
            }

            public IPartition<TSource> Skip(int count)
            {
                return new EnumerablePartition<TSource>(this, count, -1);
            }

            public IPartition<TSource> Take(int count)
            {
                return new EnumerablePartition<TSource>(this, 0, count - 1);
            }

            public TSource TryGetElementAt(int index, out bool found)
            {
                if ((uint)index < (uint)_source.Length)
                {
                    found = true;
                    return _selector(_source[index]);
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetFirst(out bool found)
            {
                Debug.Assert(_source.Length > 0); // See assert in constructor

                found = true;
                return _selector(_source[0]);
            }

            public TSource TryGetLast(out bool found)
            {
                using (IEnumerator<TSource> en = _source.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        found = true;
                        return en.Current;
                    }
                }

                found = false;
                return default(TSource);
            }
        }
    }
}
