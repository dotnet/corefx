// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    // OSX implementation of NetworkChange
    public class NetworkChange
    {
        static public event NetworkAddressChangedEventHandler NetworkAddressChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }
    }
}
