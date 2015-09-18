// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace System.Net
{   
    internal partial class CertModule : CertInterface
    {
        #region internal Methods
        internal override SslPolicyErrors VerifyRemoteCertName(X509Chain chain, bool isServer, string hostName)
        {
            SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;

            unsafe
            {
                uint status = 0;

                var eppStruct = new Interop.Crypt32.SSL_EXTRA_CERT_CHAIN_POLICY_PARA()
                {
                    cbSize = (uint)Marshal.SizeOf<Interop.Crypt32.SSL_EXTRA_CERT_CHAIN_POLICY_PARA>(),
                    dwAuthType = isServer ? Interop.Crypt32.AuthType.AUTHTYPE_SERVER : Interop.Crypt32.AuthType.AUTHTYPE_CLIENT,
                    fdwChecks = 0,
                    pwszServerName = null
                };

                var cppStruct = new Interop.Crypt32.CERT_CHAIN_POLICY_PARA()
                {
                    cbSize = (uint)Marshal.SizeOf<Interop.Crypt32.CERT_CHAIN_POLICY_PARA>(),
                    dwFlags = 0,
                    pvExtraPolicyPara = &eppStruct
                };

                fixed (char* namePtr = hostName)
                {
                    eppStruct.pwszServerName = namePtr;
                    cppStruct.dwFlags |= (Interop.Crypt32.CertChainPolicyIgnoreFlags.CERT_CHAIN_POLICY_IGNORE_ALL &
                                        ~Interop.Crypt32.CertChainPolicyIgnoreFlags.CERT_CHAIN_POLICY_IGNORE_INVALID_NAME_FLAG);

                    SafeX509ChainHandle chainContext = chain.SafeHandle;

                    status = Verify(chainContext, ref cppStruct);

                    if (status == Interop.Crypt32.CertChainPolicyErrors.CERT_E_CN_NO_MATCH)
                    {
                        sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
                    }
                }
            }

            return sslPolicyErrors;
        }

        //
        // Extracts a remote certificate upon request.
        //
        internal override X509Certificate2 GetRemoteCertificate(SafeDeleteContext securityContext, out X509Certificate2Collection remoteCertificateStore)
        {
            remoteCertificateStore = null;

            if (securityContext == null)
            {
                return null;
            }

            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::RemoteCertificate{get;}");
            X509Certificate2 result = null;
            SafeFreeCertContext remoteContext = null;
            try
            {
                int errorCode = SSPIWrapper.QueryContextRemoteCertificate(GlobalSSPI.SSPISecureChannel, securityContext, out remoteContext);
                if (remoteContext != null && !remoteContext.IsInvalid)
                {
                    result = new X509Certificate2(remoteContext.DangerousGetHandle());
                }
            }
            finally
            {
                if (remoteContext != null && !remoteContext.IsInvalid)
                {
                    remoteCertificateStore = UnmanagedCertificateContext.GetRemoteCertificatesFromStoreContext(remoteContext);

                    remoteContext.Dispose();
                }
            }

            if (Logging.On)
            {
                Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_remote_certificate, (result == null ? "null" : result.ToString(true))));
            }

            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::RemoteCertificate{get;}", (result == null ? "null" : result.Subject));

            return result;
        }      

        //
        // Used only by client SSL code, never returns null.
        //
        internal override string[] GetRequestCertificateAuthorities(SafeDeleteContext securityContext)
        {
            string[] issuers = Array.Empty<string>();

            object outObj;         

            int errorCode = SSPIWrapper.QueryContextIssuerList(GlobalSSPI.SSPISecureChannel, securityContext, out outObj);

            GlobalLog.Assert(errorCode == 0, "QueryContextIssuerList returned errorCode:"+errorCode);

            Interop.Secur32.IssuerListInfoEx issuerList = (Interop.Secur32.IssuerListInfoEx)outObj;

            try
            {
                if (issuerList.cIssuers > 0)
                {
                    unsafe
                    {
                        uint count = issuerList.cIssuers;
                        issuers = new string[issuerList.cIssuers];
                        Interop.Secur32._CERT_CHAIN_ELEMENT* pIL = (Interop.Secur32._CERT_CHAIN_ELEMENT*)issuerList.aIssuers.DangerousGetHandle();
                        for (int i = 0; i < count; ++i)
                        {
                            Interop.Secur32._CERT_CHAIN_ELEMENT* pIL2 = pIL + i;
                            GlobalLog.Assert(pIL2->cbSize > 0, "SecureChannel::GetIssuers()", "Interop.Secur32._CERT_CHAIN_ELEMENT size is not positive: " + pIL2->cbSize.ToString());
                            if (pIL2->cbSize > 0)
                            {
                                uint size = pIL2->cbSize;
                                byte* ptr = (byte*)(pIL2->pCertContext);
                                byte[] x = new byte[size];
                                for (int j = 0; j < size; j++)
                                {
                                    x[j] = *(ptr + j);
                                }

                                X500DistinguishedName x500DistinguishedName = new X500DistinguishedName(x);
                                issuers[i] = x500DistinguishedName.Name;
                                GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::GetIssuers() IssuerListEx[" + i + "]:" + issuers[i]);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (issuerList.aIssuers != null)
                {
                    issuerList.aIssuers.Dispose();
                }
            }

            return issuers;
        }

        #endregion

        #region Private Methods

        private uint Verify(SafeX509ChainHandle chainContext, ref Interop.Crypt32.CERT_CHAIN_POLICY_PARA cpp)
        {
            GlobalLog.Enter("SecureChannel::VerifyChainPolicy", "chainContext=" + chainContext + ", options=" + String.Format("0x{0:x}", cpp.dwFlags));
            var status = new Interop.Crypt32.CERT_CHAIN_POLICY_STATUS();
            status.cbSize = (uint)Marshal.SizeOf<Interop.Crypt32.CERT_CHAIN_POLICY_STATUS>();

            bool errorCode =  Interop.Crypt32.CertVerifyCertificateChainPolicy( (IntPtr)Interop.Crypt32.CertChainPolicy.CERT_CHAIN_POLICY_SSL, chainContext, ref cpp, ref status);

            GlobalLog.Print("SecureChannel::VerifyChainPolicy() CertVerifyCertificateChainPolicy returned: " + errorCode);
#if TRACE_VERBOSE
            GlobalLog.Print("SecureChannel::VerifyChainPolicy() error code: " + status.dwError + String.Format(" [0x{0:x8}", status.dwError) + " " + Interop.MapSecurityStatus(status.dwError) + "]");
#endif
            GlobalLog.Leave("SecureChannel::VerifyChainPolicy", status.dwError.ToString());
            return status.dwError;
        }    

        #endregion
    }

}
