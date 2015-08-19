// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System
{
    // Tracking with DevDiv2# 1072696
    internal static class GCExtensions
    {
        public static int GetGeneration(object obj)
        {
            return GC.MaxGeneration;
        }
    }
}

