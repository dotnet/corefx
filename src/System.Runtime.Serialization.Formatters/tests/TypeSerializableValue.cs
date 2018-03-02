// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace System.Runtime.Serialization.Formatters.Tests
{
    public readonly struct TypeSerializableValue
    {
        public readonly string Base64Blob;
        public readonly TargetFrameworkMoniker Platform;

        public TypeSerializableValue(string base64Blob, TargetFrameworkMoniker platform)
        {
            Base64Blob = base64Blob;
            Platform = platform;
        }

        public static int GetPlatformIndex(TypeSerializableValue[] blobs)
        {
            var blobList = blobs.ToList();
            int index;
            // .NET Framework
            if (PlatformDetection.IsFullFramework)
            {
                // Check if a specialized blob for >=net471 is present and return if found.
                // If no newer blob for >=net471 is present use existing one. 
                // If no netfx blob is present then -1 will be returned.
                index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx471);
                return PlatformDetection.IsNetfx471OrNewer && (index >= 0) ? index : blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx);
            }

            // .NET Core
            index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netcoreapp21);
            return PlatformDetection.IsNetCore && (index >= 0) ? index : blobList.FindIndex((b => b.Platform == TargetFrameworkMoniker.netcoreapp));
        }
    }

    public enum TargetFrameworkMoniker
    {
        netfx,
        netfx471,
        netcoreapp,
        netcoreapp21,
    }
}
