// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Read the UTF-16 encoded JSON <see cref="System.String"/> and return the converted value.
        /// </summary>
        /// <returns>The converted value.</returns>
        /// <param name="json">The UTF-16 encoded <see cref="System.String"/>.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used to read and convert the JSON.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="json"/> is null.
        /// </exception>
        /// <exception cref="JsonReaderException">
        /// Thrown when the JSON is invalid or when <typeparamref name="TValue"/> is not compatible with the JSON.
        /// </exception>
        /// <remarks>Using a UTF-16 <see cref="System.String"/> is not as efficient as using the
        /// UTF-8 methods since the implementation natively uses UTF-8.
        /// </remarks>
        public static TValue Parse<TValue>(string json, JsonSerializerOptions options = null)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            return (TValue)ParseCore(json, typeof(TValue), options);
        }

        /// <summary>
        /// Read the UTF-16 encoded JSON <see cref="System.String"/> and return the converted value.
        /// </summary>
        /// <returns>The converted value.</returns>
        /// <param name="json">The UTF-16 encoded <see cref="System.String"/>.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used to read and convert the JSON.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="json"/> or <paramref name="returnType"/> is null.
        /// </exception>
        /// <exception cref="JsonReaderException">
        /// Thrown when the JSON is invalid or when <paramref name="returnType"/> is not compatible with the JSON.
        /// </exception>
        /// <remarks>Using a UTF-16 <see cref="System.String"/> is not as efficient as using the
        /// UTF-8 methods since the implementation natively uses UTF-8.
        /// </remarks>
        public static object Parse(string json, Type returnType, JsonSerializerOptions options = null)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            return ParseCore(json, returnType, options);
        }

        private static object ParseCore(string json, Type returnType, JsonSerializerOptions options = null)
        {
            if (options == null)
                options = s_defaultSettings;

            // todo: use an array pool here for smaller requests to avoid the alloc. Also doc the API that UTF8 is preferred for perf. 
            byte[] jsonBytes = JsonReaderHelper.s_utf8Encoding.GetBytes(json);
            var readerState = new JsonReaderState(options: options.ReaderOptions);
            var reader = new Utf8JsonReader(jsonBytes, isFinalBlock: true, readerState);
            object result = ReadCore(returnType, options, ref reader);

            readerState = reader.CurrentState;
            if (readerState.BytesConsumed != jsonBytes.Length)
            {
                throw new JsonReaderException(SR.Format(SR.DeserializeDataRemaining,
                    jsonBytes.Length, jsonBytes.Length - readerState.BytesConsumed), readerState);
            }

            return result;
        }
    }
}
