﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography.Asn1;
using System.Text;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates
{
    public sealed class SubjectAlternativeNameBuilder
    {
        private static readonly IdnMapping s_idnMapping = new IdnMapping();

        // Because GeneralNames is a SEQUENCE, just make a rolling list, it doesn't need to be re-sorted.
        private readonly List<byte[]> _encodedNames = new List<byte[]>();

        public void AddEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentOutOfRangeException(nameof(emailAddress), SR.Arg_EmptyOrNullString);

            AddGeneralName(new GeneralNameAsn { Rfc822Name = emailAddress });
        }

        public void AddDnsName(string dnsName)
        {
            if (string.IsNullOrEmpty(dnsName))
                throw new ArgumentOutOfRangeException(nameof(dnsName), SR.Arg_EmptyOrNullString);

            AddGeneralName(new GeneralNameAsn { DnsName = s_idnMapping.GetAscii(dnsName) });
        }

        public void AddUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            AddGeneralName(new GeneralNameAsn { Uri = uri.AbsoluteUri.ToString() });
        }

        public void AddIpAddress(IPAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException(nameof(ipAddress));

            AddGeneralName(new GeneralNameAsn { IPAddress = ipAddress.GetAddressBytes() });
        }

        public void AddUserPrincipalName(string upn)
        {
            if (string.IsNullOrEmpty(upn))
                throw new ArgumentOutOfRangeException(nameof(upn), SR.Arg_EmptyOrNullString);

            byte[] otherNameValue;
            using (AsnWriter writer = new AsnWriter(AsnEncodingRules.DER))
            {
                writer.WriteCharacterString(UniversalTagNumber.UTF8String, upn);
                otherNameValue = writer.Encode();
            }

            OtherNameAsn otherName = new OtherNameAsn
            {
                TypeId = Oids.UserPrincipalName,
                Value = otherNameValue,
            };

            AddGeneralName(new GeneralNameAsn { OtherName = otherName });
        }

        public X509Extension Build(bool critical=false)
        {
            using (AsnWriter writer = new AsnWriter(AsnEncodingRules.DER))
            {
                writer.PushSequence();
                foreach (byte[] encodedName in _encodedNames)
                {
                    writer.WriteEncodedValue(encodedName);
                }
                writer.PopSequence();

                return new X509Extension(
                    Oids.SubjectAltName,
                    writer.Encode(),
                    critical);
            }
        }

        private void AddGeneralName(GeneralNameAsn generalName)
        {
            try
            {
                // Verify that the general name can be serialized and store it.
                using (AsnWriter writer = new AsnWriter(AsnEncodingRules.DER))
                {
                    generalName.Encode(writer);
                    _encodedNames.Add(writer.Encode());
                }
            }
            catch (EncoderFallbackException)
            {
                throw new CryptographicException(SR.Cryptography_Invalid_IA5String);
            }
        }
    }
}
