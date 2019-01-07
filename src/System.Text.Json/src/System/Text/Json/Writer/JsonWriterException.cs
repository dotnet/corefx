// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// Defines a custom exception object that is thrown by the <see cref="Utf8JsonWriter"/> whenever it
    /// tries to write invalid JSON text. This exception is also thrown whenever
    /// you write past the pre-set maximum depth or if the you try to write invalid UTF-8 text.
    /// </summary>
    //TODO: Add a test? Do we need to override GetObjectData in this case?
    [Serializable]
    public sealed class JsonWriterException : Exception
    {
        /// <summary>
        /// Creates a new exception object to relay error information to the user.
        /// </summary>
        /// <param name="message">The context specific error message.</param>
        public JsonWriterException(string message) : base(message)
        {
        }

        private JsonWriterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
