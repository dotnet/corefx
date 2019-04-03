﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterDateTimeOffset : JsonValueConverter<DateTimeOffset>
    {
        public override bool TryRead(Type valueType, ref Utf8JsonReader reader, out DateTimeOffset value)
        {
            return reader.TryGetDateTimeOffset(out value);
        }

        public override void Write(DateTimeOffset value, ref Utf8JsonWriter writer)
        {
            writer.WriteStringValue(value);
        }

        public override void Write(Span<byte> escapedPropertyName, DateTimeOffset value, ref Utf8JsonWriter writer)
        {
            writer.WriteString(escapedPropertyName, value);
        }
    }
}
