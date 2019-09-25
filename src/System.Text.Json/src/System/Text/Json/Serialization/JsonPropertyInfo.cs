// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    [DebuggerDisplay("PropertyInfo={PropertyInfo}, Element={CollectionElementClassInfo}")]
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

        public static readonly JsonPropertyInfo s_missingProperty = new JsonPropertyInfoNotNullable<object, object, object>();

        public static TAttribute GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return (TAttribute)propertyInfo?.GetCustomAttribute(typeof(TAttribute), inherit: false);
        }

        // Create a property that is ignored at run-time. It uses the same type (typeof(sbyte)) to help
        // prevent issues with unsupported types and helps ensure we don't accidently (de)serialize it.
        public static JsonPropertyInfo CreateIgnoredPropertyPlaceholder(PropertyInfo propertyInfo, JsonSerializerOptions options)
        {
            JsonPropertyInfo jsonPropertyInfo = new JsonPropertyInfoNotNullable<sbyte, sbyte, sbyte>
            {
                Options = options,
                PropertyInfo = propertyInfo
            };
            jsonPropertyInfo.DeterminePropertyName();

            Debug.Assert(!jsonPropertyInfo.ShouldDeserialize);
            Debug.Assert(!jsonPropertyInfo.ShouldSerialize);

            return jsonPropertyInfo;
        }

        private JsonClassInfo _collectionElementClassInfo;
        private JsonClassInfo _runtimeClassInfo;
        private JsonClassInfo _declaredTypeClassInfo;

        public bool CanBeNull { get; private set; }

        public ClassType ClassType { get; private set; }

        public abstract JsonConverter ConverterBase { get; set; }

        public Type ParentClassType { get; private set; }

        /// <summary>
        /// Return the JsonClassInfo for the declared type.
        /// </summary>
        /// <remarks>
        /// This should not be called during warm-up (initial creation of JsonClassInfos) to avoid recursive behavior
        /// which could result in a StackOverflowException.
        /// </remarks>
        public JsonClassInfo DeclaredClassInfo
        {
            get
            {
                if (_declaredTypeClassInfo == null)
                {
                    _declaredTypeClassInfo = Options.GetOrAddClass(DeclaredPropertyType);
                }

                return _declaredTypeClassInfo;
            }
        }

        public Type DeclaredPropertyType { get; private set; }

        public Type ImplementedCollectionPropertyType { get; private set; }

        /// <summary>
        /// Return the JsonClassInfo for the runtime type.
        /// </summary>
        /// <remarks>
        /// This should not be called during warm-up (initial creation of JsonClassInfos) to avoid recursive behavior
        /// which could result in a StackOverflowException.
        /// </remarks>
        public JsonClassInfo RuntimeClassInfo
        {
            get
            {
                if (_runtimeClassInfo == null)
                {
                    _runtimeClassInfo = Options.GetOrAddClass(RuntimePropertyType);
                }

                return _runtimeClassInfo;
            }
        }

        public Type RuntimePropertyType { get; internal set; }

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
                        ClassType == ClassType.Dictionary);

                    _collectionElementClassInfo = Options.GetOrAddClass(CollectionElementType);
                }

                return _collectionElementClassInfo;
            }
        }

        public Type CollectionElementType { get; private set; }

        public JsonEnumerableConverter EnumerableConverter { get; private set; }
        public JsonDictionaryConverter DictionaryConverter { get; private set; }

        // The escaped name passed to the writer.
        // Use a field here (not a property) to avoid value semantics.
        public JsonEncodedText? EscapedName;

        public bool HasGetter { get; protected set; }
        public bool HasSetter { get; protected set; }

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

        public PropertyInfo PropertyInfo { get; private set; }

        public bool ShouldSerialize { get; private set; }
        public bool ShouldDeserialize { get; private set; }

        public virtual void Initialize(
            ClassType propertyClassType,
            Type parentClassType,
            Type declaredPropertyType,
            Type runtimePropertyType,
            Type implementedCollectionPropertyType,
            Type collectionElementType,
            PropertyInfo propertyInfo,
            JsonConverter converter,
            JsonSerializerOptions options)
        {
            ClassType = propertyClassType;
            ParentClassType = parentClassType;
            DeclaredPropertyType = declaredPropertyType;
            ImplementedCollectionPropertyType = implementedCollectionPropertyType;
            CollectionElementType = collectionElementType;
            PropertyInfo = propertyInfo;
            Options = options;
            IsNullableType = declaredPropertyType.IsGenericType && declaredPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            CanBeNull = IsNullableType || !declaredPropertyType.IsValueType;

            if (converter != null)
            {
                ConverterBase = converter;
                RuntimePropertyType = runtimePropertyType ?? declaredPropertyType;

                // Avoid calling GetClassType since it will re-ask if there is a converter which is slow.
                if (RuntimePropertyType == typeof(object))
                {
                    ClassType = ClassType.Unknown;
                }
                else
                {
                    ClassType = ClassType.Value;
                }
            }
            else if (propertyClassType == ClassType.Enumerable || propertyClassType == ClassType.Dictionary)
            {
                DetermineEnumerableOrDictionaryConverter();
            }
        }

        public abstract object GetValueAsObject(object obj);
        public abstract void SetValueAsObject(object obj, object value);

        protected abstract void OnRead(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader);
        protected abstract void OnReadEnumerable(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader);
        protected abstract void OnWrite(ref WriteStackFrame current, Utf8JsonWriter writer);
        protected virtual void OnWriteDictionary(ref WriteStackFrame current, Utf8JsonWriter writer) { }
        protected abstract void OnWriteEnumerable(ref WriteStackFrame current, Utf8JsonWriter writer);

        // Copy any settings defined at run-time to the new property.
        public void CopyRuntimeSettingsTo(JsonPropertyInfo other)
        {
            other.EscapedName = EscapedName;
            other.Name = Name;
            other.NameAsString = NameAsString;
            other.PropertyNameKey = PropertyNameKey;
        }

        public virtual void GetPolicies()
        {
            DetermineSerializationCapabilities();
            DeterminePropertyName();
            IgnoreNullValues = Options.IgnoreNullValues;
        }

        public void Read(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader)
        {
            Debug.Assert(ShouldDeserialize);

            JsonPropertyInfo propertyInfo;
            if (CollectionElementClassInfo != null && (propertyInfo = CollectionElementClassInfo.PolicyProperty) != null)
            {
                // Forward the setter to the value-based JsonPropertyInfo.
                propertyInfo.ReadEnumerable(tokenType, ref state, ref reader);
            }
            else
            {
                JsonTokenType originalTokenType = reader.TokenType;
                int originalDepth = reader.CurrentDepth;
                long originalBytesConsumed = reader.BytesConsumed;

                OnRead(tokenType, ref state, ref reader);

                VerifyRead(originalTokenType, originalDepth, originalBytesConsumed, ref state, ref reader);
            }
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

        private void VerifyRead(JsonTokenType tokenType, int depth, long bytesConsumed, ref ReadStack state, ref Utf8JsonReader reader)
        {
            switch (tokenType)
            {
                case JsonTokenType.StartArray:
                    if (reader.TokenType != JsonTokenType.EndArray)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath(), ConverterBase.ToString());
                    }
                    else if (depth != reader.CurrentDepth)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath(), ConverterBase.ToString());
                    }

                    // Should not be possible to have not read anything.
                    Debug.Assert(bytesConsumed < reader.BytesConsumed);
                    break;

                case JsonTokenType.StartObject:
                    if (reader.TokenType != JsonTokenType.EndObject)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath(), ConverterBase.ToString());
                    }
                    else if (depth != reader.CurrentDepth)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath(), ConverterBase.ToString());
                    }

                    // Should not be possible to have not read anything.
                    Debug.Assert(bytesConsumed < reader.BytesConsumed);
                    break;

                default:
                    // Reading a single property value.
                    if (reader.BytesConsumed != bytesConsumed)
                    {
                        ThrowHelper.ThrowJsonException_SerializationConverterRead(reader, state.JsonPath(), ConverterBase.ToString());
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
                ThrowHelper.ThrowJsonException_SerializationConverterWrite(state.PropertyPath(), ConverterBase.ToString());
            }
        }

        public void WriteDictionary(ref WriteStack state, Utf8JsonWriter writer)
        {
            Debug.Assert(ShouldSerialize);
            int originalDepth = writer.CurrentDepth;

            OnWriteDictionary(ref state.Current, writer);

            if (originalDepth != writer.CurrentDepth)
            {
                ThrowHelper.ThrowJsonException_SerializationConverterWrite(state.PropertyPath(), ConverterBase.ToString());
            }
        }

        public void WriteEnumerable(ref WriteStack state, Utf8JsonWriter writer)
        {
            Debug.Assert(ShouldSerialize);
            int originalDepth = writer.CurrentDepth;

            OnWriteEnumerable(ref state.Current, writer);

            if (originalDepth != writer.CurrentDepth)
            {
                ThrowHelper.ThrowJsonException_SerializationConverterWrite(state.PropertyPath(), ConverterBase.ToString());
            }
        }

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

            // Cache the escaped property name.
            EscapedName = JsonEncodedText.Encode(Name, Options.Encoder);

            ulong key = JsonClassInfo.GetKey(Name);
            PropertyNameKey = key;
        }

        private void DetermineSerializationCapabilities()
        {
            if (ClassType != ClassType.Enumerable &&
                ClassType != ClassType.Dictionary)
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
                    }
                }
            }
        }

        private void DetermineEnumerableOrDictionaryConverter()
        {
            if (DeclaredPropertyType.IsArray)
            {
                // Verify that we don't have a multidimensional array.
                if (DeclaredPropertyType.GetArrayRank() > 1)
                {
                    ThrowHelper.ThrowNotSupportedException_SerializationNotSupportedCollection(DeclaredPropertyType, ParentClassType, PropertyInfo);
                    return;
                }

                EnumerableConverter = s_jsonArrayConverter;

                RuntimePropertyType = EnumerableConverter.ResolveRunTimeType(this);
            }
            else if (ClassType == ClassType.Dictionary)
            {
                if (s_jsonImmutableDictionaryConverter.OwnsImplementedCollectionType(ImplementedCollectionPropertyType, CollectionElementType))
                {
                    DictionaryConverter = s_jsonImmutableDictionaryConverter;

                    RuntimePropertyType = DictionaryConverter.ResolveRunTimeType(this);

                    DefaultImmutableDictionaryConverter.RegisterImmutableDictionary(RuntimePropertyType, CollectionElementType, Options);
                }
                else if (s_jsonIDictionaryConverter.OwnsImplementedCollectionType(ImplementedCollectionPropertyType, CollectionElementType))
                {
                    DictionaryConverter = s_jsonIDictionaryConverter;

                    RuntimePropertyType = DictionaryConverter.ResolveRunTimeType(this);
                }
                else if (s_jsonDerivedDictionaryConverter.OwnsImplementedCollectionType(ImplementedCollectionPropertyType, CollectionElementType))
                {
                    DictionaryConverter = s_jsonDerivedDictionaryConverter;

                    RuntimePropertyType = DictionaryConverter.ResolveRunTimeType(this);
                }
                else
                {
                    ThrowHelper.ThrowNotSupportedException_SerializationNotSupportedCollection(DeclaredPropertyType, ParentClassType, PropertyInfo);
                }
            }
            else if (ClassType == ClassType.Enumerable)
            {
                if (s_jsonImmutableEnumerableConverter.OwnsImplementedCollectionType(ImplementedCollectionPropertyType, CollectionElementType))
                {
                    EnumerableConverter = s_jsonImmutableEnumerableConverter;

                    RuntimePropertyType = EnumerableConverter.ResolveRunTimeType(this);

                    DefaultImmutableEnumerableConverter.RegisterImmutableCollection(RuntimePropertyType, CollectionElementType, Options);
                }
                else if (s_jsonICollectionConverter.OwnsImplementedCollectionType(ImplementedCollectionPropertyType, CollectionElementType))
                {
                    EnumerableConverter = s_jsonICollectionConverter;

                    RuntimePropertyType = EnumerableConverter.ResolveRunTimeType(this);
                }
                else if (s_jsonDerivedEnumerableConverter.OwnsImplementedCollectionType(ImplementedCollectionPropertyType, CollectionElementType))
                {
                    EnumerableConverter = s_jsonDerivedEnumerableConverter;

                    RuntimePropertyType = EnumerableConverter.ResolveRunTimeType(this);
                }
                else
                {
                    ThrowHelper.ThrowNotSupportedException_SerializationNotSupportedCollection(DeclaredPropertyType, ParentClassType, PropertyInfo);
                }
            }
            else
                throw new InvalidOperationException();
        }
    }
}
