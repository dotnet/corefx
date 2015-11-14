// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Threading.Tasks.Tests
{
    internal static class Waits
    {
        public static readonly TimeSpan Infinite = TimeSpan.FromMilliseconds(-1);
        public static readonly TimeSpan Instant = TimeSpan.Zero;
        public static readonly TimeSpan Short = TimeSpan.FromMilliseconds(2);
        public static readonly TimeSpan Long = TimeSpan.FromMilliseconds(10);
    }
}
