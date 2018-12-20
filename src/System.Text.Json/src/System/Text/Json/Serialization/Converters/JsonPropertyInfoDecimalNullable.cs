// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoDecimalNullable : JsonPropertyInfo<decimal?>, IJsonValueConverter<decimal?>
    {
        public JsonPropertyInfoDecimalNullable(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public bool TryRead(Type valueType, ref Utf8JsonReader reader, out decimal? value)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                value = default;
                return false;
            }

            value = reader.GetDecimal();
            return true;
        }

        public void Write(decimal? value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumberValue(value.Value);
        }

        public void Write(Span<byte> escapedPropertyName, decimal? value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumber(escapedPropertyName, value.Value);
        }
    }
}
