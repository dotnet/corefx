// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultDerivedDictionaryConverter : JsonDictionaryConverter
    {
        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            throw new NotImplementedException();
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            if (jsonPropertyInfo.DeclaredPropertyType.IsInterface)
            {
                return typeof(Dictionary<,>).MakeGenericType(typeof(string), jsonPropertyInfo.CollectionElementType);
            }

            return jsonPropertyInfo.DeclaredPropertyType;
        }

        public override void BeginDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState == null);

            if (state.Current.JsonPropertyInfo.DeclaredPropertyType.IsInterface)
            {
                state.Current.DictionaryConverterState = new JsonDictionaryConverterState
                {
                    FinalInstance = (IDictionary)state.Current.JsonPropertyInfo.RuntimeClassInfo.CreateObject()
                };
            }
            else if (state.Current.JsonPropertyInfo.DeclaredPropertyType == state.Current.JsonPropertyInfo.RuntimePropertyType)
            {
                state.Current.DictionaryConverterState = new JsonDictionaryConverterState
                {
                    FinalInstance = (IDictionary)state.Current.JsonPropertyInfo.DeclaredClassInfo.CreateObject()
                };
            }
            else
            {
                state.Current.DictionaryConverterState = new JsonDictionaryConverterState
                {
                    TemporaryDictionary = (IDictionary)state.Current.JsonPropertyInfo.RuntimeClassInfo.CreateObject()
                };
            }
        }

        public override void AddItemToDictionary(ref ReadStack state, JsonSerializerOptions options, string key, object value)
        {
            Debug.Assert(state.Current.DictionaryConverterState == null);

            JsonDictionaryConverterState convertState = state.Current.DictionaryConverterState;

            (convertState.FinalInstance ?? convertState.TemporaryDictionary).Add(key, value);
        }

        public override object EndDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState != null);

            JsonDictionaryConverterState convertState = state.Current.DictionaryConverterState;

            if (convertState.FinalInstance != null)
                return convertState.FinalInstance;

            object instance = state.Current.JsonPropertyInfo.DeclaredClassInfo.CreateObject();

            if (instance is IDictionary instanceOfIDictionary)
            {
                if (!instanceOfIDictionary.IsReadOnly)
                {
                    foreach (DictionaryEntry entry in convertState.TemporaryDictionary)
                    {
                        instanceOfIDictionary.Add((string)entry.Key, entry.Value);
                    }
                    return instanceOfIDictionary;
                }
            }
            /*
            else if (instance is IDictionary<string, TRuntimeProperty> instanceOfGenericIDictionary)
            {
                if (!instanceOfGenericIDictionary.IsReadOnly)
                {
                    foreach (DictionaryEntry entry in sourceDictionary)
                    {
                        instanceOfGenericIDictionary.Add((string)entry.Key, (TRuntimeProperty)entry.Value);
                    }
                    return instanceOfGenericIDictionary;
                }
            }
            */

            ThrowHelper.ThrowNotSupportedException_SerializationNotSupportedCollection(
                state.Current.JsonPropertyInfo.DeclaredPropertyType,
                state.Current.JsonPropertyInfo.ParentClassType,
                state.Current.JsonPropertyInfo.PropertyInfo);
            return null;
        }
    }
}
