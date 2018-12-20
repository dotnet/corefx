// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Represents a strongly-typed property to prevent boxing.
    /// </summary>
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    class JsonPropertyInfo<TValue> : JsonPropertyInfo
    {
        internal delegate TValue GetterDelegate(object obj);
        internal delegate void SetterDelegate(object obj, TValue value);
        public JsonValueConverter<TValue> ValueConverter { get; private set; }

        internal bool HasGetter { get; private set; }
        internal bool HasSetter { get; private set; }

        internal GetterDelegate Get { get; private set; }
        internal SetterDelegate Set { get; private set; }

        internal JsonPropertyInfo(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options, Type elementType = null) :
            base(classType, propertyType, propertyInfo, elementType, options)
        {
            if (propertyInfo != null)
            {
                if (propertyInfo.GetMethod?.IsPublic == true)
                {
                    HasGetter = true;
                    Get = options.ClassMaterializerStrategy.CreateGetter<TValue>(propertyInfo);
                }

                if (propertyInfo.SetMethod?.IsPublic == true)
                {
                    HasSetter = true;
                    Set = options.ClassMaterializerStrategy.CreateSetter<TValue>(propertyInfo);
                }
            }
            else
            {
                HasGetter = true;
                HasSetter = true;

                // Used when current.obj contains the return value and this is the property policy.
                Get = delegate (object obj)
                {
                    return (TValue)obj;
                };
            }

            GetPolicies(options);
        }

        internal override object GetValueAsObject(object obj, JsonSerializerOptions options)
        {
            Debug.Assert(Get != null);
            return Get(obj);
        }

        internal override void SetValueAsObject(object obj, object value, JsonSerializerOptions options)
        {
            Debug.Assert(Set != null);
            TValue typedValue = (TValue)value;

            if (typedValue != null || !IgnoreNullPropertyValueOnWrite(options))
            {
                Set(obj, (TValue)value);
            }
        }

        internal override void Read(JsonTokenType tokenType, JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (ElementClassInfo != null)
            {
                // Forward the setter to the value-based JsonPropertyInfo.
                JsonPropertyInfo propertyInfo = ElementClassInfo.GetPolicyProperty();
                propertyInfo.ReadEnumerable(tokenType, options, ref state, ref reader);
            }
            else if (HasSetter)
            {
                if (ValueConverter != null)
                {
                    Type propertyType = PropertyType;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = Nullable.GetUnderlyingType(propertyType);
                    }

                    if (ValueConverter.TryRead(propertyType, ref reader, out TValue value))
                    {
                        if (value != null || !IgnoreNullPropertyValueOnRead(options))
                        {
                            SetValueAsObject(state.Current.ReturnValue, value, options);
                        }

                        return;
                    }
                }
                else if (this is IJsonValueConverter<TValue> converter)
                {
                    if (converter.TryRead(PropertyType, ref reader, out TValue value))
                    {
                        if (state.Current.ReturnValue == null)
                        {
                            state.Current.ReturnValue = value;
                        }
                        else
                        {
                            if (value != null || !IgnoreNullPropertyValueOnRead(options))
                            {
                                Set(state.Current.ReturnValue, value);
                            }
                        }

                        return;
                    }
                }

                throw new JsonReaderException(SR.Format(SR.DeserializeUnableToConvertValue, state.PropertyPath, PropertyType), reader.CurrentState);
            }
        }

        internal override void ReadEnumerable(JsonTokenType tokenType, JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (ValueConverter != null)
            {
                Type propertyType = PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }

                if (ValueConverter.TryRead(propertyType, ref reader, out TValue value))
                {
                    ReadStackFrame.SetReturnValue(value, options, ref state.Current);
                    return;
                }
            }
            else if (this is IJsonValueConverter<TValue> converter)
            {
                if (converter.TryRead(PropertyType, ref reader, out TValue value))
                {
                    ReadStackFrame.SetReturnValue(value, options, ref state.Current);
                    return;
                }
            }

            throw new JsonReaderException(SR.Format(SR.DeserializeUnableToConvertValue, state.PropertyPath, PropertyType), reader.CurrentState);
        }

        // todo: have the caller check if current.Enumerator != null and call WriteEnumerable of the underlying property directly to avoid an extra virtual call.
        internal override void Write(JsonSerializerOptions options, ref WriteStackFrame current, ref Utf8JsonWriter writer)
        {
            if (current.Enumerator != null)
            {
                // Forward the setter to the value-based JsonPropertyInfo.
                JsonPropertyInfo propertyInfo = ElementClassInfo.GetPolicyProperty();
                propertyInfo.WriteEnumerable(options, ref current, ref writer);
            }
            else if (HasGetter)
            {
                if (ValueConverter != null)
                {
                    TValue value = Get(current.CurrentValue);
                    if (value == null)
                    {
                        if (_escapedName == null)
                        {
                            writer.WriteNullValue();
                        }
                        else if (!IgnoreNullPropertyValueOnWrite(options))
                        {
                            writer.WriteNull(_escapedName);
                        }
                    }
                    else
                    {
                        if (_escapedName != null)
                        {
                            ValueConverter.Write(_escapedName, value, ref writer);
                        }
                        else
                        {
                            ValueConverter.Write(value, ref writer);
                        }
                    }
                }
                else
                {
                    TValue value = Get(current.CurrentValue);

                    if (value == null)
                    {
                        if (_escapedName == null)
                        {
                            writer.WriteNullValue();
                        }
                        else if (!IgnoreNullPropertyValueOnWrite(options))
                        {
                            writer.WriteNull(_escapedName);
                        }
                    }
                    else
                    {
                        if (this is IJsonValueConverter<TValue> converter)
                        {
                            if (_escapedName != null)
                            {
                                converter.Write(_escapedName, value, ref writer);
                            }
                            else
                            {
                                converter.Write(value, ref writer);
                            }
                        }
                        // else there is no converter; this is not an exception case.
                    }
                }
            }
        }

        internal override void WriteEnumerable(JsonSerializerOptions options, ref WriteStackFrame current, ref Utf8JsonWriter writer)
        {
            if (ValueConverter != null)
            {
                Debug.Assert(current.Enumerator != null);

                object value = current.Enumerator.Current;
                if (value == null)
                {
                    writer.WriteNull(_name);
                }
                else
                {
                    if (_escapedName != null)
                    {
                        ValueConverter.Write(_escapedName, (TValue)value, ref writer);
                    }
                    else
                    {
                        ValueConverter.Write((TValue)value, ref writer);
                    }
                }
            }
            else
            {
                if (this is IJsonValueConverter<TValue> converter)
                {
                    Debug.Assert(current.Enumerator != null);
                    TValue value = (TValue)current.Enumerator.Current;
                    if (value == null)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        converter.Write(value, ref writer);
                    }
                }
                // else there is no converter; this is not an exception case.
            }
        }

        public IJsonValueConverter<TValue> GetValueConverter()
        {
            if (this is IJsonValueConverter<TValue> converter)
            {
                return (IJsonValueConverter<TValue>)this;
            }

            return ValueConverter;
        }

        internal override void GetPolicies(JsonSerializerOptions options)
        {
            // ValueConverter
            ValueConverter = GetPropertyValueConverter(options);

            base.GetPolicies(options);
        }

        internal JsonValueConverter<TValue> GetPropertyValueConverter(JsonSerializerOptions options)
        {
            JsonValueConverterAttribute attr = DefaultConverters.GetPropertyValueConverter(ParentClassType, PropertyInfo, PropertyType, options);
            if (attr != null)
            {
                return attr.GetConverter<TValue>();
            }

            return null;
        }
    }
}
