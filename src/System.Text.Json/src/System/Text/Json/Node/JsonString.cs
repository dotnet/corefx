// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json
{
    /// <summary>
    ///   Represents a mutable text JSON value.
    /// </summary>
    public sealed class JsonString : JsonNode, IEquatable<JsonString>
    {
        private string _value;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonString"/> class representing the empty value.
        /// </summary>
        public JsonString() => Value = string.Empty;

        /// <summary>
        ///  Initializes a new instance of the <see cref="JsonString"/> class representing a specified value.
        /// </summary>
        /// <param name="value">The value to represent as a JSON string.</param>
        /// <exception cref="ArgumentNullException">
        ///   Provided value is null.
        /// </exception>
        public JsonString(string value) => Value = value;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonString"/> class representing a specified value.
        /// </summary>
        /// <param name="value">The value to represent as a JSON string.</param>
        public JsonString(ReadOnlySpan<char> value) => Value = value.ToString();

        /// <summary>
        ///  Initializes a new instance of the <see cref="JsonString"/> with a string representation of the <see cref="Guid"/> structure.
        /// </summary>
        /// <param name="value">The value to represent as a JSON string.</param>
        public JsonString(Guid value) => Value = value.ToString("D");

        /// <summary>
        ///  Initializes a new instance of the <see cref="JsonString"/> with an ISO 8601 representation of the <see cref="DateTime"/> structure.
        /// </summary>
        /// <param name="value">The value to represent as a JSON string.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The date and time is outside the range of dates supported by the calendar used by the invariant culture.
        /// </exception>
        public JsonString(DateTime value) => Value = value.ToString("s", CultureInfo.InvariantCulture);

        /// <summary>
        ///  Initializes a new instance of the <see cref="JsonString"/> with an ISO 8601 representation of the <see cref="DateTimeOffset"/> structure.
        /// </summary>
        /// <param name="value">The value to represent as a JSON string.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The date and time is outside the range of dates supported by the calendar used by the invariant culture.
        /// </exception>
        public JsonString(DateTimeOffset value) => Value = value.ToString("s", CultureInfo.InvariantCulture);

        /// <summary>
        ///   Gets or sets the text value represented by the instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///   Provided value is null.
        /// </exception>
        public string Value
        {
            get => _value;
            set => _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///   Returns the text value represented by the instance.
        /// </summary>
        /// <returns>The value represented by this instance.</returns>
        public override string ToString() => _value;

        /// <summary>
        ///   Converts the ISO 8601 text value of this instance to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> equivalent to the text stored by this instance.</returns>
        /// <exception cref="FormatException">
        ///   Text value of this instance is not in an ISO 8601 defined DateTime format.
        /// </exception>
        public DateTime GetDateTime() => DateTime.ParseExact(_value, "s", CultureInfo.InvariantCulture);

        /// <summary>
        ///   Converts the ISO 8601 text value of this instance to <see cref="DateTimeOffset"/> equivalent.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> equivalent to the text stored by this instance.</returns>
        /// <exception cref="FormatException">
        ///   Text value of this instance is not in an ISO 8601 defined DateTimeOffset format.
        /// </exception>
        public DateTimeOffset GetDateTimeOffset() => DateTimeOffset.ParseExact(_value, "s", CultureInfo.InvariantCulture);

        /// <summary>
        ///   Converts the text value of this instance to its <see cref="Guid"/> equivalent.
        /// </summary>
        /// <returns>A <see cref="Guid"/> equivalent to the text stored by this instance.</returns>
        /// <exception cref="FormatException">
        ///   Text value of this instance is not in a GUID recognized format.
        /// </exception>
        public Guid GetGuid() => Guid.ParseExact(_value, "D");

        /// <summary>
        ///   Converts the ISO 8601 text value of this instance to its ISO 8601 <see cref="DateTime"/> equivalent.
        ///   A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">
        ///   When this method returns, contains the see cref="DateTime"/> value equivalent of the text contained in this instance,
        ///   if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>
        ///  <see langword="true"/> if instance was converted successfully;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetDateTime(out DateTime value) => DateTime.TryParseExact(_value, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

        /// <summary>
        ///   Converts the ISO 8601 text value of this instance to its <see cref="DateTimeOffset"/> equivalent.
        ///   A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">
        ///   When this method returns, contains the <see cref="DateTimeOffset"/> value equivalent of the text contained in this instance,
        ///   if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>
        ///  <see langword="true"/> if instance was converted successfully;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetDateTimeOffset(out DateTimeOffset value) => DateTimeOffset.TryParseExact(_value, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

        /// <summary>
        ///   Converts the text value of this instance to its <see cref="Guid"/> equivalent.
        /// </summary>
        /// <returns>A <see cref="Guid"/> equivalent to the text stored by this instance.</returns>
        /// <exception cref="FormatException">
        ///   Text value of this instance is not in a GUID recognized format.
        /// </exception>
        public bool TryGetGuid(out Guid value) => Guid.TryParseExact(_value, "D", out value);

        /// <summary>
        ///   Compares <paramref name="obj"/> to the value of this instance.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the text value of this instance matches <paramref name="obj"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is JsonString jsonString && Equals(jsonString);

        /// <summary>
        ///   Calculates a hash code of this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        ///   Compares other JSON string to the value of this instance.
        /// </summary>
        /// <param name="other">The JSON string to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the text value of this instance matches <paramref name="other"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(JsonString other) => !(other is null) && Value == other.Value;

        /// <summary>
        ///   Compares values of two JSON strings.
        /// </summary>
        /// <param name="left">The JSON string to compare.</param>
        /// <param name="right">The JSON string to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(JsonString left, JsonString right)
        {
            // Test "right" first to allow branch elimination when inlined for null checks (== null)
            // so it can become a simple test
            if (right is null)
            {
                // return true/false not the test result https://github.com/dotnet/coreclr/issues/914
                return (left is null) ? true : false;
            }

            return right.Equals(left);
        }

        /// <summary>
        ///   Compares values of two JSON strings.
        /// </summary>
        /// <param name="left">The JSON string to compare.</param>
        /// <param name="right">The JSON string to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances do not match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(JsonString left, JsonString right) => !(left == right);

        /// <summary>
        ///   Creates a new JSON string that is a copy of the current instance.
        /// </summary>
        /// <returns>A new JSON string that is a copy of this instance.</returns>
        public override JsonNode Clone() => new JsonString(Value);

        /// <summary>
        ///   Returns <see cref="JsonValueKind.String"/>
        /// </summary>
        public override JsonValueKind ValueKind { get => JsonValueKind.String; }

        /// <summary>
        ///   Converts the text value of this instance, which should encode binary data as base-64 digits, to an equivalent 8-bit unsigned <see cref="byte"/> array.
        ///   The return value indicates wether the conversion succeeded.
        /// </summary>
        /// <param name="value">
        ///   When this method returns, contains the <see cref="byte"/> array equivalent of the text contained in this instance,
        ///   if the conversion succeeded.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if text was converted successfully; othwerwise returns <see langword="false"/>.
        /// </returns>
        internal bool TryGetBytesFromBase64(out byte[] value)
        {
            Debug.Assert(_value != null);

            // Shortest length of a base-64 string is 4 characters.
            if (_value.Length < 4)
            {
                value = default;
                return false;
            }

#if BUILDING_INBOX_LIBRARY
            // we decode string -> byte, so the resulting length will
            // be /4 * 3 - padding. To be on the safe side, keep padding and slice later
            int bufferSize = _value.Length / 4 * 3;

            byte[] arrayToReturnToPool = null;
            Span<byte> buffer = bufferSize <= JsonConstants.StackallocThreshold
                ? stackalloc byte[bufferSize]
                : arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(bufferSize);

            if (Convert.TryFromBase64String(_value, buffer, out int bytesWritten))
            {
                buffer = buffer.Slice(0, bytesWritten);
                value = buffer.ToArray();
                buffer.Clear();
                ReturnToPool(arrayToReturnToPool);
                return true;
            }
            else
            {
                value = default;
                buffer.Clear();
                ReturnToPool(arrayToReturnToPool);
                return false;
            }

#else
            try
            {
                value = Convert.FromBase64String(_value);
                return true;
            }
            catch (FormatException)
            {
                value = null;
                return false;
            }
#endif
        }

        /// <summary>
        /// Clears a byte array and returns it to the pool.
        /// </summary>
        /// <param name="arrayToReturnToPool">Byte array retrieved from ArrayPool&lt;byte&gt;.Shared.Rent</param>
        private void ReturnToPool(byte[] arrayToReturnToPool)
        {
            if (arrayToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(arrayToReturnToPool);
            }
        }
    }
}
