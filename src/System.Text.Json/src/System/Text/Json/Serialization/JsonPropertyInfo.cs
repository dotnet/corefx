// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    [DebuggerDisplay("PropertyInfo={PropertyInfo}, Element={ElementClassInfo}")]
    internal abstract class JsonPropertyInfo
    {
        // Cache the converters so they don't get created for every enumerable property.
        private static readonly JsonEnumerableConverter s_jsonDerivedEnumerableConverter = new DefaultDerivedEnumerableConverter();
        private static readonly JsonEnumerableConverter s_jsonArrayConverter = new DefaultArrayConverter();
        private static readonly JsonEnumerableConverter s_jsonICollectionConverter = new DefaultICollectionConverter();
        private static readonly JsonEnumerableConverter s_jsonImmutableEnumerableConverter = new DefaultImmutableEnumerableConverter();
        private static readonly JsonDictionaryConverter s_jsonDerivedDictionaryConverter = new DefaultDerivedDictionaryConverter();
        private static readonly JsonDictionaryConverter s_jsonIDictionaryConverter = new DefaultIDictionaryConverter();
        private static readonly JsonDictionaryConverter s_jsonImmutableDictionaryConverter = new DefaultImmutableDictionaryConverter();

        public static readonly JsonPropertyInfo s_missingProperty = new JsonPropertyInfoNotNullable<object, object, object, object>();

        private JsonClassInfo _collectionElementClassInfo;

        public bool CanBeNull { get; private set; }

        public ClassType ClassType { get; private set; }

        public abstract JsonConverter ConverterBase { get; set; }

        // Copy any settings defined at run-time to the new property.
        public void CopyRuntimeSettingsTo(JsonPropertyInfo other)
        {
            other.EscapedName = EscapedName;
            other.Name = Name;
            other.NameAsString = NameAsString;
            other.PropertyNameKey = PropertyNameKey;
        }

        public abstract IList CreateConverterList();

        public abstract IEnumerable CreateDerivedEnumerableInstance(JsonPropertyInfo collectionPropertyInfo, IList sourceList, string jsonPath, JsonSerializerOptions options);

        public abstract object CreateDerivedDictionaryInstance(JsonPropertyInfo collectionPropertyInfo, IDictionary sourceDictionary, string jsonPath, JsonSerializerOptions options);

        public abstract IEnumerable CreateIEnumerableInstance(Type parentType, IList sourceList, string jsonPath, JsonSerializerOptions options);

        public abstract IDictionary CreateIDictionaryInstance(Type parentType, IDictionary sourceDictionary, string jsonPath, JsonSerializerOptions options);

        // Create a property that is ignored at run-time. It uses the same type (typeof(sbyte)) to help
        // prevent issues with unsupported types and helps ensure we don't accidently (de)serialize it.
        public static JsonPropertyInfo CreateIgnoredPropertyPlaceholder(PropertyInfo propertyInfo, JsonSerializerOptions options)
        {
            JsonPropertyInfo jsonPropertyInfo = new JsonPropertyInfoNotNullable<sbyte, sbyte, sbyte, sbyte>
            {
                Options = options,
                PropertyInfo = propertyInfo
            };
            jsonPropertyInfo.DeterminePropertyName();

            Debug.Assert(!jsonPropertyInfo.ShouldDeserialize);
            Debug.Assert(!jsonPropertyInfo.ShouldSerialize);

            return jsonPropertyInfo;
        }

        public Type PropertyType { get; private set; }

        public Type ImplementedCollectionPropertyType { get; private set; }

        private void DeterminePropertyName()
        {
            if (PropertyInfo == null)
            {
                return;
            }

            JsonPropertyNameAttribute nameAttribute = GetAttribute<JsonPropertyNameAttribute>(PropertyInfo);
            if (nameAttribute != null)
            {
                string name = nameAttribute.Name;
                if (name == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameNull(ParentClassType, this);
                }

                NameAsString = name;
            }
            else if (Options.PropertyNamingPolicy != null)
            {
                string name = Options.PropertyNamingPolicy.ConvertName(PropertyInfo.Name);
                if (name == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameNull(ParentClassType, this);
                }

                NameAsString = name;
            }
            else
            {
                NameAsString = PropertyInfo.Name;
            }

            Debug.Assert(NameAsString != null);

            // At this point propertyName is valid UTF16, so just call the simple UTF16->UTF8 encoder.
            Name = Encoding.UTF8.GetBytes(NameAsString);

            // Cache the escaped name.
            EscapedName = JsonEncodedText.Encode(Name);

            ulong key = JsonClassInfo.GetKey(Name);
            PropertyNameKey = key;
        }

        private void DetermineSerializationCapabilities()
        {
            if (ClassType != ClassType.Enumerable &&
                ClassType != ClassType.ICollectionConstructible &&
                ClassType != ClassType.Dictionary &&
                ClassType != ClassType.IDictionaryConstructible)
            {
                // We serialize if there is a getter + not ignoring readonly properties.
                ShouldSerialize = HasGetter && (HasSetter || !Options.IgnoreReadOnlyProperties);

                // We deserialize if there is a setter.
                ShouldDeserialize = HasSetter;
            }
            else
            {
                if (HasGetter)
                {
                    ShouldSerialize = true;

                    if (HasSetter)
                    {
                        ShouldDeserialize = true;

                        if (PropertyType.IsArray)
                        {
                            // Verify that we don't have a multidimensional array.
                            if (PropertyType.GetArrayRank() > 1)
                            {
                                throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(PropertyType, ParentClassType, PropertyInfo);
                            }

                            EnumerableConverter = s_jsonArrayConverter;
                        }
                        else if (ClassType == ClassType.IDictionaryConstructible)
                        {
                            if (ImplementedCollectionPropertyType.FullName.StartsWith(JsonClassInfo.ImmutableNamespaceName))
                            {
                                //DefaultImmutableDictionaryConverter.RegisterImmutableDictionary(ImplementedCollectionPropertyType, ElementType, Options);

                                DictionaryConverter = s_jsonImmutableDictionaryConverter;
                            }
                            else
                            {
                                DictionaryConverter = s_jsonIDictionaryConverter;
                            }
                        }
                        else if (ClassType == ClassType.Dictionary)
                        {
                            DictionaryConverter = s_jsonDerivedDictionaryConverter;
                        }
                        else if (ClassType == ClassType.ICollectionConstructible)
                        {
                            if (ImplementedCollectionPropertyType.FullName.StartsWith(JsonClassInfo.ImmutableNamespaceName))
                            {
                                //DefaultImmutableEnumerableConverter.RegisterImmutableCollection(PropertyType, ElementType, Options);

                                EnumerableConverter = s_jsonImmutableEnumerableConverter;
                            }
                            else
                            {
                                EnumerableConverter = s_jsonICollectionConverter;
                            }
                        }
                        else if (ClassType == ClassType.Enumerable)
                        {
                            EnumerableConverter = s_jsonDerivedEnumerableConverter;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return the JsonClassInfo for the element type, or null if the property is not an enumerable or dictionary.
        /// </summary>
        /// <remarks>
        /// This should not be called during warm-up (initial creation of JsonClassInfos) to avoid recursive behavior
        /// which could result in a StackOverflowException.
        /// </remarks>
        public JsonClassInfo CollectionElementClassInfo
        {
            get
            {
                if (_collectionElementClassInfo == null && CollectionElementType != null)
                {
                    Debug.Assert(ClassType == ClassType.Enumerable ||
                        ClassType == ClassType.ICollectionConstructible ||
                        ClassType == ClassType.Dictionary ||
                        ClassType == ClassType.IDictionaryConstructible);

                    _collectionElementClassInfo = Options.GetOrAddClass(CollectionElementType);
                }

                return _collectionElementClassInfo;
            }
        }

        public Type CollectionElementType { get; set; }

        public JsonEnumerableConverter EnumerableConverter { get; private set; }
        public JsonDictionaryConverter DictionaryConverter { get; private set; }

        // The escaped name passed to the writer.
        public JsonEncodedText? EscapedName { get; private set; }

        public static TAttribute GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return (TAttribute)propertyInfo?.GetCustomAttribute(typeof(TAttribute), inherit: false);
        }

        public virtual void GetPolicies()
        {
            DetermineSerializationCapabilities();
            DeterminePropertyName();
            IgnoreNullValues = Options.IgnoreNullValues;
        }

        public abstract object GetValueAsObject(object obj);

        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }

        public virtual void Initialize(
            ClassType propertyClassType,
            Type parentClassType,
            Type propertyType,
            Type implementedCollectionPropertyType,
             Type collectionElementType,
            PropertyInfo propertyInfo,
            JsonConverter converter,
            JsonSerializerOptions options)
        {
            ClassType = propertyClassType;
            ParentClassType = parentClassType;
            PropertyType = propertyType;
            ImplementedCollectionPropertyType = implementedCollectionPropertyType;
            CollectionElementType = collectionElementType;
            PropertyInfo = propertyInfo;
            Options = options;
            IsNullableType = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            CanBeNull = IsNullableType || !propertyType.IsValueType;

            if (converter != null)
            {
                ConverterBase = converter;
            }
        }

        public bool IgnoreNullValues { get; private set; }

        public bool IsNullableType { get; private set; }

        public bool IsPropertyPolicy { get; protected set; }

        // The name from a Json value. This is cached for performance on first deserialize.
        public byte[] JsonPropertyName { get; set; }

        // The name of the property with any casing policy or the name specified from JsonPropertyNameAttribute.
        public byte[] Name { get; private set; }
        public string NameAsString { get; private set; }

        // Key for fast property name lookup.
        public ulong PropertyNameKey { get; set; }

        // Options can be referenced here since all JsonPropertyInfos originate from a JsonClassInfo that is cached on JsonSerializerOptions.
        protected JsonSerializerOptions Options { get; set; }

        protected abstract void OnRead(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader);
        protected abstract void OnReadEnumerable(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader);
        protected abstract void OnWrite(ref WriteStackFrame current, Utf8JsonWriter writer);
        protected virtual void OnWriteDictionary(ref WriteStackFrame current, Utf8JsonWriter writer) { }
        protected abstract void OnWriteEnumerable(ref WriteStackFrame current, Utf8JsonWriter writer);

        public Type ParentClassType { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public void Read(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader)
        {
            Debug.Assert(ShouldDeserialize);

            JsonTokenType originalTokenType = reader.TokenType;
            int originalDepth = reader.CurrentDepth;
            long originalBytesConsumed = reader.BytesConsumed;

            OnRead(tokenType, ref state, ref reader);

            VerifyRead(originalTokenType, originalDepth, originalBytesConsumed, ref state, ref reader);
        }

        public void ReadEnumerable(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader)
        {
            Debug.Assert(ShouldDeserialize);

            JsonTokenType originalTokenType = reader.TokenType;
            int originalDepth = reader.CurrentDepth;
            long originalBytesConsumed = reader.BytesConsumed;

            OnReadEnumerable(tokenType, ref state, ref reader);

            VerifyRead(originalTokenType, originalDepth, originalBytesConsumed, ref state, ref reader);
        }

        public abstract void SetValueAsObject(object obj, object value);

        public bool ShouldSerialize { get; private set; }
        public bool ShouldDeserialize { get; private set; }

        private void VerifyRead(JsonTokenType tokenType, int depth, long bytesConsumed, ref ReadStack state, ref Utf8JsonReader reader)
        {
            switch (tokenType)
            {
                case JsonTokenType.StartArray:
                    if (reader.TokenType != JsonTokenType.EndArray)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath, ConverterBase.ToString());
                    }
                    else if (depth != reader.CurrentDepth)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath, ConverterBase.ToString());
                    }

                    // Should not be possible to have not read anything.
                    Debug.Assert(bytesConsumed < reader.BytesConsumed);
                    break;

                case JsonTokenType.StartObject:
                    if (reader.TokenType != JsonTokenType.EndObject)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath, ConverterBase.ToString());
                    }
                    else if (depth != reader.CurrentDepth)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath, ConverterBase.ToString());
                    }

                    // Should not be possible to have not read anything.
                    Debug.Assert(bytesConsumed < reader.BytesConsumed);
                    break;

                default:
                    // Reading a single property value.
                    if (reader.BytesConsumed != bytesConsumed)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath, ConverterBase.ToString());
                    }

                    // Should not be possible to change token type.
                    Debug.Assert(reader.TokenType == tokenType);

                    break;
            }
        }

        public void Write(ref WriteStack state, Utf8JsonWriter writer)
        {
            Debug.Assert(ShouldSerialize);

            int originalDepth = writer.CurrentDepth;

            OnWrite(ref state.Current, writer);

            if (originalDepth != writer.CurrentDepth)
            {
                ThrowHelper.ThrowJsonException_SerializationConverterWrite(state.PropertyPath, ConverterBase.ToString());
            }
        }

        public void WriteDictionary(ref WriteStack state, Utf8JsonWriter writer)
        {
            Debug.Assert(ShouldSerialize);
            int originalDepth = writer.CurrentDepth;

            OnWriteDictionary(ref state.Current, writer);

            if (originalDepth != writer.CurrentDepth)
            {
                ThrowHelper.ThrowJsonException_SerializationConverterWrite(state.PropertyPath, ConverterBase.ToString());
            }
        }

        public void WriteEnumerable(ref WriteStack state, Utf8JsonWriter writer)
        {
            Debug.Assert(ShouldSerialize);
            int originalDepth = writer.CurrentDepth;

            OnWriteEnumerable(ref state.Current, writer);

            if (originalDepth != writer.CurrentDepth)
            {
                ThrowHelper.ThrowJsonException_SerializationConverterWrite(state.PropertyPath, ConverterBase.ToString());
            }
        }
    }
}
