// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static class HeimdalNtlm
    {
        internal static byte[] CreateNegotiateMessage(uint flags)
        {
            SafeNtlmBufferHandle data;
            int dataLen;
            int status = NetSecurity.HeimNtlmEncodeType1(flags, out data, out dataLen);
            NetSecurity.HeimdalNtlmException.AssertOrThrowIfError("HeimNtlmEncodeType1 failed", status);
            using (data)
            {
                return data.ToByteArray(dataLen,0);
            }
        }

        internal static byte[] CreateAuthenticateMessage(uint flags, string username, string password, string domain,
                                                         byte[] type2Data, int offset, int count, out byte[] sessionKey)
        {
            using (SafeNtlmType3Handle challengeMessage = new SafeNtlmType3Handle(type2Data, offset, count))
            {
                return  challengeMessage.GetResponse(flags, username, password, domain, out sessionKey);
            }
        }

        internal static void CreateKeys(byte[] sessionKey, out SafeNtlmKeyHandle serverSignKey, out SafeNtlmKeyHandle serverSealKey, out SafeNtlmKeyHandle clientSignKey, out SafeNtlmKeyHandle clientSealKey)
        {
            serverSignKey = new SafeNtlmKeyHandle(sessionKey, false, false);
            serverSealKey = new SafeNtlmKeyHandle(sessionKey, false, true);
            clientSignKey = new SafeNtlmKeyHandle(sessionKey, true, false);
            clientSealKey = new SafeNtlmKeyHandle(sessionKey, true, true);
        }
    }
}

