// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace System.Net.Security
{
    internal sealed partial class SafeFreeNegoCredentials : SafeFreeCredentials
    {
        private readonly string _password;

        public string Password
        {
            get { return _password; }
        }

        public SafeFreeNegoCredentials(bool ntlmOnly, string username, string password, string domain) : base(IntPtr.Zero, true)
        {
            _isNtlm = ntlmOnly;
            _isdefault = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);
            if (!ntlmOnly)
            {
                try
                {
                    _credential = SafeGssCredHandle.Create(username, password, domain);
                }
                catch
                {
                    // NTLM fallback is not possible with default credentials
                    if (_isdefault)
                    {
                        throw new PlatformNotSupportedException(SR.net_ntlm_not_possible_default_cred);
                    }

                    _isNtlm = true;
                }
            }

            // Even if Kerberos TGT could be obtained, we might later need
            // to fall back to NTLM if service ticket cannot be fetched
            _username = username;
            _password = password;
            _domain = domain;
        }
    }

    internal sealed partial class SafeDeleteNegoContext : SafeDeleteContext
    {
        private readonly Interop.NetNtlmNative.NtlmFlags _flags;
        private Interop.HeimdalNtlm.SigningKey _serverSignKey;
        private Interop.HeimdalNtlm.SealingKey _serverSealKey;
        private Interop.HeimdalNtlm.SigningKey _clientSignKey;
        private Interop.HeimdalNtlm.SealingKey _clientSealKey;

        public Interop.NetNtlmNative.NtlmFlags Flags
        {
            get { return _flags; }
        }

        public SafeDeleteNegoContext(SafeFreeNegoCredentials credential, Interop.NetNtlmNative.NtlmFlags flags)
            : base(credential)
        {
            _flags = flags;
            _isNtlm = true;
        }

        public void SetKeys(byte[] sessionKey)
        {
            Interop.HeimdalNtlm.CreateKeys(sessionKey, out _serverSignKey, out _serverSealKey, out _clientSignKey, out _clientSealKey);
        }

        public byte[] MakeClientSignature(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_clientSignKey != null, "_clientSignKey cannot be null");
            return _clientSignKey.Sign(_clientSealKey, buffer, offset, count);
        }

        public byte[] MakeServerSignature(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_serverSignKey != null, "_serverSignKey cannot be null");
            return _serverSignKey.Sign(_serverSealKey, buffer, offset, count);
        }

        public byte[] Encrypt(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_clientSignKey != null, "_clientSealKey cannot be null");
            return _clientSealKey.SealOrUnseal(buffer, offset, count);
        }

        public byte[] Decrypt(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_serverSignKey != null, "_serverSealKey cannot be null");
            return _serverSealKey.SealOrUnseal(buffer, offset, count);
        }
    }
}
