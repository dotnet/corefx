using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
    /// <summary>
    /// Implements a NetworkInterface on Linux.
    /// </summary>
    internal class LinuxNetworkInterface : NetworkInterface
    {
        private string _name;
        private int _index;
        private NetworkInterfaceType _networkInterfaceType = NetworkInterfaceType.Unknown;
        private PhysicalAddress _physicalAddress;
        private List<IPAddress> _addresses = new List<IPAddress>();
        private Dictionary<IPAddress, IPAddress> _netMasks = new Dictionary<IPAddress, IPAddress>();

        // If this is an ipv6 device, contains the Scope ID.
        private uint? _ipv6ScopeId = null;

        /// <summary>
        /// The system's index for this network device.
        /// </summary>
        public int Index { get { return _index; } }

        /// <summary>
        /// Returns a list of all of the interface's IP Addresses.
        /// </summary>
        public List<IPAddress> Addresses { get { return _addresses; } }

        private LinuxNetworkInterface(string name)
        {
            _name = name;
        }

        public static NetworkInterface[] GetLinuxNetworkInterfaces()
        {
            IntPtr headPtr = IntPtr.Zero;
            if (Interop.libc.getifaddrs(out headPtr) == 0)
            {
                try
                {
                    return GetInterfacesFromLinkedList(headPtr);
                }
                finally
                {
                    Interop.libc.freeifaddrs(headPtr);
                }
            }
            else
            {
                throw new NetworkInformationException(Marshal.GetLastWin32Error());
            }
        }

        private static unsafe NetworkInterface[] GetInterfacesFromLinkedList(IntPtr headPtr)
        {
            /*  
                TODO: Reimplement this logic using a shim on top of free/getifaddrs. The majority of this logic
                should be applicable on all platforms that impelement the getifaddrs function. This logic
                should also be lifted into a common base class ("UnixNetworkInterface"), or exposed generically
                so that the constructors of derived interfaces ("LinuxNetworkInterface", "MacNetworkInterface"
                can use the data retrieved to construct themselves.
            */
            if (headPtr == IntPtr.Zero)
            {
                // TODO: Is this legitimate? getifaddrs behaviour is unclear if there are no network interfaces.
                return Array.Empty<NetworkInterface>();
            }

            // The linked list structure will contain multiple nodes per device interface, one for each
            // address address associated with a device. We need to enumerate all of these addresses and store them in the correct interface.
            Dictionary<string, LinuxNetworkInterface> interfacesByName = new Dictionary<string, LinuxNetworkInterface>();

            while (headPtr != IntPtr.Zero)
            {
                Interop.libc.ifaddrs headInterface = Marshal.PtrToStructure<Interop.libc.ifaddrs>(headPtr);
                LinuxNetworkInterface lni = GetOrCreate(interfacesByName, headInterface.ifa_name);

                // Process address by family (ipv4, ipv6, link-level)
                switch (headInterface.ifa_addr->sa_family)
                {
                    // IPv4 Address
                    case Interop.libc.AF_INET:
                        {
                            ProcessIpv4Address(headInterface, lni);
                            break;
                        }

                    // IPv6 Address
                    case Interop.libc.AF_INET6:
                        {
                            ProcessIpv6Address(headInterface, lni);
                            break;
                        }

                    // Link-Layer Address (MAC)
                    case Interop.libc.AF_PACKET:
                        {
                            ProcessLinkLevelAddress(headInterface, lni);
                            break;
                        }

                    // TODO: Determine if we should throw an exception here or if there are other valid address types.
                    default:
                        Debug.WriteLine("Unrecognized sa_family: " + headInterface.ifa_addr->sa_family);
                        break;
                }

                headPtr = headInterface.ifa_next;
            }

            return interfacesByName.Values.ToArray();
        }

        private static void ProcessIpv4Address(Interop.libc.ifaddrs headInterface, LinuxNetworkInterface lni)
        {
            Interop.libc.sockaddr_in ipv4Address = headInterface.GetIPv4Address();
            IPAddress addr = new IPAddress(ipv4Address.sin_addr.s_addr);
            lni.AddAddress(addr);
            Debug.WriteLine("Interface " + headInterface.ifa_name + " has an IPv4 address: " + new IPAddress(ipv4Address.sin_addr.s_addr));

            IPAddress netMask = new IPAddress(headInterface.GetNetMask().sin_addr.s_addr);
            Debug.Assert(!lni._netMasks.ContainsKey(addr), "NetworkInterface already contains a net mask for address " + addr);
            lni._netMasks.Add(addr, netMask); // This will throw if there is somehow a duplicate address.
        }

        private static unsafe void ProcessIpv6Address(Interop.libc.ifaddrs headInterface, LinuxNetworkInterface lni)
        {
            Interop.libc.sockaddr_in6 ipv6Address = headInterface.GetIPv6Address();
            lni._ipv6ScopeId = ipv6Address.sin6_scope_id;
            byte[] addressBytes = new byte[16];
            fixed (byte* ipv6BytesPtr = addressBytes)
            {
                Buffer.MemoryCopy(ipv6Address.sin6_addr.s6_addr, ipv6BytesPtr, 16, 16);
            }
            lni.AddAddress(new IPAddress(addressBytes));
            Debug.WriteLine("Interface " + headInterface.ifa_name + " has an IPv6 address: " + new IPAddress(addressBytes));
        }

        private static unsafe void ProcessLinkLevelAddress(Interop.libc.ifaddrs headInterface, LinuxNetworkInterface lni)
        {
            Interop.libc.sockaddr_ll linkLevelAddress = headInterface.GetLinkLevelAddress();
            lni._index = linkLevelAddress.sll_ifindex;
            Debug.WriteLine("Interface  " + headInterface.ifa_name + " has index " + lni._index);
            if (linkLevelAddress.sll_halen > 0)
            {
                byte[] macAddress = new byte[linkLevelAddress.sll_halen];
                fixed (byte* macAddressPtr = macAddress)
                {
                    Buffer.MemoryCopy(linkLevelAddress.sll_addr, macAddressPtr, macAddress.Length, macAddress.Length);
                }
                lni._physicalAddress = new PhysicalAddress(macAddress);

                Debug.WriteLine(headInterface.ifa_name + " MAC Address: " + lni._physicalAddress);
            }

            lni._networkInterfaceType = MapArpHardwareType(linkLevelAddress.sll_hatype);
        }

        // Maps ARPHRD_* values to analogous NetworkInterfaceType values, as closely as possible.
        private static NetworkInterfaceType MapArpHardwareType(ushort arpHardwareType)
        {
            switch (arpHardwareType)
            {
                case Interop.libc.ARPHRD_ETHER:
                case Interop.libc.ARPHRD_EETHER:
                    return NetworkInterfaceType.Ethernet;

                case Interop.libc.ARPHRD_PRONET:
                    return NetworkInterfaceType.TokenRing;

                case Interop.libc.ARPHRD_ATM:
                    return NetworkInterfaceType.Atm;

                case Interop.libc.ARPHRD_SLIP:
                case Interop.libc.ARPHRD_CSLIP:
                case Interop.libc.ARPHRD_SLIP6:
                case Interop.libc.ARPHRD_CSLIP6:
                    return NetworkInterfaceType.Slip;

                case Interop.libc.ARPHRD_PPP:
                    return NetworkInterfaceType.Ppp;

                case Interop.libc.ARPHRD_TUNNEL:
                case Interop.libc.ARPHRD_TUNNEL6:
                    return NetworkInterfaceType.Tunnel;

                case Interop.libc.ARPHRD_LOOPBACK:
                    return NetworkInterfaceType.Loopback;

                case Interop.libc.ARPHRD_FDDI:
                    return NetworkInterfaceType.Fddi;

                case Interop.libc.ARPHRD_IEEE80211:
                case Interop.libc.ARPHRD_IEEE80211_PRISM:
                case Interop.libc.ARPHRD_IEEE80211_RADIOTAP:
                    return NetworkInterfaceType.Wireless80211;

                default:
                    Debug.WriteLine("Unmapped ARP Hardware type: " + arpHardwareType);
                    return NetworkInterfaceType.Unknown;
            }
        }

        // Adds any IPAddress to this interface's List of addresses.
        private void AddAddress(IPAddress ipAddress)
        {
            _addresses.Add(ipAddress);
        }

        /// <summary>
        /// Gets or creates a LinuxNetworkInterface, based on whether it already exists in the given Dictionary.
        /// If created, it is added to the Dictionary.
        /// </summary>
        /// <param name="interfaces">The Dictionary of existing interfaces.</param>
        /// <param name="name">The name of the interface.</param>
        /// <returns>The cached or new LinuxNetworkInterface with the given name.</returns>
        private static LinuxNetworkInterface GetOrCreate(Dictionary<string, LinuxNetworkInterface> interfaces, string name)
        {
            LinuxNetworkInterface lni;
            if (!interfaces.TryGetValue(name, out lni))
            {
                lni = new LinuxNetworkInterface(name);
                interfaces.Add(name, lni);
            }

            return lni;
        }

        public override string Name { get { return _name; } }

        public override NetworkInterfaceType NetworkInterfaceType { get { return _networkInterfaceType; } }

        public override bool SupportsMulticast { get { return GetSupportsMulticast(); } }

        private bool GetSupportsMulticast()
        {
            string path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _name, "flags");
            string fileContents = File.ReadAllText(path).Trim();
            LinuxNetDeviceFlags flags = (LinuxNetDeviceFlags)Convert.ToInt32(fileContents, 16);
            return (flags & LinuxNetDeviceFlags.IFF_MULTICAST) == LinuxNetDeviceFlags.IFF_MULTICAST;
        }

        public override IPInterfaceProperties GetIPProperties()
        {
            return new LinuxIPInterfaceProperties(this);
        }

        public override IPInterfaceStatistics GetIPStatistics()
        {
            return new LinuxIPInterfaceStatistics(_name);
        }

        public override IPv4InterfaceStatistics GetIPv4Statistics()
        {
            return new LinuxIpv4InterfaceStatisticsWrapper(_name);
        }

        public override PhysicalAddress GetPhysicalAddress()
        {
            Debug.Assert(_physicalAddress != null, "_physicalAddress was never initialized. This means no address with type AF_PACKET was discovered.");
            return _physicalAddress;
        }

        public override OperationalStatus OperationalStatus
        {
            get
            {
                // /sys/class/net/<name>/operstate
                string path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _name, "operstate");
                string state = File.ReadAllText(path).Trim();
                return MapState(state);
            }
        }

        // Maps values from /sys/class/net/<interface>/operstate to OperationStatus values.
        private OperationalStatus MapState(string state)
        {
            // TODO: Figure out the possible values that Linux might return.
            switch (state)
            {
                case "up":
                    return OperationalStatus.Up;
                case "down":
                    return OperationalStatus.Down;
                default:
                    return OperationalStatus.Unknown;
            }
        }

        public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
        {
            Sockets.AddressFamily family =
                (networkInterfaceComponent == NetworkInterfaceComponent.IPv4)
                ? Sockets.AddressFamily.InterNetwork
                : Sockets.AddressFamily.InterNetworkV6;

            return _addresses.Any(addr => addr.AddressFamily == family);
        }

        public override string Id { get { throw new PlatformNotSupportedException(); } }

        public override string Description { get { throw new PlatformNotSupportedException(); } }

        public override long Speed
        {
            get
            {
                try
                {
                    string path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _name, "speed");
                    string contents = File.ReadAllText(path);
                    long val;
                    if (long.TryParse(contents, out val))
                    {
                        return val;
                    }
                    else
                    {
                        throw new PlatformNotSupportedException();
                    }
                }
                catch (IOException) // Some interfaces may give an "Invalid argument" error when opening this file.
                {
                    throw new PlatformNotSupportedException();
                }
            }
        }

        public override bool IsReceiveOnly { get { throw new PlatformNotSupportedException(); } }

        public IPAddress GetNetMaskForIPv4Address(IPAddress address)
        {
            Debug.Assert(address.AddressFamily == Sockets.AddressFamily.InterNetwork);
            return _netMasks[address];
        }
    }
}
