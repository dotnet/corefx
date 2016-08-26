// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
** Purpose: A circular-array implementation of a generic queue.
**
**
=============================================================================*/

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // A simple Queue of generic objects.  Internally it is implemented as a 
    // circular buffer, so Enqueue can be O(n).  Dequeue is O(1).
    [DebuggerTypeProxy(typeof(QueueDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class Queue<T> : IEnumerable<T>,
        System.Collections.ICollection,
        IReadOnlyCollection<T>
    {
        private T[] _array;
        private int _head;       // The index from which to dequeue if the queue isn't empty.
        private int _tail;       // The index at which to enqueue if the queue isn't full.
        private int _size;       // Number of elements.
        private int _version;
        private object _syncRoot;

        private const int MinimumGrow = 4;
        private const int GrowFactor = 200;  // double each time

        // Creates a queue with room for capacity objects. The default initial
        // capacity and grow factor are used.
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Queue"]/*' />
        public Queue()
        {
            _array = Array.Empty<T>();
        }

        // Creates a queue with room for capacity objects. The default grow factor
        // is used.
        //
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Queue1"]/*' />
        public Queue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, SR.ArgumentOutOfRange_NeedNonNegNum);
            _array = new T[capacity];
        }

        // Fills a Queue with the elements of an ICollection.  Uses the enumerator
        // to get each of the elements.
        //
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Queue3"]/*' />
        public Queue(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            _array = EnumerableHelpers.ToArray(collection, out _size);
            if (_size != _array.Length) _tail = _size;
        }

        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Count"]/*' />
        public int Count
        {
            get { return _size; }
        }

        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.IsSynchronized"]/*' />
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        // Removes all Objects from the queue.
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Clear"]/*' />
        public void Clear()
        {
            if (_size != 0)
            {
                if (_head < _tail)
                    Array.Clear(_array, _head, _size);
                else
                {
                    Array.Clear(_array, _head, _array.Length - _head);
                    Array.Clear(_array, 0, _tail);
                }

                _size = 0;
            }

            _head = 0;
            _tail = 0;
            _version++;
        }

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        // 
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.CopyTo"]/*' />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, SR.ArgumentOutOfRange_Index);
            }

            int arrayLen = array.Length;
            if (arrayLen - arrayIndex < _size)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            int numToCopy = _size;
            if (numToCopy == 0) return;

            int firstPart = Math.Min(_array.Length - _head, numToCopy);
            Array.Copy(_array, _head, array, arrayIndex, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
            {
                Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(array));
            }

            int arrayLen = array.Length;
            if (index < 0 || index > arrayLen)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
            }

            if (arrayLen - index < _size)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            int numToCopy = _size;
            if (numToCopy == 0) return;

            try
            {
                int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
                Array.Copy(_array, _head, array, index, firstPart);
                numToCopy -= firstPart;

                if (numToCopy > 0)
                {
                    Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
            }
        }

        // Adds item to the tail of the queue.
        //
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Enqueue"]/*' />
        public void Enqueue(T item)
        {
            if (_size == _array.Length)
            {
                int newcapacity = (int)((long)_array.Length * (long)GrowFactor / 100);
                if (newcapacity < _array.Length + MinimumGrow)
                {
                    newcapacity = _array.Length + MinimumGrow;
                }
                SetCapacity(newcapacity);
            }

            _array[_tail] = item;
            MoveNext(ref _tail);
            _size++;
            _version++;
        }

        // GetEnumerator returns an IEnumerator over this Queue.  This
        // Enumerator will support removing.
        // 
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.GetEnumerator"]/*' />
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        // Removes the object at the head of the queue and returns it. If the queue
        // is empty, this method simply returns null.
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Dequeue"]/*' />
        public T Dequeue()
        {
            if (_size == 0)
                throw new InvalidOperationException(SR.InvalidOperation_EmptyQueue);

            T removed = _array[_head];
            _array[_head] = default(T);
            MoveNext(ref _head);
            _size--;
            _version++;
            return removed;
        }

        // Returns the object at the head of the queue. The object remains in the
        // queue. If the queue is empty, this method throws an 
        // InvalidOperationException.
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Peek"]/*' />
        public T Peek()
        {
            if (_size == 0)
                throw new InvalidOperationException(SR.InvalidOperation_EmptyQueue);

            return _array[_head];
        }

        // Returns true if the queue contains at least one object equal to item.
        // Equality is determined using item.Equals().
        //
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.Contains"]/*' />
        public bool Contains(T item)
        {
            int index = _head;
            int count = _size;

            EqualityComparer<T> c = EqualityComparer<T>.Default;
            while (count-- > 0)
            {
                if (c.Equals(_array[index], item))
                {
                    return true;
                }
                MoveNext(ref index);
            }

            return false;
        }

        // Iterates over the objects in the queue, returning an array of the
        // objects in the Queue, or an empty array if the queue is empty.
        // The order of elements in the array is first in to last in, the same
        // order produced by successive calls to Dequeue.
        /// <include file='doc\Queue.uex' path='docs/doc[@for="Queue.ToArray"]/*' />
        public T[] ToArray()
        {
            if (_size == 0)
            {
                return Array.Empty<T>();
            }

            T[] arr = new T[_size];

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
            T[] newarray = new T[capacity];
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

        // Increments the index wrapping it if necessary.
        private void MoveNext(ref int index)
        {
            // It is tempting to use the remainder operator here but it is actually much slower 
            // than a simple comparison and a rarely taken branch.   
            int tmp = index + 1;
            index = (tmp == _array.Length) ? 0 : tmp;
        }

        public void TrimExcess()
        {
            int threshold = (int)(((double)_array.Length) * 0.9);
            if (_size < threshold)
            {
                SetCapacity(_size);
            }
        }

        // Implements an enumerator for a Queue.  The enumerator uses the
        // internal version number of the list to ensure that no modifications are
        // made to the list while an enumeration is in progress.
        /// <include file='doc\Queue.uex' path='docs/doc[@for="QueueEnumerator"]/*' />
        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
        public struct Enumerator : IEnumerator<T>,
            System.Collections.IEnumerator
        {
            private readonly Queue<T> _q;
            private readonly int _version;
            private int _index;   // -1 = not started, -2 = ended/disposed
            private T _currentElement;

            internal Enumerator(Queue<T> q)
            {
                _q = q;
                _version = q._version;
                _index = -1;
                _currentElement = default(T);
            }

            /// <include file='doc\Queue.uex' path='docs/doc[@for="QueueEnumerator.Dispose"]/*' />
            public void Dispose()
            {
                _index = -2;
                _currentElement = default(T);
            }

            /// <include file='doc\Queue.uex' path='docs/doc[@for="QueueEnumerator.MoveNext"]/*' />
            public bool MoveNext()
            {
                if (_version != _q._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                // Instead of return true/false, we assign the result to a variable and
                // return it at the end. This helps decrease code size, as currently the
                // jit cannot do this for us and generates code for 3 returns.
                bool result = false;

                if (_index != -2)
                {
                    _index++;

                    if (_index == _q._size)
                    {
                        // We've run past the last element
                        _index = -2;
                        _currentElement = default(T);
                    }
                    else
                    {
                        // Cache some fields in locals to decrease code size
                        T[] array = _q._array;
                        int capacity = array.Length;

                        // _index represents the 0-based index into the queue, however the queue
                        // doesn't have to start from 0 and it may not even be stored contiguously in memory.

                        int arrayIndex = _q._head + _index; // this is the actual index into the queue's backing array
                        if (arrayIndex >= capacity)
                        {
                            // NOTE: Originally we were using the modulo operator here, however
                            // on Intel processors it has a very high instruction latency which
                            // was slowing down the loop quite a bit.
                            // Replacing it with simple comparison/subtraction operations sped up
                            // the average foreach loop by 2x.

                            arrayIndex -= capacity; // wrap around if needed
                        }
                        
                        _currentElement = array[arrayIndex];
                        result = true;
                    }
                }

                return result;
            }

            /// <include file='doc\Queue.uex' path='docs/doc[@for="QueueEnumerator.Current"]/*' />
            public T Current
            {
                get
                {
                    if (_index < 0)
                        ThrowEnumerationNotStartedOrEnded();
                    return _currentElement;
                }
            }

            private void ThrowEnumerationNotStartedOrEnded()
            {
                Debug.Assert(_index == -1 || _index == -2);
                throw new InvalidOperationException(_index == -1 ? SR.InvalidOperation_EnumNotStarted : SR.InvalidOperation_EnumEnded);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            void IEnumerator.Reset()
            {
                if (_version != _q._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                _index = -1;
                _currentElement = default(T);
            }
        }
    }
}
