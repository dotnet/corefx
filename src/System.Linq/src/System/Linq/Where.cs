// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using static System.Linq.Utilities;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            Iterator<TSource> iterator = source as Iterator<TSource>;
            if (iterator != null)
            {
                return iterator.Where(predicate);
            }

            IList<TSource> ilist = source as IList<TSource>;
            if (ilist != null)
            {
                TSource[] array = source as TSource[];
                if (array != null)
                {
                    return array.Length == 0 ?
                        (IEnumerable<TSource>)EmptyPartition<TSource>.Instance :
                        new WhereArrayIterator<TSource>(array, predicate);
                }

                List<TSource> list = source as List<TSource>;
                if (list != null)
                {
                    return new WhereListIterator<TSource>(list, predicate);
                }

                return new WhereIListIterator<TSource>(ilist, predicate);
            }

            return new WhereEnumerableIterator<TSource>(source, predicate);
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            return WhereIndexIterator(source, predicate);
        }

        private static IEnumerable<TSource> WhereIndexIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            int index = -1;
            foreach (TSource element in source)
            {
                checked
                {
                    index++;
                }

                if (predicate(element, index))
                {
                    yield return element;
                }
            }
        }

        internal abstract class WhereIterator<TSource> : Iterator<TSource>, IPartition<TSource>
        {
            public abstract int GetCount(bool onlyIfCheap);

            public abstract override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector);

            public IPartition<TSource> Skip(int count) => new EnumerablePartition<TSource>(this, count, -1);

            public IPartition<TSource> Take(int count) => new EnumerablePartition<TSource>(this, 0, count - 1);

            public abstract TSource[] ToArray();

            public abstract List<TSource> ToList();

            public TSource TryGetElementAt(int index, out bool found) => EnumerableHelpers.TryGetElementAt(index, out found, source: this);

            public abstract TSource TryGetFirst(out bool found);

            public abstract TSource TryGetLast(out bool found);
        }

        /// <summary>
        /// An iterator that filters each item of an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        internal sealed class WhereEnumerableIterator<TSource> : WhereIterator<TSource>
        {
            private readonly IEnumerable<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private IEnumerator<TSource> _enumerator;

            public WhereEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate)
            {
                Debug.Assert(source != null && !(source is IList<TSource>));
                Debug.Assert(predicate != null);
                _source = source;
                _predicate = predicate;
            }

            public override Iterator<TSource> Clone()
            {
                return new WhereEnumerableIterator<TSource>(_source, _predicate);
            }

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }

                base.Dispose();
            }

            public override int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                foreach (TSource item in _source)
                {
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

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            if (_predicate(item))
                            {
                                _current = item;
                                return true;
                            }
                        }

                        Dispose();
                        break;
                }

                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
            {
                return new WhereSelectEnumerableIterator<TSource, TResult>(_source, _predicate, selector);
            }

            public override TSource[] ToArray()
            {
                var builder = new LargeArrayBuilder<TSource>(initialize: true);

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        builder.Add(item);
                    }
                }

                return builder.ToArray();
            }

            public override List<TSource> ToList()
            {
                var list = new List<TSource>();

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }

            public override TSource TryGetFirst(out bool found) => EnumerableHelpers.TryGetFirst(_predicate, out found, source: _source);

            public override TSource TryGetLast(out bool found) => EnumerableHelpers.TryGetLast(_predicate, out found, source: _source);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
            {
                return new WhereEnumerableIterator<TSource>(_source, CombinePredicates(_predicate, predicate));
            }
        }

        /// <summary>
        /// An iterator that filters each item of a <see cref="T:TSource[]"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source array.</typeparam>
        internal sealed class WhereArrayIterator<TSource> : WhereIterator<TSource>
        {
            private readonly TSource[] _source;
            private readonly Func<TSource, bool> _predicate;

            public WhereArrayIterator(TSource[] source, Func<TSource, bool> predicate)
            {
                Debug.Assert(source?.Length > 0);
                Debug.Assert(predicate != null);
                _source = source;
                _predicate = predicate;
            }

            public override Iterator<TSource> Clone()
            {
                return new WhereArrayIterator<TSource>(_source, _predicate);
            }

            public override int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                foreach (TSource item in _source)
                {
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

            public override bool MoveNext()
            {
                int index = _state - 1;
                TSource[] source = _source;

                while (unchecked((uint)index < (uint)source.Length))
                {
                    TSource item = source[index];
                    index = _state++;
                    if (_predicate(item))
                    {
                        _current = item;
                        return true;
                    }
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
            {
                return new WhereSelectArrayIterator<TSource, TResult>(_source, _predicate, selector);
            }

            public override TSource[] ToArray()
            {
                var builder = new LargeArrayBuilder<TSource>(_source.Length);

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        builder.Add(item);
                    }
                }

                return builder.ToArray();
            }

            public override List<TSource> ToList()
            {
                var list = new List<TSource>();

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }

            public override TSource TryGetFirst(out bool found) => EnumerableHelpers.TryGetFirst(_predicate, out found, array: _source);

            public override TSource TryGetLast(out bool found) => EnumerableHelpers.TryGetLast(_predicate, out found, array: _source);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
            {
                return new WhereArrayIterator<TSource>(_source, CombinePredicates(_predicate, predicate));
            }
        }

        /// <summary>
        /// An iterator that filters each item of a <see cref="List{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source list.</typeparam>
        internal sealed class WhereListIterator<TSource> : WhereIterator<TSource>
        {
            private readonly List<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private List<TSource>.Enumerator _enumerator;

            public WhereListIterator(List<TSource> source, Func<TSource, bool> predicate)
            {
                Debug.Assert(source != null);
                Debug.Assert(predicate != null);
                _source = source;
                _predicate = predicate;
            }

            public override Iterator<TSource> Clone()
            {
                return new WhereListIterator<TSource>(_source, _predicate);
            }

            public override int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                for (int i = 0; i < _source.Count; i++)
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

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            if (_predicate(item))
                            {
                                _current = item;
                                return true;
                            }
                        }

                        Dispose();
                        break;
                }

                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
            {
                return new WhereSelectListIterator<TSource, TResult>(_source, _predicate, selector);
            }

            public override TSource[] ToArray()
            {
                var builder = new LargeArrayBuilder<TSource>(_source.Count);

                for (int i = 0; i < _source.Count; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(item);
                    }
                }

                return builder.ToArray();
            }

            public override List<TSource> ToList()
            {
                var list = new List<TSource>();

                for (int i = 0; i < _source.Count; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }

            public override TSource TryGetFirst(out bool found) => EnumerableHelpers.TryGetFirst(_predicate, out found, list: _source);

            public override TSource TryGetLast(out bool found) => EnumerableHelpers.TryGetLast(_predicate, out found, list: _source);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
            {
                return new WhereListIterator<TSource>(_source, CombinePredicates(_predicate, predicate));
            }
        }

        internal sealed class WhereIListIterator<TSource> : WhereIterator<TSource>
        {
            private readonly IList<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private IEnumerator<TSource> _enumerator;

            public WhereIListIterator(IList<TSource> source, Func<TSource, bool> predicate)
            {
                Debug.Assert(source != null && !(source is TSource[] || source is List<TSource>));
                Debug.Assert(predicate != null);
                _source = source;
                _predicate = predicate;
            }                

            public override Iterator<TSource> Clone()
            {
                return new WhereIListIterator<TSource>(_source, _predicate);
            }

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }

                base.Dispose();
            }

            public override int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }
                
                int count = 0;
                int maxCount = _source.Count;

                for (int i = 0; i < maxCount; i++)
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

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            if (_predicate(item))
                            {
                                _current = item;
                                return true;
                            }
                        }

                        break;
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
            {
                return new WhereSelectIListIterator<TSource, TResult>(_source, _predicate, selector);
            }

            public override TSource[] ToArray()
            {
                int maxCount = _source.Count;
                var builder = new LargeArrayBuilder<TSource>(maxCount);

                for (int i = 0; i < maxCount; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(item);
                    }
                }

                return builder.ToArray();
            }

            public override List<TSource> ToList()
            {
                int maxCount = _source.Count;
                var list = new List<TSource>();

                for (int i = 0; i < maxCount; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }

            public override TSource TryGetFirst(out bool found) => EnumerableHelpers.TryGetFirst(_predicate, out found, ilist: _source);

            public override TSource TryGetLast(out bool found) => EnumerableHelpers.TryGetLast(_predicate, out found, ilist: _source);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
            {
                return new WhereIListIterator<TSource>(_source, CombinePredicates(_predicate, predicate));
            }
        }

        /// <summary>
        /// An iterator that filters, then maps, each item of a <see cref="T:TSource[]"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source array.</typeparam>
        /// <typeparam name="TResult">The type of the mapped items.</typeparam>
        internal sealed class WhereSelectArrayIterator<TSource, TResult> : WhereIterator<TResult>
        {
            private readonly TSource[] _source;
            private readonly Func<TSource, bool> _predicate;
            private readonly Func<TSource, TResult> _selector;

            public WhereSelectArrayIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                Debug.Assert(source?.Length > 0);
                Debug.Assert(predicate != null);
                Debug.Assert(selector != null);
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }

            public override Iterator<TResult> Clone()
            {
                return new WhereSelectArrayIterator<TSource, TResult>(_source, _predicate, _selector);
            }

            public override int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                foreach (TSource item in _source)
                {
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

            public override bool MoveNext()
            {
                int index = _state - 1;
                TSource[] source = _source;

                while (unchecked((uint)index < (uint)source.Length))
                {
                    TSource item = source[index];
                    index = _state++;
                    if (_predicate(item))
                    {
                        _current = _selector(item);
                        return true;
                    }
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
            {
                return new WhereSelectArrayIterator<TSource, TResult2>(_source, _predicate, CombineSelectors(_selector, selector));
            }

            public override TResult[] ToArray()
            {
                var builder = new LargeArrayBuilder<TResult>(_source.Length);

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        builder.Add(_selector(item));
                    }
                }

                return builder.ToArray();
            }

            public override List<TResult> ToList()
            {
                var list = new List<TResult>();

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        list.Add(_selector(item));
                    }
                }

                return list;
            }

            public override TResult TryGetFirst(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetFirst(_predicate, out found, array: _source);
                return found ? _selector(item) : default(TResult);
            }

            public override TResult TryGetLast(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetLast(_predicate, out found, array: _source);
                return found ? _selector(item) : default(TResult);
            }
        }

        /// <summary>
        /// An iterator that filters, then maps, each item of a <see cref="List{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source list.</typeparam>
        /// <typeparam name="TResult">The type of the mapped items.</typeparam>
        internal sealed class WhereSelectListIterator<TSource, TResult> : WhereIterator<TResult>
        {
            private readonly List<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private readonly Func<TSource, TResult> _selector;
            private List<TSource>.Enumerator _enumerator;

            public WhereSelectListIterator(List<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null);
                Debug.Assert(predicate != null);
                Debug.Assert(selector != null);
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }

            public override Iterator<TResult> Clone()
            {
                return new WhereSelectListIterator<TSource, TResult>(_source, _predicate, _selector);
            }

            public override int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                for (int i = 0; i < _source.Count; i++)
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

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            if (_predicate(item))
                            {
                                _current = _selector(item);
                                return true;
                            }
                        }

                        Dispose();
                        break;
                }

                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
            {
                return new WhereSelectListIterator<TSource, TResult2>(_source, _predicate, CombineSelectors(_selector, selector));
            }

            public override TResult[] ToArray()
            {
                var builder = new LargeArrayBuilder<TResult>(_source.Count);

                for (int i = 0; i < _source.Count; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(_selector(item));
                    }
                }

                return builder.ToArray();
            }

            public override List<TResult> ToList()
            {
                var list = new List<TResult>();

                for (int i = 0; i < _source.Count; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(_selector(item));
                    }
                }

                return list;
            }

            public override TResult TryGetFirst(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetFirst(_predicate, out found, list: _source);
                return found ? _selector(item) : default(TResult);
            }

            public override TResult TryGetLast(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetLast(_predicate, out found, list: _source);
                return found ? _selector(item) : default(TResult);
            }
        }

        /// <summary>
        /// An iterator that filters, then maps, each item of an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        /// <typeparam name="TResult">The type of the mapped items.</typeparam>
        internal sealed class WhereSelectEnumerableIterator<TSource, TResult> : WhereIterator<TResult>
        {
            private readonly IEnumerable<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private readonly Func<TSource, TResult> _selector;
            private IEnumerator<TSource> _enumerator;

            public WhereSelectEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null && !(source is IList<TSource>));
                Debug.Assert(predicate != null);
                Debug.Assert(selector != null);
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }

            public override Iterator<TResult> Clone()
            {
                return new WhereSelectEnumerableIterator<TSource, TResult>(_source, _predicate, _selector);
            }

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }

                base.Dispose();
            }

            public override int GetCount(bool onlyIfCheap)
            {
                // In case someone uses Count() to force evaluation of
                // the selector, run it provided `onlyIfCheap` is false.

                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;

                foreach (TSource item in _source)
                {
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

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            if (_predicate(item))
                            {
                                _current = _selector(item);
                                return true;
                            }
                        }

                        Dispose();
                        break;
                }

                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
            {
                return new WhereSelectEnumerableIterator<TSource, TResult2>(_source, _predicate, CombineSelectors(_selector, selector));
            }

            public override TResult[] ToArray()
            {
                var builder = new LargeArrayBuilder<TResult>(initialize: true);

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        builder.Add(_selector(item));
                    }
                }

                return builder.ToArray();
            }

            public override List<TResult> ToList()
            {
                var list = new List<TResult>();

                foreach (TSource item in _source)
                {
                    if (_predicate(item))
                    {
                        list.Add(_selector(item));
                    }
                }

                return list;
            }

            public override TResult TryGetFirst(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetFirst(_predicate, out found, source: _source);
                return found ? _selector(item) : default(TResult);
            }

            public override TResult TryGetLast(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetLast(_predicate, out found, source: _source);
                return found ? _selector(item) : default(TResult);
            }
        }

        internal sealed class WhereSelectIListIterator<TSource, TResult> : WhereIterator<TResult>
        {
            private readonly IList<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private readonly Func<TSource, TResult> _selector;
            private IEnumerator<TSource> _enumerator;

            public WhereSelectIListIterator(IList<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null && !(source is TSource[] || source is List<TSource>));
                Debug.Assert(predicate != null);
                Debug.Assert(selector != null);
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }                

            public override Iterator<TResult> Clone()
            {
                return new WhereSelectIListIterator<TSource, TResult>(_source, _predicate, _selector);
            }

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }

                base.Dispose();
            }

            public override int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }
                
                int count = 0;
                int maxCount = _source.Count;

                for (int i = 0; i < maxCount; i++)
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

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            if (_predicate(item))
                            {
                                _current = _selector(item);
                                return true;
                            }
                        }

                        break;
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
            {
                return new WhereSelectIListIterator<TSource, TResult2>(_source, _predicate, CombineSelectors(_selector, selector));
            }

            public override TResult[] ToArray()
            {
                int maxCount = _source.Count;
                var builder = new LargeArrayBuilder<TResult>(maxCount);

                for (int i = 0; i < maxCount; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        builder.Add(_selector(item));
                    }
                }

                return builder.ToArray();
            }

            public override List<TResult> ToList()
            {
                int maxCount = _source.Count;
                var list = new List<TResult>();

                for (int i = 0; i < maxCount; i++)
                {
                    TSource item = _source[i];
                    if (_predicate(item))
                    {
                        list.Add(_selector(item));
                    }
                }

                return list;
            }

            public override TResult TryGetFirst(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetFirst(_predicate, out found, ilist: _source);
                return found ? _selector(item) : default(TResult);
            }

            public override TResult TryGetLast(out bool found)
            {
                TSource item = EnumerableHelpers.TryGetLast(_predicate, out found, ilist: _source);
                return found ? _selector(item) : default(TResult);
            }
        }
    }
}
