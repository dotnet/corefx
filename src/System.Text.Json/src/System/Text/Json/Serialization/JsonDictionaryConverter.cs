// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonDictionaryConverterState
    {
        public IDictionary TemporaryDictionary;
        public IDictionary FinalInstance;
    }

    internal abstract class JsonTemporaryDictionaryConverter : JsonDictionaryConverter
    {
        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            // Should runtimetype be something else?

            return typeof(Dictionary<,>).MakeGenericType(typeof(string), jsonPropertyInfo.CollectionElementType);
        }

        public override void BeginDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState == null);

            state.Current.DictionaryConverterState = new JsonDictionaryConverterState
            {
                TemporaryDictionary = (IDictionary)state.Current.JsonPropertyInfo.RuntimeClassInfo.CreateObject()
            };
        }

        public override void AddItemToDictionary(ref ReadStack state, JsonSerializerOptions options, string key, object value)
        {
            Debug.Assert(state.Current.DictionaryConverterState?.TemporaryDictionary != null);

            state.Current.DictionaryConverterState.TemporaryDictionary.Add(key, value);
        }
    }

    // Helper to deserialize data into collections that store key-value pairs (not including KeyValuePair<,>)
    // e.g. IDictionary, Hashtable, Dictionary<,> IDictionary<,>, SortedList etc.
    // We'll call these collections "dictionaries".
    // Note: the KeyValuePair<,> type has a value converter, so its deserialization flow will not reach here.
    // Also, KeyValuePair<,> is sealed, so deserialization will flow here to support custom types that
    // implement KeyValuePair<,>.
    internal abstract class JsonDictionaryConverter
    {
        public abstract bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType);
        public abstract Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo);
        public abstract void BeginDictionary(ref ReadStack state, JsonSerializerOptions options);
        public abstract void AddItemToDictionary(ref ReadStack state, JsonSerializerOptions options, string key, object value);
        public abstract object EndDictionary(ref ReadStack state, JsonSerializerOptions options);
    }
}
