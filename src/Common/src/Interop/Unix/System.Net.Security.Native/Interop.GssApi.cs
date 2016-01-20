// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static class GssApi
    {
        internal static bool EstablishSecurityContext(
            ref SafeGssContextHandle context,
            SafeGssCredHandle credential,
            bool isNtlm,
            SafeGssNameHandle targetName,
            Interop.NetSecurity.GssFlags inFlags,
            byte[] buffer,
            out byte[] outputBuffer,
            out uint outFlags)
        {
            outputBuffer = null;
            outFlags = 0;

            if (context == null)
            {
                context = new SafeGssContextHandle();
            }

            SafeGssBufferHandle outputToken;
            NetSecurity.Status status;
            NetSecurity.Status minorStatus;
            int outputLength;
            status = NetSecurity.InitSecContext(out minorStatus,
                                                credential,
                                                ref context,
                                                isNtlm,
                                                targetName,
                                                (uint)inFlags,
                                                buffer,
                                                (buffer == null) ? 0 : buffer.Length,
                                                out outputToken,
                                                out outputLength,
                                                out outFlags);

            using (outputToken)
            {
                if ((status != NetSecurity.Status.GSS_S_COMPLETE) && (status != NetSecurity.Status.GSS_S_CONTINUE_NEEDED))
                {
                    throw new NetSecurity.GssApiException(SR.net_context_establishment_failed, status, minorStatus);
                }

                outputBuffer = new byte[outputLength];
                if (outputLength > 0)
                {
                    Interop.NetSecurity.CopyBuffer(outputToken, outputBuffer, 0);
                }
            }
            return (status == NetSecurity.Status.GSS_S_COMPLETE) ? true : false;
        }

        internal static int Encrypt(
            SafeGssContextHandle context,
            bool encrypt,
            byte[] buffer,
            int offset,
            int count,
            out byte[] outputBuffer)
        {
            outputBuffer = null;
            Debug.Assert((buffer != null) && (buffer.Length > 0), "Invalid input buffer passed to Encrypt");
            Debug.Assert((offset >= 0) && (offset < buffer.Length), "Invalid input offset passed to Encrypt");
            Debug.Assert((count > 0) && (count <= (buffer.Length - offset)), "Invalid input count passed to Encrypt");

            SafeGssBufferHandle outputToken;
            int msgLength;
            NetSecurity.Status minorStatus;
            NetSecurity.Status status = NetSecurity.Wrap(out minorStatus, context, encrypt, buffer, offset, count, out outputToken, out msgLength);
            using (outputToken)
            {
                if (status != NetSecurity.Status.GSS_S_COMPLETE)
                {
                    throw new NetSecurity.GssApiException(SR.net_context_wrap_failed, status, minorStatus);
                }
                outputBuffer = new byte[msgLength]; // Always return non-null
                if (msgLength > 0)
                {
                    NetSecurity.CopyBuffer(outputToken, outputBuffer, 0);
                }

                return outputBuffer.Length;
            }
        }

        internal static int Decrypt(
            SafeGssContextHandle context,
            byte[] buffer,
            int offset,
            int count)
        {
            Debug.Assert((buffer != null) && (buffer.Length > 0), "Invalid input buffer passed to Decrypt");
            Debug.Assert((offset >= 0) && (offset < buffer.Length), "Invalid input offset passed to Decrypt");
            Debug.Assert((count > 0) && (count <= (buffer.Length - offset)), "Invalid input count passed to Decrypt");

            SafeGssBufferHandle outputToken;
            int msgLength;
            NetSecurity.Status minorStatus;
            NetSecurity.Status status = NetSecurity.Unwrap(out minorStatus, context, buffer, offset, count, out outputToken, out msgLength);

            using (outputToken)
            {
                if (status != NetSecurity.Status.GSS_S_COMPLETE)
                {
                    throw new NetSecurity.GssApiException(SR.net_context_unwrap_failed, status, minorStatus);
                }

                int length = msgLength - offset;
                if (msgLength > length)
                {
                    throw new NetSecurity.GssApiException(SR.Format(SR.net_context_buffer_too_small, msgLength, length));
                }

                if (msgLength > 0)
                {
                    NetSecurity.CopyBuffer(outputToken, buffer, offset);
                }
            }

            return msgLength;
        }
    }
}


