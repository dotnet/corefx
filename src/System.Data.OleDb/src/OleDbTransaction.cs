// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Data.ProviderBase;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Data.OleDb {
    public sealed class OleDbTransaction : DbTransaction {

        private readonly OleDbTransaction _parentTransaction; // strong reference to keep parent alive
        private readonly System.Data.IsolationLevel _isolationLevel;

        private WeakReference _nestedTransaction; // child transactions
        private WrappedTransaction _transaction;

        internal OleDbConnection _parentConnection;

        private static int _objectTypeCount; // Bid counter
        internal readonly int _objectID = System.Threading.Interlocked.Increment(ref _objectTypeCount);

        private sealed class WrappedTransaction : WrappedIUnknown {

            private bool _mustComplete;

            internal WrappedTransaction(UnsafeNativeMethods.ITransactionLocal transaction, int isolevel, out OleDbHResult hr) : base(transaction) {
                int transactionLevel = 0;
                Bid.Trace("<oledb.ITransactionLocal.StartTransaction|API|OLEDB>\n");
                RuntimeHelpers.PrepareConstrainedRegions();
                try { } finally {
                    hr = transaction.StartTransaction(isolevel, 0, IntPtr.Zero, out transactionLevel);
                    if (0 <= hr) {
                        _mustComplete = true;
                    }
                }
                Bid.Trace("<oledb.ITransactionLocal.StartTransaction|API|OLEDB|RET> %08X{HRESULT}\n", hr);
            }

            internal bool MustComplete {
                get { return _mustComplete; }
            }

            internal OleDbHResult Abort() {
                Debug.Assert(_mustComplete, "transaction already completed");
                OleDbHResult hr;
                bool mustRelease = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try {
                    DangerousAddRef(ref mustRelease);

                    Bid.Trace("<oledb.ITransactionLocal.Abort|API|OLEDB> handle=%p\n", base.handle);
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try { } finally {
                        hr = (OleDbHResult)NativeOledbWrapper.ITransactionAbort(DangerousGetHandle());
                        _mustComplete = false;
                    }
                    Bid.Trace("<oledb.ITransactionLocal.Abort|API|OLEDB|RET> %08X{HRESULT}\n", hr);
                }
                finally {
                    if (mustRelease) {
                        DangerousRelease();
                    }
                }
                return hr;
            }

            internal OleDbHResult Commit() {
                Debug.Assert(_mustComplete, "transaction already completed");
                OleDbHResult hr;
                bool mustRelease = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try {
                    DangerousAddRef(ref mustRelease);

                    Bid.Trace("<oledb.ITransactionLocal.Commit|API|OLEDB> handle=%p\n", base.handle);
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try { } finally {
                        hr = (OleDbHResult)NativeOledbWrapper.ITransactionCommit(DangerousGetHandle());
                        if ((0 <= (int)hr) || (OleDbHResult.XACT_E_NOTRANSACTION == hr)) {
                            _mustComplete = false;
                        }
                    }
                    Bid.Trace("<oledb.ITransactionLocal.Commit|API|OLEDB|RET> %08X{HRESULT}\n", hr);
                }
                finally {
                    if (mustRelease) {
                        DangerousRelease();
                    }
                }
                return hr;
            }

            override protected bool ReleaseHandle() {
                if (_mustComplete && (IntPtr.Zero != base.handle)) {
                    Bid.Trace("<oledb.ITransactionLocal.Abort|API|OLEDB|INFO> handle=%p\n", base.handle);
                    OleDbHResult hr = (OleDbHResult)NativeOledbWrapper.ITransactionAbort(base.handle);
                    _mustComplete = false;
                    Bid.Trace("<oledb.ITransactionLocal.Abort|API|OLEDB|INFO|RET> %08X{HRESULT}\n", hr);
                }
                return base.ReleaseHandle();
            }
        }

        internal OleDbTransaction(OleDbConnection connection, OleDbTransaction transaction, IsolationLevel isolevel) {

            _parentConnection = connection;
            _parentTransaction = transaction;

            switch(isolevel) {
            case IsolationLevel.Unspecified: // OLE DB doesn't support this isolevel on local transactions
                isolevel = IsolationLevel.ReadCommitted;
                break;
            case IsolationLevel.Chaos:
            case IsolationLevel.ReadUncommitted:
            case IsolationLevel.ReadCommitted:
            case IsolationLevel.RepeatableRead:
            case IsolationLevel.Serializable:
            case IsolationLevel.Snapshot:
                break;
            default:
                throw ADP.InvalidIsolationLevel(isolevel); // MDAC 74269
            }
            _isolationLevel = isolevel;
        }

        new public OleDbConnection Connection { // MDAC 66655
            get {
                return _parentConnection;
            }
        }

        override protected DbConnection DbConnection {
            get {
                return Connection;
            }
        }

        override public IsolationLevel IsolationLevel {
            get {
                if (null == _transaction) {
                    throw ADP.TransactionZombied(this);
                }
                return _isolationLevel;
            }
        }

        internal int ObjectID {
            get {
                return _objectID;
            }
        }

        internal OleDbTransaction Parent {
            get {
                return _parentTransaction;
            }
        }

        public OleDbTransaction Begin(IsolationLevel isolevel) {

            IntPtr hscp;
            Bid.ScopeEnter(out hscp, "<oledb.OleDbTransaction.Begin|API> %d#, isolevel=%d{IsolationLevel}", ObjectID, (int)isolevel);
            try {
                if (null == _transaction) {
                    throw ADP.TransactionZombied(this);
                }
                else if ((null != _nestedTransaction) && _nestedTransaction.IsAlive) {
                    throw ADP.ParallelTransactionsNotSupported(Connection);
                }
                // either the connection will be open or this will be a zombie

                OleDbTransaction transaction = new OleDbTransaction(_parentConnection, this, isolevel);
                _nestedTransaction = new WeakReference(transaction, false);

                UnsafeNativeMethods.ITransactionLocal wrapper = null;
                try {
                    wrapper = (UnsafeNativeMethods.ITransactionLocal)_transaction.ComWrapper();
                    transaction.BeginInternal(wrapper);
                }
                finally {
                    if (null != wrapper) {
                        Marshal.ReleaseComObject(wrapper);
                    }
                }
                return transaction;
            }
            finally {
                Bid.ScopeLeave(ref hscp);
            }
        }

        public OleDbTransaction Begin() {
            return Begin(IsolationLevel.ReadCommitted);
        }

        internal void BeginInternal(UnsafeNativeMethods.ITransactionLocal transaction) {
            OleDbHResult hr;
            _transaction = new WrappedTransaction(transaction, (int) _isolationLevel, out hr);
            if (hr < 0) {
                _transaction.Dispose();
                _transaction = null;                
                ProcessResults(hr);
            }
        }

        override public void Commit() {

            IntPtr hscp;
            Bid.ScopeEnter(out hscp, "<oledb.OleDbTransaction.Commit|API> %d#", ObjectID);
            try {
                if (null == _transaction) {
                    throw ADP.TransactionZombied(this);
                }
                CommitInternal();
            }
            finally {
                Bid.ScopeLeave(ref hscp);
            }
        }

        private void CommitInternal() {
            if (null == _transaction) {
                return;
            }
            if (null != _nestedTransaction) {
                OleDbTransaction transaction = (OleDbTransaction) _nestedTransaction.Target;
                if ((null != transaction) && _nestedTransaction.IsAlive) {
                    transaction.CommitInternal();
                }
                _nestedTransaction = null;
            }
            OleDbHResult hr = _transaction.Commit();
            if (!_transaction.MustComplete) {
                _transaction.Dispose();
                _transaction = null;

                DisposeManaged();
            }
            if (hr < 0) {
                // if an exception is thrown, user can try to commit their transaction again
                ProcessResults(hr);
            }
        }

        /*public OleDbCommand CreateCommand() { // MDAC 68309
            OleDbCommand cmd = Connection.CreateCommand();
            cmd.Transaction = this;
            return cmd;
        }

        IDbCommand IDbTransaction.CreateCommand() {
            return CreateCommand();
        }*/

        protected override void Dispose(bool disposing) {
            if (disposing) {
                DisposeManaged();
                RollbackInternal(false);
            }
            base.Dispose(disposing);
        }

        private void DisposeManaged() {
            if (null != _parentTransaction) {
                _parentTransaction._nestedTransaction = null;
                //_parentTransaction = null;
            }
            else if (null != _parentConnection) { // MDAC 67287
                _parentConnection.LocalTransaction = null;
            }
            _parentConnection = null;
        }

        private void ProcessResults(OleDbHResult hr) {
            Exception e = OleDbConnection.ProcessResults(hr, _parentConnection, this);
            if (null != e) { throw e; }
        }

        override public void Rollback() {
            IntPtr hscp;
            Bid.ScopeEnter(out hscp, "<oledb.OleDbTransaction.Rollback|API> %d#", ObjectID);
            try {
                if (null == _transaction) {
                    throw ADP.TransactionZombied(this);
                }
                DisposeManaged();
                RollbackInternal(true); // no recover if this throws an exception
            }
            finally {
                Bid.ScopeLeave(ref hscp);
            }
        }

        /*protected virtual*/internal OleDbHResult RollbackInternal(bool exceptionHandling) {
            OleDbHResult hr = 0;
            if (null != _transaction) {
                if (null != _nestedTransaction) {
                    OleDbTransaction transaction = (OleDbTransaction) _nestedTransaction.Target;
                    if ((null != transaction) && _nestedTransaction.IsAlive) {
                        hr = transaction.RollbackInternal(exceptionHandling);
                        if (exceptionHandling && (hr < 0)) {
                            SafeNativeMethods.Wrapper.ClearErrorInfo();
                            return hr;
                        }
                    }
                    _nestedTransaction = null;
                }
                hr = _transaction.Abort();
                _transaction.Dispose();
                _transaction = null;
                if (hr < 0) {
                    if (exceptionHandling) {
                        ProcessResults(hr);
                    }
                    else {
                        SafeNativeMethods.Wrapper.ClearErrorInfo();
                    }
                }
            }
            return hr;
        }

        static internal OleDbTransaction TransactionLast(OleDbTransaction head) {
            if (null != head._nestedTransaction) {
                OleDbTransaction current = (OleDbTransaction) head._nestedTransaction.Target;
                if ((null != current) && head._nestedTransaction.IsAlive) {
                    return TransactionLast(current);
                }
            }
            return head;
        }

        static internal OleDbTransaction TransactionUpdate(OleDbTransaction transaction) {
            if ((null != transaction) && (null == transaction._transaction)) {
                return null;
            }
            return transaction;
        }
    }
}
