// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Specifies handling of object references during serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class JsonReferenceHandlingAttribute : JsonAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonPropertyNameAttribute"/> with the specified property name.
        /// </summary>
        /// <param name="handling">One of the enumeration values on ReferenceLoopHandling.</param>
        public JsonReferenceHandlingAttribute(ReferenceHandling handling)
        {
            Handling = handling;
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public ReferenceHandling Handling { get; }
    }
}
