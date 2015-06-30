// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
    public class PingReply
    {
        private IPAddress _address;
        private PingOptions _options;
        private IPStatus _ipStatus;  // the status code returned by icmpsendecho, or the icmp status field on the raw socket
        private long _rtt;  // the round trip time.
        private byte[] _buffer; //buffer of the data


        internal PingReply()
        {
        }

        internal PingReply(IPStatus ipStatus)
        {
            _ipStatus = ipStatus;
            _buffer = Array.Empty<byte>();
        }

        // The downlevel constructor. 
        internal PingReply(byte[] data, int dataLength, IPAddress address, int time)
        {
            _address = address;
            _rtt = time;


            _ipStatus = GetIPStatus((IcmpV4Type)data[20], (IcmpV4Code)data[21]);

            if (_ipStatus == IPStatus.Success)
            {
                _buffer = new byte[dataLength - 28];
                Array.Copy(data, 28, _buffer, 0, dataLength - 28);
            }
            else
                _buffer = Array.Empty<byte>();
        }


        // the main constructor for the icmpsendecho apis
        internal PingReply(IcmpEchoReply reply)
        {
            _address = new IPAddress(reply.address);
            _ipStatus = (IPStatus)reply.status; //the icmpsendecho ip status codes

            //only copy the data if we succeed w/ the ping operation
            if (_ipStatus == IPStatus.Success)
            {
                _rtt = (long)reply.roundTripTime;
                _buffer = new byte[reply.dataSize];
                Marshal.Copy(reply.data, _buffer, 0, reply.dataSize);
                _options = new PingOptions(reply.options);
            }
            else
                _buffer = Array.Empty<byte>();
        }

        // the main constructor for the icmpsendecho apis
        internal PingReply(Icmp6EchoReply reply, IntPtr dataPtr, int sendSize)
        {
            _address = new IPAddress(reply.Address.Address, reply.Address.ScopeID);
            _ipStatus = (IPStatus)reply.Status; //the icmpsendecho ip status codes

            //only copy the data if we succeed w/ the ping operation
            if (_ipStatus == IPStatus.Success)
            {
                _rtt = (long)reply.RoundTripTime;
                _buffer = new byte[sendSize];
                Marshal.Copy(IntPtrHelper.Add(dataPtr, 36), _buffer, 0, sendSize);
                //options = new PingOptions (reply.options);
            }
            else
                _buffer = Array.Empty<byte>();
        }



        //translates the relevant icmpsendecho codes to a ipstatus code
        private IPStatus GetIPStatus(IcmpV4Type type, IcmpV4Code code)
        {
            switch (type)
            {
                case IcmpV4Type.ICMP4_ECHO_REPLY:
                    return IPStatus.Success;
                case IcmpV4Type.ICMP4_SOURCE_QUENCH:
                    return IPStatus.SourceQuench;
                case IcmpV4Type.ICMP4_PARAM_PROB:
                    return IPStatus.ParameterProblem;
                case IcmpV4Type.ICMP4_TIME_EXCEEDED:
                    return IPStatus.TtlExpired;

                case IcmpV4Type.ICMP4_DST_UNREACH:
                    {
                        switch (code)
                        {
                            case IcmpV4Code.ICMP4_UNREACH_NET:
                                return IPStatus.DestinationNetworkUnreachable;
                            case IcmpV4Code.ICMP4_UNREACH_HOST:
                                return IPStatus.DestinationHostUnreachable;
                            case IcmpV4Code.ICMP4_UNREACH_PROTOCOL:
                                return IPStatus.DestinationProtocolUnreachable;
                            case IcmpV4Code.ICMP4_UNREACH_PORT:
                                return IPStatus.DestinationPortUnreachable;
                            case IcmpV4Code.ICMP4_UNREACH_FRAG_NEEDED:
                                return IPStatus.PacketTooBig;
                            default:
                                return IPStatus.DestinationUnreachable;
                        }
                    }
            }
            return IPStatus.Unknown;
        }

        //the basic properties
        public IPStatus Status { get { return _ipStatus; } }
        public IPAddress Address { get { return _address; } }
        public long RoundtripTime { get { return _rtt; } }
        public PingOptions Options
        {
            get
            {
                return _options;
            }
        }
        public byte[] Buffer { get { return _buffer; } }
    }
}
