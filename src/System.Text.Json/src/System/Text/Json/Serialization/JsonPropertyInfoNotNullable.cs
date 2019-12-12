// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json
{
    /// <summary>
    /// Represents a strongly-typed property that is not a <see cref="Nullable{T}"/>.
    /// </summary>
    internal sealed class JsonPropertyInfoNotNullable<TClass, TDeclaredProperty, TRuntimeProperty, TConverter> :
        JsonPropertyInfoCommon<TClass, TDeclaredProperty, TRuntimeProperty, TConverter>
        where TConverter : TDeclaredProperty
    {
        protected override void OnRead(ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (Converter == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType);
            }

            TConverter value = Converter.Read(ref reader, RuntimePropertyType, Options);

            if (state.Current.ReturnValue == null)
            {
                state.Current.ReturnValue = value;
            }
            else
            {
                Set(state.Current.ReturnValue, value);
            }
        }

        protected override void OnReadEnumerable(ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (Converter == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType);
            }

            if (state.Current.KeyName == null && state.Current.IsProcessingDictionaryOrIDictionaryConstructible())
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType);
                return;
            }

            // We need an initialized array in order to store the values.
            if (state.Current.IsProcessingEnumerable() && state.Current.TempEnumerableValues == null && state.Current.ReturnValue == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType);
                return;
            }

            TConverter value = Converter.Read(ref reader, RuntimePropertyType, Options);
            JsonSerializer.ApplyValueToEnumerable(ref value, ref state);
        }

        protected override void OnWrite(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            TConverter value;
            if (IsPropertyPolicy)
            {
                value = (TConverter)current.CurrentValue;
            }
            else
            {
                value = (TConverter)Get(current.CurrentValue);
            }

            if (value == null)
            {
                Debug.Assert(EscapedName.HasValue);

                if (!IgnoreNullValues)
                {
                    writer.WriteNull(EscapedName.Value);
                }
            }
            else if (Converter != null)
            {
                if (EscapedName.HasValue)
                {
                    writer.WritePropertyName(EscapedName.Value);
                }

                Converter.Write(writer, value, Options);
            }
        }

        protected override void OnWriteDictionary(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            JsonSerializer.WriteDictionary(Converter, Options, ref current, writer);
        }

        protected override void OnWriteEnumerable(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            if (Converter != null)
            {
                Debug.Assert(current.CollectionEnumerator != null);

                TConverter value;
                if (current.CollectionEnumerator is IEnumerator<TConverter> enumerator)
                {
                    // Avoid boxing for strongly-typed enumerators such as returned from IList<T>.
                    value = enumerator.Current;
                }
                else
                {
                    value = (TConverter)current.CollectionEnumerator.Current;
                }

                if (value == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    Converter.Write(writer, value, Options);
                }
            }
        }

        public override Type GetDictionaryConcreteType()
        {
            return typeof(Dictionary<string, TRuntimeProperty>);
        }

        public override void GetDictionaryKeyAndValueFromGenericDictionary(ref WriteStackFrame writeStackFrame, out string key, out object value)
        {
            if (writeStackFrame.CollectionEnumerator is IEnumerator<KeyValuePair<string, TDeclaredProperty>> genericEnumerator)
            {
                key = genericEnumerator.Current.Key;
                value = genericEnumerator.Current.Value;
            }
            else
            {
                throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(
                    writeStackFrame.JsonPropertyInfo.DeclaredPropertyType,
                    writeStackFrame.JsonPropertyInfo.ParentClassType,
                    writeStackFrame.JsonPropertyInfo.PropertyInfo);
            }
        }
    }
}
