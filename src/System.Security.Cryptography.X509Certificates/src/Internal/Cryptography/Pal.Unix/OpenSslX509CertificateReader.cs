﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed class OpenSslX509CertificateReader : ICertificatePal
    {
        private static DateTimeFormatInfo s_validityDateTimeFormatInfo;

        private SafeX509Handle _cert;
        private X500DistinguishedName _subjectName;
        private X500DistinguishedName _issuerName;

        internal OpenSslX509CertificateReader(SafeX509Handle handle)
        {
            // X509_check_purpose has the effect of populating the sha1_hash value,
            // and other "initialize" type things.
            bool init = Interop.libcrypto.X509_check_purpose(handle, -1, 0);

            if (!init)
            {
                throw Interop.libcrypto.CreateOpenSslCryptographicException();
            }

            _cert = handle;
        }

        internal unsafe OpenSslX509CertificateReader(byte[] data)
        {
            SafeX509Handle cert;

            // If the first byte is a hyphen then this is likely PEM-encoded,
            // otherwise it's DER-encoded (or not a certificate).
            if (data[0] == '-')
            {
                using (SafeBioHandle bio = Interop.libcrypto.BIO_new(Interop.libcrypto.BIO_s_mem()))
                {
                    Interop.libcrypto.CheckValidOpenSslHandle(bio);

                    Interop.libcrypto.BIO_write(bio, data, data.Length);
                    cert = Interop.libcrypto.PEM_read_bio_X509_AUX(bio, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                }
            }
            else
            {
                cert = Interop.libcrypto.OpenSslD2I(Interop.libcrypto.d2i_X509, data);
            }

            Interop.libcrypto.CheckValidOpenSslHandle(cert);

            // X509_check_purpose has the effect of populating the sha1_hash value,
            // and other "initialize" type things.
            bool init = Interop.libcrypto.X509_check_purpose(cert, -1, 0);

            if (!init)
            {
                throw Interop.libcrypto.CreateOpenSslCryptographicException();
            }

            _cert = cert;
        }

        public bool HasPrivateKey
        {
            get { return false; }
        }

        public AsymmetricAlgorithm PrivateKey
        {
            get { return null; }
        }

        public IntPtr Handle
        {
            get { return _cert == null ? IntPtr.Zero : _cert.DangerousGetHandle(); }
        }

        internal SafeX509Handle SafeHandle
        {
            get { return _cert; }
        }

        public string Issuer
        {
            get { return IssuerName.Name; }
        }

        public string Subject
        {
            get { return SubjectName.Name; }
        }

        public byte[] Thumbprint
        {
            get
            {
                return Interop.NativeCrypto.GetX509Thumbprint(_cert);
            }
        }

        public string KeyAlgorithm
        {
            get
            {
                IntPtr oidPtr = Interop.NativeCrypto.GetX509PublicKeyAlgorithm(_cert);
                return Interop.libcrypto.OBJ_obj2txt_helper(oidPtr);
            }
        }

        public byte[] KeyAlgorithmParameters
        {
            get
            {
                return Interop.NativeCrypto.GetX509PublicKeyParameterBytes(_cert);
            }
        }

        public byte[] PublicKeyValue
        {
            get
            {
                IntPtr keyBytesPtr = Interop.NativeCrypto.GetX509PublicKeyBytes(_cert);
                return Interop.NativeCrypto.GetAsn1StringBytes(keyBytesPtr);
            }
        }

        public byte[] SerialNumber
        {
            get
            {
                IntPtr serialNumberPtr = Interop.libcrypto.X509_get_serialNumber(_cert);
                byte[] serial = Interop.NativeCrypto.GetAsn1StringBytes(serialNumberPtr);

                // Windows returns this in BigInteger Little-Endian,
                // OpenSSL returns this in BigInteger Big-Endian.
                Array.Reverse(serial);
                return serial;
            }
        }

        public string SignatureAlgorithm
        {
            get
            {
                IntPtr oidPtr = Interop.NativeCrypto.GetX509SignatureAlgorithm(_cert);
                return Interop.libcrypto.OBJ_obj2txt_helper(oidPtr);
            }
        }

        public DateTime NotAfter
        {
            get
            {
                return ExtractValidityDateTime(Interop.NativeCrypto.GetX509NotAfter(_cert));
            }
        }

        public DateTime NotBefore
        {
            get
            {
                return ExtractValidityDateTime(Interop.NativeCrypto.GetX509NotBefore(_cert));
            }
        }

        public unsafe byte[] RawData
        {
            get { return Interop.libcrypto.OpenSslI2D(Interop.libcrypto.i2d_X509, _cert); }
        }

        public int Version
        {
            get
            {
                int version = Interop.NativeCrypto.GetX509Version(_cert);

                if (version < 0)
                {
                    throw new CryptographicException();
                }

                // The file encoding is v1(0), v2(1), v3(2).
                // The .NET answers are 1, 2, 3.
                return version + 1;
            }
        }

        public bool Archived
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public string FriendlyName
        {
            get { return ""; }
            set { throw new NotImplementedException(); }
        }

        public X500DistinguishedName SubjectName
        {
            get
            {
                if (_subjectName == null)
                {
                    _subjectName = LoadX500Name(Interop.libcrypto.X509_get_subject_name(_cert));
                }

                return _subjectName;
            }
        }

        public X500DistinguishedName IssuerName
        {
            get
            {
                if (_issuerName == null)
                {
                    _issuerName = LoadX500Name(Interop.libcrypto.X509_get_issuer_name(_cert));
                }

                return _issuerName;
            }
        }

        public IEnumerable<X509Extension> Extensions
        {
            get
            {
                int extensionCount = Interop.libcrypto.X509_get_ext_count(_cert);
                LowLevelListWithIList<X509Extension> extensions = new LowLevelListWithIList<X509Extension>(extensionCount);

                for (int i = 0; i < extensionCount; i++)
                {
                    IntPtr ext = Interop.libcrypto.X509_get_ext(_cert, i);

                    Interop.libcrypto.CheckValidOpenSslHandle(ext);

                    IntPtr oidPtr = Interop.libcrypto.X509_EXTENSION_get_object(ext);

                    Interop.libcrypto.CheckValidOpenSslHandle(oidPtr);

                    string oidValue = Interop.libcrypto.OBJ_obj2txt_helper(oidPtr);
                    Oid oid = new Oid(oidValue);

                    IntPtr dataPtr = Interop.libcrypto.X509_EXTENSION_get_data(ext);

                    Interop.libcrypto.CheckValidOpenSslHandle(dataPtr);

                    byte[] extData = Interop.NativeCrypto.GetAsn1StringBytes(dataPtr);
                    bool critical = Interop.libcrypto.X509_EXTENSION_get_critical(ext);
                    X509Extension extension = new X509Extension(oid, extData, critical);

                    extensions.Add(extension);
                }

                return extensions;
            }
        }

        public void SetPrivateKey(AsymmetricAlgorithm privateKey, AsymmetricAlgorithm publicKey)
        {
            throw new NotImplementedException();
        }

        public string GetNameInfo(X509NameType nameType, bool forIssuer)
        {
            using (SafeBioHandle bioHandle = Interop.NativeCrypto.GetX509NameInfo(_cert, (int)nameType, forIssuer))
            {
                if (bioHandle.IsInvalid)
                {
                    return "";
                }

                int bioSize = Interop.libcrypto.GetMemoryBioSize(bioHandle);
                // Ensure space for the trailing \0
                StringBuilder builder = new StringBuilder(bioSize + 1);
                int read = Interop.libcrypto.BIO_gets(bioHandle, builder, builder.Capacity);

                if (read < 0)
                {
                    throw Interop.libcrypto.CreateOpenSslCryptographicException();
                }

                return builder.ToString();
            }
        }

        public void AppendPrivateKeyInfo(StringBuilder sb)
        {
        }

        public void Dispose()
        {
            if (_cert != null)
            {
                _cert.Dispose();
                _cert = null;
            }
        }

        private static X500DistinguishedName LoadX500Name(IntPtr namePtr)
        {
            Interop.libcrypto.CheckValidOpenSslHandle(namePtr);

            byte[] buf = Interop.NativeCrypto.GetX509NameRawBytes(namePtr);
            return new X500DistinguishedName(buf);
        }

        private static DateTime ExtractValidityDateTime(IntPtr validityDatePtr)
        {
            byte[] bytes = Interop.NativeCrypto.GetAsn1StringBytes(validityDatePtr);

            // RFC 5280 (X509v3 - https://tools.ietf.org/html/rfc5280)
            // states that the validity times are either UTCTime:YYMMDDHHMMSSZ (13 bytes)
            // or GeneralizedTime:YYYYMMDDHHMMSSZ (15 bytes).
            // Technically, both UTCTime and GeneralizedTime can have more complicated
            // representations, but X509 restricts them to only the one form each.
            //
            // Furthermore, the UTCTime year values are to be interpreted as 1950-2049.
            //
            // No mention is made in RFC 5280 of different rules for v1 or v2 certificates.

            Debug.Assert(bytes != null);
            Debug.Assert(
                bytes.Length == 13 || bytes.Length == 15,
                "DateTime value should be UTCTime (13 bytes) or GeneralizedTime (15 bytes)");

            Debug.Assert(
                bytes[bytes.Length - 1] == 'Z',
                "DateTime value should end with Z marker");

            if (bytes == null || bytes.Length < 1 || bytes[bytes.Length - 1] != 'Z')
            {
                throw new CryptographicException();
            }

            string dateString = Encoding.ASCII.GetString(bytes);

            if (s_validityDateTimeFormatInfo == null)
            {
                DateTimeFormatInfo validityFormatInfo =
                    (DateTimeFormatInfo)CultureInfo.InvariantCulture.DateTimeFormat.Clone();

                // Two-digit years are 1950-2049
                validityFormatInfo.Calendar.TwoDigitYearMax = 2049;

                s_validityDateTimeFormatInfo = validityFormatInfo;
            }

            if (bytes.Length == 13)
            {
                DateTime utcTime;

                if (!DateTime.TryParseExact(
                    dateString,
                    "yyMMddHHmmss'Z'",
                    s_validityDateTimeFormatInfo,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out utcTime))
                {
                    throw new CryptographicException();
                }

                return utcTime.ToLocalTime();
            }

            if (bytes.Length == 15)
            {
                DateTime generalizedTime;

                if (!DateTime.TryParseExact(
                    dateString,
                    "yyyyMMddHHmmss'Z'",
                    s_validityDateTimeFormatInfo,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out generalizedTime))
                {
                    throw new CryptographicException();
                }

                return generalizedTime.ToLocalTime();
            }

            throw new CryptographicException();
        }
    }
}
