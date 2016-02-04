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
    internal static partial class HeimdalNtlm
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

                int status = Interop.NetSecurityNative.HeimNtlmDecodeType2(type2Data, offset, count, out _type2Handle);
                Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
            }

            public byte[] GetResponse(uint flags, string username, string password, string domain,
                                      out byte[] sessionKey)
            {
                Debug.Assert(username != null, "username cannot be null");
                Debug.Assert(password != null, "password cannot be null");
                Debug.Assert(domain != null,  "domain cannot be null");
                MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.1");

                // reference for NTLM response: https://msdn.microsoft.com/en-us/library/cc236700.aspx

                sessionKey = null;
                Interop.NetSecurityNative.NtlmBuffer key = default(Interop.NetSecurityNative.NtlmBuffer);
                Interop.NetSecurityNative.NtlmBuffer lmResponse = default(Interop.NetSecurityNative.NtlmBuffer);
                Interop.NetSecurityNative.NtlmBuffer ntResponse = default(Interop.NetSecurityNative.NtlmBuffer);
                Interop.NetSecurityNative.NtlmBuffer sessionKeyBuffer = default(Interop.NetSecurityNative.NtlmBuffer);
                Interop.NetSecurityNative.NtlmBuffer outputData = default(Interop.NetSecurityNative.NtlmBuffer);

                try
                {
                    int status = Interop.NetSecurityNative.HeimNtlmNtKey(password, ref key);
                    Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                    MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.2");

                    byte[] baseSessionKey = new byte[Interop.NetSecurityNative.MD5DigestLength];
                    status = Interop.NetSecurityNative.HeimNtlmCalculateResponse(true, ref key, _type2Handle, username, domain,
                             baseSessionKey, baseSessionKey.Length, ref lmResponse);
                    Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                    MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.3");

                    status = Interop.NetSecurityNative.HeimNtlmCalculateResponse(false, ref key, _type2Handle, username, domain,
                                                                           baseSessionKey, baseSessionKey.Length, ref ntResponse);
                    Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                    MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.4");

                    status = Interop.NetSecurityNative.CreateType3Message(ref key, _type2Handle, username, domain, flags,
                        ref lmResponse, ref ntResponse, baseSessionKey,baseSessionKey.Length, ref sessionKeyBuffer, ref outputData);
                    Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                    MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.5");

                    sessionKey = sessionKeyBuffer.ToByteArray();
                    return outputData.ToByteArray();
                }
                finally
                {
                    key.Dispose();
                    lmResponse.Dispose();
                    ntResponse.Dispose();
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
