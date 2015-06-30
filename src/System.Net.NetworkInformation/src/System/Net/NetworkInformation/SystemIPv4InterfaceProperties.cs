
    /// <summary><para>
    ///    Provides support for ip configuation information and statistics.
    ///</para></summary>
    ///
namespace System.Net.NetworkInformation {

    using System.Net;
    using System;
    using System.Runtime.InteropServices;

    internal class SystemIPv4InterfaceProperties:IPv4InterfaceProperties{

        //these are only valid for ipv4 interfaces
        bool haveWins = false;
        bool dhcpEnabled = false;
        bool routingEnabled = false;
        bool autoConfigEnabled = false;
        bool autoConfigActive = false;
        uint index = 0;
        uint mtu = 0;

        // Vista+
        internal SystemIPv4InterfaceProperties(FIXED_INFO fixedInfo, IpAdapterAddresses ipAdapterAddresses) {
            index = ipAdapterAddresses.index;
            routingEnabled = fixedInfo.enableRouting;
            dhcpEnabled = ((ipAdapterAddresses.flags & AdapterFlags.DhcpEnabled) != 0);
            haveWins = (ipAdapterAddresses.firstWinsServerAddress != IntPtr.Zero);

            mtu = ipAdapterAddresses.mtu;

            GetPerAdapterInfo(ipAdapterAddresses.index);
        }

        /// <summary>Only valid for Ipv4 Uses WINS for name resolution.</summary>
        public override bool UsesWins{get {return haveWins;}}


        public override bool IsDhcpEnabled{
            get { return dhcpEnabled; }
        }

        public override bool IsForwardingEnabled{get {return routingEnabled;}}      //proto
                      


        /// <summary>Auto configuration of an ipv4 address for a client
        /// on a network where a DHCP server
        /// isn't available.</summary>
        public override bool IsAutomaticPrivateAddressingEnabled{
            get{
                return autoConfigEnabled;
            }
        } // proto

        public override bool IsAutomaticPrivateAddressingActive{
            get{
                return autoConfigActive;
            }
        }


        /// <summary>Specifies the Maximum transmission unit in bytes. Uses GetIFEntry.</summary>
        //We cache this to be consistent across all platforms
        public override int Mtu{
            get {
                return (int) mtu;
            }
        }

        public override int Index{
            get {
                return (int) index;
            }
        }

        private void GetPerAdapterInfo(uint index) {

            if (index != 0){
                uint size = 0;
                SafeLocalFree buffer = null;
    
                uint result = UnsafeNetInfoNativeMethods.GetPerAdapterInfo(index,SafeLocalFree.Zero,ref size);
                while (result == IpHelperErrors.ErrorBufferOverflow) {
                    try {
                        //now we allocate the buffer and read the network parameters.
                        buffer =  SafeLocalFree.LocalAlloc((int)size);
                        result = UnsafeNetInfoNativeMethods.GetPerAdapterInfo(index,buffer,ref size);
                        if ( result == IpHelperErrors.Success ) {
                            IpPerAdapterInfo ipPerAdapterInfo = Marshal.PtrToStructure<IpPerAdapterInfo>(buffer.DangerousGetHandle());

                            autoConfigEnabled = ipPerAdapterInfo.autoconfigEnabled;
                            autoConfigActive = ipPerAdapterInfo.autoconfigActive;
                        }
                    }
                    finally {
                        if (buffer != null)
                            buffer.Dispose();
                    }
                }
                
                if (result != IpHelperErrors.Success) {
                    throw new NetworkInformationException((int)result);
                }
            }
        }
    }
}
