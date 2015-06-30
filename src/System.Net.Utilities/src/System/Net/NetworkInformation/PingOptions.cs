// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//determines which options will be used for sending icmp requests, as well as what options
//were set in the returned icmp reply.

namespace System.Net.NetworkInformation
{
    // Represent the possible ip options used for the icmp packet
    public class PingOptions
    {
        const int DontFragmentFlag = 2;
        int ttl = 128;
        bool dontFragment;

        internal PingOptions(IPOptions options)
        {
            this.ttl = options.ttl;
            this.dontFragment = ((options.flags & DontFragmentFlag) > 0 ? true : false);
        }

        public PingOptions(int ttl, bool dontFragment)
        {
            if (ttl <= 0)
            {
                throw new ArgumentOutOfRangeException("ttl");
            }

            this.ttl = ttl;
            this.dontFragment = dontFragment;
        }

        public PingOptions()
        {
        }

        public int Ttl
        {
            get
            {
                return ttl;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                ttl = value; //useful to discover routes
            }
        }

        public bool DontFragment
        {
            get
            {
                return dontFragment;
            }
            set
            {
                dontFragment = value;  //useful for discovering mtu
            }
        }
    }
}
