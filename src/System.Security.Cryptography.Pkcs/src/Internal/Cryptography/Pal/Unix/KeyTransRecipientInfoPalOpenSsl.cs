// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.Pkcs;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed class KeyTransRecipientInfoPalOpenSsl : KeyTransRecipientInfoPal
    {
        private readonly int _version;
        private readonly AlgorithmIdentifier _keyEncryptionAlgorithm;
        private readonly SubjectIdentifier _recipientIdentifier;
        private readonly byte[] _encryptedKey;

        internal KeyTransRecipientInfoPalOpenSsl(
            int version,
            SubjectIdentifier recipientIdentifier,
            AlgorithmIdentifier keyEncryptionAlgorithm,
            byte[] encrypted)
        {
            _version = version;
            _keyEncryptionAlgorithm = keyEncryptionAlgorithm;
            _recipientIdentifier = recipientIdentifier;
            _encryptedKey = encrypted;
        }

        public override byte[] EncryptedKey
        {
            get
            {
                return _encryptedKey;
            }
        }

        public override AlgorithmIdentifier KeyEncryptionAlgorithm
        {
            get
            {
                return _keyEncryptionAlgorithm;
            }
        }

        public override SubjectIdentifier RecipientIdentifier
        {
            get
            {
                return _recipientIdentifier;
            }
        }

        public override int Version
        {
            get
            {
                return _version;
            }
        }
    }
}
