﻿using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
    [StructLayout(LayoutKind.Sequential)]
    internal partial struct OriginatorIdentifierOrKeyAsn
    {
        internal System.Security.Cryptography.Pkcs.Asn1.IssuerAndSerialNumberAsn? IssuerAndSerialNumber;
        internal ReadOnlyMemory<byte>? SubjectKeyIdentifier;
        internal System.Security.Cryptography.Pkcs.Asn1.OriginatorPublicKeyAsn? OriginatorKey;

#if DEBUG
        static OriginatorIdentifierOrKeyAsn()
        {
            var usedTags = new System.Collections.Generic.Dictionary<Asn1Tag, string>();
            Action<Asn1Tag, string> ensureUniqueTag = (tag, fieldName) =>
            {
                if (usedTags.TryGetValue(tag, out string existing))
                {
                    throw new InvalidOperationException($"Tag '{tag}' is in use by both '{existing}' and '{fieldName}'");
                }

                usedTags.Add(tag, fieldName);
            };
            
            ensureUniqueTag(Asn1Tag.Sequence, "IssuerAndSerialNumber");
            ensureUniqueTag(new Asn1Tag(TagClass.ContextSpecific, 0), "SubjectKeyIdentifier");
            ensureUniqueTag(new Asn1Tag(TagClass.ContextSpecific, 1), "OriginatorKey");
        }
#endif

        internal void Encode(AsnWriter writer)
        {
            bool wroteValue = false; 
            
            if (IssuerAndSerialNumber.HasValue)
            {
                if (wroteValue)
                    throw new CryptographicException();
                
                IssuerAndSerialNumber.Value.Encode(writer);
                wroteValue = true;
            }

            if (SubjectKeyIdentifier.HasValue)
            {
                if (wroteValue)
                    throw new CryptographicException();
                
                writer.WriteOctetString(new Asn1Tag(TagClass.ContextSpecific, 0), SubjectKeyIdentifier.Value.Span);
                wroteValue = true;
            }

            if (OriginatorKey.HasValue)
            {
                if (wroteValue)
                    throw new CryptographicException();
                
                OriginatorKey.Value.Encode(writer, new Asn1Tag(TagClass.ContextSpecific, 1));
                wroteValue = true;
            }

            if (!wroteValue)
            {
                throw new CryptographicException();
            }
        }

        internal static OriginatorIdentifierOrKeyAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
        {
            AsnReader reader = new AsnReader(encoded, ruleSet);
            
            Decode(reader, out OriginatorIdentifierOrKeyAsn decoded);
            reader.ThrowIfNotEmpty();
            return decoded;
        }

        internal static void Decode(AsnReader reader, out OriginatorIdentifierOrKeyAsn decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            decoded = default;
            Asn1Tag tag = reader.PeekTag();
            
            if (tag.HasSameClassAndValue(Asn1Tag.Sequence))
            {
                System.Security.Cryptography.Pkcs.Asn1.IssuerAndSerialNumberAsn tmpIssuerAndSerialNumber;
                System.Security.Cryptography.Pkcs.Asn1.IssuerAndSerialNumberAsn.Decode(reader, out tmpIssuerAndSerialNumber);
                decoded.IssuerAndSerialNumber = tmpIssuerAndSerialNumber;

            }
            else if (tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
            {

                if (reader.TryGetPrimitiveOctetStringBytes(new Asn1Tag(TagClass.ContextSpecific, 0), out ReadOnlyMemory<byte> tmpSubjectKeyIdentifier))
                {
                    decoded.SubjectKeyIdentifier = tmpSubjectKeyIdentifier;
                }
                else
                {
                    decoded.SubjectKeyIdentifier = reader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, 0));
                }

            }
            else if (tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 1)))
            {
                System.Security.Cryptography.Pkcs.Asn1.OriginatorPublicKeyAsn tmpOriginatorKey;
                System.Security.Cryptography.Pkcs.Asn1.OriginatorPublicKeyAsn.Decode(reader, new Asn1Tag(TagClass.ContextSpecific, 1), out tmpOriginatorKey);
                decoded.OriginatorKey = tmpOriginatorKey;

            }
            else
            {
                throw new CryptographicException();
            }
        }
    }
}
