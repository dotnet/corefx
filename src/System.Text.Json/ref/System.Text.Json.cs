// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Text.Json
{
    public enum JsonCommentHandling : byte
    {
        Allow = (byte)1,
        Disallow = (byte)0,
        Skip = (byte)2,
    }
    public sealed partial class JsonDocument : System.IDisposable
    {
        internal JsonDocument() { }
        public System.Text.Json.JsonElement RootElement { get { throw null; } }
        public void Dispose() { }
        public static System.Text.Json.JsonDocument Parse(System.Buffers.ReadOnlySequence<byte> utf8Json, System.Text.Json.JsonReaderOptions readerOptions = default(System.Text.Json.JsonReaderOptions)) { throw null; }
        public static System.Text.Json.JsonDocument Parse(System.IO.Stream utf8Json, System.Text.Json.JsonReaderOptions readerOptions = default(System.Text.Json.JsonReaderOptions)) { throw null; }
        public static System.Text.Json.JsonDocument Parse(System.ReadOnlyMemory<byte> utf8Json, System.Text.Json.JsonReaderOptions readerOptions = default(System.Text.Json.JsonReaderOptions)) { throw null; }
        public static System.Text.Json.JsonDocument Parse(System.ReadOnlyMemory<char> json, System.Text.Json.JsonReaderOptions readerOptions = default(System.Text.Json.JsonReaderOptions)) { throw null; }
        public static System.Text.Json.JsonDocument Parse(string json, System.Text.Json.JsonReaderOptions readerOptions = default(System.Text.Json.JsonReaderOptions)) { throw null; }
        public static System.Threading.Tasks.Task<System.Text.Json.JsonDocument> ParseAsync(System.IO.Stream utf8Json, System.Text.Json.JsonReaderOptions readerOptions = default(System.Text.Json.JsonReaderOptions), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
    public readonly partial struct JsonElement
    {
        private readonly object _dummy;
        public System.Text.Json.JsonElement this[int index] { get { throw null; } }
        public System.Text.Json.JsonValueType Type { get { throw null; } }
        public System.Text.Json.JsonElement.ArrayEnumerator EnumerateArray() { throw null; }
        public System.Text.Json.JsonElement.ObjectEnumerator EnumerateObject() { throw null; }
        public int GetArrayLength() { throw null; }
        public bool GetBoolean() { throw null; }
        public decimal GetDecimal() { throw null; }
        public double GetDouble() { throw null; }
        public int GetInt32() { throw null; }
        public long GetInt64() { throw null; }
        public System.Text.Json.JsonElement GetProperty(System.ReadOnlySpan<byte> utf8PropertyName) { throw null; }
        public System.Text.Json.JsonElement GetProperty(System.ReadOnlySpan<char> propertyName) { throw null; }
        public System.Text.Json.JsonElement GetProperty(string propertyName) { throw null; }
        public string GetRawText() { throw null; }
        public float GetSingle() { throw null; }
        public string GetString() { throw null; }
        [System.CLSCompliantAttribute(false)]
        public uint GetUInt32() { throw null; }
        [System.CLSCompliantAttribute(false)]
        public ulong GetUInt64() { throw null; }
        public override string ToString() { throw null; }
        public bool TryGetDecimal(out decimal value) { throw null; }
        public bool TryGetDouble(out double value) { throw null; }
        public bool TryGetInt32(out int value) { throw null; }
        public bool TryGetInt64(out long value) { throw null; }
        public bool TryGetProperty(System.ReadOnlySpan<byte> utf8PropertyName, out System.Text.Json.JsonElement value) { throw null; }
        public bool TryGetProperty(System.ReadOnlySpan<char> propertyName, out System.Text.Json.JsonElement value) { throw null; }
        public bool TryGetProperty(string propertyName, out System.Text.Json.JsonElement value) { throw null; }
        public bool TryGetSingle(out float value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public bool TryGetUInt32(out uint value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public bool TryGetUInt64(out ulong value) { throw null; }
        public partial struct ArrayEnumerator : System.Collections.Generic.IEnumerable<System.Text.Json.JsonElement>, System.Collections.Generic.IEnumerator<System.Text.Json.JsonElement>, System.Collections.IEnumerable, System.Collections.IEnumerator, System.IDisposable
        {
            private object _dummy;
            public System.Text.Json.JsonElement Current { get { throw null; } }
            object System.Collections.IEnumerator.Current { get { throw null; } }
            public void Dispose() { }
            public System.Text.Json.JsonElement.ArrayEnumerator GetEnumerator() { throw null; }
            public bool MoveNext() { throw null; }
            public void Reset() { }
            System.Collections.Generic.IEnumerator<System.Text.Json.JsonElement> System.Collections.Generic.IEnumerable<System.Text.Json.JsonElement>.GetEnumerator() { throw null; }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        }
        public partial struct ObjectEnumerator : System.Collections.Generic.IEnumerable<System.Text.Json.JsonProperty>, System.Collections.Generic.IEnumerator<System.Text.Json.JsonProperty>, System.Collections.IEnumerable, System.Collections.IEnumerator, System.IDisposable
        {
            private object _dummy;
            public System.Text.Json.JsonProperty Current { get { throw null; } }
            object System.Collections.IEnumerator.Current { get { throw null; } }
            public void Dispose() { }
            public System.Text.Json.JsonElement.ObjectEnumerator GetEnumerator() { throw null; }
            public bool MoveNext() { throw null; }
            public void Reset() { }
            System.Collections.Generic.IEnumerator<System.Text.Json.JsonProperty> System.Collections.Generic.IEnumerable<System.Text.Json.JsonProperty>.GetEnumerator() { throw null; }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        }
    }
    public readonly partial struct JsonProperty
    {
        private readonly object _dummy;
        public string Name { get { throw null; } }
        public System.Text.Json.JsonElement Value { get { throw null; } }
        public override string ToString() { throw null; }
    }
    public sealed partial class JsonReaderException : System.Exception
    {
        public JsonReaderException(string message, long lineNumber, long bytePositionInLine) { }
        public long BytePositionInLine { get { throw null; } }
        public long LineNumber { get { throw null; } }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public partial struct JsonReaderOptions
    {
        private object _dummy;
        public System.Text.Json.JsonCommentHandling CommentHandling { get { throw null; } set { } }
    }
    public partial struct JsonReaderState
    {
        private object _dummy;
        public JsonReaderState(int maxDepth = 64, System.Text.Json.JsonReaderOptions options = default(System.Text.Json.JsonReaderOptions)) { throw null; }
        public long BytesConsumed { get { throw null; } }
        public int MaxDepth { get { throw null; } }
        public System.Text.Json.JsonReaderOptions Options { get { throw null; } }
        public System.SequencePosition Position { get { throw null; } }
    }
    public enum JsonTokenType : byte
    {
        Comment = (byte)11,
        EndArray = (byte)4,
        EndObject = (byte)2,
        False = (byte)9,
        None = (byte)0,
        Null = (byte)10,
        Number = (byte)7,
        PropertyName = (byte)5,
        StartArray = (byte)3,
        StartObject = (byte)1,
        String = (byte)6,
        True = (byte)8,
    }
    public enum JsonValueType : byte
    {
        Array = (byte)2,
        False = (byte)6,
        Null = (byte)7,
        Number = (byte)4,
        Object = (byte)1,
        String = (byte)3,
        True = (byte)5,
        Undefined = (byte)0,
    }
    public ref partial struct Utf8JsonReader
    {
        private object _dummy;
        public Utf8JsonReader(in System.Buffers.ReadOnlySequence<byte> jsonData, bool isFinalBlock, System.Text.Json.JsonReaderState state) { throw null; }
        public Utf8JsonReader(System.ReadOnlySpan<byte> jsonData, bool isFinalBlock, System.Text.Json.JsonReaderState state) { throw null; }
        public long BytesConsumed { get { throw null; } }
        public int CurrentDepth { get { throw null; } }
        public System.Text.Json.JsonReaderState CurrentState { get { throw null; } }
        public bool HasValueSequence { get { throw null; } }
        public System.SequencePosition Position { get { throw null; } }
        public System.Text.Json.JsonTokenType TokenType { get { throw null; } }
        public System.Buffers.ReadOnlySequence<byte> ValueSequence { get { throw null; } }
        public System.ReadOnlySpan<byte> ValueSpan { get { throw null; } }
        public bool GetBooleanValue() { throw null; }
        public string GetStringValue() { throw null; }
        public bool Read() { throw null; }
        public bool TryGetDecimalValue(out decimal value) { throw null; }
        public bool TryGetDoubleValue(out double value) { throw null; }
        public bool TryGetInt32Value(out int value) { throw null; }
        public bool TryGetInt64Value(out long value) { throw null; }
        public bool TryGetSingleValue(out float value) { throw null; }
    }
}
