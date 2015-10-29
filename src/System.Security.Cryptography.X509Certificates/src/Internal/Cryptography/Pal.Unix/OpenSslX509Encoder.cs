// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal class OpenSslX509Encoder : IX509Pal
    {
        public AsymmetricAlgorithm DecodePublicKey(Oid oid, byte[] encodedKeyValue, byte[] encodedParameters, ICertificatePal certificatePal)
        {
            switch (oid.Value)
            {
                case Oids.RsaRsa:
                    return BuildRsaPublicKey(encodedKeyValue);
                case Oids.Ecc:
                    return ((OpenSslX509CertificateReader)certificatePal).GetECDsaPublicKey();
            }

            // NotSupportedException is what desktop and CoreFx-Windows throw in this situation.
            throw new NotSupportedException(SR.NotSupported_KeyAlgorithm);
        }

        public string X500DistinguishedNameDecode(byte[] encodedDistinguishedName, X500DistinguishedNameFlags flags)
        {
            return X500NameEncoder.X500DistinguishedNameDecode(encodedDistinguishedName, true, flags);
        }

        public byte[] X500DistinguishedNameEncode(string distinguishedName, X500DistinguishedNameFlags flag)
        {
            throw new NotImplementedException();
        }

        public string X500DistinguishedNameFormat(byte[] encodedDistinguishedName, bool multiLine)
        {
            throw new NotImplementedException();
        }

        public X509ContentType GetCertContentType(byte[] rawData)
        {
            {
                ICertificatePal certPal;

                if (CertificatePal.TryReadX509Der(rawData, out certPal) ||
                    CertificatePal.TryReadX509Pem(rawData, out certPal))
                {
                    certPal.Dispose();

                    return X509ContentType.Cert;
                }
            }

            if (PkcsFormatReader.IsPkcs7(rawData))
            {
                return X509ContentType.Pkcs7;
            }

            {
                OpenSslPkcs12Reader pfx;

                if (OpenSslPkcs12Reader.TryRead(rawData, out pfx))
                {
                    pfx.Dispose();
                    return X509ContentType.Pkcs12;
                }
            }

            // Unsupported format.
            // Windows throws new CryptographicException(CRYPT_E_NO_MATCH)
            throw new CryptographicException();
        }

        public X509ContentType GetCertContentType(string fileName)
        {
            // If we can't open the file, fail right away.
            using (SafeBioHandle fileBio = Interop.Crypto.BioNewFile(fileName, "rb"))
            {
                Interop.Crypto.CheckValidOpenSslHandle(fileBio);

                int bioPosition = Interop.Crypto.BioTell(fileBio);
                Debug.Assert(bioPosition >= 0);

                // X509ContentType.Cert
                {
                    ICertificatePal certPal;

                    if (CertificatePal.TryReadX509Der(fileBio, out certPal))
                    {
                        certPal.Dispose();

                        return X509ContentType.Cert;
                    }

                    CertificatePal.RewindBio(fileBio, bioPosition);

                    if (CertificatePal.TryReadX509Pem(fileBio, out certPal))
                    {
                        certPal.Dispose();

                        return X509ContentType.Cert;
                    }

                    CertificatePal.RewindBio(fileBio, bioPosition);
                }

                // X509ContentType.Pkcs7
                {
                    if (PkcsFormatReader.IsPkcs7Der(fileBio))
                    {
                        return X509ContentType.Pkcs7;
                    }

                    CertificatePal.RewindBio(fileBio, bioPosition);

                    if (PkcsFormatReader.IsPkcs7Pem(fileBio))
                    {
                        return X509ContentType.Pkcs7;
                    }

                    CertificatePal.RewindBio(fileBio, bioPosition);
                }

                // X509ContentType.Pkcs12 (aka PFX)
                {
                    OpenSslPkcs12Reader pkcs12Reader;

                    if (OpenSslPkcs12Reader.TryRead(fileBio, out pkcs12Reader))
                    {
                        pkcs12Reader.Dispose();

                        return X509ContentType.Pkcs12;
                    }

                    CertificatePal.RewindBio(fileBio, bioPosition);
                }
            }

            // Unsupported format.
            // Windows throws new CryptographicException(CRYPT_E_NO_MATCH)
            throw new CryptographicException();
        }

        public byte[] EncodeX509KeyUsageExtension(X509KeyUsageFlags keyUsages)
        {
            // The numeric values of X509KeyUsageFlags mean that if we interpret it as a little-endian
            // ushort it will line up with the flags in the spec.
            ushort ushortValue = unchecked((ushort)(int)keyUsages);
            byte[] data = BitConverter.GetBytes(ushortValue);

            // RFC 3280 section 4.2.1.3 (https://tools.ietf.org/html/rfc3280#section-4.2.1.3) defines
            // digitalSignature (0) through decipherOnly (8), making 9 named bits.
            const int namedBitsCount = 9;

            // The expected output of this method isn't the SEQUENCE value, but just the payload bytes.
            byte[][] segments = DerEncoder.SegmentedEncodeNamedBitList(data, namedBitsCount);
            Debug.Assert(segments.Length == 3);
            return ConcatenateArrays(segments);
        }

        public void DecodeX509KeyUsageExtension(byte[] encoded, out X509KeyUsageFlags keyUsages)
        {
            using (SafeAsn1BitStringHandle bitString = Interop.Crypto.DecodeAsn1BitString(encoded, encoded.Length))
            {
                Interop.Crypto.CheckValidOpenSslHandle(bitString);

                byte[] decoded = Interop.Crypto.GetAsn1StringBytes(bitString.DangerousGetHandle());

                // Only 9 bits are defined.
                if (decoded.Length > 2)
                {
                    throw new CryptographicException();
                }

                // DER encodings of BIT_STRING values number the bits as
                // 01234567 89 (big endian), plus a number saying how many bits of the last byte were padding.
                //
                // So digitalSignature (0) doesn't mean 2^0 (0x01), it means the most significant bit
                // is set in this byte stream.
                //
                // BIT_STRING values are compact.  So a value of cRLSign (6) | keyEncipherment (2), which
                // is 0b0010001 => 0b0010 0010 (1 bit padding) => 0x22 encoded is therefore
                // 0x02 (length remaining) 0x01 (1 bit padding) 0x22.
                //
                // OpenSSL's d2i_ASN1_BIT_STRING is going to take that, and return 0x22.  0x22 lines up
                // exactly with X509KeyUsageFlags.CrlSign (0x20) | X509KeyUsageFlags.KeyEncipherment (0x02)
                //
                // Once the decipherOnly (8) bit is added to the mix, the values become:
                // 0b001000101 => 0b0010 0010 1000 0000 (7 bits padding)
                // { 0x03 0x07 0x22 0x80 }
                // And OpenSSL returns new byte[] { 0x22 0x80 }
                //
                // The value of X509KeyUsageFlags.DecipherOnly is 0x8000.  0x8000 in a little endian
                // representation is { 0x00 0x80 }.  This means that the DER storage mechanism has effectively
                // ended up being little-endian for BIT_STRING values.  Untwist the bytes, and now the bits all
                // line up with the existing X509KeyUsageFlags.

                int value = 0;

                if (decoded.Length > 0)
                {
                    value = decoded[0];
                }

                if (decoded.Length > 1)
                {
                    value |= decoded[1] << 8;
                }

                keyUsages = (X509KeyUsageFlags)value;
            }
        }

        public byte[] EncodeX509BasicConstraints2Extension(
            bool certificateAuthority,
            bool hasPathLengthConstraint,
            int pathLengthConstraint)
        {
            throw new NotImplementedException();
        }

        public void DecodeX509BasicConstraintsExtension(
            byte[] encoded,
            out bool certificateAuthority,
            out bool hasPathLengthConstraint,
            out int pathLengthConstraint)
        {
            throw new NotImplementedException();
        }

        public void DecodeX509BasicConstraints2Extension(
            byte[] encoded,
            out bool certificateAuthority,
            out bool hasPathLengthConstraint,
            out int pathLengthConstraint)
        {
            if (!Interop.Crypto.DecodeX509BasicConstraints2Extension(
                encoded,
                encoded.Length,
                out certificateAuthority,
                out hasPathLengthConstraint,
                out pathLengthConstraint))
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }

        public byte[] EncodeX509EnhancedKeyUsageExtension(OidCollection usages)
        {
            throw new NotImplementedException();
        }

        public void DecodeX509EnhancedKeyUsageExtension(byte[] encoded, out OidCollection usages)
        {
            OidCollection oids = new OidCollection();

            using (SafeEkuExtensionHandle eku = Interop.Crypto.DecodeExtendedKeyUsage(encoded, encoded.Length))
            {
                Interop.Crypto.CheckValidOpenSslHandle(eku);

                int count = Interop.Crypto.GetX509EkuFieldCount(eku);

                for (int i = 0; i < count; i++)
                {
                    IntPtr oidPtr = Interop.Crypto.GetX509EkuField(eku, i);

                    if (oidPtr == IntPtr.Zero)
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    string oidValue = Interop.Crypto.GetOidValue(oidPtr);

                    oids.Add(new Oid(oidValue));
                }
            }

            usages = oids;
        }

        public byte[] EncodeX509SubjectKeyIdentifierExtension(byte[] subjectKeyIdentifier)
        {
            throw new NotImplementedException();
        }

        public void DecodeX509SubjectKeyIdentifierExtension(byte[] encoded, out byte[] subjectKeyIdentifier)
        {
            subjectKeyIdentifier = DecodeX509SubjectKeyIdentifierExtension(encoded);
        }

        internal static byte[] DecodeX509SubjectKeyIdentifierExtension(byte[] encoded)
        {
            DerSequenceReader reader = DerSequenceReader.CreateForPayload(encoded);
            return reader.ReadOctetString();
        }

        public byte[] ComputeCapiSha1OfPublicKey(PublicKey key)
        {
            throw new NotImplementedException();
        }

        private static RSA BuildRsaPublicKey(byte[] encodedData)
        {
            using (SafeRsaHandle rsaHandle = Interop.Crypto.DecodeRsaPublicKey(encodedData, encodedData.Length))
            {
                Interop.Crypto.CheckValidOpenSslHandle(rsaHandle);

                RSAParameters rsaParameters = Interop.Crypto.ExportRsaParameters(rsaHandle, false);
                RSA rsa = new RSAOpenSsl();
                rsa.ImportParameters(rsaParameters);
                return rsa;
            }
        }

        private static byte[] ConcatenateArrays(byte[][] segments)
        {
            int length = 0;

            foreach (byte[] segment in segments)
            {
                length += segment.Length;
            }

            byte[] concatenated = new byte[length];

            int offset = 0;

            foreach (byte[] segment in segments)
            {
                Buffer.BlockCopy(segment, 0, concatenated, offset, segment.Length);
                offset += segment.Length;
            }

            return concatenated;
        }
    }
}
