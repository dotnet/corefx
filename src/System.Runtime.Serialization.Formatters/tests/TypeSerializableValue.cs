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
                // Check if a specialized blob for >=netfx471 is present and return if found.
                if (PlatformDetection.IsNetfx471OrNewer)
                {
                    index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx471);

                    if (index >= 0)
                        return index;
                }
                    
                // If no newer blob for >=netfx471 is present use existing one. 
                // If no netfx blob is present then -1 will be returned.
                return blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx);
            }

            // .NET Core
            if (PlatformDetection.IsNetCore)
            {
                // Check if a specialized blob for >=netcoreapp2.1 is present and return if found.
                index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netcoreapp21);

                if (index >= 0)
                    return index;
            }

            // If no newer blob for >=netcoreapp2.1 is present use existing one. 
            // If no netfx blob is present then -1 will be returned.
            return blobList.FindIndex((b => b.Platform == TargetFrameworkMoniker.netcoreapp));
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
