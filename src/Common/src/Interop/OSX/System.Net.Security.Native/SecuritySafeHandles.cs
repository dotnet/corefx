// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
    internal sealed partial class SafeFreeNegoCredentials : SafeFreeCredentials
    {

        public SafeFreeNegoCredentials(bool ntlmOnly, string username, string password, string domain) : base(IntPtr.Zero, true)
        {
            _isNtlm = ntlmOnly;
            _isdefault = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);
            _username = username;
            _domain = domain;
            _credential = SafeGssCredHandle.Create(username, password, domain);
        }
    }

    internal sealed partial class SafeDeleteNegoContext : SafeDeleteContext
    {
        private const char At = '@';
        public SafeDeleteNegoContext(SafeFreeNegoCredentials credential)
            : base(credential)
        {
            // Try to construct target in user@domain format
            string targetName = credential.UserName;
            string domain = credential.Domain;

            //remove any leading and trailing whitespace
            if (domain != null)
            {
                domain = domain.Trim();
            }

            if ((targetName.IndexOf(At) < 0) && !string.IsNullOrEmpty(domain))
            {
                targetName += At + domain;
            }

            try
            {
                _targetName = SafeGssNameHandle.CreatePrincipal(targetName);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public byte[] MakeClientSignature(byte[] buffer, int offset, int count)
        {
            //MakeClientSignature is not supported on OSX
            throw new PlatformNotSupportedException();
        }

        public byte[] MakeServerSignature(byte[] buffer, int offset, int count)
        {
            //MakeServerSignature is not supported on OSX
            throw new PlatformNotSupportedException();
        }

        public byte[] Encrypt(byte[] buffer, int offset, int count)
        {
            //Encrypt is not supported on OSX
            throw new PlatformNotSupportedException();
        }

        public byte[] Decrypt(byte[] buffer, int offset, int count)
        {
            //Decrypt is not supported on OSX
            throw new PlatformNotSupportedException();
        }
    }
}
