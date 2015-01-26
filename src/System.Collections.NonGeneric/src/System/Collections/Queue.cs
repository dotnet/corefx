// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/*=============================================================================
**
** Class: Queue
**
** Purpose: Represents a first-in, first-out collection of objects.
**
=============================================================================*/

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Collections
{
    // A simple Queue of objects.  Internally it is implemented as a circular
    // buffer, so Enqueue can be O(n).  Dequeue is O(1).
    [DebuggerTypeProxy(typeof(QueueDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Queue : ICollection
    {
        private Object[] _array;
        private int _head;       // First valid element in the queue
        private int _tail;       // Last valid element in the queue
        private int _size;       // Number of elements.
        private readonly int _growFactor; // 100 == 1.0, 130 == 1.3, 200 == 2.0
        private int _version;
        private Object _syncRoot;

        private const int _MinimumGrow = 4;
        private const int _ShrinkThreshold = 32;

        // Creates a queue with room for capacity objects. The default initial
        // capacity and grow factor are used.
        public Queue()
            : this(32, (float)2.0)
        {
        }

        // Creates a queue with room for capacity objects. The default grow factor
        // is used.
        //
        public Queue(int capacity)
            : this(capacity, (float)2.0)
        {
        }

        // Creates a queue with room for capacity objects. When full, the new
        // capacity is set to the old capacity * growFactor.
        //
        public Queue(int capacity, float growFactor)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (!(growFactor >= 1.0 && growFactor <= 10.0))
                throw new ArgumentOutOfRangeException("growFactor", SR.Format(SR.ArgumentOutOfRange_QueueGrowFactor, 1, 10));
            Contract.EndContractBlock();

            _array = new Object[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
            _growFactor = (int)(growFactor * 100);
        }

        // Fills a Queue with the elements of an ICollection.  Uses the enumerator
        // to get each of the elements.
        //
        public Queue(ICollection col) : this((col == null ? 32 : col.Count))
        {
            if (col == null)
                throw new ArgumentNullException("col");
            Contract.EndContractBlock();
            IEnumerator en = col.GetEnumerator();
            while (en.MoveNext())
                Enqueue(en.Current);
        }


        public virtual int Count
        {
            get { return _size; }
        }

        public virtual Object Clone()
        {
            Queue q = new Queue(_size);
            q._size = _size;

            int numToCopy = _size;
            int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, q._array, 0, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
                Array.Copy(_array, 0, q._array, _array.Length - _head, numToCopy);

            q._version = _version;
            return q;
        }

        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual Object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        // Removes all Objects from the queue.
        public virtual void Clear()
        {
            if (_head < _tail)
                Array.Clear(_array, _head, _size);
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _size = 0;
            _version++;
        }

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        // 
        public virtual void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", SR.ArgumentOutOfRange_Index);
            Contract.EndContractBlock();
            int arrayLen = array.Length;
            if (arrayLen - index < _size)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            int numToCopy = _size;
            if (numToCopy == 0)
                return;
            int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, array, index, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
                Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
        }

        // Adds obj to the tail of the queue.
        //
        public virtual void Enqueue(Object obj)
        {
            if (_size == _array.Length)
            {
                int newcapacity = (int)((long)_array.Length * (long)_growFactor / 100);
                if (newcapacity < _array.Length + _MinimumGrow)
                {
                    newcapacity = _array.Length + _MinimumGrow;
                }
                SetCapacity(newcapacity);
            }

            _array[_tail] = obj;
            _tail = (_tail + 1) % _array.Length;
            _size++;
            _version++;
        }

        // GetEnumerator returns an IEnumerator over this Queue.  This
        // Enumerator will support removing.
        // 
        public virtual IEnumerator GetEnumerator()
        {
            return new QueueEnumerator(this);
        }

        // Removes the object at the head of the queue and returns it. If the queue
        // is empty, this method simply returns null.
        public virtual Object Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException(SR.InvalidOperation_EmptyQueue);
            Contract.EndContractBlock();

            Object removed = _array[_head];
            _array[_head] = null;
            _head = (_head + 1) % _array.Length;
            _size--;
            _version++;
            return removed;
        }

        // Returns the object at the head of the queue. The object remains in the
        // queue. If the queue is empty, this method throws an 
        // InvalidOperationException.
        public virtual Object Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException(SR.InvalidOperation_EmptyQueue);
            Contract.EndContractBlock();

            return _array[_head];
        }

        // Returns a synchronized Queue.  Returns a synchronized wrapper
        // class around the queue - the caller must not use references to the
        // original queue.
        // 
        public static Queue Synchronized(Queue queue)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");
            Contract.EndContractBlock();
            return new SynchronizedQueue(queue);
        }

        // Returns true if the queue contains at least one object equal to obj.
        // Equality is determined using obj.Equals().
        //
        // Exceptions: ArgumentNullException if obj == null.
        public virtual bool Contains(Object obj)
        {
            int index = _head;
            int count = _size;

            while (count-- > 0)
            {
                if (obj == null)
                {
                    if (_array[index] == null)
                        return true;
                }
                else if (_array[index] != null && _array[index].Equals(obj))
                {
                    return true;
                }
                index = (index + 1) % _array.Length;
            }

            return false;
        }

        internal Object GetElement(int i)
        {
            return _array[(_head + i) % _array.Length];
        }

        // Iterates over the objects in the queue, returning an array of the
        // objects in the Queue, or an empty array if the queue is empty.
        // The order of elements in the array is first in to last in, the same
        // order produced by successive calls to Dequeue.
        public virtual Object[] ToArray()
        {
            if (_size == 0)
                return Array.Empty<Object>();

            Object[] arr = new Object[_size];
            if (_head < _tail)
            {
                Array.Copy(_array, _head, arr, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, arr, 0, _array.Length - _head);
                Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
            }

            return arr;
        }


        // PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
        // must be >= _size.
        private void SetCapacity(int capacity)
        {
            Object[] newarray = new Object[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
            _version++;
        }

        public virtual void TrimToSize()
        {
            SetCapacity(_size);
        }


        // Implements a synchronization wrapper around a queue.
        private class SynchronizedQueue : Queue
        {
            private readonly Queue _q;
            private readonly Object _root;

            internal SynchronizedQueue(Queue q)
            {
                _q = q;
                _root = _q.SyncRoot;
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override Object SyncRoot
            {
                get
                {
                    return _root;
                }
            }

            public override int Count
            {
                get
                {
                    lock (_root)
                    {
                        return _q.Count;
                    }
                }
            }

            public override void Clear()
            {
                lock (_root)
                {
                    _q.Clear();
                }
            }

            public override Object Clone()
            {
                lock (_root)
                {
                    return new SynchronizedQueue((Queue)_q.Clone());
                }
            }

            public override bool Contains(Object obj)
            {
                lock (_root)
                {
                    return _q.Contains(obj);
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (_root)
                {
                    _q.CopyTo(array, arrayIndex);
                }
            }

            public override void Enqueue(Object value)
            {
                lock (_root)
                {
                    _q.Enqueue(value);
                }
            }

            [SuppressMessage("Microsoft.Contracts", "CC1055")]  // Thread safety problems with precondition - can't express the precondition as of Dev10.
            public override Object Dequeue()
            {
                lock (_root)
                {
                    return _q.Dequeue();
                }
            }

            public override IEnumerator GetEnumerator()
            {
                lock (_root)
                {
                    return _q.GetEnumerator();
                }
            }

            [SuppressMessage("Microsoft.Contracts", "CC1055")]  // Thread safety problems with precondition - can't express the precondition as of Dev10.
            public override Object Peek()
            {
                lock (_root)
                {
                    return _q.Peek();
                }
            }

            public override Object[] ToArray()
            {
                lock (_root)
                {
                    return _q.ToArray();
                }
            }

            public override void TrimToSize()
            {
                lock (_root)
                {
                    _q.TrimToSize();
                }
            }
        }


        // Implements an enumerator for a Queue.  The enumerator uses the
        // internal version number of the list to ensure that no modifications are
        // made to the list while an enumeration is in progress.
        private class QueueEnumerator : IEnumerator
        {
            private readonly Queue _q;
            private int _index;
            private readonly int _version;
            private Object _currentElement;

            internal QueueEnumerator(Queue q)
            {
                _q = q;
                _version = _q._version;
                _index = 0;
                _currentElement = _q._array;
                if (_q._size == 0)
                    _index = -1;
            }

            public virtual bool MoveNext()
            {
                if (_version != _q._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                if (_index < 0)
                {
                    _currentElement = _q._array;
                    return false;
                }

                _currentElement = _q.GetElement(_index);
                _index++;

                if (_index == _q._size)
                    _index = -1;
                return true;
            }

            public virtual Object Current
            {
                get
                {
                    if (_currentElement == _q._array)
                    {
                        if (_index == 0)
                            throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                        else
                            throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
                    }
                    return _currentElement;
                }
            }

            public virtual void Reset()
            {
                if (_version != _q._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                if (_q._size == 0)
                    _index = -1;
                else
                    _index = 0;
                _currentElement = _q._array;
            }
        }

        internal class QueueDebugView
        {
            private readonly Queue _queue;

            public QueueDebugView(Queue queue)
            {
                if (queue == null)
                    throw new ArgumentNullException("queue");
                Contract.EndContractBlock();

                _queue = queue;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Object[] Items
            {
                get
                {
                    return _queue.ToArray();
                }
            }
        }
    }
}
