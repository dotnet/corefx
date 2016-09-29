// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.WebSockets
{
    public enum WebSocketError
    {
        Success = 0,
        InvalidMessageType = 1,
        Faulted = 2,
        NativeError = 3,
        NotAWebSocket = 4,
        UnsupportedVersion = 5,
        UnsupportedProtocol = 6,
        HeaderError = 7,
        ConnectionClosedPrematurely = 8,
        InvalidState = 9
    }
}
