

using System;
using System.Net;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// <copyright file="SocketErrors.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Defines socket error constants.
    ///    </para>
    /// </devdoc>


    public enum SocketError : int
    {
        /// <devdoc>
        ///    <para>
        ///       The operation completed successfully.
        ///    </para>
        /// </devdoc>
        Success = 0,

        /// <devdoc>
        ///    <para>
        ///       The socket has an error.
        ///    </para>
        /// </devdoc>
        SocketError = (-1),


        /*
         * All Windows Sockets error constants are biased by WSABASEERR from
         * the "normal"
         */
        /// <devdoc>
        ///    <para>
        ///       The base value of all socket error constants. All other socket errors are
        ///       offset from this value.
        ///    </para>
        /// </devdoc>


        ///WSABASEERR = 10000;

        /*
         * Windows Sockets definitions of regular Microsoft C error constants
         */
        /// <devdoc>
        ///    <para>
        ///       A blocking socket call was canceled.
        ///    </para>
        /// </devdoc>
        Interrupted = (10000 + 4),      //WSAEINTR
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
        //WSAEBADF               = (10000+9),   //
        /// <devdoc>
        ///    <para>
        ///       Permission denied.
        ///    </para>
        /// </devdoc>
        AccessDenied = (10000 + 13),      //WSAEACCES
        /// <devdoc>
        ///    <para>
        ///       Bad address.
        ///    </para>
        /// </devdoc>
        Fault = (10000 + 14),        //WSAEFAULT
        /// <devdoc>
        ///    <para>
        ///       Invalid argument.
        ///    </para>
        /// </devdoc>
        InvalidArgument = (10000 + 22),    //WSAEINVAL
        /// <devdoc>
        ///    <para>
        ///       Too many open
        ///       files.
        ///    </para>
        /// </devdoc>
        TooManyOpenSockets = (10000 + 24),  //WSAEMFILE


        /*
         * Windows Sockets definitions of regular Berkeley error constants
         */
        /// <devdoc>
        ///    <para>
        ///       Resource temporarily unavailable.
        ///    </para>
        /// </devdoc>
        WouldBlock = (10000 + 35),   //WSAEWOULDBLOCK
        /// <devdoc>
        ///    <para>
        ///       Operation now in progress.
        ///    </para>
        /// </devdoc>
        InProgress = (10000 + 36),  // WSAEINPROGRESS
        /// <devdoc>
        ///    <para>
        ///       Operation already in progress.
        ///    </para>
        /// </devdoc>
        AlreadyInProgress = (10000 + 37),  //WSAEALREADY
        /// <devdoc>
        ///    <para>
        ///       Socket operation on nonsocket.
        ///    </para>
        /// </devdoc>
        NotSocket = (10000 + 38),   //WSAENOTSOCK
        /// <devdoc>
        ///    <para>
        ///       Destination address required.
        ///    </para>
        /// </devdoc>
        DestinationAddressRequired = (10000 + 39), //WSAEDESTADDRREQ
        /// <devdoc>
        ///    <para>
        ///       Message too long.
        ///    </para>
        /// </devdoc>
        MessageSize = (10000 + 40),  //WSAEMSGSIZE
        /// <devdoc>
        ///    <para>
        ///       Protocol wrong type for socket.
        ///    </para>
        /// </devdoc>
        ProtocolType = (10000 + 41), //WSAEPROTOTYPE
        /// <devdoc>
        ///    <para>
        ///       Bad protocol option.
        ///    </para>
        /// </devdoc>
        ProtocolOption = (10000 + 42), //WSAENOPROTOOPT
        /// <devdoc>
        ///    <para>
        ///       Protocol not supported.
        ///    </para>
        /// </devdoc>
        ProtocolNotSupported = (10000 + 43), //WSAEPROTONOSUPPORT
        /// <devdoc>
        ///    <para>
        ///       Socket type not supported.
        ///    </para>
        /// </devdoc>
        SocketNotSupported = (10000 + 44), //WSAESOCKTNOSUPPORT
        /// <devdoc>
        ///    <para>
        ///       Operation not supported.
        ///    </para>
        /// </devdoc>
        OperationNotSupported = (10000 + 45), //WSAEOPNOTSUPP
        /// <devdoc>
        ///    <para>
        ///       Protocol family not supported.
        ///    </para>
        /// </devdoc>
        ProtocolFamilyNotSupported = (10000 + 46), //WSAEPFNOSUPPORT
        /// <devdoc>
        ///    <para>
        ///       Address family not supported by protocol family.
        ///    </para>
        /// </devdoc>
        AddressFamilyNotSupported = (10000 + 47), //WSAEAFNOSUPPORT
        /// <devdoc>
        ///    Address already in use.
        /// </devdoc>
        AddressAlreadyInUse = (10000 + 48), // WSAEADDRINUSE
        /// <devdoc>
        ///    <para>
        ///       Cannot assign requested address.
        ///    </para>
        /// </devdoc>
        AddressNotAvailable = (10000 + 49), //WSAEADDRNOTAVAIL
        /// <devdoc>
        ///    <para>
        ///       Network is down.
        ///    </para>
        /// </devdoc>
        NetworkDown = (10000 + 50), //WSAENETDOWN
        /// <devdoc>
        ///    <para>
        ///       Network is unreachable.
        ///    </para>
        /// </devdoc>
        NetworkUnreachable = (10000 + 51), //WSAENETUNREACH
        /// <devdoc>
        ///    <para>
        ///       Network dropped connection on reset.
        ///    </para>
        /// </devdoc>
        NetworkReset = (10000 + 52), //WSAENETRESET
        /// <devdoc>
        ///    <para>
        ///       Software caused connection to abort.
        ///    </para>
        /// </devdoc>
        ConnectionAborted = (10000 + 53), //WSAECONNABORTED
        /// <devdoc>
        ///    <para>
        ///       Connection reset by peer.
        ///    </para>
        /// </devdoc>
        ConnectionReset = (10000 + 54), //WSAECONNRESET
        /// <devdoc>
        ///    No buffer space available.
        /// </devdoc>
        NoBufferSpaceAvailable = (10000 + 55), //WSAENOBUFS
        /// <devdoc>
        ///    <para>
        ///       Socket is already connected.
        ///    </para>
        /// </devdoc>
        IsConnected = (10000 + 56), //WSAEISCONN
        /// <devdoc>
        ///    <para>
        ///       Socket is not connected.
        ///    </para>
        /// </devdoc>
        NotConnected = (10000 + 57), //WSAENOTCONN
        /// <devdoc>
        ///    <para>
        ///       Cannot send after socket shutdown.
        ///    </para>
        /// </devdoc>
        Shutdown = (10000 + 58), //WSAESHUTDOWN
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
     //   WSAETOOMANYREFS        = (10000+59), //WSAETOOMANYREFS
        /// <devdoc>
        ///    <para>
        ///       Connection timed out.
        ///    </para>
        /// </devdoc>
        TimedOut = (10000 + 60), //WSAETIMEDOUT
        /// <devdoc>
        ///    <para>
        ///       Connection refused.
        ///    </para>
        /// </devdoc>
        ConnectionRefused = (10000 + 61), //WSAECONNREFUSED
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
      //  WSAELOOP               = (10000+62),
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
     //   WSAENAMETOOLONG        = (10000+63),
        /// <devdoc>
        ///    <para>
        ///       Host is down.
        ///    </para>
        /// </devdoc>
        HostDown = (10000 + 64), //WSAEHOSTDOWN
        /// <devdoc>
        ///    <para>
        ///       No route to host.
        ///    </para>
        /// </devdoc>
        HostUnreachable = (10000 + 65), //WSAEHOSTUNREACH
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
       // WSAENOTEMPTY           = (10000+66),
        /// <devdoc>
        ///    <para>
        ///       Too many processes.
        ///    </para>
        /// </devdoc>
        ProcessLimit = (10000 + 67), //WSAEPROCLIM
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
      //  WSAEUSERS              = (10000+68),
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
      //  WSAEDQUOT              = (10000+69),
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
       // WSAESTALE              = (10000+70),
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
        /*
         * Extended Windows Sockets error constant definitions
         */
        /// <devdoc>
        ///    <para>
        ///       Network subsystem is unavailable.
        ///    </para>
        /// </devdoc>
        SystemNotReady = (10000 + 91), //WSASYSNOTREADY
        /// <devdoc>
        ///    <para>
        ///       Winsock.dll out of range.
        ///    </para>
        /// </devdoc>
        VersionNotSupported = (10000 + 92), //WSAVERNOTSUPPORTED
        /// <devdoc>
        ///    <para>
        ///       Successful startup not yet performed.
        ///    </para>
        /// </devdoc>
        NotInitialized = (10000 + 93), //WSANOTINITIALISED


        // WSAEREMOTE             = (10000+71),
        /// <devdoc>
        ///    <para>
        ///       Graceful shutdown in progress.
        ///    </para>
        /// </devdoc>
        Disconnecting = (10000 + 101), //WSAEDISCON


        TypeNotFound = (10000 + 109), //WSATYPE_NOT_FOUND


        /*
         * Error return codes from gethostbyname() and gethostbyaddr()
         *              = (when using the resolver). Note that these errors are
         * retrieved via WSAGetLastError() and must therefore follow
         * the rules for avoiding clashes with error numbers from
         * specific implementations or language run-time systems.
         * For this reason the codes are based at 10000+1001.
         * Note also that [WSA]NO_ADDRESS is defined only for
         * compatibility purposes.
         */



        /// <devdoc>
        ///    <para>
        ///       Host not found (Authoritative Answer: Host not found).
        ///    </para>
        /// </devdoc>
        HostNotFound = (10000 + 1001), //WSAHOST_NOT_FOUND
        /// <devdoc>
        ///    <para>
        ///       Nonauthoritative host not found (Non-Authoritative: Host not found, or SERVERFAIL).
        ///    </para>
        /// </devdoc>
        TryAgain = (10000 + 1002), //WSATRY_AGAIN
        /// <devdoc>
        ///    <para>
        ///       This is a nonrecoverable error (Non recoverable errors, FORMERR, REFUSED, NOTIMP).
        ///    </para>
        /// </devdoc>
        NoRecovery = (10000 + 1003), //WSANO_RECOVERY
        /// <devdoc>
        ///    <para>
        ///       Valid name, no data record of requested type.
        ///    </para>
        /// </devdoc>
        NoData = (10000 + 1004), //WSANO_DATA



        //OS dependent errors

        /// <devdoc>
        ///    <para>
        ///       Overlapped operations will complete later.
        ///    </para>
        /// </devdoc>
        IOPending = (int)UnsafeCommonNativeMethods.ErrorCodes.ERROR_IO_PENDING,          // 997
        /// <devdoc>
        ///    <para>
        ///       [To be supplied.]
        ///    </para>
        /// </devdoc>
        OperationAborted = (int)UnsafeCommonNativeMethods.ErrorCodes.ERROR_OPERATION_ABORTED,   // 995, WSA_OPERATION_ABORTED
    }
}
