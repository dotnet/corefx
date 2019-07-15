// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

namespace System.Drawing
{
    public static class Helpers
    {
        public const string IsDrawingSupported = nameof(Helpers) + "." + nameof(GetIsDrawingSupported);

        public static bool GetIsDrawingSupported() => /* ActiveIssue(24525) */ PlatformDetection.IsNotRedHatFamily6 && PlatformDetection.IsDrawingSupported;
    }
}
