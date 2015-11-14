// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Threading.Tasks.Tests
{
    internal static class Workloads
    {
        public static readonly TimeSpan VeryLight = TimeSpan.FromTicks(100);
        public static readonly TimeSpan Light = TimeSpan.FromTicks(1000);
        public static readonly TimeSpan Medium = TimeSpan.FromMilliseconds(1);
        public static readonly TimeSpan Heavy = TimeSpan.FromMilliseconds(5);
        public static readonly TimeSpan VeryHeavy = TimeSpan.FromMilliseconds(10);
    }
}
