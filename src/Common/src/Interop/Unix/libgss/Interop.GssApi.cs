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
            Interop.libgssapi.GssFlags inFlags,
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

            using (SafeGssBufferHandle outputToken = new SafeGssBufferHandle())
            {
                libgssapi.Status status;
                libgssapi.Status minorStatus;

                status = libgssapi.GssInitSecContext(
                                    out minorStatus,
                                    credential,
                                    ref context,
                                    isNtlm,
                                    targetName,
                                    (uint)inFlags,
                                    buffer,
                                    (buffer == null) ? 0 : buffer.Length,
                                    outputToken,
                                    out outFlags);

                if ((status != libgssapi.Status.GSS_S_COMPLETE) && (status != libgssapi.Status.GSS_S_CONTINUE_NEEDED))
                {
                    throw libgssapi.GssApiException.Create(SR.net_context_establishment_failed, status, minorStatus);
                }

                outputBuffer = new byte[outputToken.Length]; // Always return non-null
                if (outputToken.Length > 0)
                {
                    Marshal.Copy(outputToken.Value, outputBuffer, 0, outputToken.Length);
                }
                return (status == libgssapi.Status.GSS_S_COMPLETE) ? true : false;
            }
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

            using (SafeGssBufferHandle outputToken = new SafeGssBufferHandle())
            {
                libgssapi.Status minorStatus;
                libgssapi.Status status = libgssapi.GssWrap(out minorStatus, context, encrypt, buffer, offset, count, outputToken);
                if (status != libgssapi.Status.GSS_S_COMPLETE)
                {
                    throw libgssapi.GssApiException.Create(SR.net_context_wrap_failed, status, minorStatus);
                }

                outputBuffer = new byte[outputToken.Length]; // Always return non-null
                if (outputToken.Length > 0)
                {
                    Marshal.Copy(outputToken.Value, outputBuffer, 0, outputToken.Length);
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

            using (SafeGssBufferHandle outputToken = new SafeGssBufferHandle())
            {
                libgssapi.Status minorStatus;
                libgssapi.Status status = libgssapi.GssUnwrap(out minorStatus, context, buffer, offset, count, outputToken);
                if (status != libgssapi.Status.GSS_S_COMPLETE)
                {
                    throw libgssapi.GssApiException.Create(SR.net_context_unwrap_failed, status, minorStatus);
                }

                int length = buffer.Length - offset;
                if (outputToken.Length > length)
                {
                    throw libgssapi.GssApiException.Create(SR.Format(SR.net_context_buffer_too_small, outputToken.Length, length));
                }

                if (outputToken.Length > 0)
                {
                    Marshal.Copy(outputToken.Value, buffer, offset, outputToken.Length);
                }

                return outputToken.Length;
            }
        }

        internal static string GetSourceName(SafeGssContextHandle context)
        {
            libgssapi.Status minorStatus;
            SafeGssNameHandle sourceName;
            libgssapi.Status status = libgssapi.GssInquireSourceName(
                         out minorStatus,
                         context,
                         out sourceName);

            if (status != libgssapi.Status.GSS_S_COMPLETE)
            {
                throw libgssapi.GssApiException.Create(status, minorStatus);
            }

            using (sourceName)
            using (SafeGssBufferHandle outputBuffer = new SafeGssBufferHandle())
            {
                status = libgssapi.GssDisplayName(out minorStatus, sourceName, outputBuffer);
                if (status != libgssapi.Status.GSS_S_COMPLETE)
                {
                    throw libgssapi.GssApiException.Create(status, minorStatus);
                }

                // String may not be NULL terminated so PtrToStringAnsi cannot be used
                unsafe
                {
                    return Encoding.UTF8.GetString((byte*)outputBuffer.Value.ToPointer(), outputBuffer.Length);
                }
            }
        }
    }
}


