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
        private readonly SafeSharedCmsRecipientInfoHandle _recipientHandle;

        internal KeyTransRecipientInfoPalOpenSsl(SafeSharedCmsRecipientInfoHandle recipient)
        {
            _recipientHandle = recipient;
            // TODO(3334): Design decision, how to deal with opaqued fields for CMS_RecipientInfo
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
