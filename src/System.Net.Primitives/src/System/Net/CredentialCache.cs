// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Globalization;

namespace System.Net
{
    // More sophisticated password cache that stores multiple
    // name-password pairs and associates these with host/realm
    /// <devdoc>
    ///    <para>Provides storage for multiple credentials.</para>
    /// </devdoc>
    public class CredentialCache : ICredentials, ICredentialsByHost, IEnumerable
    {
        // fields

        private Hashtable cache = new Hashtable();
        private Hashtable cacheForHosts = new Hashtable();
        internal int m_version;

        private int m_NumbDefaultCredInCache = 0;

        // [thread token optimization] The resulting counter of default credential resided in the cache.
        internal bool IsDefaultInCache
        {
            get
            {
                return m_NumbDefaultCredInCache != 0;
            }
        }

        // constructors

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Net.CredentialCache'/> class.
        ///    </para>
        /// </devdoc>
        public CredentialCache()
        {
        }

        // properties

        // methods

        /// <devdoc>
        /// <para>Adds a <see cref='System.Net.NetworkCredential'/>
        /// instance to the credential cache.</para>
        /// </devdoc>
        public void Add(Uri uriPrefix, string authType, NetworkCredential cred)
        {
            //
            // parameter validation
            //
            if (uriPrefix == null)
            {
                throw new ArgumentNullException("uriPrefix");
            }
            if (authType == null)
            {
                throw new ArgumentNullException("authType");
            }

            ++m_version;

            CredentialKey key = new CredentialKey(uriPrefix, authType);

            GlobalLog.Print("CredentialCache::Add() Adding key:[" + key.ToString() + "], cred:[" + cred.Domain + "],[" + cred.UserName + "]");

            cache.Add(key, cred);
            if (cred is SystemNetworkCredential)
            {
                ++m_NumbDefaultCredInCache;
            }
        }


        public void Add(string host, int port, string authenticationType, NetworkCredential credential)
        {
            //
            // parameter validation
            //
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (authenticationType == null)
            {
                throw new ArgumentNullException("authenticationType");
            }

            if (host.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, "host"));
            }

            if (port < 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            ++m_version;

            CredentialHostKey key = new CredentialHostKey(host, port, authenticationType);

            GlobalLog.Print("CredentialCache::Add() Adding key:[" + key.ToString() + "], cred:[" + credential.Domain + "],[" + credential.UserName + "]");

            cacheForHosts.Add(key, credential);
            if (credential is SystemNetworkCredential)
            {
                ++m_NumbDefaultCredInCache;
            }
        }


        /// <devdoc>
        /// <para>Removes a <see cref='System.Net.NetworkCredential'/>
        /// instance from the credential cache.</para>
        /// </devdoc>
        public void Remove(Uri uriPrefix, string authType)
        {
            if (uriPrefix == null || authType == null)
            {
                // these couldn't possibly have been inserted into
                // the cache because of the test in Add()
                return;
            }

            ++m_version;

            CredentialKey key = new CredentialKey(uriPrefix, authType);

            GlobalLog.Print("CredentialCache::Remove() Removing key:[" + key.ToString() + "]");

            if (cache[key] is SystemNetworkCredential)
            {
                --m_NumbDefaultCredInCache;
            }
            cache.Remove(key);
        }


        public void Remove(string host, int port, string authenticationType)
        {
            if (host == null || authenticationType == null)
            {
                // these couldn't possibly have been inserted into
                // the cache because of the test in Add()
                return;
            }

            if (port < 0)
            {
                return;
            }


            ++m_version;

            CredentialHostKey key = new CredentialHostKey(host, port, authenticationType);

            GlobalLog.Print("CredentialCache::Remove() Removing key:[" + key.ToString() + "]");

            if (cacheForHosts[key] is SystemNetworkCredential)
            {
                --m_NumbDefaultCredInCache;
            }
            cacheForHosts.Remove(key);
        }

        /// <devdoc>
        ///    <para>
        ///       Returns the <see cref='System.Net.NetworkCredential'/>
        ///       instance associated with the supplied Uri and
        ///       authentication type.
        ///    </para>
        /// </devdoc>
        public NetworkCredential GetCredential(Uri uriPrefix, string authType)
        {
            if (uriPrefix == null)
                throw new ArgumentNullException("uriPrefix");
            if (authType == null)
                throw new ArgumentNullException("authType");

            GlobalLog.Print("CredentialCache::GetCredential(uriPrefix=\"" + uriPrefix + "\", authType=\"" + authType + "\")");

            int longestMatchPrefix = -1;
            NetworkCredential mostSpecificMatch = null;
            IDictionaryEnumerator credEnum = cache.GetEnumerator();

            //
            // Enumerate through every credential in the cache
            //

            while (credEnum.MoveNext())
            {
                CredentialKey key = (CredentialKey)credEnum.Key;

                //
                // Determine if this credential is applicable to the current Uri/AuthType
                //

                if (key.Match(uriPrefix, authType))
                {
                    int prefixLen = key.UriPrefixLength;

                    //
                    // Check if the match is better than the current-most-specific match
                    //

                    if (prefixLen > longestMatchPrefix)
                    {
                        //
                        // Yes-- update the information about currently preferred match
                        //

                        longestMatchPrefix = prefixLen;
                        mostSpecificMatch = (NetworkCredential)credEnum.Value;
                    }
                }
            }

            GlobalLog.Print("CredentialCache::GetCredential returning " + ((mostSpecificMatch == null) ? "null" : "(" + mostSpecificMatch.UserName + ":" + mostSpecificMatch.Domain + ")"));

            return mostSpecificMatch;
        }


        public NetworkCredential GetCredential(string host, int port, string authenticationType)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if (authenticationType == null)
            {
                throw new ArgumentNullException("authenticationType");
            }
            if (host.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, "host"));
            }
            if (port < 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }


            GlobalLog.Print("CredentialCache::GetCredential(host=\"" + host + ":" + port.ToString() + "\", authenticationType=\"" + authenticationType + "\")");

            NetworkCredential match = null;

            IDictionaryEnumerator credEnum = cacheForHosts.GetEnumerator();

            //
            // Enumerate through every credential in the cache
            //

            while (credEnum.MoveNext())
            {
                CredentialHostKey key = (CredentialHostKey)credEnum.Key;

                //
                // Determine if this credential is applicable to the current Uri/AuthType
                //

                if (key.Match(host, port, authenticationType))
                {
                    match = (NetworkCredential)credEnum.Value;
                }
            }

            GlobalLog.Print("CredentialCache::GetCredential returning " + ((match == null) ? "null" : "(" + match.UserName + ":" + match.Domain + ")"));
            return match;
        }



        /// <devdoc>
        ///    [To be supplied]
        /// </devdoc>

        //
        // IEnumerable interface
        //

        public IEnumerator GetEnumerator()
        {
            return new CredentialEnumerator(this, cache, cacheForHosts, m_version);
        }


        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the default system credentials from the <see cref='System.Net.CredentialCache'/>.
        ///    </para>
        /// </devdoc>
        public static ICredentials DefaultCredentials
        {
            get
            {
                return SystemNetworkCredential.defaultCredential;
            }
        }

        public static NetworkCredential DefaultNetworkCredentials
        {
            get
            {
                return SystemNetworkCredential.defaultCredential;
            }
        }

        private class CredentialEnumerator : IEnumerator
        {
            // fields

            private CredentialCache m_cache;
            private ICredentials[] m_array;
            private int m_index = -1;
            private int m_version;

            // constructors

            internal CredentialEnumerator(CredentialCache cache, Hashtable table, Hashtable hostTable, int version)
            {
                m_cache = cache;
                m_array = new ICredentials[table.Count + hostTable.Count];
                table.Values.CopyTo(m_array, 0);
                hostTable.Values.CopyTo(m_array, table.Count);
                m_version = version;
            }

            // IEnumerator interface

            // properties

            object IEnumerator.Current
            {
                get
                {
                    if (m_index < 0 || m_index >= m_array.Length)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    if (m_version != m_cache.m_version)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                    }
                    return m_array[m_index];
                }
            }

            // methods

            bool IEnumerator.MoveNext()
            {
                if (m_version != m_cache.m_version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                if (++m_index < m_array.Length)
                {
                    return true;
                }
                m_index = m_array.Length;
                return false;
            }

            void IEnumerator.Reset()
            {
                m_index = -1;
            }
        } // class CredentialEnumerator
    } // class CredentialCache



    // Abstraction for credentials in password-based
    // authentication schemes (basic, digest, NTLM, Kerberos)
    // Note this is not applicable to public-key based
    // systems such as SSL client authentication
    // "Password" here may be the clear text password or it
    // could be a one-way hash that is sufficient to
    // authenticate, as in HTTP/1.1 digest.

    //
    // Object representing default credentials
    //
    internal class SystemNetworkCredential : NetworkCredential
    {
        internal static readonly SystemNetworkCredential defaultCredential = new SystemNetworkCredential();

        // We want reference equality to work.  Making this private is a good way to guarantee that.
        private SystemNetworkCredential() :
            base(string.Empty, string.Empty, string.Empty)
        {
        }
    }


    internal class CredentialHostKey
    {
        internal string Host;
        internal string AuthenticationType;
        internal int Port;

        internal CredentialHostKey(string host, int port, string authenticationType)
        {
            Host = host;
            Port = port;
            AuthenticationType = authenticationType;
        }

        internal bool Match(string host, int port, string authenticationType)
        {
            if (host == null || authenticationType == null)
            {
                return false;
            }
            //
            // If the protocols dont match this credential
            // is not applicable for the given Uri
            //
            if (string.Compare(authenticationType, AuthenticationType, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }
            if (string.Compare(Host, host, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }
            if (port != Port)
            {
                return false;
            }

            GlobalLog.Print("CredentialKey::Match(" + Host.ToString() + ":" + Port.ToString() + " & " + host.ToString() + ":" + port.ToString() + ")");
            return true;
        }


        private int m_HashCode = 0;
        private bool m_ComputedHashCode = false;
        public override int GetHashCode()
        {
            if (!m_ComputedHashCode)
            {
                //
                // compute HashCode on demand
                //

                m_HashCode = AuthenticationType.ToUpperInvariant().GetHashCode() + Host.ToUpperInvariant().GetHashCode() + Port.GetHashCode();
                m_ComputedHashCode = true;
            }
            return m_HashCode;
        }

        public override bool Equals(object comparand)
        {
            CredentialHostKey comparedCredentialKey = comparand as CredentialHostKey;

            if (comparand == null)
            {
                //
                // this covers also the compared==null case
                //
                return false;
            }

            bool equals =
                (string.Compare(AuthenticationType, comparedCredentialKey.AuthenticationType, StringComparison.OrdinalIgnoreCase) == 0) &&
                (string.Compare(Host, comparedCredentialKey.Host, StringComparison.OrdinalIgnoreCase) == 0) &&
                Port == comparedCredentialKey.Port;

            GlobalLog.Print("CredentialKey::Equals(" + ToString() + ", " + comparedCredentialKey.ToString() + ") returns " + equals.ToString());

            return equals;
        }

        public override string ToString()
        {
            return "[" + Host.Length.ToString(NumberFormatInfo.InvariantInfo) + "]:" + Host + ":" + Port.ToString(NumberFormatInfo.InvariantInfo) + ":" + Logging.ObjectToString(AuthenticationType);
        }
    } // class CredentialKey


    internal class CredentialKey
    {
        internal Uri UriPrefix;
        internal int UriPrefixLength = -1;
        internal string AuthenticationType;

        internal CredentialKey(Uri uriPrefix, string authenticationType)
        {
            UriPrefix = uriPrefix;
            UriPrefixLength = UriPrefix.ToString().Length;
            AuthenticationType = authenticationType;
        }

        internal bool Match(Uri uri, string authenticationType)
        {
            if (uri == null || authenticationType == null)
            {
                return false;
            }
            //
            // If the protocols dont match this credential
            // is not applicable for the given Uri
            //
            if (string.Compare(authenticationType, AuthenticationType, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            GlobalLog.Print("CredentialKey::Match(" + UriPrefix.ToString() + " & " + uri.ToString() + ")");

            return IsPrefix(uri, UriPrefix);
        }
        //
        // IsPrefix (Uri)
        //
        //  Determines whether <prefixUri> is a prefix of this URI. A prefix
        //  match is defined as:
        //
        //      scheme match
        //      + host match
        //      + port match, if any
        //      + <prefix> path is a prefix of <URI> path, if any
        //
        // Returns:
        //  True if <prefixUri> is a prefix of this URI
        //
        internal bool IsPrefix(Uri uri, Uri prefixUri)
        {
            if (prefixUri.Scheme != uri.Scheme || prefixUri.Host != uri.Host || prefixUri.Port != uri.Port)
                return false;

            int prefixLen = prefixUri.AbsolutePath.LastIndexOf('/');
            if (prefixLen > uri.AbsolutePath.LastIndexOf('/'))
                return false;

            return String.Compare(uri.AbsolutePath, 0, prefixUri.AbsolutePath, 0, prefixLen, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private int m_HashCode = 0;
        private bool m_ComputedHashCode = false;
        public override int GetHashCode()
        {
            if (!m_ComputedHashCode)
            {
                //
                // compute HashCode on demand
                //

                m_HashCode = AuthenticationType.ToUpperInvariant().GetHashCode() + UriPrefixLength + UriPrefix.GetHashCode();
                m_ComputedHashCode = true;
            }
            return m_HashCode;
        }

        public override bool Equals(object comparand)
        {
            CredentialKey comparedCredentialKey = comparand as CredentialKey;

            if (comparand == null)
            {
                //
                // this covers also the compared==null case
                //
                return false;
            }

            bool equals =
                (string.Compare(AuthenticationType, comparedCredentialKey.AuthenticationType, StringComparison.OrdinalIgnoreCase) == 0) &&
                UriPrefix.Equals(comparedCredentialKey.UriPrefix);

            GlobalLog.Print("CredentialKey::Equals(" + ToString() + ", " + comparedCredentialKey.ToString() + ") returns " + equals.ToString());

            return equals;
        }

        public override string ToString()
        {
            return "[" + UriPrefixLength.ToString(NumberFormatInfo.InvariantInfo) + "]:" + Logging.ObjectToString(UriPrefix) + ":" + Logging.ObjectToString(AuthenticationType);
        }
    } // class CredentialKey
} // namespace System.Net
