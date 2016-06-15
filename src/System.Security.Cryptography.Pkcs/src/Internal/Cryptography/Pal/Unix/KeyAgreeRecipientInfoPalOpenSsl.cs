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
        public override DateTime Date
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override byte[] EncryptedKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override AlgorithmIdentifier KeyEncryptionAlgorithm
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override SubjectIdentifierOrKey OriginatorIdentifierOrKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override CryptographicAttributeObject OtherKeyAttribute
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override SubjectIdentifier RecipientIdentifier
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Version
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
