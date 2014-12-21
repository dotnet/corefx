// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// The CaptureCollection lists the captured Capture numbers
// contained in a compiled Regex.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.RegularExpressions
{
    /*
     * This collection returns the Captures for a group
     * in the order in which they were matched (left to right
     * or right to left). It is created by Group.Captures
     */
    /// <summary>
    /// Represents a sequence of capture substrings. The object is used
    /// to return the set of captures done by a single capturing group.
    /// </summary>
    public class CaptureCollection : IList<Capture>, IReadOnlyList<Capture>, IList
    {
        internal Group _group;
        internal int _capcount;
        internal Capture[] _captures;

        internal CaptureCollection(Group group)
        {
            _group = group;
            _capcount = _group._capcount;
        }

        /// <summary>
        /// The object on which to synchronize.
        /// </summary>
        Object ICollection.SyncRoot
        {
            get
            {
                return _group;
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
        /// Returns the number of captures.
        /// </summary>
        public int Count
        {
            get
            {
                return _capcount;
            }
        }

        /// <summary>
        /// Returns a specific capture, by index, in this collection.
        /// </summary>
        public Capture this[int i]
        {
            get
            {
                return GetCapture(i);
            }
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
            return new CaptureEnumerator(this);
        }

        IEnumerator<Capture> IEnumerable<Capture>.GetEnumerator()
        {
            return new CaptureEnumerator(this);
        }

        /*
         * Nonpublic code to return set of captures for the group
         */
        internal Capture GetCapture(int i)
        {
            if (i == _capcount - 1 && i >= 0)
                return _group;

            if (i >= _capcount || i < 0)
                throw new ArgumentOutOfRangeException("i");

            // first time a capture is accessed, compute them all
            if (_captures == null)
            {
                _captures = new Capture[_capcount];
                for (int j = 0; j < _capcount - 1; j++)
                {
                    _captures[j] = new Capture(_group._text, _group._caps[j * 2], _group._caps[j * 2 + 1]);
                }
            }

            return _captures[i];
        }

        int IList<Capture>.IndexOf(Capture item)
        {
            var comparer = EqualityComparer<Capture>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                    return i;
            }
            return -1;
        }

        void IList<Capture>.Insert(int index, Capture item)
        {
            throw new NotSupportedException();
        }

        void IList<Capture>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        Capture IList<Capture>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        void ICollection<Capture>.Add(Capture item)
        {
            throw new NotSupportedException();
        }

        void ICollection<Capture>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<Capture>.Contains(Capture item)
        {
            var comparer = EqualityComparer<Capture>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                    return true;
            }
            return false;
        }

        void ICollection<Capture>.CopyTo(Capture[] array, int arrayIndex)
        {
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        bool ICollection<Capture>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<Capture>.Remove(Capture item)
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
            return value is Capture && ((ICollection<Capture>)this).Contains((Capture)value);
        }

        int IList.IndexOf(object value)
        {
            return value is Capture ? ((IList<Capture>)this).IndexOf((Capture)value) : -1;
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

    internal class CaptureEnumerator : IEnumerator<Capture>
    {
        internal CaptureCollection _rcc;
        internal int _curindex;

        /*
         * Nonpublic constructor
         */
        internal CaptureEnumerator(CaptureCollection rcc)
        {
            _curindex = -1;
            _rcc = rcc;
        }

        /*
         * As required by IEnumerator
         */
        public bool MoveNext()
        {
            int size = _rcc.Count;

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

        Capture IEnumerator<Capture>.Current
        {
            get { return Capture; }
        }

        /*
         * Returns the current capture
         */
        public Capture Capture
        {
            get
            {
                if (_curindex < 0 || _curindex >= _rcc.Count)
                    throw new InvalidOperationException(SR.EnumNotStarted);

                return _rcc[_curindex];
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
