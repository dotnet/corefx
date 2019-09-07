// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleStartDictionary(JsonSerializerOptions options, ref Utf8JsonReader reader, ref ReadStack state)
        {
            throw new NotImplementedException();
        }

        private static void HandleEndDictionary(JsonSerializerOptions options, ref Utf8JsonReader reader, ref ReadStack state)
        {
            throw new NotImplementedException();
        }
    }
}
