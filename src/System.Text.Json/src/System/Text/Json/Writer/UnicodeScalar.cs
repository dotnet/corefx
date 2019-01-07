// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    /// <summary>
    /// Represents a 24-bit Unicode scalar value.
    /// A scalar value is any value in the range [U+0000..U+D7FF] or [U+E000..U+10FFFF].
    /// </summary>
    internal struct UnicodeScalar : IComparable<UnicodeScalar>, IEquatable<UnicodeScalar>
    {
        /// <summary>
        /// The Unicode Replacement Character U+FFFD.
        /// </summary>
        public static readonly UnicodeScalar s_replacementChar = new UnicodeScalar(0xFFFD);

        /// <summary>
        /// The integer value of this scalar.
        /// </summary>
        public readonly int Value; // = U+0000 if using default init

        /// <summary>
        /// Constructs a Unicode scalar from the given value.
        /// The value must represent a valid scalar.
        /// </summary>
        public UnicodeScalar(int value)
            : this((uint)value)
        {
            // None of the APIs on this type are guaranteed to produce correct results
            // if we don't validate the input during construction.

            if (!IsValidScalar((uint)Value))
            {
                throw new ArgumentOutOfRangeException(
                    message: "Value must be between U+0000 and U+D7FF, inclusive; or value must be between U+E000 and U+10FFFF, inclusive.",
                    paramName: nameof(value));
            }
        }

        // non-validating ctor for internal use
        private UnicodeScalar(uint value)
        {
            Value = (int)value;
        }

        internal static UnicodeScalar CreateWithoutValidation(uint value) => new UnicodeScalar(value);

        public int CompareTo(UnicodeScalar other) => Value.CompareTo(other.Value);

        public override bool Equals(object other) => (other is UnicodeScalar) && Equals((UnicodeScalar)other);

        public bool Equals(UnicodeScalar other) => Value == other.Value;

        public override int GetHashCode() => Value;

        private static bool IsValidScalar(uint value)
            => (value < 0xD800U) || IsInRangeInclusive(value, 0xE000U, 0x10FFFFU);

        /// <summary>
        /// Returns <see langword="true"/> iff <paramref name="value"/> is between
        /// <paramref name="lowerBound"/> and <paramref name="upperBound"/>, inclusive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound)
            => (value - lowerBound) <= (upperBound - lowerBound);
    }
}
