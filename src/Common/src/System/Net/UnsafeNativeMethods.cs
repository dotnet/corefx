//------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net {
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using System.Net.Sockets;
    using System.Text;

    internal class IpHelperErrors
    {

        internal const uint Success = 0;
        internal const uint ErrorInvalidFunction = 1;
        internal const uint ErrorNoSuchDevice = 2;
        internal const uint ErrorInvalidData = 13;
        internal const uint ErrorInvalidParameter = 87;
        internal const uint ErrorBufferOverflow = 111;
        internal const uint ErrorInsufficientBuffer = 122;
        internal const uint ErrorNoData = 232;
        internal const uint Pending = 997;
        internal const uint ErrorNotFound = 1168;
    }

    internal static class UnsafeCommonNativeMethods {
        internal static class ErrorCodes
        {
            internal const uint ERROR_SUCCESS               = 0;
            internal const uint ERROR_HANDLE_EOF            = 38;
            internal const uint ERROR_NOT_SUPPORTED         = 50;
            internal const uint ERROR_INVALID_PARAMETER     = 87;
            internal const uint ERROR_ALREADY_EXISTS        = 183;
            internal const uint ERROR_MORE_DATA             = 234;
            internal const uint ERROR_OPERATION_ABORTED     = 995;
            internal const uint ERROR_IO_PENDING            = 997;
            internal const uint ERROR_NOT_FOUND             = 1168;
            internal const uint ERROR_CONNECTION_INVALID    = 1229;
        }

        internal enum NtStatus : uint
        {
            Success             = 0x00000000,
            InvalidParameter    = 0xc000000d
        }

#if !PROJECTN
        [DllImport(ExternDll.APIMSWINCOREHEAPOBSOLETEL1, ExactSpelling = true, SetLastError = true)]
        internal static extern SafeLocalFree LocalAlloc(int uFlags, UIntPtr sizetdwBytes);

        [DllImport(ExternDll.APIMSWINCOREHEAPOBSOLETEL1, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr LocalFree(IntPtr handle);

        [DllImport(ExternDll.APIMSWINCOREHEAPOBSOLETEL1, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GlobalFree(IntPtr handle);

        [DllImport(ExternDll.IPHLPAPI, ExactSpelling = true)]
        internal extern static uint GetNetworkParams(SafeLocalFree pFixedInfo, ref uint pOutBufLen);
#endif //PROJECTN

        [DllImport(ExternDll.NTDLL, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal extern static NtStatus RtlIpv6StringToAddressExW(
            [In] string s,
            [Out] byte[] address,
            [Out] out UInt32 scopeId,
            [Out] out UInt16 port);

        [DllImport(ExternDll.NTDLL, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal extern static NtStatus RtlIpv4StringToAddressExW(
            [In] string s,
            [In] bool strict,
            [Out] byte[] address,
            [Out] out UInt16 port);

        [DllImport(ExternDll.NTDLL, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal extern static NtStatus RtlIpv6AddressToStringExW(
            [In] byte[] address,
            [In] UInt32 scopeId,
            [In] UInt16 port,
            [In, Out] StringBuilder addressString,
            [In, Out] ref UInt32 addressStringLength);

        [DllImport(ExternDll.NTDLL, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal extern static NtStatus RtlIpv4AddressToStringExW(
            [In] byte[] address,
            [In] UInt16 port,
            [In, Out] StringBuilder addressString,
            [In, Out] ref UInt32 addressStringLength);

#if STRESS || !DEBUG

#if !PROJECTN
        [DllImport(ExternDll.APIMSWINCOREDEBUGL1, ExactSpelling = true)]
        internal static extern void DebugBreak();
#endif

#endif

        internal unsafe static class SecureStringHelper
        {
#if DEBUG
            // this method is only called as part of an assert
            internal static bool AreEqualValues(SecureString secureString1, SecureString secureString2)
            {
                if (secureString1 == null)
                {
                    if (secureString2 == null)
                        return true;
                    else
                        return false;
                }
                else if (secureString2 == null)
                {
                    return false;
                }

                // strings are non-null at this point

                if ((object)secureString1 == (object)secureString2)
                    return true;  // same objects

                if (secureString1.Length != secureString2.Length)
                    return false;

                // strings are same length.  decrypt to unmanaged memory and compare them.

                var string1 = CreateString(secureString1);
                var string2 = CreateString(secureString2);
                
                return (String.Compare(string1, string2, StringComparison.Ordinal) == 0);
            }
#endif

            internal static string CreateString(SecureString secureString)
            {
                string plainString;
                IntPtr bstr = IntPtr.Zero;

                if (secureString == null || secureString.Length == 0)
                {
                    return String.Empty;
                }

#if !PROJECTN
                IntPtr plainData = IntPtr.Zero;
                try
                {
                    plainData = SecureStringMarshal.SecureStringToCoTaskMemUnicode(secureString);
                    plainString = Marshal.PtrToStringUni(plainData);
                }
                finally
                {
                    if (plainData != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeCoTaskMemUnicode(plainData);
                    }
                }
#else
                plainString = secureString.GetInsecureString();
#endif
                return plainString;
            }

            internal static SecureString CreateSecureString(string plainString)
            {
                SecureString secureString;

                if (plainString == null || plainString.Length == 0)
                    return new SecureString();

                fixed (char* pch = plainString)
                {
                    secureString = new SecureString(pch, plainString.Length);
                }

                return secureString;
            }
        }
    }
}
