// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Data.OleDb {

    using System;
    using System.Data.Common;
    using System.Data.ProviderBase;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Runtime.ConstrainedExecution;

    internal sealed class DualCoTaskMem : SafeHandle {

        private IntPtr handle2;   // this must be protected so derived classes can use out params.

        private DualCoTaskMem() : base(IntPtr.Zero, true) {
            this.handle2 = IntPtr.Zero;
        }

        // IDBInfo.GetLiteralInfo
        internal DualCoTaskMem(UnsafeNativeMethods.IDBInfo dbInfo, int[] literals, out int literalCount, out IntPtr literalInfo, out OleDbHResult hr) : this() {
            int count = (null != literals) ? literals.Length : 0;

            Bid.Trace("<oledb.IDBInfo.GetLiteralInfo|API|OLEDB>\n");
            hr = dbInfo.GetLiteralInfo(count, literals, out literalCount, out base.handle, out this.handle2);
            literalInfo = base.handle;
            Bid.Trace("<oledb.IDBInfo.GetLiteralInfo|API|OLEDB|RET> %08X{HRESULT}\n", hr);
        }

        // IColumnsInfo.GetColumnInfo
        internal DualCoTaskMem(UnsafeNativeMethods.IColumnsInfo columnsInfo, out IntPtr columnCount, out IntPtr columnInfos, out OleDbHResult hr) : this() {
            Bid.Trace("<oledb.IColumnsInfo.GetColumnInfo|API|OLEDB>\n");
            hr = columnsInfo.GetColumnInfo(out columnCount, out base.handle, out this.handle2);
            columnInfos = base.handle;
            Bid.Trace("<oledb.IColumnsInfo.GetColumnInfo|API|OLEDB|RET> %08X{HRESULT}\n", hr);
        }

        // IDBSchemaRowset.GetSchemas
        internal DualCoTaskMem(UnsafeNativeMethods.IDBSchemaRowset dbSchemaRowset, out int schemaCount, out IntPtr schemaGuids, out IntPtr schemaRestrictions, out OleDbHResult hr) : this() {
            Bid.Trace("<oledb.IDBSchemaRowset.GetSchemas|API|OLEDB>\n");
            hr = dbSchemaRowset.GetSchemas(out schemaCount, out base.handle, out this.handle2);
            schemaGuids = base.handle;
            schemaRestrictions = this.handle2;
            Bid.Trace("<oledb.IDBSchemaRowset.GetSchemas|API|OLEDB|RET> %08X{HRESULT}\n", hr);
        }

        internal DualCoTaskMem(UnsafeNativeMethods.IColumnsRowset icolumnsRowset, out IntPtr cOptColumns, out OleDbHResult hr) : base(IntPtr.Zero, true) {
            Bid.Trace("<oledb.IColumnsRowset.GetAvailableColumns|API|OLEDB>\n");
            hr = icolumnsRowset.GetAvailableColumns(out cOptColumns, out base.handle);
            Bid.Trace("<oledb.IColumnsRowset.GetAvailableColumns|API|OLEDB|RET> %08X{HRESULT}\n", hr);
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
                Bid.Trace("<oledb.IChapteredRowset.ReleaseChapter|API|OLEDB> Chapter=%Id\n", chapter);
                OleDbHResult hr = (OleDbHResult)NativeOledbWrapper.IChapteredRowsetReleaseChapter(base.handle, chapter);
                Bid.Trace("<oledb.IChapteredRowset.ReleaseChapter|API|OLEDB|RET> %08X{HRESULT}\n", hr);
            }
            return base.ReleaseHandle();
        }
    }
}
