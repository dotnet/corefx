// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace System.Security.Principal
{
    public class WindowsImpersonationContext : IDisposable
    {
        private SafeAccessTokenHandle _previousUserToken = SafeAccessTokenHandle.InvalidHandle;

        private WindowsImpersonationContext() { }

        internal WindowsImpersonationContext(SafeAccessTokenHandle currentUserToken, bool isImpersonating)
        {
            if (currentUserToken.IsInvalid)
                throw new ArgumentException(SR.Argument_InvalidImpersonationToken);

            if (isImpersonating)
            {
                // if we'are in a impersonated context we need to duplicate current token to allow revert to correct identity 
                // after impersonation
                _previousUserToken = WindowsIdentity.DuplicateAccessToken(currentUserToken);
            }
        }

        public void Undo()
        {
            if (!Interop.Advapi32.RevertToSelf())
            { 
                Environment.FailFast(new Win32Exception().Message);
            }

            // revert impersonating token and impersonate previous identity
            if (!_previousUserToken.IsInvalid)
            {
                if (!Interop.Advapi32.ImpersonateLoggedOnUser(_previousUserToken))
                {
                    throw new SecurityException(SR.Argument_ImpersonateUser);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_previousUserToken != null && !_previousUserToken.IsClosed)
                {
                    Undo();
                    _previousUserToken.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
