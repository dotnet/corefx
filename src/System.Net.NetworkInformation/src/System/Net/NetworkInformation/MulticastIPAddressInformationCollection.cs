// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
    public class MulticastIPAddressInformationCollection : ICollection<MulticastIPAddressInformation>
    {
        private readonly Collection<MulticastIPAddressInformation> _addresses = new Collection<MulticastIPAddressInformation>();

        protected internal MulticastIPAddressInformationCollection()
        {
        }

        public virtual void CopyTo(MulticastIPAddressInformation[] array, int offset)
        {
            _addresses.CopyTo(array, offset);
        }

        public virtual int Count
        {
            get
            {
                return _addresses.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public virtual void Add(MulticastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }

        internal void InternalAdd(MulticastIPAddressInformation address)
        {
            _addresses.Add(address);
        }

        public virtual bool Contains(MulticastIPAddressInformation address)
        {
            return _addresses.Contains(address);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual IEnumerator<MulticastIPAddressInformation> GetEnumerator()
        {
            return _addresses.GetEnumerator();
        }

        public virtual MulticastIPAddressInformation this[int index]
        {
            get
            {
                return _addresses[index];
            }
        }

        public virtual bool Remove(MulticastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }

        public virtual void Clear()
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }
    }
}
