﻿// Licensed to the .NET Foundation under one or more agreements.
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
                throw Error.ArgumentOutOfRange(nameof(count));
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
        private sealed partial class RangeIterator : Iterator<int>
        {
            private readonly int _start;
            private readonly int _end;

            public RangeIterator(int start, int count)
            {
                Debug.Assert(count > 0);
                _start = start;
                _end = unchecked(start + count);
            }

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
