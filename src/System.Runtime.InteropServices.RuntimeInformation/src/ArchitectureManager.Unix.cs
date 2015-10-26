// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace System.Runtime.InteropServices
{
    internal partial struct ArchitectureManager
    {
        private static Architecture ProcessArchCore
        {
            get
            {
                if (IsARM)
                {
                    return Architecture.ARM32;
                }
                else
                {
                    return _is32BitProcess ? Architecture.BIT32 : Architecture.BIT64;
                }
            }
        }

        private static Architecture OSArchCore
        {
            get
            {
                string arch = null;
                int result = Interop.Sys.UName(out arch);
                if (result >= 0 && arch != null)
                {
                    if (arch.IndexOf("arm", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Architecture.ARM32;

                    if (arch.IndexOf("x86_64", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Architecture.BIT64;

                    return Architecture.BIT32;
                }
                else
                {
                    // uname failed, returned -1 or arch is not set.
                    throw new Win32Exception();
                }
            }
        }

        private static bool IsARM
        {
            get
            {
                string osArch = null;
                if (Interop.Sys.UName(out osArch) >= 0 && osArch != null)
                {
                    if (osArch.IndexOf("arm", StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                    else
                        return false;
                }
                else
                {
                    // UName failed, returned -1 or arch is not set.
                    throw new Win32Exception();
                }
            }
        }
    }
}
