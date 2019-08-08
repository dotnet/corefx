// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    ///   Represents a text JSON value.
    /// </summary>
    public sealed class JsonString : JsonNode, IEquatable<JsonString>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonString"/> class representing the empty value.
        /// </summary>
        public JsonString() => Value = "";

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonString"/> class representing a specified value.
        /// </summary>
        public JsonString(string value) => Value = value ?? throw new ArgumentNullException();

        /// <summary>
        ///   Gets or sets the text value represented by the instance.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///   Converts a <see cref="string"/> to a JSON string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonString(string value) => new JsonString(value);

        /// <summary>
        ///   Compares <paramref name="obj"/> to the value of this instance.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the text value of this instance matches <paramref name="obj"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is JsonString jsonString && Value == jsonString.Value;

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
        public static bool operator ==(JsonString left, JsonString right) => left?.Value == right?.Value;

        /// <summary>
        ///   Compares values of two JSON strings.
        /// </summary>
        /// <param name="left">The JSON string to compare.</param>
        /// <param name="right">The JSON string to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances do not match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(JsonString left, JsonString right) => left?.Value != right?.Value;
    }
}
