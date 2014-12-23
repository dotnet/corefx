// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// The GroupCollection lists the captured Capture numbers
// contained in a compiled Regex.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.RegularExpressions
{
    /// <summary>
    /// Represents a sequence of capture substrings. The object is used
    /// to return the set of captures done by a single capturing group.
    /// </summary>
    public class GroupCollection : ICollection, IReadOnlyList<Group>
    {
        private Match _match;
        private Dictionary<int, int> _captureMap;

        // cache of Group objects fed to the user
        private Group[] _groups;

        /*
         * Nonpublic constructor
         */
        internal GroupCollection(Match match, Dictionary<int, int> caps)
        {
            _match = match;
            _captureMap = caps;
        }

        /// <summary>
        /// The object on which to synchronize
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return _match;
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
        /// Returns the number of groups.
        /// </summary>
        public int Count
        {
            get
            {
                return _match._matchcount.Length;
            }
        }

        public Group this[int groupnum]
        {
            get
            {
                return GetGroup(groupnum);
            }
        }

        public Group this[string groupname]
        {
            get
            {
                if (_match._regex == null)
                    return Group._emptygroup;

                return GetGroup(_match._regex.GroupNumberFromName(groupname));
            }
        }

        private Group GetGroup(int groupnum)
        {
            if (_captureMap != null)
            {
                Object o;

                o = _captureMap[groupnum];
                if (o == null)
                    return Group._emptygroup;

                return GetGroupImpl((int)o);
            }
            else
            {
                if (groupnum >= _match._matchcount.Length || groupnum < 0)
                    return Group._emptygroup;

                return GetGroupImpl(groupnum);
            }
        }


        /*
         * Caches the group objects
         */
        private Group GetGroupImpl(int groupnum)
        {
            if (groupnum == 0)
                return _match;

            // Construct all the Group objects the first time GetGroup is called

            if (_groups == null)
            {
                _groups = new Group[_match._matchcount.Length - 1];
                for (int i = 0; i < _groups.Length; i++)
                {
                    _groups[i] = new Group(_match._text, _match._matches[i + 1], _match._matchcount[i + 1]);
                }
            }

            return _groups[groupnum - 1];
        }

        /// <summary>
        /// Copies all the elements of the collection to the given array
        /// beginning at the given index.
        /// </summary>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (int i = arrayIndex, j = 0; j < Count; i++, j++)
            {
                array.SetValue(this[j], i);
            }
        }

        /// <summary>
        /// Provides an enumerator in the same order as Item[].
        /// </summary>
        public IEnumerator<Group> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return GetGroup(i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
