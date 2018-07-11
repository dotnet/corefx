﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.Formatters.Tests
{
    internal static class PlatformExtensions
    {
        public static bool IsNetfxPlatform(this TargetFrameworkMoniker targetFrameworkMoniker)
        {
            switch (targetFrameworkMoniker)
            {
                case TargetFrameworkMoniker.netfx461:
                case TargetFrameworkMoniker.netfx471:
                case TargetFrameworkMoniker.netfx472:
                    return true;
            }

            return false;
        }

        public static int GetPlatformIndex(this TypeSerializableValue[] blobs)
        {
            bool IsNetfx472PatchedOrNewer()
            {
                if (!PlatformDetection.IsNetfx472OrNewer)
                    return false;

                // .NET Framework 4.7.3062.0 is min patched.
                string versionRaw = RuntimeInformation.FrameworkDescription.Replace(".NET Framework", "").Trim();
                if (Version.TryParse(versionRaw, out Version version))
                {
                    return version.Minor >= 7 && version.Build >= 3062;
                }

                return false;
            }

            List<TypeSerializableValue> blobList = blobs.ToList();

            // .NET Framework
            if (PlatformDetection.IsFullFramework)
            {
                // Check if a specialized blob for >=netfx472 is present and return if found.
                if (IsNetfx472PatchedOrNewer())
                {
                    int index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx472);

                    if (index >= 0)
                        return index;
                }

                // Check if a specialized blob for >=netfx471 is present and return if found.
                if (PlatformDetection.IsNetfx471OrNewer)
                {
                    int index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx471);

                    if (index >= 0)
                        return index;
                }

                // If no newer blob for >=netfx471 is present use existing one. 
                // If no netfx blob is present then -1 will be returned.
                return blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx461);
            }

            // .NET Core
            if (PlatformDetection.IsNetCore)
            {
                // Check if a specialized blob for >=netcoreapp2.1 is present and return if found.
                int index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netcoreapp21);

                if (index >= 0)
                    return index;

                // If no newer blob for >=netcoreapp2.1 is present use existing one.
                // If no netfx blob is present then -1 will be returned.
                return blobList.FindIndex((b => b.Platform == TargetFrameworkMoniker.netcoreapp20));
            }

            return -1;
        }
    }
}
