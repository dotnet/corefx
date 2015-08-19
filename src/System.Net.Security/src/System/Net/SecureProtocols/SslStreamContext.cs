// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net
{
    internal class SslStreamContext : TransportContext
    {
        internal SslStreamContext(SslStream sslStream)
        {
            GlobalLog.Assert(sslStream != null, "SslStreamContext..ctor(): Not expecting a null sslStream!");
            this.sslStream = sslStream;
        }

        public override ChannelBinding GetChannelBinding(ChannelBindingKind kind)
        {
            return sslStream.GetChannelBinding(kind);
        }

        private SslStream sslStream;
    }
}
