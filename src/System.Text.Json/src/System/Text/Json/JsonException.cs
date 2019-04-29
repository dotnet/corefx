// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// Defines a custom exception object that is thrown when invalid JSON text is encountered, when the defined maximum depth is passed,
    /// or the JSON text is not compatible with the type of a property on an object.
    /// </summary>
    [Serializable]
    public abstract class JsonException : Exception
    {
        /// <summary>
        /// Creates a new exception object to relay error information to the user.
        /// </summary>
        /// <param name="message">The context specific error message.</param>
        public JsonException(string message) : base(message) { }

        /// <summary>
        /// Creates a new exception object to relay error information to the user.
        /// </summary>
        /// <param name="innerException">The exception that caused the current exception.</param>
        /// <param name="message">The context specific error message.</param>
        public JsonException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new exception object with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds
        /// the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that
        /// contains contextual information about the source or destination. </param>
        protected JsonException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        ///  Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context) { }
    }
}
