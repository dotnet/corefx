// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using Internal.NativeCrypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics.Contracts;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography {

    //
    // Public facing enumerations
    //

    /// <summary>
    ///     Flags to control how often and in which format a key is allowed to be exported
    /// </summary>
    [Flags]
    internal enum CngExportPolicies {
        None = 0x00000000,
        AllowExport = 0x00000001,                       // NCRYPT_ALLOW_EXPORT_FLAG
        AllowPlaintextExport = 0x00000002,              // NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG
        AllowArchiving = 0x00000004,                    // NCRYPT_ALLOW_ARCHIVING_FLAG
        AllowPlaintextArchiving = 0x00000008            // NCRYPT_ALLOW_PLAINTEXT_ARCHIVING_FLAG
    }

    /// <summary>
    ///     Flags controlling how the key is created
    /// </summary>
    [Flags]
    internal enum CngKeyCreationOptions
    {
        None = 0x00000000,
        MachineKey = 0x00000020,                        // NCRYPT_MACHINE_KEY_FLAG
        OverwriteExistingKey = 0x00000080               // NCRYPT_OVERWRITE_KEY_FLAG               
    }

    /// <summary>
    ///     Flags controlling how a key is opened
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Approved API exception to have an easy way to express user keys")]
    internal enum CngKeyOpenOptions
    {
        None = 0x00000000,
        UserKey = 0x00000000,
        MachineKey = 0x00000020,                        // NCRYPT_MACHINE_KEY_FLAG
        Silent = 0x00000040                             // NCRYPT_SILENT_FLAG                      
    }

    /// <summary>
    ///     Flags indicating the type of key
    /// </summary>
    [Flags]
    internal enum CngKeyTypes {
        None = 0x00000000,
        MachineKey = 0x00000020                         // NCRYPT_MACHINE_KEY_FLAG
    }

    /// <summary>
    ///     Bits defining what operations are valid to use a key with
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "Flags are defined by the native ncrypt API")]
    internal enum CngKeyUsages {
        None = 0x00000000,
        Decryption = 0x00000001,                        // NCRYPT_ALLOW_DECRYPT_FLAG
        Signing = 0x00000002,                           // NCRYPT_ALLOW_SIGNING_FLAG
        KeyAgreement = 0x00000004,                      // NCRYPT_ALLOW_KEY_AGREEMENT_FLAG
        AllUsages = 0x00ffffff                          // NCRYPT_ALLOW_ALL_USAGES
    }

    /// <summary>
    ///     Options affecting how a property is interpreted by CNG
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "Flags are defined by the native ncrypt API")]
    internal enum CngPropertyOptions {
        None = 0x00000000,
        CustomProperty = 0x40000000,                    // NCRYPT_PERSIST_ONLY_FLAG
        Persist = unchecked((int)0x80000000)            // NCRYPT_PERSIST_FLAG
    }

    /// <summary>
    ///     Levels of UI protection available for a key
    /// </summary>
    [Flags]
    internal enum CngUIProtectionLevels {
        None = 0x00000000,
        ProtectKey = 0x00000001,                        // NCRYPT_UI_PROTECT_KEY_FLAG    
        ForceHighProtection = 0x00000002                // NCRYPT_UI_FORCE_HIGH_PROTECTION_FLAG
    }

    /// <summary>
    ///     Native interop with CNG's NCrypt layer. Native definitions are in ncrypt.h
    /// </summary>
    internal static class NCryptNative {
        //
        // Enumerations
        //

        /// <summary>
        ///     Types of NCryptBuffers
        /// </summary>
        internal enum BufferType {
            KdfHashAlgorithm = 0x00000000,              // KDF_HASH_ALGORITHM
            KdfSecretPrepend = 0x00000001,              // KDF_SECRET_PREPEND
            KdfSecretAppend = 0x00000002,               // KDF_SECRET_APPEND
            KdfHmacKey = 0x00000003,                    // KDF_HMAC_KEY
            KdfTlsLabel = 0x00000004,                   // KDF_TLS_PRF_LABEL
            KdfTlsSeed = 0x00000005                     // KDF_TLS_PRF_SEED
        }

        /// <summary>
        ///     Result codes from NCrypt APIs
        /// </summary>
        internal enum ErrorCode {
            Success = 0,                                            // ERROR_SUCCESS
            BadSignature = unchecked((int)0x80090006),              // NTE_BAD_SIGNATURE
            NotFound = unchecked((int)0x80090011),                  // NTE_NOT_FOUND
            KeyDoesNotExist = unchecked((int)0x80090016),           // NTE_BAD_KEYSET
            BufferTooSmall = unchecked((int)0x80090028)             // NTE_BUFFER_TOO_SMALL
        }

        /// <summary>
        ///     Well known names of key properties
        /// </summary>
        internal static class KeyPropertyName {
            internal const string Algorithm = "Algorithm Name";                 // NCRYPT_ALGORITHM_PROPERTY
            internal const string AlgorithmGroup = "Algorithm Group";           // NCRYPT_ALGORITHM_GROUP_PROPERTY
            internal const string ExportPolicy = "Export Policy";               // NCRYPT_EXPORT_POLICY_PROPERTY
            internal const string KeyType = "Key Type";                         // NCRYPT_KEY_TYPE_PROPERTY
            internal const string KeyUsage = "Key Usage";                       // NCRYPT_KEY_USAGE_PROPERTY
            internal const string Length = "Length";                            // NCRYPT_LENGTH_PROPERTY
            internal const string Name = "Name";                                // NCRYPT_NAME_PROPERTY
            internal const string ParentWindowHandle = "HWND Handle";           // NCRYPT_WINDOW_HANDLE_PROPERTY
            internal const string ProviderHandle = "Provider Handle";           // NCRYPT_PROVIDER_HANDLE_PROPERTY
            internal const string UIPolicy = "UI Policy";                       // NCRYPT_UI_POLICY_PROPERTY
            internal const string UniqueName = "Unique Name";                   // NCRYPT_UNIQUE_NAME_PROPERTY
            internal const string UseContext = "Use Context";                   // NCRYPT_USE_CONTEXT_PROPERTY

            //
            // Properties defined by the CLR
            //

            /// <summary>
            ///     Is the key a CLR created ephemeral key, it will contain a single byte with value 1 if the
            ///     key was created by the CLR as an ephemeral key.
            /// </summary>
            internal const string ClrIsEphemeral = "CLR IsEphemeral";
        }

        /// <summary>
        ///     Well known names of provider properties
        /// </summary>
        internal static class ProviderPropertyName {
            internal const string Name = "Name";        // NCRYPT_NAME_PROPERTY
        }

        /// <summary>
        ///     Flags for code:System.Security.Cryptography.NCryptNative.UnsafeNativeMethods.NCryptSecretAgreement
        /// </summary>
        [Flags]
        internal enum SecretAgreementFlags {
            None = 0x00000000,
            UseSecretAsHmacKey = 0x00000001             // KDF_USE_SECRET_AS_HMAC_KEY_FLAG
        }

        //
        // Structures
        //

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCRYPT_UI_POLICY {
            public int dwVersion;
            public CngUIProtectionLevels dwFlags;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCreationTitle;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszFriendlyName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCryptBuffer {
            public int cbBuffer;
            public BufferType BufferType;
            public IntPtr pvBuffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCryptBufferDesc {
            public int ulVersion;
            public int cBuffers;
            public IntPtr pBuffers;         // NCryptBuffer[cBuffers]
        }

#pragma warning disable 618 // System.Core.dll still uses SecurityRuleSet.Level1
        [SecurityCritical]
#pragma warning restore 618
        internal static class UnsafeNativeMethods {
            const string NCRYPT_DLL = "ncrypt.dll";
            /// <summary>
            ///     Create an NCrypt key
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptCreatePersistedKey(SafeNCryptProviderHandle hProvider,
                                                                      [Out] out SafeNCryptKeyHandle phKey,
                                                                      string pszAlgId,
                                                                      string pszKeyName,
                                                                      int dwLegacyKeySpec,
                                                                      CngKeyCreationOptions dwFlags);

            /// <summary>
            ///     Delete a key
            /// </summary>
            [DllImport(NCRYPT_DLL)]
            internal static extern ErrorCode NCryptDeleteKey(SafeNCryptKeyHandle hKey, int flags);

            /// <summary>
            ///     Generate a key from a secret agreement
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptDeriveKey(SafeNCryptSecretHandle hSharedSecret,
                                                             string pwszKDF,
                                                             [In] ref NCryptBufferDesc pParameterList,
                                                             [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbDerivedKey,
                                                             int cbDerivedKey,
                                                             [Out] out int pcbResult,
                                                             SecretAgreementFlags dwFlags);

            /// <summary>
            ///     Export a key from the KSP
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptExportKey(SafeNCryptKeyHandle hKey,
                                                             IntPtr hExportKey,               // NCRYPT_KEY_HANDLE
                                                             string pszBlobType,
                                                             IntPtr pParameterList,           // NCryptBufferDesc *
                                                             [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                             int cbOutput,
                                                             [Out] out int pcbResult,
                                                             int dwFlags);

            /// <summary>
            ///     Finalize a key to prepare it for use
            /// </summary>
            [DllImport(NCRYPT_DLL)]
            internal static extern ErrorCode NCryptFinalizeKey(SafeNCryptKeyHandle hKey, int dwFlags);

            /// <summary>
            ///     Get the value of a property of an NCrypt object
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptGetProperty(SafeNCryptHandle hObject,
                                                               string pszProperty,
                                                               [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                               int cbOutput,
                                                               [Out] out int pcbResult,
                                                               CngPropertyOptions dwFlags);

            /// <summary>
            ///     Get the value of a pointer property of an NCrypt object
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptGetProperty(SafeNCryptHandle hObject,
                                                               string pszProperty,
                                                               [Out] out IntPtr pbOutput,
                                                               int cbOutput,
                                                               [Out] out int pcbResult,
                                                               CngPropertyOptions dwFlags);

            /// <summary>
            ///     Import a key into the KSP
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptImportKey(SafeNCryptProviderHandle hProvider,
                                                             IntPtr hImportKey,     // NCRYPT_KEY_HANDLE
                                                             string pszBlobType,
                                                             IntPtr pParameterList, // NCryptBufferDesc *
                                                             [Out] out SafeNCryptKeyHandle phKey,
                                                             [MarshalAs(UnmanagedType.LPArray)] byte[] pbData,
                                                             int cbData,
                                                             int dwFlags);
                                
            /// <summary>
            ///     Open an existing key
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptOpenKey(SafeNCryptProviderHandle hProvider,
                                                           [Out] out SafeNCryptKeyHandle phKey,
                                                           string pszKeyName,
                                                           int dwLegacyKeySpec,
                                                           CngKeyOpenOptions dwFlags);

            /// <summary>
            ///     Acquire a handle to a key storage provider
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptOpenStorageProvider([Out] out SafeNCryptProviderHandle phProvider,
                                                                       string pszProviderName,
                                                                       int dwFlags);

            /// <summary>
            ///     Generate a secret agreement for generating shared key material
            /// </summary>
            [DllImport(NCRYPT_DLL)]
            internal static extern ErrorCode NCryptSecretAgreement(SafeNCryptKeyHandle hPrivKey,
                                                                   SafeNCryptKeyHandle hPubKey,
                                                                   [Out] out SafeNCryptSecretHandle phSecret,
                                                                   int dwFlags);

            /// <summary>
            ///     Set a property value on an NCrypt object
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptSetProperty(SafeNCryptHandle hObject,
                                                               string pszProperty,
                                                               [MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                               int cbInput,
                                                               CngPropertyOptions dwFlags);

            /// <summary>
            ///     Set a string property value on an NCrypt object
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptSetProperty(SafeNCryptHandle hObject,
                                                               string pszProperty,
                                                               string pbInput,
                                                               int cbInput,
                                                               CngPropertyOptions dwFlags);

            /// <summary>
            ///     Set a property value on an NCrypt object when a pointer to the buffer already exists in
            ///     managed code. To set a pointer valued property, use the ref IntPtr overload.
            /// </summary>
            [DllImport(NCRYPT_DLL, CharSet = CharSet.Unicode)]
            internal static extern ErrorCode NCryptSetProperty(SafeNCryptHandle hObject,
                                                               string pszProperty,
                                                               IntPtr pbInput,
                                                               int cbInput,
                                                               CngPropertyOptions dwFlags);

            /// <summary>
            ///     Create a signature for a hash value
            /// </summary>
            [DllImport(NCRYPT_DLL)]
            internal static extern ErrorCode NCryptSignHash(SafeNCryptKeyHandle hKey,
                                                            IntPtr pPaddingInfo,
                                                            [MarshalAs(UnmanagedType.LPArray)] byte[] pbHashValue,
                                                            int cbHashValue,
                                                            [MarshalAs(UnmanagedType.LPArray)] byte[] pbSignature,
                                                            int cbSignature,
                                                            [Out] out int pcbResult,
                                                            int dwFlags);

            /// <summary>
            ///     Verify a signature over a hash value
            /// </summary>
            [DllImport(NCRYPT_DLL)]
            internal static extern ErrorCode NCryptVerifySignature(SafeNCryptKeyHandle hKey,
                                                                   IntPtr pPaddingInfo,
                                                                   [MarshalAs(UnmanagedType.LPArray)] byte[] pbHashValue,
                                                                   int cbHashValue,
                                                                   [MarshalAs(UnmanagedType.LPArray)] byte[] pbSignature,
                                                                   int cbSignature,
                                                                   int dwFlags);
        }

        //
        // Utility and wrapper functions
        //

        /// <summary>
        ///     Determine if NCrypt is supported on the current machine
        /// </summary>
        internal static bool NCryptSupported {
            [SecuritySafeCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Reviewed")]
            get {
                // TFS 1114292: do we support CNG everywhere this code will run?
                return true;
            }
        }
        /// <summary>
        ///     Build an ECC public key blob to represent the given parameters
        /// </summary>
        internal static byte[] BuildEccPublicBlob(string algorithm, BigInteger x, BigInteger y) {
            Contract.Requires(!String.IsNullOrEmpty(algorithm));
            Contract.Ensures(Contract.Result<byte[]>() != null);

            //
            // #ECCPublicBlobFormat
            // The ECC public key blob format is as follows:
            //
            // DWORD dwMagic
            // DWORD cbKey
            // X parameter (cbKey bytes long, byte-reversed)
            // Y parameter (cbKey bytes long, byte-reversed)
            //

            // First map the algorithm name to its magic number and key size
            BCryptNative.KeyBlobMagicNumber algorithmMagic;
            int keySize;
            BCryptNative.MapAlgorithmIdToMagic(algorithm, out algorithmMagic, out keySize);

            // Next generate the public key parameters
            byte[] xBytes = ReverseBytes(FillKeyParameter(x.ToByteArray(), keySize));
            byte[] yBytes = ReverseBytes(FillKeyParameter(y.ToByteArray(), keySize));

            // Finally, lay out the structure itself
            byte[] blob = new byte[2 * sizeof(int) + xBytes.Length + yBytes.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((int)algorithmMagic), 0, blob, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(xBytes.Length), 0, blob, sizeof(int), sizeof(int));
            Buffer.BlockCopy(xBytes, 0, blob, 2 * sizeof(int), xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, blob, 2 * sizeof(int) + xBytes.Length, yBytes.Length);

            return blob;
        }

        /// <summary>
        ///     Create a random CNG key
        /// </summary>
        [System.Security.SecurityCritical]
        internal static SafeNCryptKeyHandle CreatePersistedKey(SafeNCryptProviderHandle provider,
                                                               string algorithm,
                                                               string name,
                                                               CngKeyCreationOptions options) {
            Contract.Requires(provider != null && !provider.IsInvalid && !provider.IsClosed);
            Contract.Requires(!String.IsNullOrEmpty(algorithm));
            Contract.Ensures(Contract.Result<SafeNCryptKeyHandle>() != null &&
                             !Contract.Result<SafeNCryptKeyHandle>().IsInvalid &&
                             !Contract.Result<SafeNCryptKeyHandle>().IsClosed);

            SafeNCryptKeyHandle keyHandle = null;
            ErrorCode error = UnsafeNativeMethods.NCryptCreatePersistedKey(provider,
                                                                           out keyHandle,
                                                                           algorithm,
                                                                           name,
                                                                           0,
                                                                           options);
            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return keyHandle;
        }

        /// <summary>
        ///     Delete a key
        /// </summary>
        [System.Security.SecurityCritical]
        internal static void DeleteKey(SafeNCryptKeyHandle key) {
            Contract.Requires(key != null);

            ErrorCode error = UnsafeNativeMethods.NCryptDeleteKey(key, 0);
            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            // Key handles are no longer valid after deleting
            key.Dispose();
        }

        /// <summary>
        ///     Derive key material from a hash or HMAC KDF
        /// </summary>
        /// <returns></returns>
        [System.Security.SecurityCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Reviewed")]
        private static byte[] DeriveKeyMaterial(SafeNCryptSecretHandle secretAgreement,
                                                string kdf,
                                                string hashAlgorithm,
                                                byte[] hmacKey,
                                                byte[] secretPrepend,
                                                byte[] secretAppend,
                                                SecretAgreementFlags flags) {
            Contract.Requires(secretAgreement != null);
            Contract.Requires(!String.IsNullOrEmpty(kdf));
            Contract.Requires(!String.IsNullOrEmpty(hashAlgorithm));
            Contract.Requires(hmacKey == null || kdf == BCryptNative.KeyDerivationFunction.Hmac);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            List<NCryptBuffer> parameters = new List<NCryptBuffer>();

            // First marshal the hash algoritm
            IntPtr hashAlgorithmString = IntPtr.Zero;

            try {
                hashAlgorithmString = Marshal.StringToCoTaskMemUni(hashAlgorithm);

                // We always need to marshal the hashing function
                NCryptBuffer hashAlgorithmBuffer = new NCryptBuffer();
                hashAlgorithmBuffer.cbBuffer = (hashAlgorithm.Length + 1) * sizeof(char);
                hashAlgorithmBuffer.BufferType = BufferType.KdfHashAlgorithm;
                hashAlgorithmBuffer.pvBuffer = hashAlgorithmString;
                parameters.Add(hashAlgorithmBuffer);

                unsafe {
                    fixed (byte* pHmacKey = hmacKey, pSecretPrepend = secretPrepend, pSecretAppend = secretAppend) {
                        //
                        // Now marshal the other parameters
                        //

                        if (pHmacKey != null) {
                            NCryptBuffer hmacKeyBuffer = new NCryptBuffer();
                            hmacKeyBuffer.cbBuffer = hmacKey.Length;
                            hmacKeyBuffer.BufferType = BufferType.KdfHmacKey;
                            hmacKeyBuffer.pvBuffer = new IntPtr(pHmacKey);
                            parameters.Add(hmacKeyBuffer);
                        }

                        if (pSecretPrepend != null) {
                            NCryptBuffer secretPrependBuffer = new NCryptBuffer();
                            secretPrependBuffer.cbBuffer = secretPrepend.Length;
                            secretPrependBuffer.BufferType = BufferType.KdfSecretPrepend;
                            secretPrependBuffer.pvBuffer = new IntPtr(pSecretPrepend);
                            parameters.Add(secretPrependBuffer);
                        }

                        if (pSecretAppend != null) {
                            NCryptBuffer secretAppendBuffer = new NCryptBuffer();
                            secretAppendBuffer.cbBuffer = secretAppend.Length;
                            secretAppendBuffer.BufferType = BufferType.KdfSecretAppend;
                            secretAppendBuffer.pvBuffer = new IntPtr(pSecretAppend);
                            parameters.Add(secretAppendBuffer);
                        }

                        return DeriveKeyMaterial(secretAgreement,
                                                 kdf,
                                                 parameters.ToArray(),
                                                 flags);
                    }
                }
            }
            finally {
                if (hashAlgorithmString != IntPtr.Zero) {
                    Marshal.FreeCoTaskMem(hashAlgorithmString);
                }
            }
        }

        /// <summary>
        ///     Derive key material using a given KDF and secret agreement
        /// </summary>
        [System.Security.SecurityCritical]
        private static byte[] DeriveKeyMaterial(SafeNCryptSecretHandle secretAgreement,
                                                string kdf,
                                                NCryptBuffer[] parameters,
                                                SecretAgreementFlags flags) {
            Contract.Requires(secretAgreement != null);
            Contract.Requires(!String.IsNullOrEmpty(kdf));
            Contract.Requires(parameters != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            unsafe {
                fixed (NCryptBuffer* pParameters = parameters) {
                    NCryptBufferDesc parameterDesc = new NCryptBufferDesc();
                    parameterDesc.ulVersion = 0;
                    parameterDesc.cBuffers = parameters.Length;
                    parameterDesc.pBuffers = new IntPtr(pParameters);

                    // Figure out how big the key material is
                    int keySize = 0;
                    ErrorCode error = UnsafeNativeMethods.NCryptDeriveKey(secretAgreement,
                                                                          kdf,
                                                                          ref parameterDesc,
                                                                          null,
                                                                          0,
                                                                          out keySize,
                                                                          flags);
                    if (error != ErrorCode.Success && error != ErrorCode.BufferTooSmall) {
                        throw new CryptographicException((int)error);
                    }

                    // Allocate memory for the key material and generate it
                    byte[] keyMaterial = new byte[keySize];
                    error = UnsafeNativeMethods.NCryptDeriveKey(secretAgreement,
                                                                kdf,
                                                                ref parameterDesc,
                                                                keyMaterial,
                                                                keyMaterial.Length,
                                                                out keySize,
                                                                flags);

                    if (error != ErrorCode.Success) {
                        throw new CryptographicException((int)error);
                    }

                    return keyMaterial;
                }
            }
        }

        /// <summary>
        ///     Derive key material from a secret agreement using a hash KDF
        /// </summary>
        [System.Security.SecurityCritical]
        internal static byte[] DeriveKeyMaterialHash(SafeNCryptSecretHandle secretAgreement,
                                                     string hashAlgorithm,
                                                     byte[] secretPrepend,
                                                     byte[] secretAppend,
                                                     SecretAgreementFlags flags) {
            Contract.Requires(secretAgreement != null);
            Contract.Requires(!String.IsNullOrEmpty(hashAlgorithm));
            Contract.Ensures(Contract.Result<byte[]>() != null);

            return DeriveKeyMaterial(secretAgreement,
                                     BCryptNative.KeyDerivationFunction.Hash,
                                     hashAlgorithm,
                                     null,
                                     secretPrepend,
                                     secretAppend,
                                     flags);
        }

        /// <summary>
        ///     Derive key material from a secret agreement using a HMAC KDF
        /// </summary>
        [System.Security.SecurityCritical]
        internal static byte[] DeriveKeyMaterialHmac(SafeNCryptSecretHandle secretAgreement,
                                                     string hashAlgorithm,
                                                     byte[] hmacKey,
                                                     byte[] secretPrepend,
                                                     byte[] secretAppend,
                                                     SecretAgreementFlags flags) {
            Contract.Requires(secretAgreement != null);
            Contract.Requires(!String.IsNullOrEmpty(hashAlgorithm));
            Contract.Ensures(Contract.Result<byte[]>() != null);

            return DeriveKeyMaterial(secretAgreement,
                                     BCryptNative.KeyDerivationFunction.Hmac,
                                     hashAlgorithm,
                                     hmacKey,
                                     secretPrepend,
                                     secretAppend,
                                     flags);
        }

        /// <summary>
        ///     Derive key material from a secret agreeement using the TLS KDF
        /// </summary>
        [System.Security.SecurityCritical]
        internal static byte[] DeriveKeyMaterialTls(SafeNCryptSecretHandle secretAgreement,
                                                    byte[] label,
                                                    byte[] seed,
                                                    SecretAgreementFlags flags) {
            Contract.Requires(secretAgreement != null);
            Contract.Requires(label != null && seed != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            NCryptBuffer[] buffers = new NCryptBuffer[2];

            unsafe {
                fixed (byte* pLabel = label, pSeed = seed) {
                    NCryptBuffer labelBuffer = new NCryptBuffer();
                    labelBuffer.cbBuffer = label.Length;
                    labelBuffer.BufferType = BufferType.KdfTlsLabel;
                    labelBuffer.pvBuffer = new IntPtr(pLabel);
                    buffers[0] = labelBuffer;

                    NCryptBuffer seedBuffer = new NCryptBuffer();
                    seedBuffer.cbBuffer = seed.Length;
                    seedBuffer.BufferType = BufferType.KdfTlsSeed;
                    seedBuffer.pvBuffer = new IntPtr(pSeed);
                    buffers[1] = seedBuffer;

                    return DeriveKeyMaterial(secretAgreement,
                                             BCryptNative.KeyDerivationFunction.Tls,
                                             buffers,
                                             flags);
                }
            }
        }

        /// <summary>
        ///     Generate a secret agreement value for between two parties
        /// </summary>
        [System.Security.SecurityCritical]
        internal static SafeNCryptSecretHandle DeriveSecretAgreement(SafeNCryptKeyHandle privateKey,
                                                                     SafeNCryptKeyHandle otherPartyPublicKey) {
            Contract.Requires(privateKey != null);
            Contract.Requires(otherPartyPublicKey != null);
            Contract.Ensures(Contract.Result<SafeNCryptSecretHandle>() != null &&
                             !Contract.Result<SafeNCryptSecretHandle>().IsClosed &&
                             !Contract.Result<SafeNCryptSecretHandle>().IsInvalid);

            SafeNCryptSecretHandle secretAgreement;
            ErrorCode error = UnsafeNativeMethods.NCryptSecretAgreement(privateKey,
                                                                        otherPartyPublicKey,
                                                                        out secretAgreement,
                                                                        0);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return secretAgreement;
        }

        /// <summary>
        ///     Export a key from the KSP
        /// </summary>
        [System.Security.SecurityCritical]
        internal static byte[] ExportKey(SafeNCryptKeyHandle key, string format) {
            Contract.Requires(key != null);
            Contract.Requires(!String.IsNullOrEmpty(format));
            Contract.Ensures(Contract.Result<byte[]>() != null);

            // Figure out how big of a buffer we need to export into
            int bufferSize = 0;
            ErrorCode error = UnsafeNativeMethods.NCryptExportKey(key,
                                                                  IntPtr.Zero,
                                                                  format,
                                                                  IntPtr.Zero,
                                                                  null,
                                                                  0,
                                                                  out bufferSize,
                                                                  0);

            if (error != ErrorCode.Success && error != ErrorCode.BufferTooSmall) {
                throw new CryptographicException((int)error);
            }

            // Export the key
            Debug.Assert(bufferSize > 0, "bufferSize > 0");
            byte[] keyBlob = new byte[bufferSize];
            error = UnsafeNativeMethods.NCryptExportKey(key,
                                                        IntPtr.Zero,
                                                        format,
                                                        IntPtr.Zero,
                                                        keyBlob,
                                                        keyBlob.Length,
                                                        out bufferSize,
                                                        0);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return keyBlob;
        }

        /// <summary>
        ///     Make sure that a key is padded out to be its full size
        /// </summary>
        private static byte[] FillKeyParameter(byte[] key, int keySize) {
            Contract.Requires(key != null);
            Contract.Requires(keySize > 0);
            Contract.Ensures(Contract.Result<byte[]>() != null && Contract.Result<byte[]>().Length >= keySize / 8);

            int bytesRequired = (keySize / 8) + (keySize % 8 == 0 ? 0 : 1);
            if (key.Length == bytesRequired) {
                return key;
            }

#if DEBUG
            // If the key is longer than required, it should have been padded out with zeros
            if (key.Length > bytesRequired) {
                for (int i = bytesRequired; i < key.Length; i++) {
                    Debug.Assert(key[i] == 0, "key[i] == 0");
                }
            }
#endif
            byte[] fullKey = new byte[bytesRequired];
            Buffer.BlockCopy(key, 0, fullKey, 0, Math.Min(key.Length, fullKey.Length));
            return fullKey;
        }

        /// <summary>
        ///     Finalize a key and prepare it for use
        /// </summary>
        [System.Security.SecurityCritical]
        internal static void FinalizeKey(SafeNCryptKeyHandle key) {
            Contract.Requires(key != null && !key.IsInvalid && !key.IsClosed);

            ErrorCode error = UnsafeNativeMethods.NCryptFinalizeKey(key, 0);
            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }
        }

        /// <summary>
        ///     Get the value of an NCrypt property
        /// </summary>
        [System.Security.SecurityCritical]
        internal static byte[] GetProperty(SafeNCryptHandle ncryptObject,
                                           string propertyName,
                                           CngPropertyOptions propertyOptions,
                                           out bool foundProperty) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            // Find out how big of a buffer we need to store the property in
            int bufferSize = 0;
            ErrorCode error = UnsafeNativeMethods.NCryptGetProperty(ncryptObject,
                                                                    propertyName,
                                                                    null,
                                                                    0,
                                                                    out bufferSize,
                                                                    propertyOptions);

            //
            // NTE_NOT_FOUND means this property does not exist, any other error besides NTE_BUFFER_TOO_SMALL
            // indicates a real problem.
            //

            if (error != ErrorCode.Success && error != ErrorCode.BufferTooSmall && error != ErrorCode.NotFound) {
                    throw new CryptographicException((int)error);
            }

            foundProperty = error != ErrorCode.NotFound;

            // Pull back the property value
            byte[] value = null;
            if (error != ErrorCode.NotFound && bufferSize > 0) {
                value = new byte[bufferSize];
                error = UnsafeNativeMethods.NCryptGetProperty(ncryptObject,
                                                              propertyName,
                                                              value,
                                                              value.Length,
                                                              out bufferSize,
                                                              propertyOptions);
                if (error != ErrorCode.Success) {
                    throw new CryptographicException((int)error);
                }

                foundProperty = true;
            }

            return value;
        }

        /// <summary>
        ///     Get the value of a DWORD NCrypt property
        /// </summary>
        [System.Security.SecurityCritical]
        internal static int GetPropertyAsDWord(SafeNCryptHandle ncryptObject,
                                               string propertyName,
                                               CngPropertyOptions propertyOptions) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            bool foundProperty;
            byte[] valueBytes = GetProperty(ncryptObject, propertyName, propertyOptions, out foundProperty);

            if (!foundProperty || valueBytes == null) {
                return 0;
            }
            else {
                return BitConverter.ToInt32(valueBytes, 0);
            }
        }

        /// <summary>
        ///     Get the value of a pointer NCrypt property
        /// </summary>
        [SecurityCritical]
        internal static IntPtr GetPropertyAsIntPtr(SafeNCryptHandle ncryptObject,
                                                   string propertyName,
                                                   CngPropertyOptions propertyOptions) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            // Find out how big of a buffer we need to store the property in
            int bufferSize = IntPtr.Size;
            IntPtr value = IntPtr.Zero;
            ErrorCode error = UnsafeNativeMethods.NCryptGetProperty(ncryptObject,
                                                                    propertyName,
                                                                    out value,
                                                                    IntPtr.Size,
                                                                    out bufferSize,
                                                                    propertyOptions);

            // NTE_NOT_FOUND means this property was not set, so return a NULL pointer
            if (error == ErrorCode.NotFound) {
                return IntPtr.Zero;
            }

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return value;
        }

        /// <summary>
        ///     Get the value of a string NCrypt property
        /// </summary>
        [System.Security.SecurityCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Reviewed")]
        internal static string GetPropertyAsString(SafeNCryptHandle ncryptObject,
                                                   string propertyName,
                                                   CngPropertyOptions propertyOptions) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            bool foundProperty;
            byte[] valueBytes = GetProperty(ncryptObject, propertyName, propertyOptions, out foundProperty);

            if (!foundProperty || valueBytes == null) {
                return null;
            }
            else if (valueBytes.Length == 0) {
                return String.Empty;
            }
            else {
                unsafe {
                    fixed (byte* pValueBytes = valueBytes) {
                        return Marshal.PtrToStringUni(new IntPtr(pValueBytes));
                    }
                }
            }
        }

        /// <summary>
        ///     Get the value of an NCrypt structure property -- this will return an empty structure if the
        ///     property does not exist.
        /// </summary>
        [System.Security.SecurityCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Reviewed")]
        internal static T GetPropertyAsStruct<T>(SafeNCryptHandle ncryptObject,
                                                 string propertyName,
                                                 CngPropertyOptions propertyOptions) where T : struct {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            bool foundProperty;
            byte[] valueBytes = GetProperty(ncryptObject, propertyName, propertyOptions, out foundProperty);

            if (!foundProperty || valueBytes == null) {
                return new T();
            }

            unsafe {
                fixed (byte *pValue = valueBytes) {
                    return (T)Marshal.PtrToStructure(new IntPtr(pValue), typeof(T));
                }
            }
        }

        /// <summary>
        ///     Import a key into the KSP
        /// </summary>
        [System.Security.SecurityCritical]
        internal static SafeNCryptKeyHandle ImportKey(SafeNCryptProviderHandle provider,
                                                      byte[] keyBlob,
                                                      string format) {
            Contract.Requires(provider != null);
            Contract.Requires(keyBlob != null);
            Contract.Requires(!String.IsNullOrEmpty(format));
            Contract.Ensures(Contract.Result<SafeNCryptKeyHandle>() != null &&
                             !Contract.Result<SafeNCryptKeyHandle>().IsInvalid &&
                             !Contract.Result<SafeNCryptKeyHandle>().IsClosed);

            SafeNCryptKeyHandle keyHandle = null;
            ErrorCode error = UnsafeNativeMethods.NCryptImportKey(provider,
                                                                  IntPtr.Zero,
                                                                  format,
                                                                  IntPtr.Zero,
                                                                  out keyHandle,
                                                                  keyBlob,
                                                                  keyBlob.Length,
                                                                  0);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return keyHandle;
        }

        /// <summary>
        ///     Open an existing key
        /// </summary>
        [System.Security.SecurityCritical]
        internal static SafeNCryptKeyHandle OpenKey(SafeNCryptProviderHandle provider,
                                                    string name,
                                                    CngKeyOpenOptions options) {
            Contract.Requires(provider != null && !provider.IsInvalid && !provider.IsClosed);
            Contract.Requires(name != null);

            SafeNCryptKeyHandle key = null;
            ErrorCode error = UnsafeNativeMethods.NCryptOpenKey(provider, out key, name, 0, options);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return key;
        }

        /// <summary>
        ///     Open the specified key storage provider
        /// </summary>
        [System.Security.SecurityCritical]
        internal static SafeNCryptProviderHandle OpenStorageProvider(string providerName) {
            Contract.Requires(!String.IsNullOrEmpty(providerName));
            Contract.Ensures(Contract.Result<SafeNCryptProviderHandle>() != null &&
                             !Contract.Result<SafeNCryptProviderHandle>().IsInvalid &&
                             !Contract.Result<SafeNCryptProviderHandle>().IsClosed);

            SafeNCryptProviderHandle providerHandle = null;
            ErrorCode error = UnsafeNativeMethods.NCryptOpenStorageProvider(out providerHandle,
                                                                            providerName,
                                                                            0);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return providerHandle;
        }

        /// <summary>
        ///     Reverse the bytes in a buffer
        /// </summary>
        private static byte[] ReverseBytes(byte[] buffer) {
            Contract.Requires(buffer != null);
            Contract.Ensures(Contract.Result<byte[]>() != null && Contract.Result<byte[]>().Length == buffer.Length);
            return ReverseBytes(buffer, 0, buffer.Length, false);
        }

        /// <summary>
        ///     Reverse a section of bytes within a buffer
        /// </summary>
        private static byte[] ReverseBytes(byte[] buffer, int offset, int count) {
            return ReverseBytes(buffer, offset, count, false);
        }
        
        private static byte[] ReverseBytes(byte[] buffer, int offset, int count, bool padWithZeroByte) {
            Contract.Requires(buffer != null);
            Contract.Requires(offset >= 0 && offset < buffer.Length);
            Contract.Requires(count >= 0 && buffer.Length - count >= offset);
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == (padWithZeroByte ? count + 1 : count));
            Contract.Ensures(padWithZeroByte ? Contract.Result<byte[]>()[count] == 0 : true);

            byte[] reversed;
            if(padWithZeroByte)
            {
                reversed = new byte[count+1]; // the last (most-significant) byte will be left as 0x00
            }
            else
            {
                reversed = new byte[count];
            }

            int lastByte = offset + count - 1;
            for (int i = 0; i < count; i++) {
                reversed[i] = buffer[lastByte - i];
            }

            return reversed;
        }

        /// <summary>
        ///     Set a DWORD property on an NCrypt object
        /// </summary>
        [System.Security.SecurityCritical]
        internal static void SetProperty(SafeNCryptHandle ncryptObject,
                                         string propertyName,
                                         int value,
                                         CngPropertyOptions propertyOptions) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            SetProperty(ncryptObject, propertyName, BitConverter.GetBytes(value), propertyOptions);
        }

        /// <summary>
        ///     Set a string property
        /// </summary>
        [System.Security.SecurityCritical]
        internal static void SetProperty(SafeNCryptHandle ncryptObject,
                                         string propertyName,
                                         string value,
                                         CngPropertyOptions propertyOptions) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            ErrorCode error = UnsafeNativeMethods.NCryptSetProperty(ncryptObject,
                                                                    propertyName,
                                                                    value,
                                                                    (value.Length + 1) * sizeof(char),
                                                                    propertyOptions);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }
        }

        /// <summary>
        ///     Set a structure property
        /// </summary>
        [System.Security.SecurityCritical]
        internal static void SetProperty<T>(SafeNCryptHandle ncryptObject,
                                            string propertyName,
                                            T value,
                                            CngPropertyOptions propertyOptions) where T : struct {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
           
            unsafe {
                fixed (byte *pBuffer = buffer) {

                    bool marshaledStructure = false;
                    try {
                        // If we successfully marshal into the buffer, make sure to destroy the buffer when we're done
                        Marshal.StructureToPtr(value, new IntPtr(pBuffer), false);
                        marshaledStructure = true;

                        SetProperty(ncryptObject, propertyName, buffer, propertyOptions);
                    }
                    finally {
                        if (marshaledStructure) {
                            Marshal.DestroyStructure(new IntPtr(pBuffer), typeof(T));
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Set a property on an NCrypt object
        /// </summary>
        [System.Security.SecurityCritical]
        internal static void SetProperty(SafeNCryptHandle ncryptObject,
                                         string propertyName,
                                         byte[] value,
                                         CngPropertyOptions propertyOptions) {
            Contract.Requires(ncryptObject != null);
            Contract.Requires(propertyName != null);

            ErrorCode error = UnsafeNativeMethods.NCryptSetProperty(ncryptObject,
                                                                    propertyName,
                                                                    value,
                                                                    value != null ? value.Length : 0,
                                                                    propertyOptions);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }
        }

        /// <summary>
        ///     Sign a hash using no padding
        /// </summary>
        [System.Security.SecurityCritical]
        internal static byte[] SignHash(SafeNCryptKeyHandle key, byte[] hash) {
            Contract.Requires(key != null);
            Contract.Requires(hash != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            // Figure out how big the signature is
            int signatureSize = 0;
            ErrorCode error = UnsafeNativeMethods.NCryptSignHash(key,
                                                                 IntPtr.Zero,
                                                                 hash,
                                                                 hash.Length,
                                                                 null,
                                                                 0,
                                                                 out signatureSize,
                                                                 0);

            if (error != ErrorCode.Success && error != ErrorCode.BufferTooSmall) {
                throw new CryptographicException((int)error);
            }

            // Sign the data
            Debug.Assert(signatureSize > 0, "signatureSize > 0");
            byte[] signature = new byte[signatureSize];

            error = UnsafeNativeMethods.NCryptSignHash(key,
                                                       IntPtr.Zero,
                                                       hash,
                                                       hash.Length,
                                                       signature,
                                                       signature.Length,
                                                       out signatureSize,
                                                       0);

            if (error != ErrorCode.Success) {
                throw new CryptographicException((int)error);
            }

            return signature;
        }

        /// <summary>
        ///     Verify a signature created with no padding
        /// </summary>
        [SecurityCritical]
        internal static bool VerifySignature(SafeNCryptKeyHandle key, byte[] hash, byte[] signature) {
            Contract.Requires(key != null);
            Contract.Requires(hash != null);
            Contract.Requires(signature != null);

            ErrorCode error = UnsafeNativeMethods.NCryptVerifySignature(key,
                                                                        IntPtr.Zero,
                                                                        hash,
                                                                        hash.Length,
                                                                        signature,
                                                                        signature.Length,
                                                                        0);

            if (error != ErrorCode.Success && error != ErrorCode.BadSignature) {
                throw new CryptographicException((int)error);
            }

            return error == ErrorCode.Success;
        }
    }
}
