using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPv4InterfaceProperties : IPv4InterfaceProperties
    {
        private LinuxNetworkInterface _linuxNetworkInterface;

        public LinuxIPv4InterfaceProperties(LinuxNetworkInterface linuxNetworkInterface)
        {
            _linuxNetworkInterface = linuxNetworkInterface;
        }

        public override int Index
        {
            get
            {
                return _linuxNetworkInterface.Index;
            }
        }

        public override bool IsAutomaticPrivateAddressingActive
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool IsAutomaticPrivateAddressingEnabled
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool IsDhcpEnabled
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool IsForwardingEnabled
        {
            get
            {
                // /proc/sys/net/ipv4/conf/<name>/forwarding
                string path = Path.Combine(LinuxNetworkFiles.ProcSysNetFolder, "ipv4", "conf", _linuxNetworkInterface.Name, "forwarding");
                return int.Parse(File.ReadAllText(path)) == 1;
            }
        }

        public override int Mtu
        {
            get
            {
                // /proc/sys/net/ipv4/conf/<name>/mtu
                string path = Path.Combine(LinuxNetworkFiles.ProcSysNetFolder, "ipv4", "conf", _linuxNetworkInterface.Name, "mtu");
                return int.Parse(File.ReadAllText(path));
            }
        }

        public override bool UsesWins
        {
            get
            {
                // TODO: This seems Windows-specific. Can we just return false?
                throw new PlatformNotSupportedException();
            }
        }
    }
}