// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Security.SslStream;

namespace System.Net.Security
{
    public partial class SslStream
    {
        internal void ValidateCreateContext(SslClientAuthenticationOptions sslClientAuthenticationOptions, RemoteCertValidationCallback remoteCallback, LocalCertSelectionCallback localCallback)
        {
            if(_shutdown == true && _nestedWrite == 0)
            {

            }
            _context = null;
            _exception = null;
            _internalBuffer = null;
            _internalBufferCount = 0;
            _internalOffset = 0;
            _nestedWrite = 0;
        }

        internal void ValidateParameters(byte[] buffer, int offset, int count)
        {
        }

        internal void ValidateCreateContext(SslAuthenticationOptions sslAuthenticationOptions)
        {
        }

        internal ValueTask WriteAsyncInternal<TWriteAdapter>(TWriteAdapter writeAdapter, ReadOnlyMemory<byte> buffer)
            where TWriteAdapter : struct, ISslWriteAdapter => default;

        internal ValueTask<int> ReadAsyncInternal<TReadAdapter>(TReadAdapter adapter, Memory<byte> buffer) => default;

        internal Task CheckEnqueueWriteAsync() => default;

        internal void CheckEnqueueWrite()
        {
        }

        internal ValueTask<int> CheckEnqueueReadAsync(Memory<byte> buffer) => default;

        internal int CheckEnqueueRead(Memory<byte> buffer) => default;

        internal bool RemoteCertRequired => default;

        private X509Certificate InternalLocalCertificate => default;

        internal bool HandshakeCompleted => default;

        public bool IsShutdown { get; internal set; }

        internal void CheckThrow(bool authSuccessCheck, bool shutdownCheck = false)
        {
        }

        internal void CloseInternal()
        {
        }
        //
        // This method assumes that a SSPI context is already in a good shape.
        // For example it is either a fresh context or already authenticated context that needs renegotiation.
        //
        internal void ProcessAuthentication(LazyAsyncResult lazyResult)
        {
        }

        internal void EndProcessAuthentication(IAsyncResult result)
        {
        }

        internal void ReturnReadBufferIfEmpty()
        {
        }
    }

    internal class SecureChannel
    {
        internal bool IsValidContext => default;
        internal bool IsServer => default;
        internal SslConnectionInfo ConnectionInfo => default;
        internal ChannelBinding GetChannelBinding(ChannelBindingKind kind) => default;
        internal X509Certificate LocalServerCertificate => default;
        internal bool IsRemoteCertificateAvailable => default;
        internal SslApplicationProtocol NegotiatedApplicationProtocol => default;
        internal X509Certificate LocalClientCertificate => default;
        internal X509RevocationMode CheckCertRevocationStatus => default;
        internal ProtocolToken CreateShutdownToken() => default;
    }

    internal class ProtocolToken
    {
        public ProtocolToken()
        {
            Payload = null; 
        }
        internal byte[] Payload;
    }
}
