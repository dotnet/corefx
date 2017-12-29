// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System
{
    public static partial class PlatformDetection
    {
        public static bool TargetsNetFx452OrLower => false;
        public static bool IsNetfx462OrNewer() { return false; }
        public static bool IsNetfx470OrNewer() { return false; }
        public static bool IsNetfx471OrNewer() { return false; }
    }
}
