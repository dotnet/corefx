// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// The MatchCollection lists the successful matches that
// result when searching a string for a regular expression.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.RegularExpressions
{
    /*
     * This collection returns a sequence of successful match results, either
     * from GetMatchCollection() or GetExecuteCollection(). It stops when the
     * first failure is encountered (it does not return the failed match).
     */
    /// <summary>
    /// Represents the set of names appearing as capturing group
    /// names in a regular expression.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(RegexCollectionDebuggerProxy<>))]
    public class MatchCollection : IList<Match>, IReadOnlyList<Match>, IList
    {
        internal Regex _regex;
        internal List<Match> _matches;
        internal bool _done;
        internal String _input;
        internal int _beginning;
        internal int _length;
        internal int _startat;
        internal int _prevlen;

        private static int s_infinite = 0x7FFFFFFF;

        internal MatchCollection(Regex regex, String input, int beginning, int length, int startat)
        {
            if (startat < 0 || startat > input.Length)
                throw new ArgumentOutOfRangeException("startat", SR.BeginIndexNotNegative);

            _regex = regex;
            _input = input;
            _beginning = beginning;
            _length = length;
            _startat = startat;
            _prevlen = -1;
            _matches = new List<Match>();
            _done = false;
        }

        internal Match GetMatch(int i)
        {
            if (i < 0)
                return null;

            if (_matches.Count > i)
                return (Match)_matches[i];

            if (_done)
                return null;

            Match match;

            do
            {
                match = _regex.Run(false, _prevlen, _input, _beginning, _length, _startat);

                if (!match.Success)
                {
                    _done = true;
                    return null;
                }

                _matches.Add(match);

                _prevlen = match._length;
                _startat = match._textpos;
            } while (_matches.Count <= i);

            return match;
        }

        private void EnsureInitialized()
        {
            if (!_done)
            {
                GetMatch(s_infinite);
            }
        }

        /// <summary>
        /// Returns the number of captures.
        /// </summary>
        public int Count
        {
            get
            {
                EnsureInitialized();
                return _matches.Count;
            }
        }

        /// <summary>
        /// Returns the ith Match in the collection.
        /// </summary>
        public virtual Match this[int i]
        {
            get
            {
                Match match;

                match = GetMatch(i);

                if (match == null)
                    throw new ArgumentOutOfRangeException("i");

                return match;
            }
        }

        /// <summary>
        /// Copies all the elements of the collection to the given array
        /// starting at the given index.
        /// </summary>
        public void CopyTo(Match[] array, int arrayIndex)
        {
            EnsureInitialized();
            _matches.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Provides an enumerator in the same order as Item[i].
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return new MatchEnumerator(this);
        }

        IEnumerator<Match> IEnumerable<Match>.GetEnumerator()
        {
            return new MatchEnumerator(this);
        }

        int IList<Match>.IndexOf(Match item)
        {
            var comparer = EqualityComparer<Match>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                    return i;
            }
            return -1;
        }

        void IList<Match>.Insert(int index, Match item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void IList<Match>.RemoveAt(int index)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        Match IList<Match>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection); }
        }

        void ICollection<Match>.Add(Match item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void ICollection<Match>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool ICollection<Match>.Contains(Match item)
        {
            var comparer = EqualityComparer<Match>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                    return true;
            }
            return false;
        }

        bool ICollection<Match>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<Match>.Remove(Match item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void IList.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool IList.Contains(object value)
        {
            return value is Match && ((ICollection<Match>)this).Contains((Match)value);
        }

        int IList.IndexOf(object value)
        {
            return value is Match ? ((IList<Match>)this).IndexOf((Match)value) : -1;
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection); }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            EnsureInitialized();
            ((ICollection)_matches).CopyTo(array, arrayIndex);
        }
    }

    /*
     * This non-public enumerator lists all the group matches.
     * Should it be public?
     */
    internal class MatchEnumerator : IEnumerator<Match>
    {
        internal MatchCollection _matchcoll;
        internal Match _match = null;
        internal int _curindex;
        internal bool _done;

        /*
         * Nonpublic constructor
         */
        internal MatchEnumerator(MatchCollection matchcoll)
        {
            _matchcoll = matchcoll;
        }

        /*
         * Advance to the next match
         */
        public bool MoveNext()
        {
            if (_done)
                return false;

            _match = _matchcoll.GetMatch(_curindex);
            _curindex++;

            if (_match == null)
            {
                _done = true;
                return false;
            }

            return true;
        }

        /*
         * The current match
         */
        public Object Current
        {
            get { return Match; }
        }

        Match IEnumerator<Match>.Current
        {
            get { return Match; }
        }

        private Match Match
        {
            get
            {
                if (_match == null)
                    throw new InvalidOperationException(SR.EnumNotStarted);
                return _match;
            }
        }

        /*
         * Position before the first item
         */
        public void Reset()
        {
            _curindex = 0;
            _done = false;
            _match = null;
        }

        void IDisposable.Dispose()
        {
        }
    }
}
