//------------------------------------------------------------------------------
// <copyright file="IPAddressParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net
{
    using System.Net.Sockets;
    using System.Text;

    internal class IPAddressParser
    {
        internal const int IPv4AddressBytes = 4;
        internal const int IPv6AddressBytes = 16;

        internal const int INET_ADDRSTRLEN = 22;
        internal const int INET6_ADDRSTRLEN = 65;

        internal static IPAddress Parse(string ipString, bool tryParse)
        {
            if (ipString == null)
            {
                if (tryParse)
                {
                    return null;
                }
                throw new ArgumentNullException("ipString");
            }

            UnsafeCommonNativeMethods.NtStatus error = 0;

            //
            // IPv6 Changes: Detect probable IPv6 addresses and use separate
            //               parse method.
            //
            if (ipString.IndexOf(':') != -1)
            {
                //
                // If the address string contains the colon character
                // then it can only be an IPv6 address. Use a separate
                // parse method to unpick it all. Note: we don't support
                // port specification at the end of address and so can
                // make this decision.
                //
                uint scope = 0;
                ushort port = 0;
                byte[] bytes = new byte[IPv6AddressBytes];

                error = UnsafeCommonNativeMethods.RtlIpv6StringToAddressExW(
                    ipString,
                    bytes,
                    out scope,
                    out port);

                if (error == UnsafeCommonNativeMethods.NtStatus.Success)
                {
                    // AppCompat: .Net 4.5 ignores a correct port if the address was specified in brackes.
                    // Will still throw for an incorrect port.

                    return new IPAddress(bytes, (long)scope);
                }
            }
            else
            {
                ushort port = 0;
                byte[] bytes = new byte[IPv4AddressBytes];

                error = UnsafeCommonNativeMethods.RtlIpv4StringToAddressExW(
                    ipString,
                    false,
                    bytes,
                    out port);

                if (error == 0)
                {
                    if (port != 0)
                    {
                        throw new FormatException(SR.dns_bad_ip_address);
                    }

                    return new IPAddress(bytes);
                }
            }

            if (tryParse)
            {
                return null;
            }

            Exception e = new SocketException(NtStatusToSocketErrorAdapter(error));
            throw new FormatException(SR.dns_bad_ip_address, e);
        }

        internal static string IPv4AddressToString(byte[] numbers)
        {
            UnsafeCommonNativeMethods.NtStatus errorCode = 0;

            StringBuilder sb = new StringBuilder(INET_ADDRSTRLEN);
            uint length = (uint)sb.Capacity;

            errorCode = UnsafeCommonNativeMethods.RtlIpv4AddressToStringExW(
                numbers,
                0,
                sb,
                ref length);

            if (errorCode == UnsafeCommonNativeMethods.NtStatus.Success)
            {
                return sb.ToString();
            }
            else
            {
                throw new SocketException(NtStatusToSocketErrorAdapter(errorCode));
            }
        }

        internal static string IPv6AddressToString(byte[] numbers, UInt32 scopeId)
        {
            UnsafeCommonNativeMethods.NtStatus  errorCode = 0;

            StringBuilder sb = new StringBuilder(INET6_ADDRSTRLEN);
            uint length = (uint)sb.Capacity;

            errorCode = UnsafeCommonNativeMethods.RtlIpv6AddressToStringExW(
                numbers,
                scopeId,
                0,
                sb,
                ref length);

            if (errorCode == UnsafeCommonNativeMethods.NtStatus.Success)
            {
                return sb.ToString();
            }
            else
            {
                throw new SocketException(NtStatusToSocketErrorAdapter(errorCode));
            }
        }

        private static SocketError NtStatusToSocketErrorAdapter(UnsafeCommonNativeMethods.NtStatus status)
        {
            switch (status)
            {
                case UnsafeCommonNativeMethods.NtStatus.Success:
                    return SocketError.Success;
                case UnsafeCommonNativeMethods.NtStatus.InvalidParameter:
                    return SocketError.InvalidArgument;
                default:
                    return (SocketError)status;
            }
        }
    }
}
