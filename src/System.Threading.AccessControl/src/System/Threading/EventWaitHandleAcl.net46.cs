// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.AccessControl;

namespace System.Threading
{
    public static class EventWaitHandleAcl
    {
        public static EventWaitHandle Create(
            bool initialState,
            EventResetMode mode,
            string name,
            out bool createdNew,
            EventWaitHandleSecurity eventSecurity)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(SR.Format(SR.Argument_CannotBeNullOrEmpty), nameof(name));
            }
            if (name.Length > Interop.Kernel32.MAX_PATH)
            {
                throw new ArgumentException(SR.Format(SR.Argument_WaitHandleNameTooLong, name), nameof(name));
            }
            if (eventSecurity == null)
            {
                throw new ArgumentNullException(nameof(eventSecurity));
            }
            if (mode != EventResetMode.AutoReset && mode != EventResetMode.ManualReset)
            {
                throw new ArgumentOutOfRangeException(nameof(mode), SR.Format(SR.ArgumentOutOfRange_Enum));
            }

            return new EventWaitHandle(initialState, mode, name, out createdNew, eventSecurity);
        }
    }
}
