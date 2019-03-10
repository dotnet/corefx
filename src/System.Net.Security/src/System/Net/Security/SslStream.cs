// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Security
{
    public enum EncryptionPolicy
    {
        // Prohibit null ciphers (current system defaults)
        RequireEncryption = 0,

        // Add null ciphers to current system defaults
        AllowNoEncryption,

        // Request null ciphers only
        NoEncryption
    }

    // A user delegate used to verify remote SSL certificate.
    public delegate bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);

    // A user delegate used to select local SSL certificate.
    public delegate X509Certificate LocalCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers);

    public delegate X509Certificate ServerCertificateSelectionCallback(object sender, string hostName);

    // Internal versions of the above delegates.
    internal delegate bool RemoteCertValidationCallback(string host, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);
    internal delegate X509Certificate LocalCertSelectionCallback(string targetHost, X509CertificateCollection localCertificates, X509Certificate2 remoteCertificate, string[] acceptableIssuers);
    internal delegate X509Certificate ServerCertSelectionCallback(string hostName);

    public partial class SslStream : AuthenticatedStream
    {
        private X509Certificate2 _remoteCertificate;
        private bool _remoteCertificateExposed;

        internal RemoteCertificateValidationCallback _userCertificateValidationCallback;
        internal LocalCertificateSelectionCallback _userCertificateSelectionCallback;
        internal ServerCertificateSelectionCallback _userServerCertificateSelectionCallback;
        internal RemoteCertValidationCallback _certValidationDelegate;
        internal LocalCertSelectionCallback _certSelectionDelegate;
        internal EncryptionPolicy _encryptionPolicy;

        private readonly Stream _innerStream;
        private readonly SslStreamInternal _secureStream;
        private SecureChannel _context;

        private ExceptionDispatchInfo _exception;
        private bool _shutdown;

        public SslStream(Stream innerStream)
                : this(innerStream, false, null, null)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen)
                : this(innerStream, leaveInnerStreamOpen, null, null, EncryptionPolicy.RequireEncryption)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback)
                : this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, null, EncryptionPolicy.RequireEncryption)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback)
                : this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, EncryptionPolicy.RequireEncryption)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
            : base(innerStream, leaveInnerStreamOpen)
        {
            if (encryptionPolicy != EncryptionPolicy.RequireEncryption && encryptionPolicy != EncryptionPolicy.AllowNoEncryption && encryptionPolicy != EncryptionPolicy.NoEncryption)
            {
                throw new ArgumentException(SR.Format(SR.net_invalid_enum, "EncryptionPolicy"), nameof(encryptionPolicy));
            }

            _userCertificateValidationCallback = userCertificateValidationCallback;
            _userCertificateSelectionCallback = userCertificateSelectionCallback;
            _encryptionPolicy = encryptionPolicy;
            _certValidationDelegate = new RemoteCertValidationCallback(UserCertValidationCallbackWrapper);
            _certSelectionDelegate = userCertificateSelectionCallback == null ? null : new LocalCertSelectionCallback(UserCertSelectionCallbackWrapper);

            _innerStream = innerStream;
            _secureStream = new SslStreamInternal(this);
        }

        public SslApplicationProtocol NegotiatedApplicationProtocol
        {
            get
            {
                if (_context == null)
                    return default;

                return _context.NegotiatedApplicationProtocol;
            }
        }

        private void SetAndVerifyValidationCallback(RemoteCertificateValidationCallback callback)
        {
            if (_userCertificateValidationCallback == null)
            {
                _userCertificateValidationCallback = callback;
                _certValidationDelegate = new RemoteCertValidationCallback(UserCertValidationCallbackWrapper);
            }
            else if (callback != null && _userCertificateValidationCallback != callback)
            {
                throw new InvalidOperationException(SR.Format(SR.net_conflicting_options, nameof(RemoteCertificateValidationCallback)));
            }
        }

        private void SetAndVerifySelectionCallback(LocalCertificateSelectionCallback callback)
        {
            if (_userCertificateSelectionCallback == null)
            {
                _userCertificateSelectionCallback = callback;
                _certSelectionDelegate = _userCertificateSelectionCallback == null ? null : new LocalCertSelectionCallback(UserCertSelectionCallbackWrapper);
            }
            else if (callback != null && _userCertificateSelectionCallback != callback)
            {
                throw new InvalidOperationException(SR.Format(SR.net_conflicting_options, nameof(LocalCertificateSelectionCallback)));
            }
        }

        private bool UserCertValidationCallbackWrapper(string hostName, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            _remoteCertificate = certificate == null ? null : new X509Certificate2(certificate);
            if (_userCertificateValidationCallback == null)
            {
                if (!RemoteCertRequired)
                {
                    sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateNotAvailable;
                }

                return (sslPolicyErrors == SslPolicyErrors.None);
            }
            else
            {
                return _userCertificateValidationCallback(this, certificate, chain, sslPolicyErrors);
            }
        }

        private X509Certificate UserCertSelectionCallbackWrapper(string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return _userCertificateSelectionCallback(this, targetHost, localCertificates, remoteCertificate, acceptableIssuers);
        }

        private X509Certificate ServerCertSelectionCallbackWrapper(string targetHost)
        {
            return _userServerCertificateSelectionCallback(this, targetHost);
        }

        private SslAuthenticationOptions CreateAuthenticationOptions(SslServerAuthenticationOptions sslServerAuthenticationOptions)
        {
            if (sslServerAuthenticationOptions.ServerCertificate == null && sslServerAuthenticationOptions.ServerCertificateSelectionCallback == null && _certSelectionDelegate == null)
            {
                throw new ArgumentNullException(nameof(sslServerAuthenticationOptions.ServerCertificate));
            }

            if ((sslServerAuthenticationOptions.ServerCertificate != null || _certSelectionDelegate != null) && sslServerAuthenticationOptions.ServerCertificateSelectionCallback != null)
            {
                throw new InvalidOperationException(SR.Format(SR.net_conflicting_options, nameof(ServerCertificateSelectionCallback)));
            }

            var authOptions = new SslAuthenticationOptions(sslServerAuthenticationOptions);

            _userServerCertificateSelectionCallback = sslServerAuthenticationOptions.ServerCertificateSelectionCallback;
            authOptions.ServerCertSelectionDelegate = _userServerCertificateSelectionCallback == null ? null : new ServerCertSelectionCallback(ServerCertSelectionCallbackWrapper);

            authOptions.CertValidationDelegate = _certValidationDelegate;
            authOptions.CertSelectionDelegate = _certSelectionDelegate;

            return authOptions;
        }

        //
        // Client side auth.
        //
        public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
        {
            return BeginAuthenticateAsClient(targetHost, null, SecurityProtocol.SystemDefaultSecurityProtocols, false,
                                           asyncCallback, asyncState);
        }

        public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates,
                                                            bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
        {
            return BeginAuthenticateAsClient(targetHost, clientCertificates, SecurityProtocol.SystemDefaultSecurityProtocols, checkCertificateRevocation, asyncCallback, asyncState);
        }

        public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates,
                                                            SslProtocols enabledSslProtocols, bool checkCertificateRevocation,
                                                            AsyncCallback asyncCallback, object asyncState)
        {
            SslClientAuthenticationOptions options = new SslClientAuthenticationOptions
            {
                TargetHost = targetHost,
                ClientCertificates = clientCertificates,
                EnabledSslProtocols = enabledSslProtocols,
                CertificateRevocationCheckMode = checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                EncryptionPolicy = _encryptionPolicy,
            };

            return BeginAuthenticateAsClient(options, CancellationToken.None, asyncCallback, asyncState);
        }

        internal virtual IAsyncResult BeginAuthenticateAsClient(SslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken, AsyncCallback asyncCallback, object asyncState)
        {
            SetAndVerifyValidationCallback(sslClientAuthenticationOptions.RemoteCertificateValidationCallback);
            SetAndVerifySelectionCallback(sslClientAuthenticationOptions.LocalCertificateSelectionCallback);

            ValidateCreateContext(sslClientAuthenticationOptions, _certValidationDelegate, _certSelectionDelegate);

            LazyAsyncResult result = new LazyAsyncResult(this, asyncState, asyncCallback);
            ProcessAuthentication(result);
            return result;
        }

        public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
        {
            EndProcessAuthentication(asyncResult);
        }

        //
        // Server side auth.
        //
        public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback asyncCallback, object asyncState)

        {
            return BeginAuthenticateAsServer(serverCertificate, false, SecurityProtocol.SystemDefaultSecurityProtocols, false,
                                                          asyncCallback,
                                                            asyncState);
        }

        public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired,
                                                            bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
        {
            return BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, SecurityProtocol.SystemDefaultSecurityProtocols, checkCertificateRevocation, asyncCallback, asyncState);
        }

        public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired,
                                                            SslProtocols enabledSslProtocols, bool checkCertificateRevocation,
                                                            AsyncCallback asyncCallback,
                                                            object asyncState)
        {
            SslServerAuthenticationOptions options = new SslServerAuthenticationOptions
            {
                ServerCertificate = serverCertificate,
                ClientCertificateRequired = clientCertificateRequired,
                EnabledSslProtocols = enabledSslProtocols,
                CertificateRevocationCheckMode = checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                EncryptionPolicy = _encryptionPolicy,
            };

            return BeginAuthenticateAsServer(options, CancellationToken.None, asyncCallback, asyncState);
        }

        private IAsyncResult BeginAuthenticateAsServer(SslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken, AsyncCallback asyncCallback, object asyncState)
        {
            SetAndVerifyValidationCallback(sslServerAuthenticationOptions.RemoteCertificateValidationCallback);

            ValidateCreateContext(CreateAuthenticationOptions(sslServerAuthenticationOptions));

            LazyAsyncResult result = new LazyAsyncResult(this, asyncState, asyncCallback);
            ProcessAuthentication(result);
            return result;
        }

        public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
        {
            EndProcessAuthentication(asyncResult);
        }

        internal virtual IAsyncResult BeginShutdown(AsyncCallback asyncCallback, object asyncState)
        {
            CheckThrow(authSuccessCheck: true, shutdownCheck: true);

            ProtocolToken message = _context.CreateShutdownToken();
            return TaskToApm.Begin(InnerStream.WriteAsync(message.Payload, 0, message.Payload.Length), asyncCallback, asyncState);
        }

        internal virtual void EndShutdown(IAsyncResult asyncResult)
        {
            CheckThrow(authSuccessCheck: true, shutdownCheck: true);

            TaskToApm.End(asyncResult);
            _shutdown = true;
        }

        public TransportContext TransportContext
        {
            get
            {
                return new SslStreamContext(this);
            }
        }

        internal ChannelBinding GetChannelBinding(ChannelBindingKind kind)
        {
            return (_context == null) ? null : _context.GetChannelBinding(kind);
        }

        #region Synchronous methods
        public virtual void AuthenticateAsClient(string targetHost)
        {
            AuthenticateAsClient(targetHost, null, SecurityProtocol.SystemDefaultSecurityProtocols, false);
        }

        public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, bool checkCertificateRevocation)
        {
            AuthenticateAsClient(targetHost, clientCertificates, SecurityProtocol.SystemDefaultSecurityProtocols, checkCertificateRevocation);
        }

        public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            SslClientAuthenticationOptions options = new SslClientAuthenticationOptions
            {
                TargetHost = targetHost,
                ClientCertificates = clientCertificates,
                EnabledSslProtocols = enabledSslProtocols,
                CertificateRevocationCheckMode = checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                EncryptionPolicy = _encryptionPolicy,
            };

            AuthenticateAsClient(options);
        }

        private void AuthenticateAsClient(SslClientAuthenticationOptions sslClientAuthenticationOptions)
        {
            SetAndVerifyValidationCallback(sslClientAuthenticationOptions.RemoteCertificateValidationCallback);
            SetAndVerifySelectionCallback(sslClientAuthenticationOptions.LocalCertificateSelectionCallback);

            ValidateCreateContext(sslClientAuthenticationOptions, _certValidationDelegate, _certSelectionDelegate);
            ProcessAuthentication(null);
        }

        public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
        {
            AuthenticateAsServer(serverCertificate, false, SecurityProtocol.SystemDefaultSecurityProtocols, false);
        }

        public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation)
        {
            AuthenticateAsServer(serverCertificate, clientCertificateRequired, SecurityProtocol.SystemDefaultSecurityProtocols, checkCertificateRevocation);
        }

        public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            SslServerAuthenticationOptions options = new SslServerAuthenticationOptions
            {
                ServerCertificate = serverCertificate,
                ClientCertificateRequired = clientCertificateRequired,
                EnabledSslProtocols = enabledSslProtocols,
                CertificateRevocationCheckMode = checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                EncryptionPolicy = _encryptionPolicy,
            };

            AuthenticateAsServer(options);
        }

        private void AuthenticateAsServer(SslServerAuthenticationOptions sslServerAuthenticationOptions)
        {
            SetAndVerifyValidationCallback(sslServerAuthenticationOptions.RemoteCertificateValidationCallback);

            ValidateCreateContext(CreateAuthenticationOptions(sslServerAuthenticationOptions));
            ProcessAuthentication(null);
        }
        #endregion

        #region Task-based async public methods
        public virtual Task AuthenticateAsClientAsync(string targetHost) =>
            Task.Factory.FromAsync(
                (arg1, callback, state) => ((SslStream)state).BeginAuthenticateAsClient(arg1, callback, state),
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsClient(iar),
                targetHost,
                this);

        public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, bool checkCertificateRevocation) =>
            Task.Factory.FromAsync(
                (arg1, arg2, arg3, callback, state) => ((SslStream)state).BeginAuthenticateAsClient(arg1, arg2, SecurityProtocol.SystemDefaultSecurityProtocols, arg3, callback, state),
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsClient(iar),
                targetHost, clientCertificates, checkCertificateRevocation,
                this);

        public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            var beginMethod = checkCertificateRevocation ? (Func<string, X509CertificateCollection, SslProtocols, AsyncCallback, object, IAsyncResult>)
                ((arg1, arg2, arg3, callback, state) => ((SslStream)state).BeginAuthenticateAsClient(arg1, arg2, arg3, true, callback, state)) :
                ((arg1, arg2, arg3, callback, state) => ((SslStream)state).BeginAuthenticateAsClient(arg1, arg2, arg3, false, callback, state));
            return Task.Factory.FromAsync(
                beginMethod,
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsClient(iar),
                targetHost, clientCertificates, enabledSslProtocols,
                this);
        }

        public Task AuthenticateAsClientAsync(SslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken)
        {
            return Task.Factory.FromAsync(
                (arg1, arg2, callback, state) => ((SslStream)state).BeginAuthenticateAsClient(arg1, arg2, callback, state),
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsClient(iar),
                sslClientAuthenticationOptions, cancellationToken,
                this);
        }

        public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate) =>
            Task.Factory.FromAsync(
                (arg1, callback, state) => ((SslStream)state).BeginAuthenticateAsServer(arg1, callback, state),
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsServer(iar),
                serverCertificate,
                this);

        public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation) =>
            Task.Factory.FromAsync(
                (arg1, arg2, arg3, callback, state) => ((SslStream)state).BeginAuthenticateAsServer(arg1, arg2, SecurityProtocol.SystemDefaultSecurityProtocols, arg3, callback, state),
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsServer(iar),
                serverCertificate, clientCertificateRequired, checkCertificateRevocation,
                this);

        public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            var beginMethod = checkCertificateRevocation ? (Func<X509Certificate, bool, SslProtocols, AsyncCallback, object, IAsyncResult>)
                ((arg1, arg2, arg3, callback, state) => ((SslStream)state).BeginAuthenticateAsServer(arg1, arg2, arg3, true, callback, state)) :
                ((arg1, arg2, arg3, callback, state) => ((SslStream)state).BeginAuthenticateAsServer(arg1, arg2, arg3, false, callback, state));
            return Task.Factory.FromAsync(
                beginMethod,
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsServer(iar),
                serverCertificate, clientCertificateRequired, enabledSslProtocols,
                this);
        }

        public Task AuthenticateAsServerAsync(SslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken)
        {
            return Task.Factory.FromAsync(
                (arg1, arg2, callback, state) => ((SslStream)state).BeginAuthenticateAsServer(arg1, arg2, callback, state),
                iar => ((SslStream)iar.AsyncState).EndAuthenticateAsServer(iar),
                sslServerAuthenticationOptions, cancellationToken,
                this);
        }

        public virtual Task ShutdownAsync() =>
            Task.Factory.FromAsync(
                (callback, state) => ((SslStream)state).BeginShutdown(callback, state),
                iar => ((SslStream)iar.AsyncState).EndShutdown(iar),
                this);
        #endregion

        public override bool IsAuthenticated
        {
            get
            {
                return _context != null && _context.IsValidContext && _exception == null && HandshakeCompleted;
            }
        }

        public override bool IsMutuallyAuthenticated
        {
            get
            {
                return
                    IsAuthenticated &&
                    (_context.IsServer ? _context.LocalServerCertificate : _context.LocalClientCertificate) != null &&
                    _context.IsRemoteCertificateAvailable; /* does not work: Context.IsMutualAuthFlag;*/
            }
        }

        public override bool IsEncrypted
        {
            get
            {
                return IsAuthenticated;
            }
        }

        public override bool IsSigned
        {
            get
            {
                return IsAuthenticated;
            }
        }

        public override bool IsServer
        {
            get
            {
                return _context != null && _context.IsServer;
            }
        }

        public virtual SslProtocols SslProtocol
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return SslProtocols.None;
                }

                SslProtocols proto = (SslProtocols)info.Protocol;
                SslProtocols ret = SslProtocols.None;

#pragma warning disable 0618 // Ssl2, Ssl3 are deprecated.
                // Restore client/server bits so the result maps exactly on published constants.
                if ((proto & SslProtocols.Ssl2) != 0)
                {
                    ret |= SslProtocols.Ssl2;
                }

                if ((proto & SslProtocols.Ssl3) != 0)
                {
                    ret |= SslProtocols.Ssl3;
                }
#pragma warning restore

                if ((proto & SslProtocols.Tls) != 0)
                {
                    ret |= SslProtocols.Tls;
                }

                if ((proto & SslProtocols.Tls11) != 0)
                {
                    ret |= SslProtocols.Tls11;
                }

                if ((proto & SslProtocols.Tls12) != 0)
                {
                    ret |= SslProtocols.Tls12;
                }

                if ((proto & SslProtocols.Tls13) != 0)
                {
                    ret |= SslProtocols.Tls13;
                }

                return ret;
            }
        }

        public virtual bool CheckCertRevocationStatus
        {
            get
            {
                return _context != null && _context.CheckCertRevocationStatus != X509RevocationMode.NoCheck;
            }
        }

        //
        // This will return selected local cert for both client/server streams
        //
        public virtual X509Certificate LocalCertificate
        {
            get
            {
                CheckThrow(true);
                return InternalLocalCertificate;
            }
        }

        public virtual X509Certificate RemoteCertificate
        {
            get
            {
                CheckThrow(true);
                _remoteCertificateExposed = true;
                return _remoteCertificate;
            }
        }

        public virtual CipherAlgorithmType CipherAlgorithm
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return CipherAlgorithmType.None;
                }
                return (CipherAlgorithmType)info.DataCipherAlg;
            }
        }

        public virtual int CipherStrength
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return 0;
                }

                return info.DataKeySize;
            }
        }

        public virtual HashAlgorithmType HashAlgorithm
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return (HashAlgorithmType)0;
                }
                return (HashAlgorithmType)info.DataHashAlg;
            }
        }

        public virtual int HashStrength
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return 0;
                }

                return info.DataHashKeySize;
            }
        }

        public virtual ExchangeAlgorithmType KeyExchangeAlgorithm
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return (ExchangeAlgorithmType)0;
                }

                return (ExchangeAlgorithmType)info.KeyExchangeAlg;
            }
        }

        public virtual int KeyExchangeStrength
        {
            get
            {
                CheckThrow(true);
                SslConnectionInfo info = _context.ConnectionInfo;
                if (info == null)
                {
                    return 0;
                }

                return info.KeyExchKeySize;
            }
        }

        //
        // Stream contract implementation.
        //
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanRead
        {
            get
            {
                return IsAuthenticated && InnerStream.CanRead;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return InnerStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return IsAuthenticated && InnerStream.CanWrite && !IsShutdown;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return InnerStream.ReadTimeout;
            }
            set
            {
                InnerStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return InnerStream.WriteTimeout;
            }
            set
            {
                InnerStream.WriteTimeout = value;
            }
        }

        public override long Length
        {
            get
            {
                return InnerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return InnerStream.Position;
            }
            set
            {
                throw new NotSupportedException(SR.net_noseek);
            }
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return InnerStream.FlushAsync(cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!_remoteCertificateExposed)
                {
                    _remoteCertificate?.Dispose();
                    _remoteCertificate = null;
                    _remoteCertificateExposed = false;
                }
                CloseInternal();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                CloseInternal();
            }
            finally
            {
                await base.DisposeAsync().ConfigureAwait(false);
            }
        }

        public override int ReadByte()
        {
            return SecureStream.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return SecureStream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer)
        {
            SecureStream.Write(buffer, 0, buffer.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            SecureStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            return SecureStream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return SecureStream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            return SecureStream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            SecureStream.EndWrite(asyncResult);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return SecureStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            return SecureStream.WriteAsync(buffer, cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return SecureStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return SecureStream.ReadAsync(buffer, cancellationToken);
        }
    }
}
