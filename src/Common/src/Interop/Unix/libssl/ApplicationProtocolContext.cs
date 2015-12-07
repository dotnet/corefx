// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    internal enum ApplicationProtocolNegotiationStatus
    {
        None = 0,
        Success,
        SelectedClientOnly
    }

    internal enum ApplicationProtocolNegotiationExt
    {
        None = 0,
        NPN,
        ALPN
    }

    internal class ApplicationProtocolContext
    {
        public ApplicationProtocolNegotiationStatus NegotiationStatus = ApplicationProtocolNegotiationStatus.None;
        public ApplicationProtocolNegotiationExt NegotiationExtension = ApplicationProtocolNegotiationExt.None;

        public string GetProtocolId()
        {
            return null;
        }
    }
}