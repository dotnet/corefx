// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Read the UTF-8 encoded JSON and return the converted value.
        /// </summary>
        /// <returns>The converted value.</returns>
        /// <param name="utf8Json">The UTF-8 encoded JSON"/>.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used to read and convert the JSON.</param>
        /// <exception cref="JsonReaderException">
        /// Thrown when the JSON is invalid or when <typeparamref name="TValue"/> is not compatible with the JSON.
        /// </exception>
        public static TValue Parse<TValue>(ReadOnlySpan<byte> utf8Json, JsonSerializerOptions options = null)
        {
            return (TValue)ParseCore(utf8Json, typeof(TValue), options);
        }

        /// <summary>
        /// Read the UTF-8 encoded JSON and return the converted value.
        /// </summary>
        /// <returns>The converted value.</returns>
        /// <param name="utf8Json">The UTF-8 encoded JSON.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used to read and convert the JSON.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="returnType"/> is null.
        /// </exception>
        /// <exception cref="JsonReaderException">
        /// Thrown when the JSON is invalid or when <paramref name="returnType"/> is not compatible with the JSON.
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
                options = s_defaultSettings;

            var readerState = new JsonReaderState(options: options.ReaderOptions);
            var reader = new Utf8JsonReader(utf8Json, isFinalBlock: true, readerState);
            object result = ReadCore(returnType, options, ref reader);

            readerState = reader.CurrentState;
            if (readerState.BytesConsumed != utf8Json.Length)
            {
                throw new JsonReaderException(SR.Format(SR.DeserializeDataRemaining,
                    utf8Json.Length, utf8Json.Length - readerState.BytesConsumed), readerState);
            }

            return result;
        }
    }
}
