// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.Versioning;

namespace System
{
    public static partial class PlatformDetection
    {
        private static string FrameworkName => AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
        private static Version TargetVersion => new FrameworkName(FrameworkName).Version;

        public static bool IsFullFramework452OrDownTargeting => TargetVersion.CompareTo(new Version(4, 5, 3, 0)) < 0;
    }
}
