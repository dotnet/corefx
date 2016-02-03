// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

using System.Threading;

namespace System.Net.Security
{
#if DEBUG
    internal sealed class SecurityContextTokenHandle : DebugCriticalHandleZeroOrMinusOneIsInvalid
    {
#else
    internal sealed class SecurityContextTokenHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
#endif
        private int _disposed;

        private SecurityContextTokenHandle() : base()
        {
        }

        internal IntPtr DangerousGetHandle()
        {
            return handle;
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                if (Interlocked.Increment(ref _disposed) == 1)
                {
                    return Interop.mincore.CloseHandle(handle);
                }
            }
            return true;
        }
    }
}
