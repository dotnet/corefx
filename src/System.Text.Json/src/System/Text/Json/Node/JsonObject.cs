// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

// for now disabling error caused by not adding documentation to methods 
#pragma warning disable CS1591

namespace System.Text.Json
{
    public partial class JsonObject : JsonNode, IEnumerable<KeyValuePair<string, JsonNode>>
    {
        public JsonObject() { }
        public JsonObject(IEnumerable<KeyValuePair<string, JsonNode>> jsonProperties) { }

        public JsonNode this[string key] { get => throw null; set => throw null; }
        public IEnumerator<KeyValuePair<string, JsonNode>> GetEnumerator() { throw null; }

        public void Add(KeyValuePair<string, JsonNode> jsonProperty) { }
        public void Add(string propertyName, JsonArray propertyValue) { }
        public void Add(string propertyName, JsonNode propertyValue) { }
        public void Add(string propertyName, string propertyValue) { }
        public void Add(string propertyName, bool propertyValue) { }
        public void Add(string propertyName, byte propertyValue) { }
        public void Add(string propertyName, short propertyValue) { }
        public void Add(string propertyName, int propertyValue) { }
        public void Add(string propertyName, long propertyValue) { }
        public void Add(string propertyName, float propertyValue) { }
        public void Add(string propertyName, double propertyValue) { }
        [CLSCompliant(false)]
        public void Add(string propertyName, sbyte propertyValue) { }
        [CLSCompliant(false)]
        public void Add(string propertyName, ushort propertyValue) { }
        [CLSCompliant(false)]
        public void Add(string propertyName, uint propertyValue) { }
        [CLSCompliant(false)]
        public void Add(string propertyName, ulong propertyValue) { }
        public void Add(string propertyName, IEnumerable<JsonNode> propertyValue) { }
        public void AddRange(IEnumerable<KeyValuePair<string, JsonNode>> jsonProperties) { }

        public bool Remove(string propertyName) { throw null; }
        public bool ContainsProperty(string propertyName) { throw null; }

        public bool ModifyPropertyName(string oldName, string newName) { throw null; }

        public JsonNode GetProperty(string propertyName) { throw null; }
        public bool TryGetProperty(string propertyName, out JsonNode jsonNode) { throw null; }
        public JsonObject GetObjectProperty(string propertyName) { throw null; }
        public bool TryGetObjectProperty(string propertyName, out JsonObject jsonObject) { throw null; }
        public JsonArray GetArrayProperty(string propertyName) { throw null; }
        public bool TryGetArrayProperty(string propertyName, out JsonArray jsonArray) { throw null; }
        public IEnumerable<JsonNode> GetAllProperties(string propertyName) { throw null; }


        public ICollection<string> PropertyNames => throw null;
        public ICollection<JsonNode> Values => throw null;

        IEnumerator IEnumerable.GetEnumerator() { throw null; }
    }
}

#pragma warning restore CS1591
