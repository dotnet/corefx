﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static bool HandleNull(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options)
        {
            if (state.Current.SkipProperty)
            {
                return false;
            }

            // If null is read at the top level and the type is nullable, then the root is "null" so just return.
            if (reader.CurrentDepth == 0 && (state.Current.JsonPropertyInfo == null || state.Current.JsonPropertyInfo.CanBeNull))
            {
                Debug.Assert(state.IsLastFrame);
                Debug.Assert(state.Current.ReturnValue == null);
                return true;
            }

            JsonPropertyInfo propertyInfo = state.Current.JsonPropertyInfo;
            if (!propertyInfo.CanBeNull)
            {
                ThrowHelper.ThrowJsonException_DeserializeCannotBeNull(reader, state.JsonPath);
            }

            if (state.Current.IsEnumerable || state.Current.IsDictionary || state.Current.IsIDictionaryConstructible)
            {
                ApplyObjectToEnumerable(null, ref state, ref reader);
                return false;
            }

            if (state.Current.IsEnumerableProperty || state.Current.IsDictionaryProperty || state.Current.IsIDictionaryConstructibleProperty)
            {
                bool setPropertyToNull = !state.Current.PropertyInitialized;
                ApplyObjectToEnumerable(null, ref state, ref reader, setPropertyDirectly: setPropertyToNull);
                return false;
            }

            if (state.Current.ReturnValue == null)
            {
                Debug.Assert(state.IsLastFrame);
                return true;
            }

            if (!propertyInfo.IgnoreNullValues)
            {
                state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value : null);
            }

            return false;
        }
    }
}
