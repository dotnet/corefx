// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace System.Diagnostics
{
    public enum TraceEventType
    {
        Critical = 0x01,
        Error = 0x02,
        Warning = 0x04,
        Information = 0x08,
        Verbose = 0x10
    }
}

