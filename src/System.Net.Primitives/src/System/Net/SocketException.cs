// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Provides socket exceptions to the application.
    ///    </para>
    /// </devdoc>
    public class SocketException : Win32Exception
    {
        private EndPoint m_EndPoint;

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the <see cref='System.Net.Sockets.SocketException'/> class with the default error code.
        ///    </para>
        /// </devdoc>
        public SocketException() : base(Marshal.GetLastWin32Error())
        {
            GlobalLog.Print("SocketException::.ctor() " + NativeErrorCode.ToString() + ":" + Message);
        }

        internal SocketException(EndPoint endPoint) : base(Marshal.GetLastWin32Error())
        {
            m_EndPoint = endPoint;
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the <see cref='System.Net.Sockets.SocketException'/> class with the specified error code.
        ///    </para>
        /// </devdoc>
        public SocketException(int errorCode) : base(errorCode)
        {
            GlobalLog.Print("SocketException::.ctor(int) " + NativeErrorCode.ToString() + ":" + Message);
        }

        internal SocketException(int errorCode, EndPoint endPoint) : base(errorCode)
        {
            m_EndPoint = endPoint;
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the <see cref='System.Net.Sockets.SocketException'/> class with the specified error code as SocketError.
        ///    </para>
        /// </devdoc>
        internal SocketException(SocketError socketError) : base((int)socketError)
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int ErrorCode
        {
            //
            // the base class returns the HResult with this property
            // we need the Win32 Error Code, hence the override.
            //
            get
            {
                return NativeErrorCode;
            }
        }

        public override string Message
        {
            get
            {
                // If not null add EndPoint.ToString() to end of base Message
                if (m_EndPoint == null)
                {
                    return base.Message;
                }
                else
                {
                    return base.Message + " " + m_EndPoint.ToString();
                }
            }
        }


        public SocketError SocketErrorCode
        {
            //
            // the base class returns the HResult with this property
            // we need the Win32 Error Code, hence the override.
            //
            get
            {
                return (SocketError)NativeErrorCode;
            }
        }
    }; // class SocketException
} // namespace System.Net
