// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoBooleanNullable : JsonPropertyInfo<bool?>, IJsonValueConverter<bool?>
    {
        public JsonPropertyInfoBooleanNullable(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public bool TryRead(Type valueType, ref Utf8JsonReader reader, out bool? value)
        {
            if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
            {
                value = default;
                return false;
            }

            value = reader.GetBoolean();
            return true;
        }

        public void Write(bool? value, ref Utf8JsonWriter writer)
        {
            writer.WriteBooleanValue(value.Value);
        }

        public void Write(Span<byte> escapedPropertyName, bool? value, ref Utf8JsonWriter writer)
        {
            writer.WriteBoolean(escapedPropertyName, value.Value);
        }
    }
}
