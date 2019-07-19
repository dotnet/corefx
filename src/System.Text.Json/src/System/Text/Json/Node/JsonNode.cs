// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// for now disabling error caused by not adding documentation to methods 
#pragma warning disable CS1591

using System.Buffers;
using System.IO;

namespace System.Text.Json
{
    public abstract partial class JsonNode
    {
        private protected JsonNode() { }
        public JsonElement AsJsonElement() { throw null; }

        public static JsonNode GetNode(JsonElement jsonElement) { throw null; }
        public static bool TryGetNode(JsonElement jsonElement, out JsonNode jsonNode) { throw null; }

        public static JsonNode Parse(string json) { throw null; }
        public static JsonNode Parse(ReadOnlySequence<byte> utf8Json) { throw null; }
        public static JsonNode Parse(Stream utf8Json) { throw null; }
        public static JsonNode Parse(ReadOnlyMemory<byte> utf8Json) { throw null; }
        public static JsonNode Parse(ReadOnlyMemory<char> json) { throw null; }


        public static JsonNode DeepCopy(JsonNode jsonNode) { throw null; }
        public static JsonNode DeepCopy(JsonElement jsonElement) { throw null; }
        public static JsonNode DeepCopy(JsonDocument jsonDocument) { throw null; }
    }
}

#pragma warning restore CS1591
