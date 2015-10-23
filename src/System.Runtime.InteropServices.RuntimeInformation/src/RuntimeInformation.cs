// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.InteropServices
{
    public static partial class RuntimeInformation
    {
        public static bool Is64BitOS()
        {
            return Architecture.BIT64 == ArchitectureManager.OSArchitecture;
        }

        public static bool Is64BitProcess()
        {
            return Architecture.BIT64 == ArchitectureManager.ProcessArchitecture;
        }

        public static bool IsArmProcessor()
        {
            return Architecture.ARM32 == ArchitectureManager.OSArchitecture;
        }
    }
}
