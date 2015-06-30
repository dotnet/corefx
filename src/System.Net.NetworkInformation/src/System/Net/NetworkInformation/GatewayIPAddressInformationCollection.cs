// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
    public class GatewayIPAddressInformationCollection : ICollection<GatewayIPAddressInformation>
    {
        private Collection<GatewayIPAddressInformation> _addresses = new Collection<GatewayIPAddressInformation>();

        protected internal GatewayIPAddressInformationCollection()
        {
        }

        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.CopyTo"]/*' />
        public virtual void CopyTo(GatewayIPAddressInformation[] array, int offset)
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

        public virtual GatewayIPAddressInformation this[int index]
        {
            get
            {
                return (GatewayIPAddressInformation)_addresses[index];
            }
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Add"]/*' />
        public virtual void Add(GatewayIPAddressInformation address)
        {
            throw new NotSupportedException(SR.net_collection_readonly);
        }



        internal void InternalAdd(GatewayIPAddressInformation address)
        {
            _addresses.Add(address);
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Contains"]/*' />
        public virtual bool Contains(GatewayIPAddressInformation address)
        {
            return _addresses.Contains(address);
        }


        public virtual IEnumerator<GatewayIPAddressInformation> GetEnumerator()
        {
            return (IEnumerator<GatewayIPAddressInformation>)_addresses.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        /// <include file='doc\HttpListenerPrefixCollection.uex' path='docs/doc[@for="HttpListenerPrefixCollection.Remove"]/*' />
        public virtual bool Remove(GatewayIPAddressInformation address)
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
