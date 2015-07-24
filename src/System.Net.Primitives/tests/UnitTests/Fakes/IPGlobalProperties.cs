// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    public static class IPGlobalProperties
    {
        public static System.Net.NetworkInformation.IPGlobalProperties InternalGetIPGlobalProperties()
        {
            return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
        }
    }
}
