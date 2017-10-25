// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Management.Tests
{
    public static class WmiTestHelper
    {
        private static readonly bool s_ElevatedAndNotNanoServer = AdminHelpers.IsProcessElevated() && PlatformDetection.IsNotWindowsNanoServer;
        private static readonly string s_systemDriveId = Environment.GetEnvironmentVariable("SystemDrive");

        public static string Namespace => "root/WmiEBvt";
        public static string SystemDriveId => s_systemDriveId;
        public static bool IsElevatedAndNotNanoServer => s_ElevatedAndNotNanoServer;
    }
}
