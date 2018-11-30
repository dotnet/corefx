using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    [ComImport]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDispatch
    {
        void GetIDsOfNames(
            ref Guid riid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2), In]
            string[] rgszNames,
            int cNames,
            int lcid,
            [Out] int[] rgDispId);

        ITypeInfo GetTypeInfo(
            int iTInfo,
            int lcid);

        int GetTypeInfoCount();

        void Invoke(
            int dispIdMember,
            ref Guid riid,
            int lcid,
            InvokeFlags wFlags,
            ref DISPPARAMS pDispParams,
            out object pVarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr);
    }

    [Flags]
    internal enum InvokeFlags : short
    {
        Method = 1,
        PropertyGet = 2,
        PropertyPut = 4,
        PropertyPutRef = 8
    }
}
