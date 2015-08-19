// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
/*++
Copyright (c) 2003 Microsoft Corporation

Module Name:

    AuthenticatedStream.cs

Abstract:

    A public contact for a base abstract authenticated stream.

Author:
    Alexei Vopilov    Sept 28-2003

Revision History:

--*/

using System;
using System.IO;
using System.Threading;
using System.Security.Principal;

namespace System.Net.Security
{
    public abstract class AuthenticatedStream : Stream
    {
        private Stream _InnerStream;
        private bool _LeaveStreamOpen;

        protected AuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen)
        {
            if (innerStream == null || innerStream == Stream.Null)
            {
                throw new ArgumentNullException("innerStream");
            }

            if (!innerStream.CanRead || !innerStream.CanWrite)
            {
                throw new ArgumentException(SR.net_io_must_be_rw_stream, "innerStream");
            }

            _InnerStream = innerStream;
            _LeaveStreamOpen = leaveInnerStreamOpen;
        }

        public bool LeaveInnerStreamOpen
        {
            get
            {
                return _LeaveStreamOpen;
            }
        }
        //
        //
        protected Stream InnerStream
        {
            get
            {
                return _InnerStream;
            }
        }
        //
        //
        protected override void Dispose(bool disposing)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User))
            {
#endif
            try
            {
                if (disposing)
                {
                    if (_LeaveStreamOpen)
                    {
                        _InnerStream.Flush();
                    }
                    else
                    {
                        _InnerStream.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
#if DEBUG
            }
#endif
        }

        //
        // General informational properties
        //
        public abstract bool IsAuthenticated { get; }
        public abstract bool IsMutuallyAuthenticated { get; }
        public abstract bool IsEncrypted { get; }
        public abstract bool IsSigned { get; }
        public abstract bool IsServer { get; }
    }
}



