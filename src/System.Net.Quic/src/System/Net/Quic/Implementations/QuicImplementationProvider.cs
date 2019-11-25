// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;

namespace System.Net.Quic.Implementations
{
    public abstract class QuicImplementationProvider
    {
        internal QuicImplementationProvider() { }

        internal abstract QuicListenerProvider CreateListener(IPEndPoint listenEndPoint, SslServerAuthenticationOptions sslServerAuthenticationOptions);

        internal abstract QuicConnectionProvider CreateConnection(IPEndPoint remoteEndPoint, SslClientAuthenticationOptions sslClientAuthenticationOptions, IPEndPoint localEndPoint);
    }
}
