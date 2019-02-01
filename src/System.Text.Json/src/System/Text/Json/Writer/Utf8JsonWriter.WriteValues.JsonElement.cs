// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        // TODO: Add comments
        public void WriteElementValue(JsonElement value)
        {
            if (_writerOptions.Indented)
            {
                // Should this throw something like NotSupportedException instead?
            }

            ValidateWritingValue();
            ReadOnlySpan<char> escapedValue = value.GetRawText().AsSpan();

            int idx = 0;
            WriteListSeparator(ref idx);

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedValue);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _buffer.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(ref idx);
            }
            Advance(idx);

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = ToTokenTypeValue(value.Type);
        }

        private JsonTokenType ToTokenTypeValue(JsonValueType valueType)
        {
            switch (valueType)
            {
                case JsonValueType.Undefined:
                    return JsonTokenType.None;
                case JsonValueType.Array:
                    return JsonTokenType.StartArray;
                case JsonValueType.Object:
                    return JsonTokenType.StartArray;
                case JsonValueType.String:
                case JsonValueType.Number:
                case JsonValueType.True:
                case JsonValueType.False:
                case JsonValueType.Null:
                    return (JsonTokenType)((byte)valueType + 3);
                default:
                    Debug.Fail($"No mapping for token type {valueType}");
                    return JsonTokenType.None;
            }
        }
    }
}
