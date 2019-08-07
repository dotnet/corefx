// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    ///   Represents a boolean JSON value.
    /// </summary>
    public class JsonBoolean : JsonNode, IEquatable<JsonBoolean>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonBoolean"/> class representing the value <see langword="false"/>.
        /// </summary>
        public JsonBoolean() => Value = false;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonBoolean"/> class representing a specified value.
        /// </summary>
        public JsonBoolean(bool value) => Value = value;

        /// <summary>
        ///   Gets or sets the boolean value represented by the instance.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        ///   Converts a <see cref="bool"/> to a JSON boolean.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonBoolean(bool value) => new JsonBoolean(value);

        /// <summary>
        ///   Compares <paramref name="obj"/> to the value of this instance. 
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the boolean value of this instance matches <paramref name="obj"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is JsonBoolean jsonBoolean && Value == jsonBoolean.Value;

        /// <summary>
        ///   Calculates a hash code of this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        ///   Compares other JSON boolean to the value of this instance. 
        /// </summary>
        /// <param name="other">The JSON boolean to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the boolean value of this instance matches <paramref name="other"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(JsonBoolean other) => !(other is null) && Value == other.Value;

        /// <summary>
        ///   Compares values of two JSON booleans. 
        /// </summary>
        /// <param name="left">The JSON boolean to compare.</param>
        /// <param name="right">The JSON boolean to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(JsonBoolean left, JsonBoolean right) => left?.Value == right?.Value;

        /// <summary>
        ///   Compares values of two JSON booleans. 
        /// </summary>
        /// <param name="left">The JSON boolean to compare.</param>
        /// <param name="right">The JSON boolean to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances do not match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(JsonBoolean left, JsonBoolean right) => left?.Value != right?.Value;
    }
}
