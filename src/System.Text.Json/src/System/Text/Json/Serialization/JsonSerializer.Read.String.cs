﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Parse the text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// </summary>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="json">JSON text to parse.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="json"/> is null.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <remarks>Using a <see cref="string"/> is not as efficient as using the
        /// UTF-8 methods since the implementation natively uses UTF-8.
        /// </remarks>
        public static TValue Deserialize<TValue>(string json, JsonSerializerOptions options = null)
        {
            return (TValue)Deserialize(json, typeof(TValue), options);
        }

        /// <summary>
        /// Parse the text representing a single JSON value into a <paramref name="returnType"/>.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="json">JSON text to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="json"/> or <paramref name="returnType"/> is null.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// the <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <remarks>Using a <see cref="string"/> is not as efficient as using the
        /// UTF-8 methods since the implementation natively uses UTF-8.
        /// </remarks>
        public static object Deserialize(string json, Type returnType, JsonSerializerOptions options = null)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (returnType == null)
            {
                throw new ArgumentNullException(nameof(returnType));
            }

            if (options == null)
            {
                options = JsonSerializerOptions.s_defaultOptions;
            }

            object result;
            byte[] tempArray = null;

            int maxByteCount;
            if (json.Length > JsonConstants.MaxEscapedTokenSize / JsonConstants.MaxExpansionFactorWhileTranscoding)
            {
                // Get the actual byte count in order to handle large input.
                maxByteCount = JsonReaderHelper.GetUtf8ByteCount(json);
            }
            else
            {
                maxByteCount = json.Length * JsonConstants.MaxExpansionFactorWhileTranscoding;
            }

            Span<byte> utf8 = maxByteCount <= JsonConstants.StackallocThreshold ?
                stackalloc byte[maxByteCount] :
                (tempArray = ArrayPool<byte>.Shared.Rent(maxByteCount));

            try
            {
                int actualByteCount = JsonReaderHelper.GetUtf8FromText(json, utf8);
                utf8 = utf8.Slice(0, actualByteCount);

                var readerState = new JsonReaderState(options.GetReaderOptions());
                var reader = new Utf8JsonReader(utf8, isFinalBlock: true, readerState);
                result = ReadCore(returnType, options, ref reader);

                if (reader.BytesConsumed != actualByteCount)
                {
                    ThrowHelper.ThrowJsonException_DeserializeDataRemaining(
                        actualByteCount, actualByteCount - reader.BytesConsumed);
                }
            }
            finally
            {
                if (tempArray != null)
                {
                    utf8.Clear();
                    ArrayPool<byte>.Shared.Return(tempArray);
                }
            }

            return result;
        }
    }
}
