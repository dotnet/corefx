// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Wrapper around an output gss_buffer_desc*
    /// </summary>
    internal sealed class SafeGssBufferHandle : SafeHandle
    {
        private SafeGssBufferHandle() : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        internal void Copy(byte[] destination, int offset)
        {
            Debug.Assert(destination != null, "destination buffer cannot be null");
            Debug.Assert(offset <= destination.Length , "offset in the destination buffer must be valid");
            if (destination.Length > 0)
            {
                Interop.NetSecurity.CopyBuffer(this, destination, capacity: destination.Length, offset: offset);
            }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurity.Status minorStatus;
            Interop.NetSecurity.Status status = Interop.NetSecurity.ReleaseBuffer(out minorStatus, handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurity.Status.GSS_S_COMPLETE;
        }
    }

    /// <summary>
    /// Wrapper around a gss_name_t_desc*
    /// </summary>
    internal sealed class SafeGssNameHandle : SafeHandle
    {
        public static SafeGssNameHandle Create(string name, bool isUser)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Invalid name passed to SafeGssNameHandle create");
            SafeGssNameHandle retHandle;
            Interop.NetSecurity.Status minorStatus;
            Interop.NetSecurity.Status status = isUser?
                Interop.NetSecurity.ImportUserName(out minorStatus, name, name.Length, out retHandle) :
                Interop.NetSecurity.ImportPrincipalName(out minorStatus, name, name.Length, out retHandle);
            if (status != Interop.NetSecurity.Status.GSS_S_COMPLETE)
            {
                throw new Interop.NetSecurity.GssApiException(status, minorStatus);
            }

            return retHandle;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurity.Status minorStatus;
            Interop.NetSecurity.Status status = Interop.NetSecurity.ReleaseName(out minorStatus, ref handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurity.Status.GSS_S_COMPLETE;
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
        public static SafeGssCredHandle Create(string username, string password, string domain)
        {
            SafeGssCredHandle retHandle = null;

            // Empty username is OK if Kerberos ticket was already obtained
            if (!string.IsNullOrEmpty(username))
            {
                using (SafeGssNameHandle userHandle = SafeGssNameHandle.Create(username, true))
                {
                    Interop.NetSecurity.Status status;
                    Interop.NetSecurity.Status minorStatus;
                    if (string.IsNullOrEmpty(password))
                    {
                        status = Interop.NetSecurity.AcquireCredSpNego(out minorStatus, userHandle, true, out retHandle);
                    }
                    else
                    {
                        status = Interop.NetSecurity.AcquireCredWithPassword(out minorStatus, userHandle, password, password.Length, true, out retHandle);
                    }

                    if (status != Interop.NetSecurity.Status.GSS_S_COMPLETE)
                    {
                        throw new Interop.NetSecurity.GssApiException(status, minorStatus);
                    }
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
            Interop.NetSecurity.Status minorStatus;
            Interop.NetSecurity.Status status = Interop.NetSecurity.ReleaseCred(out minorStatus, ref handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurity.Status.GSS_S_COMPLETE;
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
            Interop.NetSecurity.Status minorStatus;
            Interop.NetSecurity.Status status = Interop.NetSecurity.DeleteSecContext(out minorStatus, ref handle);
            SetHandle(IntPtr.Zero);
            return status == Interop.NetSecurity.Status.GSS_S_COMPLETE;
        }
    }
}
