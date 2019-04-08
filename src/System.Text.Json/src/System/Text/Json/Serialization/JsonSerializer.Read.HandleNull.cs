﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        private static bool HandleNull(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options)
        {
            if (state.Current.Skip())
            {
                return false;
            }

            // If we don't have a valid property, that means we read "null" for a root object so just return.
            if (state.Current.JsonPropertyInfo == null)
            {
                Debug.Assert(state.IsLastFrame);
                Debug.Assert(state.Current.ReturnValue == null);
                return true;
            }

            JsonPropertyInfo propertyInfo = state.Current.JsonPropertyInfo;
            if (!propertyInfo.CanBeNull)
            {
                ThrowHelper.ThrowJsonReaderException_DeserializeCannotBeNull(reader, state);
            }

            if (state.Current.IsEnumerable() || state.Current.IsPropertyEnumerable())
            {
                ReadStackFrame.SetReturnValue(null, options, ref state.Current);
                return false;
            }

            if (state.Current.ReturnValue == null)
            {
                Debug.Assert(state.IsLastFrame);
                return true;
            }

            if (!propertyInfo.IgnoreNullPropertyValueOnRead(options))
            {
                state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, null, options);
            }

            return false;
        }
    }
}
