// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
    public sealed class OidCollection : ICollection
    {
        private Oid[] _oids = Array.Empty<Oid>();
        private int _count;

        public OidCollection() { }

        public int Add(Oid oid)
        {
            int count = _count;
            if (count == _oids.Length)
            {
                Array.Resize(ref _oids, count == 0 ? 4 : count * 2);
            }
            _oids[count] = oid;
            _count = count + 1;
            return count;
        }

        public Oid this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                {
                    // For compat, throw an ArgumentOutOfRangeException instead of
                    // the IndexOutOfRangeException that comes from the array's indexer.
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _oids[index];
            }
        }

        // Indexer using an OID friendly name or value.
        public Oid this[string oid]
        {
            get
            {
                // If we were passed the friendly name, retrieve the value String.
                string oidValue = OidLookup.ToOid(oid, OidGroup.All, fallBackToAllGroups: false);
                if (oidValue == null)
                {
                    oidValue = oid;
                }
                for (int i = 0; i < _count; i++)
                {
                    Oid entry = _oids[i];
                    if (entry.Value == oidValue)
                        return entry;
                }
                return null;
            }
        }

        public int Count => _count;

        public OidEnumerator GetEnumerator() => new OidEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_Index);
            if (index + Count > array.Length)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            for (int i = 0; i < Count; i++)
            {
                array.SetValue(this[i], index);
                index++;
            }
        }

        public void CopyTo(Oid[] array, int index)
        {
            // Need to do part of the argument validation ourselves as OidCollection throws
            // ArgumentOutOfRangeException where List<>.CopyTo() throws ArgumentException.

            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_Index);

            Array.Copy(_oids, 0, array, index, _count);
        }

        public bool IsSynchronized => false;

        public object SyncRoot => this;
    }
}
