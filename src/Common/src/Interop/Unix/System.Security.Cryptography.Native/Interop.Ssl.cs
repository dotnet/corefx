// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Ssl
    {
        internal delegate int SslCtxSetVerifyCallback(int preverify_ok, IntPtr x509_ctx);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EnsureLibSslInitialized")]
        internal static extern void EnsureLibSslInitialized();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslV2_3Method")]
        internal static extern IntPtr SslV2_3Method();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCreate")]
        internal static extern SafeSslHandle SslCreate(SafeSslContextHandle ctx);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetError")]
        internal static extern SslErrorCode SslGetError(SafeSslHandle ssl, int ret);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetError")]
        internal static extern SslErrorCode SslGetError(IntPtr ssl, int ret);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslSetQuietShutdown")]
        internal static extern void SslSetQuietShutdown(SafeSslHandle ssl, int mode);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslDestroy")]
        internal static extern void SslDestroy(IntPtr ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslSetConnectState")]
        internal static extern void SslSetConnectState(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslSetAcceptState")]
        internal static extern void SslSetAcceptState(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetVersion")]
        private static extern IntPtr SslGetVersion(SafeSslHandle ssl);

        internal static string GetProtocolVersion(SafeSslHandle ssl)
        {
            return Marshal.PtrToStringAnsi(SslGetVersion(ssl));
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetSslConnectionInfo")]
        internal static extern bool GetSslConnectionInfo(
            SafeSslHandle ssl,
            out int dataCipherAlg,
            out int keyExchangeAlg,
            out int dataHashAlg,
            out int dataKeySize,
            out int hashKeySize);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslWrite")]
        internal static extern unsafe int SslWrite(SafeSslHandle ssl, byte* buf, int num);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslRead")]
        internal static extern unsafe int SslRead(SafeSslHandle ssl, byte* buf, int num);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_IsSslRenegotiatePending")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsSslRenegotiatePending(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslShutdown")]
        internal static extern int SslShutdown(IntPtr ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslShutdown")]
        internal static extern int SslShutdown(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslSetBio")]
        internal static extern void SslSetBio(SafeSslHandle ssl, SafeBioHandle rbio, SafeBioHandle wbio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslDoHandshake")]
        internal static extern int SslDoHandshake(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_IsSslStateOK")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsSslStateOK(SafeSslHandle ssl);

        // NOTE: this is just an (unsafe) overload to the BioWrite method from Interop.Bio.cs.
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioWrite")]
        internal static extern unsafe int BioWrite(SafeBioHandle b, byte* data, int len);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetPeerCertificate")]
        internal static extern SafeX509Handle SslGetPeerCertificate(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetPeerCertChain")]
        internal static extern SafeSharedX509StackHandle SslGetPeerCertChain(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetPeerFinished")]
        internal static extern int SslGetPeerFinished(SafeSslHandle ssl, IntPtr buf, int count);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetFinished")]
        internal static extern int SslGetFinished(SafeSslHandle ssl, IntPtr buf, int count);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslSessionReused")]
        internal static extern bool SslSessionReused(SafeSslHandle ssl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslAddExtraChainCert")]
        internal static extern bool SslAddExtraChainCert(SafeSslHandle ssl, SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslGetClientCAList")]
        private static extern SafeSharedX509NameStackHandle SslGetClientCAList_private(SafeSslHandle ssl);

        internal static SafeSharedX509NameStackHandle SslGetClientCAList(SafeSslHandle ssl)
        {
            Crypto.CheckValidOpenSslHandle(ssl);

            SafeSharedX509NameStackHandle handle = SslGetClientCAList_private(ssl);

            if (!handle.IsInvalid)
            {
                handle.SetParent(ssl);
            }

            return handle;
        }

        internal static bool AddExtraChainCertificates(SafeSslHandle sslContext, X509Chain chain)
        {
            Debug.Assert(chain != null, "X509Chain should not be null");
            Debug.Assert(chain.ChainElements.Count > 0, "chain.Build should have already been called");

            for (int i = chain.ChainElements.Count - 2; i > 0; i--)
            {
                SafeX509Handle dupCertHandle = Crypto.X509UpRef(chain.ChainElements[i].Certificate.Handle);
                Crypto.CheckValidOpenSslHandle(dupCertHandle);
                if (!SslAddExtraChainCert(sslContext, dupCertHandle))
                {
                    dupCertHandle.Dispose(); // we still own the safe handle; clean it up
                    return false;
                }
                dupCertHandle.SetHandleAsInvalid(); // ownership has been transferred to sslHandle; do not free via this safe handle
            }

            return true;
        }

        internal static class SslMethods
        {
            internal static readonly IntPtr SSLv23_method = SslV2_3Method();
        }

        internal enum SslErrorCode
        {
            SSL_ERROR_NONE = 0,
            SSL_ERROR_SSL = 1,
            SSL_ERROR_WANT_READ = 2,
            SSL_ERROR_WANT_WRITE = 3,
            SSL_ERROR_SYSCALL = 5,
            SSL_ERROR_ZERO_RETURN = 6,

            // NOTE: this SslErrorCode value doesn't exist in OpenSSL, but
            // we use it to distinguish when a renegotiation is pending.
            // Choosing an arbitrarily large value that shouldn't conflict
            // with any actual OpenSSL error codes
            SSL_ERROR_RENEGOTIATE = 29304
        }
    }
}

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeSslHandle : SafeHandle
    {
        private ReadBioBuffer _readBio;
        private WriteBioBuffer _writeBio;
        private bool _isServer;
        private bool _handshakeCompleted = false;

        public bool IsServer
        {
            get { return _isServer; }
        }

        public ReadBioBuffer InputBio => _readBio;
        public WriteBioBuffer OutputBio => _writeBio;
        
        internal void MarkHandshakeCompleted()
        {
            _handshakeCompleted = true;
        }

        public static SafeSslHandle Create(SafeSslContextHandle context, bool isServer)
        {
            SafeBioHandle readBio = Interop.CustomBio.CreateCustomBio();
            SafeBioHandle writeBio = Interop.CustomBio.CreateCustomBio();
            SafeSslHandle handle = Interop.Ssl.SslCreate(context);
            if (readBio.IsInvalid || writeBio.IsInvalid || handle.IsInvalid)
            {
                readBio.Dispose();
                writeBio.Dispose();
                handle.Dispose(); // will make IsInvalid==true if it's not already
                return handle;
            }
            handle._isServer = isServer;

            // SslSetBio will transfer ownership of the BIO handles to the SSL context
            try
            {
                readBio.TransferOwnershipToParent(handle);
                writeBio.TransferOwnershipToParent(handle);
                handle._readBio = new ReadBioBuffer(readBio);
                handle._writeBio = new WriteBioBuffer(writeBio);
                Interop.Ssl.SslSetBio(handle, readBio, writeBio);
            }
            catch (Exception exc)
            {
                // The only way this should be able to happen without thread aborts is if we hit OOMs while
                // manipulating the safe handles, in which case we may leak the bio handles.
                Debug.Fail("Unexpected exception while transferring SafeBioHandle ownership to SafeSslHandle", exc.ToString());
                throw;
            }

            if (isServer)
            {
                Interop.Ssl.SslSetAcceptState(handle);
            }
            else
            {
                Interop.Ssl.SslSetConnectState(handle);
            }
            return handle;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readBio?.Dispose();
                _writeBio?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override bool ReleaseHandle()
        {
            if (_handshakeCompleted)
            {
                Disconnect();
            }

            IntPtr h = handle;
            SetHandle(IntPtr.Zero);
            Interop.Ssl.SslDestroy(h); // will free the handles underlying _readBio and _writeBio

            return true;
        }

        private void Disconnect()
        {
            Debug.Assert(!IsInvalid, "Expected a valid context in Disconnect");

            int retVal = Interop.Ssl.SslShutdown(handle);

            // Here, we are ignoring checking for <0 return values from Ssl_Shutdown,
            // since the underlying memory bio is already disposed, we are not
            // interested in reading or writing to it.
            if (retVal == 0)
            {
                // Do a bi-directional shutdown.
                retVal = Interop.Ssl.SslShutdown(handle);
            }
        }

        private SafeSslHandle() : base(IntPtr.Zero, true)
        {
        }

        internal SafeSslHandle(IntPtr validSslPointer, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
            handle = validSslPointer;
        }

        internal class ReadBioBuffer : IDisposable
        {
            private SafeBioHandle _bioHandle;
            private GCHandle _handle;
            private int _bytesAvailable;
            private byte[] _byteArray;
            private int _offset;

            internal ReadBioBuffer(SafeBioHandle bioHandle)
            {
                _bioHandle = bioHandle;
                _handle = GCHandle.Alloc(this, GCHandleType.Normal);
                Interop.CustomBio.BioSetGCHandle(_bioHandle, _handle);
                Interop.Crypto.BioSetFlags(bioHandle, Interop.Crypto.BIO_FLAGS.BIO_FLAGS_READ | Interop.Crypto.BIO_FLAGS.BIO_FLAGS_SHOULD_RETRY);
            }

            public void SetBio(byte[] buffer, int offset, int length)
            {
                _byteArray = buffer;
                _offset = offset;
                _bytesAvailable = length;
            }

            public int Read(Span<byte> output)
            {
                var bytesToCopy = Math.Min(output.Length, _bytesAvailable);
                if (bytesToCopy == 0) return -1;
                var span = new Span<byte>(_byteArray, _offset, bytesToCopy);
                span.CopyTo(output);
                _offset += bytesToCopy;
                _bytesAvailable -= bytesToCopy;
                return bytesToCopy;
            }
            
            //Bio is already released by the ssl object
            private void Dispose(bool isDisposing)
            {
                if (_handle.IsAllocated)
                {
                    _handle.Free();
                }
            }

            public void Dispose()
            {
                Dispose(false);
                GC.SuppressFinalize(this);
            }

            ~ReadBioBuffer()
            {
                Dispose(false);
            }
        }

        internal class WriteBioBuffer :IDisposable
        {
            private SafeBioHandle _bioHandle;
            private GCHandle _handle;
            private byte[] _byteArray;
            private int _bytesWritten;
            private bool _isHandshake;

            internal WriteBioBuffer(SafeBioHandle bioHandle)
            {
                _bioHandle = bioHandle;
                _handle = GCHandle.Alloc(this, GCHandleType.Normal);
                Interop.CustomBio.BioSetGCHandle(_bioHandle, _handle);
            }
                        
            public void SetBio(byte[] buffer, bool isHandshake)
            {
                _byteArray = buffer;
                _bytesWritten = 0;
                _isHandshake = isHandshake;
            }

            public int TakeBytes(out byte[] output)
            {
                var bytes = _bytesWritten;
                output = _byteArray;
                _bytesWritten = 0;
                _byteArray = null;
                return bytes;
            }
            
            public int Write(Span<byte> input)
            {
                if (_isHandshake)
                {
                    if (_byteArray == null)
                    {
                        _byteArray = new byte[input.Length];
                    }
                    else if (_byteArray.Length - _bytesWritten < input.Length)
                    {
                        var oldSpan = new Span<byte>(_byteArray);
                        _byteArray = new byte[input.Length + _bytesWritten];
                        oldSpan.CopyTo(_byteArray);
                    }
                }
                var bytesToWrite = Math.Min(input.Length, input.Length - _bytesWritten);
                input.Slice(0, bytesToWrite).CopyTo(new Span<byte>(_byteArray, _bytesWritten));
                _bytesWritten += bytesToWrite;
                return bytesToWrite;
            }
            
            //Bio is already released by the ssl object
            private void Dispose(bool isDisposing)
            {
                if (_handle.IsAllocated)
                {
                    _handle.Free();
                }
            }

            public void Dispose()
            {
                Dispose(false);
                GC.SuppressFinalize(this);
            }

            ~WriteBioBuffer()
            {
                Dispose(false);
            }
        }
    }
}
