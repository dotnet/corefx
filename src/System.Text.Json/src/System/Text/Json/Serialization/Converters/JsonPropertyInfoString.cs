// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoString : JsonPropertyInfo<string>, IJsonValueConverter<string>
    {
        public JsonPropertyInfoString(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public bool TryRead(Type valueType, ref Utf8JsonReader reader, out string value)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                value = default;
                return false;
            }

            value = reader.GetString();
            return true;
        }

        public void Write(string value, ref Utf8JsonWriter writer)
        {
            writer.WriteStringValue(value);
        }

        public void Write(Span<byte> escapedPropertyName, string value, ref Utf8JsonWriter writer)
        {
            writer.WriteString(escapedPropertyName, value);
        }
    }
}
