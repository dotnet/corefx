// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// The GroupCollection lists the captured Capture numbers
// contained in a compiled Regex.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.RegularExpressions
{
    /// <summary>
    /// Represents a sequence of capture substrings. The object is used
    /// to return the set of captures done by a single capturing group.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(RegexCollectionDebuggerProxy<>))]
    public class GroupCollection : IList<Group>, IReadOnlyList<Group>, IList
    {
        internal Match _match;
        internal Dictionary<Int32, Int32> _captureMap;

        // cache of Group objects fed to the user
        internal Group[] _groups;

        /*
         * Nonpublic constructor
         */
        internal GroupCollection(Match match, Dictionary<Int32, Int32> caps)
        {
            _match = match;
            _captureMap = caps;
        }

        /// <summary>
        /// The object on which to synchronize
        /// </summary>
        Object ICollection.SyncRoot
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

        public Group this[String groupname]
        {
            get
            {
                if (_match._regex == null)
                    return Group._emptygroup;

                return GetGroup(_match._regex.GroupNumberFromName(groupname));
            }
        }

        internal Group GetGroup(int groupnum)
        {
            if (_captureMap != null)
            {
                Object o;

                o = _captureMap[groupnum];
                if (o == null)
                    return Group._emptygroup;
                //throw new ArgumentOutOfRangeException("groupnum");

                return GetGroupImpl((int)o);
            }
            else
            {
                //if (groupnum >= _match._regex.CapSize || groupnum < 0)
                //   throw new ArgumentOutOfRangeException("groupnum");
                if (groupnum >= _match._matchcount.Length || groupnum < 0)
                    return Group._emptygroup;

                return GetGroupImpl(groupnum);
            }
        }


        /*
         * Caches the group objects
         */
        internal Group GetGroupImpl(int groupnum)
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
        public IEnumerator GetEnumerator()
        {
            return new GroupEnumerator(this);
        }

        IEnumerator<Group> IEnumerable<Group>.GetEnumerator()
        {
            return new GroupEnumerator(this);
        }

        int IList<Group>.IndexOf(Group item)
        {
            var comparer = EqualityComparer<Group>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                    return i;
            }
            return -1;
        }

        void IList<Group>.Insert(int index, Group item)
        {
            throw new NotSupportedException();
        }

        void IList<Group>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        Group IList<Group>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        void ICollection<Group>.Add(Group item)
        {
            throw new NotSupportedException();
        }

        void ICollection<Group>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<Group>.Contains(Group item)
        {
            var comparer = EqualityComparer<Group>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                    return true;
            }
            return false;
        }

        void ICollection<Group>.CopyTo(Group[] array, int arrayIndex)
        {
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        bool ICollection<Group>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<Group>.Remove(Group item)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            return value is Group && ((ICollection<Group>)this).Contains((Group)value);
        }

        int IList.IndexOf(object value)
        {
            return value is Group ? ((IList<Group>)this).IndexOf((Group)value) : -1;
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }
    }


    /*
     * This non-public enumerator lists all the captures
     * Should it be public?
     */
    internal class GroupEnumerator : IEnumerator<Group>
    {
        internal GroupCollection _rgc;
        internal int _curindex;

        /*
         * Nonpublic constructor
         */
        internal GroupEnumerator(GroupCollection rgc)
        {
            _curindex = -1;
            _rgc = rgc;
        }

        /*
         * As required by IEnumerator
         */
        public bool MoveNext()
        {
            int size = _rgc.Count;

            if (_curindex >= size)
                return false;

            _curindex++;

            return (_curindex < size);
        }

        /*
         * As required by IEnumerator
         */
        public Object Current
        {
            get { return Capture; }
        }

        Group IEnumerator<Group>.Current
        {
            get { return (Group)Capture; }
        }

        /*
         * Returns the current capture
         */
        public Capture Capture
        {
            get
            {
                if (_curindex < 0 || _curindex >= _rgc.Count)
                    throw new InvalidOperationException(SR.EnumNotStarted);

                return _rgc[_curindex];
            }
        }

        /*
         * Reset to before the first item
         */
        public void Reset()
        {
            _curindex = -1;
        }

        void IDisposable.Dispose()
        {
        }
    }
}
