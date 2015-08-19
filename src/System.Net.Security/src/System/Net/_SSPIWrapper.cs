// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Net.Sockets;
using System.Globalization;
using System.Net.Security;

namespace System.Net
{
    internal static class SSPIWrapper
    {
        internal static SecurityPackageInfoClass[] EnumerateSecurityPackages(SSPIInterface SecModule)
        {
            GlobalLog.Enter("EnumerateSecurityPackages");
            if (SecModule.SecurityPackages == null)
            {
                lock (SecModule)
                {
                    if (SecModule.SecurityPackages == null)
                    {
                        int moduleCount = 0;
                        SafeFreeContextBuffer arrayBaseHandle = null;
                        try
                        {
                            int errorCode = SecModule.EnumerateSecurityPackages(out moduleCount, out arrayBaseHandle);
                            GlobalLog.Print("SSPIWrapper::arrayBase: " + (arrayBaseHandle.DangerousGetHandle().ToString("x")));
                            if (errorCode != 0)
                            {
                                throw new Win32Exception(errorCode);
                            }
                            SecurityPackageInfoClass[] securityPackages = new SecurityPackageInfoClass[moduleCount];
                            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.net_log_sspi_enumerating_security_packages);
                            int i;
                            for (i = 0; i < moduleCount; i++)
                            {
                                securityPackages[i] = new SecurityPackageInfoClass(arrayBaseHandle, i);
                                if (Logging.On) Logging.PrintInfo(Logging.Web, "    " + securityPackages[i].Name);
                            }
                            SecModule.SecurityPackages = securityPackages;
                        }
                        finally
                        {
                            if (arrayBaseHandle != null)
                            {
                                arrayBaseHandle.Dispose();
                            }
                        }
                    }
                }
            }
            GlobalLog.Leave("EnumerateSecurityPackages");
            return SecModule.SecurityPackages;
        }

        internal static SecurityPackageInfoClass GetVerifyPackageInfo(SSPIInterface secModule, string packageName)
        {
            return GetVerifyPackageInfo(secModule, packageName, false);
        }

        internal static SecurityPackageInfoClass GetVerifyPackageInfo(SSPIInterface secModule, string packageName, bool throwIfMissing)
        {
            SecurityPackageInfoClass[] supportedSecurityPackages = EnumerateSecurityPackages(secModule);
            if (supportedSecurityPackages != null)
            {
                for (int i = 0; i < supportedSecurityPackages.Length; i++)
                {
                    if (string.Compare(supportedSecurityPackages[i].Name, packageName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return supportedSecurityPackages[i];
                    }
                }
            }

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_sspi_security_package_not_found, packageName));

            // error
            if (throwIfMissing)
            {
                throw new NotSupportedException(SR.net_securitypackagesupport);
            }

            return null;
        }

        public static SafeFreeCredentials AcquireDefaultCredential(SSPIInterface SecModule, string package, Interop.CredentialUse intent)
        {
            GlobalLog.Print("SSPIWrapper::AcquireDefaultCredential(): using " + package);
            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "AcquireDefaultCredential(" +
    "package = " + package + ", " +
    "intent  = " + intent + ")");


            SafeFreeCredentials outCredential = null;
            int errorCode = SecModule.AcquireDefaultCredential(package, intent, out outCredential);

            if (errorCode != 0)
            {
#if TRAVE
                GlobalLog.Print("SSPIWrapper::AcquireDefaultCredential(): error " + SecureChannel.MapSecurityStatus((uint)errorCode));
#endif
                if (Logging.On) Logging.PrintError(Logging.Web, SR.Format(SR.net_log_operation_failed_with_error, "AcquireDefaultCredential()", String.Format(CultureInfo.CurrentCulture, "0X{0:X}", errorCode)));
                throw new Win32Exception(errorCode);
            }
            return outCredential;
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(SSPIInterface SecModule, string package, Interop.CredentialUse intent, ref Interop.AuthIdentity authdata)
        {
            GlobalLog.Print("SSPIWrapper::AcquireCredentialsHandle#2(): using " + package);

            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "AcquireCredentialsHandle(" +
    "package  = " + package + ", " +
    "intent   = " + intent + ", " +
    "authdata = " + authdata + ")");

            SafeFreeCredentials credentialsHandle = null;
            int errorCode = SecModule.AcquireCredentialsHandle(package,
                                                               intent,
                                                               ref authdata,
                                                               out credentialsHandle
                                                               );

            if (errorCode != 0)
            {
#if TRAVE
                GlobalLog.Print("SSPIWrapper::AcquireCredentialsHandle#2(): error " + SecureChannel.MapSecurityStatus((uint)errorCode));
#endif
                if (Logging.On) Logging.PrintError(Logging.Web, SR.Format(SR.net_log_operation_failed_with_error, "AcquireCredentialsHandle()", String.Format(CultureInfo.CurrentCulture, "0X{0:X}", errorCode)));
                throw new Win32Exception(errorCode);
            }
            return credentialsHandle;
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(SSPIInterface SecModule, string package, Interop.CredentialUse intent, ref SafeSspiAuthDataHandle authdata)
        {
            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "AcquireCredentialsHandle(" +
    "package  = " + package + ", " +
    "intent   = " + intent + ", " +
    "authdata = " + authdata + ")");

            SafeFreeCredentials credentialsHandle = null;
            int errorCode = SecModule.AcquireCredentialsHandle(package, intent, ref authdata, out credentialsHandle);

            if (errorCode != 0)
            {
                if (Logging.On) Logging.PrintError(Logging.Web, SR.Format(SR.net_log_operation_failed_with_error, "AcquireCredentialsHandle()", String.Format(CultureInfo.CurrentCulture, "0X{0:X}", errorCode)));
                throw new Win32Exception(errorCode);
            }
            return credentialsHandle;
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(SSPIInterface SecModule, string package, Interop.CredentialUse intent, Interop.SecureCredential scc)
        {
            GlobalLog.Print("SSPIWrapper::AcquireCredentialsHandle#3(): using " + package);

            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "AcquireCredentialsHandle(" +
    "package = " + package + ", " +
    "intent  = " + intent + ", " +
    "scc     = " + scc + ")");

            SafeFreeCredentials outCredential = null;
            int errorCode = SecModule.AcquireCredentialsHandle(
                                            package,
                                            intent,
                                            ref scc,
                                            out outCredential
                                            );
            if (errorCode != 0)
            {
#if TRAVE
                GlobalLog.Print("SSPIWrapper::AcquireCredentialsHandle#3(): error " + SecureChannel.MapSecurityStatus((uint)errorCode));
#endif
                if (Logging.On) Logging.PrintError(Logging.Web, SR.Format(SR.net_log_operation_failed_with_error, "AcquireCredentialsHandle()", String.Format(CultureInfo.CurrentCulture, "0X{0:X}", errorCode)));
                throw new Win32Exception(errorCode);
            }

#if TRAVE
            GlobalLog.Print("SSPIWrapper::AcquireCredentialsHandle#3(): cred handle = " + outCredential.ToString());
#endif
            return outCredential;
        }

        internal static int InitializeSecurityContext(SSPIInterface SecModule, ref SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, Interop.ContextFlags inFlags, Interop.Endianness datarep, Interop.SecurityBuffer inputBuffer, Interop.SecurityBuffer outputBuffer, ref Interop.ContextFlags outFlags)
        {
            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "InitializeSecurityContext(" +
    "credential = " + credential.ToString() + ", " +
    "context = " + Logging.ObjectToString(context) + ", " +
    "targetName = " + targetName + ", " +
    "inFlags = " + inFlags + ")");

            int errorCode = SecModule.InitializeSecurityContext(ref credential, ref context, targetName, inFlags, datarep, inputBuffer, outputBuffer, ref outFlags);

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_sspi_security_context_input_buffer, "InitializeSecurityContext", (inputBuffer == null ? 0 : inputBuffer.size), outputBuffer.size, (SecurityStatus)errorCode));

            return errorCode;
        }

        internal static int InitializeSecurityContext(SSPIInterface SecModule, SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, Interop.ContextFlags inFlags, Interop.Endianness datarep, Interop.SecurityBuffer[] inputBuffers, Interop.SecurityBuffer outputBuffer, ref Interop.ContextFlags outFlags)
        {
            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "InitializeSecurityContext(" +
    "credential = " + credential.ToString() + ", " +
    "context = " + Logging.ObjectToString(context) + ", " +
    "targetName = " + targetName + ", " +
    "inFlags = " + inFlags + ")");

            int errorCode = SecModule.InitializeSecurityContext(credential, ref context, targetName, inFlags, datarep, inputBuffers, outputBuffer, ref outFlags);

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_sspi_security_context_input_buffers, "InitializeSecurityContext", (inputBuffers == null ? 0 : inputBuffers.Length), outputBuffer.size, (SecurityStatus)errorCode));

            return errorCode;
        }

        internal static int AcceptSecurityContext(SSPIInterface SecModule, ref SafeFreeCredentials credential, ref SafeDeleteContext context, Interop.ContextFlags inFlags, Interop.Endianness datarep, Interop.SecurityBuffer inputBuffer, Interop.SecurityBuffer outputBuffer, ref Interop.ContextFlags outFlags)
        {
            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "AcceptSecurityContext(" +
    "credential = " + credential.ToString() + ", " +
    "context = " + Logging.ObjectToString(context) + ", " +
    "inFlags = " + inFlags + ")");

            int errorCode = SecModule.AcceptSecurityContext(ref credential, ref context, inputBuffer, inFlags, datarep, outputBuffer, ref outFlags);

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_sspi_security_context_input_buffer, "AcceptSecurityContext", (inputBuffer == null ? 0 : inputBuffer.size), outputBuffer.size, (SecurityStatus)errorCode));

            return errorCode;
        }

        internal static int AcceptSecurityContext(SSPIInterface SecModule, SafeFreeCredentials credential, ref SafeDeleteContext context, Interop.ContextFlags inFlags, Interop.Endianness datarep, Interop.SecurityBuffer[] inputBuffers, Interop.SecurityBuffer outputBuffer, ref Interop.ContextFlags outFlags)
        {
            if (Logging.On)
                Logging.PrintInfo(Logging.Web,
    "AcceptSecurityContext(" +
    "credential = " + credential.ToString() + ", " +
    "context = " + Logging.ObjectToString(context) + ", " +
    "inFlags = " + inFlags + ")");

            int errorCode = SecModule.AcceptSecurityContext(credential, ref context, inputBuffers, inFlags, datarep, outputBuffer, ref outFlags);

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_sspi_security_context_input_buffers, "AcceptSecurityContext", (inputBuffers == null ? 0 : inputBuffers.Length), outputBuffer.size, (SecurityStatus)errorCode));

            return errorCode;
        }

        internal static int CompleteAuthToken(SSPIInterface SecModule, ref SafeDeleteContext context, Interop.SecurityBuffer[] inputBuffers)
        {
            int errorCode = SecModule.CompleteAuthToken(ref context, inputBuffers);

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_operation_returned_something, "CompleteAuthToken()", (SecurityStatus)errorCode));

            return errorCode;
        }

        public static int QuerySecurityContextToken(SSPIInterface SecModule, SafeDeleteContext context, out SafeCloseHandle token)
        {
            return SecModule.QuerySecurityContextToken(context, out token);
        }

        public static int EncryptMessage(SSPIInterface secModule, SafeDeleteContext context, Interop.SecurityBuffer[] input, uint sequenceNumber)
        {
            return EncryptDecryptHelper(OP.Encrypt, secModule, context, input, sequenceNumber);
        }

        public static int DecryptMessage(SSPIInterface secModule, SafeDeleteContext context, Interop.SecurityBuffer[] input, uint sequenceNumber)
        {
            return EncryptDecryptHelper(OP.Decrypt, secModule, context, input, sequenceNumber);
        }

        internal static int MakeSignature(SSPIInterface secModule, SafeDeleteContext context, Interop.SecurityBuffer[] input, uint sequenceNumber)
        {
            return EncryptDecryptHelper(OP.MakeSignature, secModule, context, input, sequenceNumber);
        }

        public static int VerifySignature(SSPIInterface secModule, SafeDeleteContext context, Interop.SecurityBuffer[] input, uint sequenceNumber)
        {
            return EncryptDecryptHelper(OP.VerifySignature, secModule, context, input, sequenceNumber);
        }

        private enum OP
        {
            Encrypt = 1,
            Decrypt,
            MakeSignature,
            VerifySignature
        }
        //
        private unsafe static int EncryptDecryptHelper(OP op, SSPIInterface SecModule, SafeDeleteContext context, Interop.SecurityBuffer[] input, uint sequenceNumber)
        {
            Interop.SecurityBufferDescriptor sdcInOut = new Interop.SecurityBufferDescriptor(input.Length);
            Interop.SecurityBufferStruct[] unmanagedBuffer = new Interop.SecurityBufferStruct[input.Length];

            fixed (Interop.SecurityBufferStruct* unmanagedBufferPtr = unmanagedBuffer)
            {
                sdcInOut.UnmanagedPointer = unmanagedBufferPtr;
                GCHandle[] pinnedBuffers = new GCHandle[input.Length];
                byte[][] buffers = new byte[input.Length][];
                try
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        Interop.SecurityBuffer iBuffer = input[i];
                        unmanagedBuffer[i].count = iBuffer.size;
                        unmanagedBuffer[i].type = iBuffer.type;
                        if (iBuffer.token == null || iBuffer.token.Length == 0)
                        {
                            unmanagedBuffer[i].token = IntPtr.Zero;
                        }
                        else
                        {
                            pinnedBuffers[i] = GCHandle.Alloc(iBuffer.token, GCHandleType.Pinned);
                            unmanagedBuffer[i].token = Marshal.UnsafeAddrOfPinnedArrayElement(iBuffer.token, iBuffer.offset);
                            buffers[i] = iBuffer.token;
                        }
                    }

                    // The result is written in the input Buffer passed as type=BufferType.Data.
                    int errorCode;
                    switch (op)
                    {
                        case OP.Encrypt:
                            errorCode = SecModule.EncryptMessage(context, sdcInOut, sequenceNumber);
                            break;

                        case OP.Decrypt:
                            errorCode = SecModule.DecryptMessage(context, sdcInOut, sequenceNumber);
                            break;

                        case OP.MakeSignature:
                            errorCode = SecModule.MakeSignature(context, sdcInOut, sequenceNumber);
                            break;

                        case OP.VerifySignature:
                            errorCode = SecModule.VerifySignature(context, sdcInOut, sequenceNumber);
                            break;

                        default: throw ExceptionHelper.MethodNotImplementedException;
                    }

                    // Marshalling back returned sizes / data.
                    for (int i = 0; i < input.Length; i++)
                    {
                        Interop.SecurityBuffer iBuffer = input[i];
                        iBuffer.size = unmanagedBuffer[i].count;
                        iBuffer.type = unmanagedBuffer[i].type;

                        if (iBuffer.size == 0)
                        {
                            iBuffer.offset = 0;
                            iBuffer.token = null;
                        }
                        else
                            checked
                            {
                                // Find the buffer this is inside of.  Usually they all point inside buffer 0.
                                int j;
                                for (j = 0; j < input.Length; j++)
                                {
                                    if (buffers[j] == null)
                                    {
                                        continue;
                                    }

                                    byte* bufferAddress = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buffers[j], 0);
                                    if ((byte*)unmanagedBuffer[i].token >= bufferAddress &&
                                        (byte*)unmanagedBuffer[i].token + iBuffer.size <= bufferAddress + buffers[j].Length)
                                    {
                                        iBuffer.offset = (int)((byte*)unmanagedBuffer[i].token - bufferAddress);
                                        iBuffer.token = buffers[j];
                                        break;
                                    }
                                }

                                if (j >= input.Length)
                                {
                                    GlobalLog.Assert("SSPIWrapper::EncryptDecryptHelper", "Output buffer out of range.");
                                    iBuffer.size = 0;
                                    iBuffer.offset = 0;
                                    iBuffer.token = null;
                                }
                            }

                        // Backup validate the new sizes.
                        GlobalLog.Assert(iBuffer.offset >= 0 && iBuffer.offset <= (iBuffer.token == null ? 0 : iBuffer.token.Length), "SSPIWrapper::EncryptDecryptHelper|'offset' out of range.  [{0}]", iBuffer.offset);
                        GlobalLog.Assert(iBuffer.size >= 0 && iBuffer.size <= (iBuffer.token == null ? 0 : iBuffer.token.Length - iBuffer.offset), "SSPIWrapper::EncryptDecryptHelper|'size' out of range.  [{0}]", iBuffer.size);
                    }

                    if (errorCode != 0)
                        if (Logging.On)
                        {
                            if (errorCode == 0x90321)
                                Logging.PrintError(Logging.Web, SR.Format(SR.net_log_operation_returned_something, op, "SEC_I_RENEGOTIATE"));
                            else
                                Logging.PrintError(Logging.Web, SR.Format(SR.net_log_operation_failed_with_error, op, String.Format(CultureInfo.CurrentCulture, "0X{0:X}", errorCode)));
                        }
                    return errorCode;
                }
                finally
                {
                    for (int i = 0; i < pinnedBuffers.Length; ++i)
                    {
                        if (pinnedBuffers[i].IsAllocated)
                        {
                            pinnedBuffers[i].Free();
                        }
                    }
                }
            }
        }

        public static SafeFreeContextBufferChannelBinding QueryContextChannelBinding(SSPIInterface SecModule, SafeDeleteContext securityContext, Interop.ContextAttribute contextAttribute)
        {
            GlobalLog.Enter("QueryContextChannelBinding", contextAttribute.ToString());

            SafeFreeContextBufferChannelBinding result;
            int errorCode = SecModule.QueryContextChannelBinding(securityContext, contextAttribute, out result);
            if (errorCode != 0)
            {
                GlobalLog.Leave("QueryContextChannelBinding", "ERROR = " + ErrorDescription(errorCode));
                return null;
            }

            GlobalLog.Leave("QueryContextChannelBinding", Logging.HashString(result));
            return result;
        }

        public static object QueryContextAttributes(SSPIInterface SecModule, SafeDeleteContext securityContext, Interop.ContextAttribute contextAttribute)
        {
            int errorCode;
            return QueryContextAttributes(SecModule, securityContext, contextAttribute, out errorCode);
        }

        public static object QueryContextAttributes(SSPIInterface SecModule, SafeDeleteContext securityContext, Interop.ContextAttribute contextAttribute, out int errorCode)
        {
            GlobalLog.Enter("QueryContextAttributes", contextAttribute.ToString());

            int nativeBlockSize = IntPtr.Size;
            Type handleType = null;

            switch (contextAttribute)
            {
                case Interop.ContextAttribute.Sizes:
                    nativeBlockSize = SecSizes.SizeOf;
                    break;
                case Interop.ContextAttribute.StreamSizes:
                    nativeBlockSize = StreamSizes.SizeOf;
                    break;

                case Interop.ContextAttribute.Names:
                    handleType = typeof(SafeFreeContextBuffer);
                    break;

                case Interop.ContextAttribute.PackageInfo:
                    handleType = typeof(SafeFreeContextBuffer);
                    break;

                case Interop.ContextAttribute.NegotiationInfo:
                    handleType = typeof(SafeFreeContextBuffer);
                    nativeBlockSize = Marshal.SizeOf<NegotiationInfo>();
                    break;

                case Interop.ContextAttribute.ClientSpecifiedSpn:
                    handleType = typeof(SafeFreeContextBuffer);
                    break;

                case Interop.ContextAttribute.RemoteCertificate:
                    handleType = typeof(SafeFreeCertContext);
                    break;

                case Interop.ContextAttribute.LocalCertificate:
                    handleType = typeof(SafeFreeCertContext);
                    break;

                case Interop.ContextAttribute.IssuerListInfoEx:
                    nativeBlockSize = Marshal.SizeOf<Interop.IssuerListInfoEx>();
                    handleType = typeof(SafeFreeContextBuffer);
                    break;

                case Interop.ContextAttribute.ConnectionInfo:
                    nativeBlockSize = Marshal.SizeOf<SslConnectionInfo>();
                    break;

                default:
                    throw new ArgumentException(SR.Format(SR.net_invalid_enum, "ContextAttribute"), "contextAttribute");
            }

            SafeHandle SspiHandle = null;
            object attribute = null;

            try
            {
                byte[] nativeBuffer = new byte[nativeBlockSize];
                errorCode = SecModule.QueryContextAttributes(securityContext, contextAttribute, nativeBuffer, handleType, out SspiHandle);
                if (errorCode != 0)
                {
                    GlobalLog.Leave("Win32:QueryContextAttributes", "ERROR = " + ErrorDescription(errorCode));
                    return null;
                }

                switch (contextAttribute)
                {
                    case Interop.ContextAttribute.Sizes:
                        attribute = new SecSizes(nativeBuffer);
                        break;

                    case Interop.ContextAttribute.StreamSizes:
                        attribute = new StreamSizes(nativeBuffer);
                        break;

                    case Interop.ContextAttribute.Names:
                        attribute = Marshal.PtrToStringUni(SspiHandle.DangerousGetHandle());
                        break;

                    case Interop.ContextAttribute.PackageInfo:
                        attribute = new SecurityPackageInfoClass(SspiHandle, 0);
                        break;

                    case Interop.ContextAttribute.NegotiationInfo:
                        unsafe
                        {
                            fixed (void* ptr = nativeBuffer)
                            {
                                attribute = new NegotiationInfoClass(SspiHandle, Marshal.ReadInt32(new IntPtr(ptr), NegotiationInfo.NegotiationStateOffest));
                            }
                        }
                        break;

                    case Interop.ContextAttribute.ClientSpecifiedSpn:
                        attribute = Marshal.PtrToStringUni(SspiHandle.DangerousGetHandle());
                        break;

                    case Interop.ContextAttribute.LocalCertificate:
                        goto case Interop.ContextAttribute.RemoteCertificate;
                    case Interop.ContextAttribute.RemoteCertificate:
                        attribute = SspiHandle;
                        SspiHandle = null;
                        break;

                    case Interop.ContextAttribute.IssuerListInfoEx:
                        attribute = new Interop.IssuerListInfoEx(SspiHandle, nativeBuffer);
                        SspiHandle = null;
                        break;

                    case Interop.ContextAttribute.ConnectionInfo:
                        attribute = new SslConnectionInfo(nativeBuffer);
                        break;
                    default:
                        // will return null
                        break;
                }
            }
            finally
            {
                if (SspiHandle != null)
                {
                    SspiHandle.Dispose();
                }
            }
            GlobalLog.Leave("QueryContextAttributes", Logging.ObjectToString(attribute));
            return attribute;
        }

        public static int SetContextAttributes(SSPIInterface SecModule, SafeDeleteContext securityContext, Interop.ContextAttribute contextAttribute, object value)
        {
            GlobalLog.Enter("SetContextAttributes", contextAttribute.ToString());

            byte[] nativeBuffer;

            switch (contextAttribute)
            {
                case Interop.ContextAttribute.UiInfo:
                    Debug.Assert(value is IntPtr, "Type Mismatch");
                    IntPtr hwnd = (IntPtr)value; // A window handle
                    nativeBuffer = new byte[IntPtr.Size];
                    if (IntPtr.Size == 4) // 32bit
                    {
                        int ptr = hwnd.ToInt32();
                        nativeBuffer[0] = (byte)(ptr);
                        nativeBuffer[1] = (byte)(ptr >> 8);
                        nativeBuffer[2] = (byte)(ptr >> 16);
                        nativeBuffer[3] = (byte)(ptr >> 24);
                    }
                    else // 64bit
                    {
                        long ptr = hwnd.ToInt64();
                        nativeBuffer[0] = (byte)(ptr);
                        nativeBuffer[1] = (byte)(ptr >> 8);
                        nativeBuffer[2] = (byte)(ptr >> 16);
                        nativeBuffer[3] = (byte)(ptr >> 24);
                        nativeBuffer[4] = (byte)(ptr >> 32);
                        nativeBuffer[5] = (byte)(ptr >> 40);
                        nativeBuffer[6] = (byte)(ptr >> 48);
                        nativeBuffer[7] = (byte)(ptr >> 56);
                    }
                    break;

                default:
                    throw new ArgumentException(SR.Format(SR.net_invalid_enum, "ContextAttribute"), "contextAttribute");
            }

            return SecModule.SetContextAttributes(securityContext, contextAttribute, nativeBuffer);
        }

        public static string ErrorDescription(int errorCode)
        {
            if (errorCode == -1)
            {
                return "An exception when invoking Win32 API";
            }
            switch ((SecurityStatus)errorCode)
            {
                case SecurityStatus.InvalidHandle:
                    return "Invalid handle";
                case SecurityStatus.InvalidToken:
                    return "Invalid token";
                case SecurityStatus.ContinueNeeded:
                    return "Continue needed";
                case SecurityStatus.IncompleteMessage:
                    return "Message incomplete";
                case SecurityStatus.WrongPrincipal:
                    return "Wrong principal";
                case SecurityStatus.TargetUnknown:
                    return "Target unknown";
                case SecurityStatus.PackageNotFound:
                    return "Package not found";
                case SecurityStatus.BufferNotEnough:
                    return "Buffer not enough";
                case SecurityStatus.MessageAltered:
                    return "Message altered";
                case SecurityStatus.UntrustedRoot:
                    return "Untrusted root";
                default:
                    return "0x" + errorCode.ToString("x", NumberFormatInfo.InvariantInfo);
            }
        }
    } // class SSPIWrapper


    [StructLayout(LayoutKind.Sequential)]
    internal class StreamSizes
    {
        public int header;
        public int trailer;
        public int maximumMessage;
        public int buffersCount;
        public int blockSize;

        internal unsafe StreamSizes(byte[] memory)
        {
            fixed (void* voidPtr = memory)
            {
                IntPtr unmanagedAddress = new IntPtr(voidPtr);
                try
                {
                    header = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress));
                    trailer = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 4));
                    maximumMessage = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 8));
                    buffersCount = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 12));
                    blockSize = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 16));
                }
                catch (OverflowException)
                {
                    GlobalLog.Assert(false, "StreamSizes::.ctor", "Negative size.");
                    throw;
                }
            }
        }
        public static readonly int SizeOf = Marshal.SizeOf<StreamSizes>();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class SecSizes
    {
        public readonly int MaxToken;
        public readonly int MaxSignature;
        public readonly int BlockSize;
        public readonly int SecurityTrailer;

        internal unsafe SecSizes(byte[] memory)
        {
            fixed (void* voidPtr = memory)
            {
                IntPtr unmanagedAddress = new IntPtr(voidPtr);
                try
                {
                    MaxToken = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress));
                    MaxSignature = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 4));
                    BlockSize = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 8));
                    SecurityTrailer = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 12));
                }
                catch (OverflowException)
                {
                    GlobalLog.Assert(false, "SecSizes::.ctor", "Negative size.");
                    throw;
                }
            }
        }
        public static readonly int SizeOf = Marshal.SizeOf<SecSizes>();
    }

    //From Schannel.h
    [StructLayout(LayoutKind.Sequential)]
    internal class SslConnectionInfo
    {
        public readonly int Protocol;
        public readonly int DataCipherAlg;
        public readonly int DataKeySize;
        public readonly int DataHashAlg;
        public readonly int DataHashKeySize;
        public readonly int KeyExchangeAlg;
        public readonly int KeyExchKeySize;

        internal unsafe SslConnectionInfo(byte[] nativeBuffer)
        {
            fixed (void* voidPtr = nativeBuffer)
            {
                try
                {                
                    IntPtr unmanagedAddress = new IntPtr(voidPtr);
                    Protocol = Marshal.ReadInt32(unmanagedAddress);
                    DataCipherAlg = Marshal.ReadInt32(unmanagedAddress, 4);
                    DataKeySize = Marshal.ReadInt32(unmanagedAddress, 8);
                    DataHashAlg = Marshal.ReadInt32(unmanagedAddress, 12);
                    DataHashKeySize = Marshal.ReadInt32(unmanagedAddress, 16);
                    KeyExchangeAlg = Marshal.ReadInt32(unmanagedAddress, 20);
                    KeyExchKeySize = Marshal.ReadInt32(unmanagedAddress, 24);
               }
               catch (OverflowException)
               {
                   GlobalLog.Assert(false, "SslConnectionInfo::.ctor", "Negative size.");
                   throw;
               }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NegotiationInfo
    {
        // see SecPkgContext_NegotiationInfoW in <sspi.h>

        // [MarshalAs(UnmanagedType.LPStruct)] internal SecurityPackageInfo PackageInfo;
        internal IntPtr PackageInfo;
        internal uint NegotiationState;
        internal static readonly int Size = Marshal.SizeOf<NegotiationInfo>();
        internal static readonly int NegotiationStateOffest = (int)Marshal.OffsetOf<NegotiationInfo>("NegotiationState");
    }

    // we keep it simple since we use this only to know if NTLM or
    // Kerberos are used in the context of a Negotiate handshake
    internal class NegotiationInfoClass
    {
        internal const string NTLM = "NTLM";
        internal const string Kerberos = "Kerberos";
        internal const string WDigest = "WDigest";
        internal const string Negotiate = "Negotiate";
        internal string AuthenticationPackage;

        internal NegotiationInfoClass(SafeHandle safeHandle, int negotiationState)
        {
            if (safeHandle.IsInvalid)
            {
                GlobalLog.Print("NegotiationInfoClass::.ctor() the handle is invalid:" + (safeHandle.DangerousGetHandle()).ToString("x"));
                return;
            }
            IntPtr packageInfo = safeHandle.DangerousGetHandle();
            GlobalLog.Print("NegotiationInfoClass::.ctor() packageInfo:" + packageInfo.ToString("x8") + " negotiationState:" + negotiationState.ToString("x8"));

            const int SECPKG_NEGOTIATION_COMPLETE = 0;
            const int SECPKG_NEGOTIATION_OPTIMISTIC = 1;
            // const int SECPKG_NEGOTIATION_IN_PROGRESS     = 2;
            // const int SECPKG_NEGOTIATION_DIRECT          = 3;
            // const int SECPKG_NEGOTIATION_TRY_MULTICRED   = 4;

            if (negotiationState == SECPKG_NEGOTIATION_COMPLETE || negotiationState == SECPKG_NEGOTIATION_OPTIMISTIC)
            {
                IntPtr unmanagedString = Marshal.ReadIntPtr(packageInfo, SecurityPackageInfo.NameOffest);
                string name = null;
                if (unmanagedString != IntPtr.Zero)
                {
                    name = Marshal.PtrToStringUni(unmanagedString);
                }
                GlobalLog.Print("NegotiationInfoClass::.ctor() packageInfo:" + packageInfo.ToString("x8") + " negotiationState:" + negotiationState.ToString("x8") + " name:" + Logging.ObjectToString(name));

                // an optimization for future string comparisons
                if (string.Compare(name, Kerberos, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = Kerberos;
                }
                else if (string.Compare(name, NTLM, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = NTLM;
                }
                else if (string.Compare(name, WDigest, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = WDigest;
                }
                else
                {
                    AuthenticationPackage = name;
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityPackageInfo
    {
        // see SecPkgInfoW in <sspi.h>
        internal int Capabilities;
        internal short Version;
        internal short RPCID;
        internal int MaxToken;
        internal IntPtr Name;
        internal IntPtr Comment;

        internal static readonly int Size = Marshal.SizeOf<SecurityPackageInfo>();
        internal static readonly int NameOffest = (int)Marshal.OffsetOf<SecurityPackageInfo>("Name");
    }

    internal class SecurityPackageInfoClass
    {
        internal int Capabilities = 0;
        internal short Version = 0;
        internal short RPCID = 0;
        internal int MaxToken = 0;
        internal string Name = null;
        internal string Comment = null;

        /*
         *  This is to support SSL under semi trusted enviornment.
         *  Note that it is only for SSL with no client cert
         */
        internal SecurityPackageInfoClass(SafeHandle safeHandle, int index)
        {
            if (safeHandle.IsInvalid)
            {
                GlobalLog.Print("SecurityPackageInfoClass::.ctor() the pointer is invalid: " + (safeHandle.DangerousGetHandle()).ToString("x"));
                return;
            }
            IntPtr unmanagedAddress = IntPtrHelper.Add(safeHandle.DangerousGetHandle(), SecurityPackageInfo.Size * index);
            GlobalLog.Print("SecurityPackageInfoClass::.ctor() unmanagedPointer: " + ((long)unmanagedAddress).ToString("x"));

            Capabilities = Marshal.ReadInt32(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Capabilities"));
            Version = Marshal.ReadInt16(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Version"));
            RPCID = Marshal.ReadInt16(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("RPCID"));
            MaxToken = Marshal.ReadInt32(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("MaxToken"));

            IntPtr unmanagedString;

            unmanagedString = Marshal.ReadIntPtr(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Name"));
            if (unmanagedString != IntPtr.Zero)
            {
                Name = Marshal.PtrToStringUni(unmanagedString);
                GlobalLog.Print("Name: " + Name);
            }

            unmanagedString = Marshal.ReadIntPtr(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Comment"));
            if (unmanagedString != IntPtr.Zero)
            {
                Comment = Marshal.PtrToStringUni(unmanagedString);
                GlobalLog.Print("Comment: " + Comment);
            }

            GlobalLog.Print("SecurityPackageInfoClass::.ctor(): " + ToString());
        }

        public override string ToString()
        {
            return "Capabilities:" + String.Format(CultureInfo.InvariantCulture, "0x{0:x}", Capabilities)
                + " Version:" + Version.ToString(NumberFormatInfo.InvariantInfo)
                + " RPCID:" + RPCID.ToString(NumberFormatInfo.InvariantInfo)
                + " MaxToken:" + MaxToken.ToString(NumberFormatInfo.InvariantInfo)
                + " Name:" + ((Name == null) ? "(null)" : Name)
                + " Comment:" + ((Comment == null) ? "(null)" : Comment
                );
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Bindings
    {
        // see SecPkgContext_Bindings in <sspi.h>
        internal int BindingsLength;
        internal IntPtr pBindings;
    }
}
