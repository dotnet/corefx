// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    ///   Specifies the JSON node value.
    /// </summary>
    public enum JsonNodeKind : byte
    {
        /// <summary>
        ///   Indicates that a value is a JSON object.
        /// </summary>
        Object,

        /// <summary>
        ///   Indicates that a value is a JSON array.
        /// </summary>
        Array,

        /// <summary>
        ///   Indicates that a value is a JSON string.
        /// </summary>
        String,

        /// <summary>
        ///   Indicates that a value is a JSON number.
        /// </summary>
        Number,

        /// <summary>
        ///   Indicates that a value is a JSON boolean.
        /// </summary>
        Boolean
    }
}
