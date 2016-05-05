// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Resources
{
    /// <summary>
    /// Enumerates the resources in a binary resources (.resources) file by reading sequential resource
    /// name/value pairs.Security Note: Calling methods in this class with untrusted data is a security
    /// risk. Call the methods in the class only with trusted data. For more information, see Untrusted Data
    /// Security Risks.
    /// </summary>
    public sealed partial class ResourceReader : System.IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReader" /> class for
        /// the specified stream.
        /// </summary>
        /// <param name="stream">The input stream for reading resources.</param>
        /// <exception cref="ArgumentException">The <paramref name="stream" /> parameter is not readable.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> parameter is null.</exception>
        /// <exception cref="IO.IOException">
        /// An I/O error has occurred while accessing <paramref name="stream" />.
        /// </exception>
        [System.Security.SecurityCriticalAttribute]
        public ResourceReader(System.IO.Stream stream) { }
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="ResourceReader" />
        /// class.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Returns an enumerator for this <see cref="ResourceReader" /> object.
        /// </summary>
        /// <returns>
        /// An enumerator for this <see cref="ResourceReader" /> object.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The reader has been closed or disposed, and cannot be accessed.
        /// </exception>
        public System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
    }
}
