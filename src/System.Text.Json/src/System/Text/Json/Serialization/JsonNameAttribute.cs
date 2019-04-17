// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Specifies a property name. This overrides any naming policy specified by <see cref="JsonPropertyNamingPolicy"/>.
    /// </summary>
    /// <remarks>If a value is not provided, the current property name is used.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonNameAttribute : JsonAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonNameAttribute"/>.
        /// </summary>
        public JsonNameAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNameAttribute"/> with the speicified property name.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public JsonNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the property that is present in JSON during deserialization and
        /// written during serialization
        /// </summary>
        public string Name { get; set; }
    }
}
