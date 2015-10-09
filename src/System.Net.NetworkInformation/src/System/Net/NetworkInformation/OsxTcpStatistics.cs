// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class OsxTcpStatistics : TcpStatistics
    {
        private readonly long _connectionsAccepted;
        private readonly long _connectionsInitiated;
        private readonly long _currentConnections;
        private readonly long _errorsReceived;
        private readonly long _failedConnectionAttempts;
        private readonly long _segmentsSent;
        private readonly long _segmentsResent;
        private readonly long _segmentsReceived;

        public OsxTcpStatistics()
        {
            Interop.Sys.TcpGlobalStatistics statistics;
            if (Interop.Sys.GetTcpGlobalStatistics(out statistics) != 0)
            {
                throw new NetworkInformationException((int)Interop.Sys.GetLastError());
            }

            _connectionsAccepted = (long)statistics.ConnectionsAccepted;
            _connectionsInitiated = (long)statistics.ConnectionsInitiated;
            _errorsReceived = (long)statistics.ErrorsReceived;
            _failedConnectionAttempts = (long)statistics.FailedConnectionAttempts;
            _segmentsSent = (long)statistics.SegmentsSent;
            _segmentsResent = (long)statistics.SegmentsResent;
            _segmentsReceived = (long)statistics.SegmentsReceived;

            // TODO: This should be obtainable.
            _currentConnections = 0;
        }

        public override long ConnectionsAccepted
        {
            get
            {
                return _connectionsAccepted;
            }
        }

        public override long ConnectionsInitiated
        {
            get
            {
                return _connectionsInitiated;
            }
        }

        public override long CumulativeConnections
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long CurrentConnections
        {
            get
            {
                return _currentConnections;
            }
        }

        public override long ErrorsReceived
        {
            get
            {
                return _errorsReceived;
            }
        }

        public override long FailedConnectionAttempts
        {
            get
            {
                return _failedConnectionAttempts;
            }
        }

        public override long MaximumConnections
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long MaximumTransmissionTimeout
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long MinimumTransmissionTimeout
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long ResetConnections
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long ResetsSent
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override long SegmentsReceived
        {
            get
            {
                return _segmentsReceived;
            }
        }

        public override long SegmentsResent
        {
            get
            {
                return _segmentsResent;
            }
        }

        public override long SegmentsSent
        {
            get
            {
                return _segmentsSent;
            }
        }
    }
}