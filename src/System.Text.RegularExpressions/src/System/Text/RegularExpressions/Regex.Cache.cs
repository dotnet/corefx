﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Text.RegularExpressions
{
    public partial class Regex
    {
        // the cache of code and factories that are currently loaded:
        internal static Dictionary<CachedCodeEntryKey, CachedCodeEntry> s_livecode = new Dictionary<CachedCodeEntryKey, CachedCodeEntry>(s_cacheSize);
        internal static CachedCodeEntry s_livecode_first = null;
        internal static CachedCodeEntry s_livecode_last = null;
        internal static int s_cacheSize = 15;

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "Reviewed for thread-safety")]
        public static int CacheSize
        {
            get
            {
                return s_cacheSize;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                lock (s_livecode)
                {
                    s_cacheSize = value;  // not to allow other thread to change it while we use cache
                    while (s_livecode.Count > s_cacheSize)
                    {
                        CachedCodeEntry last = s_livecode_last;
                        s_livecode.Remove(last._key);

                        // update linked list:
                        s_livecode_last = last._next;
                        if (last._next != null)
                            last._next._previous = null;
                        else  // last one removed
                            s_livecode_first = null;
                    }
                }
            }
        }

        /// <summary>
        ///  Find cache based on options+pattern+culture and optionally add new cache if not found
        /// </summary>
        private CachedCodeEntry GetCachedCode(CachedCodeEntryKey key, bool isToAdd)
        {
            if (s_cacheSize == 0)
                return null;
            lock (s_livecode)
            {
                // first look for it in the cache and move it to the head
                var entry = LookupCachedAndPromote(key);
                // it wasn't in the cache, so we'll add a new one.  Shortcut out for the case where cacheSize is zero.
                if (entry == null && isToAdd && s_cacheSize != 0)  // check cache size again in case it changed
                {
                    entry = new CachedCodeEntry(key, capnames, capslist, _code, caps, capsize, _runnerref, _replref);
                    s_livecode.Add(key, entry);
                    // put in linked list:
                    if (s_livecode_first != null)
                    {
                        s_livecode_first._next = entry;
                        entry._previous = s_livecode_first;
                    }
                    s_livecode_first = entry;
                    if (s_livecode_last == null)
                    {
                        s_livecode_last = entry;
                    }
                    else
                    {
                        if (s_livecode.Count > s_cacheSize)
                        {
                            var last = s_livecode_last;
                            s_livecode.Remove(last._key);

                            last._next._previous = null;
                            s_livecode_last = last._next;
                            last._next = null;
                        }
                    }
                }
                return entry;
            }
        }

        private static CachedCodeEntry LookupCachedAndPromote(CachedCodeEntryKey key)
        {
            if (s_livecode_first?._key == key) // most used regex should be at the top already
                return s_livecode_first;
            s_livecode.TryGetValue(key, out var entry);
            if (entry != null)
            {
                if (entry._previous != null)
                {
                    entry._previous._next = entry._next;
                    entry._next._previous = entry._previous;  // not first so should exist _next
                }

                if (s_livecode_last == entry)
                {
                    s_livecode_last = entry._next;
                }

                s_livecode_first._next = entry;
                entry._previous = s_livecode_first;
                entry._next = null;
                s_livecode_first = entry;
            }
            return entry;
        }

        /// <summary>
        /// Used as a key for CacheCodeEntry
        /// </summary>
		internal readonly struct CachedCodeEntryKey : IEquatable<CachedCodeEntryKey>
		{
			private readonly RegexOptions _options;
			private readonly string _cultureKey;
			private readonly string _pattern;

			internal CachedCodeEntryKey(RegexOptions options, string cultureKey, string pattern)
			{
				_options = options;
				_cultureKey = cultureKey;
				_pattern = pattern;
			}

			public override bool Equals(object obj)
			{
				return obj is CachedCodeEntryKey && Equals((CachedCodeEntryKey)obj);
			}

			public bool Equals(CachedCodeEntryKey other)
			{
				return _pattern.Equals(other._pattern) && _options == other._options && _cultureKey.Equals(other._cultureKey);
			}

			public static bool operator ==(CachedCodeEntryKey left, CachedCodeEntryKey right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(CachedCodeEntryKey left, CachedCodeEntryKey right)
			{
				return !left.Equals(right);
			}

			public override int GetHashCode()
			{
				return ((int)_options) ^ _cultureKey.GetHashCode() ^ _pattern.GetHashCode();
			}
		}

        /// <summary>
        /// Used to cache byte codes
        /// </summary>
        internal sealed class CachedCodeEntry
        {
			public CachedCodeEntry _next = null;
			public CachedCodeEntry _previous = null;
            public CachedCodeEntryKey _key;
            public RegexCode _code;
            public Hashtable _caps;
            public Hashtable _capnames;
            public string[] _capslist;
#if FEATURE_COMPILED
            public RegexRunnerFactory _factory;
#endif
            public int _capsize;
            public ExclusiveReference _runnerref;
            public SharedReference _replref;

            public CachedCodeEntry(CachedCodeEntryKey key, Hashtable capnames, string[] capslist, RegexCode code, 
                Hashtable caps, int capsize, ExclusiveReference runner, SharedReference repl)
            {
                _key = key;
                _capnames = capnames;
                _capslist = capslist;

                _code = code;
                _caps = caps;
                _capsize = capsize;

                _runnerref = runner;
                _replref = repl;
            }

#if FEATURE_COMPILED
            public void AddCompiled(RegexRunnerFactory factory)
            {
                _factory = factory;
                _code = null;
            }
#endif
        }
    }
}
