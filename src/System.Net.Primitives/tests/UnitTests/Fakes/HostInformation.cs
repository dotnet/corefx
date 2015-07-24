// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    public static class HostInformation
    {
        public static string DomainName
        {
            get
            {
                var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

                return properties.DomainName;
            }
        }
    }
}
