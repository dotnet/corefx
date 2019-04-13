// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Represents a strongly-typed property that is a <see cref="Nullable{T}"/>.
    /// </summary>
    internal sealed class JsonPropertyInfoNullable<TClass, TProperty>
        : JsonPropertyInfoCommon<TClass, TProperty?, TProperty>
        where TProperty : struct
    {
        // should this be cached somewhere else so that it's not populated per TClass as well as TProperty?
        private static readonly Type s_underlyingType = typeof(TProperty);

        internal JsonPropertyInfoNullable(
            Type parentClassType,
            Type declaredPropertyType,
            Type runtimePropertyType,
            PropertyInfo propertyInfo,
            Type elementType,
            JsonSerializerOptions options) :
            base(parentClassType, declaredPropertyType, runtimePropertyType, propertyInfo, elementType, options)
        {
        }

        internal override void Read(JsonTokenType tokenType, JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (ElementClassInfo != null)
            {
                // Forward the setter to the value-based JsonPropertyInfo.
                JsonPropertyInfo propertyInfo = ElementClassInfo.GetPolicyProperty();
                propertyInfo.ReadEnumerable(tokenType, options, ref state, ref reader);
            }
            else if (ShouldDeserialize)
            {
                if (ValueConverter != null)
                {
                    if (ValueConverter.TryRead(s_underlyingType, ref reader, out TProperty value))
                    {
                        if (state.Current.ReturnValue == null)
                        {
                            state.Current.ReturnValue = value;
                        }
                        else
                        {
                            Set((TClass)state.Current.ReturnValue, value);
                        }

                        return;
                    }
                }

                ThrowHelper.ThrowJsonReaderException_DeserializeUnableToConvertValue(RuntimePropertyType, reader, state);
            }
        }

        internal override void ReadEnumerable(JsonTokenType tokenType, JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (ValueConverter == null || !ValueConverter.TryRead(typeof(TProperty), ref reader, out TProperty value))
            {
                ThrowHelper.ThrowJsonReaderException_DeserializeUnableToConvertValue(RuntimePropertyType, reader, state);
                return;
            }

            // Converting to TProperty? here lets us share a common ApplyValue() with ApplyNullValue().
            TProperty? nullableValue = new TProperty?(value);
            ApplyValue(nullableValue, options, ref state.Current);
        }

        // If this method is changed, also change JsonPropertyInfoNotNullable.ApplyValue and JsonSerializer.ApplyObjectToEnumerable
        private void ApplyValue(TProperty? value, JsonSerializerOptions options, ref ReadStackFrame frame)
        {
            if (frame.IsEnumerable())
            {
                if (frame.TempEnumerableValues != null)
                {
                    ((IList<TProperty?>)frame.TempEnumerableValues).Add(value);
                }
                else
                {
                    ((IList<TProperty?>)frame.ReturnValue).Add(value);
                }
            }
            else if (frame.IsPropertyEnumerable())
            {
                Debug.Assert(frame.JsonPropertyInfo != null);
                Debug.Assert(frame.ReturnValue != null);
                if (frame.TempEnumerableValues != null)
                {
                    ((IList<TProperty?>)frame.TempEnumerableValues).Add(value);
                }
                else
                {
                    ((IList<TProperty?>)frame.JsonPropertyInfo.GetValueAsObject(frame.ReturnValue, options)).Add(value);
                }
            }
            else
            {
                Debug.Assert(frame.JsonPropertyInfo != null);
                frame.JsonPropertyInfo.SetValueAsObject(frame.ReturnValue, value, options);
            }
        }

        internal override void ApplyNullValue(JsonSerializerOptions options, ref ReadStack state)
        {
            ApplyValue(null, options, ref state.Current);
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
            else if (ShouldSerialize)
            {
                TProperty? value;
                if (_isPropertyPolicy)
                {
                    value = (TProperty?)current.CurrentValue;
                }
                else
                {
                    value = Get((TClass)current.CurrentValue);
                }

                if (value == null)
                {
                    if (_escapedName == null)
                    {
                        writer.WriteNullValue();
                    }
                    else if (!IgnoreNullValues)
                    {
                        writer.WriteNull(_escapedName);
                    }
                }
                else if (ValueConverter != null)
                {
                    if (_escapedName != null)
                    {
                        ValueConverter.Write(_escapedName, value.GetValueOrDefault(), ref writer);
                    }
                    else
                    {
                        ValueConverter.Write(value.GetValueOrDefault(), ref writer);
                    }
                }
            }
        }

        internal override void WriteEnumerable(JsonSerializerOptions options, ref WriteStackFrame current, ref Utf8JsonWriter writer)
        {
            if (ValueConverter != null)
            {
                Debug.Assert(current.Enumerator != null);

                TProperty? value;
                if (current.Enumerator is IEnumerator<TProperty?>)
                {
                    value = ((IEnumerator<TProperty?>)current.Enumerator).Current;
                }
                else
                {
                    value = (TProperty?)current.Enumerator.Current;
                }

                if (value == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    ValueConverter.Write(value.GetValueOrDefault(), ref writer);
                }
            }
        }
    }
}
