//------------------------------------------------------------------------------
// <copyright file="COM2AboutBoxPropertyDescriptor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

    namespace System.Runtime.InteropServices {
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject"]/*' />
    /// <internalonly/>
    /// <devdoc>
    /// Replaces the standard CLR free-threaded marshaler with the standard OLE STA one.  This prevents the calls made into
    /// our hosting object by OLE from coming in on threads other than the UI thread.
    ///
    /// </devdoc>
    [ComVisible(true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1403:AutoLayoutTypesShouldNotBeComVisible")]
    public class StandardOleMarshalObject : MarshalByRefObject, UnsafeNativeMethods.IMarshal
    {
        static readonly Guid CLSID_StdMarshal = new Guid("00000017-0000-0000-c000-000000000046");

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetMarshalSizeMax_Delegate(IntPtr _this, ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags, out int pSize);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int MarshalInterface_Delegate(IntPtr _this, IntPtr pStm, ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags);

        protected StandardOleMarshalObject()
        { }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        private IntPtr GetStdMarshaler(ref Guid riid, int dwDestContext, int mshlflags)
        {
            IntPtr pStandardMarshal = IntPtr.Zero;

            IntPtr pUnk = Marshal.GetIUnknownForObject(this);
            if (pUnk != IntPtr.Zero)
            {
                try
                {
                    if (NativeMethods.S_OK == UnsafeNativeMethods.CoGetStandardMarshal(ref riid, pUnk, dwDestContext, IntPtr.Zero, mshlflags, out pStandardMarshal))
                    {
                        Debug.Assert(pStandardMarshal != null, "Failed to get marshaler for interface '" + riid.ToString() + "', CoGetStandardMarshal returned S_OK");
                        return pStandardMarshal;
                    }
                }
                finally
                {
                    Marshal.Release(pUnk);
                }
            }
            throw new InvalidOperationException(SR.GetString(SR.StandardOleMarshalObjectGetMarshalerFailed, riid.ToString()));
        }
        
        /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject.UnsafeNativeMethods.IMarshal.GetUnmarshalClass"]/*' />
        /// <internalonly/>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.GetUnmarshalClass(ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags, out Guid pCid)
        {
            pCid = CLSID_StdMarshal;
            return NativeMethods.S_OK;
        }
        
        /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject.UnsafeNativeMethods.IMarshal.GetMarshalSizeMax"]/*' />
        /// <internalonly/>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        unsafe int UnsafeNativeMethods.IMarshal.GetMarshalSizeMax(ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags, out int pSize)
        {
            IntPtr pStandardMarshal = GetStdMarshaler(ref riid, dwDestContext, mshlflags);

            try
            {
                // we must not wrap pStandardMarshal with an RCW because that would trigger QIs for random IIDs and the marshaler
                // (aka stub manager object) does not really handle these well and we would risk triggering an AppVerifier break
                IntPtr vtable = *(IntPtr*)pStandardMarshal.ToPointer();
                IntPtr method = *((IntPtr*)vtable.ToPointer() + 4); // GetMarshalSizeMax is 4th slot

                GetMarshalSizeMax_Delegate del = (GetMarshalSizeMax_Delegate)Marshal.GetDelegateForFunctionPointer(method, typeof(GetMarshalSizeMax_Delegate));
                return del(pStandardMarshal, ref riid, pv, dwDestContext, pvDestContext, mshlflags, out pSize);
            }
            finally
            {
                Marshal.Release(pStandardMarshal);
            }
        }
        
        /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject.UnsafeNativeMethods.IMarshal.MarshalInterface"]/*' />
        /// <internalonly/>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        unsafe int UnsafeNativeMethods.IMarshal.MarshalInterface(IntPtr pStm, ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags)
        {
            IntPtr pStandardMarshal = GetStdMarshaler(ref riid, dwDestContext, mshlflags);

            try
            {
                // we must not wrap pStandardMarshal with an RCW because that would trigger QIs for random IIDs and the marshaler
                // (aka stub manager object) does not really handle these well and we would risk triggering an AppVerifier break
                IntPtr vtable = *(IntPtr *)pStandardMarshal.ToPointer();
                IntPtr method = *((IntPtr *)vtable.ToPointer() + 5); // GetMarshalSizeMax is 5th slot

                MarshalInterface_Delegate del = (MarshalInterface_Delegate)Marshal.GetDelegateForFunctionPointer(method, typeof(MarshalInterface_Delegate));
                return del(pStandardMarshal, pStm, ref riid, pv, dwDestContext, pvDestContext, mshlflags);
            }
            finally
            {
                Marshal.Release(pStandardMarshal);
            }
        }
        
        /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject.UnsafeNativeMethods.IMarshal.UnmarshalInterface"]/*' />
        /// <internalonly/>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.UnmarshalInterface(IntPtr pStm, ref Guid riid, out IntPtr ppv)
        {
            // this should never be called on this interface, but on the standard one handed back by the previous calls.
            Debug.Fail("IMarshal::UnmarshalInterface should not be called.");
            ppv = IntPtr.Zero;
            return NativeMethods.E_NOTIMPL;
        }
        
        /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject.UnsafeNativeMethods.IMarshal.ReleaseMarshalData"]/*' />
        /// <internalonly/>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.ReleaseMarshalData(IntPtr pStm)
        {
            // this should never be called on this interface, but on the standard one handed back by the previous calls.
            Debug.Fail("IMarshal::ReleaseMarshalData should not be called.");
            return NativeMethods.E_NOTIMPL;
        }
        
        /// <include file='doc\StandardOleMarshalObject.uex' path='docs/doc[@for="StandardOleMarshalObject.UnsafeNativeMethods.IMarshal.DisconnectObject"]/*' />
        /// <internalonly/>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.DisconnectObject(int dwReserved)
        {
            // this should never be called on this interface, but on the standard one handed back by the previous calls.
            Debug.Fail("IMarshal::DisconnectObject should not be called.");
            return NativeMethods.E_NOTIMPL;
        }
    }
}
