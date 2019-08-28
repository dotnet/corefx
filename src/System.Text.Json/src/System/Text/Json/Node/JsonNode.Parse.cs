// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document.
    /// </summary>
    public abstract partial class JsonNode
    {
        /// <summary>
        ///   Parses a string representiong JSON document into <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns><see cref="JsonNode"/> representation of <paramref name="json"/>.</returns>
        public static JsonNode Parse(string json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return DeepCopy(document.RootElement);
            }
        }
    }
}
