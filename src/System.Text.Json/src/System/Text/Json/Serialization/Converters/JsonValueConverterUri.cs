// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterUri : JsonValueConverter<Uri>
    {
        public override bool TryRead(Type valueType, ref Utf8JsonReader reader, out Uri value)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                value = default;
                return false;
            }

            // TODO: use reader.GetUri() when https://github.com/dotnet/corefx/issues/38647 is implemented.
            string uriString = reader.GetString();

            try
            {
                value = new Uri(uriString);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public override void Write(Uri value, Utf8JsonWriter writer)
        {
            // TODO: remove preprocessing when https://github.com/dotnet/corefx/issues/38647 is implemented.
            string uriString = value.OriginalString;

            if (uriString == string.Empty)
            {
                uriString = value.ToString();
            }

            writer.WriteStringValue(uriString);
        }

        public override void Write(JsonEncodedText propertyName, Uri value, Utf8JsonWriter writer)
        {
            // TODO: remove preprocessing when https://github.com/dotnet/corefx/issues/38647 is implemented.
            string uriString = value.OriginalString;

            if (uriString == string.Empty)
            {
                uriString = value.ToString();
            }

            writer.WriteString(propertyName, uriString);
        }
    }
}
