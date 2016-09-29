// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// Ported from EventWaitHandle.cs, Mutex.cs, Semaphore.cs and made extension methods (or renamed statics, where necessary) to allow 
// extending the class without System.Threading needing to rely on System.Security.AccessControl

using System;
using System.Security.AccessControl;
using System.Diagnostics.Contracts;

namespace System.Threading
{
    public static class ThreadingAclExtensions
    {
        [System.Security.SecuritySafeCritical]  // auto-generated
        public static EventWaitHandleSecurity GetAccessControl(this EventWaitHandle handle)
        {
            return new EventWaitHandleSecurity(handle.GetSafeWaitHandle(), AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public static void SetAccessControl(this EventWaitHandle handle, EventWaitHandleSecurity eventSecurity)
        {
            if (eventSecurity == null)
                throw new ArgumentNullException(nameof(eventSecurity));
            Contract.EndContractBlock();

            eventSecurity.Persist(handle.GetSafeWaitHandle());
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public static MutexSecurity GetAccessControl(this Mutex mutex)
        {
            return new MutexSecurity(mutex.GetSafeWaitHandle(), AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public static void SetAccessControl(this Mutex mutex, MutexSecurity mutexSecurity)
        {
            if (mutexSecurity == null)
                throw new ArgumentNullException(nameof(mutexSecurity));
            Contract.EndContractBlock();

            mutexSecurity.Persist(mutex.GetSafeWaitHandle());
        }

        public static SemaphoreSecurity GetAccessControl(this Semaphore semaphore)
        {
            return new SemaphoreSecurity(semaphore.GetSafeWaitHandle(), AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
        }

        public static void SetAccessControl(this Semaphore semaphore, SemaphoreSecurity semaphoreSecurity)
        {
            if (semaphoreSecurity == null)
                throw new ArgumentNullException(nameof(semaphoreSecurity));

            semaphoreSecurity.Persist(semaphore.GetSafeWaitHandle());
        }
    }
}
