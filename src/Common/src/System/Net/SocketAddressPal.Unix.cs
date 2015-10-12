// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace System.Net
{
    internal static class SocketAddressPal
    {
        public const int DataOffset = 0;

        public readonly static int IPv6AddressSize = GetIPv6AddressSize();
        public readonly static int IPv4AddressSize = GetIPv4AddressSize();

        private static unsafe int GetIPv6AddressSize()
        {
            int ipv6AddressSize, unused;
            Interop.Error err = Interop.Sys.GetIPSocketAddressSizes(&unused, &ipv6AddressSize);
            Debug.Assert(err == Interop.Error.SUCCESS);
            return ipv6AddressSize;
        }

        private static unsafe int GetIPv4AddressSize()
        {
            int ipv4AddressSize, unused;
            Interop.Error err = Interop.Sys.GetIPSocketAddressSizes(&ipv4AddressSize, &unused);
            Debug.Assert(err == Interop.Error.SUCCESS);
            return ipv4AddressSize;
        }

        private static void ThrowOnFailure(Interop.Error err)
        {
            switch (err)
            {
                case Interop.Error.SUCCESS:
                    return;

                case Interop.Error.EFAULT:
                    // The buffer was either null or too small.
                    throw new IndexOutOfRangeException();

                case Interop.Error.EAFNOSUPPORT:
                    // There was no appropriate mapping from the platform address family.
                    throw new PlatformNotSupportedException();

                default:
                    Debug.Fail("Unexpected failure in GetAddressFamily");
                    throw new PlatformNotSupportedException();
            }
        }

        public static unsafe AddressFamily GetAddressFamily(byte[] buffer)
        {
            AddressFamily family;
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.GetAddressFamily(rawAddress, buffer.Length, (int*)&family);
            }

            ThrowOnFailure(err);
            return family;
        }

        public static unsafe void SetAddressFamily(byte[] buffer, AddressFamily family)
        {
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.SetAddressFamily(rawAddress, buffer.Length, (int)family);
            }

            ThrowOnFailure(err);
        }

        public static unsafe ushort GetPort(byte[] buffer)
        {
            ushort port;
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.GetPort(rawAddress, buffer.Length, &port);
            }

            ThrowOnFailure(err);
            return port;
        }

        public static unsafe void SetPort(byte[] buffer, ushort port)
        {
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.SetPort(rawAddress, buffer.Length, port);
            }

            ThrowOnFailure(err);
        }

        public static unsafe uint GetIPv4Address(byte[] buffer)
        {
            uint ipAddress;
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.GetIPv4Address(rawAddress, buffer.Length, &ipAddress);
            }

            ThrowOnFailure(err);
            return ipAddress;
        }

        public static unsafe void GetIPv6Address(byte[] buffer, byte[] address, out uint scope)
        {
            uint localScope;
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            fixed (byte* ipAddress = address)
            {
                err = Interop.Sys.GetIPv6Address(rawAddress, buffer.Length, ipAddress, address.Length, &localScope);
            }

            ThrowOnFailure(err);
            scope = localScope;
        }

        public static unsafe void SetIPv4Address(byte[] buffer, uint address)
        {
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.SetIPv4Address(rawAddress, buffer.Length, address);
            }

            ThrowOnFailure(err);
        }

        public static unsafe void SetIPv4Address(byte[] buffer, byte* address)
        {
            uint addr = (uint)System.Runtime.InteropServices.Marshal.ReadInt32((IntPtr)address);
            SetIPv4Address(buffer, addr);
        }

        public static unsafe void SetIPv6Address(byte[] buffer, byte[] address, uint scope)
        {
            fixed (byte* rawInput = address)
            {
                SetIPv6Address(buffer, rawInput, address.Length, scope);
            }
        }

        public static unsafe void SetIPv6Address(byte[] buffer, byte* address, int addressLength, uint scope)
        {
            Interop.Error err;
            fixed (byte* rawAddress = buffer)
            {
                err = Interop.Sys.SetIPv6Address(rawAddress, buffer.Length, address, addressLength, scope);
            }

            ThrowOnFailure(err);
        }
    }
}
