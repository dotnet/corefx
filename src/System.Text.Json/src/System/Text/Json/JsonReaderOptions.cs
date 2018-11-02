// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    /// Provides the ability for the user to define custom behaviour when reading JSON
    /// using the <see cref="JsonUtf8Reader"/> that may deviate from strict adherence
    /// to the JSON specification (as per the JSON RFC - https://tools.ietf.org/html/rfc8259),
    /// which is the default behaviour.
    /// </summary>
    public struct JsonReaderOptions
    {
        /// <summary>
        /// Defines how the <see cref="JsonUtf8Reader"/> should handle comments when reading through the JSON.
        /// </summary>
        public JsonCommentHandling CommentHandling { get; set; }

        internal bool IsDefault => CommentHandling == JsonCommentHandling.Default;
    }
}
