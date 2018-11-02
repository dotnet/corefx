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
        AllowComments = (byte)1,
        Default = (byte)0,
        SkipComments = (byte)2,
    }
    public partial class JsonReaderException : System.Exception
    {
        public JsonReaderException(string message, long lineNumber, long lineBytePosition) { }
        public long LineBytePosition { get { throw null; } }
        public long LineNumber { get { throw null; } }
    }
    public partial struct JsonReaderOptions
    {
        public System.Text.Json.JsonCommentHandling CommentHandling { get { throw null; } set { } }
    }
    public partial struct JsonReaderState
    {
        public long BytesConsumed { get { throw null; } }
        public int MaxDepth { get { throw null; } set { } }
        public System.Text.Json.JsonReaderOptions Options { get { throw null; } set { } }
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
    public ref partial struct JsonUtf8Reader
    {
        public JsonUtf8Reader(System.ReadOnlySpan<byte> jsonData, bool isFinalBlock, System.Text.Json.JsonReaderState state) { throw null; }
        public long BytesConsumed { get { throw null; } }
        public int CurrentDepth { get { throw null; } }
        public System.Text.Json.JsonReaderState CurrentState { get { throw null; } }
        public bool IsValueMultiSegment { get { throw null; } }
        public System.SequencePosition Position { get { throw null; } }
        public System.Text.Json.JsonTokenType TokenType { get { throw null; } }
        public System.Buffers.ReadOnlySequence<byte> ValueSequence { get { throw null; } }
        public System.ReadOnlySpan<byte> ValueSpan { get { throw null; } }
        public bool Read() { throw null; }
    }
}
