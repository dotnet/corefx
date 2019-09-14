// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    [DebuggerDisplay("ClassType.{ClassType}, {Type.Name}")]
    internal sealed partial class JsonClassInfo
    {
        // The length of the property name embedded in the key (in bytes).
        // The key is a ulong (8 bytes) containing the first 7 bytes of the property name
        // followed by a byte representing the length.
        private const int PropertyNameKeyLength = 7;

        // The limit to how many property names from the JSON are cached in _propertyRefsSorted before using PropertyCache.
        private const int PropertyNameCountCacheThreshold = 64;

        // All of the serializable properties on a POCO keyed on property name.
        public volatile Dictionary<string, JsonPropertyInfo> PropertyCache;

        // Fast cache of properties by first JSON ordering; may not contain all properties. Accessed before PropertyCache.
        // Use an array (instead of List<T>) for highest performance.
        private volatile PropertyRef[] _propertyRefsSorted;

        public delegate object ConstructorDelegate();
        private readonly ConstructorDelegate _createObject;
        public ConstructorDelegate CreateConcreteEnumerable { get; private set; }
        public ConstructorDelegate CreateConcreteDictionary { get; private set; }

        public ClassType ClassType { get; private set; }

        public JsonPropertyInfo DataExtensionProperty { get; private set; }

        // If enumerable, the JsonClassInfo for the element type.
        private JsonClassInfo _elementClassInfo;

        /// <summary>
        /// Return the JsonClassInfo for the element type, or null if the type is not an enumerable or dictionary.
        /// </summary>
        /// <remarks>
        /// This should not be called during warm-up (initial creation of JsonClassInfos) to avoid recursive behavior
        /// which could result in a StackOverflowException.
        /// </remarks>
        public JsonClassInfo ElementClassInfo
        {
            get
            {
                if (_elementClassInfo == null && ElementType != null)
                {
                    Debug.Assert(ClassType == ClassType.Enumerable ||
                        ClassType == ClassType.ICollectionConstructible ||
                        ClassType == ClassType.Dictionary ||
                        ClassType == ClassType.IDictionaryConstructible);

                    _elementClassInfo = Options.GetOrAddClass(ElementType);
                }

                return _elementClassInfo;
            }
        }

        public Type ElementType { get; set; }

        public JsonSerializerOptions Options { get; private set; }

        public Type Type { get; private set; }

        public void UpdateSortedPropertyCache(ref ReadStackFrame frame)
        {
            Debug.Assert(frame.PropertyRefCache != null);

            // frame.PropertyRefCache is only read\written by a single thread -- the thread performing
            // the deserialization for a given object instance.

            List<PropertyRef> listToAppend = frame.PropertyRefCache;

            // _propertyRefsSorted can be accessed by multiple threads, so replace the reference when
            // appending to it. No lock() is necessary.

            if (_propertyRefsSorted != null)
            {
                List<PropertyRef> replacementList = new List<PropertyRef>(_propertyRefsSorted);
                Debug.Assert(replacementList.Count <= PropertyNameCountCacheThreshold);

                // Verify replacementList will not become too large.
                while (replacementList.Count + listToAppend.Count > PropertyNameCountCacheThreshold)
                {
                    // This code path is rare; keep it simple by using RemoveAt() instead of RemoveRange() which requires calculating index\count.
                    listToAppend.RemoveAt(listToAppend.Count - 1);
                }

                // Add the new items; duplicates are possible but that is tolerated during property lookup.
                replacementList.AddRange(listToAppend);
                _propertyRefsSorted = replacementList.ToArray();
            }
            else
            {
                _propertyRefsSorted = listToAppend.ToArray();
            }

            frame.PropertyRefCache = null;
        }

        public JsonClassInfo(Type type, JsonSerializerOptions options)
        {
            Type = type;
            Options = options;
            ClassType = GetClassType(type, options);

            _createObject = options.MemberAccessorStrategy.CreateConstructor(type);

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

                        if (DetermineExtensionDataProperty(cache))
                        {
                            // Remove from cache since it is handled independently.
                            cache.Remove(DataExtensionProperty.NameAsString);
                        }

                        // Set as a unit to avoid concurrency issues.
                        PropertyCache = cache;
                    }
                    break;
                case ClassType.Enumerable:
                case ClassType.Dictionary:
                    {
                        // Add a single property that maps to the class type so we can have policies applied.
                        AddPolicyProperty(type, options);

                        if (PolicyProperty.DeclaredPropertyType.IsInterface)
                            _createObject = options.MemberAccessorStrategy.CreateConstructor(PolicyProperty.RuntimePropertyType);

                        ElementType = GetElementType(type, parentType: null, memberInfo: null, options: options);
                    }
                    break;
                case ClassType.ICollectionConstructible:
                    {
                        // Add a single property that maps to the class type so we can have policies applied.
                        AddPolicyProperty(type, options);

                        ElementType = GetElementType(type, parentType: null, memberInfo: null, options: options);

                        CreateConcreteEnumerable = options.MemberAccessorStrategy.CreateConstructor(
                           typeof(List<>).MakeGenericType(ElementType));
                    }
                    break;
                case ClassType.IDictionaryConstructible:
                    {
                        // Add a single property that maps to the class type so we can have policies applied.
                        AddPolicyProperty(type, options);

                        ElementType = GetElementType(type, parentType: null, memberInfo: null, options: options);

                        CreateConcreteDictionary = options.MemberAccessorStrategy.CreateConstructor(
                           typeof(Dictionary<,>).MakeGenericType(typeof(string), ElementType));
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

        private bool DetermineExtensionDataProperty(Dictionary<string, JsonPropertyInfo> cache)
        {
            JsonPropertyInfo jsonPropertyInfo = GetPropertyWithUniqueAttribute(typeof(JsonExtensionDataAttribute), cache);
            if (jsonPropertyInfo != null)
            {
                Type declaredPropertyType = jsonPropertyInfo.DeclaredPropertyType;
                if (!typeof(IDictionary<string, JsonElement>).IsAssignableFrom(declaredPropertyType) &&
                    !typeof(IDictionary<string, object>).IsAssignableFrom(declaredPropertyType))
                {
                    ThrowHelper.ThrowInvalidOperationException_SerializationDataExtensionPropertyInvalid(this, jsonPropertyInfo);
                }

                DataExtensionProperty = jsonPropertyInfo;
                return true;
            }

            return false;
        }

        private JsonPropertyInfo GetPropertyWithUniqueAttribute(Type attributeType, Dictionary<string, JsonPropertyInfo> cache)
        {
            JsonPropertyInfo property = null;

            foreach (JsonPropertyInfo jsonPropertyInfo in cache.Values)
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

        // AggressiveInlining used although a large method it is only called from one location and is on a hot path.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonPropertyInfo GetProperty(ReadOnlySpan<byte> propertyName, ref ReadStackFrame frame)
        {
            JsonPropertyInfo info = null;

            // Keep a local copy of the cache in case it changes by another thread.
            PropertyRef[] localPropertyRefsSorted = _propertyRefsSorted;

            ulong key = GetKey(propertyName);

            // If there is an existing cache, then use it.
            if (localPropertyRefsSorted != null)
            {
                // Start with the current property index, and then go forwards\backwards.
                int propertyIndex = frame.PropertyIndex;

                int count = localPropertyRefsSorted.Length;
                int iForward = Math.Min(propertyIndex, count);
                int iBackward = iForward - 1;

                while (true)
                {
                    if (iForward < count)
                    {
                        PropertyRef propertyRef = localPropertyRefsSorted[iForward];
                        if (TryIsPropertyRefEqual(propertyRef, propertyName, key, ref info))
                        {
                            return info;
                        }

                        ++iForward;

                        if (iBackward >= 0)
                        {
                            propertyRef = localPropertyRefsSorted[iBackward];
                            if (TryIsPropertyRefEqual(propertyRef, propertyName, key, ref info))
                            {
                                return info;
                            }

                            --iBackward;
                        }
                    }
                    else if (iBackward >= 0)
                    {
                        PropertyRef propertyRef = localPropertyRefsSorted[iBackward];
                        if (TryIsPropertyRefEqual(propertyRef, propertyName, key, ref info))
                        {
                            return info;
                        }

                        --iBackward;
                    }
                    else
                    {
                        // Property was not found.
                        break;
                    }
                }
            }

            // No cached item was found. Try the main list which has all of the properties.

            string stringPropertyName = JsonHelpers.Utf8GetString(propertyName);
            if (!PropertyCache.TryGetValue(stringPropertyName, out info))
            {
                info = JsonPropertyInfo.s_missingProperty;
            }

            Debug.Assert(info != null);

            // Three code paths to get here:
            // 1) info == s_missingProperty. Property not found.
            // 2) key == info.PropertyNameKey. Exact match found.
            // 3) key != info.PropertyNameKey. Match found due to case insensitivity.
            Debug.Assert(info == JsonPropertyInfo.s_missingProperty || key == info.PropertyNameKey || Options.PropertyNameCaseInsensitive);

            // Check if we should add this to the cache.
            // Only cache up to a threshold length and then just use the dictionary when an item is not found in the cache.
            int cacheCount = 0;
            if (localPropertyRefsSorted != null)
            {
                cacheCount = localPropertyRefsSorted.Length;
            }

            // Do a quick check for the stable (after warm-up) case.
            if (cacheCount < PropertyNameCountCacheThreshold)
            {
                // Do a slower check for the warm-up case.
                if (frame.PropertyRefCache != null)
                {
                    cacheCount += frame.PropertyRefCache.Count;
                }

                // Check again to append the cache up to the threshold.
                if (cacheCount < PropertyNameCountCacheThreshold)
                {
                    if (frame.PropertyRefCache == null)
                    {
                        frame.PropertyRefCache = new List<PropertyRef>();
                    }

                    PropertyRef propertyRef = new PropertyRef(key, info);
                    frame.PropertyRefCache.Add(propertyRef);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryIsPropertyRefEqual(in PropertyRef propertyRef, ReadOnlySpan<byte> propertyName, ulong key, ref JsonPropertyInfo info)
        {
            if (key == propertyRef.Key)
            {
                // We compare the whole name, although we could skip the first 7 bytes (but it's not any faster)
                if (propertyName.Length <= PropertyNameKeyLength ||
                    propertyName.SequenceEqual(propertyRef.Info.Name))
                {
                    info = propertyRef.Info;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get a key from the property name.
        /// The key consists of the first 7 bytes of the property name and then the length.
        /// </summary>
        public static ulong GetKey(ReadOnlySpan<byte> propertyName)
        {
            const int BitsInByte = 8;
            ulong key;
            int length = propertyName.Length;

            if (length > 7)
            {
                key = MemoryMarshal.Read<ulong>(propertyName);

                // Max out the length byte.
                // The comparison logic tests for equality against the full contents instead of just
                // the key if the property name length is >7.
                key |= 0xFF00000000000000;
            }
            else if (length > 3)
            {
                key = MemoryMarshal.Read<uint>(propertyName);

                if (length == 7)
                {
                    key |= (ulong)propertyName[6] << (6 * BitsInByte)
                        | (ulong)propertyName[5] << (5 * BitsInByte)
                        | (ulong)propertyName[4] << (4 * BitsInByte)
                        | (ulong)7 << (7 * BitsInByte);
                }
                else if (length == 6)
                {
                    key |= (ulong)propertyName[5] << (5 * BitsInByte)
                        | (ulong)propertyName[4] << (4 * BitsInByte)
                        | (ulong)6 << (7 * BitsInByte);
                }
                else if (length == 5)
                {
                    key |= (ulong)propertyName[4] << (4 * BitsInByte)
                        | (ulong)5 << (7 * BitsInByte);
                }
                else
                {
                    key |= (ulong)4 << (7 * BitsInByte);
                }
            }
            else if (length > 1)
            {
                key = MemoryMarshal.Read<ushort>(propertyName);

                if (length == 3)
                {
                    key |= (ulong)propertyName[2] << (2 * BitsInByte)
                        | (ulong)3 << (7 * BitsInByte);
                }
                else
                {
                    key |= (ulong)2 << (7 * BitsInByte);
                }
            }
            else if (length == 1)
            {
                key = propertyName[0]
                    | (ulong)1 << (7 * BitsInByte);
            }
            else
            {
                // An empty name is valid.
                key = 0;
            }

            // Verify key contains the embedded bytes as expected.
            Debug.Assert(
                (length < 1 || propertyName[0] == ((key & ((ulong)0xFF << 8 * 0)) >> 8 * 0)) &&
                (length < 2 || propertyName[1] == ((key & ((ulong)0xFF << 8 * 1)) >> 8 * 1)) &&
                (length < 3 || propertyName[2] == ((key & ((ulong)0xFF << 8 * 2)) >> 8 * 2)) &&
                (length < 4 || propertyName[3] == ((key & ((ulong)0xFF << 8 * 3)) >> 8 * 3)) &&
                (length < 5 || propertyName[4] == ((key & ((ulong)0xFF << 8 * 4)) >> 8 * 4)) &&
                (length < 6 || propertyName[5] == ((key & ((ulong)0xFF << 8 * 5)) >> 8 * 5)) &&
                (length < 7 || propertyName[6] == ((key & ((ulong)0xFF << 8 * 6)) >> 8 * 6)));

            return key;
        }

        // Return the element type of the IEnumerable or return null if not an IEnumerable.
        public static Type GetElementType(Type propertyType, Type parentType, MemberInfo memberInfo, JsonSerializerOptions options)
        {
            // We want to handle as the implemented collection type, if applicable.
            Type implementedType = GetImplementedCollectionType(parentType, propertyType, propertyInfo: null, out _, options);

            if (!typeof(IEnumerable).IsAssignableFrom(implementedType))
            {
                return null;
            }

            // Check for Array.
            Type elementType = implementedType.GetElementType();
            if (elementType != null)
            {
                return elementType;
            }

            // Check for Dictionary<TKey, TValue> or IEnumerable<T>
            if (implementedType.IsGenericType)
            {
                Type[] args = implementedType.GetGenericArguments();
                ClassType classType = GetClassType(implementedType, options);

                if ((classType == ClassType.Dictionary || classType == ClassType.IDictionaryConstructible) &&
                    args.Length >= 2) // It is >= 2 in case there is a IDictionary<TKey, TValue, TSomeExtension>.
                {
                    if (args[0].UnderlyingSystemType == typeof(string))
                        return args[1];

                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(propertyType, parentType, memberInfo);
                }

                if ((classType == ClassType.Enumerable || classType == ClassType.ICollectionConstructible) &&
                    args.Length >= 1) // It is >= 1 in case there is an IEnumerable<T, TSomeExtension>.
                {
                    return args[0];
                }
            }

            if (implementedType.IsAssignableFrom(typeof(IList)) ||
                implementedType.IsAssignableFrom(typeof(IDictionary)) ||
                IsDeserializedByConstructingWithIList(implementedType) ||
                IsDeserializedByConstructingWithIDictionary(implementedType))
            {
                return typeof(object);
            }

            // Drive HashTable, SortedList...
            if (typeof(IList).IsAssignableFrom(implementedType) ||
                typeof(IDictionary).IsAssignableFrom(implementedType))
            {
                return typeof(object);
            }

            throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(propertyType, parentType, memberInfo);
        }

        public static ClassType GetClassType(Type type, JsonSerializerOptions options)
        {
            Debug.Assert(type != null);

            // We want to handle as the implemented collection type, if applicable.
            Type implementedType = GetImplementedCollectionType(typeof(object), type, propertyInfo: null, out _, options);

            if (implementedType.IsGenericType && implementedType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                implementedType = Nullable.GetUnderlyingType(implementedType);
            }

            if (implementedType == typeof(object))
            {
                return ClassType.Unknown;
            }

            if (options.HasConverter(implementedType))
            {
                return ClassType.Value;
            }

            if (DefaultImmutableDictionaryConverter.IsImmutableDictionary(implementedType) ||
                IsDeserializedByConstructingWithIDictionary(implementedType))
            {
                return ClassType.IDictionaryConstructible;
            }

            if (typeof(IDictionary).IsAssignableFrom(implementedType))
            {
                return ClassType.Dictionary;
            }

            if (IsGenericDictionary(implementedType))
            {
                return type.IsInterface
                    ? ClassType.Dictionary // IDictionary<,> we can use a concrete type for that.
                    : ClassType.IDictionaryConstructible; // A type implementing IDictionary<,> but not IDictionary, have to buffer that.
            }

            if (implementedType.IsArray ||
                DefaultImmutableEnumerableConverter.IsImmutableEnumerable(implementedType) ||
                IsDeserializedByConstructingWithIList(implementedType))
            {
                return ClassType.ICollectionConstructible;
            }

            if (typeof(IList).IsAssignableFrom(implementedType))
            {
                return ClassType.Enumerable;
            }

            if (typeof(IEnumerable).IsAssignableFrom(implementedType))
            {
                return type.IsInterface
                    ? ClassType.Enumerable // IEnumerable we can use a concrete type for that.
                    : ClassType.ICollectionConstructible; // A type implementing IEnumerable but not IList, have to buffer that.
            }

            return ClassType.Object;
        }

        public static bool IsGenericDictionary(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));
        }

        public object CreateObject()
        {
            if (_createObject == null)
            {
                if (Type.IsInterface)
                {
                    ThrowHelper.ThrowInvalidOperationException_DeserializePolymorphicInterface(Type);
                }
                else
                {
                    ThrowHelper.ThrowInvalidOperationException_DeserializeMissingParameterlessConstructor(Type);
                }
            }

            return _createObject();
        }
    }
}
