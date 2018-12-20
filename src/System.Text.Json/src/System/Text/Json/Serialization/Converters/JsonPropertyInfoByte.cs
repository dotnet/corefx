// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoByte : JsonPropertyInfo<byte>, IJsonValueConverter<byte>
    {
        public JsonPropertyInfoByte(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public bool TryRead(Type valueType, ref Utf8JsonReader reader, out byte value)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                value = default;
                return false;
            }

            checked
            {
                value = (byte)reader.GetInt32();
            }

            return true;
        }

        public void Write(byte value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumberValue(value);
        }

        public void Write(Span<byte> escapedPropertyName, byte value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumber(escapedPropertyName, value);
        }
    }
}
