// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;

namespace System.Collections.Immutable
{
    public partial struct ImmutableArray<T> : IReadOnlyList<T>, IList<T>, IEquatable<ImmutableArray<T>>, IList, IImmutableArray, IStructuralComparable, IStructuralEquatable
    {
        /// <summary>
        /// Gets or sets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        /// <exception cref="NotSupportedException">Always thrown from the setter.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        T IList<T>.this[int index]
        {
            get
            {
                var self = this;
                self.ThrowInvalidOperationIfNotInitialized();
                return self[index];
            }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the number of array in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int ICollection<T>.Count
        {
            get
            {
                var self = this;
                self.ThrowInvalidOperationIfNotInitialized();
                return self.Length;
            }
        }

        /// <summary>
        /// Gets the number of array in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int IReadOnlyCollection<T>.Count
        {
            get
            {
                var self = this;
                self.ThrowInvalidOperationIfNotInitialized();
                return self.Length;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The element.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                var self = this;
                self.ThrowInvalidOperationIfNotInitialized();
                return self[index];
            }
        }

        #region Explicit interface methods

        [Pure]
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        [Pure]
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        [Pure]
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [Pure]
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        [Pure]
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds an item to the <see cref="IList"/>.
        /// </summary>
        /// <param name="value">The object to add to the <see cref="IList"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        [ExcludeFromCodeCoverage]
        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        [ExcludeFromCodeCoverage]
        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="IList"/>.</param>
        /// <returns>
        /// true if the <see cref="object"/> is found in the <see cref="IList"/>; otherwise, false.
        /// </returns>
        [ExcludeFromCodeCoverage]
        bool IList.Contains(object value)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.Contains((T)value);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList"/>.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        [ExcludeFromCodeCoverage]
        int IList.IndexOf(object value)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            return self.IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The object to insert into the <see cref="IList"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        [ExcludeFromCodeCoverage]
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed size.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed size; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IList.IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IList.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the size of the array.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int ICollection.Count
        {
            get
            {
                var self = this;
                self.ThrowInvalidOperationIfNotInitialized();
                return self.Length;
            }
        }

        /// <summary>
        /// See the <see cref="ICollection"/> interface.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection.IsSynchronized
        {
            get
            {
                // This is immutable, so it is always thread-safe.
                return true;
            }
        }

        /// <summary>
        /// Gets the sync root.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IList"/>.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="IList"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        [ExcludeFromCodeCoverage]
        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the <see cref="IList{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        [ExcludeFromCodeCoverage]
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Always thrown from the setter.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        [ExcludeFromCodeCoverage]
        object IList.this[int index]
        {
            get
            {
                var self = this;
                self.ThrowInvalidOperationIfNotInitialized();
                return self[index];
            }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        [ExcludeFromCodeCoverage]
        void ICollection.CopyTo(Array array, int index)
        {
            var self = this;
            self.ThrowInvalidOperationIfNotInitialized();
            Array.Copy(self.array, 0, array, index, self.Length);
        }

        /// <summary>
        /// Determines whether an object is structurally equal to the current instance.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <param name="comparer">An object that determines whether the current instance and other are equal.</param>
        /// <returns>true if the two objects are equal; otherwise, false.</returns>
        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var self = this;
            Array otherArray = other as Array;
            if (otherArray == null)
            {
                var theirs = other as IImmutableArray;
                if (theirs != null)
                {
                    if (self.array == null && theirs.Array == null)
                    {
                        return true;
                    }
                    else if (self.array == null)
                    {
                        return false;
                    }

                    otherArray = theirs.Array;
                }
            }

            IStructuralEquatable ours = self.array;
            return ours.Equals(otherArray, comparer);
        }

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <param name="comparer">An object that computes the hash code of the current object.</param>
        /// <returns>The hash code for the current instance.</returns>
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            var self = this;
            IStructuralEquatable ours = self.array;
            return ours != null ? ours.GetHashCode(comparer) : self.GetHashCode();
        }

        /// <summary>
        /// Determines whether the current collection object precedes, occurs in the
        /// same position as, or follows another object in the sort order.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <param name="comparer">
        /// An object that compares members of the current collection object with the
        /// corresponding members of other.
        /// </param>
        /// <returns>
        /// An integer that indicates the relationship of the current collection object
        /// to other.
        /// </returns>
        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            var self = this;
            Array otherArray = other as Array;
            if (otherArray == null)
            {
                var theirs = other as IImmutableArray;
                if (theirs != null)
                {
                    if (self.array == null && theirs.Array == null)
                    {
                        return 0;
                    }
                    else if (self.array == null ^ theirs.Array == null)
                    {
                        throw new ArgumentException(SR.ArrayInitializedStateNotEqual, nameof(other));
                    }

                    otherArray = theirs.Array;
                }
            }

            if (otherArray != null)
            {
                IStructuralComparable ours = self.array;
                return ours.CompareTo(otherArray, comparer);
            }

            throw new ArgumentException(SR.ArrayLengthsNotEqual, nameof(other));
        }

        #endregion
    }
}