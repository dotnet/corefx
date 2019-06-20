﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonConverterUri : JsonConverter<Uri>
    {
        public override Uri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // TODO: use reader.GetUri() when https://github.com/dotnet/corefx/issues/38647 is implemented.
            string uriString = reader.GetString();
            if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out Uri value))
            {
                return value;
            }

            ThrowHelper.ThrowJsonException();
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
        {
            // TODO: remove preprocessing when https://github.com/dotnet/corefx/issues/38647 is implemented.
            writer.WriteStringValue(value.OriginalString);
        }

        public override void Write(Utf8JsonWriter writer, Uri value, JsonEncodedText propertyName, JsonSerializerOptions options)
        {
            // TODO: remove preprocessing when https://github.com/dotnet/corefx/issues/38647 is implemented.
            writer.WriteString(propertyName, value.OriginalString);
        }
    }
}
