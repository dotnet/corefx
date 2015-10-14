// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Net
{
    // _SecPkgInfoW in sspi.h.
    internal class SecurityPackageInfoClass
    {
        internal int Capabilities = 0;
        internal short Version = 0;
        internal short RPCID = 0;
        internal int MaxToken = 0;
        internal string Name = null;
        internal string Comment = null;

        /*
         *  This is to support SSL under semi trusted environment.
         *  Note that it is only for SSL with no client cert
         *
         *  Important: safeHandle should not be Disposed during construction of this object.
         * 
         *  _SecPkgInfoW in sspi.h
         */
        internal SecurityPackageInfoClass(SafeHandle safeHandle, int index)
        {
            if (safeHandle.IsInvalid)
            {
                GlobalLog.Print("SecurityPackageInfoClass::.ctor() the pointer is invalid: " + (safeHandle.DangerousGetHandle()).ToString("x"));
                return;
            }

            IntPtr unmanagedAddress = IntPtrHelper.Add(safeHandle.DangerousGetHandle(), SecurityPackageInfo.Size * index);
            GlobalLog.Print("SecurityPackageInfoClass::.ctor() unmanagedPointer: " + ((long)unmanagedAddress).ToString("x"));

            Capabilities = Marshal.ReadInt32(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Capabilities"));
            Version = Marshal.ReadInt16(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Version"));
            RPCID = Marshal.ReadInt16(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("RPCID"));
            MaxToken = Marshal.ReadInt32(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("MaxToken"));

            IntPtr unmanagedString;

            unmanagedString = Marshal.ReadIntPtr(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Name"));
            if (unmanagedString != IntPtr.Zero)
            {
                Name = Marshal.PtrToStringUni(unmanagedString);
                GlobalLog.Print("Name: " + Name);
            }

            unmanagedString = Marshal.ReadIntPtr(unmanagedAddress, (int)Marshal.OffsetOf<SecurityPackageInfo>("Comment"));
            if (unmanagedString != IntPtr.Zero)
            {
                Comment = Marshal.PtrToStringUni(unmanagedString);
                GlobalLog.Print("Comment: " + Comment);
            }

            GlobalLog.Print("SecurityPackageInfoClass::.ctor(): " + ToString());
        }

        public override string ToString()
        {
            return "Capabilities:" + String.Format(CultureInfo.InvariantCulture, "0x{0:x}", Capabilities)
                + " Version:" + Version.ToString(NumberFormatInfo.InvariantInfo)
                + " RPCID:" + RPCID.ToString(NumberFormatInfo.InvariantInfo)
                + " MaxToken:" + MaxToken.ToString(NumberFormatInfo.InvariantInfo)
                + " Name:" + ((Name == null) ? "(null)" : Name)
                + " Comment:" + ((Comment == null) ? "(null)" : Comment);
        }
    }
}
