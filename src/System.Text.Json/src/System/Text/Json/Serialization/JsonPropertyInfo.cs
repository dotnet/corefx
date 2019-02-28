// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization
{
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    abstract class JsonPropertyInfo
    {
        public JsonPropertyNamePolicyAttribute NameConverter { get; private set; }
        private bool? _ignoreNullPropertyValueOnRead;
        private bool? _ignoreNullPropertyValueOnWrite;

        internal ClassType ClassType;

        internal byte[] _name = default;
        internal byte[] _escapedName = default;

        internal bool HasGetter { get; set; }
        internal bool HasSetter { get; set; }

        public ReadOnlySpan<byte> EscapedName
        {
            get
            {
                return _escapedName;
            }
        }

        public ReadOnlySpan<byte> Name
        {
            get
            {
                return _name;
            }
        }

        // todo: to minimize hashtable lookups, cache JsonClassInfo:
        //public JsonClassInfo ClassInfo;

        internal JsonPropertyInfo(Type parentClassType, Type propertyType, PropertyInfo propertyInfo, Type elementType, JsonSerializerOptions options)
        { 
            ParentClassType = parentClassType;
            PropertyType = propertyType;
            PropertyInfo = propertyInfo;
            ClassType = JsonClassInfo.GetClassType(propertyType);
            if (elementType != null)
            {
                ElementClassInfo = options.GetOrAddClass(elementType);
            }

            IsNullableType = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            CanBeNull = IsNullableType || !propertyType.IsValueType;
        }

        internal JsonEnumerableConverter EnumerableConverter { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        internal Type PropertyType { get; private set; }

        internal Type ParentClassType { get; private set; }

        internal JsonClassInfo ElementClassInfo { get; private set; }

        internal bool IsNullableType { get; private set; }

        internal bool CanBeNull { get; private set; }

        internal bool IgnoreNullPropertyValueOnRead(JsonSerializerOptions options)
        {
            if (_ignoreNullPropertyValueOnRead.HasValue)
            {
                return _ignoreNullPropertyValueOnRead.Value;
            }

            return options.IgnoreNullPropertyValueOnRead;
        }

        internal bool IgnoreNullPropertyValueOnWrite(JsonSerializerOptions options)
        {
            if (_ignoreNullPropertyValueOnWrite.HasValue)
            {
                return _ignoreNullPropertyValueOnWrite.Value;
            }

            return options.IgnoreNullPropertyValueOnWrite;
        }

        internal abstract object GetValueAsObject(object obj, JsonSerializerOptions options);
        internal abstract void SetValueAsObject(object obj, object value, JsonSerializerOptions options);

        internal abstract void Read(JsonTokenType tokenType, JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader);

        internal abstract void ReadEnumerable(JsonTokenType tokenType, JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader);

        internal abstract void Write(JsonSerializerOptions options, ref WriteStackFrame current, ref Utf8JsonWriter writer);

        internal abstract void WriteEnumerable(JsonSerializerOptions options, ref WriteStackFrame current, ref Utf8JsonWriter writer);

        internal virtual void GetPolicies(JsonSerializerOptions options)
        {
            JsonPropertyNamePolicyAttribute nameAttribute = DefaultConverters.GetPolicy<JsonPropertyNamePolicyAttribute>(ParentClassType, PropertyInfo, options);
            if (nameAttribute != null)
            {
                NameConverter = nameAttribute;
            }

            _ignoreNullPropertyValueOnRead = DefaultConverters.GetPropertyValueOption(ParentClassType, PropertyInfo, options, attr => attr.IgnoreNullValueOnRead);
            _ignoreNullPropertyValueOnWrite = DefaultConverters.GetPropertyValueOption(ParentClassType, PropertyInfo, options, attr => attr.IgnoreNullValueOnWrite);

            if (ElementClassInfo != null)
            {
                EnumerableConverter = DefaultConverters.GetEnumerableConverter(ParentClassType, PropertyInfo, PropertyType, options);
            }
        }
    }
}
