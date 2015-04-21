// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.InteropServices
{
    public static class RuntimeInfo
    {
        public static bool IsOSPlatform(OSName osName)
        {
            if (osName.Equals(OSName.Linux))
                return true;
            return false;
        }
    }
}
