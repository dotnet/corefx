// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Net
{
    // More sophisticated password cache that stores multiple
    // name-password pairs and associates these with host/realm.
    public class CredentialCache : ICredentials, ICredentialsByHost, IEnumerable
    {
        private Dictionary<CredentialKey, NetworkCredential> _cache;
        private Dictionary<CredentialHostKey, NetworkCredential> _cacheForHosts;
        private int _version;

        public CredentialCache()
        {
        }

        public void Add(Uri uriPrefix, string authenticationType, NetworkCredential credential)
        {
            if (uriPrefix == null)
            {
                throw new ArgumentNullException(nameof(uriPrefix));
            }
            if (authenticationType == null)
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            ++_version;

            var key = new CredentialKey(uriPrefix, authenticationType);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::Add() Adding key:[" + key.ToString() + "], cred:[" + credential.Domain + "],[" + credential.UserName + "]");
            }

            if (_cache == null)
            {
                _cache = new Dictionary<CredentialKey, NetworkCredential>();
            }

            _cache.Add(key, credential);
        }

        public void Add(string host, int port, string authenticationType, NetworkCredential credential)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (authenticationType == null)
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            if (host.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, nameof(host)), nameof(host));
            }

            if (port < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            ++_version;

            var key = new CredentialHostKey(host, port, authenticationType);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::Add() Adding key:[" + key.ToString() + "], cred:[" + credential.Domain + "],[" + credential.UserName + "]");
            }

            if (_cacheForHosts == null)
            {
                _cacheForHosts = new Dictionary<CredentialHostKey, NetworkCredential>();
            }

            _cacheForHosts.Add(key, credential);
        }

        public void Remove(Uri uriPrefix, string authenticationType)
        {
            if (uriPrefix == null || authenticationType == null)
            {
                // These couldn't possibly have been inserted into
                // the cache because of the test in Add().
                return;
            }

            if (_cache == null)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("CredentialCache::Remove() Short-circuiting because the dictionary is null.");
                }

                return;
            }

            ++_version;

            var key = new CredentialKey(uriPrefix, authenticationType);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::Remove() Removing key:[" + key.ToString() + "]");
            }

            _cache.Remove(key);
        }

        public void Remove(string host, int port, string authenticationType)
        {
            if (host == null || authenticationType == null)
            {
                // These couldn't possibly have been inserted into
                // the cache because of the test in Add().
                return;
            }

            if (port < 0)
            {
                return;
            }

            if (_cacheForHosts == null)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("CredentialCache::Remove() Short-circuiting because the dictionary is null.");
                }

                return;
            }

            ++_version;

            var key = new CredentialHostKey(host, port, authenticationType);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::Remove() Removing key:[" + key.ToString() + "]");
            }

            _cacheForHosts.Remove(key);
        }

        public NetworkCredential GetCredential(Uri uriPrefix, string authenticationType)
        {
            if (uriPrefix == null)
            {
                throw new ArgumentNullException(nameof(uriPrefix));
            }
            if (authenticationType == null)
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::GetCredential(uriPrefix=\"" + uriPrefix + "\", authType=\"" + authenticationType + "\")");
            }

            if (_cache == null)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("CredentialCache::GetCredential short-circuiting because the dictionary is null.");
                }

                return null;
            }

            int longestMatchPrefix = -1;
            NetworkCredential mostSpecificMatch = null;

            // Enumerate through every credential in the cache
            foreach (KeyValuePair<CredentialKey, NetworkCredential> pair in _cache)
            {
                CredentialKey key = pair.Key;

                // Determine if this credential is applicable to the current Uri/AuthType
                if (key.Match(uriPrefix, authenticationType))
                {
                    int prefixLen = key.UriPrefixLength;

                    // Check if the match is better than the current-most-specific match
                    if (prefixLen > longestMatchPrefix)
                    {
                        // Yes: update the information about currently preferred match
                        longestMatchPrefix = prefixLen;
                        mostSpecificMatch = pair.Value;
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::GetCredential returning " + ((mostSpecificMatch == null) ? "null" : "(" + mostSpecificMatch.UserName + ":" + mostSpecificMatch.Domain + ")"));
            }

            return mostSpecificMatch;
        }

        public NetworkCredential GetCredential(string host, int port, string authenticationType)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (authenticationType == null)
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }
            if (host.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, nameof(host)), nameof(host));
            }
            if (port < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::GetCredential(host=\"" + host + ":" + port.ToString() + "\", authenticationType=\"" + authenticationType + "\")");
            }

            if (_cacheForHosts == null)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("CredentialCache::GetCredential short-circuiting because the dictionary is null.");
                }

                return null;
            }

            var key = new CredentialHostKey(host, port, authenticationType);

            NetworkCredential match = null;
            _cacheForHosts.TryGetValue(key, out match);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialCache::GetCredential returning " + ((match == null) ? "null" : "(" + match.UserName + ":" + match.Domain + ")"));
            }

            return match;
        }

        public IEnumerator GetEnumerator() => CredentialEnumerator.Create(this);

        public static ICredentials DefaultCredentials => SystemNetworkCredential.s_defaultCredential;

        public static NetworkCredential DefaultNetworkCredentials => SystemNetworkCredential.s_defaultCredential;

        private class CredentialEnumerator : IEnumerator
        {
            internal static CredentialEnumerator Create(CredentialCache cache)
            {
                Debug.Assert(cache != null);

                if (cache._cache != null)
                {
                    return cache._cacheForHosts != null ?
                        new DoubleTableCredentialEnumerator(cache) :
                        new SingleTableCredentialEnumerator<CredentialKey>(cache, cache._cache);
                }
                else
                {
                    return cache._cacheForHosts != null ?
                        new SingleTableCredentialEnumerator<CredentialHostKey>(cache, cache._cacheForHosts) :
                        new CredentialEnumerator(cache);
                }
            }

            private readonly CredentialCache _cache;
            private readonly int _version;
            private bool _enumerating;
            private NetworkCredential _current;

            private CredentialEnumerator(CredentialCache cache)
            {
                Debug.Assert(cache != null);

                _cache = cache;
                _version = cache._version;
            }

            public object Current
            {
                get
                {
                    if (!_enumerating)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    if (_version != _cache._version)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                    }

                    return _current;
                }
            }

            public bool MoveNext()
            {
                if (_version != _cache._version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                return _enumerating = MoveNext(out _current);
            }

            protected virtual bool MoveNext(out NetworkCredential current)
            {
                current = null;
                return false;
            }

            public virtual void Reset()
            {
                _enumerating = false;
            }

            private class SingleTableCredentialEnumerator<TKey> : CredentialEnumerator
            {
                private Dictionary<TKey, NetworkCredential>.ValueCollection.Enumerator _enumerator; // mutable struct field deliberately not readonly.

                public SingleTableCredentialEnumerator(CredentialCache cache, Dictionary<TKey, NetworkCredential> table) : base(cache)
                {
                    Debug.Assert(table != null);

                    // Despite the ValueCollection allocation, ValueCollection's enumerator is faster
                    // than Dictionary's enumerator for enumerating the values because it avoids
                    // KeyValuePair copying.
                    _enumerator = table.Values.GetEnumerator();
                }

                protected override bool MoveNext(out NetworkCredential current) =>
                    DictionaryEnumeratorHelper.MoveNext(ref _enumerator, out current);

                public override void Reset()
                {
                    DictionaryEnumeratorHelper.Reset(ref _enumerator);
                    base.Reset();
                }
            }

            private sealed class DoubleTableCredentialEnumerator : SingleTableCredentialEnumerator<CredentialKey>
            {
                private Dictionary<CredentialHostKey, NetworkCredential>.ValueCollection.Enumerator _enumerator; // mutable struct field deliberately not readonly.
                private bool _onThisEnumerator;

                public DoubleTableCredentialEnumerator(CredentialCache cache) : base(cache, cache._cache)
                {
                    Debug.Assert(cache._cacheForHosts != null);

                    // Despite the ValueCollection allocation, ValueCollection's enumerator is faster
                    // than Dictionary's enumerator for enumerating the values because it avoids
                    // KeyValuePair copying.
                    _enumerator = cache._cacheForHosts.Values.GetEnumerator();
                }

                protected override bool MoveNext(out NetworkCredential current)
                {
                    if (!_onThisEnumerator)
                    {
                        if (base.MoveNext(out current))
                        {
                            return true;
                        }
                        else
                        {
                            _onThisEnumerator = true;
                        }
                    }

                    return DictionaryEnumeratorHelper.MoveNext(ref _enumerator, out current);
                }

                public override void Reset()
                {
                    _onThisEnumerator = false;
                    DictionaryEnumeratorHelper.Reset(ref _enumerator);
                    base.Reset();
                }
            }

            private static class DictionaryEnumeratorHelper
            {
                internal static bool MoveNext<TKey, TValue>(ref Dictionary<TKey, TValue>.ValueCollection.Enumerator enumerator, out TValue current)
                {
                    bool result = enumerator.MoveNext();
                    current = enumerator.Current;
                    return result;
                }

                // Allows calling Reset on Dictionary's struct enumerator without a box allocation.
                internal static void Reset<TEnumerator>(ref TEnumerator enumerator) where TEnumerator : IEnumerator
                {
                    // The Dictionary enumerator's Reset method throws if the Dictionary has changed, but
                    // CredentialCache.Reset should not throw, so we catch and swallow the exception.
                    try { enumerator.Reset(); } catch (InvalidOperationException) { }
                }
            }
        }
    }

    // Abstraction for credentials in password-based
    // authentication schemes (basic, digest, NTLM, Kerberos).
    //
    // Note that this is not applicable to public-key based
    // systems such as SSL client authentication.
    //
    // "Password" here may be the clear text password or it
    // could be a one-way hash that is sufficient to
    // authenticate, as in HTTP/1.1 digest.
    internal sealed class SystemNetworkCredential : NetworkCredential
    {
        internal static readonly SystemNetworkCredential s_defaultCredential = new SystemNetworkCredential();

        // We want reference equality to work. Making this private is a good way to guarantee that.
        private SystemNetworkCredential() :
            base(string.Empty, string.Empty, string.Empty)
        {
        }
    }

    internal struct CredentialHostKey : IEquatable<CredentialHostKey>
    {
        public readonly string Host;
        public readonly string AuthenticationType;
        public readonly int Port;

        internal CredentialHostKey(string host, int port, string authenticationType)
        {
            Debug.Assert(!string.IsNullOrEmpty(host));
            Debug.Assert(port >= 0);
            Debug.Assert(authenticationType != null);

            Host = host;
            Port = port;
            AuthenticationType = authenticationType;
        }

        public override int GetHashCode() =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(AuthenticationType) ^
            StringComparer.OrdinalIgnoreCase.GetHashCode(Host) ^
            Port.GetHashCode();

        public bool Equals(CredentialHostKey other)
        {
            bool equals =
                string.Equals(AuthenticationType, other.AuthenticationType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase) &&
                Port == other.Port;

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialHostKey::Equals(" + ToString() + ", " + other.ToString() + ") returns " + equals.ToString());
            }

            return equals;
        }

        public override bool Equals(object obj) =>
            obj is CredentialHostKey && Equals((CredentialHostKey)obj);

        public override string ToString() =>
            Host + ":" + Port.ToString(NumberFormatInfo.InvariantInfo) + ":" + LoggingHash.ObjectToString(AuthenticationType);
    }

    internal sealed class CredentialKey : IEquatable<CredentialKey>
    {
        public readonly Uri UriPrefix;
        public readonly int UriPrefixLength = -1;
        public readonly string AuthenticationType;

        internal CredentialKey(Uri uriPrefix, string authenticationType)
        {
            Debug.Assert(uriPrefix != null);
            Debug.Assert(authenticationType != null);

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

            // If the protocols don't match, this credential is not applicable for the given Uri.
            if (!string.Equals(authenticationType, AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialKey::Match(" + UriPrefix.ToString() + " & " + uri.ToString() + ")");
            }

            return IsPrefix(uri, UriPrefix);
        }

        // IsPrefix (Uri)
        //
        // Determines whether <prefixUri> is a prefix of this URI. A prefix
        // match is defined as:
        //
        //     scheme match
        //     + host match
        //     + port match, if any
        //     + <prefix> path is a prefix of <URI> path, if any
        //
        // Returns:
        // True if <prefixUri> is a prefix of this URI
        private static bool IsPrefix(Uri uri, Uri prefixUri)
        {
            Debug.Assert(uri != null);
            Debug.Assert(prefixUri != null);

            if (prefixUri.Scheme != uri.Scheme || prefixUri.Host != uri.Host || prefixUri.Port != uri.Port)
            {
                return false;
            }

            int prefixLen = prefixUri.AbsolutePath.LastIndexOf('/');
            if (prefixLen > uri.AbsolutePath.LastIndexOf('/'))
            {
                return false;
            }

            return string.Compare(uri.AbsolutePath, 0, prefixUri.AbsolutePath, 0, prefixLen, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override int GetHashCode() =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(AuthenticationType) ^
            UriPrefix.GetHashCode();

        public bool Equals(CredentialKey other)
        {
            if (other == null)
            {
                return false;
            }

            bool equals =
                string.Equals(AuthenticationType, other.AuthenticationType, StringComparison.OrdinalIgnoreCase) &&
                UriPrefix.Equals(other.UriPrefix);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("CredentialKey::Equals(" + ToString() + ", " + other.ToString() + ") returns " + equals.ToString());
            }
            return equals;
        }

        public override bool Equals(object obj) => Equals(obj as CredentialKey);

        public override string ToString() =>
            "[" + UriPrefixLength.ToString(NumberFormatInfo.InvariantInfo) + "]:" + LoggingHash.ObjectToString(UriPrefix) + ":" + LoggingHash.ObjectToString(AuthenticationType);
    }
}
