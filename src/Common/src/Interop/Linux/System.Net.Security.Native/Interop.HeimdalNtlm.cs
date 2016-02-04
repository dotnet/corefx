// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static class HeimdalNtlm
    {
        internal static byte[] CreateNegotiateMessage(uint flags)
        {
            NetSecurityNative.NtlmBuffer buffer = default(NetSecurityNative.NtlmBuffer);
            try
            {
                int status = NetSecurityNative.HeimNtlmEncodeType1(flags, ref buffer);
                NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                MockUtils.MockLogging.PrintInfo("kapilash", "created negotiate message");
                return buffer.ToByteArray();
            }
            finally
            {
                buffer.Dispose();
            }
        }

        internal static byte[] CreateAuthenticateMessage(uint flags, string username, string password, string domain,
                                                         byte[] type2Data, int offset, int count, out byte[] sessionKey)
        {
            using (SafeNtlmType3Handle challengeMessage = new SafeNtlmType3Handle(type2Data, offset, count))
            {
                return challengeMessage.GetResponse(flags, username, password, domain, out sessionKey);
            }
        }

        internal static void CreateKeys(byte[] sessionKey, out SafeNtlmKeyHandle serverSignKey, out SafeNtlmKeyHandle serverSealKey, out SafeNtlmKeyHandle clientSignKey, out SafeNtlmKeyHandle clientSealKey)
        {
            serverSignKey = new SafeNtlmKeyHandle(sessionKey, isClient: false, isSealingKey: false);
            serverSealKey = new SafeNtlmKeyHandle(sessionKey, isClient: false, isSealingKey: true);
            clientSignKey = new SafeNtlmKeyHandle(sessionKey, isClient: true, isSealingKey: false);
            clientSealKey = new SafeNtlmKeyHandle(sessionKey, isClient: true, isSealingKey: true);
        }
    }
}

