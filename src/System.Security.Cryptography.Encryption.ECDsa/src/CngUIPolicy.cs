// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Configuration parameters for the UI displayed by CNG when accessing a protected key
    /// </summary>
    internal sealed class CngUIPolicy
    {
        private string _creationTitle;
        private string _description;
        private string _friendlyName;
        private CngUIProtectionLevels _protectionLevel;
        private string _useContext;

        public CngUIPolicy(CngUIProtectionLevels protectionLevel) :
            this(protectionLevel, null)
        {
        }

        public CngUIPolicy(CngUIProtectionLevels protectionLevel, string friendlyName) :
            this(protectionLevel, friendlyName, null)
        {
        }

        public CngUIPolicy(CngUIProtectionLevels protectionLevel, string friendlyName, string description) :
            this(protectionLevel, friendlyName, description, null)
        {
        }

        public CngUIPolicy(CngUIProtectionLevels protectionLevel,
                           string friendlyName,
                           string description,
                           string useContext) :
            this(protectionLevel, friendlyName, description, useContext, null)
        {
        }

        public CngUIPolicy(CngUIProtectionLevels protectionLevel,
                           string friendlyName,
                           string description,
                           string useContext,
                           string creationTitle)
        {
            _creationTitle = creationTitle;
            _description = description;
            _friendlyName = friendlyName;
            _protectionLevel = protectionLevel;
            _useContext = useContext;
        }

        /// <summary>
        ///     Title of the dialog box displaed when a newly created key is finalized, null for the default title
        /// </summary>
        public string CreationTitle
        {
            get { return _creationTitle; }
        }

        /// <summary>
        ///     Description text displayed in the dialog box when the key is accessed, null for the default text
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        ///     Friendly name to describe the key with in the dialog box that appears when the key is accessed,
        ///     null for default name
        /// </summary>
        public string FriendlyName
        {
            get { return _friendlyName; }
        }

        /// <summary>
        ///     Level of UI protection to apply to the key
        /// </summary>
        public CngUIProtectionLevels ProtectionLevel
        {
            get { return _protectionLevel; }
        }

        /// <summary>
        ///     Description of how the key will be used
        /// </summary>
        public string UseContext
        {
            get { return _useContext; }
        }
    }
}
