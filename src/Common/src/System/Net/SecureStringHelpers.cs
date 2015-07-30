// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Security;

namespace System.Net
{
    internal static class SecureStringHelpers
    {
        internal static bool AreEqualValues(SecureString left, SecureString right)
        {
            if (left == null)
            {
                return right == null;
            }

            if ((object)left == (object)right)
            {
                return true;
            }

            if (left.Length == right.Length)
            {
                return GetPlaintext(left).Equals(GetPlaintext(right), StringComparison.Ordinal);
            }

            return false;
        }

        internal static string GetPlaintext(SecureString secureString)
        {
            if (secureString == null || secureString.Length == 0)
            {
                return string.Empty;
            }

            string plaintext;

#if !NETNative
            IntPtr plainData = IntPtr.Zero;
            try
            {
                plainData = SecureStringMarshal.SecureStringToCoTaskMemUnicode(secureString);
                plaintext = Marshal.PtrToStringUni(plainData);
            }
            finally
            {
                if (plainData != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(plainData);
                }
            }
#else
            plaintext = secureString.GetInsecureString();
#endif

            return plaintext;
        }

        internal static SecureString CreateSecureString(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return new SecureString();
            }

            unsafe
            {
                fixed (char* plainData = plaintext)
                {
                    return new SecureString(plainData, plaintext.Length);
                }
            }
        }
    }
}
