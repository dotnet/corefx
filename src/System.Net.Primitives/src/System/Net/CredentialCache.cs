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

        private Hashtable _cache = new Hashtable();
        private Hashtable _cacheForHosts = new Hashtable();
        internal int m_version;

        private int _numbDefaultCredInCache = 0;

        // [thread token optimization] The resulting counter of default credential resided in the cache.
        internal bool IsDefaultInCache
        {
            get
            {
                return _numbDefaultCredInCache != 0;
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

            _cache.Add(key, cred);
            if (cred is SystemNetworkCredential)
            {
                ++_numbDefaultCredInCache;
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

            _cacheForHosts.Add(key, credential);
            if (credential is SystemNetworkCredential)
            {
                ++_numbDefaultCredInCache;
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

            if (_cache[key] is SystemNetworkCredential)
            {
                --_numbDefaultCredInCache;
            }
            _cache.Remove(key);
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

            if (_cacheForHosts[key] is SystemNetworkCredential)
            {
                --_numbDefaultCredInCache;
            }
            _cacheForHosts.Remove(key);
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
            IDictionaryEnumerator credEnum = _cache.GetEnumerator();

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

            IDictionaryEnumerator credEnum = _cacheForHosts.GetEnumerator();

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
            return new CredentialEnumerator(this, _cache, _cacheForHosts, m_version);
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

            private CredentialCache _cache;
            private ICredentials[] _array;
            private int _index = -1;
            private int _version;

            // constructors

            internal CredentialEnumerator(CredentialCache cache, Hashtable table, Hashtable hostTable, int version)
            {
                _cache = cache;
                _array = new ICredentials[table.Count + hostTable.Count];
                table.Values.CopyTo(_array, 0);
                hostTable.Values.CopyTo(_array, table.Count);
                _version = version;
            }

            // IEnumerator interface

            // properties

            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0 || _index >= _array.Length)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    if (_version != _cache.m_version)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                    }
                    return _array[_index];
                }
            }

            // methods

            bool IEnumerator.MoveNext()
            {
                if (_version != _cache.m_version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                if (++_index < _array.Length)
                {
                    return true;
                }
                _index = _array.Length;
                return false;
            }

            void IEnumerator.Reset()
            {
                _index = -1;
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


        private int _hashCode = 0;
        private bool _computedHashCode = false;
        public override int GetHashCode()
        {
            if (!_computedHashCode)
            {
                //
                // compute HashCode on demand
                //

                _hashCode = AuthenticationType.ToUpperInvariant().GetHashCode() + Host.ToUpperInvariant().GetHashCode() + Port.GetHashCode();
                _computedHashCode = true;
            }
            return _hashCode;
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

        private int _hashCode = 0;
        private bool _computedHashCode = false;
        public override int GetHashCode()
        {
            if (!_computedHashCode)
            {
                //
                // compute HashCode on demand
                //

                _hashCode = AuthenticationType.ToUpperInvariant().GetHashCode() + UriPrefixLength + UriPrefix.GetHashCode();
                _computedHashCode = true;
            }
            return _hashCode;
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
