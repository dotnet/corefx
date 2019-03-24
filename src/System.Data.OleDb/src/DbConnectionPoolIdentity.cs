// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace System.Data.ProviderBase {

    using System;
    using System.Collections;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;
    using System.Runtime.Versioning;

    [Serializable] // Serializable so SqlDependencyProcessDispatcher can marshall cross domain to SqlDependency.
    sealed internal class DbConnectionPoolIdentity {
        private const int E_NotImpersonationToken      = unchecked((int)0x8007051D);
        private const int Win32_CheckTokenMembership   = 1;
        private const int Win32_GetTokenInformation_1  = 2;
        private const int Win32_GetTokenInformation_2  = 3;
        private const int Win32_ConvertSidToStringSidW = 4;
        private const int Win32_CreateWellKnownSid     = 5;

        static public  readonly DbConnectionPoolIdentity NoIdentity = new DbConnectionPoolIdentity(String.Empty, false, true);
        static private readonly byte[]                   NetworkSid = (ADP.IsWindowsNT ? CreateWellKnownSid(WellKnownSidType.NetworkSid) : null);
        static private DbConnectionPoolIdentity _lastIdentity = null;

        private readonly string _sidString;
        private readonly bool   _isRestricted;
        private readonly bool   _isNetwork;
        private readonly int    _hashCode;

        private DbConnectionPoolIdentity (string sidString, bool isRestricted, bool isNetwork) {
            _sidString = sidString;
            _isRestricted = isRestricted;
            _isNetwork = isNetwork;
            _hashCode = sidString == null ? 0 : sidString.GetHashCode();
        }

        internal bool IsRestricted {
            get { return _isRestricted; }
        }

        internal bool IsNetwork {
            get { return _isNetwork; }
        }

        static private byte[] CreateWellKnownSid(WellKnownSidType sidType) {
            // Passing an array as big as it can ever be is a small price to pay for
            // not having to P/Invoke twice (once to get the buffer, once to get the data)

            uint length = ( uint )SecurityIdentifier.MaxBinaryLength;
            byte[] resultSid = new byte[ length ];

            // NOTE - We copied this code from System.Security.Principal.Win32.CreateWellKnownSid...

            if ( 0 == UnsafeNativeMethods.CreateWellKnownSid(( int )sidType, null, resultSid, ref length )) {
                IntegratedSecurityError(Win32_CreateWellKnownSid);
            }
            return resultSid;
        }

        override public bool Equals(object value) {
            bool result = ((this == NoIdentity) || (this == value));
            if (!result && (null != value)) {
                DbConnectionPoolIdentity that = ((DbConnectionPoolIdentity) value);
                result = ((this._sidString == that._sidString) && (this._isRestricted == that._isRestricted) && (this._isNetwork == that._isNetwork));
            }
            return result;
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.ControlPrincipal)]
        static internal WindowsIdentity GetCurrentWindowsIdentity() {
            return WindowsIdentity.GetCurrent();
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
        static private IntPtr GetWindowsIdentityToken(WindowsIdentity identity) {
            return identity.Token;
        }

        [ResourceExposure(ResourceScope.None)] // SxS: this method does not create named objects
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
        static internal DbConnectionPoolIdentity GetCurrent() {

            // DEVNOTE: GetTokenInfo and EqualSID do not work on 9x.  WindowsIdentity does not
            //          work either on 9x.  In fact, after checking with native there is no way
            //          to validate the user on 9x, so simply don't.  It is a known issue in
            //          native, and we will handle this the same way.

            if (!ADP.IsWindowsNT) {
                return NoIdentity;
            }

            WindowsIdentity identity     = GetCurrentWindowsIdentity();
            IntPtr          token        = GetWindowsIdentityToken(identity); // Free'd by WindowsIdentity.
            uint            bufferLength = 2048;           // Suggested default given by Greg Fee.
            uint            lengthNeeded = 0;

            IntPtr          tokenStruct = IntPtr.Zero;
            IntPtr          SID;
            IntPtr          sidStringBuffer = IntPtr.Zero;
            bool            isNetwork;

            // Win32NativeMethods.IsTokenRestricted will raise exception if the native call fails
            bool            isRestricted = Win32NativeMethods.IsTokenRestrictedWrapper(token);
            
            DbConnectionPoolIdentity current = null;

            RuntimeHelpers.PrepareConstrainedRegions();
            try {
                if (!UnsafeNativeMethods.CheckTokenMembership(token, NetworkSid, out isNetwork)) {
                    // will always fail with 0x8007051D if token is not an impersonation token
                    IntegratedSecurityError(Win32_CheckTokenMembership);
                }
                
                RuntimeHelpers.PrepareConstrainedRegions();
                try { } finally {
                    // allocating memory and assigning to tokenStruct must happen
                    tokenStruct = SafeNativeMethods.LocalAlloc(DbBuffer.LMEM_FIXED, (IntPtr)bufferLength);
                }
                if (IntPtr.Zero == tokenStruct) {
                    throw new OutOfMemoryException();
                }
                if (!UnsafeNativeMethods.GetTokenInformation(token, 1, tokenStruct, bufferLength, ref lengthNeeded)) {
                    if (lengthNeeded > bufferLength) {
                        bufferLength = lengthNeeded;

                        RuntimeHelpers.PrepareConstrainedRegions();
                        try { } finally {
                            // freeing token struct and setting tokenstruct to null must happen together
                            // allocating memory and assigning to tokenStruct must happen
                            SafeNativeMethods.LocalFree(tokenStruct);
                            tokenStruct = IntPtr.Zero; // protect against LocalAlloc throwing an exception
                            tokenStruct = SafeNativeMethods.LocalAlloc(DbBuffer.LMEM_FIXED, (IntPtr)bufferLength);
                        }
                        if (IntPtr.Zero == tokenStruct) {
                            throw new OutOfMemoryException();
                        }

                        if (!UnsafeNativeMethods.GetTokenInformation(token, 1, tokenStruct, bufferLength, ref lengthNeeded)) {
                            IntegratedSecurityError(Win32_GetTokenInformation_1);
                        }
                    }
                    else {
                        IntegratedSecurityError(Win32_GetTokenInformation_2);
                    }
                }

                identity.Dispose(); // Keep identity variable alive until after GetTokenInformation calls.


                SID = Marshal.ReadIntPtr(tokenStruct, 0);

                if (!UnsafeNativeMethods.ConvertSidToStringSidW(SID, out sidStringBuffer)) {
                    IntegratedSecurityError(Win32_ConvertSidToStringSidW);
                }

                if (IntPtr.Zero == sidStringBuffer) {
                    throw ADP.InternalError(ADP.InternalErrorCode.ConvertSidToStringSidWReturnedNull);
                }

                string sidString = Marshal.PtrToStringUni(sidStringBuffer);

                var lastIdentity = _lastIdentity;
                if ((lastIdentity != null) && (lastIdentity._sidString == sidString) && (lastIdentity._isRestricted == isRestricted) && (lastIdentity._isNetwork == isNetwork)) {
                    current = lastIdentity;
                }
                else {
                    current = new DbConnectionPoolIdentity(sidString, isRestricted, isNetwork);
                }
            }
            finally {
                // Marshal.FreeHGlobal does not have a ReliabilityContract
                if (IntPtr.Zero != tokenStruct) {
                    SafeNativeMethods.LocalFree(tokenStruct);
                    tokenStruct = IntPtr.Zero;
                }
                if (IntPtr.Zero != sidStringBuffer) {
                    SafeNativeMethods.LocalFree(sidStringBuffer);
                    sidStringBuffer = IntPtr.Zero;
                }
            }
            _lastIdentity = current;
            return current;
        }

        override public int GetHashCode() {
            return _hashCode;
        }

        static private void IntegratedSecurityError(int caller) {
            // passing 1,2,3,4,5 instead of true/false so that with a debugger
            // we could determine more easily which Win32 method call failed
            int lastError = Marshal.GetHRForLastWin32Error();
            if ((Win32_CheckTokenMembership != caller) || (E_NotImpersonationToken != lastError)) {
                Marshal.ThrowExceptionForHR(lastError); // will only throw if (hresult < 0)
            }
        }
        
    }
}

