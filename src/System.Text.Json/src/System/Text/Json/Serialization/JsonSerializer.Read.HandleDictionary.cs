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

            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;

            if (state.Current.CollectionPropertyInitialized)
            {
                // A nested object or dictionary so push new frame.
                Type elementType = jsonPropertyInfo.CollectionElementType;

                state.Push();
                state.Current.Initialize(elementType, options);
                HandleEndDictionary(options, ref state);
                return;
            }

            state.Current.CollectionPropertyInitialized = true;

            Debug.Assert(jsonPropertyInfo?.DictionaryConverter != null);

            jsonPropertyInfo.DictionaryConverter.BeginDictionary(ref state, options);
        }

        private static void HandleEndDictionary(
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            Debug.Assert(state.Current.JsonPropertyInfo?.DictionaryConverter != null);

            object DictionaryInstance = state.Current.JsonPropertyInfo.DictionaryConverter.EndDictionary(ref state, options);

            state.Current.EndProperty();

            if (state.IsLastFrame)
            {
                // Set the return value directly since this will be returned to the user.
                state.Current.Reset();
                state.Current.ReturnValue = DictionaryInstance;
            }
            else
            {
                state.Pop();

                if (state.Current.IsProcessingEnumerableOrDictionary)
                {
                    // Outer enumerable or dictionary.
                    ApplyValueToEnumerable(options, ref state, ref DictionaryInstance);
                }
            }
        }
    }
}
