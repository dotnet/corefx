// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Win32
{
#if REGISTRY_ASSEMBLY
    public
#else
    internal
#endif
    enum RegistryValueKind
    {
        String = Interop.Kernel32.RegistryValues.REG_SZ,
        ExpandString = Interop.Kernel32.RegistryValues.REG_EXPAND_SZ,
        Binary = Interop.Kernel32.RegistryValues.REG_BINARY,
        DWord = Interop.Kernel32.RegistryValues.REG_DWORD,
        MultiString = Interop.Kernel32.RegistryValues.REG_MULTI_SZ,
        QWord = Interop.Kernel32.RegistryValues.REG_QWORD,
        Unknown = 0,                          // REG_NONE is defined as zero but BCL
        None = unchecked((int)0xFFFFFFFF), //  mistakenly overrode this value.  
    }   // Now instead of using Interop.Kernel32.RegistryValues.REG_NONE we use "-1".
}
