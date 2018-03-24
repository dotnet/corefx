// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using SysDebug = System.Diagnostics.Debug;  // as Regex.Debug
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Text.RegularExpressions
{
    public partial class Regex
    {
        // the cache of code and factories that are currently loaded:
        // Dictionary for large cache
        internal static readonly Dictionary<CachedCodeEntryKey, CachedCodeEntry> s_livecode = new Dictionary<CachedCodeEntryKey, CachedCodeEntry>(s_cacheSize);
         // linked list for MRU and for small cache
        internal static int s_livecode_count = 0;
        private const int CacheDictionarySwitchLimit = 10;
        internal static CachedCodeEntry s_livecode_first = null;
        internal static CachedCodeEntry s_livecode_last = null;
        internal static int s_cacheSize = 15;

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
                    if (s_cacheSize < CacheDictionarySwitchLimit)
                        s_livecode.Clear();
                    while (s_livecode_count > s_cacheSize)
                    {
                        CachedCodeEntry last = s_livecode_last;
                        if (s_livecode_count >= CacheDictionarySwitchLimit)
                        {
                            s_livecode.Remove(last.Key);
                        }

                        // update linked list:
                        s_livecode_last = last.Next;
                        if (last.Next != null)
                        {
                            SysDebug.Assert(s_livecode_first != null);
                            SysDebug.Assert(s_livecode_first != last);
                            SysDebug.Assert(last.Next.Previous == last);
                            last.Next.Previous = null;
                        }
                        else // last one removed
                        {
                            SysDebug.Assert(s_livecode_first == last);
                            s_livecode_first = null;
                        }

                        s_livecode_count--;
                    }
                }
            }
        }

        /// <summary>
        ///  Find cache based on options+pattern+culture and optionally add new cache if not found
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CachedCodeEntry GetCachedCode(CachedCodeEntryKey key, bool isToAdd)
        {
            // to avoid lock:
            CachedCodeEntry first = s_livecode_first;
            if (first?.Key == key)
                return first;
            if (s_cacheSize == 0)
                return null;

            return GetCachedCodeEntryInternal(key, isToAdd);
        }

        private CachedCodeEntry GetCachedCodeEntryInternal(CachedCodeEntryKey key, bool isToAdd)
        {
            lock (s_livecode)
            {
                // first look for it in the cache and move it to the head
                CachedCodeEntry entry = LookupCachedAndPromote(key);
                // it wasn't in the cache, so we'll add a new one
                if (entry == null && isToAdd && s_cacheSize != 0) // check cache size again in case it changed
                {
                    entry = new CachedCodeEntry(key, capnames, capslist, _code, caps, capsize, _runnerref, _replref);
                    // put first in linked list:
                    if (s_livecode_first != null)
                    {
                        SysDebug.Assert(s_livecode_first.Next == null);
                        s_livecode_first.Next = entry;
                        entry.Previous = s_livecode_first;
                    }
                    s_livecode_first = entry;

                    s_livecode_count++;
                    if (s_livecode_count >= CacheDictionarySwitchLimit)
                    {
                        if (s_livecode_count == CacheDictionarySwitchLimit)
                            FillCacheDictionary();
                        else
                            s_livecode.Add(key, entry);
                        SysDebug.Assert(s_livecode_count == s_livecode.Count);
                    }

                    // update last in linked list:
                    if (s_livecode_last == null)
                    {
                        s_livecode_last = entry;
                    }
                    else if (s_livecode_count > s_cacheSize) // remove last
                    {
                        CachedCodeEntry last = s_livecode_last;
                        if (s_livecode_count >= CacheDictionarySwitchLimit)
                        {
                            SysDebug.Assert(s_livecode[last.Key] == s_livecode_last);
                            s_livecode.Remove(last.Key);
                        }

                        SysDebug.Assert(last.Previous == null);
                        SysDebug.Assert(last.Next != null);
                        SysDebug.Assert(last.Next.Previous == last);
                        last.Next.Previous = null;
                        s_livecode_last = last.Next;
                        s_livecode_count--;
                    }

                }
                return entry;
            }
        }

        private void FillCacheDictionary()
        {
            s_livecode.Clear();
            CachedCodeEntry next = s_livecode_first;
            while (next != null)
            {
                s_livecode.Add(next.Key, next);
                next = next.Previous;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetCacheValue(CachedCodeEntryKey key, out CachedCodeEntry entry)
        {
            if (s_livecode_count >= CacheDictionarySwitchLimit)
            {
                SysDebug.Assert((s_livecode_first != null && s_livecode_last != null && s_livecode.Count > 0) || 
                                (s_livecode_first == null && s_livecode_last == null && s_livecode.Count == 0), 
                                "Linked list and Dict should be synchronized");
                return s_livecode.TryGetValue(key, out entry);
            }
            entry = s_livecode_first?.Previous; // first already checked
            while (entry != null)
            {
                if (entry.Key == key)
                    return true;
                entry = entry.Previous;
            }

            return false;
        }

        private static CachedCodeEntry LookupCachedAndPromote(CachedCodeEntryKey key)
        {
            SysDebug.Assert(Monitor.IsEntered(s_livecode));
            if (s_livecode_first?.Key == key) // again check this as could have been promoted by other thread
                return s_livecode_first;
            
            if (TryGetCacheValue(key, out var entry))
            {
                SysDebug.Assert(s_livecode_first != entry, "key should not get s_livecode_first");
                SysDebug.Assert(s_livecode_first != null, "as Dict has at least one");
                SysDebug.Assert(s_livecode_first.Next == null);
                SysDebug.Assert(s_livecode_first.Previous != null);
                SysDebug.Assert(entry.Next != null, "not first so Next should exist");
                SysDebug.Assert(entry.Next.Previous == entry);
                if (s_livecode_last == entry)
                {
                    SysDebug.Assert(entry.Previous == null, "last");
                    s_livecode_last = entry.Next;
                }
                else
                {
                    SysDebug.Assert(entry.Previous != null, "in middle");
                    SysDebug.Assert(entry.Previous.Next == entry);
                    entry.Previous.Next = entry.Next;
                }
                entry.Next.Previous = entry.Previous;

                s_livecode_first.Next = entry;
                entry.Previous = s_livecode_first;
                entry.Next = null;
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

            public CachedCodeEntryKey(RegexOptions options, string cultureKey, string pattern)
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
            public CachedCodeEntry Next;
            public CachedCodeEntry Previous;
            public readonly CachedCodeEntryKey Key;
            public RegexCode Code;
            public readonly Hashtable Caps;
            public readonly Hashtable Capnames;
            public readonly string[] Capslist;
#if FEATURE_COMPILED
            public RegexRunnerFactory Factory;
#endif
            public readonly int Capsize;
            public readonly ExclusiveReference Runnerref;
            public readonly WeakReference<RegexReplacement> ReplRef;

            public CachedCodeEntry(CachedCodeEntryKey key, Hashtable capnames, string[] capslist, RegexCode code, 
                Hashtable caps, int capsize, ExclusiveReference runner, WeakReference<RegexReplacement> replref)
            {
                Key = key;
                Capnames = capnames;
                Capslist = capslist;

                Code = code;
                Caps = caps;
                Capsize = capsize;

                Runnerref = runner;
                ReplRef = replref;
            }

#if FEATURE_COMPILED
            public void AddCompiled(RegexRunnerFactory factory)
            {
                Factory = factory;
                Code = null;
            }
#endif
        }
    }
}
