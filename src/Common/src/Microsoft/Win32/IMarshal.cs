// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
    [ComImport, Guid("00000003-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMarshal
    {
        [PreserveSig]
        int GetUnmarshalClass(ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags, out Guid pCid);
        [PreserveSig]
        int GetMarshalSizeMax(ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags, out int pSize);
        [PreserveSig]
        int MarshalInterface(IntPtr pStm, ref Guid riid, IntPtr pv, int dwDestContext, IntPtr pvDestContext, int mshlflags);
        [PreserveSig]
        int UnmarshalInterface(IntPtr pStm, ref Guid riid, out IntPtr ppv);
        [PreserveSig]
        int ReleaseMarshalData(IntPtr pStm);
        [PreserveSig]
        int DisconnectObject(int dwReserved);
    }

    internal static class NativeMethods
    {
        public const int S_OK = 0x0;
        public const int E_NOTIMPL = unchecked((int)0x80004001);
    }
}
