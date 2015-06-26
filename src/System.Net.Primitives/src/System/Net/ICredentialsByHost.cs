// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace System.Net
{
    //
    // This is an extensible interface that authenticators
    // must implement to support credential lookup.
    // During execution of the protocol, if authentication
    // information is needed the GetCredential function will
    // be called with the host and realm information.
    //

    /// <devdoc>
    ///    <para>Provides the base authentication interface for Web client authentication.</para>
    /// </devdoc>
    public interface ICredentialsByHost
    {
        /// <devdoc>
        ///    <para>
        ///       Returns a NetworkCredential object that
        ///       is associated with the supplied host, realm, and authentication type.
        ///    </para>
        /// </devdoc>

        //
        // CONVENTION:
        // returns null if no information is available
        // for the specified host&realm
        //
        NetworkCredential GetCredential(string host, int port, string authenticationType);
    } // interface ICredentials
} // namespace System.Net
