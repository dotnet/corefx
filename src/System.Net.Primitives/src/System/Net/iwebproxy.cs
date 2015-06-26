// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    /// <devdoc>
    ///    <para>
    ///       Holds the interface for implementation of the proxy interface.
    ///       Used to implement and control proxy use of WebRequests. 
    ///    </para>
    /// </devdoc>
    public interface IWebProxy
    {
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>

        Uri GetProxy(Uri destination);
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        bool IsBypassed(Uri host);
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ICredentials Credentials { get; set; }
    }
}
