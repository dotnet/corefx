// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxTcpStatistics : TcpStatistics
    {
        private int _rtoAlgorithm;
        private int _rtoMin;
        private int _rtoMax;
        private int _maxConn;
        private int _activeOpens;
        private int _passiveOpens;
        private int _attemptFails;
        private int _estabResets;
        private int _currEstab;
        private int _inSegs;
        private int _outSegs;
        private int _retransSegs;
        private int _inErrs;
        private int _outRsts;
        private int _inCsumErrors;

        private int _currentConnections;

        private LinuxTcpStatistics() { }

        internal static TcpStatistics CreateTcpStatistics(bool ipv6)
        {
            LinuxTcpStatistics stats = new LinuxTcpStatistics();
            // NOTE: There is no information in the snmp6 file regarding TCP statistics.
            // Is /proc/net/snmp correct for IPv6?
            string fileContents = File.ReadAllText(LinuxNetworkFiles.SnmpV4StatsFile);
            int firstTcpHeader = fileContents.IndexOf("Tcp:");
            int secondTcpHeader = fileContents.IndexOf("Tcp:", firstTcpHeader + 1);
            int endOfSecondLine = fileContents.IndexOf(Environment.NewLine, secondTcpHeader);
            string tcpData = fileContents.Substring(secondTcpHeader, endOfSecondLine - secondTcpHeader);
            StringParser parser = new StringParser(tcpData, ' ');

            // NOTE: Need to verify that this order is consistent. Otherwise, we need to parse the first-line header
            // to determine the order of information contained in the row.

            parser.MoveNextOrFail(); // Skip Tcp:

            stats._rtoAlgorithm = parser.ParseNextInt32();
            stats._rtoMin = parser.ParseNextInt32();
            stats._rtoMax = parser.ParseNextInt32();
            stats._maxConn = parser.ParseNextInt32();
            stats._activeOpens = parser.ParseNextInt32();
            stats._passiveOpens = parser.ParseNextInt32();
            stats._attemptFails = parser.ParseNextInt32();
            stats._estabResets = parser.ParseNextInt32();
            stats._currEstab = parser.ParseNextInt32();
            stats._inSegs = parser.ParseNextInt32();
            stats._outSegs = parser.ParseNextInt32();
            stats._retransSegs = parser.ParseNextInt32();
            stats._inErrs = parser.ParseNextInt32();
            stats._outRsts = parser.ParseNextInt32();
            stats._inCsumErrors = parser.ParseNextInt32();

            // Parse the number of active connections out of /proc/net/sockstat
            string sockstatFile = File.ReadAllText(ipv6 ? LinuxNetworkFiles.Sockstat6File : LinuxNetworkFiles.SockstatFile);
            string tcpLabel = ipv6 ? "TCP6:" : "TCP:";
            int indexOfTcp = sockstatFile.IndexOf(tcpLabel);
            int endOfTcpLine = sockstatFile.IndexOf(Environment.NewLine, indexOfTcp + 1);
            string tcpLineData = sockstatFile.Substring(indexOfTcp, endOfTcpLine - indexOfTcp);
            StringParser sockstatParser = new StringParser(tcpLineData, ' ');
            sockstatParser.MoveNextOrFail(); // Skip "TCP(6):"
            sockstatParser.MoveNextOrFail(); // Skip: "inuse"
            stats._currentConnections = sockstatParser.ParseNextInt32();

            return stats;
        }

        public override long ConnectionsAccepted { get { throw new NotImplementedException(); } }
        public override long ConnectionsInitiated { get { throw new NotImplementedException(); } }
        public override long CumulativeConnections { get { throw new NotImplementedException(); } }
        public override long CurrentConnections { get { return _currentConnections; } }
        public override long ErrorsReceived { get { return _inErrs; } }
        public override long FailedConnectionAttempts { get { return _attemptFails; } }
        public override long MaximumConnections { get { return _maxConn; } }
        public override long MaximumTransmissionTimeout { get { throw new NotImplementedException(); } }
        public override long MinimumTransmissionTimeout { get { throw new NotImplementedException(); } }
        public override long ResetConnections { get { return _estabResets; } }
        public override long ResetsSent { get { throw new NotImplementedException(); } }
        public override long SegmentsReceived { get { return _inSegs; } }
        public override long SegmentsResent { get { return _retransSegs; } }
        public override long SegmentsSent { get { return _outSegs; } }
    }
}
