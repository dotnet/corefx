// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace System.Net.Sockets
{
    public class InternalSocketException : SocketException
    {
        internal InternalSocketException(SocketError errorCode, int platformError)
            : base((int)errorCode)
        {
            HResult = platformError;
        }
    }
}
