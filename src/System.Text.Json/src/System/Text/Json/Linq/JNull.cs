// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Linq
{
    /// <summary>
    ///   Represents the null JSON value.
    /// </summary>
    public sealed class JNull : JNode, IEquatable<JNull>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="JNull"/> class representing the value <see langword="null"/>.
        /// </summary>
        public JNull() { }

        /// <summary>
        ///   Converts the null value to the string in JSON format.
        /// </summary>
        /// <returns>The string representation of the null value.</returns>
        public override string ToString() => "null";

        /// <summary>
        ///   Compares <paramref name="obj"/> to the value of this instance.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>
        ///   <see langword="true"/> if the <paramref name="obj"/> is <see cref="JNull"/>,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is JNull;

        /// <summary>
        ///   Calculates a hash code of this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() => -1;

        /// <summary>
        ///   Compares other JSON null to the value of this instance.
        /// </summary>
        /// <param name="other">The JSON null to compare against.</param>
        /// <returns><see langword="true"/></returns>
        public bool Equals(JNull other) => true;

        /// <summary>
        ///   Compares values of two JSON nulls.
        /// </summary>
        /// <param name="left">The JSON null to compare.</param>
        /// <param name="right">The JSON null to compare.</param>
        /// <returns><see langword="true"/></returns>
        public static bool operator ==(JNull left, JNull right) => true;

        /// <summary>
        ///   Compares values of two JSON nulls.
        /// </summary>
        /// <param name="left">The JSON null to compare.</param>
        /// <param name="right">The JSON null to compare.</param>
        /// <returns><see langword="false"/></returns>
        public static bool operator !=(JNull left, JNull right) => false;

        /// <summary>
        ///   Creates a new JSON null that is a copy of the current instance.
        /// </summary>
        /// <returns>A new JSON null that is a copy of this instance.</returns>
        public override JNode Clone() => new JNull();

        /// <summary>
        ///   Returns <see cref="JsonValueKind.Null"/>.
        /// </summary>
        public override JsonValueKind ValueKind { get => JsonValueKind.Null; }
    }
}
