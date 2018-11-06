// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    /// Defines a custom exception object that is thrown by the <see cref="Utf8JsonReader"/> whenever it
    /// encounters an invalid JSON text while reading through it. This exception is also thrown
    /// whenever you read past the defined maximum depth.
    /// </summary>
    public sealed class JsonReaderException : Exception
    {
        /// <summary>
        /// Creates a new exception object to relay error information to the user.
        /// </summary>
        /// <param name="message">The context specific error message.</param>
        /// <param name="lineNumber">The line number at which the invalid JSON was encountered (starting at 0).</param>
        /// <param name="bytePositionInLine">The byte count within the current line where the invalid JSON was encountered (starting at 0).</param>
        /// <remarks>
        /// Note that the <paramref name="bytePositionInLine"/> counts the number of bytes (i.e. UTF-8 code units) and not characters or scalars.
        /// </remarks>
        public JsonReaderException(string message, long lineNumber, long bytePositionInLine) : base(message)
        {
            LineNumber = lineNumber;
            BytePositionInLine = bytePositionInLine;
        }

        /// <summary>
        /// The number of lines read so far before the exception (starting at 0).
        /// </summary>
        public long LineNumber { get; }

        /// <summary>
        /// The number of bytes read within the current line before the exception (starting at 0).
        /// </summary>
        public long BytePositionInLine { get; }
    }
}
