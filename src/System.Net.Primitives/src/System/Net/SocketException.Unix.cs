// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Net.Sockets
{
    public partial class SocketException : Win32Exception
    {
        /// <summary>Creates a new instance of the <see cref='System.Net.Sockets.SocketException'/> class with the default error code.</summary>
        public SocketException() : this(Interop.Sys.GetLastErrorInfo())
        {
        }

        internal SocketException(SocketError errorCode, uint platformError) : base((int)platformError)
        {
            _errorCode = errorCode;

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print($"SocketException::.ctor(SocketError={errorCode}, uint={platformError}):{Message}");
            }
        }

        private SocketException(Interop.ErrorInfo error) : this(SocketErrorPal.GetSocketErrorForNativeError(error.Error), (uint)error.RawErrno)
        {
        }

        private static int GetNativeErrorForSocketError(SocketError error)
        {
            return error != SocketError.SocketError ?
                SocketErrorPal.GetNativeErrorForSocketError(error).Info().RawErrno :
                (int)error;
        }
    }
}
