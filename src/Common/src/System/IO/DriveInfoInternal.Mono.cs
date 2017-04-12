// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    /// <summary>Contains internal volume helpers that are shared between many projects.</summary>
    internal static partial class DriveInfoInternal
    {
        public static string[] GetLogicalDrives()
        {
            if (PlatformHelper.IsWin32)
            {
                return DriveInfoInternalWin32.GetLogicalDrives();
            }
            else if (PlatformHelper.IsUnix)
            {
                return DriveInfoInternalUnix.GetLogicalDrives();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
