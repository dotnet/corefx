// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    internal partial class JsonClassInfo
    {
        public const string ImmutableNamespaceName = "System.Collections.Immutable";

        private const string EnumerableGenericInterfaceTypeName = "System.Collections.Generic.IEnumerable`1";
        private const string EnumerableInterfaceTypeName = "System.Collections.IEnumerable";

        private const string ListInterfaceTypeName = "System.Collections.IList";
        private const string ListGenericInterfaceTypeName = "System.Collections.Generic.IList`1";
        private const string ListGenericTypeName = "System.Collections.Generic.List`1";

        public const string CollectionGenericInterfaceTypeName = "System.Collections.Generic.ICollection`1";
        private const string CollectionGenericTypeName = "System.Collections.ObjectModel.Collection`1";
        public const string ObservableCollectionGenericTypeName = "System.Collections.ObjectModel.ObservableCollection`1";
        private const string CollectionInterfaceTypeName = "System.Collections.ICollection";

        public const string ReadOnlyListGenericInterfaceTypeName = "System.Collections.Generic.IReadOnlyList`1";

        public const string ReadOnlyCollectionGenericInterfaceTypeName = "System.Collections.Generic.IReadOnlyCollection`1";
        public const string ReadOnlyCollectionGenericTypeName = "System.Collections.ObjectModel.ReadOnlyCollection`1";
        public const string ReadOnlyObservableCollectionGenericTypeName = "System.Collections.ObjectModel.ReadOnlyObservableCollection`1";

        public const string HashtableTypeName = "System.Collections.Hashtable";
        public const string SortedListTypeName = "System.Collections.SortedList";

        public const string StackTypeName = "System.Collections.Stack";
        public const string StackGenericTypeName = "System.Collections.Generic.Stack`1";

        public const string QueueTypeName = "System.Collections.Queue";
        public const string QueueGenericTypeName = "System.Collections.Generic.Queue`1";

        public const string SetGenericInterfaceTypeName = "System.Collections.Generic.ISet`1";
        public const string SortedSetGenericTypeName = "System.Collections.Generic.SortedSet`1";
        public const string HashSetGenericTypeName = "System.Collections.Generic.HashSet`1";

        public const string LinkedListGenericTypeName = "System.Collections.Generic.LinkedList`1";

        public const string DictionaryInterfaceTypeName = "System.Collections.IDictionary";
        public const string DictionaryGenericTypeName = "System.Collections.Generic.Dictionary`2";
        public const string DictionaryGenericInterfaceTypeName = "System.Collections.Generic.IDictionary`2";
        public const string ReadOnlyDictionaryGenericInterfaceTypeName = "System.Collections.Generic.IReadOnlyDictionary`2";
        public const string ReadOnlyDictionaryGenericTypeName = "System.Collections.ObjectModel.ReadOnlyDictionary`2";
        public const string SortedDictionaryGenericTypeName = "System.Collections.Generic.SortedDictionary`2";
        public const string KeyValuePairGenericTypeName = "System.Collections.Generic.KeyValuePair`2";

        public const string ArrayListTypeName = "System.Collections.ArrayList";

        // Any additional natively supported generic collection must be registered here.
        private static readonly HashSet<string> s_nativelySupportedGenericCollections = new HashSet<string>()
        {
            ListGenericTypeName,
            EnumerableGenericInterfaceTypeName,
            ListGenericInterfaceTypeName,
            CollectionGenericInterfaceTypeName,
            CollectionGenericTypeName,
            ObservableCollectionGenericTypeName,
            ReadOnlyListGenericInterfaceTypeName,
            ReadOnlyCollectionGenericInterfaceTypeName,
            ReadOnlyCollectionGenericTypeName,
            ReadOnlyObservableCollectionGenericTypeName,
            SetGenericInterfaceTypeName,
            StackGenericTypeName,
            QueueGenericTypeName,
            HashSetGenericTypeName,
            LinkedListGenericTypeName,
            SortedSetGenericTypeName,
            DictionaryInterfaceTypeName,
            DictionaryGenericTypeName,
            DictionaryGenericInterfaceTypeName,
            ReadOnlyDictionaryGenericInterfaceTypeName,
            ReadOnlyDictionaryGenericTypeName,
            SortedDictionaryGenericTypeName,
            KeyValuePairGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableArrayGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableListGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableListGenericInterfaceTypeName,
            DefaultImmutableEnumerableConverter.ImmutableStackGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableStackGenericInterfaceTypeName,
            DefaultImmutableEnumerableConverter.ImmutableQueueGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableQueueGenericInterfaceTypeName,
            DefaultImmutableEnumerableConverter.ImmutableSortedSetTypeName,
            DefaultImmutableEnumerableConverter.ImmutableSortedSetGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableHashSetGenericTypeName,
            DefaultImmutableEnumerableConverter.ImmutableSetGenericInterfaceTypeName,
            DefaultImmutableDictionaryConverter.ImmutableDictionaryGenericTypeName,
            DefaultImmutableDictionaryConverter.ImmutableDictionaryGenericInterfaceTypeName,
            DefaultImmutableDictionaryConverter.ImmutableSortedDictionaryGenericTypeName,
        };

        // Any additional natively supported non-generic collection must be registered here.
        private static readonly HashSet<string> s_nativelySupportedNonGenericCollections = new HashSet<string>()
        {
            EnumerableInterfaceTypeName,
            CollectionInterfaceTypeName,
            ListInterfaceTypeName,
            DictionaryInterfaceTypeName,
            StackTypeName,
            QueueTypeName,
            HashtableTypeName,
            ArrayListTypeName,
            SortedListTypeName,
        };

        public static Type GetImplementedCollectionType(
            Type parentClassType,
            Type queryType,
            PropertyInfo propertyInfo,
            out JsonConverter converter,
            JsonSerializerOptions options)
        {
            Debug.Assert(queryType != null);

            if (!typeof(IEnumerable).IsAssignableFrom(queryType) ||
                queryType == typeof(string) ||
                queryType.IsInterface ||
                queryType.IsArray ||
                IsNativelySupportedCollection(queryType))
            {
                converter = null;
                return queryType;
            }

            // If a converter was provided, we should not detect implemented types and instead use the converter later.
            converter = options.DetermineConverterForProperty(parentClassType, queryType, propertyInfo);
            if (converter != null)
            {
                return queryType;
            }

            Type baseType = queryType.BaseType;
            while (baseType != null)
            {
                // Check if the base type is a supported concrete collection.
                if (IsNativelySupportedCollection(baseType))
                {
                    return baseType;
                }
                baseType = baseType.BaseType;
            }

            foreach (Type implementedInterface in queryType.GetInterfaces())
            {
                if (implementedInterface.IsGenericType)
                {
                    Type genericTypeDefinition = implementedInterface.GetGenericTypeDefinition();
                    if (IsNativelySupportedCollection(genericTypeDefinition))
                    {
                        return implementedInterface;
                    }
                }
                else if (IsNativelySupportedCollection(implementedInterface))
                {
                    return implementedInterface;
                }
            }

            return typeof(IEnumerable);
        }

        public static bool IsDeserializedByConstructingWithIList(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.IsInterface && type.FullName.StartsWith(ImmutableNamespaceName))
                    return true;

                switch (type.GetGenericTypeDefinition().FullName)
                {
                    // interfaces
                    case ReadOnlyCollectionGenericInterfaceTypeName:
                    case ReadOnlyListGenericInterfaceTypeName:
                    // types
                    case ReadOnlyCollectionGenericTypeName:
                    case ReadOnlyObservableCollectionGenericTypeName:
                    case StackGenericTypeName:
                    case QueueGenericTypeName:
                        return true;
                    default:
                        return false;
                }
            }

            switch (type.FullName)
            {
                case StackTypeName:
                case QueueTypeName:
                case ArrayListTypeName:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDeserializedByConstructingWithIDictionary(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.IsInterface && type.FullName.StartsWith(ImmutableNamespaceName))
                    return true;

                switch (type.GetGenericTypeDefinition().FullName)
                {
                    case ReadOnlyDictionaryGenericInterfaceTypeName:
                    case ReadOnlyDictionaryGenericTypeName:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        public static bool IsNativelySupportedCollection(Type queryType)
        {
            Debug.Assert(queryType != null);

            if (queryType.IsGenericType)
            {
                return s_nativelySupportedGenericCollections.Contains(queryType.GetGenericTypeDefinition().FullName);
            }

            return s_nativelySupportedNonGenericCollections.Contains(queryType.FullName);
        }
    }
}
