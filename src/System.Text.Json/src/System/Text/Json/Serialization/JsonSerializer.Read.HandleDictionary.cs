// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleStartDictionary(
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

            state.Current.CollectionPropertyInitialized = true;

            Debug.Assert(state.Current.JsonPropertyInfo?.DictionaryConverter != null);

            state.Current.JsonPropertyInfo.DictionaryConverter.BeginDictionary(ref state, options);
        }

        private static void HandleEndDictionary(
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            Debug.Assert(state.Current.JsonPropertyInfo?.DictionaryConverter != null);

            object DictionaryInstance = state.Current.JsonPropertyInfo.DictionaryConverter.EndDictionary(ref state, options);

            if (state.Current.IsDictionaryProperty)
            {
                if (state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo)
                {
                    // Handle special case of DataExtensionProperty where we just added a dictionary element to the extension property.
                    // Since the JSON value is not a dictionary element (it's a normal property in JSON) a JsonTokenType.EndObject
                    // encountered here is from the outer object so forward to HandleEndObject().
                    HandleEndObject(options, ref state);
                }
                else
                {
                    // Set instance as property on currently building object.
                    state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, DictionaryInstance);
                    state.Current.EndProperty();
                }
            }
            else
            {
                if (state.IsLastFrame)
                {
                    // Set the return value directly since this will be returned to the user.
                    state.Current.Reset();
                    state.Current.ReturnValue = DictionaryInstance;
                }
                else
                {
                    state.Pop();

                    Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

                    // Outer enumerable or dictionary.
                    ApplyValueToEnumerable(options, ref state, ref DictionaryInstance);
                }
            }
        }
    }
}
