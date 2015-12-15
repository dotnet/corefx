// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security;
using System.Diagnostics.Contracts;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Flags providing information about a raw key handle being opened
    /// </summary>
    [Flags]
    internal enum CngKeyHandleOpenOptions
    {
        None = 0x00000000,

        /// <summary>
        ///     The key handle being opened represents an ephemeral key
        /// </summary>
        EphemeralKey = 0x00000001
    }

    /// <summary>
    ///     Managed representation of an NCrypt key
    /// </summary>
    internal sealed class CngKey : IDisposable
    {
        private SafeNCryptKeyHandle _keyHandle;
        private SafeNCryptProviderHandle _kspHandle;

        [System.Security.SecurityCritical]
        private CngKey(SafeNCryptProviderHandle kspHandle, SafeNCryptKeyHandle keyHandle)
        {
            Contract.Requires(keyHandle != null && !keyHandle.IsInvalid && !keyHandle.IsClosed);
            Contract.Requires(kspHandle != null && !kspHandle.IsInvalid && !kspHandle.IsClosed);
            Contract.Ensures(_keyHandle != null && !_keyHandle.IsInvalid && !_keyHandle.IsClosed);
            Contract.Ensures(kspHandle != null && !kspHandle.IsInvalid && !kspHandle.IsClosed);

            _keyHandle = keyHandle;
            _kspHandle = kspHandle;
        }

        //
        // Key properties
        //

        /// <summary>
        ///     Algorithm group this key can be used with
        /// </summary>
        internal CngAlgorithmGroup AlgorithmGroup
        {
            [SecuritySafeCritical]
            [Pure]
            get
            {
                Contract.Assert(_keyHandle != null);
                string group = NCryptNative.GetPropertyAsString(_keyHandle,
                                                                NCryptNative.KeyPropertyName.AlgorithmGroup,
                                                                CngPropertyOptions.None);

                if (group == null)
                {
                    return null;
                }
                else
                {
                    return new CngAlgorithmGroup(group);
                }
            }
        }

        /// <summary>
        ///     Name of the algorithm this key can be used with
        /// </summary>
        internal CngAlgorithm Algorithm
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                string algorithm = NCryptNative.GetPropertyAsString(_keyHandle,
                                                                    NCryptNative.KeyPropertyName.Algorithm,
                                                                    CngPropertyOptions.None);
                return new CngAlgorithm(algorithm);
            }
        }

        /// <summary>
        ///     Export restrictions on the key
        /// </summary>
        internal CngExportPolicies ExportPolicy
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                int policy = NCryptNative.GetPropertyAsDWord(_keyHandle,
                                                             NCryptNative.KeyPropertyName.ExportPolicy,
                                                             CngPropertyOptions.None);

                return (CngExportPolicies)policy;
            }
        }

        /// <summary>
        ///     Native handle for the key
        /// </summary>
        internal SafeNCryptKeyHandle Handle
        {
            [SecurityCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                return _keyHandle.Duplicate();
            }
        }

        /// <summary>
        ///     Is this key ephemeral or persisted
        /// </summary>
        /// <remarks>
        ///     Any ephemeral key created by the CLR will have a property 'CLR IsEphemeral' which consists
        ///     of a single byte containing the value 1. We cannot detect ephemeral keys created by other
        ///     APIs and imported via handle.
        /// </remarks>
        internal bool IsEphemeral
        {
            [SecuritySafeCritical]
            [Pure]
            get
            {
                Contract.Assert(_keyHandle != null);

                bool foundProperty;
                byte[] ephemeralProperty = null;
                try
                {
                    ephemeralProperty = NCryptNative.GetProperty(_keyHandle,
                                                                        NCryptNative.KeyPropertyName.ClrIsEphemeral,
                                                                        CngPropertyOptions.CustomProperty,
                                                                        out foundProperty);
                }
                catch (CryptographicException)
                {
                    // Third party Key providers, and Windows PCP KSP won't recognize this property;
                    // and Win32 layer does not enforce error return contract.
                    // Therefore, they can return whatever error code they think appropriate.
                    return false;
                }

                return foundProperty &&
                       ephemeralProperty != null &&
                       ephemeralProperty.Length == 1 &&
                       ephemeralProperty[0] == 1;
            }

            [System.Security.SecurityCritical]
            private set
            {
                Contract.Assert(_keyHandle != null);

                NCryptNative.SetProperty(_keyHandle,
                                         NCryptNative.KeyPropertyName.ClrIsEphemeral,
                                         new byte[] { value ? (byte)1 : (byte)0 },
                                         CngPropertyOptions.CustomProperty);
            }
        }

        /// <summary>
        ///     Is this a machine key or a user key
        /// </summary>
        internal bool IsMachineKey
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                int type = NCryptNative.GetPropertyAsDWord(_keyHandle,
                                                           NCryptNative.KeyPropertyName.KeyType,
                                                           CngPropertyOptions.None);

                return ((CngKeyTypes)type & CngKeyTypes.MachineKey) == CngKeyTypes.MachineKey;
            }
        }

        /// <summary>
        ///     The name of the key, null if it is ephemeral. We can only detect ephemeral keys created by
        ///     the CLR. Other ephemeral keys, such as those imported by handle, will get a CryptographicException
        ///     if they read this property.
        /// </summary>
        internal string KeyName
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);

                if (IsEphemeral)
                {
                    return null;
                }
                else
                {
                    return NCryptNative.GetPropertyAsString(_keyHandle,
                                                            NCryptNative.KeyPropertyName.Name,
                                                            CngPropertyOptions.None);
                }
            }
        }

        /// <summary>
        ///     Size, in bits, of the key
        /// </summary>
        internal int KeySize
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                return NCryptNative.GetPropertyAsDWord(_keyHandle,
                                                       NCryptNative.KeyPropertyName.Length,
                                                       CngPropertyOptions.None);
            }
        }

        /// <summary>
        ///     Usage restrictions on the key
        /// </summary>
        internal CngKeyUsages KeyUsage
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                int keyUsage = NCryptNative.GetPropertyAsDWord(_keyHandle,
                                                               NCryptNative.KeyPropertyName.KeyUsage,
                                                               CngPropertyOptions.None);
                return (CngKeyUsages)keyUsage;
            }
        }

        /// <summary>
        ///     HWND of the window to use as a parent for any UI
        /// </summary>
        internal IntPtr ParentWindowHandle
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);
                return NCryptNative.GetPropertyAsIntPtr(_keyHandle,
                                                        NCryptNative.KeyPropertyName.ParentWindowHandle,
                                                        CngPropertyOptions.None);
            }

            [SecuritySafeCritical]
            set
            {
                Contract.Assert(_keyHandle != null);
                NCryptNative.SetProperty(_keyHandle,
                                         NCryptNative.KeyPropertyName.ParentWindowHandle,
                                         value,
                                         CngPropertyOptions.None);
            }
        }

        /// <summary>
        ///     KSP which holds this key
        /// </summary>
        internal CngProvider Provider
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_kspHandle != null);
                string provider = NCryptNative.GetPropertyAsString(_kspHandle,
                                                                   NCryptNative.ProviderPropertyName.Name,
                                                                   CngPropertyOptions.None);

                if (provider == null)
                {
                    return null;
                }
                else
                {
                    return new CngProvider(provider);
                }
            }
        }

        /// <summary>
        ///     Native handle to the KSP associated with this key
        /// </summary>
        internal SafeNCryptProviderHandle ProviderHandle
        {
            [SecurityCritical]
            get
            {
                Contract.Assert(_kspHandle != null);
                return _kspHandle.Duplicate();
            }
        }

        /// <summary>
        ///     Unique name of the key, null if it is ephemeral. See the comments on the Name property for
        ///     details about names of ephemeral keys.
        /// </summary>
        internal string UniqueName
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Assert(_keyHandle != null);

                if (IsEphemeral)
                {
                    return null;
                }
                else
                {
                    return NCryptNative.GetPropertyAsString(_keyHandle,
                                                            NCryptNative.KeyPropertyName.UniqueName,
                                                            CngPropertyOptions.None);
                }
            }
        }

        /// <summary>
        ///     UI strings associated with a key
        /// </summary>
        internal CngUIPolicy UIPolicy
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Ensures(Contract.Result<CngUIPolicy>() != null);
                Contract.Assert(_keyHandle != null);

                NCryptNative.NCRYPT_UI_POLICY uiPolicy =
                    NCryptNative.GetPropertyAsStruct<NCryptNative.NCRYPT_UI_POLICY>(_keyHandle,
                                                                                    NCryptNative.KeyPropertyName.UIPolicy,
                                                                                    CngPropertyOptions.None);

                string useContext = NCryptNative.GetPropertyAsString(_keyHandle,
                                                                     NCryptNative.KeyPropertyName.UseContext,
                                                                     CngPropertyOptions.None);

                return new CngUIPolicy(uiPolicy.dwFlags,
                                       uiPolicy.pszFriendlyName,
                                       uiPolicy.pszDescription,
                                       useContext,
                                       uiPolicy.pszCreationTitle);
            }
        }

        //
        // Creation factory methods
        //

        internal static CngKey Create(CngAlgorithm algorithm)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);
            return Create(algorithm, null);
        }

        internal static CngKey Create(CngAlgorithm algorithm, string keyName)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);
            return Create(algorithm, keyName, null);
        }

        [SecuritySafeCritical]
        internal static CngKey Create(CngAlgorithm algorithm, string keyName, CngKeyCreationParameters creationParameters)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);

            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }

            if (creationParameters == null)
            {
                creationParameters = new CngKeyCreationParameters();
            }

            // Make sure that NCrypt is supported on this platform
            if (!NCryptNative.NCryptSupported)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_PlatformNotSupported);
            }

            //
            // Create the native handles representing the new key, setup the creation parameters on it, and
            // finalize it for use.
            //

            SafeNCryptProviderHandle kspHandle = NCryptNative.OpenStorageProvider(creationParameters.Provider.Provider);
            SafeNCryptKeyHandle keyHandle = NCryptNative.CreatePersistedKey(kspHandle,
                                                                            algorithm.Algorithm,
                                                                            keyName,
                                                                            creationParameters.KeyCreationOptions);

            SetKeyProperties(keyHandle, creationParameters);
            NCryptNative.FinalizeKey(keyHandle);

            CngKey key = new CngKey(kspHandle, keyHandle);

            // No name translates to an ephemeral key
            if (keyName == null)
            {
                key.IsEphemeral = true;
            }

            return key;
        }

        /// <summary>
        ///     Delete this key
        /// </summary>
        [SecuritySafeCritical]
        internal void Delete()
        {
            Contract.Assert(_keyHandle != null);

            // Make sure we have permission to delete this key
            NCryptNative.DeleteKey(_keyHandle);

            // Once the key is deleted, the handles are no longer valid so dispose of this instance
            Dispose();
        }

        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Reviewed")]
        public void Dispose()
        {
            if (_kspHandle != null)
            {
                _kspHandle.Dispose();
            }

            if (_keyHandle != null)
            {
                _keyHandle.Dispose();
            }
        }

        //
        // Check to see if a key already exists
        //

        internal static bool Exists(string keyName)
        {
            return Exists(keyName, CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        internal static bool Exists(string keyName, CngProvider provider)
        {
            return Exists(keyName, provider, CngKeyOpenOptions.None);
        }

        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Reviewed")]
        internal static bool Exists(string keyName, CngProvider provider, CngKeyOpenOptions options)
        {
            if (keyName == null)
            {
                throw new ArgumentNullException("keyName");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            // Make sure that NCrypt is supported on this platform
            if (!NCryptNative.NCryptSupported)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_PlatformNotSupported);
            }

            using (SafeNCryptProviderHandle kspHandle = NCryptNative.OpenStorageProvider(provider.Provider))
            {

                SafeNCryptKeyHandle keyHandle = null;

                try
                {
                    NCryptNative.ErrorCode error = NCryptNative.UnsafeNativeMethods.NCryptOpenKey(kspHandle,
                                                                                                  out keyHandle,
                                                                                                  keyName,
                                                                                                  0,
                                                                                                  options);

                    // CNG will return either NTE_NOT_FOUND or NTE_BAD_KEYSET for the case where the key does
                    // not exist, so we need to check for both return codes.
                    bool keyNotFound = error == NCryptNative.ErrorCode.KeyDoesNotExist ||
                                       error == NCryptNative.ErrorCode.NotFound;

                    if (error != NCryptNative.ErrorCode.Success && !keyNotFound)
                    {
                        throw new CryptographicException((int)error);
                    }

                    return error == NCryptNative.ErrorCode.Success;
                }
                finally
                {
                    if (keyHandle != null)
                    {
                        keyHandle.Dispose();
                    }
                }
            }
        }

        //
        // Import factory methods
        //

        internal static CngKey Import(byte[] keyBlob, CngKeyBlobFormat format)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);
            return Import(keyBlob, format, CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        [SecuritySafeCritical]
        internal static CngKey Import(byte[] keyBlob, CngKeyBlobFormat format, CngProvider provider)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);

            if (keyBlob == null)
            {
                throw new ArgumentNullException("keyBlob");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            // Make sure that NCrypt is supported on this platform
            if (!NCryptNative.NCryptSupported)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_PlatformNotSupported);
            }

            // If we don't know for sure that the key will be ephemeral, then we need to demand Import
            // permission.  Since we won't know the name of the key until it's too late, we demand a full Import
            // rather than one scoped to the key.
            bool safeKeyImport = format == CngKeyBlobFormat.EccPublicBlob ||
                                 format == CngKeyBlobFormat.GenericPublicBlob;

            // Import the key into the KSP
            SafeNCryptProviderHandle kspHandle = NCryptNative.OpenStorageProvider(provider.Provider);
            SafeNCryptKeyHandle keyHandle = NCryptNative.ImportKey(kspHandle, keyBlob, format.Format);

            // Prepare the key for use
            CngKey key = new CngKey(kspHandle, keyHandle);

            // We can't tell directly if an OpaqueTransport blob imported as an ephemeral key or not
            key.IsEphemeral = format != CngKeyBlobFormat.OpaqueTransportBlob;

            return key;
        }

        /// <summary>
        ///     Export the key out of the KSP
        /// </summary>
        [SecuritySafeCritical]
        internal byte[] Export(CngKeyBlobFormat format)
        {
            Contract.Assert(_keyHandle != null);

            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            return NCryptNative.ExportKey(_keyHandle, format.Format);
        }

        /// <summary>
        ///     Get the value of an arbitrary property
        /// </summary>
        [SecuritySafeCritical]
        internal CngProperty GetProperty(string name, CngPropertyOptions options)
        {
            Contract.Assert(_keyHandle != null);

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            bool foundProperty;
            byte[] value = NCryptNative.GetProperty(_keyHandle, name, options, out foundProperty);

            if (!foundProperty)
            {
                throw new CryptographicException((int)NCryptNative.ErrorCode.NotFound);
            }

            return new CngProperty(name, value, options);
        }

        /// <summary>
        ///     Determine if a property exists on the key
        /// </summary>
        [SecuritySafeCritical]
        internal bool HasProperty(string name, CngPropertyOptions options)
        {
            Contract.Assert(_keyHandle != null);

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            bool foundProperty;
            NCryptNative.GetProperty(_keyHandle, name, options, out foundProperty);

            return foundProperty;
        }

        //
        // Open factory methods
        //

        internal static CngKey Open(string keyName)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);
            return Open(keyName, CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        internal static CngKey Open(string keyName, CngProvider provider)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);
            return Open(keyName, provider, CngKeyOpenOptions.None);
        }

        [SecuritySafeCritical]
        internal static CngKey Open(string keyName, CngProvider provider, CngKeyOpenOptions openOptions)
        {
            Contract.Ensures(Contract.Result<CngKey>() != null);

            if (keyName == null)
            {
                throw new ArgumentNullException("keyName");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            // Make sure that NCrypt is supported on this platform
            if (!NCryptNative.NCryptSupported)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_PlatformNotSupported);
            }

            // Open the key
            SafeNCryptProviderHandle kspHandle = NCryptNative.OpenStorageProvider(provider.Provider);
            SafeNCryptKeyHandle keyHandle = NCryptNative.OpenKey(kspHandle, keyName, openOptions);

            return new CngKey(kspHandle, keyHandle);
        }

        /// <summary>
        ///     Wrap an existing key handle with a CngKey object
        /// </summary>
        [SecurityCritical]
        internal static CngKey Open(SafeNCryptKeyHandle keyHandle, CngKeyHandleOpenOptions keyHandleOpenOptions)
        {
            if (keyHandle == null)
            {
                throw new ArgumentNullException("keyHandle");
            }
            if (keyHandle.IsClosed || keyHandle.IsInvalid)
            {
                throw new ArgumentException(SR.Cryptography_OpenInvalidHandle, "keyHandle");
            }

            SafeNCryptKeyHandle keyHandleCopy = keyHandle.Duplicate();

            // Get a handle to the key's KSP
            SafeNCryptProviderHandle kspHandle = new SafeNCryptProviderHandle();
            IntPtr rawHandle = NCryptNative.GetPropertyAsIntPtr(keyHandle,
                                                                NCryptNative.KeyPropertyName.ProviderHandle,
                                                                CngPropertyOptions.None);
            kspHandle.SetHandleValue(rawHandle);

            // Setup a key object wrapping the handle
            CngKey key = null;
            bool keyFullySetup = false;
            try
            {
                key = new CngKey(kspHandle, keyHandleCopy);

                bool openingEphemeralKey = (keyHandleOpenOptions & CngKeyHandleOpenOptions.EphemeralKey) == CngKeyHandleOpenOptions.EphemeralKey;

                //
                // If we're wrapping a handle to an ephemeral key, we need to make sure that IsEphemeral is
                // setup to return true.  In the case that the handle is for an ephemeral key that was created
                // by the CLR, then we don't have anything to do as the IsEphemeral CLR property will already
                // be setup.  However, if the key was created outside of the CLR we will need to setup our
                // ephemeral detection property.
                // 
                // This enables consumers of CngKey objects to always be able to rely on the result of
                // calling IsEphemeral, and also allows them to safely access the Name property.
                // 
                // Finally, if we detect that this is an ephemeral key that the CLR created but we were not
                // told that it was an ephemeral key we'll throw an exception.  This prevents us from having
                // to decide who to believe -- the key property or the caller of the API.  Since other code
                // relies on the ephemeral flag being set properly to avoid tripping over bugs in CNG, we
                // need to reject the case that we suspect that the flag is incorrect.
                // 

                if (!key.IsEphemeral && openingEphemeralKey)
                {
                    key.IsEphemeral = true;
                }
                else if (key.IsEphemeral && !openingEphemeralKey)
                {
                    throw new ArgumentException(SR.Cryptography_OpenEphemeralKeyHandleWithoutEphemeralFlag, "keyHandleOpenOptions");
                }

                keyFullySetup = true;
            }
            finally
            {
                // Make sure that we don't leak the handle the CngKey duplicated
                if (!keyFullySetup && key != null)
                {
                    key.Dispose();
                }
            }

            return key;
        }

        /// <summary>
        ///     Setup the key properties specified in the key creation parameters
        /// </summary>
        /// <param name="keyHandle"></param>
        /// <param name="creationParameters"></param>
        [System.Security.SecurityCritical]
        private static void SetKeyProperties(SafeNCryptKeyHandle keyHandle,
                                             CngKeyCreationParameters creationParameters)
        {
            Contract.Requires(keyHandle != null && !keyHandle.IsInvalid && !keyHandle.IsClosed);
            Contract.Requires(creationParameters != null);

            //
            // Setup the well-known properties.
            //

            if (creationParameters.ExportPolicy.HasValue)
            {
                NCryptNative.SetProperty(keyHandle,
                                         NCryptNative.KeyPropertyName.ExportPolicy,
                                         (int)creationParameters.ExportPolicy.Value,
                                         CngPropertyOptions.Persist);
            }

            if (creationParameters.KeyUsage.HasValue)
            {
                NCryptNative.SetProperty(keyHandle,
                                         NCryptNative.KeyPropertyName.KeyUsage,
                                         (int)creationParameters.KeyUsage.Value,
                                         CngPropertyOptions.Persist);
            }

            if (creationParameters.ParentWindowHandle != IntPtr.Zero)
            {
                NCryptNative.SetProperty(keyHandle,
                                         NCryptNative.KeyPropertyName.ParentWindowHandle,
                                         creationParameters.ParentWindowHandle,
                                         CngPropertyOptions.None);
            }

            if (creationParameters.UIPolicy != null)
            {
                NCryptNative.NCRYPT_UI_POLICY uiPolicy = new NCryptNative.NCRYPT_UI_POLICY();
                uiPolicy.dwVersion = 1;
                uiPolicy.dwFlags = creationParameters.UIPolicy.ProtectionLevel;
                uiPolicy.pszCreationTitle = creationParameters.UIPolicy.CreationTitle;
                uiPolicy.pszFriendlyName = creationParameters.UIPolicy.FriendlyName;
                uiPolicy.pszDescription = creationParameters.UIPolicy.Description;

                NCryptNative.SetProperty(keyHandle,
                                         NCryptNative.KeyPropertyName.UIPolicy,
                                         uiPolicy,
                                         CngPropertyOptions.Persist);

                // The use context is a seperate property from the standard UI context
                if (creationParameters.UIPolicy.UseContext != null)
                {
                    NCryptNative.SetProperty(keyHandle,
                                             NCryptNative.KeyPropertyName.UseContext,
                                             creationParameters.UIPolicy.UseContext,
                                             CngPropertyOptions.Persist);
                }
            }

            // Iterate over the custom properties, setting those as well.
            foreach (CngProperty property in creationParameters.ParametersNoDemand)
            {
                NCryptNative.SetProperty(keyHandle, property.Name, property.Value, property.Options);
            }
        }

        /// <summary>
        ///     Set an arbitrary property on the key
        /// </summary>
        [SecuritySafeCritical]
        internal void SetProperty(CngProperty property)
        {
            Contract.Assert(_keyHandle != null);
            NCryptNative.SetProperty(_keyHandle, property.Name, property.Value, property.Options);
        }
    }
}
