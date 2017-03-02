﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Security.Cryptography.Xml
{
    internal static class CryptoHelpers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5350", Justification = "SHA1 needed for compat.")]
        public static object CreateFromName(string name)
        {
            switch (name)
            {
                case "http://www.w3.org/TR/2001/REC-xml-c14n-20010315":
                    return new XmlDsigC14NTransform();
                case "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments":
                    return new XmlDsigC14NWithCommentsTransform();
                case "http://www.w3.org/2001/10/xml-exc-c14n#":
                    return new XmlDsigExcC14NTransform();
                case "http://www.w3.org/2001/10/xml-exc-c14n#WithComments":
                    return new XmlDsigExcC14NWithCommentsTransform();
                case "http://www.w3.org/2000/09/xmldsig#base64":
                    return new XmlDsigBase64Transform();
                case "http://www.w3.org/TR/1999/REC-xpath-19991116":
                    return new XmlDsigXPathTransform();
                case "http://www.w3.org/TR/1999/REC-xslt-19991116":
                    return new XmlDsigXsltTransform();
                case "http://www.w3.org/2000/09/xmldsig#enveloped-signature":
                    return new XmlDsigEnvelopedSignatureTransform();
                case "http://www.w3.org/2002/07/decrypt#XML":
                    return new XmlDecryptionTransform();
                case "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform":
                    return new XmlLicenseTransform();
                case "http://www.w3.org/2000/09/xmldsig# X509Data":
                    return new KeyInfoX509Data();
                case "http://www.w3.org/2000/09/xmldsig# KeyName":
                    return new KeyInfoName();
                case "http://www.w3.org/2000/09/xmldsig# KeyValue/DSAKeyValue":
                    return new DSAKeyValue();
                case "http://www.w3.org/2000/09/xmldsig# KeyValue/RSAKeyValue":
                    return new RSAKeyValue();
                case "http://www.w3.org/2000/09/xmldsig# RetrievalMethod":
                    return new KeyInfoRetrievalMethod();
                case "http://www.w3.org/2001/04/xmlenc# EncryptedKey":
                    return new KeyInfoEncryptedKey();
                case "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160":
                    throw new NotImplementedException(name);
                    //return new System.Security.Cryptography.HMACRIPEMD160();
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                    throw new NotImplementedException(name);
                    //return new DSASignatureDescription();
                case "System.Security.Cryptography.DSASignatureDescription":
                    throw new NotImplementedException(name);
                    //return new DSASignatureDescription();
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                    return new RSAPKCS1SHA1SignatureDescription();
                case "System.Security.Cryptography.RSASignatureDescription":
                    throw new NotImplementedException(name);
                    //return new RSAPKCS1SHA1SignatureDescription();
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                    throw new NotImplementedException(name);
                    //return new RSAPKCS1SHA256SignatureDescription();
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384":
                    throw new NotImplementedException(name);
                    //return new RSAPKCS1SHA384SignatureDescription();
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512":
                    throw new NotImplementedException(name);
                    //return new RSAPKCS1SHA512SignatureDescription();

                // workarounds for issue https://github.com/dotnet/corefx/issues/16563
                // remove attribute from this method when removing them
                case "http://www.w3.org/2000/09/xmldsig#sha1":
                    return SHA1.Create();
            }

            return CryptoConfig.CreateFromName(name);
        }
    }
}
