// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    /// This enum defines the various JSON tokens that make up a JSON text and is used by
    /// the <see cref="JsonUtf8Reader"/> when moving from one token to the next.
    /// The <see cref="JsonUtf8Reader"/> starts at 'None' by default. The 'Comment' enum value
    /// is only ever reached in a specific <see cref="JsonUtf8Reader"/> mode and is not
    /// reachable by default.
    /// </summary>
    public enum JsonTokenType : byte
    {
        None,
        StartObject,
        EndObject,
        StartArray,
        EndArray,
        PropertyName,
        String,
        Number,
        True,
        False,
        Null,
        Comment,
    }
}
