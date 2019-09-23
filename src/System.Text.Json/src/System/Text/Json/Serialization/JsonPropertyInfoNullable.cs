﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// Represents a strongly-typed property that is a <see cref="Nullable{T}"/>.
    /// </summary>
    internal sealed class JsonPropertyInfoNullable<TClass, TProperty>
        : JsonPropertyInfoCommon<TClass, TProperty?, TProperty>
        where TProperty : struct
    {
        private static readonly Type s_underlyingType = typeof(TProperty);

        protected override void OnRead(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (Converter == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType, reader, state.JsonPath());
            }

            TProperty value = Converter.Read(ref reader, s_underlyingType, Options);

            if (state.Current.ReturnValue == null)
            {
                state.Current.ReturnValue = value;
            }
            else
            {
                Set(state.Current.ReturnValue, value);
            }
        }

        protected override void OnReadEnumerable(JsonTokenType tokenType, ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (Converter == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType, reader, state.JsonPath());
            }

            TProperty value = Converter.Read(ref reader, s_underlyingType, Options);
            TProperty? nullableValue = new TProperty?(value);
            JsonSerializer.ApplyValueToEnumerable(Options, ref state, ref nullableValue);
        }

        protected override void OnWrite(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            TProperty? value;
            if (IsPropertyPolicy)
            {
                value = (TProperty?)current.CurrentValue;
            }
            else
            {
                value = Get(current.CurrentValue);
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

                Converter.Write(writer, value.GetValueOrDefault(), Options);
            }
        }

        protected override void OnWriteDictionary(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            Debug.Assert(Converter != null && current.CollectionEnumerator != null);

            string key = null;
            TProperty? value = null;
            if (current.CollectionEnumerator is IEnumerator<KeyValuePair<string, TProperty?>> enumerator)
            {
                key = enumerator.Current.Key;
                value = enumerator.Current.Value;
            }
            else if (current.CollectionEnumerator is IDictionaryEnumerator dictionaryEnumerator)
            {
                key = (string)dictionaryEnumerator.Key;
                value = (TProperty?)dictionaryEnumerator.Value;
            }

            Debug.Assert(key != null);

            if (value == null)
            {
                writer.WriteNull(key);
            }
            else
            {
                if (Options.DictionaryKeyPolicy != null)
                {
                    key = Options.DictionaryKeyPolicy.ConvertName(key);

                    if (key == null)
                    {
                        ThrowHelper.ThrowInvalidOperationException_SerializerDictionaryKeyNull(Options.DictionaryKeyPolicy.GetType());
                    }
                }

                writer.WritePropertyName(key);
                Converter.Write(writer, value.GetValueOrDefault(), Options);
            }
        }

        protected override void OnWriteEnumerable(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            if (Converter != null)
            {
                Debug.Assert(current.CollectionEnumerator != null);

                TProperty? value;
                if (current.CollectionEnumerator is IEnumerator<TProperty?> enumerator)
                {
                    // Avoid boxing for strongly-typed enumerators such as returned from IList<T>.
                    value = enumerator.Current;
                }
                else
                {
                    value = (TProperty?)current.CollectionEnumerator.Current;
                }

                if (value == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    Converter.Write(writer, value.GetValueOrDefault(), Options);
                }
            }
        }
    }
}
