// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleStartObject(JsonSerializerOptions options, ref ReadStack state)
        {
            Debug.Assert(!state.Current.IsProcessingDictionary);

            if (state.Current.IsProcessingEnumerable)
            {
                // A nested object within an enumerable.
                Type objType = state.Current.JsonPropertyInfo.CollectionElementType;
                state.Push();
                state.Current.Initialize(objType, options);
            }
            else if (state.Current.JsonPropertyInfo != null)
            {
                // Nested object.
                Type objType = state.Current.JsonPropertyInfo.RuntimePropertyType;
                state.Push();
                state.Current.Initialize(objType, options);
            }

            state.Current.ReturnValue = state.Current.JsonClassInfo.CreateObject();
        }

        private static void HandleEndObject(JsonSerializerOptions options, ref ReadStack state)
        {
            // Only allow dictionaries to be processed here if this is the DataExtensionProperty.
            Debug.Assert(!state.Current.IsProcessingDictionary || state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo);

            // Check if we are trying to build the sorted cache.
            if (state.Current.PropertyRefCache != null)
            {
                state.Current.JsonClassInfo.UpdateSortedPropertyCache(ref state.Current);
            }

            object value = state.Current.ReturnValue;

            if (state.IsLastFrame)
            {
                state.Current.Reset();
                state.Current.ReturnValue = value;
            }
            else
            {
                state.Pop();
                if (state.Current.IsProcessingEnumerableOrDictionary)
                {
                    // Outer enumerable or dictionary.
                    ApplyValueToEnumerable(options, ref state, ref value);
                }
            }
        }
    }
}
