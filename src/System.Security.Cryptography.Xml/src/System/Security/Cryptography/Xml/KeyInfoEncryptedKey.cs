// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.Security.Cryptography.Xml
{
    public class KeyInfoEncryptedKey : KeyInfoClause
    {
        public KeyInfoEncryptedKey() { }

        public KeyInfoEncryptedKey(EncryptedKey encryptedKey)
        {
            EncryptedKey = encryptedKey;
        }

        public EncryptedKey EncryptedKey { get; set; }

        public override XmlElement GetXml()
        {
            if (EncryptedKey == null)
                throw new CryptographicException(SR.Cryptography_Xml_InvalidElement, "KeyInfoEncryptedKey");
            return EncryptedKey.GetXml();
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            if (EncryptedKey == null)
                throw new CryptographicException(SR.Cryptography_Xml_InvalidElement, "KeyInfoEncryptedKey");
            return EncryptedKey.GetXml(xmlDocument);
        }

        public override void LoadXml(XmlElement value)
        {
            EncryptedKey = new EncryptedKey();
            EncryptedKey.LoadXml(value);
        }
    }
}
