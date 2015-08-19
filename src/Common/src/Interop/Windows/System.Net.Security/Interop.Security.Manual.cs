// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;

internal static partial class Interop
{
    internal enum ContextAttribute
    {
        //
        // look into <sspi.h> and <schannel.h>
        //
        Sizes = 0x00,
        Names = 0x01,
        Lifespan = 0x02,
        DceInfo = 0x03,
        StreamSizes = 0x04,
        //KeyInfo             = 0x05, must not be used, see ConnectionInfo instead
        Authority = 0x06,
        // SECPKG_ATTR_PROTO_INFO          = 7,
        // SECPKG_ATTR_PASSWORD_EXPIRY     = 8,
        // SECPKG_ATTR_SESSION_KEY         = 9,
        PackageInfo = 0x0A,
        // SECPKG_ATTR_USER_FLAGS          = 11,
        NegotiationInfo = 0x0C,
        // SECPKG_ATTR_NATIVE_NAMES        = 13,
        // SECPKG_ATTR_FLAGS               = 14,
        // SECPKG_ATTR_USE_VALIDATED       = 15,
        // SECPKG_ATTR_CREDENTIAL_NAME     = 16,
        // SECPKG_ATTR_TARGET_INFORMATION  = 17,
        // SECPKG_ATTR_ACCESS_TOKEN        = 18,
        // SECPKG_ATTR_TARGET              = 19,
        // SECPKG_ATTR_AUTHENTICATION_ID   = 20,
        UniqueBindings = 0x19,
        EndpointBindings = 0x1A,
        ClientSpecifiedSpn = 0x1B, // SECPKG_ATTR_CLIENT_SPECIFIED_TARGET = 27
        RemoteCertificate = 0x53,
        LocalCertificate = 0x54,
        RootStore = 0x55,
        IssuerListInfoEx = 0x59,
        ConnectionInfo = 0x5A,
        // SECPKG_ATTR_EAP_KEY_BLOCK        0x5b   // returns SecPkgContext_EapKeyBlock  
        // SECPKG_ATTR_MAPPED_CRED_ATTR     0x5c   // returns SecPkgContext_MappedCredAttr  
        // SECPKG_ATTR_SESSION_INFO         0x5d   // returns SecPkgContext_SessionInfo  
        // SECPKG_ATTR_APP_DATA             0x5e   // sets/returns SecPkgContext_SessionAppData  
        // SECPKG_ATTR_REMOTE_CERTIFICATES  0x5F   // returns SecPkgContext_Certificates  
        // SECPKG_ATTR_CLIENT_CERT_POLICY   0x60   // sets    SecPkgCred_ClientCertCtlPolicy  
        // SECPKG_ATTR_CC_POLICY_RESULT     0x61   // returns SecPkgContext_ClientCertPolicyResult  
        // SECPKG_ATTR_USE_NCRYPT           0x62   // Sets the CRED_FLAG_USE_NCRYPT_PROVIDER FLAG on cred group  
        // SECPKG_ATTR_LOCAL_CERT_INFO      0x63   // returns SecPkgContext_CertInfo  
        // SECPKG_ATTR_CIPHER_INFO          0x64   // returns new CNG SecPkgContext_CipherInfo  
        // SECPKG_ATTR_EAP_PRF_INFO         0x65   // sets    SecPkgContext_EapPrfInfo  
        // SECPKG_ATTR_SUPPORTED_SIGNATURES 0x66   // returns SecPkgContext_SupportedSignatures  
        // SECPKG_ATTR_REMOTE_CERT_CHAIN    0x67   // returns PCCERT_CONTEXT  
        UiInfo = 0x68, // sets SEcPkgContext_UiInfo  
    }

    internal enum Endianness
    {
        Network = 0x00,
        Native = 0x10,
    }

    internal enum CredentialUse
    {
        Inbound = 0x1,
        Outbound = 0x2,
        Both = 0x3,
    }

    internal enum BufferType
    {
        Empty = 0x00,
        Data = 0x01,
        Token = 0x02,
        Parameters = 0x03,
        Missing = 0x04,
        Extra = 0x05,
        Trailer = 0x06,
        Header = 0x07,
        Padding = 0x09,    // non-data padding
        Stream = 0x0A,
        ChannelBindings = 0x0E,
        TargetHost = 0x10,
        ReadOnlyFlag = unchecked((int)0x80000000),
        ReadOnlyWithChecksum = 0x10000000
    }

    internal enum ChainPolicyType
    {
        Base = 1,
        Authenticode = 2,
        Authenticode_TS = 3,
        SSL = 4,
        BasicConstraints = 5,
        NtAuth = 6,
    }

    internal enum IgnoreCertProblem
    {
        not_time_valid = 0x00000001,
        ctl_not_time_valid = 0x00000002,
        not_time_nested = 0x00000004,
        invalid_basic_constraints = 0x00000008,

        all_not_time_valid =
            not_time_valid |
            ctl_not_time_valid |
            not_time_nested,

        allow_unknown_ca = 0x00000010,
        wrong_usage = 0x00000020,
        invalid_name = 0x00000040,
        invalid_policy = 0x00000080,
        end_rev_unknown = 0x00000100,
        ctl_signer_rev_unknown = 0x00000200,
        ca_rev_unknown = 0x00000400,
        root_rev_unknown = 0x00000800,

        all_rev_unknown =
            end_rev_unknown |
            ctl_signer_rev_unknown |
            ca_rev_unknown |
            root_rev_unknown,
        none =
            not_time_valid |
            ctl_not_time_valid |
            not_time_nested |
            invalid_basic_constraints |
            allow_unknown_ca |
            wrong_usage |
            invalid_name |
            invalid_policy |
            end_rev_unknown |
            ctl_signer_rev_unknown |
            ca_rev_unknown |
            root_rev_unknown
    }

    internal enum CertUsage
    {
        MatchTypeAnd = 0x00,
        MatchTypeOr = 0x01,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ChainPolicyParameter
    {
        public uint cbSize;
        public uint dwFlags;
        public SSL_EXTRA_CERT_CHAIN_POLICY_PARA* pvExtraPolicyPara;

        public static readonly uint StructSize = (uint)Marshal.SizeOf<ChainPolicyParameter>();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SSL_EXTRA_CERT_CHAIN_POLICY_PARA
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct U
        {
            [FieldOffset(0)]
            internal uint cbStruct;  //DWORD
            [FieldOffset(0)]
            internal uint cbSize;    //DWORD
        };
        internal U u;
        internal int dwAuthType;  //DWORD
        internal uint fdwChecks;   //DWORD
        internal char* pwszServerName; //WCHAR* // used to check against CN=xxxx

        internal SSL_EXTRA_CERT_CHAIN_POLICY_PARA(bool amIServer)
        {
            u.cbStruct = StructSize;
            u.cbSize = StructSize;
            //#      define      AUTHTYPE_CLIENT         1
            //#      define      AUTHTYPE_SERVER         2
            dwAuthType = amIServer ? 1 : 2;
            fdwChecks = 0;
            pwszServerName = null;
        }
        static readonly uint StructSize = (uint)Marshal.SizeOf<SSL_EXTRA_CERT_CHAIN_POLICY_PARA>();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ChainPolicyStatus
    {
        public uint cbSize;
        public uint dwError;
        public uint lChainIndex;
        public uint lElementIndex;
        public void* pvExtraPolicyStatus;

        public static readonly uint StructSize = (uint)Marshal.SizeOf<ChainPolicyStatus>();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CertEnhKeyUse
    {
        public uint cUsageIdentifier;
        public void* rgpszUsageIdentifier;
#if TRAVE
        public override string ToString()
        {
            return "cUsageIdentifier=" + cUsageIdentifier.ToString() + " rgpszUsageIdentifier=" + new IntPtr(rgpszUsageIdentifier).ToString("x");
        }
#endif
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct CertUsageMatch
    {
        public CertUsage dwType;
        public CertEnhKeyUse Usage;
#if TRAVE
        public override string ToString()
        {
            return "dwType=" + dwType.ToString() + " " + Usage.ToString();
        }
#endif
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct ChainParameters
    {
        public uint cbSize;
        public CertUsageMatch RequestedUsage;
        public CertUsageMatch RequestedIssuancePolicy;
        public uint UrlRetrievalTimeout;
        public int BoolCheckRevocationFreshnessTime;
        public uint RevocationFreshnessTime;


        public static readonly uint StructSize = (uint)Marshal.SizeOf<ChainParameters>();
#if TRAVE
        public override string ToString()
        {
            return "cbSize=" + cbSize.ToString() + " " + RequestedUsage.ToString();
        }
#endif
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct _CERT_CHAIN_ELEMENT
    {
        public uint cbSize;
        public IntPtr pCertContext;
        // Since this structure is allocated by unmanaged code, we can
        // omit the fileds below since we don't need to access them
        // CERT_TRUST_STATUS   TrustStatus;
        // IntPtr                pRevocationInfo;
        // IntPtr                pIssuanceUsage;
        // IntPtr                pApplicationUsage;
    }

    // CRYPTOAPI_BLOB
    //[StructLayout(LayoutKind.Sequential)]
    //unsafe struct CryptoBlob {
    //    // public uint cbData;
    //    // public byte* pbData;
    //    public uint dataSize;
    //    public byte* dataBlob;
    //}

    // SecPkgContext_IssuerListInfoEx
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct IssuerListInfoEx
    {
        public SafeHandle aIssuers;
        public uint cIssuers;

        public unsafe IssuerListInfoEx(SafeHandle handle, byte[] nativeBuffer)
        {
            aIssuers = handle;
            fixed (byte* voidPtr = nativeBuffer)
            {
                // if this breaks on 64 bit, do the sizeof(IntPtr) trick
                cIssuers = *((uint*)(voidPtr + IntPtr.Size));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SecureCredential
    {
        /*
        typedef struct _SCHANNEL_CRED
        {
            DWORD           dwVersion;      // always SCHANNEL_CRED_VERSION
            DWORD           cCreds;
            PCCERT_CONTEXT *paCred;
            HCERTSTORE      hRootStore;

            DWORD           cMappers;
            struct _HMAPPER **aphMappers;

            DWORD           cSupportedAlgs;
            ALG_ID *        palgSupportedAlgs;

            DWORD           grbitEnabledProtocols;
            DWORD           dwMinimumCipherStrength;
            DWORD           dwMaximumCipherStrength;
            DWORD           dwSessionLifespan;
            DWORD           dwFlags;
            DWORD           reserved;
        } SCHANNEL_CRED, *PSCHANNEL_CRED;
        */

        public const int CurrentVersion = 0x4;

        public int version;
        public int cCreds;

        // ptr to an array of pointers
        // There is a hack done with this field.  AcquireCredentialsHandle requires an array of
        // certificate handles; we only ever use one.  In order to avoid pinning a one element array,
        // we copy this value onto the stack, create a pointer on the stack to the copied value,
        // and replace this field with the pointer, during the call to AcquireCredentialsHandle.
        // Then we fix it up afterwards.  Fine as long as all the SSPI credentials are not
        // supposed to be threadsafe.
        public IntPtr certContextArray;

        private readonly IntPtr rootStore;               // == always null, OTHERWISE NOT RELIABLE
        public int cMappers;
        private readonly IntPtr phMappers;               // == always null, OTHERWISE NOT RELIABLE
        public int cSupportedAlgs;
        private readonly IntPtr palgSupportedAlgs;       // == always null, OTHERWISE NOT RELIABLE
        public SchProtocols grbitEnabledProtocols;
        public int dwMinimumCipherStrength;
        public int dwMaximumCipherStrength;
        public int dwSessionLifespan;
        public SecureCredential.Flags dwFlags;
        public int reserved;

        [Flags]
        public enum Flags
        {
            Zero = 0,
            NoSystemMapper = 0x02,
            NoNameCheck = 0x04,
            ValidateManual = 0x08,
            NoDefaultCred = 0x10,
            ValidateAuto = 0x20,
            UseStrongCrypto = 0x00400000,
        }

        public SecureCredential(int version, X509Certificate certificate, SecureCredential.Flags flags, SchProtocols protocols, EncryptionPolicy policy)
        {
            // default values required for a struct
            rootStore = phMappers = palgSupportedAlgs = certContextArray = IntPtr.Zero;
            cCreds = cMappers = cSupportedAlgs = 0;

            if (policy == EncryptionPolicy.RequireEncryption)
            {
                // Prohibit null encryption cipher
                dwMinimumCipherStrength = 0;
                dwMaximumCipherStrength = 0;
            }
            else if (policy == EncryptionPolicy.AllowNoEncryption)
            {
                // Allow null encryption cipher in addition to other ciphers
                dwMinimumCipherStrength = -1;
                dwMaximumCipherStrength = 0;
            }
            else if (policy == EncryptionPolicy.NoEncryption)
            {
                // Suppress all encryption and require null encryption cipher only
                dwMinimumCipherStrength = -1;
                dwMaximumCipherStrength = -1;
            }
            else
            {
                throw new ArgumentException(SR.Format(SR.net_invalid_enum, "EncryptionPolicy"), "policy");
            }

            dwSessionLifespan = reserved = 0;
            this.version = version;
            dwFlags = flags;
            grbitEnabledProtocols = protocols;
            if (certificate != null)
            {
                certContextArray = certificate.Handle;
                cCreds = 1;
            }
        }

        [System.Diagnostics.Conditional("TRAVE")]
        internal void DebugDump()
        {
            GlobalLog.Print("SecureCredential #" + GetHashCode());
            GlobalLog.Print("    version                 = " + version);
            GlobalLog.Print("    cCreds                  = " + cCreds);
            GlobalLog.Print("    certContextArray        = " + String.Format("0x{0:x}", certContextArray));
            GlobalLog.Print("    rootStore               = " + String.Format("0x{0:x}", rootStore));
            GlobalLog.Print("    cMappers                = " + cMappers);
            GlobalLog.Print("    phMappers               = " + String.Format("0x{0:x}", phMappers));
            GlobalLog.Print("    cSupportedAlgs          = " + cSupportedAlgs);
            GlobalLog.Print("    palgSupportedAlgs       = " + String.Format("0x{0:x}", palgSupportedAlgs));
            GlobalLog.Print("    grbitEnabledProtocols   = " + String.Format("0x{0:x}", grbitEnabledProtocols));
            GlobalLog.Print("    dwMinimumCipherStrength = " + dwMinimumCipherStrength);
            GlobalLog.Print("    dwMaximumCipherStrength = " + dwMaximumCipherStrength);
            GlobalLog.Print("    dwSessionLifespan       = " + String.Format("0x{0:x}", dwSessionLifespan));
            GlobalLog.Print("    dwFlags                 = " + String.Format("0x{0:x}", dwFlags));
            GlobalLog.Print("    reserved                = " + String.Format("0x{0:x}", reserved));
        }
    } // SecureCredential

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SecurityBufferStruct
    {
        public int count;
        public BufferType type;
        public IntPtr token;

        public static readonly int Size = sizeof(SecurityBufferStruct);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe class SecurityBufferDescriptor
    {
        /*
        typedef struct _SecBufferDesc {
            ULONG        ulVersion;
            ULONG        cBuffers;
            PSecBuffer   pBuffers;
        } SecBufferDesc, * PSecBufferDesc;
        */
        public readonly int Version;
        public readonly int Count;
        public void* UnmanagedPointer;

        public SecurityBufferDescriptor(int count)
        {
            Version = 0;
            Count = count;
            UnmanagedPointer = null;
        }

        [System.Diagnostics.Conditional("TRAVE")]
        internal void DebugDump()
        {
            GlobalLog.Print("SecurityBufferDescriptor #" + Logging.HashString(this));
            GlobalLog.Print("    version             = " + Version);
            GlobalLog.Print("    count               = " + Count);
            GlobalLog.Print("    securityBufferArray = 0x" + (new IntPtr(UnmanagedPointer)).ToString("x"));
        }
    } // SecurityBufferDescriptor

    internal enum CertificateProblem
    {
        OK = 0x00000000,
        TrustNOSIGNATURE = unchecked((int)0x800B0100),
        CertEXPIRED = unchecked((int)0x800B0101),
        CertVALIDITYPERIODNESTING = unchecked((int)0x800B0102),
        CertROLE = unchecked((int)0x800B0103),
        CertPATHLENCONST = unchecked((int)0x800B0104),
        CertCRITICAL = unchecked((int)0x800B0105),
        CertPURPOSE = unchecked((int)0x800B0106),
        CertISSUERCHAINING = unchecked((int)0x800B0107),
        CertMALFORMED = unchecked((int)0x800B0108),
        CertUNTRUSTEDROOT = unchecked((int)0x800B0109),
        CertCHAINING = unchecked((int)0x800B010A),
        CertREVOKED = unchecked((int)0x800B010C),
        CertUNTRUSTEDTESTROOT = unchecked((int)0x800B010D),
        CertREVOCATION_FAILURE = unchecked((int)0x800B010E),
        CertCN_NO_MATCH = unchecked((int)0x800B010F),
        CertWRONG_USAGE = unchecked((int)0x800B0110),
        TrustEXPLICITDISTRUST = unchecked((int)0x800B0111),
        CertUNTRUSTEDCA = unchecked((int)0x800B0112),
        CertINVALIDPOLICY = unchecked((int)0x800B0113),
        CertINVALIDNAME = unchecked((int)0x800B0114),

        CryptNOREVOCATIONCHECK = unchecked((int)0x80092012),
        CryptREVOCATIONOFFLINE = unchecked((int)0x80092013),

        TrustSYSTEMERROR = unchecked((int)0x80096001),
        TrustNOSIGNERCERT = unchecked((int)0x80096002),
        TrustCOUNTERSIGNER = unchecked((int)0x80096003),
        TrustCERTSIGNATURE = unchecked((int)0x80096004),
        TrustTIMESTAMP = unchecked((int)0x80096005),
        TrustBADDIGEST = unchecked((int)0x80096010),
        TrustBASICCONSTRAINTS = unchecked((int)0x80096019),
        TrustFINANCIALCRITERIA = unchecked((int)0x8009601E),
    }

    // #define ISC_REQ_DELEGATE                0x00000001
    // #define ISC_REQ_MUTUAL_AUTH             0x00000002
    // #define ISC_REQ_REPLAY_DETECT           0x00000004
    // #define ISC_REQ_SEQUENCE_DETECT         0x00000008
    // #define ISC_REQ_CONFIDENTIALITY         0x00000010
    // #define ISC_REQ_USE_SESSION_KEY         0x00000020
    // #define ISC_REQ_PROMPT_FOR_CREDS        0x00000040
    // #define ISC_REQ_USE_SUPPLIED_CREDS      0x00000080
    // #define ISC_REQ_ALLOCATE_MEMORY         0x00000100
    // #define ISC_REQ_USE_DCE_STYLE           0x00000200
    // #define ISC_REQ_DATAGRAM                0x00000400
    // #define ISC_REQ_CONNECTION              0x00000800
    // #define ISC_REQ_CALL_LEVEL              0x00001000
    // #define ISC_REQ_FRAGMENT_SUPPLIED       0x00002000
    // #define ISC_REQ_EXTENDED_ERROR          0x00004000
    // #define ISC_REQ_STREAM                  0x00008000
    // #define ISC_REQ_INTEGRITY               0x00010000
    // #define ISC_REQ_IDENTIFY                0x00020000
    // #define ISC_REQ_NULL_SESSION            0x00040000
    // #define ISC_REQ_MANUAL_CRED_VALIDATION  0x00080000
    // #define ISC_REQ_RESERVED1               0x00100000
    // #define ISC_REQ_FRAGMENT_TO_FIT         0x00200000
    // #define ISC_REQ_HTTP                    0x10000000
    // Win7 SP1 +
    // #define ISC_REQ_UNVERIFIED_TARGET_NAME  0x20000000  

    // #define ASC_REQ_DELEGATE                0x00000001
    // #define ASC_REQ_MUTUAL_AUTH             0x00000002
    // #define ASC_REQ_REPLAY_DETECT           0x00000004
    // #define ASC_REQ_SEQUENCE_DETECT         0x00000008
    // #define ASC_REQ_CONFIDENTIALITY         0x00000010
    // #define ASC_REQ_USE_SESSION_KEY         0x00000020
    // #define ASC_REQ_ALLOCATE_MEMORY         0x00000100
    // #define ASC_REQ_USE_DCE_STYLE           0x00000200
    // #define ASC_REQ_DATAGRAM                0x00000400
    // #define ASC_REQ_CONNECTION              0x00000800
    // #define ASC_REQ_CALL_LEVEL              0x00001000
    // #define ASC_REQ_EXTENDED_ERROR          0x00008000
    // #define ASC_REQ_STREAM                  0x00010000
    // #define ASC_REQ_INTEGRITY               0x00020000
    // #define ASC_REQ_LICENSING               0x00040000
    // #define ASC_REQ_IDENTIFY                0x00080000
    // #define ASC_REQ_ALLOW_NULL_SESSION      0x00100000
    // #define ASC_REQ_ALLOW_NON_USER_LOGONS   0x00200000
    // #define ASC_REQ_ALLOW_CONTEXT_REPLAY    0x00400000
    // #define ASC_REQ_FRAGMENT_TO_FIT         0x00800000
    // #define ASC_REQ_FRAGMENT_SUPPLIED       0x00002000
    // #define ASC_REQ_NO_TOKEN                0x01000000
    // #define ASC_REQ_HTTP                    0x10000000

    [Flags]
    internal enum ContextFlags
    {
        Zero = 0,
        // The server in the transport application can
        // build new security contexts impersonating the
        // client that will be accepted by other servers
        // as the client's contexts.
        Delegate = 0x00000001,
        // The communicating parties must authenticate
        // their identities to each other. Without MutualAuth,
        // the client authenticates its identity to the server.
        // With MutualAuth, the server also must authenticate
        // its identity to the client.
        MutualAuth = 0x00000002,
        // The security package detects replayed packets and
        // notifies the caller if a packet has been replayed.
        // The use of this flag implies all of the conditions
        // specified by the Integrity flag.
        ReplayDetect = 0x00000004,
        // The context must be allowed to detect out-of-order
        // delivery of packets later through the message support
        // functions. Use of this flag implies all of the
        // conditions specified by the Integrity flag.
        SequenceDetect = 0x00000008,
        // The context must protect data while in transit.
        // Confidentiality is supported for NTLM with Microsoft
        // Windows NT version 4.0, SP4 and later and with the
        // Kerberos protocol in Microsoft Windows 2000 and later.
        Confidentiality = 0x00000010,
        UseSessionKey = 0x00000020,
        AllocateMemory = 0x00000100,

        // Connection semantics must be used.
        Connection = 0x00000800,

        // Client applications requiring extended error messages specify the
        // ISC_REQ_EXTENDED_ERROR flag when calling the InitializeSecurityContext
        // Server applications requiring extended error messages set
        // the ASC_REQ_EXTENDED_ERROR flag when calling AcceptSecurityContext.
        InitExtendedError = 0x00004000,
        AcceptExtendedError = 0x00008000,
        // A transport application requests stream semantics
        // by setting the ISC_REQ_STREAM and ASC_REQ_STREAM
        // flags in the calls to the InitializeSecurityContext
        // and AcceptSecurityContext functions
        InitStream = 0x00008000,
        AcceptStream = 0x00010000,
        // Buffer integrity can be verified; however, replayed
        // and out-of-sequence messages will not be detected
        InitIntegrity = 0x00010000,       // ISC_REQ_INTEGRITY
        AcceptIntegrity = 0x00020000,       // ASC_REQ_INTEGRITY

        InitManualCredValidation = 0x00080000,   // ISC_REQ_MANUAL_CRED_VALIDATION
        InitUseSuppliedCreds = 0x00000080,   // ISC_REQ_USE_SUPPLIED_CREDS
        InitIdentify = 0x00020000,   // ISC_REQ_IDENTIFY
        AcceptIdentify = 0x00080000,   // ASC_REQ_IDENTIFY

        ProxyBindings = 0x04000000,   // ASC_REQ_PROXY_BINDINGS
        AllowMissingBindings = 0x10000000,   // ASC_REQ_ALLOW_MISSING_BINDINGS

        UnverifiedTargetName = 0x20000000,   // ISC_REQ_UNVERIFIED_TARGET_NAME
    }

    internal class SecurityBuffer
    {
        public int size;
        public Interop.BufferType type;
        public byte[] token;
        public SafeHandle unmanagedToken;
        public int offset;

        public SecurityBuffer(byte[] data, int offset, int size, Interop.BufferType tokentype)
        {
            GlobalLog.Assert(offset >= 0 && offset <= (data == null ? 0 : data.Length), "SecurityBuffer::.ctor", "'offset' out of range.  [" + offset + "]");
            GlobalLog.Assert(size >= 0 && size <= (data == null ? 0 : data.Length - offset), "SecurityBuffer::.ctor", "'size' out of range.  [" + size + "]");

            this.offset = data == null || offset < 0 ? 0 : Math.Min(offset, data.Length);
            this.size = data == null || size < 0 ? 0 : Math.Min(size, data.Length - this.offset);
            this.type = tokentype;
            this.token = size == 0 ? null : data;
        }

        public SecurityBuffer(byte[] data, Interop.BufferType tokentype)
        {
            this.size = data == null ? 0 : data.Length;
            this.type = tokentype;
            this.token = size == 0 ? null : data;
        }

        public SecurityBuffer(int size, Interop.BufferType tokentype)
        {
            GlobalLog.Assert(size >= 0, "SecurityBuffer::.ctor", "'size' out of range.  [" + size + "]");

            this.size = size;
            this.type = tokentype;
            this.token = size == 0 ? null : new byte[size];
        }

        public SecurityBuffer(ChannelBinding binding)
        {
            this.size = (binding == null ? 0 : binding.Size);
            this.type = Interop.BufferType.ChannelBindings;
            this.unmanagedToken = binding;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct AuthIdentity
    {
        // see SEC_WINNT_AUTH_IDENTITY_W
        internal string UserName;
        internal int UserNameLength;
        internal string Domain;
        internal int DomainLength;
        internal string Password;
        internal int PasswordLength;
        internal int Flags;

        internal AuthIdentity(string userName, string password, string domain)
        {
            UserName = userName;
            UserNameLength = userName == null ? 0 : userName.Length;
            Password = password;
            PasswordLength = password == null ? 0 : password.Length;
            Domain = domain;
            DomainLength = domain == null ? 0 : domain.Length;
            // Flags are 2 for Unicode and 1 for ANSI. We use 2 on NT and 1 on Win9x.
            Flags = 2;
        }
        public override string ToString()
        {
            return Logging.ObjectToString(Domain) + "\\" + Logging.ObjectToString(UserName);
        }
    }

    // This is only for internal code path i.e. TLS stream.
    // See comments on GetNextBuffer() method below.
    //
    internal class SplitWritesState
    {
        private const int c_SplitEncryptedBuffersSize = 64 * 1024;
        private BufferOffsetSize[] _UserBuffers;
        private int _Index;
        private int _LastBufferConsumed;
        private BufferOffsetSize[] _RealBuffers;

        //
        internal SplitWritesState(BufferOffsetSize[] buffers)
        {
            _UserBuffers = buffers;
            _LastBufferConsumed = 0;
            _Index = 0;
            _RealBuffers = null;
        }
        //
        // Everything was handled
        //
        internal bool IsDone
        {
            get
            {
                if (_LastBufferConsumed != 0)
                    return false;

                for (int index = _Index; index < _UserBuffers.Length; ++index)
                    if (_UserBuffers[index].Size != 0)
                        return false;

                return true;
            }
        }
        // Encryption takes CPU and if the input is large (like 10 mb) then a delay may
        // be 30 sec or so. Hence split the ecnrypt and write operations in smaller chunks
        // up to c_SplitEncryptedBuffersSize total.
        // Note that upon return from here EncryptBuffers() may additonally split the input
        // into chunks each <= chkSecureChannel.MaxDataSize (~16k) yet it will complete them all as a single IO.
        //
        //  Returns null if done, returns the _buffers reference if everything is handled in one shot (also done)
        //
        //  Otheriwse returns subsequent BufferOffsetSize[] to encrypt and pass to base IO method
        //
        internal BufferOffsetSize[] GetNextBuffers()
        {
            int curIndex = _Index;
            int currentTotalSize = 0;
            int lastChunkSize = 0;

            int firstBufferConsumed = _LastBufferConsumed;

            for (; _Index < _UserBuffers.Length; ++_Index)
            {
                lastChunkSize = _UserBuffers[_Index].Size - _LastBufferConsumed;

                currentTotalSize += lastChunkSize;

                if (currentTotalSize > c_SplitEncryptedBuffersSize)
                {
                    lastChunkSize -= (currentTotalSize - c_SplitEncryptedBuffersSize);
                    currentTotalSize = c_SplitEncryptedBuffersSize;
                    break;
                }

                lastChunkSize = 0;
                _LastBufferConsumed = 0;
            }

            // Are we done done?
            if (currentTotalSize == 0)
                return null;

            // Do all buffers fit the limit?
            if (firstBufferConsumed == 0 && curIndex == 0 && _Index == _UserBuffers.Length)
                return _UserBuffers;

            // We do have something to split and send out
            int buffersCount = lastChunkSize == 0 ? _Index - curIndex : _Index - curIndex + 1;

            if (_RealBuffers == null || _RealBuffers.Length != buffersCount)
                _RealBuffers = new BufferOffsetSize[buffersCount];

            int j = 0;
            for (; curIndex < _Index; ++curIndex)
            {
                _RealBuffers[j++] = new BufferOffsetSize(_UserBuffers[curIndex].Buffer, _UserBuffers[curIndex].Offset + firstBufferConsumed, _UserBuffers[curIndex].Size - firstBufferConsumed, false);
                firstBufferConsumed = 0;
            }

            if (lastChunkSize != 0)
            {
                _RealBuffers[j] = new BufferOffsetSize(_UserBuffers[curIndex].Buffer, _UserBuffers[curIndex].Offset + _LastBufferConsumed, lastChunkSize, false);
                if ((_LastBufferConsumed += lastChunkSize) == _UserBuffers[_Index].Size)
                {
                    ++_Index;
                    _LastBufferConsumed = 0;
                }
            }

            return _RealBuffers;
        }
    }

    // Proxy class for linking between ICertificatePolicy <--> ICertificateDecider

    internal class CertificateChainPolicy
    {
        private const uint IgnoreUnmatchedCN = 0x00001000;

        internal static uint Verify(SafeFreeCertChain chainContext, ref ChainPolicyParameter cpp)
        {
            GlobalLog.Enter("PolicyWrapper::VerifyChainPolicy", "chainContext=" + chainContext + ", options=" + String.Format("0x{0:x}", cpp.dwFlags));
            ChainPolicyStatus status = new ChainPolicyStatus();
            status.cbSize = ChainPolicyStatus.StructSize;
            int errorCode =
                NativePKI.CertVerifyCertificateChainPolicy(
                    (IntPtr)ChainPolicyType.SSL,
                    chainContext,
                    ref cpp,
                    ref status);

            GlobalLog.Print("PolicyWrapper::VerifyChainPolicy() CertVerifyCertificateChainPolicy returned: " + errorCode);
#if TRAVE
            GlobalLog.Print("PolicyWrapper::VerifyChainPolicy() error code: " + status.dwError + String.Format(" [0x{0:x8}", status.dwError) + " " + SecureChannel.MapSecurityStatus(status.dwError) + "]");
#endif
            GlobalLog.Leave("PolicyWrapper::VerifyChainPolicy", status.dwError.ToString());
            return status.dwError;
        }

        private static IgnoreCertProblem MapErrorCode(uint errorCode)
        {
            switch ((CertificateProblem)errorCode)
            {
                case CertificateProblem.CertINVALIDNAME:
                case CertificateProblem.CertCN_NO_MATCH:
                    return IgnoreCertProblem.invalid_name;

                case CertificateProblem.CertINVALIDPOLICY:
                case CertificateProblem.CertPURPOSE:
                    return IgnoreCertProblem.invalid_policy;

                case CertificateProblem.CertEXPIRED:
                    return IgnoreCertProblem.not_time_valid | IgnoreCertProblem.ctl_not_time_valid;

                case CertificateProblem.CertVALIDITYPERIODNESTING:
                    return IgnoreCertProblem.not_time_nested;

                case CertificateProblem.CertCHAINING:
                case CertificateProblem.CertUNTRUSTEDCA:
                case CertificateProblem.CertUNTRUSTEDROOT:
                    return IgnoreCertProblem.allow_unknown_ca;

                case CertificateProblem.CertREVOKED:
                case CertificateProblem.CertREVOCATION_FAILURE:
                case CertificateProblem.CryptNOREVOCATIONCHECK:
                case CertificateProblem.CryptREVOCATIONOFFLINE:
                    return IgnoreCertProblem.all_rev_unknown;

                case CertificateProblem.CertROLE:
                case CertificateProblem.TrustBASICCONSTRAINTS:
                    return IgnoreCertProblem.invalid_basic_constraints;

                case CertificateProblem.CertWRONG_USAGE:
                    return IgnoreCertProblem.wrong_usage;

                default:
                    return 0;
            }
        }

        private uint[] GetChainErrors(string hostName, X509Chain chain, ref bool fatalError)
        {
            fatalError = false;
            SafeFreeCertChain chainContext = new SafeFreeCertChain(chain.GetChainContext());
            List<uint> certificateProblems = new List<uint>();
            unsafe
            {
                uint status = 0;
                ChainPolicyParameter cppStruct = new ChainPolicyParameter();
                cppStruct.cbSize = ChainPolicyParameter.StructSize;
                cppStruct.dwFlags = 0;

                SSL_EXTRA_CERT_CHAIN_POLICY_PARA eppStruct = new SSL_EXTRA_CERT_CHAIN_POLICY_PARA(false);
                cppStruct.pvExtraPolicyPara = &eppStruct;

                fixed (char* namePtr = hostName)
                {
                    // Always check the certificate host name (ServicePointManagerElement.CheckCertificateName == true)
                    eppStruct.pwszServerName = namePtr;

                    while (true)
                    {
                        status = Verify(chainContext, ref cppStruct);
                        uint ignoreErrorMask = (uint)MapErrorCode(status);

                        certificateProblems.Add(status);

                        if (status == 0)
                        {  // No more problems with the certificate?
                            break;          // Then break out of the callback loop
                        }

                        if (ignoreErrorMask == 0)
                        {  // Unrecognized error encountered
                            fatalError = true;
                            break;
                        }
                        else
                        {
                            cppStruct.dwFlags |= ignoreErrorMask;
                            if ((CertificateProblem)status == CertificateProblem.CertCN_NO_MATCH)
                            {
                                eppStruct.fdwChecks = IgnoreUnmatchedCN;
                            }
                        }
                    }
                }
            }

            return certificateProblems.ToArray();
        }
    }

    //
    // UnsafeNclNativeMethods.NativePKI class contains methods
    // imported from crypt32.dll.
    // They deal mainly with certificates handling when doing https://
    //
    internal static class NativePKI
    {
        [DllImport(ExternDll.CRYPT32, ExactSpelling = true, SetLastError = true)]
        internal static extern int CertVerifyCertificateChainPolicy(
            [In] IntPtr policy,
            [In] SafeFreeCertChain chainContext,
            [In] ref ChainPolicyParameter cpp,
            [In, Out] ref ChainPolicyStatus ps);
    }

    internal static class NativeNTSSPI
    {
        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static extern int EncryptMessage(
              ref SSPIHandle contextHandle,
              [In] uint qualityOfProtection,
              [In, Out] SecurityBufferDescriptor inputOutput,
              [In] uint sequenceNumber
              );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static unsafe extern int DecryptMessage(
              [In] ref SSPIHandle contextHandle,
              [In, Out] SecurityBufferDescriptor inputOutput,
              [In] uint sequenceNumber,
                   uint* qualityOfProtection
              );
    };

    internal static class SafeNetHandles
    {
        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static extern int QuerySecurityContextToken(ref SSPIHandle phContext, [Out] out SafeCloseHandle handle);

        [DllImport(ExternDll.APIMSWINCOREHANDLEL1, ExactSpelling = true, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport(ExternDll.APIMSWINCOREHEAPOBSOLETEL1, EntryPoint = "LocalAlloc", SetLastError = true)]
        internal static extern SafeLocalFreeChannelBinding LocalAllocChannelBinding(int uFlags, UIntPtr sizetdwBytes);

        [DllImport(ExternDll.APIMSWINCOREHEAPOBSOLETEL1, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr LocalFree(IntPtr handle);

        [DllImport(ExternDll.CRYPT32, ExactSpelling = true, SetLastError = true)]
        internal static extern void CertFreeCertificateChain(
            [In] IntPtr pChainContext);

        [DllImport(ExternDll.CRYPT32, ExactSpelling = true, SetLastError = true)]
        internal static extern void CertFreeCertificateChainList(
            [In] IntPtr ppChainContext);

        [DllImport(ExternDll.CRYPT32, ExactSpelling = true, SetLastError = true)]
        internal static extern bool CertFreeCertificateContext(      // Suppressing returned status check, it's always==TRUE,
            [In] IntPtr certContext);
    }

    internal static class SafeNetHandles_SECURITY
    {
        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static extern int FreeContextBuffer(
            [In] IntPtr contextBuffer);

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static extern int FreeCredentialsHandle(
              ref SSPIHandle handlePtr
              );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static extern int DeleteSecurityContext(
              ref SSPIHandle handlePtr
              );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int AcceptSecurityContext(
                  ref SSPIHandle credentialHandle,
                  [In] void* inContextPtr,
                  [In] SecurityBufferDescriptor inputBuffer,
                  [In] ContextFlags inFlags,
                  [In] Endianness endianness,
                  ref SSPIHandle outContextPtr,
                  [In, Out] SecurityBufferDescriptor outputBuffer,
                  [In, Out] ref ContextFlags attributes,
                  out long timeStamp
                  );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int QueryContextAttributesW(
            ref SSPIHandle contextHandle,
            [In] ContextAttribute attribute,
            [In] void* buffer);

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int SetContextAttributesW(
            ref SSPIHandle contextHandle,
            [In] ContextAttribute attribute,
            [In] byte[] buffer,
            [In] int bufferSize);

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal static extern int EnumerateSecurityPackagesW(
            [Out] out int pkgnum,
            [Out] out SafeFreeContextBuffer_SECURITY handle);

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal unsafe static extern int AcquireCredentialsHandleW(
                  [In] string principal,
                  [In] string moduleName,
                  [In] int usage,
                  [In] void* logonID,
                  [In] ref AuthIdentity authdata,
                  [In] void* keyCallback,
                  [In] void* keyArgument,
                  ref SSPIHandle handlePtr,
                  [Out] out long timeStamp
                  );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal unsafe static extern int AcquireCredentialsHandleW(
                  [In] string principal,
                  [In] string moduleName,
                  [In] int usage,
                  [In] void* logonID,
                  [In] IntPtr zero,
                  [In] void* keyCallback,
                  [In] void* keyArgument,
                  ref SSPIHandle handlePtr,
                  [Out] out long timeStamp
                  );

        //  Win7+
        [DllImport(ExternDll.SECUR32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal unsafe static extern int AcquireCredentialsHandleW(
                  [In] string principal,
                  [In] string moduleName,
                  [In] int usage,
                  [In] void* logonID,
                  [In] SafeSspiAuthDataHandle authdata,
                  [In] void* keyCallback,
                  [In] void* keyArgument,
                  ref SSPIHandle handlePtr,
                  [Out] out long timeStamp
                  );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal unsafe static extern int AcquireCredentialsHandleW(
                  [In] string principal,
                  [In] string moduleName,
                  [In] int usage,
                  [In] void* logonID,
                  [In] ref SecureCredential authData,
                  [In] void* keyCallback,
                  [In] void* keyArgument,
                  ref SSPIHandle handlePtr,
                  [Out] out long timeStamp
                  );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int InitializeSecurityContextW(
                  ref SSPIHandle credentialHandle,
                  [In] void* inContextPtr,
                  [In] byte* targetName,
                  [In] ContextFlags inFlags,
                  [In] int reservedI,
                  [In] Endianness endianness,
                  [In] SecurityBufferDescriptor inputBuffer,
                  [In] int reservedII,
                  ref SSPIHandle outContextPtr,
                  [In, Out] SecurityBufferDescriptor outputBuffer,
                  [In, Out] ref ContextFlags attributes,
                  out long timeStamp
                  );

        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int CompleteAuthToken(
                  [In] void* inContextPtr,
                  [In, Out] SecurityBufferDescriptor inputBuffers
                  );
    }

    internal static class SspiHelper
    {
        [DllImport(ExternDll.SECUR32, ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern SecurityStatus SspiFreeAuthIdentity(
            [In] IntPtr authData);
    }
}
