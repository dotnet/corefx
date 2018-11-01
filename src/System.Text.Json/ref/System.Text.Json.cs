// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Text.Json
{
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
