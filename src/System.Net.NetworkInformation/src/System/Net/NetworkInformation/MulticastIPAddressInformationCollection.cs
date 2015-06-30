// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
    public class MulticastIPAddressInformationCollection : ICollection<MulticastIPAddressInformation>
    {
        Collection<MulticastIPAddressInformation> addresses = new Collection<MulticastIPAddressInformation>();

        protected internal MulticastIPAddressInformationCollection()
        {
        }

        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.CopyTo"]/*' />
        public virtual void CopyTo(MulticastIPAddressInformation[] array, int offset)
        {
            addresses.CopyTo(array, offset);
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Count"]/*' />
        public virtual int Count
        {
            get
            {
                return addresses.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return true;
            }
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Add"]/*' />
        public virtual void Add(MulticastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }



        internal void InternalAdd(MulticastIPAddressInformation address)
        {
            addresses.Add(address);
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Contains"]/*' />
        public virtual bool Contains(MulticastIPAddressInformation address)
        {
            return addresses.Contains(address);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual IEnumerator<MulticastIPAddressInformation> GetEnumerator()
        {
            return (IEnumerator<MulticastIPAddressInformation>)addresses.GetEnumerator();
        }


        public virtual MulticastIPAddressInformation this[int index]
        {
            get
            {
                return (MulticastIPAddressInformation)addresses[index];
            }
        }

        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Remove"]/*' />
        public virtual bool Remove(MulticastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }

        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Clear"]/*' />
        public virtual void Clear()
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }
    }
}
