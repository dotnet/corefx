// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Specifies the mode for polling the status of a socket.
    ///    </para>
    /// </devdoc>
    public enum SelectMode
    {
        /// <devdoc>
        ///    <para>
        ///       Poll the read status of a socket.
        ///    </para>
        /// </devdoc>
        SelectRead = 0,
        /// <devdoc>
        ///    <para>
        ///       Poll the write status of a socket.
        ///    </para>
        /// </devdoc>
        SelectWrite = 1,
        /// <devdoc>
        ///    <para>
        ///       Poll the error status of a socket.
        ///    </para>
        /// </devdoc>
        SelectError = 2
    } // enum SelectMode
} // namespace System.Net.Sockets
