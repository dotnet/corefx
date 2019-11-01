// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
    public static class EventWaitHandleAcl
    {
        /// <summary>Creates a new <see cref="EventWaitHandle" /> instance, ensuring it is created with the specified event security.</summary>
        /// <param name="initialState"><see langword="true" /> to set the initial state to signaled if the named event is created as a result of this call; <see langword="false" /> to set it to non-signaled.</param>
        /// <param name="mode">One of the enum values that determines whether the event resets automatically or manually.</param>
        /// <param name="name">The name, if the event is a system-wide synchronization event; otherwise, <see langword="null" /> or an empty string.</param>
        /// <param name="createdNew">When this method returns, it is set to <see langword="true" /> if a local event was created (that is, if name is <see langword="null" /> or an empty string) or if the specified named system event was created; <see langword="false" /> if the specified named system event already existed. This parameter is passed uninitialized.</param>
        /// <param name="eventSecurity">The Windows access control security to apply.</param>
        /// <returns>An object that represents the named event wait handle.</returns>
        /// <exception cref="ArgumentException">The length of the name exceeds the maximum limit.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eventSecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="mode" /> enum value was out of legal range.</exception>
        /// <exception cref="DirectoryNotFoundException">Could not find a part of the path specified in <paramref name="name" />.</exception>
        /// <exception cref="WaitHandleCannotBeOpenedException">A system-wide synchronization event with the provided <paramref name="name" /> was not found.
        /// -or-
        /// An <see cref="EventWaitHandle" /> with system-wide name <paramref name="name" /> cannot be created. An <see cref="EventWaitHandle" /> of a different type might have the same name.</exception>
        public static unsafe EventWaitHandle Create(bool initialState, EventResetMode mode, string name, out bool createdNew, EventWaitHandleSecurity eventSecurity)
        {
            if (name.Length > Interop.Kernel32.MAX_PATH)
            {
                throw new ArgumentException(SR.Format(SR.Argument_WaitHandleNameTooLong, name), nameof(name));
            }
            if (eventSecurity == null)
            {
                throw new ArgumentNullException(nameof(eventSecurity));
            }

            bool isManualReset = mode switch
            {
                EventResetMode.ManualReset => true,
                EventResetMode.AutoReset => false,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), SR.Format(SR.ArgumentOutOfRange_Enum))
            };

            fixed (byte* pSecurityDescriptor = eventSecurity.GetSecurityDescriptorBinaryForm())
            {
                var secAttrs = new Interop.Kernel32.SECURITY_ATTRIBUTES
                {
                    nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES),
                    lpSecurityDescriptor = (IntPtr)pSecurityDescriptor
                };

                SafeWaitHandle handle = Interop.Kernel32.CreateEvent(ref secAttrs, isManualReset, initialState, name);

                ValidateHandle(handle, name, out createdNew);

                var ewh = new EventWaitHandle(initialState, mode);
                var old = ewh.SafeWaitHandle;
                ewh.SafeWaitHandle = handle;
                old.Dispose();

                return ewh;
            }
        }

        private static void ValidateHandle(SafeWaitHandle handle, string name, out bool createdNew)
        {
            int errorCode = Marshal.GetLastWin32Error();

            if (handle.IsInvalid)
            {
                handle.SetHandleAsInvalid();

                if (!string.IsNullOrEmpty(name) && errorCode == Interop.Errors.ERROR_INVALID_HANDLE)
                    throw new WaitHandleCannotBeOpenedException(SR.Format(SR.WaitHandleCannotBeOpenedException_InvalidHandle, name));

                throw Win32Marshal.GetExceptionForWin32Error(errorCode, name);
            }

            createdNew = (errorCode != Interop.Errors.ERROR_ALREADY_EXISTS);
        }
    }
}
