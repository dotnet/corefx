// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Parse the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// </summary>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON text to parse.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        public static TValue Parse<TValue>(ReadOnlySpan<byte> utf8Json, JsonSerializerOptions options = null)
        {
            return (TValue)ParseCore(utf8Json, typeof(TValue), options);
        }

        /// <summary>
        /// Parse the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON text to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="returnType"/> is null.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        public static object Parse(ReadOnlySpan<byte> utf8Json, Type returnType, JsonSerializerOptions options = null)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            return ParseCore(utf8Json, returnType, options);
        }

        private static object ParseCore(ReadOnlySpan<byte> utf8Json, Type returnType, JsonSerializerOptions options)
        {
            if (options == null)
            {
                options = JsonSerializerOptions.s_defaultOptions;
            }

            var readerState = new JsonReaderState(options.GetReaderOptions());
            var reader = new Utf8JsonReader(utf8Json, isFinalBlock: true, readerState);
            object result = ReadCore(returnType, options, ref reader);

            if (reader.BytesConsumed != utf8Json.Length)
            {
                readerState = reader.CurrentState;
                ThrowHelper.ThrowJsonException_DeserializeDataRemaining(utf8Json.Length, utf8Json.Length - readerState.BytesConsumed);
            }

            return result;
        }

        /// <summary>
        /// Parse the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// </summary>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="reader">JSON text to parse.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        public static TValue ReadValue<TValue>(ref Utf8JsonReader reader, JsonSerializerOptions options = null)
        {
            return (TValue)ParseValueCore(ref reader, typeof(TValue), options);
        }

        /// <summary>
        /// Parse the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="reader">JSON text to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="returnType"/> is null.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        public static object ReadValue(ref Utf8JsonReader reader, Type returnType, JsonSerializerOptions options = null)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            return ParseValueCore(ref reader, returnType, options);
        }

        private static object ParseValueCore(ref Utf8JsonReader reader, Type returnType, JsonSerializerOptions options)
        {
            if (options == null)
            {
                options = JsonSerializerOptions.s_defaultOptions;
            }

            object result = ReadValueCore(returnType, options, ref reader);

            return result;
        }

        private static object ReadValueCore(
            Type returnType,
            JsonSerializerOptions options,
            ref Utf8JsonReader reader)
        {
            Debug.Assert(options != null);

            ReadStack state = default;
            state.Current.Initialize(returnType, options);

            ReadValueCore(options, ref reader, ref state);

            return state.Current.ReturnValue;
        }

        private static void CheckSupportedOptions(JsonReaderOptions readerOptions, string paramName)
        {
            if (readerOptions.CommentHandling == JsonCommentHandling.Allow)
            {
                throw new ArgumentException(SR.JsonSerializerDoesNotSupportComments, paramName);
            }
        }

        private static void ReadValueCore(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            JsonReaderState readerState = reader.CurrentState;
            CheckSupportedOptions(readerState.Options, nameof(reader));

            // Value copy to overwrite the ref on an exception and undo the destructive reads.
            Utf8JsonReader restore = reader;

            ReadOnlySpan<byte> valueSpan = default;
            ReadOnlySequence<byte> valueSequence = default;

            try
            {
                switch (reader.TokenType)
                {
                    // A new reader was created and has never been read,
                    // so we need to move to the first token.
                    // (or a reader has terminated and we're about to throw)
                    case JsonTokenType.None:
                    // Using a reader loop the caller has identified a property they wish to
                    // hydrate into a JsonDocument. Move to the value first.
                    case JsonTokenType.PropertyName:
                        {
                            if (!reader.Read())
                            {
                                ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedJsonTokens);
                            }
                            break;
                        }
                }

                switch (reader.TokenType)
                {
                    // Any of the "value start" states are acceptable.
                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        {
                            long startingOffset = reader.TokenStartIndex;

                            if (!reader.TrySkip())
                            {
                                ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedJsonTokens);
                            }

                            long totalLength = reader.BytesConsumed - startingOffset;
                            ReadOnlySequence<byte> sequence = reader.OriginalSequence;

                            if (sequence.IsEmpty)
                            {
                                valueSpan = reader.OriginalSpan.Slice(
                                    checked((int)startingOffset),
                                    checked((int)totalLength));
                            }
                            else
                            {
                                valueSequence = sequence.Slice(startingOffset, totalLength);
                            }

                            Debug.Assert(
                                reader.TokenType == JsonTokenType.EndObject ||
                                reader.TokenType == JsonTokenType.EndArray);

                            break;
                        }

                    // Single-token values
                    case JsonTokenType.Number:
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                    case JsonTokenType.Null:
                        {
                            if (reader.HasValueSequence)
                            {
                                valueSequence = reader.ValueSequence;
                            }
                            else
                            {
                                valueSpan = reader.ValueSpan;
                            }

                            break;
                        }
                    // String's ValueSequence/ValueSpan omits the quotes, we need them back.
                    case JsonTokenType.String:
                        {
                            ReadOnlySequence<byte> sequence = reader.OriginalSequence;

                            if (sequence.IsEmpty)
                            {
                                // Since the quoted string fit in a ReadOnlySpan originally
                                // the contents length plus the two quotes can't overflow.
                                int payloadLength = reader.ValueSpan.Length + 2;
                                Debug.Assert(payloadLength > 1);

                                ReadOnlySpan<byte> readerSpan = reader.OriginalSpan;

                                Debug.Assert(
                                    readerSpan[(int)reader.TokenStartIndex] == (byte)'"',
                                    $"Calculated span starts with {readerSpan[(int)reader.TokenStartIndex]}");

                                Debug.Assert(
                                    readerSpan[(int)reader.TokenStartIndex + payloadLength - 1] == (byte)'"',
                                    $"Calculated span ends with {readerSpan[(int)reader.TokenStartIndex + payloadLength - 1]}");

                                valueSpan = readerSpan.Slice((int)reader.TokenStartIndex, payloadLength);
                            }
                            else
                            {
                                long payloadLength = 2;

                                if (reader.HasValueSequence)
                                {
                                    payloadLength += reader.ValueSequence.Length;
                                }
                                else
                                {
                                    payloadLength += reader.ValueSpan.Length;
                                }

                                valueSequence = sequence.Slice(reader.TokenStartIndex, payloadLength);
                                Debug.Assert(
                                    valueSequence.First.Span[0] == (byte)'"',
                                    $"Calculated sequence starts with {valueSequence.First.Span[0]}");

                                Debug.Assert(
                                    valueSequence.ToArray()[payloadLength - 1] == (byte)'"',
                                    $"Calculated sequence ends with {valueSequence.ToArray()[payloadLength - 1]}");
                            }

                            break;
                        }
                    default:
                        {
                            byte displayByte;

                            if (reader.HasValueSequence)
                            {
                                displayByte = reader.ValueSequence.First.Span[0];
                            }
                            else
                            {
                                displayByte = reader.ValueSpan[0];
                            }

                            ThrowHelper.ThrowJsonReaderException(
                                ref reader,
                                ExceptionResource.ExpectedStartOfValueNotFound,
                                displayByte);

                            break;
                        }
                }
            }
            catch (JsonReaderException e)
            {
                reader = restore;
                // Re-throw with Path information.
                ThrowHelper.ReThrowWithPath(e, state.PropertyPath);
            }

            int length = valueSpan.IsEmpty ? checked((int)valueSequence.Length) : valueSpan.Length;
            byte[] rented = ArrayPool<byte>.Shared.Rent(length);
            Span<byte> rentedSpan = rented.AsSpan(0, length);

            try
            {
                if (valueSpan.IsEmpty)
                {
                    valueSequence.CopyTo(rentedSpan);
                }
                else
                {
                    valueSpan.CopyTo(rentedSpan);
                }

                var newReader = new Utf8JsonReader(rentedSpan, isFinalBlock: true, state: default);
                ReadCore(options, ref newReader, ref state);

                if (newReader.BytesConsumed != length)
                {
                    readerState = newReader.CurrentState;
                    ThrowHelper.ThrowJsonException_DeserializeDataRemaining(length, length - readerState.BytesConsumed);
                }
            }
            catch
            {
                // This really shouldn't happen since the document was already checked
                // for consistency by Skip.  But if data mutations happened just after
                // the calls to Read then the copy may not be valid.
                rentedSpan.Clear();
                ArrayPool<byte>.Shared.Return(rented);
                throw;
            }
        }
    }
}
