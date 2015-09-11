// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Internal.NativeCrypto;
using Internal.Cryptography;
using Internal.Cryptography.Pal.Native;

using FILETIME = Internal.Cryptography.Pal.Native.FILETIME;

using System.Security.Cryptography;
using SafeX509ChainHandle = Microsoft.Win32.SafeHandles.SafeX509ChainHandle;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography.Pal
{
    internal sealed partial class CertificatePal : IDisposable, ICertificatePal
    {
        public static ICertificatePal FromHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentException(SR.Arg_InvalidHandle, "handle");

            SafeCertContextHandle safeCertContextHandle = Interop.crypt32.CertDuplicateCertificateContext(handle);
            if (safeCertContextHandle.IsInvalid)
                throw new CryptographicException(ErrorCode.HRESULT_INVALID_HANDLE);

            CRYPTOAPI_BLOB dataBlob;
            int cbData = 0;
            bool deleteKeyContainer = Interop.crypt32.CertGetCertificateContextProperty(safeCertContextHandle, CertContextPropId.CERT_DELETE_KEYSET_PROP_ID, out dataBlob, ref cbData);
            return new CertificatePal(safeCertContextHandle, deleteKeyContainer);
        }

        public IntPtr Handle
        {
            get { return _certContext.DangerousGetHandle(); }
        }

        public String Issuer
        {
            get
            {
                return GetIssuerOrSubject(issuer: true);
            }
        }

        public String Subject
        {
            get
            {
                return GetIssuerOrSubject(issuer: false);
            }
        }

        public byte[] Thumbprint
        {
            get
            {
                int cbData = 0;
                if (!Interop.crypt32.CertGetCertificateContextProperty(_certContext, CertContextPropId.CERT_SHA1_HASH_PROP_ID, null, ref cbData))
                    throw new CryptographicException(Marshal.GetHRForLastWin32Error());

                byte[] thumbprint = new byte[cbData];
                if (!Interop.crypt32.CertGetCertificateContextProperty(_certContext, CertContextPropId.CERT_SHA1_HASH_PROP_ID, thumbprint, ref cbData))
                    throw new CryptographicException(Marshal.GetHRForLastWin32Error());
                return thumbprint;
            }
        }

        public String KeyAlgorithm
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    String keyAlgorithm = Marshal.PtrToStringAnsi(pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId);
                    GC.KeepAlive(this);
                    return keyAlgorithm;
                }
            }
        }

        public byte[] KeyAlgorithmParameters
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    String keyAlgorithmOid = Marshal.PtrToStringAnsi(pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId);

                    int algId;
                    if (keyAlgorithmOid == Oids.RsaRsa)
                        algId = AlgId.CALG_RSA_KEYX;  // Fast-path for the most common case.
                    else
                        algId = OidInfo.FindOidInfo(CryptOidInfoKeyType.CRYPT_OID_INFO_OID_KEY, keyAlgorithmOid, OidGroup.PublicKeyAlgorithm, fallBackToAllGroups: true).AlgId;

                    unsafe
                    {
                        byte* NULL_ASN_TAG = (byte*)0x5;

                        byte[] keyAlgorithmParameters;

                        if (algId == AlgId.CALG_DSS_SIGN
                            && pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters.cbData == 0
                            && pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters.pbData == NULL_ASN_TAG)
                        {
                            //
                            // DSS certificates may not have the DSS parameters in the certificate. In this case, we try to build
                            // the certificate chain and propagate the parameters down from the certificate chain.
                            //
                            keyAlgorithmParameters = PropagateKeyAlgorithmParametersFromChain();
                        }
                        else
                        {
                            keyAlgorithmParameters = pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters.ToByteArray();
                        }

                        GC.KeepAlive(this);
                        return keyAlgorithmParameters;
                    }
                }
            }
        }

        private byte[] PropagateKeyAlgorithmParametersFromChain()
        {
            unsafe
            {
                SafeX509ChainHandle certChainContext = null;
                try
                {
                    int cbData = 0;
                    if (!Interop.crypt32.CertGetCertificateContextProperty(_certContext, CertContextPropId.CERT_PUBKEY_ALG_PARA_PROP_ID, null, ref cbData))
                    {
                        CERT_CHAIN_PARA chainPara = new CERT_CHAIN_PARA();
                        chainPara.cbSize = sizeof(CERT_CHAIN_PARA);
                        if (!Interop.crypt32.CertGetCertificateChain(ChainEngine.HCCE_CURRENT_USER, _certContext, (FILETIME*)null, SafeCertStoreHandle.InvalidHandle, ref chainPara, CertChainFlags.None, IntPtr.Zero, out certChainContext))
                            throw new CryptographicException(Marshal.GetHRForLastWin32Error());
                        if (!Interop.crypt32.CertGetCertificateContextProperty(_certContext, CertContextPropId.CERT_PUBKEY_ALG_PARA_PROP_ID, null, ref cbData))
                            throw new CryptographicException(Marshal.GetHRForLastWin32Error());
                    }

                    byte[] keyAlgorithmParameters = new byte[cbData];
                    if (!Interop.crypt32.CertGetCertificateContextProperty(_certContext, CertContextPropId.CERT_PUBKEY_ALG_PARA_PROP_ID, keyAlgorithmParameters, ref cbData))
                        throw new CryptographicException(Marshal.GetHRForLastWin32Error());

                    return keyAlgorithmParameters;
                }
                finally
                {
                    if (certChainContext != null)
                        certChainContext.Dispose();
                }
            }
        }

        public byte[] PublicKeyValue
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    byte[] publicKey = pCertContext->pCertInfo->SubjectPublicKeyInfo.PublicKey.ToByteArray();
                    GC.KeepAlive(this);
                    return publicKey;
                }
            }
        }

        public byte[] SerialNumber
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    byte[] serialNumber = pCertContext->pCertInfo->SerialNumber.ToByteArray();
                    GC.KeepAlive(this);
                    return serialNumber;
                }
            }
        }

        public String SignatureAlgorithm
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    String signatureAlgorithm = Marshal.PtrToStringAnsi(pCertContext->pCertInfo->SignatureAlgorithm.pszObjId);
                    GC.KeepAlive(this);
                    return signatureAlgorithm;
                }
            }
        }

        public DateTime NotAfter
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    DateTime notAfter = pCertContext->pCertInfo->NotAfter.ToDateTime();
                    GC.KeepAlive(this);
                    return notAfter;
                }
            }
        }

        public DateTime NotBefore
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    DateTime notBefore = pCertContext->pCertInfo->NotBefore.ToDateTime();
                    GC.KeepAlive(this);
                    return notBefore;
                }
            }
        }

        public byte[] RawData
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    int count = pCertContext->cbCertEncoded;
                    byte[] rawData = new byte[count];
                    Marshal.Copy((IntPtr)(pCertContext->pbCertEncoded), rawData, 0, count);
                    GC.KeepAlive(this);
                    return rawData;
                }
            }
        }

        public int Version
        {
            get
            {
                unsafe
                {
                    CERT_CONTEXT* pCertContext = _certContext.CertContext;
                    int version = pCertContext->pCertInfo->dwVersion + 1;
                    GC.KeepAlive(this);
                    return version;
                }
            }
        }

        public bool Archived
        {
            get
            {
                int uninteresting = 0;
                bool archivePropertyExists = Interop.crypt32.CertGetCertificateContextProperty(_certContext, CertContextPropId.CERT_ARCHIVED_PROP_ID, null, ref uninteresting);
                return archivePropertyExists;
            }

            set
            {
                unsafe
                {
                    CRYPTOAPI_BLOB blob = new CRYPTOAPI_BLOB(0, (byte*)null);
                    CRYPTOAPI_BLOB* pValue = value ? &blob : (CRYPTOAPI_BLOB*)null;
                    if (!Interop.crypt32.CertSetCertificateContextProperty(_certContext, CertContextPropId.CERT_ARCHIVED_PROP_ID, CertSetPropertyFlags.None, pValue))
                        throw new CryptographicException(Marshal.GetLastWin32Error());
                    return;
                }
            }
        }

        public String FriendlyName
        {
            get
            {
                int cbData = 0;
                if (!Interop.crypt32.CertGetCertificateContextPropertyString(_certContext, CertContextPropId.CERT_FRIENDLY_NAME_PROP_ID, null, ref cbData))
                    return String.Empty;

                StringBuilder sb = new StringBuilder((cbData + 1) / 2);
                if (!Interop.crypt32.CertGetCertificateContextPropertyString(_certContext, CertContextPropId.CERT_FRIENDLY_NAME_PROP_ID, sb, ref cbData))
                    return String.Empty;

                return sb.ToString();
            }

            set
            {
                String friendlyName = (value == null) ? String.Empty : value;
                unsafe
                {
                    IntPtr pFriendlyName = Marshal.StringToHGlobalUni(friendlyName);
                    try
                    {
                        CRYPTOAPI_BLOB blob = new CRYPTOAPI_BLOB(checked(2 * (friendlyName.Length + 1)), (byte*)pFriendlyName);
                        if (!Interop.crypt32.CertSetCertificateContextProperty(_certContext, CertContextPropId.CERT_FRIENDLY_NAME_PROP_ID, CertSetPropertyFlags.None, &blob))
                            throw new CryptographicException(Marshal.GetLastWin32Error());
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pFriendlyName);
                    }
                }
                return;
            }
        }

        public X500DistinguishedName SubjectName
        {
            get
            {
                unsafe
                {
                    byte[] encodedSubjectName = _certContext.CertContext->pCertInfo->Subject.ToByteArray();
                    X500DistinguishedName subjectName = new X500DistinguishedName(encodedSubjectName);
                    GC.KeepAlive(this);
                    return subjectName;
                }
            }
        }

        public X500DistinguishedName IssuerName
        {
            get
            {
                unsafe
                {
                    byte[] encodedIssuerName = _certContext.CertContext->pCertInfo->Issuer.ToByteArray();
                    X500DistinguishedName issuerName = new X500DistinguishedName(encodedIssuerName);
                    GC.KeepAlive(this);
                    return issuerName;
                }
            }
        }

        public IEnumerable<X509Extension> Extensions
        {
            get
            {
                unsafe
                {
                    CERT_INFO* pCertInfo = _certContext.CertContext->pCertInfo;
                    int numExtensions = pCertInfo->cExtension;
                    X509Extension[] extensions = new X509Extension[numExtensions];
                    for (int i = 0; i < numExtensions; i++)
                    {
                        CERT_EXTENSION* pCertExtension = pCertInfo->rgExtension + i;
                        String oidValue = Marshal.PtrToStringAnsi(pCertExtension->pszObjId);
                        Oid oid = new Oid(oidValue);
                        bool critical = pCertExtension->fCritical != 0;
                        byte[] rawData = pCertExtension->Value.ToByteArray();

                        extensions[i] = new X509Extension(oid, rawData, critical);
                    }
                    GC.KeepAlive(this);
                    return extensions;
                }
            }
        }

        public String GetNameInfo(X509NameType nameType, bool forIssuer)
        {
            CertNameType certNameType = MapNameType(nameType);
            CertNameFlags certNameFlags = forIssuer ? CertNameFlags.CERT_NAME_ISSUER_FLAG : CertNameFlags.None;
            CertNameStrTypeAndFlags strType = CertNameStrTypeAndFlags.CERT_X500_NAME_STR | CertNameStrTypeAndFlags.CERT_NAME_STR_REVERSE_FLAG;

            int cchCount = Interop.crypt32.CertGetNameString(_certContext, certNameType, certNameFlags, ref strType, null, 0);
            if (cchCount == 0)
                throw new CryptographicException(Marshal.GetLastWin32Error());

            StringBuilder sb = new StringBuilder(cchCount);
            if (Interop.crypt32.CertGetNameString(_certContext, certNameType, certNameFlags, ref strType, sb, cchCount) == 0)
                throw new CryptographicException(Marshal.GetLastWin32Error());

            return sb.ToString();
        }

        public void AppendPrivateKeyInfo(StringBuilder sb)
        {
            CspKeyContainerInfo cspKeyContainerInfo = null;
            try
            {
                if (HasPrivateKey)
                {
                    CspParameters parameters = GetPrivateKey();
                    cspKeyContainerInfo = new CspKeyContainerInfo(parameters);
                }
            }
            // We could not access the key container. Just return.
            catch (CryptographicException) { }

            if (cspKeyContainerInfo == null)
                return;

            sb.Append(Environment.NewLine + Environment.NewLine + "[Private Key]");
            sb.Append(Environment.NewLine + "  Key Store: ");
            sb.Append(cspKeyContainerInfo.MachineKeyStore ? "Machine" : "User");
            sb.Append(Environment.NewLine + "  Provider Name: ");
            sb.Append(cspKeyContainerInfo.ProviderName);
            sb.Append(Environment.NewLine + "  Provider type: ");
            sb.Append(cspKeyContainerInfo.ProviderType);
            sb.Append(Environment.NewLine + "  Key Spec: ");
            sb.Append(cspKeyContainerInfo.KeyNumber);
            sb.Append(Environment.NewLine + "  Key Container Name: ");
            sb.Append(cspKeyContainerInfo.KeyContainerName);

            try
            {
                String uniqueKeyContainer = cspKeyContainerInfo.UniqueKeyContainerName;
                sb.Append(Environment.NewLine + "  Unique Key Container Name: ");
                sb.Append(uniqueKeyContainer);
            }
            catch (CryptographicException) { }
            catch (NotSupportedException) { }

            bool b = false;
            try
            {
                b = cspKeyContainerInfo.HardwareDevice;
                sb.Append(Environment.NewLine + "  Hardware Device: ");
                sb.Append(b);
            }
            catch (CryptographicException) { }

            try
            {
                b = cspKeyContainerInfo.Removable;
                sb.Append(Environment.NewLine + "  Removable: ");
                sb.Append(b);
            }
            catch (CryptographicException) { }

            try
            {
                b = cspKeyContainerInfo.Protected;
                sb.Append(Environment.NewLine + "  Protected: ");
                sb.Append(b);
            }
            catch (CryptographicException) { }
            catch (NotSupportedException) { }
        }

        public void Dispose()
        {
            SafeCertContextHandle certContext = _certContext;
            _certContext = null;
            if (certContext != null && !certContext.IsInvalid)
            {
                certContext.Dispose();
            }
            return;
        }

        internal SafeCertContextHandle CertContext
        {
            get
            {
                SafeCertContextHandle certContext = Interop.crypt32.CertDuplicateCertificateContext(_certContext.DangerousGetHandle());
                GC.KeepAlive(_certContext);
                return certContext;
            }
        }

        private static CertNameType MapNameType(X509NameType nameType)
        {
            switch (nameType)
            {
                case X509NameType.SimpleName:
                    return CertNameType.CERT_NAME_SIMPLE_DISPLAY_TYPE;

                case X509NameType.EmailName:
                    return CertNameType.CERT_NAME_EMAIL_TYPE;

                case X509NameType.UpnName:
                    return CertNameType.CERT_NAME_UPN_TYPE;

                case X509NameType.DnsName:
                case X509NameType.DnsFromAlternativeName:
                    return CertNameType.CERT_NAME_DNS_TYPE;

                case X509NameType.UrlName:
                    return CertNameType.CERT_NAME_URL_TYPE;

                default:
                    throw new ArgumentException(SR.Argument_InvalidNameType);
            }
        }

        private String GetIssuerOrSubject(bool issuer)
        {
            CertNameFlags flags = issuer ? CertNameFlags.CERT_NAME_ISSUER_FLAG : CertNameFlags.None;
            CertNameStringType stringType = CertNameStringType.CERT_X500_NAME_STR | CertNameStringType.CERT_NAME_STR_REVERSE_FLAG;

            int cchCount = Interop.crypt32.CertGetNameString(_certContext, CertNameType.CERT_NAME_RDN_TYPE, flags, ref stringType, null, 0);
            if (cchCount == 0)
                throw new CryptographicException(Marshal.GetHRForLastWin32Error());

            StringBuilder sb = new StringBuilder(cchCount);
            cchCount = Interop.crypt32.CertGetNameString(_certContext, CertNameType.CERT_NAME_RDN_TYPE, flags, ref stringType, sb, cchCount);
            if (cchCount == 0)
                throw new CryptographicException(Marshal.GetHRForLastWin32Error());

            return sb.ToString();
        }

        private CertificatePal(SafeCertContextHandle certContext, bool deleteKeyContainer)
        {
            if (deleteKeyContainer)
            {
                // We need to delete any associated key container upon disposition. Thus, replace the safehandle we got with a safehandle whose
                // Release() method performs the key container deletion.
                SafeCertContextHandle oldCertContext = certContext;
                certContext = Interop.crypt32.CertDuplicateCertificateContextWithKeyContainerDeletion(oldCertContext.DangerousGetHandle());
                GC.KeepAlive(oldCertContext);
            }
            _certContext = certContext;
            return;
        }

        private SafeCertContextHandle _certContext;
    }
}
