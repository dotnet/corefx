// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoDateTime : JsonPropertyInfo<DateTime>, IJsonValueConverter<DateTime>
    {
        public JsonPropertyInfoDateTime(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public bool TryRead(Type valueType, ref Utf8JsonReader reader, out DateTime value)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                value = default;
                return false;
            }

            ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            return Utf8Parser.TryParse(span, out value, out int bytesConsumed, 'O') && span.Length == bytesConsumed;
        }

        public void Write(DateTime value, ref Utf8JsonWriter writer)
        {
            byte[] stringValue = JsonReaderHelper.s_utf8Encoding.GetBytes(value.ToString("O"));
            writer.WriteStringValue(stringValue);
        }

        public void Write(Span<byte> escapedPropertyName, DateTime value, ref Utf8JsonWriter writer)
        {
            byte[] stringValue = JsonReaderHelper.s_utf8Encoding.GetBytes(value.ToString("O"));
            writer.WriteString(escapedPropertyName, stringValue);
        }
    }
}
