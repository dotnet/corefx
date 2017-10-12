
//------------------------------------------------------------------------------
// <copyright from='1997' to='2001' company='Microsoft Corporation'>           
//    Copyright (c) Microsoft Corporation. All Rights Reserved.                
//    Information Contained Herein is Proprietary and Confidential.            
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Management.Instrumentation
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Runtime.Versioning;

    /// <summary>
    /// CoClass for getting an IMetaDataDispenser
    /// </summary>
    [ComImport]
    [Guid("E5CB7A31-7512-11d2-89CE-0080C792E5D8")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate) /*0x0002*/]
    [ClassInterface(ClassInterfaceType.None  /*(short)0x0000*/)]
    class CorMetaDataDispenser 
    {
    }

    /// <summary>
    /// This version of the IMetaDataDispenser interface defines
    /// the interfaces so that the last parameter from cor.h
    /// is the return value of the methods.  The 'raw' way to
    /// implement these methods is as follows:
    ///    void OpenScope(
    ///        [In][MarshalAs(UnmanagedType.LPWStr)]  string   szScope,
    ///        [In] UInt32 dwOpenFlags,
    ///        [In] ref Guid riid,
    ///        [Out] out IntPtr ppIUnk);
    /// The way to call this other version is as follows
    ///    IntPtr unk;
    ///    dispenser.OpenScope(assemblyName, 0, ref guidIMetaDataImport, out unk);
    ///    importInterface = (IMetaDataImport)Marshal.GetObjectForIUnknown(unk);
    ///    Marshal.Release(unk);
    /// </summary>
    [ComImport]
    [Guid("809c652e-7396-11d2-9771-00a0c9b4d50c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown /*0x0001*/)]
    [TypeLibType(TypeLibTypeFlags.FRestricted /*0x0200*/)]
    interface IMetaDataDispenser
    {
        [return:MarshalAs(UnmanagedType.Interface)]
        object DefineScope(
            [In] ref Guid rclsid,
            [In] UInt32  dwCreateFlags,
            [In] ref Guid riid);

        [return:MarshalAs(UnmanagedType.Interface)]
        object OpenScope(
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   szScope,
            [In] UInt32 dwOpenFlags,
            [In] ref Guid riid);

        [return:MarshalAs(UnmanagedType.Interface)]
        object OpenScopeOnMemory(
            [In] IntPtr pData,
            [In] UInt32 cbData,
            [In] UInt32 dwOpenFlags,
            [In] ref Guid riid);
    }


    /// <summary>
    /// This class is an INCOMPLETE IMPLEMENTATION OF IMetaDataImport.  For the purposes of
    /// System.Management.dll, we only needed to call one method, GetScopeProps
    /// </summary>
    [ComImport]
    [Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown /*0x0001*/)]
    [TypeLibType(TypeLibTypeFlags.FRestricted /*0x0200*/)]
    interface IMetaDataImportInternalOnly
    {
        void f1();
        void f2();
        void f3();
        void f4();
        void f5();
        void f6();
        void f7();
        void GetScopeProps(
            [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName,
            [In] UInt32 cchName,
            [Out] out UInt32 pchName,
            [Out] out Guid pmvid);
    }


    /// <summary>
    /// This class wraps the functionality of IMetaDataImport.  It abstracts the
    /// details of working directly with the interface.
    /// </summary>
    class MetaDataInfo : IDisposable
    {
        [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
        public MetaDataInfo(Assembly assembly) : this(assembly.Location) {}

        public MetaDataInfo(string assemblyName)
        {
            // Get guid for IMetaDataImport
            Guid guidIMetaDataImport = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(typeof(IMetaDataImportInternalOnly), typeof(GuidAttribute), false)).Value);

            // Get an IMetaDataImport for the assembly from a dispenser
            IMetaDataDispenser dispenser = (IMetaDataDispenser)new CorMetaDataDispenser();
            importInterface = (IMetaDataImportInternalOnly)dispenser.OpenScope(assemblyName, 0, ref guidIMetaDataImport);
            Marshal.ReleaseComObject(dispenser);
        }

        IMetaDataImportInternalOnly importInterface;

        // These two fields are initialized the first time either is requested
        // They are both retrieved through IMetaDataImport GetScopeProps
        string name = null;
        Guid mvid;

        void InitNameAndMvid()
        {
            // If we have never retrieved the name and MVID, get them now
            if(null==name)
            {
                UInt32 cchName;
                StringBuilder sb = new StringBuilder();
                sb.Capacity = 0;
                importInterface.GetScopeProps(sb, (UInt32)sb.Capacity, out cchName, out mvid);
                sb.Capacity = (int)cchName;
                importInterface.GetScopeProps(sb, (UInt32)sb.Capacity, out cchName, out mvid);
                name = sb.ToString();
            }
        }

        public string Name
        {
            get
            {
                InitNameAndMvid();
                return name;
            }
        }

        public Guid Mvid
        {
            get
            {
                InitNameAndMvid();
                return mvid;
            }
        }

        public void Dispose()
        {
            // We implement IDisposable on this class because the IMetaDataImport
            // can be an expensive object to keep in memory.
            if(importInterface == null)
                Marshal.ReleaseComObject(importInterface);
            importInterface = null;
            GC.SuppressFinalize(this);
        }

        ~MetaDataInfo()
        {
            Dispose();
        }

        public static Guid GetMvid(Assembly assembly)
        {
            using(MetaDataInfo info = new MetaDataInfo(assembly))
            {
                return info.Mvid;
            }
        }
    }
}

