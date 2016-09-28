// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return OfTypeIterator<TResult>(source);
        }

        private static IEnumerable<TResult> OfTypeIterator<TResult>(IEnumerable source)
        {
            foreach (object obj in source)
            {
                if (obj is TResult)
                {
                    yield return (TResult)obj;
                }
            }
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            IEnumerable<TResult> typedSource = source as IEnumerable<TResult>;
            if (typedSource != null)
            {
                return typedSource;
            }

            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }
            
            return new CastIterator<TResult>(source);
        }

        private sealed class CastIterator<TResult> : Iterator<TResult>, IIListProvider<TResult>
        {
            private readonly IEnumerable _source;
            private IEnumerator _enumerator;

            public CastIterator(IEnumerable source)
            {
                Debug.Assert(source != null);

                _source = source;
            }

            public override Iterator<TResult> Clone() => new CastIterator<TResult>(_source);
            
            public override void Dispose()
            {
                // Non-generic IEnumerators do not implement IDisposable up front.
                // However, in a foreach loop with such enumerators the C# compiler will generate
                // a type check to see if the enumerator implements IDisposable, and if so Dispose it.
                // Since Cast was originally implemented using yield return and a foreach loop,
                // let's try to preserve the original behavior.

                var disposable = _enumerator as IDisposable;
                disposable?.Dispose();
                _enumerator = null;

                base.Dispose();
            }

            public int GetCount(bool onlyIfCheap) => EnumerableHelpers.GetCount(this);

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        goto case 2;
                    case 2:
                        if (_enumerator.MoveNext())
                        {
                            _current = (TResult)_enumerator.Current;
                            return true;
                        }

                        _state = 3;
                        Dispose();
                        break;
                }

                return false;
            }

            public TResult[] ToArray()
            {
                var collection = _source as ICollection;
                if (collection == null)
                {
                    return EnumerableHelpers.ToArray(this);
                }

                int count = collection.Count;
                if (count == 0)
                {
                    return Array.Empty<TResult>();
                }

                int index = 0;
                var array = new TResult[count];

                foreach (object obj in collection)
                {
                    array[index++] = (TResult)obj;
                }

                Debug.Assert(index == array.Length);
                return array;
            }

            public List<TResult> ToList()
            {
                var collection = _source as ICollection;
                if (collection == null)
                {
                    return new List<TResult>(this);
                }

                int count = collection.Count;
                var list = new List<TResult>(count);

                if (count != 0) // Avoid the enumerator allocation
                {
                    foreach (object obj in collection)
                    {
                        list.Add((TResult)obj);
                    }
                }

                Debug.Assert(list.Count == count);
                return list;
            }
        }
    }
}
