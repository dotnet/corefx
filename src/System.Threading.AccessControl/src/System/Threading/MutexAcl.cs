// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
    public static class MutexAcl
    {
        /// <summary>Creates a new <see cref="Mutex" /> instance, indicating whether the calling thread should have initial ownership of the mutex, the name of the system mutex, a boolean that, when the method returns, indicates whether the calling thread was granted initial ownership of the mutex, allowing specifying the mutex security.</summary>
        /// <param name="initiallyOwned"><see langword="true" /> to give the calling thread initial ownership of the named system mutex if the named system mutex is created as a result of this call; otherwise, <see langword="false" />.</param>
        /// <param name="name">The name of the system mutex.</param>
        /// <param name="createdNew">When this method returns, it is set to <see langword="true" /> if a local mutex was created (that is, if name is <see langword="null" /> or an empty string) or if the specified named system mutex was created; <see langword="false" /> if the specified named system mutex already existed. This parameter is passed uninitialized.</param>
        /// <param name="mutexSecurity">An object that represents the access control security to be applied to the named system mutex.</param>
        /// <returns>An object that represents the named system mutex.</returns>
        /// <exception cref="ArgumentException">Argument cannot be null or empty.
        /// -or-
        /// The length of the name exceeds the maximum limit.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="mutexSecurity" /> is <see langword="null" />.</exception>
        /// <exception cref="WaitHandleCannotBeOpenedException">A mutex handle with the system-wide <paramref name="name" /> cannot be created. A mutex handle of a different type might have the same name.</exception>
        public static unsafe Mutex Create(bool initiallyOwned, string name, out bool createdNew, MutexSecurity mutexSecurity)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(SR.Format(SR.Argument_CannotBeNullOrEmpty), nameof(name));
            }
            if (name.Length > Interop.Kernel32.MAX_PATH)
            {
                throw new ArgumentException(SR.Format(SR.Argument_WaitHandleNameTooLong, name), nameof(name));
            }
            if (mutexSecurity == null)
            {
                throw new ArgumentNullException(nameof(mutexSecurity));
            }

            fixed (byte* pSecurityDescriptor = mutexSecurity.GetSecurityDescriptorBinaryForm())
            {
                var secAttrs = new Interop.Kernel32.SECURITY_ATTRIBUTES
                {
                    nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES),
                    lpSecurityDescriptor = (IntPtr)pSecurityDescriptor
                };

                using SafeWaitHandle mutexHandle = Interop.Kernel32.CreateMutex(ref secAttrs, initiallyOwned, name);

                ValidateMutexHandle(mutexHandle, name, out createdNew);

                try
                {
                    return Mutex.OpenExisting(name);
                }
                finally
                {
                    // Close our handle as the Mutex will have it's own.
                    mutexHandle.Dispose();
                }
            }
        }

        private static void ValidateMutexHandle(SafeWaitHandle mutexHandle, string name, out bool createdNew)
        {
            int errorCode = Marshal.GetLastWin32Error();

            if (mutexHandle.IsInvalid)
            {
                mutexHandle.SetHandleAsInvalid();

                if (errorCode == Interop.Errors.ERROR_FILENAME_EXCED_RANGE)
                {
                    // On Unix, length validation is done by CoreCLR's PAL after converting to utf-8
                    throw new ArgumentException(SR.Argument_WaitHandleNameTooLong, nameof(name));
                }

                if (errorCode == Interop.Errors.ERROR_INVALID_HANDLE)
                {
                    throw new WaitHandleCannotBeOpenedException(SR.Format(SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
                }

                throw Win32Marshal.GetExceptionForWin32Error(errorCode, name);
            }

            createdNew = (errorCode != Interop.Errors.ERROR_ALREADY_EXISTS);
        }
    }
}
