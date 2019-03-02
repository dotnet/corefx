﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        private static bool HandleValue(JsonTokenType tokenType, JsonSerializerOptions options, ref Utf8JsonReader reader, ref ReadStack state)
        {
            if (state.Current.Skip())
            {
                return false;
            }

            bool lastCall = (!state.Current.IsEnumerable() && !state.Current.IsPropertyEnumerable() && state.Current.ReturnValue == null);
            state.Current.JsonPropertyInfo.Read(tokenType, options, ref state, ref reader);
            return lastCall;
        }
    }
}
