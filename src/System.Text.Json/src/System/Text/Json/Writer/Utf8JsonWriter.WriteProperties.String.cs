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
        /// <summary>
        /// Writes the property name and string text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(string propertyName, string value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value.AsSpan(), suppressEscaping);

        /// <summary>
        /// Writes the UTF-16 property name and UTF-16 text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(propertyName, value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(propertyName, value);
            }
            else
            {
                WriteStringSuppressTrue(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        /// <summary>
        /// Writes the UTF-8 property name and UTF-8 text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="value">The UTF-8 encoded value to be written as a JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(propertyName, value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(propertyName, value);
            }
            else
            {
                WriteStringSuppressTrue(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        /// <summary>
        /// Writes the property name and UTF-16 text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(string propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        /// <summary>
        /// Writes the UTF-8 property name and UTF-16 text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(propertyName, value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(propertyName, value);
            }
            else
            {
                WriteStringSuppressTrue(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        /// <summary>
        /// Writes the property name and UTF-8 text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The UTF-8 encoded value to be written as a JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(string propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        /// <summary>
        /// Writes the UTF-16 property name and UTF-8 text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The UTF-8 encoded value to be written as a JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(propertyName, value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(propertyName, value);
            }
            else
            {
                WriteStringSuppressTrue(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        /// <summary>
        /// Writes the UTF-16 property name and string text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(ReadOnlySpan<char> propertyName, string value, bool suppressEscaping = false)
            => WriteString(propertyName, value.AsSpan(), suppressEscaping);

        /// <summary>
        /// Writes the UTF-8 property name and string text value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.
        /// The value is always escaped</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name or value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteString(ReadOnlySpan<byte> propertyName, string value, bool suppressEscaping = false)
            => WriteString(propertyName, value.AsSpan(), suppressEscaping);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(escapedPropertyName, value, valueIdx);
            }
            else
            {
                WriteStringByOptions(escapedPropertyName, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(escapedPropertyName, value, valueIdx);
            }
            else
            {
                WriteStringByOptions(escapedPropertyName, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(escapedPropertyName, value, valueIdx);
            }
            else
            {
                WriteStringByOptions(escapedPropertyName, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(escapedPropertyName, value, valueIdx);
            }
            else
            {
                WriteStringByOptions(escapedPropertyName, value);
            }
        }

        private void WriteStringEscapeValueOnly(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = ArrayPool<char>.Shared.Rent(firstEscapeIndex + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<char> span = valueArray;
            JsonWriterHelper.EscapeString(value, span, firstEscapeIndex, out int written);

            WriteStringByOptions(escapedPropertyName, span.Slice(0, written));

            ArrayPool<char>.Shared.Return(valueArray);
        }

        private void WriteStringEscapeValueOnly(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<byte> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = ArrayPool<byte>.Shared.Rent(firstEscapeIndex + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<byte> span = valueArray;
            JsonWriterHelper.EscapeString(value, span, firstEscapeIndex, out int written);

            WriteStringByOptions(escapedPropertyName, span.Slice(0, written));

            ArrayPool<byte>.Shared.Return(valueArray);
        }

        private void WriteStringEscapeValueOnly(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<byte> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = ArrayPool<byte>.Shared.Rent(firstEscapeIndex + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<byte> span = valueArray;
            JsonWriterHelper.EscapeString(value, span, firstEscapeIndex, out int written);

            WriteStringByOptions(escapedPropertyName, span.Slice(0, written));

            ArrayPool<byte>.Shared.Return(valueArray);
        }

        private void WriteStringEscapeValueOnly(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<char> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = ArrayPool<char>.Shared.Rent(firstEscapeIndex + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<char> span = valueArray;
            JsonWriterHelper.EscapeString(value, span, firstEscapeIndex, out int written);

            WriteStringByOptions(escapedPropertyName, span.Slice(0, written));

            ArrayPool<char>.Shared.Return(valueArray);
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(propertyName, value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(propertyName, value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(propertyName, value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(propertyName, value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }
        }

        private void WriteStringEscapePropertyOrValue(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] valueArray = null;
            char[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<char> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<char>.Shared.Rent(length);
                    span = valueArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        char* ptr = stackalloc char[length];
                        span = new Span<char>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(value, span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
                int length = firstEscapeIndexProp + JsonConstants.MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
                Span<char> span;
                if (length > StackallocThreshold)
                {
                    propertyArray = ArrayPool<char>.Shared.Rent(length);
                    span = propertyArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        char* ptr = stackalloc char[length];
                        span = new Span<char>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);
                propertyName = span.Slice(0, written);
            }

            WriteStringByOptions(propertyName, value);

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }

            if (propertyArray != null)
            {
                ArrayPool<char>.Shared.Return(propertyArray);
            }
        }

        private void WriteStringEscapePropertyOrValue(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] valueArray = null;
            byte[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<byte>.Shared.Rent(length);
                    span = valueArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        byte* ptr = stackalloc byte[length];
                        span = new Span<byte>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(value, span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
                int length = firstEscapeIndexProp + JsonConstants.MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    propertyArray = ArrayPool<byte>.Shared.Rent(length);
                    span = propertyArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        byte* ptr = stackalloc byte[length];
                        span = new Span<byte>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);
                propertyName = span.Slice(0, written);
            }

            WriteStringByOptions(propertyName, value);

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }

            if (propertyArray != null)
            {
                ArrayPool<byte>.Shared.Return(propertyArray);
            }
        }

        private void WriteStringEscapePropertyOrValue(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] valueArray = null;
            char[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<byte>.Shared.Rent(length);
                    span = valueArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        byte* ptr = stackalloc byte[length];
                        span = new Span<byte>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(value, span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
                int length = firstEscapeIndexProp + JsonConstants.MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
                Span<char> span;
                if (length > StackallocThreshold)
                {
                    propertyArray = ArrayPool<char>.Shared.Rent(length);
                    span = propertyArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        char* ptr = stackalloc char[length];
                        span = new Span<char>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);
                propertyName = span.Slice(0, written);
            }

            WriteStringByOptions(propertyName, value);

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }

            if (propertyArray != null)
            {
                ArrayPool<char>.Shared.Return(propertyArray);
            }
        }

        private void WriteStringEscapePropertyOrValue(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] valueArray = null;
            byte[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<char> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<char>.Shared.Rent(length);
                    span = valueArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        char* ptr = stackalloc char[length];
                        span = new Span<char>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(value, span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
                int length = firstEscapeIndexProp + JsonConstants.MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    propertyArray = ArrayPool<byte>.Shared.Rent(length);
                    span = propertyArray;
                }
                else
                {
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        byte* ptr = stackalloc byte[length];
                        span = new Span<byte>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);
                propertyName = span.Slice(0, written);
            }

            WriteStringByOptions(propertyName, value);

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }

            if (propertyArray != null)
            {
                ArrayPool<byte>.Shared.Return(propertyArray);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                WriteStringIndented(propertyName, value);
            }
            else
            {
                WriteStringMinimized(propertyName, value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                WriteStringIndented(propertyName, value);
            }
            else
            {
                WriteStringMinimized(propertyName, value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                WriteStringIndented(propertyName, value);
            }
            else
            {
                WriteStringMinimized(propertyName, value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                WriteStringIndented(propertyName, value);
            }
            else
            {
                WriteStringMinimized(propertyName, value);
            }
        }

        private void WriteStringMinimized(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<char> escapedPropertyName, ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<byte> escapedPropertyName, ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringValue(ReadOnlySpan<char> escapedValue, ref int idx)
        {
            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
            }
            _buffer[idx++] = JsonConstants.Quote;

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

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
            }
            _buffer[idx++] = JsonConstants.Quote;
        }

        private void WriteStringValue(ReadOnlySpan<byte> escapedValue, ref int idx)
        {
            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
            }
            _buffer[idx++] = JsonConstants.Quote;

            CopyLoop(escapedValue, ref idx);

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
            }
            _buffer[idx++] = JsonConstants.Quote;
        }
    }
}
