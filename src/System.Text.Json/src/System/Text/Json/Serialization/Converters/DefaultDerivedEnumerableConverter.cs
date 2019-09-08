// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultDerivedEnumerableConverter : JsonEnumerableConverter
    {
        // Cache constructors for performance.
        private static readonly Dictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new Dictionary<string, JsonClassInfo.ConstructorDelegate>();
        private static readonly Dictionary<string, JsonEnumerableConverterState.CollectionBuilderConstructorDelegate> s_collectonBuilderCtors = new Dictionary<string, JsonEnumerableConverterState.CollectionBuilderConstructorDelegate>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return typeof(IList).IsAssignableFrom(implementedCollectionType) ||
                (implementedCollectionType.IsGenericType && typeof(ICollection<>).MakeGenericType(collectionElementType).IsAssignableFrom(implementedCollectionType));
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
                Type collectionType = typeof(JsonEnumerableConverterState.CollectionBuilder<>).MakeGenericType(state.Current.JsonPropertyInfo.CollectionElementType);

                state.Current.EnumerableConverterState = new JsonEnumerableConverterState
                {
                    FinalCollection = CreateCollectionBuilderInstance(collectionType, instance, options)
                };
            }
        }

        public override void AddItemToEnumerable(ref ReadStack state, JsonSerializerOptions options, object value)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.FinalList != null ||
                state.Current.EnumerableConverterState.FinalCollection != null);

            if (state.Current.EnumerableConverterState.FinalList != null)
            {
                state.Current.EnumerableConverterState.FinalList.Add(value);
            }
            else
            {
                state.Current.EnumerableConverterState.FinalCollection.Add(value);
            }
        }

        public override object EndEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.FinalList != null ||
                state.Current.EnumerableConverterState.FinalCollection != null);

            return state.Current.EnumerableConverterState.FinalList ?? state.Current.EnumerableConverterState.FinalCollection.Instance;
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

        private JsonEnumerableConverterState.CollectionBuilder CreateCollectionBuilderInstance(Type collectionType, object instance, JsonSerializerOptions options)
        {
            JsonEnumerableConverterState.CollectionBuilderConstructorDelegate ctor = FindCachedCollectionBuilderCtor(collectionType, options);
            Debug.Assert(ctor != null);
            return ctor(instance);
        }

        private JsonClassInfo.ConstructorDelegate FindCachedCtor(Type type, JsonSerializerOptions options)
        {
            string key = type.FullName;

            if (!s_ctors.TryGetValue(key, out JsonClassInfo.ConstructorDelegate ctor))
            {
                ctor = options.MemberAccessorStrategy.CreateConstructor(type);
                s_ctors[key] = ctor;
            }

            return ctor;
        }

        private JsonEnumerableConverterState.CollectionBuilderConstructorDelegate FindCachedCollectionBuilderCtor(Type collectionType, JsonSerializerOptions options)
        {
            string key = collectionType.FullName;

            if (!s_collectonBuilderCtors.TryGetValue(key, out JsonEnumerableConverterState.CollectionBuilderConstructorDelegate ctor))
            {
                ctor = options.MemberAccessorStrategy.CreateCollectionBuilderConstructor(collectionType);
                s_collectonBuilderCtors[key] = ctor;
            }

            return ctor;
        }
    }
}
