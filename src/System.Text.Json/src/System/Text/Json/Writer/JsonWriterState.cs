// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    public struct JsonWriterState
    {
        internal long _bytesWritten;
        internal long _bytesCommitted;
        internal bool _inObject;
        internal bool _isNotPrimitive;
        internal JsonTokenType _tokenType;
        internal JsonWriterOptions _writerOptions;
        internal BitStack _bitStack;

        public long BytesWritten => _bytesWritten;

        public long BytesCommitted => _bytesCommitted;

        public JsonWriterState(JsonWriterOptions options = default)
        {
            _bytesWritten = default;
            _bytesCommitted = default;
            _inObject = default;
            _isNotPrimitive = default;
            _tokenType = default;
            _writerOptions = options;

            // Only allocate if the user writes a JSON payload beyond the depth that the _allocationFreeContainer can handle.
            // This way we avoid allocations in the common, default cases, and allocate lazily.
            _bitStack = default;
        }

        public JsonWriterOptions Options => _writerOptions;
    }
}
