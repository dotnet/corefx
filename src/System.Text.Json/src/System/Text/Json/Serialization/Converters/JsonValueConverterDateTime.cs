﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterDateTime : JsonValueConverter<DateTime>
    {
        public override bool TryRead(Type valueType, ref Utf8JsonReader reader, out DateTime value)
        {
            return reader.TryGetDateTime(out value);
        }

        public override void Write(DateTime value, ref Utf8JsonWriter writer)
        {
            writer.WriteStringValue(value);
        }

        public override void Write(Span<byte> escapedPropertyName, DateTime value, ref Utf8JsonWriter writer)
        {
            writer.WriteString(escapedPropertyName, value);
        }
    }
}
