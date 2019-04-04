// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterObject : JsonValueConverter<object>
    {
        public override bool TryRead(Type valueType, ref Utf8JsonReader reader, out object value)
        {
            // Currently we only support converting True and False tokens to typeof(object).
            if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
            {
                ThrowHelper.ThrowJsonElementConversionNotSupported();
            }

            value = reader.GetBoolean();
            return true;
        }

        public override void Write(object value, ref Utf8JsonWriter writer)
        {
            if (value.GetType() != typeof(bool))
            {
                ThrowHelper.ThrowJsonElementConversionNotSupported();
            }

            writer.WriteBooleanValue((bool)value);
        }

        public override void Write(Span<byte> escapedPropertyName, object value, ref Utf8JsonWriter writer)
        {
            if (value.GetType() != typeof(bool))
            {
                ThrowHelper.ThrowJsonElementConversionNotSupported();
            }

            writer.WriteBoolean(escapedPropertyName, (bool)value);
        }
    }
}
