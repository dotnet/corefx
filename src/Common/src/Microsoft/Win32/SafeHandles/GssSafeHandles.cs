// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Wrapper around a gss_name_t_desc*
    /// </summary>
    internal sealed class SafeGssNameHandle : SafeHandle
    {
        public static SafeGssNameHandle CreateUser(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Invalid user name passed to SafeGssNameHandle create");
            SafeGssNameHandle retHandle;
            Interop.NetSecurityNative.Status minorStatus;
            Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.ImportUserName(
                out minorStatus, name, Encoding.UTF8.GetByteCount(name), out retHandle);

            if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
            {
                retHandle.Dispose();
                throw new Interop.NetSecurityNative.GssApiException(status, minorStatus);
            }

            return retHandle;
        }

        public static SafeGssNameHandle CreatePrincipal(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Invalid principal passed to SafeGssNameHandle create");
            SafeGssNameHandle retHandle;
            Interop.NetSecurityNative.Status minorStatus;
            Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.ImportPrincipalName(
                out minorStatus, name, Encoding.UTF8.GetByteCount(name), out retHandle);

            if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
            {
                retHandle.Dispose();
                throw new Interop.NetSecurityNative.GssApiException(status, minorStatus);
            }

            return retHandle;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurityNative.Status minorStatus;
            Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.ReleaseName(out minorStatus, ref handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurityNative.Status.GSS_S_COMPLETE;
        }

        private SafeGssNameHandle()
            : base(IntPtr.Zero, true)
        {
        }
    }

    /// <summary>
    /// Wrapper around a gss_cred_id_t_desc_struct*
    /// </summary>
    internal class SafeGssCredHandle : SafeHandle
    {
        /// <summary>
        ///  returns the handle for the given credentials.
        ///  The method returns an invalid handle if the username is null or empty.
        /// </summary>
        public static SafeGssCredHandle Create(string username, string password, string domain)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new SafeGssCredHandle();
            }

            SafeGssCredHandle retHandle = null;
            using (SafeGssNameHandle userHandle = SafeGssNameHandle.CreateUser(username))
            {
                Interop.NetSecurityNative.Status status;
                Interop.NetSecurityNative.Status minorStatus;
                if (string.IsNullOrEmpty(password))
                {
                    status = Interop.NetSecurityNative.InitiateCredSpNego(out minorStatus, userHandle, out retHandle);
                }
                else
                {
                    status = Interop.NetSecurityNative.InitiateCredWithPassword(out minorStatus, userHandle, password, Encoding.UTF8.GetByteCount(password), out retHandle);
                }

                if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
                {
                    retHandle.Dispose();
                    throw new Interop.NetSecurityNative.GssApiException(status, minorStatus);
                }
            }

            return retHandle;
        }

        private SafeGssCredHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurityNative.Status minorStatus;
            Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.ReleaseCred(out minorStatus, ref handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurityNative.Status.GSS_S_COMPLETE;
        }
    }

    internal sealed class SafeGssContextHandle : SafeHandle
    {
        public SafeGssContextHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurityNative.Status minorStatus;
            Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.DeleteSecContext(out minorStatus, ref handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurityNative.Status.GSS_S_COMPLETE;
        }
    }
}
