// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Represents position in non-contiguous set of memory.
    /// Properties of this type should not be interpreted by anything but the but the
    /// type that created it.
    /// </summary>
    public readonly struct SequencePosition : IEquatable<SequencePosition>
    {
        private readonly object _segment;
        private readonly int _index;

        /// <summary>
        /// Creates new <see cref="SequencePosition"/>
        /// </summary>
        public SequencePosition(object segment, int index)
        {
            _segment = segment;
            _index = index;
        }

        /// <summary>
        /// Segment of memory this <see cref="SequencePosition"/> points to.
        /// </summary>
        public object Segment => _segment;

        /// <summary>
        /// Index inside segment of memory this <see cref="SequencePosition"/> points to.
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// Returns true if left and right point at the same segment and have the same index.
        /// </summary>
        public static bool operator ==(SequencePosition left, SequencePosition right) => left._index == right._index && left._segment == right._segment;
        /// <summary>
        /// Returns true if left and right do not point at the same segment and have the same index.
        /// </summary>
        public static bool operator !=(SequencePosition left, SequencePosition right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(SequencePosition position) => this == position;

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) =>
            obj is SequencePosition ? this == (SequencePosition)obj : false;

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            var h1 = _segment?.GetHashCode() ?? 0;
            var h2 = _index.GetHashCode();

            var shift5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)shift5 + h1) ^ h2;
        }

        /// <inheritdoc />
        public override string ToString() =>
            this == default ? "(default)" : _segment == null ? $"{_index}" : $"{_segment}[{_index}]";
    }
}
