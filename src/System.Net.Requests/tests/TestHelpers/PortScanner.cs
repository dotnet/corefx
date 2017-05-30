using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace TestHelpers
{
    /// <summary>
    /// Class helps find closed port for tests. 
    /// </summary>
    internal class PortScanner
    {
        /// <summary>
        /// Collection of all ports with state different then closed.
        /// </summary>
        private readonly IEnumerable<int> _busyPorts;
        /// <summary>
        /// End port in range of scanning ports.
        /// </summary>
        private const int EndPort = 65535;
        /// <summary>
        /// Start port in range of scanning ports.
        /// </summary>
        private const int StartPort = 49152;

        internal PortScanner()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            _busyPorts = GetAllBusyPorts(ipGlobalProperties);
        }

        /// <summary>
        /// It scans ports in local network and returns first port which is unused.
        /// </summary>
        internal int FirstClosedPort
        {
            get
            {
                for (int currentPort = StartPort; currentPort <= EndPort; currentPort++)
                {
                    if (!IsBusyPort(currentPort))
                        return currentPort;
                }

                LogNoFreePortAlert();
                return -1;
            }
        }

        private IEnumerable<int> GetAllBusyPorts(IPGlobalProperties ipGlobalProperties)
        {
            try
            {
                return ipGlobalProperties.GetActiveTcpListeners()
                    .Select(tcpListener => tcpListener.Port)
                    .Concat(ipGlobalProperties.GetActiveUdpListeners().Select(udpListener => udpListener.Port))
                    .Concat(ipGlobalProperties.GetActiveTcpConnections().Select(tcpConnection => tcpConnection.LocalEndPoint.Port));
            }
            catch (Exception)
            {
                return Enumerable.Range(StartPort, EndPort - StartPort + 1);
            }
        }

        private bool IsBusyPort(int port)
        {
            return _busyPorts.Any(busyPort => busyPort == port);
        }

        private void LogNoFreePortAlert()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("There is no free port in range {0} to {1} or there is a problem with reading informations about ports. "
                + "Test will fail. Close some port in this range and try again.", StartPort, EndPort);
            Console.ResetColor();
        }
    }
}