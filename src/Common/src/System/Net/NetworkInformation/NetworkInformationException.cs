// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
    /// <devdoc>
    ///    <para>
    ///       Provides NetworkInformation exceptions to the application.
    ///    </para>
    /// </devdoc>
    public class NetworkInformationException : Win32Exception
    {
        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the <see cref='System.Net.NetworkInformation.NetworkInformationException'/> class with the default error code.
        ///    </para>
        /// </devdoc>
        public NetworkInformationException() : base(Marshal.GetLastWin32Error())
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the <see cref='System.Net.NetworkInformation.NetworkInformationException'/> class with the specified error code.
        ///    </para>
        /// </devdoc>
        public NetworkInformationException(int errorCode) : base(errorCode)
        {
        }

        internal NetworkInformationException(SocketError socketError) : base((int)socketError)
        {
        }

        internal NetworkInformationException(string message) : base(message)
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int ErrorCode
        {
            // The base class returns the HResult with this property.
            // We need the Win32 error code, hence the override.
            get
            {
                return NativeErrorCode;
            }
        }
    }
}
