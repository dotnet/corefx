﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterUInt16Nullable : JsonValueConverter<ushort?>
    {
        public override bool TryRead(Type valueType, ref Utf8JsonReader reader, out ushort? value)
        {
            if (reader.TokenType != JsonTokenType.Number ||
                !reader.TryGetInt32(out int rawValue) ||
                !JsonHelpers.IsInRangeInclusive(rawValue, ushort.MinValue, ushort.MaxValue))
            {
                value = default;
                return false;
            }

            value = (ushort?)rawValue;
            return true;
        }

        public override void Write(ushort? value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumberValue(value.Value);
        }

        public override void Write(Span<byte> escapedPropertyName, ushort? value, ref Utf8JsonWriter writer)
        {
            writer.WriteNumber(escapedPropertyName, value.Value);
        }
    }
}
