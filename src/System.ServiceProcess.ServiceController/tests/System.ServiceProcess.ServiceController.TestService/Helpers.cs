﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceProcess.Tests
{
    public enum PipeMessageByteCode
    {
        Start = 0,
        Continue = 1,
        Pause = 2,
        Stop = 3,
        OnCustomCommand = 4,
        ExceptionThrown = 5
    };
}
