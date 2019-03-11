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
            if(_shutdown == true)
            {

            }
            _context = null;
            _exception = null;

        }

        internal void ValidateParameters(byte[] buffer, int offset, int count)
        {
        }

        internal void ValidateCreateContext(SslAuthenticationOptions sslAuthenticationOptions)
        {
        }

        internal Task CheckEnqueueWriteAsync() => default;

        internal void CheckEnqueueWrite()
        {
        }

        internal ValueTask<int> CheckEnqueueReadAsync(Memory<byte> buffer) => default;

        internal int CheckEnqueueRead(Memory<byte> buffer) => default;

        internal bool RemoteCertRequired
        {
            get
            {
                return false;
            }
        }

        private X509Certificate InternalLocalCertificate => default;

        internal SslStreamInternal SecureStream
        {
            get
            {
                return null;
            }
        }

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

    internal class SslStreamInternal : Stream
    {
        public SslStreamInternal(SslStream stream)
        {
        }

        public override bool CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal ValueTask<int> ReadAsyncInternal<TReadAdapter>(TReadAdapter adapter, Memory<byte> buffer)
            where TReadAdapter : ISslReadAdapter => default;

        internal ValueTask WriteAsyncInternal<TWriteAdapter>(TWriteAdapter writeAdapter, ReadOnlyMemory<byte> buffer)
            where TWriteAdapter : struct, ISslWriteAdapter => default;
        
        public override bool CanSeek
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool CanWrite
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.FromException(new NotImplementedException());
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public new ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
    }
}
