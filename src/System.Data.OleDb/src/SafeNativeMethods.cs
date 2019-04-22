// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading;

namespace System.Data.Common {

    [SuppressUnmanagedCodeSecurity()]
    internal static class SafeNativeMethods {
    
        [DllImport(Interop.Libraries.Ole32, SetLastError=false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern IntPtr CoTaskMemAlloc(IntPtr cb);

        [DllImport(Interop.Libraries.Ole32, SetLastError=false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern void CoTaskMemFree(IntPtr handle);

        [DllImport(Interop.Libraries.Kernel32, CharSet=CharSet.Unicode, PreserveSig=true)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern int GetUserDefaultLCID();

        internal static void ZeroMemory(IntPtr ptr, int length)
        {
            var zeroes = new byte[length];
            Marshal.Copy(zeroes, 0, ptr, length);
        }

        // <WARNING>
        // Using the int versions of the Increment() and Decrement() methods is correct.
        // Please check \fx\src\Data\System\Data\Odbc\OdbcHandle.cs for the memory layout.
        // </WARNING>

        // <NDPWHIDBEY 18133>
        // The following casting operations require these three methods to be unsafe.  This is
        // a workaround for this issue to meet the M1 exit criteria.  We need to revisit this in M2.

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        static internal unsafe IntPtr InterlockedExchangePointer(
                IntPtr lpAddress,
                IntPtr lpValue)
        {
            IntPtr  previousPtr;
            IntPtr  actualPtr = *(IntPtr *)lpAddress.ToPointer();

            do {
                previousPtr = actualPtr;
                actualPtr   = Interlocked.CompareExchange(ref *(IntPtr *)lpAddress.ToPointer(), lpValue, previousPtr);
            }
            while (actualPtr != previousPtr);

            return actualPtr;
        }

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/getcomputernameex.asp
        [DllImport(Interop.Libraries.Kernel32, CharSet = CharSet.Unicode, EntryPoint="GetComputerNameExW", SetLastError=true)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern int GetComputerNameEx(int nameType, StringBuilder nameBuffer, ref int bufferSize);

        [DllImport(Interop.Libraries.Kernel32, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        static internal extern int GetCurrentProcessId();

        [DllImport(Interop.Libraries.Kernel32, CharSet=CharSet.Auto, BestFitMapping=false, ThrowOnUnmappableChar=true)]
//        [DllImport(Interop.Libraries.Kernel32, CharSet=CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        static internal extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPTStr), In] string moduleName/*lpctstr*/);

        [DllImport(Interop.Libraries.Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
//        [DllImport(Interop.Libraries.Kernel32, CharSet=CharSet.Ansi)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern IntPtr GetProcAddress(IntPtr HModule, [MarshalAs(UnmanagedType.LPStr), In] string funcName/*lpcstr*/);

        [DllImport(Interop.Libraries.Kernel32, SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern IntPtr LocalAlloc(int flags, IntPtr countOfBytes);

        [DllImport(Interop.Libraries.Kernel32, SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern IntPtr LocalFree(IntPtr handle);

        [DllImport(Interop.Libraries.OleAut32, CharSet=CharSet.Unicode)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]            
        [ResourceExposure(ResourceScope.None)]
        internal static extern IntPtr SysAllocStringLen(String src, int len);  // BSTR

        [DllImport(Interop.Libraries.OleAut32)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern void SysFreeString(IntPtr bstr);

        // only using this to clear existing error info with null
        [DllImport(Interop.Libraries.OleAut32, CharSet=CharSet.Unicode, PreserveSig=false)]
        // TLS values are preserved between threads, need to check that we use this API to clear the error state only.
        [ResourceExposure(ResourceScope.Process)]
        static private extern void SetErrorInfo(Int32 dwReserved, IntPtr pIErrorInfo);

        [DllImport(Interop.Libraries.Kernel32, SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.Machine)]
        static internal extern int ReleaseSemaphore(IntPtr handle, int releaseCount, IntPtr previousCount);

        [DllImport(Interop.Libraries.Kernel32, SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern int WaitForMultipleObjectsEx(uint nCount, IntPtr lpHandles, bool bWaitAll, uint dwMilliseconds, bool bAlertable);

        [DllImport(Interop.Libraries.Kernel32/*, SetLastError=true*/)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern int WaitForSingleObjectEx(IntPtr lpHandles, uint dwMilliseconds, bool bAlertable);

        [DllImport(Interop.Libraries.Ole32, PreserveSig=false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern void PropVariantClear(IntPtr pObject);

        [DllImport(Interop.Libraries.OleAut32, PreserveSig=false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern void VariantClear(IntPtr pObject);

        sealed internal class Wrapper {

            private Wrapper() { }

            // SxS: clearing error information is considered safe
            [ResourceExposure(ResourceScope.None)]
            [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
            static internal void ClearErrorInfo() {
                SafeNativeMethods.SetErrorInfo(0, ADP.PtrZero);
            }
        }
    }
}
