// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
    public class IPAddressInformationCollection : ICollection<IPAddressInformation>
    {
        private Collection<IPAddressInformation> _addresses = new Collection<IPAddressInformation>();

        internal IPAddressInformationCollection()
        {
        }

        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.CopyTo"]/*' />
        public virtual void CopyTo(IPAddressInformation[] array, int offset)
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


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Add"]/*' />
        public virtual void Add(IPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }



        internal void InternalAdd(IPAddressInformation address)
        {
            _addresses.Add(address);
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Contains"]/*' />
        public virtual bool Contains(IPAddressInformation address)
        {
            return _addresses.Contains(address);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual IEnumerator<IPAddressInformation> GetEnumerator()
        {
            return (IEnumerator<IPAddressInformation>)_addresses.GetEnumerator();
        }


        public virtual IPAddressInformation this[int index]
        {
            get
            {
                return (IPAddressInformation)_addresses[index];
            }
        }

        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Remove"]/*' />
        public virtual bool Remove(IPAddressInformation address)
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
