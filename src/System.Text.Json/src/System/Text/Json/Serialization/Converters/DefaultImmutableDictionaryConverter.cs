// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultImmutableDictionaryConverter : JsonTemporaryDictionaryConverter
    {
        public const string ImmutableDictionaryTypeName = "System.Collections.Immutable.ImmutableDictionary";
        public const string ImmutableDictionaryGenericTypeName = "System.Collections.Immutable.ImmutableDictionary`2";
        public const string ImmutableDictionaryGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableDictionary`2";

        public const string ImmutableSortedDictionaryTypeName = "System.Collections.Immutable.ImmutableSortedDictionary";
        public const string ImmutableSortedDictionaryGenericTypeName = "System.Collections.Immutable.ImmutableSortedDictionary`2";

        public static void RegisterImmutableDictionary(Type immutableCollectionType, Type elementType, JsonSerializerOptions options)
        {
            // Get a unique identifier for a delegate which will point to the appropiate CreateRange method.
            string delegateKey = DefaultImmutableEnumerableConverter.GetDelegateKey(immutableCollectionType, elementType, out Type underlyingType, out string constructingTypeName);

            // Exit if we have registered this immutable dictionary type.
            if (options.CreateRangeDelegatesContainsKey(delegateKey))
            {
                return;
            }

            // Get the constructing type.
            Type constructingType = underlyingType.Assembly.GetType(constructingTypeName);

            // Create a delegate which will point to the CreateRange method.
            ImmutableCollectionCreator createRangeDelegate;
            createRangeDelegate = options.MemberAccessorStrategy.ImmutableDictionaryCreateRange(constructingType, immutableCollectionType, elementType);

            // Cache the delegate
            options.TryAddCreateRangeDelegate(delegateKey, createRangeDelegate);
        }

        public override bool OwnsImplementedCollectionType(Type declaredPropertyType, Type implementedCollectionType, Type collectionElementType)
        {
            return declaredPropertyType.FullName.StartsWith(JsonClassInfo.ImmutableNamespaceName);
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            return jsonPropertyInfo.RuntimePropertyType;
        }

        public override object EndDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState?.TemporaryDictionary != null);

            Type immutableCollectionType = state.Current.JsonPropertyInfo.RuntimePropertyType;
            Type collectionElementType = state.Current.JsonPropertyInfo.CollectionElementType;

            string delegateKey = DefaultImmutableEnumerableConverter.GetDelegateKey(immutableCollectionType, collectionElementType, out _, out _);

            return CreateImmutableDictionaryInstance(ref state, immutableCollectionType, delegateKey, state.Current.DictionaryConverterState.TemporaryDictionary, options);
        }

        // Creates an IEnumerable<TRuntimePropertyType> and populates it with the items in the
        // sourceList argument then uses the delegateKey argument to identify the appropriate cached
        // CreateRange<TRuntimePropertyType> method to create and return the desired immutable collection type.
        public static IDictionary CreateImmutableDictionaryInstance(ref ReadStack state, Type collectionType, string delegateKey, IDictionary sourceDictionary, JsonSerializerOptions options)
        {
            IDictionary collection = null;

            if (!options.TryGetCreateRangeDelegate(delegateKey, out ImmutableCollectionCreator creator) ||
                !creator.CreateImmutableDictionary(sourceDictionary, out collection))
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(collectionType, state.JsonPath());
            }

            return collection;
        }
    }
}
