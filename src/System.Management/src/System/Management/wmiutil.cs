using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WbemClient_v1;

namespace System.Management
{

    [ComImport, Guid("87A5AD68-A38A-43ef-ACA9-EFE910E5D24C"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWmiEventSource
    {
        void Indicate(IntPtr pIWbemClassObject);

        void SetStatus(
            int lFlags,
            int hResult,
            [MarshalAs(UnmanagedType.BStr)] string strParam ,
            IntPtr pObjParam
        );
    }

#if USEIWOS
    // The following is a manually defined wrapper for IWbemObjectSink
    // since the size_is attribute cannot be dealt with by TlbImp.
    [Guid("7c857801-7381-11cf-884d-00aa004b2e24"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWbemObjectSink
    {
        [PreserveSig]
        void Indicate(
            long lObjectCount,
            [MarshalAs(UnmanagedType.Interface, SizeParamIndex=0)] IWbemClassObject [] apObjArray
            );

        [PreserveSig]
        void SetStatus(
            long lFlags,
            int hResult,
            [MarshalAs(UnmanagedType.BStr)] string strParam,
            [MarshalAs(UnmanagedType.Interface)] IWbemClassObject pObjParam
            );

    };
#endif

    //Class for calling GetErrorInfo from managed code
    class WbemErrorInfo
    {
        public static IWbemClassObjectFreeThreaded GetErrorInfo()
        {
            IntPtr pErrorInfo = WmiNetUtilsHelper.GetErrorInfo_f();
            if (IntPtr.Zero != pErrorInfo && new IntPtr(-1) != pErrorInfo)
            {
                IntPtr pIWbemClassObject;
                Marshal.QueryInterface(pErrorInfo, ref IWbemClassObjectFreeThreaded.IID_IWbemClassObject, out pIWbemClassObject);
                Marshal.Release(pErrorInfo);

                // The IWbemClassObjectFreeThreaded instance will own reference count on pIWbemClassObject
                if(pIWbemClassObject != IntPtr.Zero)
                    return new IWbemClassObjectFreeThreaded(pIWbemClassObject);
            }
            return null;
        }
    }

    //RCW for IErrorInfo
    [ComImport]
    [Guid("1CF2B120-547D-101B-8E65-08002B2BD119")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IErrorInfo 
    {
        Guid GetGUID();

         [return:MarshalAs(UnmanagedType.BStr)]
        string GetSource();

        [return:MarshalAs(UnmanagedType.BStr)]
        string GetDescription();

        [return:MarshalAs(UnmanagedType.BStr)]
        string GetHelpFile();

        uint GetHelpContext();
    }

}
