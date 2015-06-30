// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//determines which options will be used for sending icmp requests, as well as what options
//were set in the returned icmp reply.

namespace System.Net.NetworkInformation
{
    // Represent the possible ip options used for the icmp packet
    public class PingOptions
    {
        private const int DontFragmentFlag = 2;
        private int _ttl = 128;
        private bool _dontFragment;

        internal PingOptions(IPOptions options)
        {
            _ttl = options.ttl;
            _dontFragment = ((options.flags & DontFragmentFlag) > 0 ? true : false);
        }

        public PingOptions(int ttl, bool dontFragment)
        {
            if (ttl <= 0)
            {
                throw new ArgumentOutOfRangeException("ttl");
            }

            _ttl = ttl;
            _dontFragment = dontFragment;
        }

        public PingOptions()
        {
        }

        public int Ttl
        {
            get
            {
                return _ttl;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _ttl = value; //useful to discover routes
            }
        }

        public bool DontFragment
        {
            get
            {
                return _dontFragment;
            }
            set
            {
                _dontFragment = value;  //useful for discovering mtu
            }
        }
    }
}
