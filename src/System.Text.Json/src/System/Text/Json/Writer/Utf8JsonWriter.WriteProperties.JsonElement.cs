// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        // TODO: Add comments
        public void WriteElement(string propertyName, JsonElement value, bool escape = true)
            => WriteElement(propertyName.AsSpan(), value, escape);

        public void WriteElement(ReadOnlySpan<char> propertyName, JsonElement value, bool escape = true)
            => WriteElementCore(propertyName, value.GetRawText().AsSpan(), value.Type, escape);

        private void WriteElementCore(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value, JsonValueType valueType, bool escape)
        {
            JsonWriterHelper.ValidatePropertyAndValue(propertyName, value);

            if (escape)
            {
                WriteElementEscape(propertyName, value);
            }
            else
            {
                WriteElementDontEscape(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = ToTokenTypeProperty(valueType);
        }

        private JsonTokenType ToTokenTypeProperty(JsonValueType valueType)
        {
            switch (valueType)
            {
                case JsonValueType.Undefined:
                    return JsonTokenType.None;
                case JsonValueType.Array:
                    return JsonTokenType.StartObject;
                case JsonValueType.Object:
                    return JsonTokenType.StartObject;
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

        private void WriteElementEscape(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteElementEscapePropertyOnly(propertyName, propertyIdx, value);
            }
            else
            {
                WriteElementByOptions(propertyName, value);
            }
        }

        private void WriteElementEscapePropertyOnly(ReadOnlySpan<char> propertyName, int firstEscapeIndex, ReadOnlySpan<char> escapedValue)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);
            Debug.Assert(firstEscapeIndex >= 0 && firstEscapeIndex < propertyName.Length);

            char[] propertyArray = ArrayPool<char>.Shared.Rent(JsonWriterHelper.GetMaxEscapedLength(propertyName.Length, firstEscapeIndex));
            Span<char> escapedPropertyName = propertyArray;
            JsonWriterHelper.EscapeString(propertyName, escapedPropertyName, firstEscapeIndex, out int written);

            WriteElementByOptions(escapedPropertyName.Slice(0, written), escapedValue);

            ArrayPool<char>.Shared.Return(propertyArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteElementDontEscape(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> value)
        {
            WriteElementByOptions(escapedPropertyName, value);
        }

        private void WriteElementByOptions(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                // Should this throw something like NotSupportedException instead?
                WriteElementIndented(propertyName, value);
            }
            else
            {
                WriteElementMinimized(propertyName, value);
            }
        }

        private void WriteElementIndented(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteElementValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteElementMinimized(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteElementValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteElementValue(ReadOnlySpan<char> escapedValue, ref int idx)
        {
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
        }
    }
}
