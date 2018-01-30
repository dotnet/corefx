// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
#pragma warning disable 1591

namespace System
{
    public readonly struct SequencePosition : IEquatable<SequencePosition>
    {
        readonly object _segment;
        readonly int _index;

        public SequencePosition(object segment, int index)
        {
            _segment = segment;
            _index = index;
        }

        public SequencePosition(object segment)
        {
            _segment = segment;
            _index = 0;
        }

        public object Segment => _segment;

        public int Index => _index;

        public static bool operator ==(SequencePosition left, SequencePosition right) => left._index == right._index && left._segment == right._segment;
        public static bool operator !=(SequencePosition left, SequencePosition right) => left._index != right._index || left._segment != right._segment;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(SequencePosition position) => this == position;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) =>
            obj is SequencePosition ? this == (SequencePosition)obj : false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            var h1 = _segment?.GetHashCode() ?? 0;
            var h2 = _index.GetHashCode();

            var shift5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)shift5 + h1) ^ h2;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() =>
            this == default ? "(default)" : _segment == null ? $"{_index}" : $"{_segment}[{_index}]";
    }
}
