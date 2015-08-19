// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
/*++
Copyright (c) 2003 Microsoft Corporation

Module Name:

    SslStream.cs

Abstract:

    A public implementation of authenticated stream using SSL protocol

Author:
    Alexei Vopilov    Sept 28-2003

Revision History:

--*/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;

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

    // A user delegate used to verify remote SSL certificate
    public delegate bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);

    // A user delegate used to select local SSL certificate
    public delegate X509Certificate LocalCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers);

    // Internal versions of the above delegates
    internal delegate bool RemoteCertValidationCallback(string host, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);
    internal delegate X509Certificate LocalCertSelectionCallback(string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers);
    //
    //
    //
    public class SslStream : AuthenticatedStream
    {
        private SslState _SslState;
        private RemoteCertificateValidationCallback _userCertificateValidationCallback;
        private LocalCertificateSelectionCallback _userCertificateSelectionCallback;
        private object m_RemoteCertificateOrBytes;

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
                throw new ArgumentException(SR.Format(SR.net_invalid_enum, "EncryptionPolicy"), "encryptionPolicy");

            _userCertificateValidationCallback = userCertificateValidationCallback;
            _userCertificateSelectionCallback = userCertificateSelectionCallback;
            RemoteCertValidationCallback _userCertValidationCallbackWrapper = new RemoteCertValidationCallback(userCertValidationCallbackWrapper);
            LocalCertSelectionCallback _userCertSelectionCallbackWrapper = userCertificateSelectionCallback == null ? null : new LocalCertSelectionCallback(userCertSelectionCallbackWrapper);
            _SslState = new SslState(innerStream, _userCertValidationCallbackWrapper, _userCertSelectionCallbackWrapper, encryptionPolicy);
        }

        private bool userCertValidationCallbackWrapper(string hostName, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            m_RemoteCertificateOrBytes = certificate == null ? null : certificate.GetRawCertData();
            if (_userCertificateValidationCallback == null)
            {
                if (!_SslState.RemoteCertRequired)
                    sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateNotAvailable;

                return (sslPolicyErrors == SslPolicyErrors.None);
            }
            else
                return _userCertificateValidationCallback(this, certificate, chain, sslPolicyErrors);
        }

        private X509Certificate userCertSelectionCallbackWrapper(string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return _userCertificateSelectionCallback(this, targetHost, localCertificates, remoteCertificate, acceptableIssuers);
        }

        private SslProtocols DefaultProtocols()
        {
            SslProtocols protocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            return protocols;
        }

        //
        // Client side auth
        //
        public virtual void AuthenticateAsClient(string targetHost)
        {
            AuthenticateAsClient(targetHost, new X509CertificateCollection(), DefaultProtocols(), false);
        }
        //
        public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            _SslState.ValidateCreateContext(false, targetHost, enabledSslProtocols, null, clientCertificates, true, checkCertificateRevocation);
            _SslState.ProcessAuthentication(null);
        }
        //

        internal virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
        {
            return BeginAuthenticateAsClient(targetHost, new X509CertificateCollection(), DefaultProtocols(), false,
                                           asyncCallback, asyncState);
        }
        //

        internal virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates,
                                                            SslProtocols enabledSslProtocols, bool checkCertificateRevocation,
                                                            AsyncCallback asyncCallback, object asyncState)
        {
            _SslState.ValidateCreateContext(false, targetHost, enabledSslProtocols, null, clientCertificates, true, checkCertificateRevocation);

            LazyAsyncResult result = new LazyAsyncResult(_SslState, asyncState, asyncCallback);
            _SslState.ProcessAuthentication(result);
            return result;
        }
        //


        internal virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
        {
            _SslState.EndProcessAuthentication(asyncResult);
        }
        //


        //
        //server side auth
        //
        public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
        {
            AuthenticateAsServer(serverCertificate, false, DefaultProtocols(), false);
        }
        //
        public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired,
                                               SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            _SslState.ValidateCreateContext(true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired, checkCertificateRevocation);
            _SslState.ProcessAuthentication(null);
        }
        //
        internal virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback asyncCallback, object asyncState)

        {
            return BeginAuthenticateAsServer(serverCertificate, false, DefaultProtocols(), false,
                                                          asyncCallback,
                                                            asyncState);
        }
        //
        internal virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired,
                                                            SslProtocols enabledSslProtocols, bool checkCertificateRevocation,
                                                            AsyncCallback asyncCallback,
                                                            object asyncState)
        {
            _SslState.ValidateCreateContext(true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired, checkCertificateRevocation);
            LazyAsyncResult result = new LazyAsyncResult(_SslState, asyncState, asyncCallback);
            _SslState.ProcessAuthentication(result);
            return result;
        }
        //

        internal virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
        {
            _SslState.EndProcessAuthentication(asyncResult);
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
            return _SslState.GetChannelBinding(kind);
        }

        //************* Task-based async public methods *************************
        public virtual Task AuthenticateAsClientAsync(string targetHost)
        {
            return Task.Factory.FromAsync(BeginAuthenticateAsClient, EndAuthenticateAsClient, targetHost, null);
        }

        public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            return Task.Factory.FromAsync((callback, state) => BeginAuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, callback, state), EndAuthenticateAsClient, null);
        }

        public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate)
        {
            return Task.Factory.FromAsync(BeginAuthenticateAsServer, EndAuthenticateAsServer, serverCertificate, null);
        }

        public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            return Task.Factory.FromAsync((callback, state) => BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation, callback, state), EndAuthenticateAsServer, null);
        }


        //
        //
        // Base class properties
        //
        public override bool IsAuthenticated
        {
            get
            {
                return _SslState.IsAuthenticated;
            }
        }
        //
        public override bool IsMutuallyAuthenticated
        {
            get
            {
                return _SslState.IsMutuallyAuthenticated;
            }
        }
        //
        public override bool IsEncrypted
        {
            get
            {
                return IsAuthenticated;
            }
        }
        //
        public override bool IsSigned
        {
            get
            {
                return IsAuthenticated;
            }
        }
        //
        public override bool IsServer
        {
            get
            {
                return _SslState.IsServer;
            }
        }
        //
        //
        //SSL specific properties
        //
        //
        public virtual SslProtocols SslProtocol
        {
            get
            {
                return _SslState.SslProtocol;
            }
        }
        //
        public virtual bool CheckCertRevocationStatus
        {
            get
            {
                return _SslState.CheckCertRevocationStatus;
            }
        }
        //
        public virtual X509Certificate LocalCertificate
        {
            get
            {
                return _SslState.LocalCertificate;
            }
        }
        //
        public virtual X509Certificate RemoteCertificate
        {
            get
            {
                _SslState.CheckThrow(true);

                object chkCertificateOrBytes = m_RemoteCertificateOrBytes;
                if (chkCertificateOrBytes != null && chkCertificateOrBytes.GetType() == typeof(byte[]))
                    return (X509Certificate)(m_RemoteCertificateOrBytes = new X509Certificate((byte[])chkCertificateOrBytes));
                else
                    return chkCertificateOrBytes as X509Certificate;
            }
        }
        //
        // More informational properties
        //
        public virtual CipherAlgorithmType CipherAlgorithm
        {
            get
            {
                return _SslState.CipherAlgorithm;
            }
        }
        //
        public virtual int CipherStrength
        {
            get
            {
                return _SslState.CipherStrength;
            }
        }
        //
        public virtual HashAlgorithmType HashAlgorithm
        {
            get
            {
                return _SslState.HashAlgorithm;
            }
        }
        //
        public virtual int HashStrength
        {
            get
            {
                return _SslState.HashStrength;
            }
        }
        //
        public virtual ExchangeAlgorithmType KeyExchangeAlgorithm
        {
            get
            {
                return _SslState.KeyExchangeAlgorithm;
            }
        }
        //
        public virtual int KeyExchangeStrength
        {
            get
            {
                return _SslState.KeyExchangeStrength;
            }
        }
        //
        //
        // Stream contract implementation
        //
        //
        //
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        //
        public override bool CanRead
        {
            get
            {
                return _SslState.IsAuthenticated && InnerStream.CanRead;
            }
        }
        //
        public override bool CanTimeout
        {
            get
            {
                return InnerStream.CanTimeout;
            }
        }
        //
        public override bool CanWrite
        {
            get
            {
                return _SslState.IsAuthenticated && InnerStream.CanWrite;
            }
        }
        //
        //
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
        //
        //
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
        //
        public override long Length
        {
            get
            {
                return InnerStream.Length;
            }
        }
        //
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
        //
        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }
        //
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.net_noseek);
        }
        //
        public override void Flush()
        {
            _SslState.Flush();
        }
        //
        //
        protected override void Dispose(bool disposing)
        {
            try
            {
                _SslState.Close();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        //
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _SslState.SecureStream.Read(buffer, offset, count);
        }

        //
        public void Write(byte[] buffer)
        {
            _SslState.SecureStream.Write(buffer, 0, buffer.Length);
        }

        //
        public override void Write(byte[] buffer, int offset, int count)
        {
            _SslState.SecureStream.Write(buffer, offset, count);
        }
    }
}



