// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleStartArray(
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;

            if (state.Current.CollectionPropertyInitialized)
            {
                // A nested json array so push a new stack frame.
                Type elementType = jsonPropertyInfo.CollectionElementType;

                state.Push();
                state.Current.Initialize(elementType, options);
                HandleStartArray(options, ref state);
                return;
            }

            state.Current.CollectionPropertyInitialized = true;

            Debug.Assert(jsonPropertyInfo?.EnumerableConverter != null);

            jsonPropertyInfo.EnumerableConverter.BeginEnumerable(ref state, options);
        }

        private static void HandleEndArray(
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            Debug.Assert(state.Current.JsonPropertyInfo?.EnumerableConverter != null);

            object EnumerableInstance = state.Current.JsonPropertyInfo.EnumerableConverter.EndEnumerable(ref state, options);

            if (state.Current.IsEnumerableProperty)
            {
                // Set instance as property on currently building object.
                state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, EnumerableInstance);
                state.Current.EndProperty();
            }
            else
            {
                if (state.IsLastFrame)
                {
                    // Set the return value directly since this will be returned to the user.
                    state.Current.Reset();
                    state.Current.ReturnValue = EnumerableInstance;
                }
                else
                {
                    state.Pop();

                    Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

                    // Outer enumerable or dictionary.
                    ApplyValueToEnumerable(options, ref state, ref EnumerableInstance);
                }
            }
        }

        internal static void ApplyValueToEnumerable<TProperty>(
            JsonSerializerOptions options,
            ref ReadStack state,
            ref TProperty value)
        {
            Debug.Assert(state.Current.JsonPropertyInfo != null);
            Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

            if (state.Current.IsProcessingEnumerable)
            {
                state.Current.JsonPropertyInfo.EnumerableConverter.AddItemToEnumerable(ref state, options, ref value);
            }
            else if (state.Current.IsProcessingDictionary)
            {
                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                state.Current.JsonPropertyInfo.DictionaryConverter.AddItemToDictionary(ref state, options, key, ref value);
            }
        }
    }
}
