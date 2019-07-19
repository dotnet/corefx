// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

// for now disabling error caused by not adding documentation to methods 
#pragma warning disable CS1591

namespace System.Text.Json
{
    public partial class JsonArray : JsonNode, IList<JsonNode>, IReadOnlyList<JsonNode>
    {
        public JsonArray() { }
        public JsonArray(IEnumerable<JsonNode> values) { }
        public JsonArray(IEnumerable<string> values) { }
        public JsonArray(IEnumerable<bool> values) { }
        public JsonArray(IEnumerable<byte> values) { }
        public JsonArray(IEnumerable<short> values) { }
        public JsonArray(IEnumerable<int> values) { }
        public JsonArray(IEnumerable<long> values) { }
        public JsonArray(IEnumerable<float> values) { }
        public JsonArray(IEnumerable<double> values) { }
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<sbyte> values) { }
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<ushort> values) { }
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<uint> values) { }
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<ulong> values) { }

        public JsonNode this[int idx] { get => throw null; set => throw null; }

        public void Add(JsonNode value) { }
        public void Add(string value) { }
        public void Add(bool value) { }
        public void Add(byte value) { }
        public void Add(short value) { }
        public void Add(int value) { }
        public void Add(long value) { }
        public void Add(float value) { }
        public void Add(double value) { }
        [CLSCompliant(false)]
        public void Add(sbyte value) { }
        [CLSCompliant(false)]
        public void Add(ushort value) { }
        [CLSCompliant(false)]
        public void Add(uint value) { }
        [CLSCompliant(false)]
        public void Add(ulong value) { }

        public void Insert(int index, JsonNode item) { throw null; }
        public void Insert(int index, string item) { throw null; }
        public void Insert(int index, bool item) { throw null; }
        public void Insert(int index, byte item) { throw null; }
        public void Insert(int index, short item) { throw null; }
        public void Insert(int index, int item) { throw null; }
        public void Insert(int index, long item) { throw null; }
        public void Insert(int index, float item) { throw null; }
        public void Insert(int index, double item) { throw null; }
        [CLSCompliant(false)]
        public void Insert(int index, sbyte item) { throw null; }
        [CLSCompliant(false)]
        public void Insert(int index, ushort item) { throw null; }
        [CLSCompliant(false)]
        public void Insert(int index, uint item) { throw null; }
        [CLSCompliant(false)]
        public void Insert(int index, ulong item) { throw null; }

        public bool Contains(JsonNode value) { throw null; }
        public bool Contains(string value) { throw null; }
        public bool Contains(bool value) { throw null; }
        public bool Contains(byte value) { throw null; }
        public bool Contains(short value) { throw null; }
        public bool Contains(int value) { throw null; }
        public bool Contains(long value) { throw null; }
        public bool Contains(float value) { throw null; }
        public bool Contains(double value) { throw null; }
        [CLSCompliant(false)]
        public bool Contains(sbyte value) { throw null; }
        [CLSCompliant(false)]
        public bool Contains(ushort value) { throw null; }
        [CLSCompliant(false)]
        public bool Contains(uint value) { throw null; }
        [CLSCompliant(false)]
        public bool Contains(ulong value) { throw null; }

        public int Count => throw null;
        public bool IsReadOnly => throw null;

        public int IndexOf(JsonNode item) { throw null; }
        public int LastIndexOf(JsonNode item) { throw null; }
        public void RemoveAt(int index) { throw null; }
        public void Clear() { throw null; }
        public bool Remove(JsonNode item) { throw null; }

        void ICollection<JsonNode>.CopyTo(JsonNode[] array, int arrayIndex) { throw null; }
        IEnumerator IEnumerable.GetEnumerator() { throw null; }
        public IEnumerator<JsonNode> GetEnumerator() { throw null; }
    }
}

#pragma warning restore CS1591
