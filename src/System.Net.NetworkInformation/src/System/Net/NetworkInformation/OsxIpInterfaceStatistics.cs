// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class OsxIpInterfaceStatistics : IPInterfaceStatistics
    {
        private readonly long _outputQueueLength;
        private readonly long _inPackets;
        private readonly long _outPackets;
        private readonly long _inBytes;
        private readonly long _outBytes;
        private readonly long _inPacketsDiscarded;
        private readonly long _inErrors;
        private readonly long _inUnknownProtocols;
        private readonly long _inNonUnicastPackets;
        private readonly long _outNonUnicastPackets;
        private readonly long _outErrors;

        // NativeIPInterfaeStatistics is a massive structure, pass it by reference.
        public OsxIpInterfaceStatistics(ref Interop.Sys.NativeIPInterfaceStatistics stats)
        {
            _outputQueueLength = (long)stats.SendQueueLength;
            _inPackets = (long)stats.InPackets;
            _outPackets = (long)stats.OutPackets;
            _inBytes = (long)stats.InBytes;
            _outBytes = (long)stats.OutBytes;
            _inPacketsDiscarded = (long)stats.InDrops;
            _inErrors = (long)stats.InErrors;
            _inUnknownProtocols = (long)stats.InNoProto;
            _inNonUnicastPackets = (long)stats.InMulticastPackets;
            _outNonUnicastPackets = (long)stats.OutMulticastPackets;
            _outErrors = (long)stats.OutErrors;
        }

        public override long BytesReceived
        {
            get
            {
                return _inBytes;
            }
        }

        public override long BytesSent
        {
            get
            {
                return _outBytes;
            }
        }

        public override long IncomingPacketsDiscarded
        {
            get
            {
                return _inPacketsDiscarded;
            }
        }

        public override long IncomingPacketsWithErrors
        {
            get
            {
                return _inErrors;
            }
        }

        public override long IncomingUnknownProtocolPackets
        {
            get
            {
                return _inUnknownProtocols;
            }
        }

        public override long NonUnicastPacketsReceived
        {
            get
            {
                return _inNonUnicastPackets;
            }
        }

        public override long NonUnicastPacketsSent
        {
            get
            {
                return _outNonUnicastPackets;
            }
        }

        public override long OutgoingPacketsDiscarded
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long OutgoingPacketsWithErrors
        {
            get
            {
                return _outErrors;
            }
        }

        public override long OutputQueueLength
        {
            get
            {
                return _outputQueueLength;
            }
        }

        public override long UnicastPacketsReceived
        {
            get
            {
                return _inPackets - _inNonUnicastPackets;
            }
        }

        public override long UnicastPacketsSent
        {
            get
            {
                return _outPackets - _outNonUnicastPackets;
            }
        }
    }
}
