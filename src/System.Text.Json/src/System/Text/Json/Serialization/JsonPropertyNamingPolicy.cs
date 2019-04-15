// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Determines the policy used to convert a JSON name to another format, such as a camel-casing format.
    /// </summary>
    public abstract class JsonPropertyNamingPolicy
    {
        protected JsonPropertyNamingPolicy() { }

        /// <summary>
        /// Returns the policy for camel-casing.
        /// </summary>
        public static JsonPropertyNamingPolicy CamelCase { get; } = new JsonCamelCaseNamePolicy();

        /// <summary>
        /// Converts the provided name.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted name</returns>
        protected abstract string ConvertName(string name);

        internal string CallConvertName(string name)
        {
            return ConvertName(name);
        }
    }
}
