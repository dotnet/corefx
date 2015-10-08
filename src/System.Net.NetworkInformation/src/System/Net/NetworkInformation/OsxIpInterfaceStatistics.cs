namespace System.Net.NetworkInformation
{
    internal class OsxIpInterfaceStatistics : IPInterfaceStatistics
    {
        private readonly string _name;

        public OsxIpInterfaceStatistics(string name)
        {
            _name = name;
        }

        public override long BytesReceived
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long BytesSent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long IncomingPacketsDiscarded
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long IncomingPacketsWithErrors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long IncomingUnknownProtocolPackets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long NonUnicastPacketsReceived
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long NonUnicastPacketsSent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long OutgoingPacketsDiscarded
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long OutgoingPacketsWithErrors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long OutputQueueLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long UnicastPacketsReceived
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long UnicastPacketsSent
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
