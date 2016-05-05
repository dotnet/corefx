// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_KEY_TRANS_RECIPIENT_ENCODE_INFO
        {
            internal int cbSize;
            internal CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
            internal IntPtr pvKeyEncryptionAuxInfo;
            internal IntPtr hCryptProv;
            internal CRYPT_BIT_BLOB RecipientPublicKey;
            internal CERT_ID RecipientId;
        }
    }
}

