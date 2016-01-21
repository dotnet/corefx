// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net
{
    [Flags]
    internal enum ContextFlagsPal
    {
        Zero = 0,
        Delegate = 0x00000001,
        MutualAuth = 0x00000002,
        ReplayDetect = 0x00000004,
        SequenceDetect = 0x00000008,
        Confidentiality = 0x00000010,
        UseSessionKey = 0x00000020,
        AllocateMemory = 0x00000100,
        Connection = 0x00000800,
        InitExtendedError = 0x00004000,
        AcceptExtendedError = 0x00008000,
        InitStream = 0x00008000,
        AcceptStream = 0x00010000,
        InitIntegrity = 0x00010000,
        AcceptIntegrity = 0x00020000,
        InitManualCredValidation = 0x00080000,
        InitUseSuppliedCreds = 0x00000080,
        InitIdentify = 0x00020000,
        AcceptIdentify = 0x00080000,
        ProxyBindings = 0x04000000,
        AllowMissingBindings = 0x10000000,
        UnverifiedTargetName = 0x20000000,
    }
}
