// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

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

    // SecPkgContext_ApplicationProtocol
    [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
    internal class ApplicationProtocolContext
    {
        private const int MAX_PROTOCOL_ID_SIZE = 0xFF;
        public ApplicationProtocolNegotiationStatus NegotiationStatus;
        public ApplicationProtocolNegotiationExt NegotiationExtension;
        public byte ProtocolIdSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_PROTOCOL_ID_SIZE)]
        public char[] ProtocolId;

        public string GetProtocolId()
        {
            return new string(ProtocolId, 0, ProtocolIdSize);
        }
    }
}