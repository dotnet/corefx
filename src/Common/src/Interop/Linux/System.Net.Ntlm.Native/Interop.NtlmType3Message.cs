// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Ntlm
    {
        /// <summary>
        /// Provides ntlm_type3 message
        /// </summary>
        internal sealed class NtlmType3Message : IDisposable
        {
            private SafeNtlmType2Handle _type2Handle;

            public NtlmType3Message(byte[] type2Data, int offset, int count)
            {
                Debug.Assert(type2Data != null, "type2Data cannot be null");
                Debug.Assert(offset >= 0 && offset <= type2Data.Length, "offset must be a valid value");
                Debug.Assert(count >= 0 , " count must be a valid value");
                Debug.Assert(type2Data.Length >= offset + count, " count and offset must match the given buffer");

                int status = Interop.NetNtlmNative.NtlmDecodeType2(type2Data, offset, count, out _type2Handle);
                Interop.NetNtlmNative.NtlmException.ThrowIfError(status);
            }

            public byte[] GetResponse(uint flags, string username, string password, string domain,
                                      out byte[] sessionKey)
            {
                Debug.Assert(username != null, "username cannot be null");
                Debug.Assert(password != null, "password cannot be null");
                Debug.Assert(domain != null,  "domain cannot be null");

                sessionKey = null;
                Interop.NetNtlmNative.NtlmBuffer sessionKeyBuffer = default(Interop.NetNtlmNative.NtlmBuffer);
                Interop.NetNtlmNative.NtlmBuffer outputData = default(Interop.NetNtlmNative.NtlmBuffer);

                try
                {
                    int status = Interop.NetNtlmNative.CreateType3Message(password, _type2Handle, username, domain, flags,
                        ref sessionKeyBuffer, ref outputData);
                    Interop.NetNtlmNative.NtlmException.ThrowIfError(status);

                    sessionKey = sessionKeyBuffer.ToByteArray();
                    return outputData.ToByteArray();
                }
                finally
                {
                    sessionKeyBuffer.Dispose();
                    outputData.Dispose();
                }
            }

            public void Dispose()
            {
                if (_type2Handle != null)
                {
                    _type2Handle.Dispose();
                    _type2Handle = null;
                }
            }
        }
    }
}
