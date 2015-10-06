// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPGlobalStatistics : IPGlobalStatistics
    {
        // Information exposed in the snmp (ipv4) and snmp6 (ipv6) files under /proc/net
        // Each piece corresponds to a data item defined in the MIB-II specification:
        // http://www.ietf.org/rfc/rfc1213.txt
        // Each field's comment indicates the name as it appears in the snmp (snmp6) file.
        // In the snmp6 file, each name is prefixed with "IP6".

        private bool _forwarding; // Forwarding
        private int _defaultTtl; // DefaultTTL

        private int _inReceives; // InReceives
        private int _inHeaderErrors; // InHdrErrors
        private int _inAddressErrors; // InAddrErrors
        private int _forwardedDatagrams; // ForwDatagrams (IP6OutForwDatagrams)
        private int _inUnknownProtocols; // InUnknownProtos
        private int _inDiscards; // InDiscards
        private int _inDelivers; // InDelivers

        private int _outRequests; // OutRequestscat 
        private int _outDiscards; // OutDiscards
        private int _outNoRoutes; // OutNoRoutes

        private int _reassemblyTimeout; // ReasmTimeout
        private int _reassemblyRequireds; // ReasmReqds
        private int _reassemblyOKs; // ReasmOKs
        private int _reassemblyFails; // ReasmFails

        private int _fragmentOKs; // FragOKs
        private int _fragmentFails; // FragFails
        private int _fragmentCreates; // FragCreates

        // Miscellaneous IP information, not defined in MIB-II.
        private int _numRoutes;
        private int _numInterfaces;

        private LinuxIPGlobalStatistics() { }

        /// <summary>
        /// Constructs an IPGlobalStatistics object from the snmp4 stats file.
        /// </summary>
        public static LinuxIPGlobalStatistics CreateIPv4Statistics()
        {
            LinuxIPGlobalStatistics stats = new LinuxIPGlobalStatistics();
            string fileContents = File.ReadAllText(LinuxNetworkFiles.SnmpV4StatsFile);
            int firstIpHeader = fileContents.IndexOf("Ip:");
            int secondIpHeader = fileContents.IndexOf("Ip:", firstIpHeader + 1);
            int endOfSecondLine = fileContents.IndexOf(Environment.NewLine, secondIpHeader);
            string ipData = fileContents.Substring(secondIpHeader, endOfSecondLine - secondIpHeader);
            StringParser parser = new StringParser(ipData, ' ');

            // NOTE: Need to verify that this order is consistent. Otherwise, we need to parse the first-line header
            // to determine the order of information contained in the file.

            parser.MoveNextOrFail(); // Skip Ip:

            // According to RFC 1213, "1" indicates "acting as a gateway". "2" indicates "NOT acting as a gateway".
            stats._forwarding = parser.MoveAndExtractNext() == "1";
            stats._defaultTtl = parser.ParseNextInt32();
            stats._inReceives = parser.ParseNextInt32();
            stats._inHeaderErrors = parser.ParseNextInt32();
            stats._inAddressErrors = parser.ParseNextInt32();
            stats._forwardedDatagrams = parser.ParseNextInt32();
            stats._inUnknownProtocols = parser.ParseNextInt32();
            stats._inDiscards = parser.ParseNextInt32();
            stats._inDelivers = parser.ParseNextInt32();
            stats._outRequests = parser.ParseNextInt32();
            stats._outDiscards = parser.ParseNextInt32();
            stats._outNoRoutes = parser.ParseNextInt32();
            stats._reassemblyTimeout = parser.ParseNextInt32();
            stats._reassemblyRequireds = parser.ParseNextInt32();
            stats._reassemblyOKs = parser.ParseNextInt32();
            stats._reassemblyFails = parser.ParseNextInt32();
            stats._fragmentOKs = parser.ParseNextInt32();
            stats._fragmentFails = parser.ParseNextInt32();
            stats._fragmentCreates = parser.ParseNextInt32();

            // Experimental
            string routeFile = ReadProcConfigFile(LinuxNetworkFiles.Ipv4RouteFile);
            stats._numRoutes = CountOccurences(Environment.NewLine, routeFile) - 1; // File includes one-line header

            // Just count the number of files under /proc/sys/net/ipv4/conf,
            // because GetAllNetworkInterfaces() is relatively expensive just for the count.
            int interfacesCount = 0;
            var files = new DirectoryInfo(LinuxNetworkFiles.Ipv4ConfigFolder).GetFiles();
            foreach (var file in files)
            {
                if (file.Name != "all" && file.Name != "default")
                {
                    interfacesCount++;
                }
            }
            stats._numInterfaces = interfacesCount;

            return stats;
        }

        /// <summary>
        /// Constructs an IPGlobalStatistics object from the snmp6 stats file.
        /// </summary>
        public static LinuxIPGlobalStatistics CreateIPv6Statistics()
        {
            LinuxIPGlobalStatistics stats = new LinuxIPGlobalStatistics();

            string fileContents = File.ReadAllText(LinuxNetworkFiles.SnmpV6StatsFile);

            // Perf improvement: Read the data in order, and have the reader remember your position.
            // Possibly add that functionality into StringParser as it does similar things.
            RowConfigReader reader = new RowConfigReader(fileContents);

            stats._forwarding = false; // Same as IPv4?
            stats._defaultTtl = 0; // Same as IPv4?
            stats._inReceives = reader.GetNextValueAsInt32("Ip6InReceives");
            stats._inHeaderErrors = reader.GetNextValueAsInt32("Ip6InHdrErrors");
            stats._inAddressErrors = reader.GetNextValueAsInt32("Ip6InAddrErrors");
            stats._inUnknownProtocols = reader.GetNextValueAsInt32("Ip6InUnknownProtos");
            stats._inDiscards = reader.GetNextValueAsInt32("Ip6InDiscards");
            stats._inDelivers = reader.GetNextValueAsInt32("Ip6InDelivers");
            stats._forwardedDatagrams = reader.GetNextValueAsInt32("Ip6OutForwDatagrams");
            stats._outRequests = reader.GetNextValueAsInt32("Ip6OutRequests");
            stats._outDiscards = reader.GetNextValueAsInt32("Ip6OutDiscards");
            stats._outNoRoutes = reader.GetNextValueAsInt32("Ip6OutNoRoutes");
            stats._reassemblyTimeout = reader.GetNextValueAsInt32("Ip6ReasmTimeout");
            stats._reassemblyRequireds = reader.GetNextValueAsInt32("Ip6ReasmReqds");
            stats._reassemblyOKs = reader.GetNextValueAsInt32("Ip6ReasmOKs");
            stats._reassemblyFails = reader.GetNextValueAsInt32("Ip6ReasmFails");
            stats._fragmentOKs = reader.GetNextValueAsInt32("Ip6FragOKs");
            stats._fragmentFails = reader.GetNextValueAsInt32("Ip6FragFails");
            stats._fragmentCreates = reader.GetNextValueAsInt32("Ip6FragCreates");

            // Experimental
            string routeFile = ReadProcConfigFile(LinuxNetworkFiles.Ipv6RouteFile);
            stats._numRoutes = CountOccurences(Environment.NewLine, routeFile);

            // Just count the number of files under /proc/sys/net/ipv6/conf (?)
            int interfaceCount = 0;
            var files = new DirectoryInfo(LinuxNetworkFiles.Ipv6ConfigFolder).GetFiles();
            foreach (var file in files)
            {
                if (file.Name != "all" && file.Name != "default")
                {
                    interfaceCount++;
                }
            }
            stats._numInterfaces = interfaceCount;

            return stats;
        }

        public override int DefaultTtl { get { return _defaultTtl; } }

        public override bool ForwardingEnabled { get { return _forwarding; } }

        public override int NumberOfInterfaces { get { return _numInterfaces; } }

        // Not sure how to determine this.
        public override int NumberOfIPAddresses
        {
            get
            {
                int count = 0;
                foreach (LinuxNetworkInterface lni in LinuxNetworkInterface.GetLinuxNetworkInterfaces())
                {
                    // Could there be duplicates, somehow?
                    count += lni.Addresses.Count;
                }
                return count;
            }
        }

        public override int NumberOfRoutes { get { return _numRoutes; } }

        public override long OutputPacketRequests { get { return _outRequests; } }

        /* Should be:

            ipRoutingDiscards OBJECT-TYPE
              SYNTAX Counter
              ACCESS read-only
              STATUS mandatory
              DESCRIPTION
                  "The number of routing entries which were chosen
                  to be discarded even though they are valid.  One
                  possible reason for discarding such an entry could
                  be to free - up buffer space for other routing

            Isn't exposed in snmp or snmp6, although it is listed as mandatory in the specification.
        */
        public override long OutputPacketRoutingDiscards { get { throw new PlatformNotSupportedException(); } }

        public override long OutputPacketsDiscarded { get { return _outDiscards; } }

        public override long OutputPacketsWithNoRoute { get { return _outNoRoutes; } }

        public override long PacketFragmentFailures { get { return _fragmentFails; } }

        public override long PacketReassembliesRequired { get { return _reassemblyRequireds; } }

        public override long PacketReassemblyFailures { get { return _reassemblyFails; } }

        public override long PacketReassemblyTimeout { get { return _reassemblyTimeout; } }

        public override long PacketsFragmented { get { return _fragmentCreates; } }

        public override long PacketsReassembled { get { return _reassemblyOKs; } }

        public override long ReceivedPackets { get { return _inReceives; } }

        public override long ReceivedPacketsDelivered { get { return _inDelivers; } }

        public override long ReceivedPacketsDiscarded { get { return _inDiscards; } }

        public override long ReceivedPacketsForwarded { get { return _forwardedDatagrams; } }

        public override long ReceivedPacketsWithAddressErrors { get { return _inAddressErrors; } }

        public override long ReceivedPacketsWithHeadersErrors { get { return _inHeaderErrors; } }

        public override long ReceivedPacketsWithUnknownProtocol { get { return _inUnknownProtocols; } }

        private static string ReadProcConfigFile(string v)
        {
            return File.ReadAllText(v);
        }

        private static int CountOccurences(string value, string candidate)
        {
            int index = 0;
            int occurrences = 0;
            while (index != -1)
            {
                index = candidate.IndexOf(value, index + 1);
                if (index != -1)
                {
                    occurrences++;
                }
            }

            return occurrences;
        }
    }
}