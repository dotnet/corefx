// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultDerivedDictionaryConverter : JsonDictionaryConverter
    {
        // Cache constructors for performance.
        private static readonly ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate>();
        private static readonly ConcurrentDictionary<string, JsonDictionaryConverterState.DictionaryBuilderConstructorDelegate> s_dictionaryBuilderCtors = new ConcurrentDictionary<string, JsonDictionaryConverterState.DictionaryBuilderConstructorDelegate>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return typeof(IDictionary).IsAssignableFrom(implementedCollectionType) ||
                (implementedCollectionType.IsGenericType && typeof(IDictionary<,>).MakeGenericType(typeof(string), collectionElementType).IsAssignableFrom(implementedCollectionType));
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
            Debug.Assert(state.Current.DictionaryConverterState == null);

            object instance = CreateConcreteInstance(ref state, options);

            if (instance is IDictionary dictionary)
            {
                state.Current.DictionaryConverterState = new JsonDictionaryConverterState
                {
                    FinalDictionary = dictionary
                };
            }
            else
            {
                Type dictionaryType = typeof(JsonDictionaryConverterState.DictionaryBuilder<>).MakeGenericType(state.Current.JsonPropertyInfo.CollectionElementType);

                state.Current.DictionaryConverterState = new JsonDictionaryConverterState
                {
                    Builder = CreateDictionaryBuilderInstance(dictionaryType, instance, options)
                };
            }
        }

        public override void AddItemToDictionary<T>(ref ReadStack state, JsonSerializerOptions options, string key, ref T value)
        {
            Debug.Assert(state.Current.DictionaryConverterState?.FinalDictionary != null ||
               state.Current.DictionaryConverterState.Builder != null);

            IDictionary finalDictionary = state.Current.DictionaryConverterState.FinalDictionary;

            if (finalDictionary != null)
            {
                if (finalDictionary is IDictionary<string, T> typedDictionary)
                {
                    typedDictionary[key] = value;
                }
                else
                {
                    finalDictionary[key] = value;
                }
            }
            else
            {
                state.Current.DictionaryConverterState.Builder.Add(key, ref value);
            }
        }

        public override object EndDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState?.FinalDictionary != null ||
                state.Current.DictionaryConverterState.Builder != null);

            return state.Current.DictionaryConverterState.FinalDictionary ?? state.Current.DictionaryConverterState.Builder.Instance;
        }

        private object CreateConcreteInstance(ref ReadStack state, JsonSerializerOptions options)
        {
            if (state.Current.JsonPropertyInfo.DeclaredPropertyType.IsInterface)
            {
                JsonClassInfo.ConstructorDelegate ctor = FindCachedCtor(state.Current.JsonPropertyInfo.RuntimePropertyType, options);
                if (ctor == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_DeserializePolymorphicInterface(state.Current.JsonPropertyInfo.RuntimePropertyType);
                }
                return ctor();
            }
            else
            {
                return state.Current.JsonPropertyInfo.DeclaredClassInfo.CreateObject();
            }
        }

        private JsonDictionaryConverterState.DictionaryBuilder CreateDictionaryBuilderInstance(Type dictionaryType, object instance, JsonSerializerOptions options)
        {
            JsonDictionaryConverterState.DictionaryBuilderConstructorDelegate ctor = FindCachedDictionaryBuilderCtor(dictionaryType, options);
            Debug.Assert(ctor != null);
            return ctor(instance);
        }

        private JsonClassInfo.ConstructorDelegate FindCachedCtor(Type type, JsonSerializerOptions options)
        {
            return s_ctors.GetOrAdd(type.FullName, _ => options.MemberAccessorStrategy.CreateConstructor(type));
        }

        private JsonDictionaryConverterState.DictionaryBuilderConstructorDelegate FindCachedDictionaryBuilderCtor(Type dictionaryType, JsonSerializerOptions options)
        {
            return s_dictionaryBuilderCtors.GetOrAdd(dictionaryType.FullName, _ => options.MemberAccessorStrategy.CreateDictionaryBuilderConstructor(dictionaryType));
        }
    }
}
