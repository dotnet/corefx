// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// The MatchCollection lists the successful matches that
// result when searching a string for a regular expression.

using System.Collections;
using System.Collections.Generic;

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
    public class MatchCollection : ICollection, IReadOnlyList<Match>
    {
        private Regex _regex;
        private List<Match> _matches;
        private bool _done;
        private string _input;
        private int _beginning;
        private int _length;
        private int _startat;
        private int _prevlen;

        private static int s_infinite = 0x7FFFFFFF;

        internal MatchCollection(Regex regex, string input, int beginning, int length, int startat)
        {
            if (startat < 0 || startat > input.Length)
                throw new ArgumentOutOfRangeException("startat", global::Resources.Strings.BeginIndexNotNegative);

            _regex = regex;
            _input = input;
            _beginning = beginning;
            _length = length;
            _startat = startat;
            _prevlen = -1;
            _matches = new List<Match>();
            _done = false;
        }

        private Match GetMatch(int i)
        {
            if (i < 0)
                return null;

            if (_matches.Count > i)
                return _matches[i];

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

        /// <summary>
        /// Returns the number of captures.
        /// </summary>
        public int Count
        {
            get
            {
                if (_done)
                    return _matches.Count;

                GetMatch(s_infinite);

                return _matches.Count;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
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
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if ((array != null) && (array.Rank != 1))
            {
                throw new ArgumentException(global::Resources.Strings.Arg_RankMultiDimNotSupported);
            }

            // property access to force computation of whole array
            int count = Count;
            try
            {
                // Array.Copy will check for null.
                Array.Copy(_matches.ToArray(), 0, array, arrayIndex, count);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException(global::Resources.Strings.Arg_InvalidArrayType, ex);
            }
        }

        /// <summary>
        /// Provides an enumerator in the same order as Item[i].
        /// </summary>
        public IEnumerator<Match> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return GetMatch(i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
