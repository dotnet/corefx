// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Media.Test
{
    public static class Helpers
    {
        public const string IsSoundPlaySupported = nameof(Helpers) + "." + nameof(GetIsSoundPlaySupported);

        public static bool GetIsSoundPlaySupported() => PlatformDetection.IsSoundPlaySupported;
    }
}
