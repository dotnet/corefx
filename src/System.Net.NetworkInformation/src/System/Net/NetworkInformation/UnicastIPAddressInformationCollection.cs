// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
    public class UnicastIPAddressInformationCollection : ICollection<UnicastIPAddressInformation>
    {
        private Collection<UnicastIPAddressInformation> _addresses = new Collection<UnicastIPAddressInformation>();


        protected internal UnicastIPAddressInformationCollection()
        {
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.CopyTo"]/*' />
        public virtual void CopyTo(UnicastIPAddressInformation[] array, int offset)
        {
            _addresses.CopyTo(array, offset);
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Count"]/*' />
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


        public virtual void Add(UnicastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }


        internal void InternalAdd(UnicastIPAddressInformation address)
        {
            _addresses.Add(address);
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Contains"]/*' />
        public virtual bool Contains(UnicastIPAddressInformation address)
        {
            return _addresses.Contains(address);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        public virtual IEnumerator<UnicastIPAddressInformation> GetEnumerator()
        {
            return (IEnumerator<UnicastIPAddressInformation>)_addresses.GetEnumerator();
        }



        // Consider removing.
        public virtual UnicastIPAddressInformation this[int index]
        {
            get
            {
                return (UnicastIPAddressInformation)_addresses[index];
            }
        }



        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Remove"]/*' />
        public virtual bool Remove(UnicastIPAddressInformation address)
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
