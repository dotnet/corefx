// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
    public static class EventWaitHandleAcl
    {
        /// <summary>Creates a new <see cref="EventWaitHandle" /> instance, ensuring it is created with the specified event security.</summary>
        /// <param name="initialState"><see langword="true" /> to set the initial state to signaled if the named event is created as a result of this call; <see langword="false" /> to set it to nonsignaled.</param>
        /// <param name="mode">One of the enum values that determines whether the event resets automatically or manually.</param>
        /// <param name="name">The name of a system-wide synchronization event.</param>
        /// <param name="createdNew">When this method returns, it is set to <see langword="true" /> if a local event was created (that is, if name is <see langword="null" /> or an empty string) or if the specified named system event was created; <see langword="false" /> if the specified named system event already existed. This parameter is passed uninitialized.</param>
        /// <param name="eventSecurity">The Windows access control security to apply.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The length of the name exceeds the maximum limit.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eventSecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="mode" /> enum value was out of legal range.</exception>
        /// <exception cref="WaitHandleCannotBeOpenedException">An <see cref="EventWaitHandle" /> with system-wide name <paramref name="name" /> cannot be created. An <see cref="EventWaitHandle" /> of a different type might have the same name.</exception>
        public static EventWaitHandle Create(bool initialState, EventResetMode mode, string name, out bool createdNew, EventWaitHandleSecurity eventSecurity)
        {
            if (name != null && name.Length > Interop.Kernel32.MAX_PATH)
            {
                throw new ArgumentException(SR.Format(SR.Argument_WaitHandleNameTooLong, name));
            }

            if (eventSecurity == null)
            {
                throw new ArgumentNullException(nameof(eventSecurity));
            }

            bool isManualReset;
            switch (mode)
            {
                case EventResetMode.ManualReset:
                    isManualReset = true;
                    break;
                case EventResetMode.AutoReset:
                    isManualReset = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), SR.Format(SR.ArgumentOutOfRange_Enum));
            };

            SafeWaitHandle handle = CreateWaitHandle(initialState, isManualReset, name, out createdNew, eventSecurity);

            try
            {
                // This constructor is private
                // return new EventWaitHandle(handle);
                return null;
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }


        private static unsafe SafeWaitHandle CreateWaitHandle(bool initialState, bool isManualReset, string name, out bool createdNew, EventWaitHandleSecurity security)
        {
            SafeWaitHandle handle;

            fixed (byte* pSecurityDescriptor = security.GetSecurityDescriptorBinaryForm())
            {
                var secAttrs = new Interop.Kernel32.SECURITY_ATTRIBUTES
                {
                    nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES),
                    lpSecurityDescriptor = (IntPtr)pSecurityDescriptor
                };

                handle = Interop.Kernel32.CreateEvent(ref secAttrs, isManualReset, initialState, name);
                ValidateHandle(handle, name, out createdNew);
            }

            return handle;
        }

        private static void ValidateHandle(SafeWaitHandle handle, string name, out bool createdNew)
        {
            createdNew = false;

            if (handle.IsInvalid)
            {
                // We call this in netfx. Is it still needed at this point?
                handle.SetHandleAsInvalid();

                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode == Interop.Errors.ERROR_INVALID_HANDLE && string.IsNullOrEmpty(name))
                {
                    throw new WaitHandleCannotBeOpenedException(
                        SR.Format(SR.WaitHandleCannotBeOpenedException_InvalidHandle, name));
                }
                else if (errorCode != Interop.Errors.ERROR_ALREADY_EXISTS)
                {
                    throw Win32Marshal.GetExceptionForWin32Error(errorCode, name);
                }

                createdNew = errorCode != Interop.Errors.ERROR_ALREADY_EXISTS;
            }
        }
    }
}
