﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    [DebuggerDisplay("ClassType.{ClassType}, {Type.Name}")]
    internal sealed partial class JsonClassInfo
    {
        // The length of the property name embedded in the key (in bytes).
        private const int PropertyNameKeyLength = 6;

        // The limit to how many property names from the JSON are cached before using PropertyCache.
        private const int PropertyNameCountCacheThreshold = 64;

        // The properties on a POCO keyed on property name.
        public volatile Dictionary<string, JsonPropertyInfo> PropertyCache;

        // Cache of properties by first JSON ordering. Use an array for highest performance.
        private volatile PropertyRef[] _propertyRefsSorted = null;

        internal delegate object ConstructorDelegate();
        internal ConstructorDelegate CreateObject { get; private set; }

        internal ClassType ClassType { get; private set; }

        public JsonPropertyInfo DataExtensionProperty { get; private set; }

        // If enumerable, the JsonClassInfo for the element type.
        internal JsonClassInfo ElementClassInfo { get; private set; }

        public JsonSerializerOptions Options { get; private set; }

        public Type Type { get; private set; }

        internal void UpdateSortedPropertyCache(ref ReadStackFrame frame)
        {
            // Check if we are trying to build the sorted cache.
            if (frame.PropertyRefCache == null)
            {
                return;
            }

            List<PropertyRef> newList;
            if (_propertyRefsSorted != null)
            {
                newList = new List<PropertyRef>(_propertyRefsSorted);
                newList.AddRange(frame.PropertyRefCache);
                _propertyRefsSorted = newList.ToArray();
            }
            else
            {
                _propertyRefsSorted = frame.PropertyRefCache.ToArray();
            }

            frame.PropertyRefCache = null;
        }

        internal JsonClassInfo(Type type, JsonSerializerOptions options)
        {
            Type = type;
            Options = options;
            ClassType = GetClassType(type, options);

            CreateObject = options.MemberAccessorStrategy.CreateConstructor(type);

            // Ignore properties on enumerable.
            switch (ClassType)
            {
                case ClassType.Object:
                    {
                        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        Dictionary<string, JsonPropertyInfo> cache = CreatePropertyCache(properties.Length);

                        foreach (PropertyInfo propertyInfo in properties)
                        {
                            // Ignore indexers
                            if (propertyInfo.GetIndexParameters().Length > 0)
                            {
                                continue;
                            }

                            // For now we only support public getters\setters
                            if (propertyInfo.GetMethod?.IsPublic == true ||
                                propertyInfo.SetMethod?.IsPublic == true)
                            {
                                JsonPropertyInfo jsonPropertyInfo = AddProperty(propertyInfo.PropertyType, propertyInfo, type, options);
                                Debug.Assert(jsonPropertyInfo != null);

                                // If the JsonPropertyNameAttribute or naming policy results in collisions, throw an exception.
                                if (!JsonHelpers.TryAdd(cache, jsonPropertyInfo.NameAsString, jsonPropertyInfo))
                                {
                                    JsonPropertyInfo other = cache[jsonPropertyInfo.NameAsString];

                                    if (other.ShouldDeserialize == false && other.ShouldSerialize == false)
                                    {
                                        // Overwrite the one just added since it has [JsonIgnore].
                                        cache[jsonPropertyInfo.NameAsString] = jsonPropertyInfo;
                                    }
                                    else if (jsonPropertyInfo.ShouldDeserialize == true || jsonPropertyInfo.ShouldSerialize == true)
                                    {
                                        ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameConflict(this, jsonPropertyInfo);
                                    }
                                    // else ignore jsonPropertyInfo since it has [JsonIgnore].
                                }
                            }
                        }

                        // Set as a unit to avoid concurrency issues.
                        PropertyCache = cache;

                        DetermineExtensionDataProperty();
                    }
                    break;
                case ClassType.Enumerable:
                case ClassType.Dictionary:
                    {
                        // Add a single property that maps to the class type so we can have policies applied.
                         AddPolicyProperty(type, options);

                        // Use the type from the property policy to get any late-bound concrete types (from an interface like IDictionary).
                        CreateObject = options.MemberAccessorStrategy.CreateConstructor(PolicyProperty.RuntimePropertyType);

                        // Create a ClassInfo that maps to the element type which is used for (de)serialization and policies.
                        Type elementType = GetElementType(type, parentType: null, memberInfo: null, options: options);
                        ElementClassInfo = options.GetOrAddClass(elementType);
                    }
                    break;
                case ClassType.IDictionaryConstructible:
                    {
                        // Add a single property that maps to the class type so we can have policies applied.
                        AddPolicyProperty(type, options);

                        Type elementType = GetElementType(type, parentType: null, memberInfo: null, options: options);

                        CreateObject = options.MemberAccessorStrategy.CreateConstructor(
                            typeof(Dictionary<,>).MakeGenericType(typeof(string), elementType));

                        // Create a ClassInfo that maps to the element type which is used for (de)serialization and policies.
                        ElementClassInfo = options.GetOrAddClass(elementType);
                    }
                    break;
                case ClassType.Value:
                    // Add a single property that maps to the class type so we can have policies applied.
                    AddPolicyProperty(type, options);
                    break;
                case ClassType.Unknown:
                    // Add a single property that maps to the class type so we can have policies applied.
                    AddPolicyProperty(type, options);
                    PropertyCache = new Dictionary<string, JsonPropertyInfo>();
                    break;
                default:
                    Debug.Fail($"Unexpected class type: {ClassType}");
                    break;
            }
        }

        private void DetermineExtensionDataProperty()
        {
            JsonPropertyInfo jsonPropertyInfo = GetPropertyThatHasAttribute(typeof(JsonExtensionDataAttribute));
            if (jsonPropertyInfo != null)
            {
                Type declaredPropertyType = jsonPropertyInfo.DeclaredPropertyType;
                if (!typeof(IDictionary<string, JsonElement>).IsAssignableFrom(declaredPropertyType) &&
                    !typeof(IDictionary<string, object>).IsAssignableFrom(declaredPropertyType))
                {
                    ThrowHelper.ThrowInvalidOperationException_SerializationDataExtensionPropertyInvalid(this, jsonPropertyInfo);
                }

                DataExtensionProperty = jsonPropertyInfo;
            }
        }

        private JsonPropertyInfo GetPropertyThatHasAttribute(Type attributeType)
        {
            Debug.Assert(PropertyCache != null);

            JsonPropertyInfo property = null;

            foreach (JsonPropertyInfo jsonPropertyInfo in PropertyCache.Values)
            {
                Attribute attribute = jsonPropertyInfo.PropertyInfo.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    if (property != null)
                    {
                        ThrowHelper.ThrowInvalidOperationException_SerializationDuplicateTypeAttribute(Type, attributeType);
                    }

                    property = jsonPropertyInfo;
                }
            }

            return property;
        }

        internal JsonPropertyInfo GetProperty(ReadOnlySpan<byte> propertyName, ref ReadStackFrame frame)
        {
            JsonPropertyInfo info = null;

            // If we're not trying to build the cache locally, and there is an existing cache, then use it.
            if (_propertyRefsSorted != null)
            {
                ulong key = GetKey(propertyName);

                // First try sorted lookup.
                int propertyIndex = frame.PropertyIndex;

                // This .Length is consistent no matter what json data intialized _propertyRefsSorted.
                int count = _propertyRefsSorted.Length;
                if (count != 0)
                {
                    int iForward = Math.Min(propertyIndex, count);
                    int iBackward = iForward - 1;
                    while (iForward < count || iBackward >= 0)
                    {
                        if (iForward < count)
                        {
                            if (TryIsPropertyRefEqual(_propertyRefsSorted[iForward], propertyName, key, ref info))
                            {
                                return info;
                            }
                            ++iForward;
                        }

                        if (iBackward >= 0)
                        {
                            if (TryIsPropertyRefEqual(_propertyRefsSorted[iBackward], propertyName, key, ref info))
                            {
                                return info;
                            }
                            --iBackward;
                        }
                    }
                }
            }

            // Try the main list which has all of the properties in a consistent order.
            // We could get here even when hasPropertyCache==true if there is a race condition with different json
            // property ordering and _propertyRefsSorted is re-assigned while in the loop above.

            string stringPropertyName = JsonHelpers.Utf8GetString(propertyName);
            if (PropertyCache.TryGetValue(stringPropertyName, out info))
            {
                // For performance, only add to cache up to a threshold and then just use the dictionary.
                int count;
                if (_propertyRefsSorted != null)
                {
                    count = _propertyRefsSorted.Length;
                }
                else
                {
                    count = 0;
                }
                
                // Do a quick check for the stable (after warm-up) case.
                if (count < PropertyNameCountCacheThreshold)
                {
                    if (frame.PropertyRefCache != null)
                    {
                        count += frame.PropertyRefCache.Count;
                    }

                    // Check again to fill up to the limit.
                    if (count < PropertyNameCountCacheThreshold)
                    {
                        if (frame.PropertyRefCache == null)
                        {
                            frame.PropertyRefCache = new List<PropertyRef>();
                        }

                        ulong key = info.PropertyNameKey;
                        PropertyRef propertyRef = new PropertyRef(key, info);
                        frame.PropertyRefCache.Add(propertyRef);
                    }
                }
            }

            return info;
        }

        private Dictionary<string, JsonPropertyInfo> CreatePropertyCache(int capacity)
        {
            StringComparer comparer;

            if (Options.PropertyNameCaseInsensitive)
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }
            else
            {
                comparer = StringComparer.Ordinal;
            }

            return new Dictionary<string, JsonPropertyInfo>(capacity, comparer);
        }

        public JsonPropertyInfo PolicyProperty { get; private set; }

        private static bool TryIsPropertyRefEqual(in PropertyRef propertyRef, ReadOnlySpan<byte> propertyName, ulong key, ref JsonPropertyInfo info)
        {
            if (key == propertyRef.Key)
            {
                if (propertyName.Length <= PropertyNameKeyLength ||
                    // We compare the whole name, although we could skip the first 6 bytes (but it's likely not any faster)
                    propertyName.SequenceEqual(propertyRef.Info.Name))
                {
                    info = propertyRef.Info;
                    return true;
                }
            }

            return false;
        }

        private static bool IsPropertyRefEqual(ref PropertyRef propertyRef, PropertyRef other)
        {
            if (propertyRef.Key == other.Key)
            {
                if (propertyRef.Info.Name.Length <= PropertyNameKeyLength ||
                    propertyRef.Info.Name.AsSpan().SequenceEqual(other.Info.Name.AsSpan()))
                {
                    return true;
                }
            }

            return false;
        }

        public static ulong GetKey(ReadOnlySpan<byte> propertyName)
        {
            ulong key;
            int length = propertyName.Length;

            // Embed the propertyName in the first 6 bytes of the key.
            if (length > 3)
            {
                key = MemoryMarshal.Read<uint>(propertyName);
                if (length > 4)
                {
                    key |= (ulong)propertyName[4] << 32;
                }
                if (length > 5)
                {
                    key |= (ulong)propertyName[5] << 40;
                }
            }
            else if (length > 1)
            {
                key = MemoryMarshal.Read<ushort>(propertyName);
                if (length > 2)
                {
                    key |= (ulong)propertyName[2] << 16;
                }
            }
            else if (length == 1)
            {
                key = propertyName[0];
            }
            else
            {
                // An empty name is valid.
                key = 0;
            }

            // Embed the propertyName length in the last two bytes.
            key |= (ulong)propertyName.Length << 48;
            return key;
        }

        // Return the element type of the IEnumerable or return null if not an IEnumerable.
        public static Type GetElementType(Type propertyType, Type parentType, MemberInfo memberInfo, JsonSerializerOptions options)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                return null;
            }

            // Check for Array.
            Type elementType = propertyType.GetElementType();
            if (elementType != null)
            {
                return elementType;
            }

            // Check for Dictionary<TKey, TValue> or IEnumerable<T>
            if (propertyType.IsGenericType)
            {
                Type[] args = propertyType.GetGenericArguments();
                ClassType classType = GetClassType(propertyType, options);

                if ((classType == ClassType.Dictionary || classType == ClassType.IDictionaryConstructible) &&
                    args.Length >= 2 && // It is >= 2 in case there is a IDictionary<TKey, TValue, TSomeExtension>.
                    args[0].UnderlyingSystemType == typeof(string))
                {
                    return args[1];
                }

                if (classType == ClassType.Enumerable && args.Length >= 1) // It is >= 1 in case there is an IEnumerable<T, TSomeExtension>.
                {
                    return args[0];
                }
            }

            if (propertyType.IsAssignableFrom(typeof(IList)) ||
                propertyType.IsAssignableFrom(typeof(IDictionary)) ||
                IsDeserializedByConstructingWithIList(propertyType) ||
                IsDeserializedByConstructingWithIDictionary(propertyType))
            {
                return typeof(object);
            }

            throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(propertyType, parentType, memberInfo);
        }

        public static ClassType GetClassType(Type type, JsonSerializerOptions options)
        {
            Debug.Assert(type != null);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type == typeof(object))
            {
                return ClassType.Unknown;
            }

            if (options.HasConverter(type))
            {
                return ClassType.Value;
            }

            if (DefaultImmutableDictionaryConverter.IsImmutableDictionary(type) ||
                IsDeserializedByConstructingWithIDictionary(type))
            {
                return ClassType.IDictionaryConstructible;
            }

            if (typeof(IDictionary).IsAssignableFrom(type) ||
                (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))))
            {
                return ClassType.Dictionary;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return ClassType.Enumerable;
            }

            return ClassType.Object;
        }

        public const string ImmutableNamespaceName = "System.Collections.Immutable";

        private const string EnumerableGenericInterfaceTypeName = "System.Collections.Generic.IEnumerable`1";
        private const string EnumerableInterfaceTypeName = "System.Collections.IEnumerable";

        private const string ListGenericInterfaceTypeName = "System.Collections.Generic.IList`1";
        private const string ListInterfaceTypeName = "System.Collections.IList";

        private const string CollectionGenericInterfaceTypeName = "System.Collections.Generic.ICollection`1";
        private const string CollectionInterfaceTypeName = "System.Collections.ICollection";

        private const string ReadOnlyListGenericInterfaceTypeName = "System.Collections.Generic.IReadOnlyList`1";

        private const string ReadOnlyCollectionGenericInterfaceTypeName = "System.Collections.Generic.IReadOnlyCollection`1";

        public const string HashtableTypeName = "System.Collections.Hashtable";
        public const string SortedListTypeName = "System.Collections.SortedList";

        public const string StackTypeName = "System.Collections.Stack";
        public const string QueueTypeName = "System.Collections.Queue";
        public const string ArrayListTypeName = "System.Collections.ArrayList";

        public static bool IsDeserializedByAssigningFromList(Type type)
        {
            if (type.IsGenericType)
            {
                switch (type.GetGenericTypeDefinition().FullName)
                {
                    case EnumerableGenericInterfaceTypeName:
                    case ListGenericInterfaceTypeName:
                    case CollectionGenericInterfaceTypeName:
                    case ReadOnlyListGenericInterfaceTypeName:
                    case ReadOnlyCollectionGenericInterfaceTypeName:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                switch (type.FullName)
                {
                    case EnumerableInterfaceTypeName:
                    case ListInterfaceTypeName:
                    case CollectionInterfaceTypeName:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static bool IsSetInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISet<>);
        }

        public static bool HasConstructorThatTakesGenericIEnumerable(Type type, JsonSerializerOptions options)
        {
            Type elementType = GetElementType(type, parentType: null, memberInfo: null, options);
            return type.GetConstructor(new Type[] { typeof(List<>).MakeGenericType(elementType) }) != null;
        }

        public static bool IsDeserializedByConstructingWithIList(Type type)
        {
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
            switch (type.FullName)
            {
                case HashtableTypeName:
                case SortedListTypeName:
                    return true;
                default:
                    return false;
            }
        }
    }
}
