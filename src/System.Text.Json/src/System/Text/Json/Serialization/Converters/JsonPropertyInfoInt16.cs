// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoInt16 : JsonPropertyInfo<short>, IJsonValueConverter<short>
    {
        public JsonPropertyInfoInt16(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public bool TryRead(Type valueType, ref Utf8JsonReader reader, out short value)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                value = default;
                return false;
            }

            checked
            {
                value = (short)reader.GetInt32();
            }

            return true;
        }

        public void Write(short value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumberValue(value);
        }

        public void Write(Span<byte> escapedPropertyName, short value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumber(escapedPropertyName, value);
        }
    }
}
