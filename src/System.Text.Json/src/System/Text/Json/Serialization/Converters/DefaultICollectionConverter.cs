// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultICollectionConverter : JsonTemporaryListConverter
    {
        // Cache factories for performance.
        private static readonly ConcurrentDictionary<string, JsonEnumerableConverterState.WrappedEnumerableFactory> s_factories = new ConcurrentDictionary<string, JsonEnumerableConverterState.WrappedEnumerableFactory>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return JsonClassInfo.IsDeserializedByConstructingWithIList(implementedCollectionType);
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            Type implementedCollectionPropertyType = jsonPropertyInfo.ImplementedCollectionPropertyType;

            if (jsonPropertyInfo.DeclaredPropertyType.IsInterface)
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

            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;
            Type collectionType = jsonPropertyInfo.RuntimePropertyType;
            Type implementedCollectionType = jsonPropertyInfo.ImplementedCollectionPropertyType;

            if (implementedCollectionType == typeof(ArrayList))
            {
                return new ArrayList(sourceList);
            }

            return CreateEnumerableInstance(collectionType, sourceList, options);
        }

        private object CreateEnumerableInstance(Type collectionType, IList temporaryList, JsonSerializerOptions options)
        {
            Debug.Assert(collectionType != null);

            JsonEnumerableConverterState.WrappedEnumerableFactory factory =
                s_factories.GetOrAdd(collectionType.FullName, _ =>
                    options.MemberAccessorStrategy.CreateWrappedEnumerableFactoryConstructor(collectionType, temporaryList.GetType())(options));

            return factory.CreateFromList(temporaryList);
        }
    }
}
