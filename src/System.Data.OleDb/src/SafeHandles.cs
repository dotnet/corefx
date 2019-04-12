// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Data.ProviderBase;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Data.OleDb {
    internal sealed class DualCoTaskMem : SafeHandle {

        private IntPtr handle2;   // this must be protected so derived classes can use out params.

        private DualCoTaskMem() : base(IntPtr.Zero, true) {
            this.handle2 = IntPtr.Zero;
        }

        // IDBInfo.GetLiteralInfo
        internal DualCoTaskMem(UnsafeNativeMethods.IDBInfo dbInfo, int[] literals, out int literalCount, out IntPtr literalInfo, out OleDbHResult hr) : this() {
            int count = (null != literals) ? literals.Length : 0;
            hr = dbInfo.GetLiteralInfo(count, literals, out literalCount, out base.handle, out this.handle2);
            literalInfo = base.handle;
        }

        // IColumnsInfo.GetColumnInfo
        internal DualCoTaskMem(UnsafeNativeMethods.IColumnsInfo columnsInfo, out IntPtr columnCount, out IntPtr columnInfos, out OleDbHResult hr) : this() {
            hr = columnsInfo.GetColumnInfo(out columnCount, out base.handle, out this.handle2);
            columnInfos = base.handle;
        }

        // IDBSchemaRowset.GetSchemas
        internal DualCoTaskMem(UnsafeNativeMethods.IDBSchemaRowset dbSchemaRowset, out int schemaCount, out IntPtr schemaGuids, out IntPtr schemaRestrictions, out OleDbHResult hr) : this() {
            hr = dbSchemaRowset.GetSchemas(out schemaCount, out base.handle, out this.handle2);
            schemaGuids = base.handle;
            schemaRestrictions = this.handle2;
        }

        internal DualCoTaskMem(UnsafeNativeMethods.IColumnsRowset icolumnsRowset, out IntPtr cOptColumns, out OleDbHResult hr) : base(IntPtr.Zero, true) {
            hr = icolumnsRowset.GetAvailableColumns(out cOptColumns, out base.handle);
        }


        public override bool IsInvalid {
            get {
                return (((IntPtr.Zero == base.handle)) && (IntPtr.Zero == this.handle2));
            }
        }

        protected override bool ReleaseHandle() {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.

            IntPtr ptr = base.handle;
            base.handle = IntPtr.Zero;
            if (IntPtr.Zero != ptr) {
                SafeNativeMethods.CoTaskMemFree(ptr);
            }

            ptr = this.handle2;
            this.handle2 = IntPtr.Zero;
            if (IntPtr.Zero != ptr) {
                SafeNativeMethods.CoTaskMemFree(ptr);
            }
            return true;
        }
    }

    internal sealed class RowHandleBuffer : DbBuffer {

        internal RowHandleBuffer(IntPtr rowHandleFetchCount) : base((int)rowHandleFetchCount*ADP.PtrSize) {
        }

        internal IntPtr GetRowHandle(int index) {
            IntPtr value = ReadIntPtr( index * ADP.PtrSize);
            Debug.Assert(ODB.DB_NULL_HROW != value, "bad rowHandle");
            return value;
        }
    }

    internal sealed class StringMemHandle : DbBuffer {

        internal StringMemHandle(string value) : base((null != value) ? checked(2+2*value.Length) : 0) {
            if (null != value) {
                // null-termination exists because of the extra 2+ which is zero'd during on allocation
                WriteCharArray(0, value.ToCharArray(), 0, value.Length);
            }
        }
    }

    internal sealed class ChapterHandle : WrappedIUnknown {

        internal static readonly ChapterHandle DB_NULL_HCHAPTER = new ChapterHandle(IntPtr.Zero);
        private IntPtr _chapterHandle;

        internal static ChapterHandle CreateChapterHandle(object chapteredRowset, RowBinding binding, int valueOffset) {
            if ((null == chapteredRowset) || (IntPtr.Zero == binding.ReadIntPtr(valueOffset))) {
                return ChapterHandle.DB_NULL_HCHAPTER;
            }
            return new ChapterHandle(chapteredRowset, binding, valueOffset);
        }

         // from ADODBRecordSetConstruction we do not want to release the initial chapter handle
        internal static ChapterHandle CreateChapterHandle(IntPtr chapter) {
            if (IntPtr.Zero == chapter) {
                return ChapterHandle.DB_NULL_HCHAPTER;
            }
            return new ChapterHandle(chapter);
        }

        // from ADODBRecordSetConstruction we do not want to release the initial chapter handle
        private ChapterHandle(IntPtr chapter) : base((object)null) {
            _chapterHandle = chapter;
        }

        private ChapterHandle(object chapteredRowset, RowBinding binding, int valueOffset) : base(chapteredRowset) {
            RuntimeHelpers.PrepareConstrainedRegions();
            try {} finally {
                _chapterHandle = binding.InterlockedExchangePointer(valueOffset);
            }
        }

        internal IntPtr HChapter {
            get {
                return _chapterHandle;
            }
        }

        protected override bool ReleaseHandle() {
            // NOTE: The SafeHandle class guarantees this will be called exactly once and is non-interrutible.
            IntPtr chapter = _chapterHandle;
            _chapterHandle = IntPtr.Zero;

            if ((IntPtr.Zero != base.handle) && (IntPtr.Zero != chapter)) {
                OleDbHResult hr = (OleDbHResult)NativeOledbWrapper.IChapteredRowsetReleaseChapter(base.handle, chapter);
            }
            return base.ReleaseHandle();
        }
    }

    [Guid("0fb15084-af41-11ce-bd2b-204c4f4f5020")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ITransaction
    {
        [PreserveSig]
        int Commit
            (
            [In] bool  fRetaining, 
            [In] uint grfTC,
            [In] uint grfRM
            );

        [PreserveSig]
        int Abort
            (
            [In] IntPtr pboidReason,
            [In]         bool  fRetaining,
            [In]         bool  fAsync
            );

        [PreserveSig]
        int GetTransactionInfo
            (
            [Out] IntPtr pinfo
            );
    }

    [Guid("0c733a93-2a1c-11ce-ade5-00aa0044773d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    unsafe internal interface ITransactionLocal : ITransaction
    {
        [PreserveSig]
        new int Commit
            (
            [In] bool  fRetaining, 
            [In] uint grfTC,
            [In] uint grfRM
            );

        [PreserveSig]
        new int Abort
            (
            [In] IntPtr pboidReason,
            [In]         bool  fRetaining,
            [In]         bool  fAsync
            );

        [PreserveSig]
        new int GetTransactionInfo
            (
            [Out] IntPtr pinfo
            );

        [PreserveSig]
        int GetOptionsObject(
            [Out, Optional] IntPtr ppOptions
        );

        [PreserveSig]
        int StartTransaction(
            [In] int isoLevel,
            [In] uint isoFlags,
            [In, Optional] IntPtr pOtherOptions,
            [Out, Optional] uint * pulTransactionLevel
        );
    }

    internal enum XACTTC 
    {
        XACTTC_NONE = 0x0000,
        XACTTC_SYNC_PHASEONE = 0x0001,
        XACTTC_SYNC_PHASETWO = 0x0002,
        XACTTC_SYNC = 0x0002,
        XACTTC_ASYNC_PHASEONE = 0x0004,
        XACTTC_ASYNC = 0x0004
    }

    internal class NativeOledbWrapper
    {
        // TODO: remove this and use "sizeof(PROPVARIANT)" wherever this is used, once PROPVARIANT is implemented in project
        // https://docs.microsoft.com/en-us/windows/desktop/api/propidl/ns-propidl-propvariant
        internal const int SizeOfPROPVARIANT = 138;//sizeof(PROPVARIANT); 

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal unsafe static OleDbHResult IChapteredRowsetReleaseChapter(System.IntPtr ptr, System.IntPtr chapter)
        {
            OleDbHResult hr = OleDbHResult.E_UNEXPECTED;
            IntPtr hchapter = chapter;
            System.Data.Common.UnsafeNativeMethods.IChapteredRowset chapteredRowset = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                Guid IID_IChapteredRowset = typeof(System.Data.Common.UnsafeNativeMethods.IChapteredRowset).GUID;
                hr = (OleDbHResult)Marshal.QueryInterface(ptr, ref IID_IChapteredRowset, out var pChapteredRowset);
                if (pChapteredRowset != IntPtr.Zero)
                {
                    chapteredRowset = (System.Data.Common.UnsafeNativeMethods.IChapteredRowset)Marshal.GetObjectForIUnknown(pChapteredRowset);
                    hr = (OleDbHResult)chapteredRowset.ReleaseChapter(hchapter, out var refcount);
                    Marshal.Release(pChapteredRowset);
                }
            }
            return hr;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal unsafe static OleDbHResult ITransactionAbort(System.IntPtr ptr)
        {
            OleDbHResult hr = OleDbHResult.E_UNEXPECTED;
            ITransactionLocal transactionLocal = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                Guid IID_ITransactionLocal = typeof(ITransactionLocal).GUID;
                hr = (OleDbHResult)Marshal.QueryInterface(ptr, ref IID_ITransactionLocal, out var pTransaction);
                if (pTransaction != IntPtr.Zero)
                {
                    transactionLocal = (ITransactionLocal)Marshal.GetObjectForIUnknown(pTransaction);
                    hr = (OleDbHResult)transactionLocal.Abort(IntPtr.Zero, false, false);
                    Marshal.Release(pTransaction);
                }
            }
            return hr;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal unsafe static OleDbHResult ITransactionCommit(System.IntPtr ptr)
        {
            OleDbHResult hr = OleDbHResult.E_UNEXPECTED;
            ITransactionLocal transactionLocal = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                Guid IID_ITransactionLocal = typeof(ITransactionLocal).GUID;
                hr = (OleDbHResult)Marshal.QueryInterface(ptr, ref IID_ITransactionLocal, out var pTransaction);
                if (pTransaction != IntPtr.Zero)
                {
                    transactionLocal = (ITransactionLocal)Marshal.GetObjectForIUnknown(pTransaction);
                    hr = (OleDbHResult)transactionLocal.Commit(false, (uint)XACTTC.XACTTC_SYNC_PHASETWO, 0);
                    Marshal.Release(pTransaction);
                }
            }
            return hr;
        }

        [ResourceExposure(ResourceScope.None)]
        internal static bool MemoryCompare(System.IntPtr buf1, System.IntPtr buf2, System.Int32 count)
        {

            Debug.Assert (buf1 != buf2, "buf1 and buf2 are the same");
            Debug.Assert (buf1.ToInt64() < buf2.ToInt64() || buf2.ToInt64() + count <= buf1.ToInt64(), "overlapping region buf1");
            Debug.Assert (buf2.ToInt64() < buf1.ToInt64() || buf1.ToInt64() + count <= buf2.ToInt64(), "overlapping region buf2");
            Debug.Assert (0 <= count, "negative count");
            unsafe
            {
                ReadOnlySpan<byte> span1 = new ReadOnlySpan<byte>(buf1.ToPointer(), count);
                ReadOnlySpan<byte> span2 = new ReadOnlySpan<byte>(buf2.ToPointer(), count);
                return !MemoryExtensions.SequenceEqual(span1, span2);
                //0​ if all count bytes of lhs and rhs are equal. 
                // TODO: confirm condition with tests
            }
        }

        [ResourceExposure(ResourceScope.None)]
        internal static void MemoryCopy(System.IntPtr dst, System.IntPtr src, System.Int32 count)
        {
            Debug.Assert (dst != src, "dst and src are the same");
            Debug.Assert (dst.ToInt64() < src.ToInt64() || src.ToInt64() + count <= dst.ToInt64(), "overlapping region dst");
            Debug.Assert (src.ToInt64() < dst.ToInt64() || dst.ToInt64() + count <= src.ToInt64(), "overlapping region src");
            Debug.Assert (0 <= count, "negative count");
            unsafe
            {
                var dstSpan = new System.Span<byte>(dst.ToPointer(), count);
                var srcSpan = new System.Span<byte>(src.ToPointer(), count);
                srcSpan.CopyTo(dstSpan);
            }
        }
    }
}
