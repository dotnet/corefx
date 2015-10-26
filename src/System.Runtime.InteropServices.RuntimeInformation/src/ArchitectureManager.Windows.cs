// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

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
                Architecture processArch = ProcessArchCore;

                if (Architecture.BIT64 == processArch)
                    return Architecture.BIT64;

                if (Architecture.ARM32 == processArch)
                    return Architecture.ARM32;

                bool isWow64 = false;
                SafeProcessHandle handle = Process.GetCurrentProcess().SafeHandle;
                bool result = Interop.mincore.IsWow64Process(handle, ref isWow64) && isWow64;

                return result ? Architecture.BIT64 : Architecture.BIT32;
            }
        }

        private static bool IsARM
        {
            get
            {
                Interop.mincore.SYSTEM_INFO sysInfo = new Interop.mincore.SYSTEM_INFO();
                Interop.mincore.GetNativeSystemInfo(out sysInfo);
                if ((short)Interop.mincore.ProcessorArchitecture.ARM == sysInfo.wProcessorArchitecture)
                    return true;

                return false;
            }
        }
    }
}
