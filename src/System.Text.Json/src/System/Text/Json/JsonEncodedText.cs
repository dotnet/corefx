// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json
{
    public readonly struct JsonEncodedText : IEquatable<JsonEncodedText>
    {
        private readonly byte[] _utf8Value;
        private readonly string _value;

        public ReadOnlySpan<byte> EncodedUtf8Bytes => _utf8Value;

        public static JsonEncodedText Encode(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Encode(value.AsSpan());
        }

        public static JsonEncodedText Encode(ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
            {
                return new JsonEncodedText(Array.Empty<byte>());
            }

            return TranscodeAndEncode(value);
        }

        private static JsonEncodedText TranscodeAndEncode(ReadOnlySpan<char> value)
        {
            JsonWriterHelper.ValidateValue(value);

            int expectedByteCount = JsonReaderHelper.GetUtf8ByteCount(value);
            byte[] utf8Bytes = ArrayPool<byte>.Shared.Rent(expectedByteCount);

            JsonEncodedText encodedText;

            try
            {
                // Since GetUtf8ByteCount above already throws on invalid input, the transcoding
                // to UTF-8 is guaranteed to succeed here. Therefore, there's no need for a catch block.
                int actualByteCount = JsonReaderHelper.GetUtf8FromText(value, utf8Bytes);
                Debug.Assert(expectedByteCount == actualByteCount);

                encodedText = EncodeHelper(utf8Bytes.AsSpan(0, actualByteCount));
            }
            finally
            {
                // On the basis that this is user data, go ahead and clear it.
                ClearAndReturn(utf8Bytes, expectedByteCount);
            }

            return encodedText;
        }

        private static void ClearAndReturn(byte[] utf8Bytes, int written)
        {
            utf8Bytes.AsSpan(0, written).Clear();
            ArrayPool<byte>.Shared.Return(utf8Bytes);
        }

        public static JsonEncodedText Encode(ReadOnlySpan<byte> utf8Value)
        {
            if (utf8Value.Length == 0)
            {
                return new JsonEncodedText(Array.Empty<byte>());
            }

            JsonWriterHelper.ValidateValue(utf8Value);
            return EncodeHelper(utf8Value);
        }

        private static JsonEncodedText EncodeHelper(ReadOnlySpan<byte> utf8Value)
        {
            int idx = JsonWriterHelper.NeedsEscaping(utf8Value);

            if (idx != -1)
            {
                return new JsonEncodedText(GetEscapedString(utf8Value, idx));
            }
            else
            {
                return new JsonEncodedText(utf8Value.ToArray());
            }
        }

        private JsonEncodedText(byte[] utf8Value)
        {
            Debug.Assert(utf8Value != null);

            _value = JsonReaderHelper.GetTextFromUtf8(utf8Value);
            _utf8Value = utf8Value;
        }

        private static byte[] GetEscapedString(ReadOnlySpan<byte> utf8Value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= utf8Value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < utf8Value.Length);

            byte[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(utf8Value.Length, firstEscapeIndexVal);

            Span<byte> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc byte[length] :
                (valueArray = ArrayPool<byte>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(utf8Value, escapedValue, firstEscapeIndexVal, out int written);

            byte[] escapedString = escapedValue.Slice(0, written).ToArray();

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }

            return escapedString;
        }

        public bool Equals(JsonEncodedText other)
        {
            if (_value == null)
            {
                return other._value == null;
            }
            else
            {
                return _value.Equals(other._value);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is JsonEncodedText encodedText)
            {
                return Equals(encodedText);
            }
            return false;
        }

        public override string ToString()
            => _value ?? string.Empty;

        public override int GetHashCode()
            => _value == null ? 0 : _value.GetHashCode();
    }
}
