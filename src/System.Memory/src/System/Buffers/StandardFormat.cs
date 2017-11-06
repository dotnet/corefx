// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers
{
    /// <summary>
    /// Represents a standard formatting string without using an actual String. A StandardFormat consists of a character (such as 'G', 'D' or 'X')
    /// and an optional precision ranging from 0..99, or the special value NoPrecision.
    /// </summary>
    public readonly struct StandardFormat : IEquatable<StandardFormat>
    {
        /// <summary>
        /// The maximum valid precision value.
        /// </summary>
        public const byte NoPrecision = byte.MaxValue;

        /// <summary>
        /// Precision values for format that don't use a precision, or for when the precision is to be unspecified.
        /// </summary>
        public const byte MaxPrecision = 99;

        private readonly byte _format;
        private readonly byte _precision;

        /// <summary>
        /// The character component of the format.
        /// </summary>
        public char Symbol => (char)_format;

        /// <summary>
        /// The precision component of the format. Ranges from 0..9 or the special value NoPrecision.
        /// </summary>
        public byte Precision => _precision;

        /// <summary>
        /// true if Precision is a value other than NoPrecision
        /// </summary>
        public bool HasPrecision => _precision != NoPrecision;

        /// <summary>
        /// true if the StandardFormat == default(StandardFormat)
        /// </summary>
        public bool IsDefault => _format == 0 && _precision == 0;

        /// <summary>
        /// Create a StandardFormat.
        /// </summary>
        /// <param name="symbol">A type-specific formatting characeter such as 'G', 'D' or 'X'</param>
        /// <param name="precision">An optional precision ranging from 0..9 or the special value NoPrecision (the default)</param>
        public StandardFormat(char symbol, byte precision = NoPrecision)
        {
            if (precision != NoPrecision && precision > MaxPrecision)
                throw new ArgumentOutOfRangeException(nameof(precision));
            if (symbol != (byte)symbol)
                throw new ArgumentOutOfRangeException(nameof(symbol));

            _format = (byte)symbol;
            _precision = precision;
        }

        /// <summary>
        /// Converts a character to a StandardFormat using the NoPrecision precision.
        /// </summary>
        public static implicit operator StandardFormat(char symbol) => new StandardFormat(symbol);

        /// <summary>
        /// Converts a classic .NET format string into a StandardFormat
        /// </summary>
        public static StandardFormat Parse(ReadOnlySpan<char> format)
        {
            return Parse(new string(format.ToArray()));
        }

        /// <summary>
        /// Converts a classic .NET format string into a StandardFormat
        /// </summary>
        public static StandardFormat Parse(string format)
        {
            if (string.IsNullOrEmpty(format))
                return default;

            char specifier = format[0];
            byte precision = NoPrecision;

            if (format.Length > 1)
            {
                if (!byte.TryParse(format.Substring(1), out precision))
                    throw new FormatException("format");

                if (precision > MaxPrecision)
                    throw new FormatException("precision");
            }

            return new StandardFormat(specifier, precision);
        }

        /// <summary>
        /// Returns true if both the Symbol and Precision are equal.
        /// </summary>
        public override bool Equals(object obj) => obj is StandardFormat other && Equals(other);

        /// <summary>
        /// Compute a hash code.
        /// </summary>
        public override int GetHashCode() => _format.GetHashCode() ^ _precision.GetHashCode();

        /// <summary>
        /// Returns true if both the Symbol and Precision are equal.
        /// </summary>
        public bool Equals(StandardFormat other) => _format == other._format && _precision == other._precision;

        /// <summary>
        /// Returns true if both the Symbol and Precision are equal.
        /// </summary>
        public static bool operator ==(StandardFormat left, StandardFormat right) => left.Equals(right);

        /// <summary>
        /// Returns false if both the Symbol and Precision are equal.
        /// </summary>
        public static bool operator !=(StandardFormat left, StandardFormat right) => !left.Equals(right);
    }
}
