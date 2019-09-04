﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    /// This enum defines the various ways the <see cref="Utf8JsonReader"/> can deal with comments.
    /// </summary>
    public enum ReferenceLoopHandling : byte
    {
        /// <summary>
        /// The Serializer will throw when a loop is found.
        /// </summary>
        Error = 0,
        /// <summary>
        /// The Serializer will ignore subsequent matches of the same object within the parent.
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// </summary>
        Serialize = 2,
    }
}
