// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ComponentModel
{
    /// <summary>
    /// Throws an exception for a Win32 error code.
    /// </summary>
    public partial class Win32Exception : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Exception" /> class
        /// with the last Win32 error that occurred.
        /// </summary>
        public Win32Exception() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Exception" /> class
        /// with the specified error.
        /// </summary>
        /// <param name="error">The Win32 error code associated with this exception.</param>
        public Win32Exception(int error) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Exception" /> class
        /// with the specified error and the specified detailed description.
        /// </summary>
        /// <param name="error">The Win32 error code associated with this exception.</param>
        /// <param name="message">A detailed description of the error.</param>
        public Win32Exception(int error, string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Exception" /> class
        /// with the specified detailed description.
        /// </summary>
        /// <param name="message">A detailed description of the error.</param>
        public Win32Exception(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Exception" /> class
        /// with the specified detailed description and the specified exception.
        /// </summary>
        /// <param name="message">A detailed description of the error.</param>
        /// <param name="innerException">A reference to the inner exception that is the cause of this exception.</param>
        public Win32Exception(string message, System.Exception innerException) { }
        /// <summary>
        /// Gets the Win32 error code associated with this exception.
        /// </summary>
        /// <returns>
        /// The Win32 error code associated with this exception.
        /// </returns>
        public int NativeErrorCode { get { return default(int); } }
    }
}
