// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Data.OleDb {

    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.ProviderBase;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Threading;
    using SysTx = System.Transactions;
    using System.Diagnostics.CodeAnalysis;

    public sealed partial class OleDbConnection : DbConnection {
        private static readonly DbConnectionFactory     _connectionFactory = OleDbConnectionFactory.SingletonInstance;

        private DbConnectionOptions     _userConnectionOptions;
        private DbConnectionPoolGroup   _poolGroup;
        private DbConnectionInternal    _innerConnection;
        private int                     _closeCount;          // used to distinguish between different uses of this object, so we don't have to maintain a list of it's children

        private static int _objectTypeCount; // Bid counter
        internal readonly int ObjectID = System.Threading.Interlocked.Increment(ref _objectTypeCount);

        public OleDbConnection() : base() {
            GC.SuppressFinalize(this);
            _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
        }

        // Copy Constructor
        private void CopyFrom(OleDbConnection connection) { // V1.2.3300
            ADP.CheckArgumentNull(connection, "connection");
            _userConnectionOptions = connection.UserConnectionOptions;
            _poolGroup = connection.PoolGroup;
            
            // SQLBU 432115
            //  Match the original connection's behavior for whether the connection was never opened,
            //  but ensure Clone is in the closed state.
            if (DbConnectionClosedNeverOpened.SingletonInstance == connection._innerConnection)
            {
                _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
            }
            else
            {
                _innerConnection = DbConnectionClosedPreviouslyOpened.SingletonInstance;
            }
        }

        /// <devdoc>We use the _closeCount to avoid having to know about all our
        ///  children; instead of keeping a collection of all the objects that
        ///  would be affected by a close, we simply increment the _closeCount
        ///  and have each of our children check to see if they're "orphaned"
        ///  </devdoc>
        internal int CloseCount {
            get {
                return _closeCount;
            }
        }

        internal DbConnectionFactory ConnectionFactory {
            get {
                return _connectionFactory;
            }
        }

        internal DbConnectionOptions ConnectionOptions {
            get {
                System.Data.ProviderBase.DbConnectionPoolGroup poolGroup = PoolGroup;
                return ((null != poolGroup) ? poolGroup.ConnectionOptions : null);
            }
        }

        private string ConnectionString_Get() {
            Bid.Trace( "<prov.DbConnectionHelper.ConnectionString_Get|API> %d#\n", ObjectID);
            bool hidePassword = InnerConnection.ShouldHidePassword;
            DbConnectionOptions connectionOptions = UserConnectionOptions;
            return ((null != connectionOptions) ? connectionOptions.UsersConnectionString(hidePassword) : "");
        }

        private void ConnectionString_Set(string value) {
            DbConnectionPoolKey key = new DbConnectionPoolKey(value);

            ConnectionString_Set(key);
        }

        private void ConnectionString_Set(DbConnectionPoolKey key) {
            DbConnectionOptions connectionOptions = null;
            System.Data.ProviderBase.DbConnectionPoolGroup poolGroup = ConnectionFactory.GetConnectionPoolGroup(key, null, ref connectionOptions);
            DbConnectionInternal connectionInternal = InnerConnection;
            bool flag = connectionInternal.AllowSetConnectionString;
            if (flag) {
                //try {
                    // NOTE: There's a race condition with multiple threads changing
                    //       ConnectionString and any thread throws an exception
                    // Closed->Busy: prevent Open during set_ConnectionString
                    flag = SetInnerConnectionFrom(DbConnectionClosedBusy.SingletonInstance, connectionInternal);
                    if (flag) {
                        _userConnectionOptions = connectionOptions;
                        _poolGroup = poolGroup;
                        _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
                    }
                //}
                //catch {
                //    // recover from exceptions to avoid sticking in busy state
                //    SetInnerConnectionFrom(connectionInternal, DbConnectionClosedBusy.SingletonInstance);
                //    throw;
                //}
            }
            if (!flag) {
                throw ADP.OpenConnectionPropertySet(ADP.ConnectionString, connectionInternal.State);
            }
            if (Bid.TraceOn) {
                string cstr = ((null != connectionOptions) ? connectionOptions.UsersConnectionStringForTrace() : "");
                Bid.Trace("<prov.DbConnectionHelper.ConnectionString_Set|API> %d#, '%ls'\n", ObjectID, cstr);
            }
        }

        internal DbConnectionInternal InnerConnection {
            get {
                return _innerConnection;
            }
        }

        internal System.Data.ProviderBase.DbConnectionPoolGroup PoolGroup {
            get {
                return _poolGroup;
            }
            set {
                // when a poolgroup expires and the connection eventually activates, the pool entry will be replaced
                Debug.Assert(null != value, "null poolGroup");
                _poolGroup = value;
            }
        }

       
        internal DbConnectionOptions UserConnectionOptions {
            get {
                return _userConnectionOptions;
            }
        }

        // Open->ClosedPreviouslyOpened, and doom the internal connection too...
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void Abort(Exception e) {
            DbConnectionInternal innerConnection = _innerConnection;  // Should not cause memory allocation...
            if (ConnectionState.Open == innerConnection.State) {
                Interlocked.CompareExchange(ref _innerConnection, DbConnectionClosedPreviouslyOpened.SingletonInstance, innerConnection);
                innerConnection.DoomThisConnection();
            }

            // NOTE: we put the tracing last, because the ToString() calls (and
            // the Bid.Trace, for that matter) have no reliability contract and
            // will end the reliable try...
            if (e is OutOfMemoryException) {
                Bid.Trace("<prov.DbConnectionHelper.Abort|RES|INFO|CPOOL> %d#, Aborting operation due to asynchronous exception: %ls\n", ObjectID, "OutOfMemory");
            }
            else {
                Bid.Trace("<prov.DbConnectionHelper.Abort|RES|INFO|CPOOL> %d#, Aborting operation due to asynchronous exception: %ls\n", ObjectID, e.ToString());
            }
        }

        internal void AddWeakReference(object value, int tag) {
            InnerConnection.AddWeakReference(value, tag);
        }


        override protected DbCommand CreateDbCommand() {
            DbCommand command = null;
            IntPtr hscp;
            Bid.ScopeEnter(out hscp, "<prov.DbConnectionHelper.CreateDbCommand|API> %d#\n", ObjectID);
            try {
                DbProviderFactory providerFactory = ConnectionFactory.ProviderFactory;
                command = providerFactory.CreateCommand();
                command.Connection = this;
            }
            finally {
                Bid.ScopeLeave(ref hscp);
            }
            return command;
        }


        override protected void Dispose(bool disposing) {
            if (disposing) {
                _userConnectionOptions = null;
                _poolGroup= null;
                Close();
            }
            DisposeMe(disposing);
            base.Dispose(disposing); // notify base classes
        }

        partial void RepairInnerConnection();

        // NOTE: This is just a private helper because OracleClient V1.1 shipped
        // with a different argument name and it's a breaking change to not use
        // the same argument names in V2.0 (VB Named Parameter Binding--Ick)
        private void EnlistDistributedTransactionHelper(System.EnterpriseServices.ITransaction transaction) {

            Bid.Trace( "<prov.DbConnectionHelper.EnlistDistributedTransactionHelper|RES|TRAN> %d#, Connection enlisting in a transaction.\n", ObjectID);
            SysTx.Transaction indigoTransaction = null;

            if (null != transaction) {
                indigoTransaction = SysTx.TransactionInterop.GetTransactionFromDtcTransaction((SysTx.IDtcTransaction)transaction);
            }

            RepairInnerConnection();
            // NOTE: since transaction enlistment involves round trips to the
            // server, we don't want to lock here, we'll handle the race conditions
            // elsewhere.
            InnerConnection.EnlistTransaction(indigoTransaction);

            // NOTE: If this outer connection were to be GC'd while we're
            // enlisting, the pooler would attempt to reclaim the inner connection
            // while we're attempting to enlist; not sure how likely that is but
            // we should consider a GC.KeepAlive(this) here.
            GC.KeepAlive(this);
        }

        override public void EnlistTransaction(SysTx.Transaction transaction) {

            Bid.Trace( "<prov.DbConnectionHelper.EnlistTransaction|RES|TRAN> %d#, Connection enlisting in a transaction.\n", ObjectID);

            // If we're currently enlisted in a transaction and we were called
            // on the EnlistTransaction method (Whidbey) we're not allowed to
            // enlist in a different transaction.

            DbConnectionInternal innerConnection = InnerConnection;

            // NOTE: since transaction enlistment involves round trips to the
            // server, we don't want to lock here, we'll handle the race conditions
            // elsewhere.
            SysTx.Transaction enlistedTransaction = innerConnection.EnlistedTransaction;
            if (enlistedTransaction != null) {
                // Allow calling enlist if already enlisted (no-op)
                if (enlistedTransaction.Equals(transaction)) {
                    return;
                }

                // Allow enlisting in a different transaction if the enlisted transaction has completed.
                if (enlistedTransaction.TransactionInformation.Status == SysTx.TransactionStatus.Active)
                {
                    throw ADP.TransactionPresent();
                }
            }
            RepairInnerConnection();
            InnerConnection.EnlistTransaction(transaction);

            // NOTE: If this outer connection were to be GC'd while we're
            // enlisting, the pooler would attempt to reclaim the inner connection
            // while we're attempting to enlist; not sure how likely that is but
            // we should consider a GC.KeepAlive(this) here.
            GC.KeepAlive(this);
        }

        private DbMetaDataFactory GetMetaDataFactory(DbConnectionInternal internalConnection) {
            return ConnectionFactory.GetMetaDataFactory(_poolGroup, internalConnection);
        }

        internal DbMetaDataFactory GetMetaDataFactoryInternal(DbConnectionInternal internalConnection) {
            return GetMetaDataFactory(internalConnection);
        }

        override public  DataTable GetSchema() {
            return this.GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);
        }

        override public DataTable GetSchema(string collectionName) {
            return this.GetSchema(collectionName, null);
        }

        override public DataTable GetSchema(string collectionName, string[] restrictionValues) {
            // NOTE: This is virtual because not all providers may choose to support
            //       returning schema data
            return InnerConnection.GetSchema(ConnectionFactory, PoolGroup, this, collectionName, restrictionValues);
        }

        internal void NotifyWeakReference(int message) {
            InnerConnection.NotifyWeakReference(message);
        }

        internal void PermissionDemand() {
            Debug.Assert(DbConnectionClosedConnecting.SingletonInstance == _innerConnection, "not connecting");

            System.Data.ProviderBase.DbConnectionPoolGroup poolGroup = PoolGroup;
            DbConnectionOptions connectionOptions = ((null != poolGroup) ? poolGroup.ConnectionOptions : null);
            if ((null == connectionOptions) || connectionOptions.IsEmpty) {
                throw ADP.NoConnectionString();
            }

            DbConnectionOptions userConnectionOptions = UserConnectionOptions;
            Debug.Assert(null != userConnectionOptions, "null UserConnectionOptions");

            userConnectionOptions.DemandPermission();
        }

        internal void RemoveWeakReference(object value) {
            InnerConnection.RemoveWeakReference(value);
        }

        // OpenBusy->Closed (previously opened)
        // Connecting->Open
        internal void SetInnerConnectionEvent(DbConnectionInternal to) {
            // Set's the internal connection without verifying that it's a specific value
            Debug.Assert(null != _innerConnection, "null InnerConnection");
            Debug.Assert(null != to, "to null InnerConnection");

            ConnectionState originalState = _innerConnection.State & ConnectionState.Open;
            ConnectionState currentState = to.State & ConnectionState.Open;

            if ((originalState != currentState) && (ConnectionState.Closed == currentState)) {
                // Increment the close count whenever we switch to Closed
                unchecked { _closeCount++; }
            }

            _innerConnection = to;

            if (ConnectionState.Closed == originalState && ConnectionState.Open == currentState) {
                OnStateChange(DbConnectionInternal.StateChangeOpen);
            }
            else if (ConnectionState.Open == originalState && ConnectionState.Closed == currentState) {
                OnStateChange(DbConnectionInternal.StateChangeClosed);
            }
            else {
                Debug.Assert(false, "unexpected state switch");
                if (originalState != currentState) {
                    OnStateChange(new StateChangeEventArgs(originalState, currentState));
                }
            }
        }

        // this method is used to securely change state with the resource being
        // the open connection protected by the connectionstring via a permission demand

        // Closed->Connecting: prevent set_ConnectionString during Open
        // Open->OpenBusy: guarantee internal connection is returned to correct pool
        // Closed->ClosedBusy: prevent Open during set_ConnectionString
        internal bool SetInnerConnectionFrom(DbConnectionInternal to, DbConnectionInternal from) {
            // Set's the internal connection, verifying that it's a specific value before doing so.
            Debug.Assert(null != _innerConnection, "null InnerConnection");
            Debug.Assert(null != from, "from null InnerConnection");
            Debug.Assert(null != to, "to null InnerConnection");

            bool result = (from == Interlocked.CompareExchange<DbConnectionInternal>(ref _innerConnection, to, from));
            return result;
        }

        // ClosedBusy->Closed (never opened)
        // Connecting->Closed (exception during open, return to previous closed state)
        internal void SetInnerConnectionTo(DbConnectionInternal to) {
            // Set's the internal connection without verifying that it's a specific value
            Debug.Assert(null != _innerConnection, "null InnerConnection");
            Debug.Assert(null != to, "to null InnerConnection");
            _innerConnection = to;
        }
    }
}

