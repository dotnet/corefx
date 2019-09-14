// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultDerivedEnumerableConverter : JsonEnumerableConverter
    {
        // Cache constructors for performance.
        private static readonly ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate>();
        private static readonly ConcurrentDictionary<string, JsonEnumerableConverterState.CollectionBuilderConstructorDelegate> s_collectonBuilderCtors = new ConcurrentDictionary<string, JsonEnumerableConverterState.CollectionBuilderConstructorDelegate>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return typeof(IEnumerable).IsAssignableFrom(implementedCollectionType);
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            Type implementedCollectionPropertyType = jsonPropertyInfo.ImplementedCollectionPropertyType;

            if (implementedCollectionPropertyType.IsInterface)
            {
                if (implementedCollectionPropertyType.IsGenericType)
                {
                    switch (implementedCollectionPropertyType.GetGenericTypeDefinition().FullName)
                    {
                        case JsonClassInfo.SetGenericInterfaceTypeName:
                            return typeof(HashSet<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
                        case JsonClassInfo.CollectionGenericInterfaceTypeName:
                            return typeof(Collection<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
                    }
                }
                return typeof(List<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
            }

            return jsonPropertyInfo.DeclaredPropertyType;
        }

        public override void BeginEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState == null);

            object instance = CreateConcreteInstance(ref state, options);

            if (instance is IList list)
            {
                state.Current.EnumerableConverterState = new JsonEnumerableConverterState
                {
                    FinalList = list
                };
            }
            else
            {
                JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;

                Type collectionType = typeof(JsonEnumerableConverterState.CollectionBuilder<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);

                state.Current.EnumerableConverterState = new JsonEnumerableConverterState
                {
                    Builder = CreateCollectionBuilderInstance(collectionType, jsonPropertyInfo, instance, options)
                };
            }
        }

        public override void AddItemToEnumerable<T>(ref ReadStack state, JsonSerializerOptions options, ref T value)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.FinalList != null ||
                state.Current.EnumerableConverterState.Builder != null);

            IList finalList = state.Current.EnumerableConverterState.FinalList;

            if (finalList != null)
            {
                if (finalList is IList<T> typedList)
                {
                    typedList.Add(value);
                }
                else
                {
                    finalList.Add(value);
                }
            }
            else
            {
                state.Current.EnumerableConverterState.Builder.Add(ref value);
            }
        }

        public override object EndEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.FinalList != null ||
                state.Current.EnumerableConverterState.Builder != null);

            return state.Current.EnumerableConverterState.FinalList ?? state.Current.EnumerableConverterState.Builder.Instance;
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

        private JsonEnumerableConverterState.CollectionBuilder CreateCollectionBuilderInstance(Type collectionType, JsonPropertyInfo source, object instance, JsonSerializerOptions options)
        {
            JsonEnumerableConverterState.CollectionBuilderConstructorDelegate ctor = FindCachedCollectionBuilderCtor(collectionType, options);
            Debug.Assert(ctor != null);
            return ctor(source, instance);
        }

        private JsonClassInfo.ConstructorDelegate FindCachedCtor(Type type, JsonSerializerOptions options)
        {
            return s_ctors.GetOrAdd(type.FullName, _ => options.MemberAccessorStrategy.CreateConstructor(type));
        }

        private JsonEnumerableConverterState.CollectionBuilderConstructorDelegate FindCachedCollectionBuilderCtor(Type collectionType, JsonSerializerOptions options)
        {
            return s_collectonBuilderCtors.GetOrAdd(collectionType.FullName, _ => options.MemberAccessorStrategy.CreateCollectionBuilderConstructor(collectionType));
        }
    }
}
