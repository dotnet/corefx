// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.AccessControl;

namespace System.Threading
{
    public static class MutexAcl
    {
        public static Mutex Create(bool initiallyOwned, string name, out bool createdNew, MutexSecurity mutexSecurity)
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

            return new Mutex(initiallyOwned, name, out createdNew, mutexSecurity);
        }
    }
}
