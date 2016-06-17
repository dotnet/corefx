// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed class KeyAgreeRecipientInfoPalOpenSsl : KeyAgreeRecipientInfoPal
    {
        private readonly DateTime _date;
        private readonly byte[] _encryptedKey;
        private readonly AlgorithmIdentifier _keyEncryptionAlgorithm;
        private readonly SubjectIdentifierOrKey _originatorIdentifierOrKey;
        private readonly CryptographicAttributeObject _otherKeyAttribute;
        private readonly SubjectIdentifier _recipientIdentifier;
        private readonly int _version;

        public KeyAgreeRecipientInfoPalOpenSsl(
            int version,
            SubjectIdentifier recipientIdentifier,
            SubjectIdentifierOrKey originatorIdentifierOrKey,
            CryptographicAttributeObject otherKeyAttribute,
            AlgorithmIdentifier keyEncryptionAlgorithm,
            byte[] encryptedKey,
            DateTime date)
        {
            _version = version;
            _encryptedKey = encryptedKey;
            _keyEncryptionAlgorithm = keyEncryptionAlgorithm;
            _originatorIdentifierOrKey = originatorIdentifierOrKey;
            _otherKeyAttribute = otherKeyAttribute;
            _recipientIdentifier = recipientIdentifier;
            _date = date;
        }

        public override DateTime Date
        {
            get
            {
                if (_recipientIdentifier.Type != SubjectIdentifierType.SubjectKeyIdentifier)
                    throw new InvalidOperationException(SR.Cryptography_Cms_Key_Agree_Date_Not_Available);

                return _date;
            }
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

        public override SubjectIdentifierOrKey OriginatorIdentifierOrKey
        {
            get
            {
                return _originatorIdentifierOrKey;
            }
        }

        public override CryptographicAttributeObject OtherKeyAttribute
        {
            get
            {
                if (_recipientIdentifier.Type != SubjectIdentifierType.SubjectKeyIdentifier)
                    throw new InvalidOperationException(SR.Cryptography_Cms_Key_Agree_Date_Not_Available);

                return _otherKeyAttribute;
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
