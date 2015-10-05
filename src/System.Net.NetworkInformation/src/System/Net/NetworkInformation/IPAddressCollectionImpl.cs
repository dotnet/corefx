using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.NetworkInformation
{
    internal class IPAddressCollectionImpl : IPAddressCollection
    {
        private List<IPAddress> _addresses;

        public IPAddressCollectionImpl(IEnumerable<IPAddress> addresses)
        {
            _addresses = new List<IPAddress>(addresses);
        }

        public override bool IsReadOnly { get { return true; } }

        public override IEnumerator<IPAddress> GetEnumerator()
        {
            return _addresses.GetEnumerator();
        }

        public override IPAddress this[int index]
        {
            get { return _addresses[index]; }
        }

        public override int Count
        {
            get { return _addresses.Count; }
        }

        public override bool Contains(IPAddress address)
        {
            return _addresses.Contains(address);
        }

        public override void CopyTo(IPAddress[] array, int offset)
        {
            _addresses.CopyTo(array, offset);
        }
    }
}
