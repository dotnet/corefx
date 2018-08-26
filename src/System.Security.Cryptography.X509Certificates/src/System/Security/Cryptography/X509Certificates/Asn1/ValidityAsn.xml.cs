using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Asn1
{
    [StructLayout(LayoutKind.Sequential)]
    internal partial struct ValidityAsn
    {
        internal System.Security.Cryptography.Asn1.TimeAsn NotBefore;
        internal System.Security.Cryptography.Asn1.TimeAsn NotAfter;
      
        internal void Encode(AsnWriter writer)
        {
            Encode(writer, Asn1Tag.Sequence);
        }
    
        internal void Encode(AsnWriter writer, Asn1Tag tag)
        {
            writer.PushSequence(tag);
            
            NotBefore.Encode(writer);
            NotAfter.Encode(writer);
            writer.PopSequence(tag);
        }

        internal static void Decode(AsnReader reader, out ValidityAsn decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Decode(reader, Asn1Tag.Sequence, out decoded);
        }

        internal static void Decode(AsnReader reader, Asn1Tag expectedTag, out ValidityAsn decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            decoded = default;
            AsnReader sequenceReader = reader.ReadSequence(expectedTag);
            
            System.Security.Cryptography.Asn1.TimeAsn.Decode(sequenceReader, out decoded.NotBefore);
            System.Security.Cryptography.Asn1.TimeAsn.Decode(sequenceReader, out decoded.NotAfter);

            sequenceReader.ThrowIfNotEmpty();
        }
    }
}
