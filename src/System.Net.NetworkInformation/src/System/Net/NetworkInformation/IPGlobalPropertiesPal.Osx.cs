﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal static class IPGlobalPropertiesPal
    {
        public static IPGlobalProperties GetIPGlobalProperties()
        {
            return new OsxIPGlobalProperties();
        }
    }
}
