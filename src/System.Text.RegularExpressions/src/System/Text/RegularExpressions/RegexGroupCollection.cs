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
        private readonly Match _match;
        private readonly Dictionary<int, int> _captureMap;

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
                int index;
                if (!_captureMap.TryGetValue(groupnum, out index))
                    return Group._emptygroup;

                return GetGroupImpl(index);
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
        public void CopyTo(Group[] array, int arrayIndex)
        {
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Provides an enumerator in the same order as Item[].
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<Group> IEnumerable<Group>.GetEnumerator()
        {
            return new Enumerator(this);
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
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void IList<Group>.RemoveAt(int index)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        Group IList<Group>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection); }
        }

        void ICollection<Group>.Add(Group item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void ICollection<Group>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
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

        bool ICollection<Group>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<Group>.Remove(Group item)
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
            return value is Group && ((ICollection<Group>)this).Contains((Group)value);
        }

        int IList.IndexOf(object value)
        {
            return value is Group ? ((IList<Group>)this).IndexOf((Group)value) : -1;
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
            get { return _match; }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (Count == 0)
                return;

            int[] indices = new int[1]; // SetValue takes a params array; lifting out the implicit allocation from the loop

            for (int i = arrayIndex, j = 0; j < Count; i++, j++)
            {
                indices[0] = i;
                array.SetValue(this[j], indices);
            }
        }

        private class Enumerator : IEnumerator<Group>
        {
            private readonly GroupCollection _collection;
            private int _index;

            internal Enumerator(GroupCollection collection)
            {
                _collection = collection;
                _index = -1;
            }

            public bool MoveNext()
            {
                int size = _collection.Count;

                if (_index >= size)
                    return false;

                _index++;

                return (_index < size);
            }

            public Group Current
            {
                get
                {
                    if (_index < 0 || _index >= _collection.Count)
                        throw new InvalidOperationException(SR.EnumNotStarted);

                    return _collection[_index];
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            void IEnumerator.Reset()
            {
                _index = -1;
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
}
