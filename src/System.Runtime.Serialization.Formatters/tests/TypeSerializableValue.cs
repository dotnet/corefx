// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace System.Runtime.Serialization.Formatters.Tests
{
    public class TypeSerializableValue
    {
        public TypeSerializableValue(string base64Blob, TargetFrameworkMoniker platform)
        {
            Base64Blob = base64Blob;
            Platform = platform;
        }

        public string Base64Blob { get; }

        public TargetFrameworkMoniker Platform { get; }

        public static int GetPlatformIndex(TypeSerializableValue[] blobs)
        {
            var blobList = blobs.ToList();

            // .NET Framework
            if (PlatformDetection.IsFullFramework)
            {
                if (PlatformDetection.IsNetfx471OrNewer)
                {
                    // Check if a specialized blob for >=net471 is present and return if found.
                    int index = blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx471);
                    if (index >= 0)
                        return index;
                }

                // If no newer blob for >=net471 is present use existing one. 
                // If no netfx blob is present then -1 will be returned.
                return blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netfx);
            }

            // .NET Core
            return PlatformDetection.IsNetCore ? blobList.FindIndex(b => b.Platform == TargetFrameworkMoniker.netcoreapp21) : -1;
        }
    }

    public enum TargetFrameworkMoniker
    {
        netfx = 0,
        netfx471 = 1,
        netcoreapp = 2,
        netcoreapp21 = 3,
    }
}

