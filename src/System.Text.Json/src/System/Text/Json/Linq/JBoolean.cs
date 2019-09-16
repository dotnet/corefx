// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Linq
{
    /// <summary>
    ///   Represents a mutable boolean JSON value.
    /// </summary>
    public sealed class JBoolean : JNode, IEquatable<JBoolean>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="JBoolean"/> class representing the value <see langword="false"/>.
        /// </summary>
        public JBoolean() => Value = false;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JBoolean"/> class representing a specified value.
        /// </summary>
        public JBoolean(bool value) => Value = value;

        /// <summary>
        ///   Gets or sets the boolean value represented by the instance.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        ///   Converts the value represented by the instance to the string in JSON format.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString() => Value ? "true" : "false";

        /// <summary>
        ///   Compares <paramref name="obj"/> to the value of this instance.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the boolean value of this instance matches <paramref name="obj"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is JBoolean jsonBoolean && Equals(jsonBoolean);

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
        public bool Equals(JBoolean other) => !(other is null) && Value == other.Value;

        /// <summary>
        ///   Compares values of two JSON booleans.
        /// </summary>
        /// <param name="left">The JSON boolean to compare.</param>
        /// <param name="right">The JSON boolean to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(JBoolean left, JBoolean right)
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
        ///   Compares values of two JSON booleans.
        /// </summary>
        /// <param name="left">The JSON boolean to compare.</param>
        /// <param name="right">The JSON boolean to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if values of instances do not match,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(JBoolean left, JBoolean right) => !(left == right);

        /// <summary>
        ///   Creates a new JSON boolean that is a copy of the current instance.
        /// </summary>
        /// <returns>A new JSON boolean that is a copy of this instance.</returns>
        public override JNode Clone() => new JBoolean(Value);

        /// <summary>
        ///   Returns <see cref="JsonValueKind.True"/> or <see cref="JsonValueKind.False"/>, accordingly to the represented value.
        /// </summary>
        public override JsonValueKind ValueKind { get => Value ? JsonValueKind.True : JsonValueKind.False; }
    }
}
