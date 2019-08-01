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
        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is IReverseProvider<TSource> reverse)
            {
                return reverse.Reverse();
            }

            if (source is IList<TSource> ilist)
            {
                if (source is TSource[] array)
                {
                    return array.Length == 0
                        ? Empty<TSource>()
                        : new ReverseArrayIterator<TSource>(array);
                }

                if (source is List<TSource> list)
                {
                    return new ReverseListIterator<TSource>(list);
                }

                return new ReverseIListIterator<TSource>(ilist);
            }

            return new ReverseEnumerableIterator<TSource>(source);
        }

        private sealed partial class ReverseArrayIterator<TSource> : Iterator<TSource>
        {
            private readonly TSource[] _source;

            public ReverseArrayIterator(TSource[] source)
            {
                Debug.Assert(source != null);
                Debug.Assert(source.Length > 0); // Caller should check this beforehand and return a cached result
                _source = source;
            }

            public override Iterator<TSource> Clone() => new ReverseArrayIterator<TSource>(_source);

            public override bool MoveNext()
            {
                if (_state < 1 | _state == _source.Length + 1)
                {
                    Dispose();
                    return false;
                }

                int index = _source.Length - _state++;
                _current = _source[index];
                return true;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) =>
                new ReverseSelectArrayIterator<TSource, TResult>(_source, selector);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate) =>
                new ReverseWhereArrayIterator<TSource>(_source, predicate);
        }

        private sealed partial class ReverseListIterator<TSource> : Iterator<TSource>
        {
            private readonly List<TSource> _source;

            public ReverseListIterator(List<TSource> source)
            {
                Debug.Assert(source != null);
                _source = source;
            }

            public override Iterator<TSource> Clone() => new ReverseListIterator<TSource>(_source);

            public override bool MoveNext()
            {
                int count = _source.Count;
                if (_state < 1 | _state == count + 1)
                {
                    Dispose();
                    return false;
                }

                int index = count - _state++;
                _current = _source[index];
                return true;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) =>
                new ReverseSelectListIterator<TSource, TResult>(_source, selector);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate) =>
                new ReverseWhereListIterator<TSource>(_source, predicate);
        }

        private sealed partial class ReverseIListIterator<TSource> : Iterator<TSource>
        {
            private readonly IList<TSource> _source;
            private TSource[] _buffer;

            public ReverseIListIterator(IList<TSource> source)
            {
                Debug.Assert(source != null);
                _source = source;
            }

            public override Iterator<TSource> Clone() => new ReverseIListIterator<TSource>(_source);

            public override bool MoveNext()
            {
                if (_state - 2 <= -2)
                {
                    // Either someone called a method and cast us to IEnumerable without calling GetEnumerator,
                    // or we were already disposed. In either case, iteration has ended, so return false.
                    // A comparison is made against -2 instead of _state <= 0 because we want to handle cases where
                    // the source is really large and adding the bias causes _state to overflow.
                    Debug.Assert(_state == -1 || _state == 0);
                    Dispose();
                    return false;
                }

                switch (_state)
                {
                    case 1:
                        // Iteration has just started. Capture the source into an array and set _state to 2 + the count.
                        // Having an extra field for the count would be more readable, but we save it into _state with a
                        // bias instead to minimize field size of the iterator.
                        Buffer<TSource> buffer = new Buffer<TSource>(_source);
                        _buffer = buffer._items;
                        _state = buffer._count + 2;
                        goto default;
                    default:
                        // At this stage, _state starts from 2 + the count. _state - 3 represents the current index into the
                        // buffer. It is continuously decremented until it hits 2, which means that we've run out of items to
                        // yield and should return false.
                        int index = _state - 3;
                        if (index != -1)
                        {
                            _current = _buffer[index];
                            --_state;
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

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) =>
                new ReverseSelectIListIterator<TSource, TResult>(_source, selector);
        }

        private sealed partial class ReverseEnumerableIterator<TSource> : Iterator<TSource>
        {
            private readonly IEnumerable<TSource> _source;
            private TSource[] _buffer;

            public ReverseEnumerableIterator(IEnumerable<TSource> source)
            {
                Debug.Assert(source != null);
                _source = source;
            }

            public override Iterator<TSource> Clone() => new ReverseEnumerableIterator<TSource>(_source);

            public override bool MoveNext()
            {
                if (_state - 2 <= -2)
                {
                    // Either someone called a method and cast us to IEnumerable without calling GetEnumerator,
                    // or we were already disposed. In either case, iteration has ended, so return false.
                    // A comparison is made against -2 instead of _state <= 0 because we want to handle cases where
                    // the source is really large and adding the bias causes _state to overflow.
                    Debug.Assert(_state == -1 || _state == 0);
                    Dispose();
                    return false;
                }

                switch (_state)
                {
                    case 1:
                        // Iteration has just started. Capture the source into an array and set _state to 2 + the count.
                        // Having an extra field for the count would be more readable, but we save it into _state with a
                        // bias instead to minimize field size of the iterator.
                        Buffer<TSource> buffer = new Buffer<TSource>(_source);
                        _buffer = buffer._items;
                        _state = buffer._count + 2;
                        goto default;
                    default:
                        // At this stage, _state starts from 2 + the count. _state - 3 represents the current index into the
                        // buffer. It is continuously decremented until it hits 2, which means that we've run out of items to
                        // yield and should return false.
                        int index = _state - 3;
                        if (index != -1)
                        {
                            _current = _buffer[index];
                            --_state;
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
        }

        private sealed partial class ReverseSelectArrayIterator<TSource, TResult> : Iterator<TResult>
        {
            private readonly TSource[] _source;
            private readonly Func<TSource, TResult> _selector;

            public ReverseSelectArrayIterator(TSource[] source, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null);
                Debug.Assert(selector != null);
                Debug.Assert(source.Length > 0); // Caller should check this beforehand and return a cached result
                _source = source;
                _selector = selector;
            }

            public override Iterator<TResult> Clone() => new ReverseSelectArrayIterator<TSource, TResult>(_source, _selector);

            public override bool MoveNext()
            {
                if (_state < 1 | _state == _source.Length + 1)
                {
                    Dispose();
                    return false;
                }

                int index = _source.Length - _state++;
                _current = _selector(_source[index]);
                return true;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new ReverseSelectArrayIterator<TSource, TResult2>(_source, CombineSelectors(_selector, selector));
        }

        private sealed partial class ReverseSelectListIterator<TSource, TResult> : Iterator<TResult>
        {
            private readonly List<TSource> _source;
            private readonly Func<TSource, TResult> _selector;

            public ReverseSelectListIterator(List<TSource> source, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null);
                Debug.Assert(selector != null);
                _source = source;
                _selector = selector;
            }

            public override Iterator<TResult> Clone() => new ReverseSelectListIterator<TSource, TResult>(_source, _selector);

            public override bool MoveNext()
            {
                int count = _source.Count;
                if (_state < 1 | _state == count + 1)
                {
                    Dispose();
                    return false;
                }

                int index = count - _state++;
                _current = _selector(_source[index]);
                return true;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new ReverseSelectListIterator<TSource, TResult2>(_source, CombineSelectors(_selector, selector));
        }

        private sealed partial class ReverseSelectIListIterator<TSource, TResult> : Iterator<TResult>
        {
            private readonly IList<TSource> _source;
            private readonly Func<TSource, TResult> _selector;
            private TSource[] _buffer;

            public ReverseSelectIListIterator(IList<TSource> source, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null);
                Debug.Assert(selector != null);
                _source = source;
                _selector = selector;
            }

            public override Iterator<TResult> Clone() => new ReverseSelectIListIterator<TSource, TResult>(_source, _selector);

            public override bool MoveNext()
            {
                if (_state - 2 <= -2)
                {
                    // Either someone called a method and cast us to IEnumerable without calling GetEnumerator,
                    // or we were already disposed. In either case, iteration has ended, so return false.
                    // A comparison is made against -2 instead of _state <= 0 because we want to handle cases where
                    // the source is really large and adding the bias causes _state to overflow.
                    Debug.Assert(_state == -1 || _state == 0);
                    Dispose();
                    return false;
                }

                switch (_state)
                {
                    case 1:
                        // Iteration has just started. Capture the source into an array and set _state to 2 + the count.
                        // Having an extra field for the count would be more readable, but we save it into _state with a
                        // bias instead to minimize field size of the iterator.
                        Buffer<TSource> buffer = new Buffer<TSource>(_source);
                        _buffer = buffer._items;
                        _state = buffer._count + 2;
                        goto default;
                    default:
                        // At this stage, _state starts from 2 + the count. _state - 3 represents the current index into the
                        // buffer. It is continuously decremented until it hits 2, which means that we've run out of items to
                        // yield and should return false.
                        int index = _state - 3;
                        if (index != -1)
                        {
                            _current = _selector(_buffer[index]);
                            --_state;
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

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new ReverseSelectIListIterator<TSource, TResult2>(_source, CombineSelectors(_selector, selector));
        }

        private sealed partial class ReverseWhereArrayIterator<TSource> : Iterator<TSource>
        {
            private readonly TSource[] _source;
            private readonly Func<TSource, bool> _predicate;

            public ReverseWhereArrayIterator(TSource[] source, Func<TSource, bool> predicate)
            {
                Debug.Assert(source != null && source.Length > 0);
                Debug.Assert(predicate != null);
                _source = source;
                _predicate = predicate;
            }

            public override Iterator<TSource> Clone() =>
                new ReverseWhereArrayIterator<TSource>(_source, _predicate);

            public override bool MoveNext()
            {
                TSource[] source = _source;
                int index = source.Length - _state;

                while (unchecked((uint)index < (uint)source.Length))
                {
                    TSource item = source[index];
                    _state++;
                    index--;
                    if (_predicate(item))
                    {
                        _current = item;
                        return true;
                    }
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) =>
                new ReverseWhereSelectArrayIterator<TSource, TResult>(_source, _predicate, selector);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate) =>
                new ReverseWhereArrayIterator<TSource>(_source, CombinePredicates(_predicate, predicate));
        }

        private sealed partial class ReverseWhereListIterator<TSource> : Iterator<TSource>
        {
            private readonly List<TSource> _source;
            private readonly Func<TSource, bool> _predicate;

            public ReverseWhereListIterator(List<TSource> source, Func<TSource, bool> predicate)
            {
                Debug.Assert(source != null);
                Debug.Assert(predicate != null);
                _source = source;
                _predicate = predicate;
            }

            public override Iterator<TSource> Clone() =>
                new ReverseWhereListIterator<TSource>(_source, _predicate);

            public override bool MoveNext()
            {
                List<TSource> source = _source;
                int count = source.Count;
                int index = count - _state;

                while (unchecked((uint)index < (uint)count))
                {
                    TSource item = source[index];
                    _state++;
                    index--;
                    if (_predicate(item))
                    {
                        _current = item;
                        return true;
                    }
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector) =>
                new ReverseWhereSelectListIterator<TSource, TResult>(_source, _predicate, selector);

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate) =>
                new ReverseWhereListIterator<TSource>(_source, CombinePredicates(_predicate, predicate));
        }

        private sealed partial class ReverseWhereSelectArrayIterator<TSource, TResult> : Iterator<TResult>
        {
            private readonly TSource[] _source;
            private readonly Func<TSource, bool> _predicate;
            private readonly Func<TSource, TResult> _selector;

            public ReverseWhereSelectArrayIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null && source.Length > 0);
                Debug.Assert(predicate != null);
                Debug.Assert(selector != null);
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }

            public override Iterator<TResult> Clone() =>
                new ReverseWhereSelectArrayIterator<TSource, TResult>(_source, _predicate, _selector);

            public override bool MoveNext()
            {
                TSource[] source = _source;
                int index = source.Length - _state;

                while (unchecked((uint)index < (uint)source.Length))
                {
                    TSource item = source[index];
                    _state++;
                    index--;
                    if (_predicate(item))
                    {
                        _current = _selector(item);
                        return true;
                    }
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new ReverseWhereSelectArrayIterator<TSource, TResult2>(_source, _predicate, CombineSelectors(_selector, selector));
        }

        private sealed partial class ReverseWhereSelectListIterator<TSource, TResult> : Iterator<TResult>
        {
            private readonly List<TSource> _source;
            private readonly Func<TSource, bool> _predicate;
            private readonly Func<TSource, TResult> _selector;

            public ReverseWhereSelectListIterator(List<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                Debug.Assert(source != null);
                Debug.Assert(predicate != null);
                Debug.Assert(selector != null);
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }

            public override Iterator<TResult> Clone() =>
                new ReverseWhereSelectListIterator<TSource, TResult>(_source, _predicate, _selector);

            public override bool MoveNext()
            {
                List<TSource> source = _source;
                int count = source.Count;
                int index = count - _state;

                while (unchecked((uint)index < (uint)count))
                {
                    TSource item = source[index];
                    _state++;
                    index--;
                    if (_predicate(item))
                    {
                        _current = _selector(item);
                        return true;
                    }
                }

                Dispose();
                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector) =>
                new ReverseWhereSelectListIterator<TSource, TResult2>(_source, _predicate, CombineSelectors(_selector, selector));
        }
    }
}
