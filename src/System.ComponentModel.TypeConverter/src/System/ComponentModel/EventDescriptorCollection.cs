// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2112:SecuredTypesShouldNotExposeFields", Scope = "type", Target = "System.ComponentModel.EventDescriptorCollection")]

namespace System.ComponentModel
{
    /// <summary>
    ///    <para>
    ///       Represents a collection of events.
    ///    </para>
    /// </summary>
    public class EventDescriptorCollection : ICollection, IList
    {
        private EventDescriptor[] _events;
        private string[] _namedSort;
        private readonly IComparer _comparer;
        private bool _eventsOwned;
        private bool _needSort = false;
        private int _eventCount;
        private readonly bool _readOnly = false;

        /// <summary>
        /// An empty AttributeCollection that can used instead of creating a new one with no items.
        /// </summary>
        public static readonly EventDescriptorCollection Empty = new EventDescriptorCollection(null, true);

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.EventDescriptorCollection'/> class.
        ///    </para>
        /// </summary>
        public EventDescriptorCollection(EventDescriptor[] events)
        {
            if (events == null)
            {
                _events = Array.Empty<EventDescriptor>();
                _eventCount = 0;
            }
            else
            {
                _events = events;
                _eventCount = events.Length;
            }
            _eventsOwned = true;
        }

        /// <summary>
        ///     Initializes a new instance of an event descriptor collection, and allows you to mark the
        ///     collection as read-only so it cannot be modified.
        /// </summary>
        public EventDescriptorCollection(EventDescriptor[] events, bool readOnly) : this(events)
        {
            _readOnly = readOnly;
        }

        private EventDescriptorCollection(EventDescriptor[] events, int eventCount, string[] namedSort, IComparer comparer)
        {
            _eventsOwned = false;
            if (namedSort != null)
            {
                _namedSort = (string[])namedSort.Clone();
            }
            _comparer = comparer;
            _events = events;
            _eventCount = eventCount;
            _needSort = true;
        }

        /// <summary>
        ///    <para>
        ///       Gets the number
        ///       of event descriptors in the collection.
        ///    </para>
        /// </summary>
        public int Count
        {
            get
            {
                return _eventCount;
            }
        }

        /// <summary>
        ///    <para>Gets the event with the specified index 
        ///       number.</para>
        /// </summary>
        public virtual EventDescriptor this[int index]
        {
            get
            {
                if (index >= _eventCount)
                {
                    throw new IndexOutOfRangeException();
                }
                EnsureEventsOwned();
                return _events[index];
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets the event with the specified name.
        ///    </para>
        /// </summary>
        public virtual EventDescriptor this[string name]
        {
            get
            {
                return Find(name, false);
            }
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public int Add(EventDescriptor value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            EnsureSize(_eventCount + 1);
            _events[_eventCount++] = value;
            return _eventCount - 1;
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public void Clear()
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            _eventCount = 0;
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public bool Contains(EventDescriptor value)
        {
            return IndexOf(value) >= 0;
        }

        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index)
        {
            EnsureEventsOwned();
            Array.Copy(_events, 0, array, index, Count);
        }

        private void EnsureEventsOwned()
        {
            if (!_eventsOwned)
            {
                _eventsOwned = true;
                if (_events != null)
                {
                    EventDescriptor[] newEvents = new EventDescriptor[Count];
                    Array.Copy(_events, 0, newEvents, 0, Count);
                    _events = newEvents;
                }
            }

            if (_needSort)
            {
                _needSort = false;
                InternalSort(_namedSort);
            }
        }

        private void EnsureSize(int sizeNeeded)
        {
            if (sizeNeeded <= _events.Length)
            {
                return;
            }

            if (_events.Length == 0)
            {
                _eventCount = 0;
                _events = new EventDescriptor[sizeNeeded];
                return;
            }

            EnsureEventsOwned();

            int newSize = Math.Max(sizeNeeded, _events.Length * 2);
            EventDescriptor[] newEvents = new EventDescriptor[newSize];
            Array.Copy(_events, 0, newEvents, 0, _eventCount);
            _events = newEvents;
        }

        /// <summary>
        ///    <para>
        ///       Gets the description of the event with the specified
        ///       name
        ///       in the collection.
        ///    </para>
        /// </summary>
        public virtual EventDescriptor Find(string name, bool ignoreCase)
        {
            EventDescriptor p = null;

            if (ignoreCase)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (String.Equals(_events[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        p = _events[i];
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (String.Equals(_events[i].Name, name, StringComparison.Ordinal))
                    {
                        p = _events[i];
                        break;
                    }
                }
            }

            return p;
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public int IndexOf(EventDescriptor value)
        {
            return Array.IndexOf(_events, value, 0, _eventCount);
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public void Insert(int index, EventDescriptor value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            EnsureSize(_eventCount + 1);
            if (index < _eventCount)
            {
                Array.Copy(_events, index, _events, index + 1, _eventCount - index);
            }
            _events[index] = value;
            _eventCount++;
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public void Remove(EventDescriptor value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            int index = IndexOf(value);

            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public void RemoveAt(int index)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            if (index < _eventCount - 1)
            {
                Array.Copy(_events, index + 1, _events, index, _eventCount - index - 1);
            }
            _events[_eventCount - 1] = null;
            _eventCount--;
        }

        /// <summary>
        ///    <para>
        ///       Gets an enumerator for this <see cref='System.ComponentModel.EventDescriptorCollection'/>.
        ///    </para>
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            // we can only return an enumerator on the events we actually have...
            if (_events.Length == _eventCount)
            {
                return _events.GetEnumerator();
            }
            else
            {
                return new ArraySubsetEnumerator(_events, _eventCount);
            }
        }

        /// <summary>
        ///    <para>
        ///       Sorts the members of this EventDescriptorCollection, using the default sort for this collection, 
        ///       which is usually alphabetical.
        ///    </para>
        /// </summary>
        public virtual EventDescriptorCollection Sort()
        {
            return new EventDescriptorCollection(_events, _eventCount, _namedSort, _comparer);
        }


        /// <summary>
        ///    <para>
        ///       Sorts the members of this EventDescriptorCollection.  Any specified NamedSort arguments will 
        ///       be applied first, followed by sort using the specified IComparer.
        ///    </para>
        /// </summary>
        public virtual EventDescriptorCollection Sort(string[] names)
        {
            return new EventDescriptorCollection(_events, _eventCount, names, _comparer);
        }

        /// <summary>
        ///    <para>
        ///       Sorts the members of this EventDescriptorCollection.  Any specified NamedSort arguments will 
        ///       be applied first, followed by sort using the specified IComparer.
        ///    </para>
        /// </summary>
        public virtual EventDescriptorCollection Sort(string[] names, IComparer comparer)
        {
            return new EventDescriptorCollection(_events, _eventCount, names, comparer);
        }

        /// <summary>
        ///    <para>
        ///       Sorts the members of this EventDescriptorCollection, using the specified IComparer to compare, 
        ///       the EventDescriptors contained in the collection.
        ///    </para>
        /// </summary>
        public virtual EventDescriptorCollection Sort(IComparer comparer)
        {
            return new EventDescriptorCollection(_events, _eventCount, _namedSort, comparer);
        }

        /// <summary>
        ///    <para>
        ///       Sorts the members of this EventDescriptorCollection.  Any specified NamedSort arguments will 
        ///       be applied first, followed by sort using the specified IComparer.
        ///    </para>
        /// </summary>
        protected void InternalSort(string[] names)
        {
            if (_events.Length == 0)
            {
                return;
            }

            InternalSort(_comparer);

            if (names != null && names.Length > 0)
            {
                ArrayList eventArrayList = new ArrayList(_events);
                int foundCount = 0;
                int eventCount = _events.Length;

                for (int i = 0; i < names.Length; i++)
                {
                    for (int j = 0; j < eventCount; j++)
                    {
                        EventDescriptor currentEvent = (EventDescriptor)eventArrayList[j];

                        // Found a matching event.  Here, we add it to our array.  We also
                        // mark it as null in our array list so we don't add it twice later.
                        //
                        if (currentEvent != null && currentEvent.Name.Equals(names[i]))
                        {
                            _events[foundCount++] = currentEvent;
                            eventArrayList[j] = null;
                            break;
                        }
                    }
                }

                // At this point we have filled in the first "foundCount" number of propeties, one for each
                // name in our name array.  If a name didn't match, then it is ignored.  Next, we must fill
                // in the rest of the properties.  We now have a sparse array containing the remainder, so
                // it's easy.
                //
                for (int i = 0; i < eventCount; i++)
                {
                    if (eventArrayList[i] != null)
                    {
                        _events[foundCount++] = (EventDescriptor)eventArrayList[i];
                    }
                }

                Debug.Assert(foundCount == eventCount, "We did not completely fill our event array");
            }
        }

        /// <summary>
        ///    <para>
        ///       Sorts the members of this EventDescriptorCollection using the specified IComparer.
        ///    </para>
        /// </summary>
        protected void InternalSort(IComparer sorter)
        {
            if (sorter == null)
            {
                TypeDescriptor.SortDescriptorArray(this);
            }
            else
            {
                Array.Sort(_events, sorter);
            }
        }

        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        int ICollection.Count 
        {
            get
            {
                return Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        

        /// <internalonly/>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (_readOnly)
                {
                    throw new NotSupportedException();
                }

                if (index >= _eventCount)
                {
                    throw new IndexOutOfRangeException();
                }
                EnsureEventsOwned();
                _events[index] = (EventDescriptor)value;
            }
        }

        /// <internalonly/>
        int IList.Add(object value)
        {
            return Add((EventDescriptor)value);
        }

        /// <internalonly/>
        bool IList.Contains(object value)
        {
            return Contains((EventDescriptor)value);
        }

        void IList.Clear()
        {
            Clear();
        }       

        /// <internalonly/>
        int IList.IndexOf(object value)
        {
            return IndexOf((EventDescriptor)value);
        }

        /// <internalonly/>
        void IList.Insert(int index, object value)
        {
            Insert(index, (EventDescriptor)value);
        }

        /// <internalonly/>
        void IList.Remove(object value)
        {
            Remove((EventDescriptor)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }        

        /// <internalonly/>
        bool IList.IsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        /// <internalonly/>
        bool IList.IsFixedSize
        {
            get
            {
                return _readOnly;
            }
        }

        private class ArraySubsetEnumerator : IEnumerator
        {
            private readonly Array _array;
            private readonly int _total;
            private int _current;

            public ArraySubsetEnumerator(Array array, int count)
            {
                Debug.Assert(count == 0 || array != null, "if array is null, count should be 0");
                Debug.Assert(array == null || count <= array.Length, "Trying to enumerate more than the array contains");

                _array = array;
                _total = count;
                _current = -1;
            }

            public bool MoveNext()
            {
                if (_current < _total - 1)
                {
                    _current++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _current = -1;
            }

            public object Current
            {
                get
                {
                    if (_current == -1)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        return _array.GetValue(_current);
                    }
                }
            }
        }
    }
}

