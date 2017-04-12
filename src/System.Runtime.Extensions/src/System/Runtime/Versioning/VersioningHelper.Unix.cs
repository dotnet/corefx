// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Versioning
{
#if !MONO
    public static partial class VersioningHelper
#else
    internal static class VersioningHelperUnix
#endif
    {
        internal static int GetCurrentProcessId() => Interop.Sys.GetPid();
    }
}
