// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// for now disabling error caused by not adding documentation to methods
#pragma warning disable CS1591

using System.Buffers;
using System.IO;

namespace System.Text.Json
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document.
    /// </summary>
    public abstract class JsonNode
    {
        private protected JsonNode() { }

        public JsonElement AsJsonElement() { throw null; }

        public abstract JsonNodeKind NodeKind { get; }

        public static JsonNode GetNode(JsonElement jsonElement) { throw null; }

        public static bool TryGetNode(JsonElement jsonElement, out JsonNode jsonNode) { throw null; }

        public static JsonNode Parse(string json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return DeepCopy(document);
            }
        }

        public static JsonNode Parse(ReadOnlySequence<byte> utf8Json)
        {
            using (JsonDocument document = JsonDocument.Parse(utf8Json))
            {
                return DeepCopy(document);
            }
        }

        public static JsonNode Parse(Stream utf8Json)
        {
            using (JsonDocument document = JsonDocument.Parse(utf8Json))
            {
                return DeepCopy(document);
            }
        }

        public static JsonNode Parse(ReadOnlyMemory<byte> utf8Json)
        {
            using (JsonDocument document = JsonDocument.Parse(utf8Json))
            {
                return DeepCopy(document);
            }
        }

        public static JsonNode Parse(ReadOnlyMemory<char> json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return DeepCopy(document);
            }
        }

        public static JsonNode DeepCopy(JsonNode jsonNode) => jsonNode.Clone();

        public abstract JsonNode Clone();

        public static JsonNode DeepCopy(JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Object:
                    JsonObject jsonObject = new JsonObject();
                    foreach (JsonProperty property in jsonElement.EnumerateObject())
                    {
                        jsonObject.Add(property.Name, DeepCopy(property.Value));
                    }
                    return jsonObject;
                case JsonValueKind.Array:
                    JsonArray jsonArray = new JsonArray();
                    foreach (JsonElement element in jsonElement.EnumerateArray())
                    {
                        jsonArray.Add(DeepCopy(element));
                    }
                    return jsonArray;
                case JsonValueKind.Number:
                    return new JsonNumber(jsonElement.GetRawText());
                case JsonValueKind.String:
                    return new JsonString(jsonElement.GetString());
                case JsonValueKind.True:
                    return new JsonBoolean(true);
                case JsonValueKind.False:
                    return new JsonBoolean(false);
                case JsonValueKind.Null:
                    return null;
                default:
                    throw new ArgumentException();
            }

        }

        public static JsonNode DeepCopy(JsonDocument jsonDocument) => DeepCopy(jsonDocument.RootElement);
    }
}

#pragma warning restore CS1591
