// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class mincore
    {
        internal static bool DeleteVolumeMountPoint(string mountPoint) 
        { 
            // DeleteVolumeMountPointW is not available to store apps. 
            // The expectation is that no store app would even have permission 
            // to call this from the app container 
            throw new UnauthorizedAccessException(); 
        } 
    }
}
