// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Linq.Utilities;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private sealed partial class ReverseArrayIterator<TSource> : IPartition<TSource>
        {
            public TSource[] ToArray()
            {
                // See assert in constructor.
                // Since _source should never be empty, we don't check for 0/return Array.Empty.
                Debug.Assert(_source.Length > 0);

                var results = (TSource[])_source.Clone();
                Array.Reverse(results);
                return results;
            }

            public List<TSource> ToList()
            {
                var results = new List<TSource>(_source);
                results.Reverse();
                return results;
            }

            public int GetCount(bool onlyIfCheap)
            {
                return _source.Length;
            }

            public IPartition<TSource> Skip(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Length
                    ? EmptyPartition<TSource>.Instance
                    : new ReverseListPartition<TSource>(_source, 0, _source.Length - count - 1);
            }

            public IPartition<TSource> Take(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Length
                    ? (IPartition<TSource>)this
                    : new ReverseListPartition<TSource>(_source, _source.Length - count, int.MaxValue);
            }

            public TSource TryGetElementAt(int index, out bool found)
            {
                if ((uint)index < (uint)_source.Length)
                {
                    found = true;
                    return _source[_source.Length - index - 1];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetFirst(out bool found)
            {
                Debug.Assert(_source.Length > 0); // See assert in constructor

                found = true;
                return _source[_source.Length - 1];
            }

            public TSource TryGetLast(out bool found)
            {
                Debug.Assert(_source.Length > 0); // See assert in constructor

                found = true;
                return _source[0];
            }
        }

        private sealed partial class ReverseSelectArrayIterator<TSource, TResult> : IPartition<TResult>
        {
            public TResult[] ToArray()
            {
                // See assert in constructor.
                // Since _source should never be empty, we don't check for 0/return Array.Empty.
                Debug.Assert(_source.Length > 0);

                var results = new TResult[_source.Length];
                for (int i = 0, j = results.Length - 1; i < results.Length; i++, j--)
                {
                    results[i] = _selector(_source[j]);
                }

                return results;
            }

            public List<TResult> ToList()
            {
                var results = new List<TResult>(_source.Length);
                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    results.Add(_selector(_source[i]));
                }

                return results;
            }

            public int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                if (!onlyIfCheap)
                {
                    for (int i = _source.Length - 1; i >= 0; i--)
                    {
                        _selector(_source[i]);
                    }
                }

                return _source.Length;
            }

            public IPartition<TResult> Skip(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Length
                    ? EmptyPartition<TResult>.Instance
                    : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, 0, _source.Length - count - 1);
            }

            public IPartition<TResult> Take(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Length
                    ? (IPartition<TResult>)this
                    : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, _source.Length - count, int.MaxValue);
            }

            public TResult TryGetElementAt(int index, out bool found)
            {
                if ((uint)index < (uint)_source.Length)
                {
                    found = true;
                    return _selector(_source[_source.Length - index - 1]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetFirst(out bool found)
            {
                Debug.Assert(_source.Length > 0); // See assert in constructor

                found = true;
                return _selector(_source[_source.Length - 1]);
            }

            public TResult TryGetLast(out bool found)
            {
                Debug.Assert(_source.Length > 0); // See assert in constructor

                found = true;
                return _selector(_source[0]);
            }
        }

        private sealed partial class ReverseListIterator<TSource> : IPartition<TSource>
        {
            public TSource[] ToArray()
            {
                int count = _source.Count;
                if (count == 0)
                {
                    return Array.Empty<TSource>();
                }

                var results = _source.ToArray();
                Array.Reverse(results);
                return results;
            }

            public List<TSource> ToList()
            {
                var results = new List<TSource>(_source);
                results.Reverse();
                return results;
            }

            public int GetCount(bool onlyIfCheap)
            {
                return _source.Count;
            }

            public IPartition<TSource> Skip(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? EmptyPartition<TSource>.Instance
                    : new ReverseListPartition<TSource>(_source, 0, _source.Count - count - 1);
            }

            public IPartition<TSource> Take(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? (IPartition<TSource>)this
                    : new ReverseListPartition<TSource>(_source, _source.Count - count, int.MaxValue);
            }

            public TSource TryGetElementAt(int index, out bool found)
            {
                int count = _source.Count;
                if ((uint)index < (uint)count)
                {
                    found = true;
                    return _source[count - index - 1];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetFirst(out bool found)
            {
                int len = _source.Count;
                if (len != 0)
                {
                    found = true;
                    return _source[len - 1];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetLast(out bool found)
            {
                if (_source.Count != 0)
                {
                    found = true;
                    return _source[0];
                }

                found = false;
                return default(TSource);
            }
        }

        private sealed partial class ReverseSelectListIterator<TSource, TResult> : IPartition<TResult>
        {
            public TResult[] ToArray()
            {
                int count = _source.Count;
                if (count == 0)
                {
                    return Array.Empty<TResult>();
                }

                var results = new TResult[count];
                for (int i = 0, j = results.Length - 1; i < results.Length; i++, j--)
                {
                    results[i] = _selector(_source[j]);
                }

                return results;
            }

            public List<TResult> ToList()
            {
                int count = _source.Count;
                var results = new List<TResult>(count);
                for (int i = count - 1; i >= 0; i--)
                {
                    results.Add(_selector(_source[i]));
                }

                return results;
            }

            public int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                int count = _source.Count;

                if (!onlyIfCheap)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        _selector(_source[i]);
                    }
                }

                return count;
            }

            public IPartition<TResult> Skip(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? EmptyPartition<TResult>.Instance
                    : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, 0, _source.Count - count - 1);
            }

            public IPartition<TResult> Take(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? (IPartition<TResult>)this
                    : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, _source.Count - count, int.MaxValue);
            }

            public TResult TryGetElementAt(int index, out bool found)
            {
                int count = _source.Count;
                if ((uint)index < (uint)count)
                {
                    found = true;
                    return _selector(_source[count - index - 1]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetFirst(out bool found)
            {
                int len = _source.Count;
                if (len != 0)
                {
                    found = true;
                    return _selector(_source[len - 1]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetLast(out bool found)
            {
                if (_source.Count != 0)
                {
                    found = true;
                    return _selector(_source[0]);
                }

                found = false;
                return default(TResult);
            }
        }

        private sealed partial class ReverseIListIterator<TSource> : IPartition<TSource>
        {
            public TSource[] ToArray()
            {
                int count = _source.Count;
                if (count == 0)
                {
                    return Array.Empty<TSource>();
                }

                var results = new TSource[count];
                _source.CopyTo(results, 0);
                Array.Reverse(results);
                return results;
            }

            public List<TSource> ToList()
            {
                var results = new List<TSource>(_source);
                results.Reverse();
                return results;
            }

            public int GetCount(bool onlyIfCheap)
            {
                return _source.Count;
            }

            public IPartition<TSource> Skip(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? EmptyPartition<TSource>.Instance
                    : new ReverseListPartition<TSource>(_source, 0, _source.Count - count - 1);
            }

            public IPartition<TSource> Take(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? (IPartition<TSource>)this
                    : new ReverseListPartition<TSource>(_source, _source.Count - count, int.MaxValue);
            }

            public TSource TryGetElementAt(int index, out bool found)
            {
                int count = _source.Count;
                if ((uint)index < (uint)count)
                {
                    found = true;
                    return _source[count - index - 1];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetFirst(out bool found)
            {
                int len = _source.Count;
                if (len != 0)
                {
                    found = true;
                    return _source[len - 1];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetLast(out bool found)
            {
                if (_source.Count != 0)
                {
                    found = true;
                    return _source[0];
                }

                found = false;
                return default(TSource);
            }
        }

        private sealed partial class ReverseSelectIListIterator<TSource, TResult> : IPartition<TResult>
        {
            public TResult[] ToArray()
            {
                int count = _source.Count;
                if (count == 0)
                {
                    return Array.Empty<TResult>();
                }

                var results = new TResult[count];
                for (int i = 0, j = results.Length - 1; i < results.Length; i++, j--)
                {
                    results[i] = _selector(_source[j]);
                }

                return results;
            }

            public List<TResult> ToList()
            {
                int count = _source.Count;
                var results = new List<TResult>(count);
                for (int i = count - 1; i >= 0; i--)
                {
                    results.Add(_selector(_source[i]));
                }

                return results;
            }

            public int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                int count = _source.Count;

                if (!onlyIfCheap)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        _selector(_source[i]);
                    }
                }

                return count;
            }

            public IPartition<TResult> Skip(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? EmptyPartition<TResult>.Instance
                    : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, 0, _source.Count - count - 1);
            }

            public IPartition<TResult> Take(int count)
            {
                Debug.Assert(count > 0);
                return count >= _source.Count
                    ? (IPartition<TResult>)this
                    : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, _source.Count - count, int.MaxValue);
            }

            public TResult TryGetElementAt(int index, out bool found)
            {
                int count = _source.Count;
                if ((uint)index < (uint)count)
                {
                    found = true;
                    return _selector(_source[count - index - 1]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetFirst(out bool found)
            {
                int len = _source.Count;
                if (len != 0)
                {
                    found = true;
                    return _selector(_source[len - 1]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetLast(out bool found)
            {
                if (_source.Count != 0)
                {
                    found = true;
                    return _selector(_source[0]);
                }

                found = false;
                return default(TResult);
            }
        }

        private sealed partial class ReverseEnumerableIterator<TSource> : IIListProvider<TSource>
        {
            public TSource[] ToArray()
            {
                TSource[] array = _source.ToArray();
                Array.Reverse(array);
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
                    switch (_source)
                    {
                        case IIListProvider<TSource> listProv:
                            return listProv.GetCount(onlyIfCheap: true);

                        case ICollection<TSource> colT:
                            return colT.Count;

                        case ICollection col:
                            return col.Count;

                        default:
                            return -1;
                    }
                }

                return _source.Count();
            }
        }

        private sealed class ReverseListPartition<TSource> : Iterator<TSource>, IPartition<TSource>, IReverseProvider<TSource>
        {
            private readonly IList<TSource> _source;
            private readonly int _minIndexInclusive;
            private readonly int _maxIndexInclusive;

            public ReverseListPartition(IList<TSource> source, int minIndexInclusive, int maxIndexInclusive)
            {
                Debug.Assert(source != null);
                Debug.Assert(minIndexInclusive >= 0);
                Debug.Assert(minIndexInclusive <= maxIndexInclusive);
                _source = source;
                _minIndexInclusive = minIndexInclusive;
                _maxIndexInclusive = maxIndexInclusive;
            }

            public override Iterator<TSource> Clone() =>
                new ReverseListPartition<TSource>(_source, _minIndexInclusive, _maxIndexInclusive);

            public override bool MoveNext()
            {
                // _state - 1 represents the zero-based index into the list.
                // Having a separate field for the index would be more readable. However, we save it
                // into _state with a bias to minimize field size of the iterator.
                int index = _state - 1;
                int count = _source.Count;
                if (unchecked((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive) && index < count - _minIndexInclusive))
                {
                    _current = _source[Math.Min(count - 1, _maxIndexInclusive) - index];
                    ++_state;
                    return true;
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) =>
                new ReverseSelectListPartitionIterator<TSource, TResult>(_source, selector, _minIndexInclusive, _maxIndexInclusive);

            public IEnumerable<TSource> Reverse() =>
                new ListPartition<TSource>(_source, _minIndexInclusive, _maxIndexInclusive);

            public TSource[] ToArray()
            {
                int count = Count;
                if (count == 0)
                {
                    return Array.Empty<TSource>();
                }

                TSource[] array = new TSource[count];
                for (int i = 0, curIdx = Math.Min(_source.Count - 1, _maxIndexInclusive); i != array.Length; ++i, --curIdx)
                {
                    array[i] = _source[curIdx];
                }

                return array;
            }

            public List<TSource> ToList()
            {
                int count = Count;
                if (count == 0)
                {
                    return new List<TSource>();
                }

                List<TSource> list = new List<TSource>(count);
                int end = _minIndexInclusive - 1;
                for (int i = Math.Min(_source.Count - 1, _maxIndexInclusive); i != end; --i)
                {
                    list.Add(_source[i]);
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap) => Count;

            public IPartition<TSource> Skip(int count)
            {
                Debug.Assert(count > 0);
                int maxIndex = Math.Min(_source.Count - 1, _maxIndexInclusive) - count;
                return maxIndex < _minIndexInclusive ? EmptyPartition<TSource>.Instance : new ReverseListPartition<TSource>(_source, _minIndexInclusive, maxIndex);
            }

            public IPartition<TSource> Take(int count)
            {
                Debug.Assert(count > 0);
                int minIndex = Math.Min(_source.Count - 1, _maxIndexInclusive) - count + 1;
                return minIndex <= _minIndexInclusive ? this : new ReverseListPartition<TSource>(_source, minIndex, _maxIndexInclusive);
            }

            public TSource TryGetElementAt(int index, out bool found)
            {
                int count = _source.Count;
                if ((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive) && index < count - _minIndexInclusive)
                {
                    found = true;
                    return _source[Math.Min(count - 1, _maxIndexInclusive) - index];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetFirst(out bool found)
            {
                int lastIndex = _source.Count - 1;
                if (lastIndex >= _minIndexInclusive)
                {
                    found = true;
                    return _source[Math.Min(lastIndex, _maxIndexInclusive)];
                }

                found = false;
                return default(TSource);
            }

            public TSource TryGetLast(out bool found)
            {
                if (_source.Count > _minIndexInclusive)
                {
                    found = true;
                    return _source[_minIndexInclusive];
                }

                found = false;
                return default(TSource);
            }

            private int Count
            {
                get
                {
                    int count = _source.Count;
                    if (count <= _minIndexInclusive)
                    {
                        return 0;
                    }

                    return Math.Min(count - 1, _maxIndexInclusive) - _minIndexInclusive + 1;
                }
            }
        }

        private sealed class ReverseSelectListPartitionIterator<TSource, TResult> : Iterator<TResult>, IPartition<TResult>, IReverseProvider<TResult>
        {
            private readonly IList<TSource> _source;
            private readonly Func<TSource, TResult> _selector;
            private readonly int _minIndexInclusive;
            private readonly int _maxIndexInclusive;

            public ReverseSelectListPartitionIterator(IList<TSource> source, Func<TSource, TResult> selector, int minIndexInclusive, int maxIndexInclusive)
            {
                Debug.Assert(source != null);
                Debug.Assert(selector != null);
                Debug.Assert(minIndexInclusive >= 0);
                Debug.Assert(minIndexInclusive <= maxIndexInclusive);
                _source = source;
                _selector = selector;
                _minIndexInclusive = minIndexInclusive;
                _maxIndexInclusive = maxIndexInclusive;
            }

            public override Iterator<TResult> Clone() =>
                new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, _minIndexInclusive, _maxIndexInclusive);

            public override bool MoveNext()
            {
                // _state - 1 represents the zero-based index into the list.
                // Having a separate field for the index would be more readable. However, we save it
                // into _state with a bias to minimize field size of the iterator.
                int index = _state - 1;
                int count = _source.Count;
                if (unchecked((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive) && index < count - _minIndexInclusive))
                {
                    _current = _selector(_source[Math.Min(count - 1, _maxIndexInclusive) - index]);
                    ++_state;
                    return true;
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new ReverseSelectListPartitionIterator<TSource, TResult2>(_source, CombineSelectors(_selector, selector), _minIndexInclusive, _maxIndexInclusive);

            public IEnumerable<TResult> Reverse() =>
                new SelectListPartitionIterator<TSource, TResult>(_source, _selector, _minIndexInclusive, _maxIndexInclusive);

            public TResult[] ToArray()
            {
                int count = Count;
                if (count == 0)
                {
                    return Array.Empty<TResult>();
                }

                TResult[] array = new TResult[count];
                for (int i = 0, curIdx = Math.Min(_source.Count - 1, _maxIndexInclusive); i != array.Length; ++i, --curIdx)
                {
                    array[i] = _selector(_source[curIdx]);
                }

                return array;
            }

            public List<TResult> ToList()
            {
                int count = Count;
                if (count == 0)
                {
                    return new List<TResult>();
                }

                List<TResult> list = new List<TResult>(count);
                int end = _minIndexInclusive - 1;
                for (int i = Math.Min(_source.Count - 1, _maxIndexInclusive); i != end; --i)
                {
                    list.Add(_selector(_source[i]));
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                int count = Count;

                if (!onlyIfCheap)
                {
                    int end = _minIndexInclusive - 1;
                    for (int i = Math.Min(_source.Count - 1, _maxIndexInclusive); i != end; --i)
                    {
                        _selector(_source[i]);
                    }
                }

                return count;
            }

            public IPartition<TResult> Skip(int count)
            {
                Debug.Assert(count > 0);
                int maxIndex = Math.Min(_source.Count - 1, _maxIndexInclusive) - count;
                return maxIndex < _minIndexInclusive ? EmptyPartition<TResult>.Instance : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, _minIndexInclusive, maxIndex);
            }

            public IPartition<TResult> Take(int count)
            {
                Debug.Assert(count > 0);
                int minIndex = Math.Min(_source.Count - 1, _maxIndexInclusive) - count + 1;
                return minIndex <= _minIndexInclusive ? this : new ReverseSelectListPartitionIterator<TSource, TResult>(_source, _selector, minIndex, _maxIndexInclusive);
            }

            public TResult TryGetElementAt(int index, out bool found)
            {
                int count = _source.Count;
                if ((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive) && index < count - _minIndexInclusive)
                {
                    found = true;
                    return _selector(_source[Math.Min(count - 1, _maxIndexInclusive) - index]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetFirst(out bool found)
            {
                int lastIndex = _source.Count - 1;
                if (lastIndex >= _minIndexInclusive)
                {
                    found = true;
                    return _selector(_source[Math.Min(lastIndex, _maxIndexInclusive)]);
                }

                found = false;
                return default(TResult);
            }

            public TResult TryGetLast(out bool found)
            {
                if (_source.Count > _minIndexInclusive)
                {
                    found = true;
                    return _selector(_source[_minIndexInclusive]);
                }

                found = false;
                return default(TResult);
            }

            private int Count
            {
                get
                {
                    int count = _source.Count;
                    if (count <= _minIndexInclusive)
                    {
                        return 0;
                    }

                    return Math.Min(count - 1, _maxIndexInclusive) - _minIndexInclusive + 1;
                }
            }
        }

        internal sealed partial class ReverseWhereArrayIterator<TSource> : IIListProvider<TSource>
        {
            public TSource[] ToArray()
            {
                var builder = new LargeArrayBuilder<TSource>(_source.Length);

                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(item);
                    }
                }

                return builder.ToArray();
            }

            public List<TSource> ToList()
            {
                var list = new List<TSource>();

                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        checked
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        private sealed partial class ReverseWhereListIterator<TSource> : Iterator<TSource>, IIListProvider<TSource>
        {
            public TSource[] ToArray()
            {
                var builder = new LargeArrayBuilder<TSource>(_source.Count);

                for (int i = _source.Count - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(item);
                    }
                }

                return builder.ToArray();
            }

            public List<TSource> ToList()
            {
                var list = new List<TSource>();

                for (int i = _source.Count - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                for (int i = _source.Count - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        checked
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        private sealed partial class ReverseWhereSelectArrayIterator<TSource, TResult> : IIListProvider<TResult>
        {
            public TResult[] ToArray()
            {
                var builder = new LargeArrayBuilder<TResult>(_source.Length);

                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(_selector(item));
                    }
                }

                return builder.ToArray();
            }

            public List<TResult> ToList()
            {
                var list = new List<TResult>();

                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(_selector(item));
                    }
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                for (int i = _source.Length - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        _selector(item);
                        checked
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        private sealed partial class ReverseWhereSelectListIterator<TSource, TResult> : IIListProvider<TResult>
        {
            public TResult[] ToArray()
            {
                var builder = new LargeArrayBuilder<TResult>(_source.Count);

                for (int i = _source.Count - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(_selector(item));
                    }
                }

                return builder.ToArray();
            }

            public List<TResult> ToList()
            {
                var list = new List<TResult>();

                for (int i = _source.Count - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(_selector(item));
                    }
                }

                return list;
            }

            public int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                for (int i = _source.Count - 1; i >= 0; i--)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        _selector(item);
                        checked
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }
    }
}
