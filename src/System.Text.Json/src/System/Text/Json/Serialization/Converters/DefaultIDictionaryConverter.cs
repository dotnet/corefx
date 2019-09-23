// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultIDictionaryConverter : JsonTemporaryDictionaryConverter
    {
        // Cache factories for performance.
        private static readonly ConcurrentDictionary<string, JsonDictionaryConverterState.WrappedDictionaryFactory> s_factories = new ConcurrentDictionary<string, JsonDictionaryConverterState.WrappedDictionaryFactory>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return JsonClassInfo.IsDeserializedByConstructingWithIDictionary(implementedCollectionType);
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
                        case JsonClassInfo.ReadOnlyDictionaryGenericInterfaceTypeName:
                            return implementedCollectionPropertyType.Assembly.GetType(JsonClassInfo.ReadOnlyDictionaryGenericTypeName).MakeGenericType(typeof(string), jsonPropertyInfo.CollectionElementType);
                    }
                }

                ThrowHelper.ThrowInvalidOperationException_DeserializePolymorphicInterface(implementedCollectionPropertyType);
            }

            return jsonPropertyInfo.DeclaredPropertyType;
        }

        public override object EndDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState?.TemporaryDictionary != null);

            IDictionary sourceDictionary = state.Current.DictionaryConverterState.TemporaryDictionary;

            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;
            Type dictionaryType = jsonPropertyInfo.RuntimePropertyType;
            Type implementedCollectionType = jsonPropertyInfo.ImplementedCollectionPropertyType;

            if (implementedCollectionType == typeof(Hashtable))
            {
                return new Hashtable(sourceDictionary);
            }

            return CreateDictionaryInstance(dictionaryType, sourceDictionary, options);
        }

        private object CreateDictionaryInstance(Type dictionaryType, IDictionary temporaryDictionary, JsonSerializerOptions options)
        {
            Debug.Assert(dictionaryType != null);

            JsonDictionaryConverterState.WrappedDictionaryFactory factory =
                s_factories.GetOrAdd(dictionaryType.FullName, _ =>
                    options.MemberAccessorStrategy.CreateWrappedDictionaryFactoryConstructor(dictionaryType, temporaryDictionary.GetType())(options));

            return factory.CreateFromDictionary(temporaryDictionary);
        }
    }
}
