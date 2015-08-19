// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
/*++
Copyright (c) Microsoft Corporation

Module Name:

    _SslSessionsCache.cs

Abstract:
    The file implements SSL session caching mechanism based on a static table of SSL credentials


Author:

    Alexei Vopilov    20-Jul-2004

Revision History:


--*/

using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Collections;

namespace System.Net.Security
{
    internal static class SslSessionsCache
    {
        private const int c_CheckExpiredModulo = 32;
        private static Hashtable s_CachedCreds = new Hashtable(32);

        //
        // Uses cryptographically strong certificate thumbprint comparison
        //
        private struct SslCredKey
        {
            private byte[] _CertThumbPrint;
            private Interop.SchProtocols _AllowedProtocols;
            private EncryptionPolicy _EncryptionPolicy;
            private int _HashCode;
            //
            // SECURITY: X509Certificate.GetCertHash() is virtual hence before going here
            //           the caller of this ctor has to ensure that a user cert object was inspected and
            //           optionally cloned.
            //
            internal SslCredKey(byte[] thumbPrint, Interop.SchProtocols allowedProtocols, EncryptionPolicy encryptionPolicy)
            {
                _CertThumbPrint = thumbPrint == null ? Array.Empty<byte>() : thumbPrint;
                _HashCode = 0;
                if (thumbPrint != null)
                {
                    _HashCode ^= _CertThumbPrint[0];
                    if (1 < _CertThumbPrint.Length)
                        _HashCode ^= (_CertThumbPrint[1] << 8);
                    if (2 < _CertThumbPrint.Length)
                        _HashCode ^= (_CertThumbPrint[2] << 16);
                    if (3 < _CertThumbPrint.Length)
                        _HashCode ^= (_CertThumbPrint[3] << 24);
                }
                _HashCode ^= (int)allowedProtocols;
                _HashCode ^= (int)encryptionPolicy;
                _AllowedProtocols = allowedProtocols;
                _EncryptionPolicy = encryptionPolicy;
            }
            //
            public override int GetHashCode()
            {
                return _HashCode;
            }
            //
            public static bool operator ==(SslCredKey sslCredKey1,
                                            SslCredKey sslCredKey2)
            {
                if ((object)sslCredKey1 == (object)sslCredKey2)
                {
                    return true;
                }
                if ((object)sslCredKey1 == null || (object)sslCredKey2 == null)
                {
                    return false;
                }
                return sslCredKey1.Equals(sslCredKey2);
            }
            //
            public static bool operator !=(SslCredKey sslCredKey1,
                                            SslCredKey sslCredKey2)
            {
                if ((object)sslCredKey1 == (object)sslCredKey2)
                {
                    return false;
                }
                if ((object)sslCredKey1 == null || (object)sslCredKey2 == null)
                {
                    return true;
                }
                return !sslCredKey1.Equals(sslCredKey2);
            }
            //
            public override bool Equals(Object y)
            {
                SslCredKey she = (SslCredKey)y;

                if (_CertThumbPrint.Length != she._CertThumbPrint.Length)
                    return false;

                if (_HashCode != she._HashCode)
                    return false;

                if (_EncryptionPolicy != she._EncryptionPolicy)
                    return false;

                if (_AllowedProtocols != she._AllowedProtocols)
                    return false;

                for (int i = 0; i < _CertThumbPrint.Length; ++i)
                    if (_CertThumbPrint[i] != she._CertThumbPrint[i])
                        return false;

                return true;
            }
        }


        //
        // Returns null or previously cached cred handle
        //
        // ATTN: The returned handle can be invalid, the callers of InitializeSecurityContext and AcceptSecurityContext
        // must be prepared to execute a backout code if the call fails.
        //
        // Note:thumbPrint is a cryptographicaly strong hash of a certificate
        //
        internal static SafeFreeCredentials TryCachedCredential(byte[] thumbPrint, Interop.SchProtocols allowedProtocols, EncryptionPolicy encryptionPolicy)
        {
            if (s_CachedCreds.Count == 0)
            {
                GlobalLog.Print("TryCachedCredential() Not Found, Current Cache Count = " + s_CachedCreds.Count);
                return null;
            }

            object key = new SslCredKey(thumbPrint, allowedProtocols, encryptionPolicy);

            SafeCredentialReference cached = s_CachedCreds[key] as SafeCredentialReference;

            if (cached == null || cached.IsClosed || cached._Target.IsInvalid)
            {
                GlobalLog.Print("TryCachedCredential() Not Found, Current Cache Count = " + s_CachedCreds.Count);
                return null;
            }

            GlobalLog.Print("TryCachedCredential() Found a cached Handle = " + cached._Target.ToString());

            return cached._Target;
        }
        //
        // The app is calling this method after starting an SSL handshake.
        //
        // ATTN: The thumbPrint must be from inspected and possbly cloned user Cert object or we get a security hole in SslCredKey ctor.
        //
        internal static void CacheCredential(SafeFreeCredentials creds, byte[] thumbPrint, Interop.SchProtocols allowedProtocols, EncryptionPolicy encryptionPolicy)
        {
            GlobalLog.Assert(creds != null, "CacheCredential|creds == null");
            if (creds.IsInvalid)
            {
                GlobalLog.Print("CacheCredential() Refused to cache an Invalid Handle = " + creds.ToString() + ", Current Cache Count = " + s_CachedCreds.Count);
                return;
            }

            object key = new SslCredKey(thumbPrint, allowedProtocols, encryptionPolicy);

            SafeCredentialReference cached = s_CachedCreds[key] as SafeCredentialReference;

            if (cached == null || cached.IsClosed || cached._Target.IsInvalid)
            {
                lock (s_CachedCreds)
                {
                    cached = s_CachedCreds[key] as SafeCredentialReference;

                    if (cached == null || cached.IsClosed)
                    {
                        cached = SafeCredentialReference.CreateReference(creds);

                        if (cached == null)
                        {
                            // Means the handle got closed in between, return it back and let caller deal with the issue.
                            return;
                        }

                        s_CachedCreds[key] = cached;
                        GlobalLog.Print("CacheCredential() Caching New Handle = " + creds.ToString() + ", Current Cache Count = " + s_CachedCreds.Count);

                        //
                        // A simplest way of preventing infinite cache grows.
                        //
                        // Security relief (DoS):
                        //     A number of active creds is never greater than a number of _outstanding_
                        //     security sessions, i.e. ssl connections.
                        //     So we will try to shrink cache to the number of active creds once in a while.
                        //
                        //    Just to make clear we won't shrink cache in the case when NO new handles are coming to it.
                        //
                        if ((s_CachedCreds.Count % c_CheckExpiredModulo) == 0)
                        {
                            DictionaryEntry[] toRemoveAttempt = new DictionaryEntry[s_CachedCreds.Count];
                            s_CachedCreds.CopyTo(toRemoveAttempt, 0);

                            for (int i = 0; i < toRemoveAttempt.Length; ++i)
                            {
                                cached = toRemoveAttempt[i].Value as SafeCredentialReference;

                                if (cached != null)
                                {
                                    creds = cached._Target;
                                    cached.Dispose();

                                    if (!creds.IsClosed && !creds.IsInvalid && (cached = SafeCredentialReference.CreateReference(creds)) != null)
                                        s_CachedCreds[toRemoveAttempt[i].Key] = cached;
                                    else
                                        s_CachedCreds.Remove(toRemoveAttempt[i].Key);
                                }
                            }
                            GlobalLog.Print("Scavenged cache, New Cache Count = " + s_CachedCreds.Count);
                        }
                    }
                    else
                    {
                        GlobalLog.Print("CacheCredential() (locked retry) Found already cached Handle = " + cached._Target.ToString());
                    }
                }
            }
            else
            {
                GlobalLog.Print("CacheCredential() Ignoring incoming handle = " + creds.ToString() + " since found already cached Handle = " + cached._Target.ToString());
            }
        }
    }
}
