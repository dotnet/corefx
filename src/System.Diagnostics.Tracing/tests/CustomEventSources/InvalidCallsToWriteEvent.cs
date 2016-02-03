// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Tracing;

// We wish to test both Microsoft.Diagnostics.Tracing (Nuget)
// and System.Diagnostics.Tracing (Framewwork), we use this Ifdef make each kind 

namespace SdtEventSources
{
    public sealed class InvalidCallsToWriteEventEventSource : EventSource
    {
        public void WriteTooFewArgs(int m, int n)
        {
            WriteEvent(1, m);
        }

        public void WriteTooManyArgs(string msg)
        {
            WriteEvent(2, msg, "-");
        }
    }
}
