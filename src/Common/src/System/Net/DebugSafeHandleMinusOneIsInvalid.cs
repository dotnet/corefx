// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;

using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net
{
#if DEBUG
    //
    // This is a helper class for debugging GC-ed handles that we define.
    // As a general rule normal code path should always destroy handles explicitly
    //
    internal abstract class DebugSafeHandleMinusOneIsInvalid : SafeHandleMinusOneIsInvalid
    {
        private string _trace;

        protected DebugSafeHandleMinusOneIsInvalid(bool ownsHandle) : base(ownsHandle)
        {
            Trace();
        }

        private void Trace()
        {
            _trace = "WARNING! GC-ed  >>" + this.GetType().FullName + "<< (should be explicitly closed) \r\n";
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Creating SafeHandle, type = " + this.GetType().FullName);
            }
#if TRACE_VERBOSE
            string stacktrace = Environment.StackTrace;
            _trace += stacktrace;
#endif //TRACE_VERBOSE
        }

        ~DebugSafeHandleMinusOneIsInvalid()
        {
            GlobalLog.SetThreadSource(ThreadKinds.Finalization);
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print(_trace);
            }
        }
    }
#endif // DEBUG
}
