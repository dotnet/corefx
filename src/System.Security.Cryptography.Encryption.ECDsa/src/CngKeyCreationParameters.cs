// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Diagnostics.Contracts;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Settings to be applied to a CNG key before it is finalized.
    /// </summary>
    internal sealed class CngKeyCreationParameters
    {
        private CngExportPolicies? _exportPolicy;
        private CngKeyCreationOptions _keyCreationOptions;
        private CngKeyUsages? _keyUsage;
        private CngPropertyCollection _parameters = new CngPropertyCollection();
        private IntPtr _parentWindowHandle;
        private CngProvider _provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;
        private CngUIPolicy _uiPolicy;

        /// <summary>
        ///     How many times can this key be exported from the KSP
        /// </summary>
        public CngExportPolicies? ExportPolicy
        {
            get { return _exportPolicy; }
            set { _exportPolicy = value; }
        }

        /// <summary>
        ///     Flags controlling how to create the key
        /// </summary>
        public CngKeyCreationOptions KeyCreationOptions
        {
            get { return _keyCreationOptions; }
            set { _keyCreationOptions = value; }
        }

        /// <summary>
        ///     Which cryptographic operations are valid for use with this key
        /// </summary>
        public CngKeyUsages? KeyUsage
        {
            get { return _keyUsage; }
            set { _keyUsage = value; }
        }

        /// <summary>
        ///     Window handle to use as the parent for the dialog shown when the key is created
        /// </summary>
        public IntPtr ParentWindowHandle
        {
            get { return _parentWindowHandle; }

            [SecuritySafeCritical]
            set { _parentWindowHandle = value; }
        }

        /// <summary>
        ///     Extra parameter values to set before the key is finalized
        /// </summary>
        public CngPropertyCollection Parameters
        {
            [SecuritySafeCritical]
            get
            {
                Contract.Ensures(Contract.Result<CngPropertyCollection>() != null);
                return _parameters;
            }
        }

        /// <summary>
        ///     Internal access to the parameters method without a demand
        /// </summary>
        internal CngPropertyCollection ParametersNoDemand
        {
            get
            {
                Contract.Ensures(Contract.Result<CngPropertyCollection>() != null);
                return _parameters;
            }
        }

        /// <summary>
        ///     KSP to create the key in
        /// </summary>
        public CngProvider Provider
        {
            get
            {
                Contract.Ensures(Contract.Result<CngProvider>() != null);
                return _provider;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _provider = value;
            }
        }

        /// <summary>
        ///     Settings for UI shown on access to the key
        /// </summary>
        public CngUIPolicy UIPolicy
        {
            get { return _uiPolicy; }

            [SecuritySafeCritical]
            set { _uiPolicy = value; }
        }
    }
}
