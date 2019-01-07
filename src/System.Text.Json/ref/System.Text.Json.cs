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
    public sealed partial class JsonReaderException : System.Exception
    {
        public JsonReaderException(string message, long lineNumber, long bytePositionInLine) { }
        public long BytePositionInLine { get { throw null; } }
        public long LineNumber { get { throw null; } }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public sealed partial class JsonWriterException : System.Exception
    {
        public JsonWriterException(string message) { }
    }
    public partial struct JsonReaderOptions
    {
        private object _dummy;
        public System.Text.Json.JsonCommentHandling CommentHandling { get { throw null; } set { } }
    }
    public partial struct JsonWriterOptions
    {
        private object _dummy;
        public bool Indented { get { throw null; } set { } }
        public bool SkipValidation { get { throw null; } set { } }
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
    public partial struct JsonWriterState
    {
        private object _dummy;
        public JsonWriterState(System.Text.Json.JsonWriterOptions options = default(System.Text.Json.JsonWriterOptions)) { throw null; }
        public long BytesCommitted { get { throw null; } }
        public long BytesWritten { get { throw null; } }
        public System.Text.Json.JsonWriterOptions Options { get { throw null; } }
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
    public ref partial struct Utf8JsonWriter
    {
        private object _dummy;
        public Utf8JsonWriter(System.Buffers.IBufferWriter<byte> bufferWriter, JsonWriterState state = default(System.Text.Json.JsonWriterState)) { throw null; }
        public long BytesCommitted { get { throw null; } }
        public long BytesWritten { get { throw null; } }
        public int CurrentDepth { get { throw null; } }
        public System.Text.Json.JsonWriterState CurrentState { get { throw null; } }
        public void Flush(bool isFinalBlock = true) { throw null; }
        public void WriteBoolean(ReadOnlySpan<byte> propertyName, bool value, bool suppressEscaping = false) { throw null; }
        public void WriteBoolean(ReadOnlySpan<char> propertyName, bool value, bool suppressEscaping = false) { throw null; }
        public void WriteBoolean(string propertyName, bool value, bool suppressEscaping = false) { throw null; }
        public void WriteBooleanValue(bool value) { throw null; }
        public void WriteCommentValue(ReadOnlySpan<byte> utf8Text, bool suppressEscaping = false) { throw null; }
        public void WriteCommentValue(ReadOnlySpan<char> utf16Text, bool suppressEscaping = false) { throw null; }
        public void WriteCommentValue(string utf16Text, bool suppressEscaping = false) { throw null; }
        public void WriteEndArray() { throw null; }
        public void WriteEndObject() { throw null; }
        public void WriteNull(ReadOnlySpan<byte> propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteNull(ReadOnlySpan<char> propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteNull(string propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteNullValue() { throw null; }
        public void WriteNumber(ReadOnlySpan<byte> propertyName, decimal value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<byte> propertyName, double value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<byte> propertyName, int value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<byte> propertyName, long value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<byte> propertyName, float value, bool suppressEscaping = false) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumber(ReadOnlySpan<byte> propertyName, uint value, bool suppressEscaping = false) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumber(ReadOnlySpan<byte> propertyName, ulong value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<char> propertyName, decimal value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<char> propertyName, double value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<char> propertyName, int value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<char> propertyName, long value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(ReadOnlySpan<char> propertyName, float value, bool suppressEscaping = false) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumber(ReadOnlySpan<char> propertyName, uint value, bool suppressEscaping = false) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumber(ReadOnlySpan<char> propertyName, ulong value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(string propertyName, decimal value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(string propertyName, double value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(string propertyName, int value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(string propertyName, long value, bool suppressEscaping = false) { throw null; }
        public void WriteNumber(string propertyName, float value, bool suppressEscaping = false) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumber(string propertyName, uint value, bool suppressEscaping = false) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumber(string propertyName, ulong value, bool suppressEscaping = false) { throw null; }
        public void WriteNumberValue(decimal value) { throw null; }
        public void WriteNumberValue(double value) { throw null; }
        public void WriteNumberValue(int value) { throw null; }
        public void WriteNumberValue(long value) { throw null; }
        public void WriteNumberValue(float value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumberValue(uint value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void WriteNumberValue(ulong value) { throw null; }
        public void WriteStartArray() { throw null; }
        public void WriteStartArray(ReadOnlySpan<byte> propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteStartArray(ReadOnlySpan<char> propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteStartArray(string propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteStartObject() { throw null; }
        public void WriteStartObject(ReadOnlySpan<byte> propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteStartObject(ReadOnlySpan<char> propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteStartObject(string propertyName, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<byte> propertyName, DateTime value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<byte> propertyName, DateTimeOffset value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<byte> propertyName, Guid value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<byte> propertyName, string value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<char> propertyName, DateTime value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<char> propertyName, DateTimeOffset value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<char> propertyName, Guid value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false) { throw null; }
        public void WriteString(ReadOnlySpan<char> propertyName, string value, bool suppressEscaping = false) { throw null; }
        public void WriteString(string propertyName, DateTime value, bool suppressEscaping = false) { throw null; }
        public void WriteString(string propertyName, DateTimeOffset value, bool suppressEscaping = false) { throw null; }
        public void WriteString(string propertyName, Guid value, bool suppressEscaping = false) { throw null; }
        public void WriteString(string propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false) { throw null; }
        public void WriteString(string propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false) { throw null; }
        public void WriteString(string propertyName, string value, bool suppressEscaping = false) { throw null; }
        public void WriteStringValue(DateTime value) { throw null; }
        public void WriteStringValue(DateTimeOffset value) { throw null; }
        public void WriteStringValue(Guid value) { throw null; }
        public void WriteStringValue(ReadOnlySpan<byte> utf8Text, bool suppressEscaping = false) { throw null; }
        public void WriteStringValue(ReadOnlySpan<char> utf16Text, bool suppressEscaping = false) { throw null; }
        public void WriteStringValue(string utf16Text, bool suppressEscaping = false) { throw null; }
    }
}
