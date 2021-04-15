// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_types.h"
#include "pal_utilities.h"
#include "pal_safecrt.h"
#include "openssl.h"

#include <assert.h>
#include <limits.h>
#include <pthread.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <unistd.h>

// See X509NameType.SimpleName
#define NAME_TYPE_SIMPLE 0
// See X509NameType.EmailName
#define NAME_TYPE_EMAIL 1
// See X509NameType.UpnName
#define NAME_TYPE_UPN 2
// See X509NameType.DnsName
#define NAME_TYPE_DNS 3
// See X509NameType.DnsFromAlternateName
#define NAME_TYPE_DNSALT 4
// See X509NameType.UrlName
#define NAME_TYPE_URL 5

/*
Function:
MakeTimeT

Used to convert the constituent elements of a struct tm into a time_t. As time_t does not have
a guaranteed blitting size, this function is static and cannot be p/invoked. It is here merely
as a utility.

Return values:
A time_t representation of the input date. See also man mktime(3).
*/
static time_t
MakeTimeT(int32_t year, int32_t month, int32_t day, int32_t hour, int32_t minute, int32_t second, int32_t isDst)
{
    struct tm currentTm;
    currentTm.tm_year = year - 1900;
    currentTm.tm_mon = month - 1;
    currentTm.tm_mday = day;
    currentTm.tm_hour = hour;
    currentTm.tm_min = minute;
    currentTm.tm_sec = second;
    currentTm.tm_isdst = isDst;
    return mktime(&currentTm);
}

/*
Function:
GetX509Thumbprint

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to copy the SHA1
digest of the certificate (the thumbprint) into a managed buffer.

Return values:
0: Invalid X509 pointer
1: Data was copied
Any negative value: The input buffer size was reported as insufficient. A buffer of size ABS(return) is required.
*/
int32_t CryptoNative_GetX509Thumbprint(X509* x509, uint8_t* pBuf, int32_t cBuf)
{
    if (!x509)
    {
        return 0;
    }

    if (cBuf < SHA_DIGEST_LENGTH)
    {
        return -SHA_DIGEST_LENGTH;
    }

    if (!X509_digest(x509, EVP_sha1(), pBuf, NULL))
    {
        return 0;
    }

    return 1;
}

/*
Function:
GetX509NotBefore

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to identify the
beginning of the validity period of the certificate in question.

Return values:
NULL if the validity cannot be determined, a pointer to the ASN1_TIME structure for the NotBefore value
otherwise.
*/
const ASN1_TIME* CryptoNative_GetX509NotBefore(X509* x509)
{
    if (x509)
    {
        return X509_get0_notBefore(x509);
    }

    return NULL;
}

/*
Function:
GetX509NotAfter

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to identify the
end of the validity period of the certificate in question.

Return values:
NULL if the validity cannot be determined, a pointer to the ASN1_TIME structure for the NotAfter value
otherwise.
*/
const ASN1_TIME* CryptoNative_GetX509NotAfter(X509* x509)
{
    if (x509)
    {
        return X509_get0_notAfter(x509);
    }

    return NULL;
}

/*
Function:
GetX509CrlNextUpdate

Used by System.Security.Cryptography.X509Certificates' CrlCache to identify the
end of the validity period of the certificate revocation list in question.

Return values:
NULL if the validity cannot be determined, a pointer to the ASN1_TIME structure for the NextUpdate value
otherwise.
*/
const ASN1_TIME* CryptoNative_GetX509CrlNextUpdate(X509_CRL* crl)
{
    if (crl)
    {
        return X509_CRL_get0_nextUpdate(crl);
    }

    return NULL;
}

/*
Function:
GetX509Version

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to identify the
X509 data format version for this certificate.

Return values:
-1 if the value cannot be determined
The encoded value of the version, otherwise:
  0: X509v1
  1: X509v2
  2: X509v3
*/
int32_t CryptoNative_GetX509Version(X509* x509)
{
    if (x509)
    {
        return (int32_t)X509_get_version(x509);
    }

    return -1;
}

/*
Function:
GetX509PublicKeyAlgorithm

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to identify the
algorithm the public key is associated with.

Return values:
NULL if the algorithm cannot be determined, otherwise a pointer to the OpenSSL ASN1_OBJECT structure
describing the object type.
*/
ASN1_OBJECT* CryptoNative_GetX509PublicKeyAlgorithm(X509* x509)
{
    if (x509)
    {
        X509_PUBKEY* pubkey = X509_get_X509_PUBKEY(x509);
        ASN1_OBJECT* algOid;

        if (pubkey && X509_PUBKEY_get0_param(&algOid, NULL, NULL, NULL, pubkey))
        {
            return algOid;
        }
    }

    return NULL;
}

/*
Function:
GetX509SignatureAlgorithm

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to identify the
algorithm used by the Certificate Authority for signing the certificate.

Return values:
NULL if the algorithm cannot be determined, otherwise a pointer to the OpenSSL ASN1_OBJECT structure
describing the object type.
*/
ASN1_OBJECT* CryptoNative_GetX509SignatureAlgorithm(X509* x509)
{
    if (x509)
    {
        const X509_ALGOR* sigAlg = X509_get0_tbs_sigalg(x509);

        if (sigAlg)
        {
            return sigAlg->algorithm;
        }
    }

    return NULL;
}

/*
Function:
GetX509PublicKeyParameterBytes

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to copy out the
parameters to the algorithm used by the certificate public key

Return values:
0: Invalid X509 pointer
1: Data was copied
Any negative value: The input buffer size was reported as insufficient. A buffer of size ABS(return) is required.
*/
int32_t CryptoNative_GetX509PublicKeyParameterBytes(X509* x509, uint8_t* pBuf, int32_t cBuf)
{
    if (!x509)
    {
        return 0;
    }

    X509_PUBKEY* pubkey = X509_get_X509_PUBKEY(x509);

    if (!pubkey)
    {
        return 0;
    }

    X509_ALGOR* alg;

    if (!X509_PUBKEY_get0_param(NULL, NULL, NULL, &alg, pubkey) || !alg)
    {
        return 0;
    }

    ASN1_TYPE* parameter = alg->parameter;

    if (!parameter)
    {
        // If pBuf is NULL we're asking for the length, so return 0 (which is negative-zero)
        // If pBuf is non-NULL we're asking to fill the data, in which case we return 1.
        return pBuf != NULL;
    }

    int len = i2d_ASN1_TYPE(parameter, NULL);

    if (cBuf < len)
    {
        return -len;
    }

    unsigned char* pBuf2 = pBuf;
    len = i2d_ASN1_TYPE(parameter, &pBuf2);

    if (len > 0)
    {
        return 1;
    }

    return 0;
}

/*
Function:
GetX509PublicKeyBytes

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to obtain the
raw bytes of the public key.

Return values:
NULL if the public key cannot be determined, a pointer to the ASN1_BIT_STRING structure representing
the public key.
*/
ASN1_BIT_STRING* CryptoNative_GetX509PublicKeyBytes(X509* x509)
{
    if (x509)
    {
        return X509_get0_pubkey_bitstr(x509);
    }

    return NULL;
}

/*
Function:
GetAsn1StringBytes

Used by the NativeCrypto shim type to extract byte[] data from OpenSSL ASN1_* types whenever a byte[] is called
for in managed code.

Return values:
0: Invalid X509 pointer
1: Data was copied
Any negative value: The input buffer size was reported as insufficient. A buffer of size ABS(return) is required.

Remarks:
 Many ASN1 types are actually the same type in OpenSSL:
   STRING
   INTEGER
   ENUMERATED
   BIT_STRING
   OCTET_STRING
   PRINTABLESTRING
   T61STRING
   IA5STRING
   GENERALSTRING
   UNIVERSALSTRING
   BMPSTRING
   UTCTIME
   TIME
   GENERALIZEDTIME
   VISIBLEStRING
   UTF8STRING

 So this function will really work on all of them.
*/
int32_t CryptoNative_GetAsn1StringBytes(ASN1_STRING* asn1, uint8_t* pBuf, int32_t cBuf)
{
    if (!asn1 || cBuf < 0)
    {
        return 0;
    }

    int length = asn1->length;
    assert(length >= 0);
    if (length < 0)
    {
        return 0;
    }

    if (!pBuf || cBuf < length)
    {
        return -length;
    }

    memcpy_s(pBuf, Int32ToSizeT(cBuf), asn1->data, Int32ToSizeT(length));
    return 1;
}

/*
Function:
GetX509NameRawBytes

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader to obtain the
DER encoded value of an X500DistinguishedName.

Return values:
0: Invalid X509 pointer
1: Data was copied
Any negative value: The input buffer size was reported as insufficient. A buffer of size ABS(return) is required.
*/
int32_t CryptoNative_GetX509NameRawBytes(X509_NAME* x509Name, uint8_t* pBuf, int32_t cBuf)
{
    const uint8_t* nameBuf;
    size_t nameBufLen;

    if (!x509Name || cBuf < 0 || !X509_NAME_get0_der(x509Name, &nameBuf, &nameBufLen))
    {
        return 0;
    }

    /*
     * length is size_t on some platforms and int on others, so the comparisons
     * are not tautological everywhere. We can let the compiler optimize away
     * any part of the check that is. We split the size checks into two checks
     * so we can get around the warnings on Linux where the Length is unsigned
     * whereas Length is signed on OS X. The first check makes sure the variable
     * value is less than INT_MAX in it's native format; once we know it is not
     * too large, we can safely cast to an int to make sure it is not negative
     */
    if (nameBufLen > INT_MAX)
    {
        assert(0 && "Huge length X509_NAME");
        return 0;
    }

    int length = (int)(nameBufLen);

    if (length < 0)
    {
        assert(0 && "Negative length X509_NAME");
        return 0;
    }

    if (!pBuf || cBuf < length)
    {
        return -length;
    }

    memcpy_s(pBuf, Int32ToSizeT(cBuf), nameBuf, Int32ToSizeT(length));
    return 1;
}

/*
Function:
GetX509EkuFieldCount

Used by System.Security.Cryptography.X509Certificates' OpenSslX509Encoder to identify the
number of Extended Key Usage OIDs present in the EXTENDED_KEY_USAGE structure.

Return values:
0 if the field count cannot be determined, or the count of OIDs present in the EKU.
Note that 0 does not always indicate an error, merely that GetX509EkuField should not be called.
*/
int32_t CryptoNative_GetX509EkuFieldCount(EXTENDED_KEY_USAGE* eku)
{
    return sk_ASN1_OBJECT_num(eku);
}

/*
Function:
GetX509EkuField

Used by System.Security.Cryptography.X509Certificates' OpenSslX509Encoder to get a pointer to the
ASN1_OBJECT structure which represents the OID in a particular spot in the EKU.

Return values:
NULL if eku is NULL or loc is out of bounds, otherwise a pointer to the ASN1_OBJECT structure encoding
that particular OID.
*/
ASN1_OBJECT* CryptoNative_GetX509EkuField(EXTENDED_KEY_USAGE* eku, int32_t loc)
{
    return sk_ASN1_OBJECT_value(eku, loc);
}

/*
Function:
GetX509NameInfo

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader as the entire
implementation of X509Certificate2.GetNameInfo.

Return values:
NULL if the certificate is invalid or no name information could be found, otherwise a pointer to a
memory-backed BIO structure which contains the answer to the GetNameInfo query
*/
BIO* CryptoNative_GetX509NameInfo(X509* x509, int32_t nameType, int32_t forIssuer)
{
    static const char szOidUpn[] = "1.3.6.1.4.1.311.20.2.3";

    if (!x509 || nameType < NAME_TYPE_SIMPLE || nameType > NAME_TYPE_URL)
    {
        return NULL;
    }

    // Algorithm behaviors (pseudocode).  When forIssuer is true, replace "Subject" with "Issuer" and
    // SAN (Subject Alternative Names) with IAN (Issuer Alternative Names).
    //
    // SimpleName: Subject[CN] ?? Subject[OU] ?? Subject[O] ?? Subject[E] ?? Subject.Rdns.FirstOrDefault() ??
    // SAN.Entries.FirstOrDefault(type == GEN_EMAIL);
    // EmailName: SAN.Entries.FirstOrDefault(type == GEN_EMAIL) ?? Subject[E];
    // UpnName: SAN.Entries.FirsOrDefaultt(type == GEN_OTHER && entry.AsOther().OID == szOidUpn).AsOther().Value;
    // DnsName: SAN.Entries.FirstOrDefault(type == GEN_DNS) ?? Subject[CN];
    // DnsFromAlternativeName: SAN.Entries.FirstOrDefault(type == GEN_DNS);
    // UrlName: SAN.Entries.FirstOrDefault(type == GEN_URI);
    if (nameType == NAME_TYPE_SIMPLE)
    {
        X509_NAME* name = forIssuer ? X509_get_issuer_name(x509) : X509_get_subject_name(x509);

        if (name)
        {
            ASN1_STRING* cn = NULL;
            ASN1_STRING* ou = NULL;
            ASN1_STRING* o = NULL;
            ASN1_STRING* e = NULL;
            ASN1_STRING* firstRdn = NULL;

            // Walk the list backwards because it is stored in stack order
            for (int i = X509_NAME_entry_count(name) - 1; i >= 0; --i)
            {
                X509_NAME_ENTRY* entry = X509_NAME_get_entry(name, i);

                if (!entry)
                {
                    continue;
                }

                ASN1_OBJECT* oid = X509_NAME_ENTRY_get_object(entry);
                ASN1_STRING* str = X509_NAME_ENTRY_get_data(entry);

                if (!oid || !str)
                {
                    continue;
                }

                int nid = OBJ_obj2nid(oid);

                if (nid == NID_commonName)
                {
                    // CN wins, so no need to keep looking.
                    cn = str;
                    break;
                }
                else if (nid == NID_organizationalUnitName)
                {
                    ou = str;
                }
                else if (nid == NID_organizationName)
                {
                    o = str;
                }
                else if (nid == NID_pkcs9_emailAddress)
                {
                    e = str;
                }
                else if (!firstRdn)
                {
                    firstRdn = str;
                }
            }

            ASN1_STRING* answer = cn;

            // If there was no CN, but there was something, then perform fallbacks.
            if (!answer && firstRdn)
            {
                answer = ou;

                if (!answer)
                {
                    answer = o;
                }

                if (!answer)
                {
                    answer = e;
                }

                if (!answer)
                {
                    answer = firstRdn;
                }
            }

            if (answer)
            {
                BIO* b = BIO_new(BIO_s_mem());
                ASN1_STRING_print_ex(b, answer, 0);
                return b;
            }
        }
    }

    if (nameType == NAME_TYPE_SIMPLE || nameType == NAME_TYPE_DNS || nameType == NAME_TYPE_DNSALT ||
        nameType == NAME_TYPE_EMAIL || nameType == NAME_TYPE_UPN || nameType == NAME_TYPE_URL)
    {
        int expectedType = -1;

        switch (nameType)
        {
            case NAME_TYPE_DNS:
            case NAME_TYPE_DNSALT:
                expectedType = GEN_DNS;
                break;
            case NAME_TYPE_SIMPLE:
            case NAME_TYPE_EMAIL:
                expectedType = GEN_EMAIL;
                break;
            case NAME_TYPE_UPN:
                expectedType = GEN_OTHERNAME;
                break;
            case NAME_TYPE_URL:
                expectedType = GEN_URI;
                break;
        }

        STACK_OF(GENERAL_NAME)* altNames = (STACK_OF(GENERAL_NAME)*)(
            X509_get_ext_d2i(x509, forIssuer ? NID_issuer_alt_name : NID_subject_alt_name, NULL, NULL));

        if (altNames)
        {
            int i;

            for (i = 0; i < sk_GENERAL_NAME_num(altNames); ++i)
            {
                GENERAL_NAME* altName = sk_GENERAL_NAME_value(altNames, i);

                if (altName && altName->type == expectedType)
                {
                    ASN1_STRING* str = NULL;

                    switch (nameType)
                    {
                        case NAME_TYPE_DNS:
                        case NAME_TYPE_DNSALT:
                            str = altName->d.dNSName;
                            break;
                        case NAME_TYPE_SIMPLE:
                        case NAME_TYPE_EMAIL:
                            str = altName->d.rfc822Name;
                            break;
                        case NAME_TYPE_URL:
                            str = altName->d.uniformResourceIdentifier;
                            break;
                        case NAME_TYPE_UPN:
                        {
                            OTHERNAME* value = altName->d.otherName;

                            if (value)
                            {
                                // Enough more padding than szOidUpn that a \0 won't accidentally align
                                char localOid[sizeof(szOidUpn) + 3];
                                int cchLocalOid = 1 + OBJ_obj2txt(localOid, sizeof(localOid), value->type_id, 1);

                                if (sizeof(szOidUpn) == cchLocalOid &&
                                    0 == strncmp(localOid, szOidUpn, sizeof(szOidUpn)))
                                {
                                    // OTHERNAME->ASN1_TYPE->union.field
                                    str = value->value->value.asn1_string;
                                }
                            }

                            break;
                        }
                    }

                    if (str)
                    {
                        BIO* b = BIO_new(BIO_s_mem());
                        ASN1_STRING_print_ex(b, str, 0);
                        sk_GENERAL_NAME_free(altNames);
                        return b;
                    }
                }
            }

            sk_GENERAL_NAME_free(altNames);
        }
    }

    if (nameType == NAME_TYPE_EMAIL || nameType == NAME_TYPE_DNS)
    {
        X509_NAME* name = forIssuer ? X509_get_issuer_name(x509) : X509_get_subject_name(x509);
        int expectedNid = NID_undef;

        switch (nameType)
        {
            case NAME_TYPE_EMAIL:
                expectedNid = NID_pkcs9_emailAddress;
                break;
            case NAME_TYPE_DNS:
                expectedNid = NID_commonName;
                break;
        }

        if (name)
        {
            // Walk the list backwards because it is stored in stack order
            for (int i = X509_NAME_entry_count(name) - 1; i >= 0; --i)
            {
                X509_NAME_ENTRY* entry = X509_NAME_get_entry(name, i);

                if (!entry)
                {
                    continue;
                }

                ASN1_OBJECT* oid = X509_NAME_ENTRY_get_object(entry);
                ASN1_STRING* str = X509_NAME_ENTRY_get_data(entry);

                if (!oid || !str)
                {
                    continue;
                }

                int nid = OBJ_obj2nid(oid);

                if (nid == expectedNid)
                {
                    BIO* b = BIO_new(BIO_s_mem());
                    ASN1_STRING_print_ex(b, str, 0);
                    return b;
                }
            }
        }
    }

    return NULL;
}

/*
Function:
CheckX509Hostname

Used by System.Net.Security's Unix CertModule to identify if the certificate presented by
the server is applicable to the hostname requested.

Return values:
1 if the hostname is a match
0 if the hostname is not a match
Any negative number indicates an error in the arguments.
*/
int32_t CryptoNative_CheckX509Hostname(X509* x509, const char* hostname, int32_t cchHostname)
{
    // Input errors.  OpenSSL might return -1 or -2, so skip those.
    if (!x509)
        return -3;
    if (cchHostname > 0 && !hostname)
        return -4;
    if (cchHostname < 0)
        return -5;

    // OpenSSL will treat a target hostname starting with '.' as special.
    // We don't expect target hostnames to start with '.', but if one gets in here, the fallback
    // and the mainline won't be the same... so just make it report false.
    if (cchHostname > 0 && hostname[0] == '.')
    {
        return 0;
    }

    return X509_check_host(
        x509,
        hostname,
        (size_t)cchHostname,
        X509_CHECK_FLAG_NO_PARTIAL_WILDCARDS,
        NULL);
}

/*
Function:
CheckX509IpAddress

Used by System.Net.Security's Unix CertModule to identify if the certificate presented by
the server is applicable to the hostname (an IP address) requested.

Return values:
1 if the hostname is a match
0 if the hostname is not a match
Any negative number indicates an error in the arguments.
*/
int32_t CryptoNative_CheckX509IpAddress(
    X509* x509, const uint8_t* addressBytes, int32_t addressBytesLen, const char* hostname, int32_t cchHostname)
{
    if (!x509)
        return -2;
    if (cchHostname > 0 && !hostname)
        return -3;
    if (cchHostname < 0)
        return -4;
    if (addressBytesLen < 0)
        return -5;
    if (!addressBytes)
        return -6;

    int subjectNid = NID_commonName;
    int sanGenType = GEN_IPADD;
    GENERAL_NAMES* san = (GENERAL_NAMES*)(X509_get_ext_d2i(x509, NID_subject_alt_name, NULL, NULL));
    int success = 0;

    if (san)
    {
        int i;
        int count = sk_GENERAL_NAME_num(san);

        for (i = 0; i < count; ++i)
        {
            GENERAL_NAME* sanEntry = sk_GENERAL_NAME_value(san, i);
            ASN1_OCTET_STRING* ipAddr;

            if (sanEntry->type != sanGenType)
            {
                continue;
            }

            ipAddr = sanEntry->d.iPAddress;

            if (!ipAddr || !ipAddr->data || ipAddr->length != addressBytesLen)
            {
                continue;
            }

            if (!memcmp(addressBytes, ipAddr->data, (size_t)addressBytesLen))
            {
                success = 1;
                break;
            }
        }

        GENERAL_NAMES_free(san);
    }

    if (!success)
    {
        // This is a shared/interor pointer, do not free!
        X509_NAME* subject = X509_get_subject_name(x509);

        if (subject)
        {
            int i = -1;

            while ((i = X509_NAME_get_index_by_NID(subject, subjectNid, i)) >= 0)
            {
                // Shared/interior pointers, do not free!
                X509_NAME_ENTRY* nameEnt = X509_NAME_get_entry(subject, i);
                ASN1_STRING* cn = X509_NAME_ENTRY_get_data(nameEnt);

                if (cn->length == cchHostname &&
                    !strncasecmp((const char*)cn->data, hostname, (size_t)cchHostname))
                {
                    success = 1;
                    break;
                }
            }
        }
    }

    return success;
}
/*
Function:
GetX509StackFieldCount

Used by System.Security.Cryptography.X509Certificates' OpenSslX509ChainProcessor to identify the
number of certificates returned in the built chain.

Return values:
0 if the field count cannot be determined, or the count of certificates in STACK_OF(X509)
Note that 0 does not always indicate an error, merely that GetX509StackField should not be called.
*/
int32_t CryptoNative_GetX509StackFieldCount(STACK_OF(X509) * stack)
{
    return sk_X509_num(stack);
}

/*
Function:
GetX509StackField

Used by System.Security.Cryptography.X509Certificates' OpenSslX509ChainProcessor to get a pointer to
the indexed member of a chain.

Return values:
NULL if stack is NULL or loc is out of bounds, otherwise a pointer to the X509 structure encoding
that particular element.
*/
X509* CryptoNative_GetX509StackField(STACK_OF(X509) * stack, int loc)
{
    return sk_X509_value(stack, loc);
}

/*
Function:
RecursiveFreeX509Stack

Used by System.Security.Cryptography.X509Certificates' OpenSslX509ChainProcessor to free a stack
when done with it.
*/
void CryptoNative_RecursiveFreeX509Stack(STACK_OF(X509) * stack)
{
    sk_X509_pop_free(stack, X509_free);
}

/*
Function:
SetX509StoreVerifyTime

Used by System.Security.Cryptography.X509Certificates' OpenSslX509ChainProcessor to assign the
verification time to the chain building.  The input is in LOCAL time, not UTC.

Return values:
0 if ctx is NULL, if ctx has no X509_VERIFY_PARAM, or the date inputs don't produce a valid time_t;
1 on success.
*/
int32_t CryptoNative_X509StoreSetVerifyTime(X509_STORE* ctx,
                                            int32_t year,
                                            int32_t month,
                                            int32_t day,
                                            int32_t hour,
                                            int32_t minute,
                                            int32_t second,
                                            int32_t isDst)
{
    if (!ctx)
    {
        return 0;
    }

    time_t verifyTime = MakeTimeT(year, month, day, hour, minute, second, isDst);

    if (verifyTime == (time_t)-1)
    {
        return 0;
    }

    X509_VERIFY_PARAM* verifyParams = X509_STORE_get0_param(ctx);

    if (!verifyParams)
    {
        return 0;
    }

    X509_VERIFY_PARAM_set_time(verifyParams, verifyTime);
    return 1;
}

/*
Function:
ReadX509AsDerFromBio

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader when attempting
to turn the contents of a file into an ICertificatePal object.

Return values:
If bio containns a valid DER-encoded X509 object, a pointer to that X509 structure that was deserialized,
otherwise NULL.
*/
X509* CryptoNative_ReadX509AsDerFromBio(BIO* bio)
{
    return d2i_X509_bio(bio, NULL);
}

/*
Function:
BioTell

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader when attempting
to turn the contents of a file into an ICertificatePal object to allow seeking back to the start point
in the event of a deserialization failure.

Return values:
The current seek position of the BIO if it is a file-related BIO, -1 on NULL inputs, and has unspecified
behavior on non-file, non-null BIO objects.

See also:
OpenSSL's BIO_tell
*/
int32_t CryptoNative_BioTell(BIO* bio)
{
    if (!bio)
    {
        return -1;
    }

    return BIO_tell(bio);
}

/*
Function:
BioTell

Used by System.Security.Cryptography.X509Certificates' OpenSslX509CertificateReader when attempting
to turn the contents of a file into an ICertificatePal object to seek back to the start point
in the event of a deserialization failure.

Return values:
-1 if bio is NULL
-1 if bio is a file-related BIO and seek fails
0 if bio is a file-related BIO and seek succeeds
otherwise unspecified

See also:
OpenSSL's BIO_seek
*/
int32_t CryptoNative_BioSeek(BIO* bio, int32_t ofs)
{
    if (!bio)
    {
        return -1;
    }

    return BIO_seek(bio, ofs);
}

/*
Function:
NewX509Stack

Used by System.Security.Cryptography.X509Certificates when needing to pass a collection
of X509* to OpenSSL.

Return values:
A STACK_OF(X509*) with no comparator.
*/
STACK_OF(X509) * CryptoNative_NewX509Stack()
{
    return sk_X509_new_null();
}

/*
Function:
PushX509StackField

Used by System.Security.Cryptography.X509Certificates when needing to pass a collection
of X509* to OpenSSL.

Return values:
1 on success
0 on a NULL stack, or an error within sk_X509_push
*/
int32_t CryptoNative_PushX509StackField(STACK_OF(X509) * stack, X509* x509)
{
    if (!stack)
    {
        return 0;
    }

    return sk_X509_push(stack, x509);
}

/*
Function:
GetRandomBytes

Puts num cryptographically strong pseudo-random bytes into buf.

Return values:
Returns a bool to managed code.
1 for success
0 for failure
*/
int32_t CryptoNative_GetRandomBytes(uint8_t* buf, int32_t num)
{
    int ret = RAND_bytes(buf, num);

    return ret == 1;
}

/*
Function:
LookupFriendlyNameByOid

Looks up the FriendlyName value for a given OID in string representation.
For example, "1.3.14.3.2.26" => "sha1".

Return values:
1 indicates that *friendlyName contains a pointer to the friendly name value
0 indicates that the OID was not found, or no friendly name exists for that OID
-1 indicates OpenSSL signalled an error, CryptographicException should be raised.
-2 indicates an error in the input arguments
*/
int32_t CryptoNative_LookupFriendlyNameByOid(const char* oidValue, const char** friendlyName)
{
    ASN1_OBJECT* oid;
    int nid;
    const char* ln;

    if (!oidValue || !friendlyName)
    {
        return -2;
    }

    // Do a lookup with no_name set. The purpose of this function is to map only the
    // dotted decimal to the friendly name. "sha1" in should not result in "sha1" out.
    oid = OBJ_txt2obj(oidValue, 1);

    if (!oid)
    {
        unsigned long err = ERR_peek_last_error();

        // If the most recent error pushed onto the error queue is NOT from OID parsing
        // then signal for an exception to be thrown.
        if (err != 0 && ERR_GET_FUNC(err) != ASN1_F_A2D_ASN1_OBJECT)
        {
            return -1;
        }

        return 0;
    }

    // Look in the predefined, and late-registered, OIDs list to get the lookup table
    // identifier for this OID.  The OBJ_txt2obj object will not have ln set.
    nid = OBJ_obj2nid(oid);

    if (nid == NID_undef)
    {
        return 0;
    }

    // Get back a shared pointer to the long name from the registration table.
    ln = OBJ_nid2ln(nid);

    if (ln)
    {
        *friendlyName = ln;
        return 1;
    }

    return 0;
}

/*
Function:
SSLeay (OpenSSL_version_num for OpenSSL 1.1+)

Gets the version of openssl library.

Return values:
Version number as MNNFFRBB (major minor fix final beta/patch)
*/
int64_t CryptoNative_OpenSslVersionNumber()
{
    return (int64_t)OpenSSL_version_num();
}

void CryptoNative_RegisterLegacyAlgorithms()
{
#if NEED_OPENSSL_3_0
    if (API_EXISTS(OSSL_PROVIDER_try_load))
    {
        OSSL_PROVIDER_try_load(NULL, "legacy", 1);
    }
#endif
}

#ifdef NEED_OPENSSL_1_0
// Lock used to make sure EnsureopenSslInitialized itself is thread safe
static pthread_mutex_t g_initLock = PTHREAD_MUTEX_INITIALIZER;

// Set of locks initialized for OpenSSL
static pthread_mutex_t* g_locks = NULL;

/*
Function:
LockingCallback

Called back by OpenSSL to lock or unlock.
*/
static void LockingCallback(int mode, int n, const char* file, int line)
{
    (void)file, (void)line; // deliberately unused parameters

// Clang complains about releasing locks that are not held.
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wthread-safety-analysis"

#ifndef CRYPTO_LOCK
#define CRYPTO_LOCK 1
#endif

    int result;
    if (mode & CRYPTO_LOCK)
    {
        result = pthread_mutex_lock(&g_locks[n]);
    }
    else
    {
        result = pthread_mutex_unlock(&g_locks[n]);
    }

    if (result != 0)
    {
        assert(0 && "LockingCallback failed.");
    }
#pragma clang diagnostic pop
}

/*
Function:
EnsureOpenSslInitialized

Initializes OpenSSL with a locking callback to ensure thread safety.

Return values:
0 on success
non-zero on failure
*/
static int32_t EnsureOpenSsl10Initialized()
{
    int ret = 0;
    int numLocks = 0;
    int locksInitialized = 0;
    int randPollResult = 0;

    pthread_mutex_lock(&g_initLock);

    if (g_locks != NULL)
    {
        // Already initialized; nothing more to do.
        goto done;
    }

    // Determine how many locks are needed
    numLocks = CRYPTO_num_locks();
    if (numLocks <= 0)
    {
        assert(0 && "CRYPTO_num_locks returned invalid value.");
        ret = 1;
        goto done;
    }

    // Create the locks array
    size_t allocationSize = 0;
    if (!multiply_s(sizeof(pthread_mutex_t), (size_t)numLocks, &allocationSize))
    {
        ret = 2;
        goto done;
    }

    g_locks = (pthread_mutex_t*)malloc(allocationSize);
    if (g_locks == NULL)
    {
        ret = 2;
        goto done;
    }

    // Initialize each of the locks
    for (locksInitialized = 0; locksInitialized < numLocks; locksInitialized++)
    {
        if (pthread_mutex_init(&g_locks[locksInitialized], NULL) != 0)
        {
            ret = 3;
            goto done;
        }
    }

    // Initialize the callback
    CRYPTO_set_locking_callback(LockingCallback);

    // Initialize the random number generator seed
    randPollResult = RAND_poll();
    if (randPollResult < 1)
    {
        ret = 4;
        goto done;
    }

    // Load the SHA-2 hash algorithms, and anything else not in the default
    // support set.
    OPENSSL_add_all_algorithms_conf();

    // Ensure that the error message table is loaded.
    ERR_load_crypto_strings();

done:
    if (ret != 0)
    {
        // Cleanup on failure
        if (g_locks != NULL)
        {
            for (int i = locksInitialized - 1; i >= 0; i--)
            {
                pthread_mutex_destroy(&g_locks[i]); // ignore failures
            }
            free(g_locks);
            g_locks = NULL;
        }
    }

    pthread_mutex_unlock(&g_initLock);
    return ret;
}
#endif // NEED_OPENSSL_1_0 */

#if defined NEED_OPENSSL_1_1 || defined NEED_OPENSSL_3_0

// Only defined in OpenSSL 1.1.1+, has no effect on 1.1.0.
#ifndef OPENSSL_INIT_NO_ATEXIT
    #define OPENSSL_INIT_NO_ATEXIT 0x00080000L
#endif

static int32_t EnsureOpenSsl11Initialized()
{
    // In OpenSSL 1.0 we call OPENSSL_add_all_algorithms_conf() and ERR_load_crypto_strings(),
    // so do the same for 1.1
    OPENSSL_init_ssl(
        // OPENSSL_add_all_algorithms_conf
            OPENSSL_INIT_ADD_ALL_CIPHERS |
            OPENSSL_INIT_ADD_ALL_DIGESTS |
            OPENSSL_INIT_LOAD_CONFIG |
        // Do not unload on process exit, as the CLR may still have threads running
            OPENSSL_INIT_NO_ATEXIT |
        // ERR_load_crypto_strings
            OPENSSL_INIT_LOAD_CRYPTO_STRINGS |
            OPENSSL_INIT_LOAD_SSL_STRINGS,
        NULL);

    return 0;
}

#endif

int32_t CryptoNative_EnsureOpenSslInitialized()
{
    // If portable then decide which OpenSSL we are, and call the right one.
    // If 1.0, call the 1.0 one.
    // Otherwise call the 1.1 one.
#ifdef FEATURE_DISTRO_AGNOSTIC_SSL
    if (API_EXISTS(SSL_state))
    {
        return EnsureOpenSsl10Initialized();
    }
    else
    {
        return EnsureOpenSsl11Initialized();
    }
#elif OPENSSL_VERSION_NUMBER < OPENSSL_VERSION_1_1_0_RTM
    return EnsureOpenSsl10Initialized();
#else
    return EnsureOpenSsl11Initialized();
#endif
}
