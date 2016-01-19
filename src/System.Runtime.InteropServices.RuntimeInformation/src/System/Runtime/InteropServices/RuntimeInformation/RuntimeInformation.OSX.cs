﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.InteropServices
{
    public static partial class RuntimeInformation
    {
        public static bool IsOSPlatform(OSPlatform osPlatform)
        {
            return OSPlatform.OSX == osPlatform;
        }
    }
}
