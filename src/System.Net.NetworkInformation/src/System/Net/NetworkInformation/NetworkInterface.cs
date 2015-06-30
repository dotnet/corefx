// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.NetworkInformation
{
    public abstract class NetworkInterface
    {
        /// Returns objects that describe the network interfaces on the local computer.
        public static NetworkInterface[] GetAllNetworkInterfaces()
        {
            return SystemNetworkInterface.GetNetworkInterfaces();
        }

        public static bool GetIsNetworkAvailable()
        {
            return SystemNetworkInterface.InternalGetIsNetworkAvailable();
        }

        public static int LoopbackInterfaceIndex
        {
            get
            {
                return SystemNetworkInterface.InternalLoopbackInterfaceIndex;
            }
        }

        public static int IPv6LoopbackInterfaceIndex
        {
            get
            {
                return SystemNetworkInterface.InternalIPv6LoopbackInterfaceIndex;
            }
        }

        public virtual string Id { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets the name of the network interface.
        public virtual string Name { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets the description of the network interface
        public virtual string Description { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets the IP properties for this network interface.
        public virtual IPInterfaceProperties GetIPProperties()
        {
            throw ExceptionHelper.MethodNotImplementedException;
        }

        /// Provides Internet Protocol (IP) statistical data for thisnetwork interface.
        /// Despite the naming, the results are not IPv4 specific.
        /// Do not use this method, use GetIPStatistics instead.
        public virtual IPv4InterfaceStatistics GetIPv4Statistics()
        {
            throw ExceptionHelper.MethodNotImplementedException;
        }

        /// Provides Internet Protocol (IP) statistical data for this network interface.
        public virtual IPInterfaceStatistics GetIPStatistics()
        {
            throw ExceptionHelper.MethodNotImplementedException;
        }

        /// Gets the current operational state of the network connection.
        public virtual OperationalStatus OperationalStatus { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets the speed of the interface in bits per second as reported by the interface.
        public virtual long Speed { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets a bool value that indicates whether the network interface is set to only receive data packets.
        public virtual bool IsReceiveOnly { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets a bool value that indicates whether this network interface is enabled to receive multicast packets.
        public virtual bool SupportsMulticast { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        /// Gets the physical address of this network interface
        /// <b>deonb. This is okay if you don't support this in Whidbey. This actually belongs in the NetworkAdapter derived class</b>
        public virtual PhysicalAddress GetPhysicalAddress()
        {
            throw ExceptionHelper.MethodNotImplementedException;
        }

        /// Gets the interface type.
        public virtual NetworkInterfaceType NetworkInterfaceType { get { throw ExceptionHelper.PropertyNotImplementedException; } }

        public virtual bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
        {
            throw ExceptionHelper.MethodNotImplementedException;
        }
    }
}

