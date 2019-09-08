// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultICollectionConverter : JsonTemporaryListConverter
    {
        // Cache factories for performance.
        private static readonly Dictionary<string, JsonEnumerableConverterState.WrappedEnumerableFactory> s_factories = new Dictionary<string, JsonEnumerableConverterState.WrappedEnumerableFactory>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return JsonClassInfo.IsDeserializedByConstructingWithIList(implementedCollectionType);
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
                        case JsonClassInfo.ReadOnlyCollectionGenericInterfaceTypeName:
                        case JsonClassInfo.ReadOnlyListGenericInterfaceTypeName:
                            return typeof(ReadOnlyCollection<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
                    }
                }

                ThrowHelper.ThrowInvalidOperationException_DeserializePolymorphicInterface(implementedCollectionPropertyType);
            }

            return jsonPropertyInfo.DeclaredPropertyType;
        }

        protected override Type ResolveTemporaryListType(JsonPropertyInfo jsonPropertyInfo)
        {
            Type implementedCollectionPropertyType = jsonPropertyInfo.ImplementedCollectionPropertyType;

            if (implementedCollectionPropertyType.IsGenericType &&
                implementedCollectionPropertyType.GetGenericTypeDefinition().FullName == JsonClassInfo.ReadOnlyObservableCollectionGenericTypeName)
            {
                return implementedCollectionPropertyType.Assembly.GetType(JsonClassInfo.ObservableCollectionGenericTypeName);
            }

            return base.ResolveTemporaryListType(jsonPropertyInfo);
        }

        public override object EndEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.TemporaryList != null);

            IList sourceList = state.Current.EnumerableConverterState.TemporaryList;
            Type collectionType = state.Current.JsonPropertyInfo.RuntimePropertyType;

            try
            {
                if (collectionType.IsGenericType)
                {
                    switch (collectionType.GetGenericTypeDefinition().FullName)
                    {
                        case JsonClassInfo.ReadOnlyCollectionGenericTypeName:
                        case JsonClassInfo.ReadOnlyObservableCollectionGenericTypeName:
                        case JsonClassInfo.StackGenericTypeName:
                        case JsonClassInfo.QueueGenericTypeName:
                        case JsonClassInfo.SortedSetGenericTypeName:
                            return CreateEnumerableInstance(
                                collectionType,
                                sourceList,
                                options);
                    }
                }
                else
                {
                    if (collectionType == typeof(ArrayList))
                    {
                        return new ArrayList(sourceList);
                    }

                    switch (collectionType.FullName)
                    {
                        case JsonClassInfo.StackTypeName:
                        case JsonClassInfo.QueueTypeName:
                            return CreateEnumerableInstance(
                                collectionType,
                                sourceList,
                                options);
                    }
                }

                return Activator.CreateInstance(collectionType, state.Current.EnumerableConverterState.TemporaryList);
            }
            catch (MissingMethodException)
            {
                ThrowHelper.ThrowNotSupportedException_DeserializeInstanceConstructorOfTypeNotFound(state.Current.JsonPropertyInfo.DeclaredPropertyType, state.Current.EnumerableConverterState.TemporaryList.GetType());
                return null;
            }
        }

        private object CreateEnumerableInstance(Type collectionType, IList temporaryList, JsonSerializerOptions options)
        {
            Debug.Assert(collectionType != null);

            string key = collectionType.FullName;

            if (!s_factories.TryGetValue(key, out JsonEnumerableConverterState.WrappedEnumerableFactory factory))
            {
                factory = options.MemberAccessorStrategy.CreateWrappedEnumerableFactoryConstructor(collectionType, temporaryList.GetType())(options);
                s_factories[key] = factory;
            }

            return factory.CreateFromList(temporaryList);
        }
    }
}
