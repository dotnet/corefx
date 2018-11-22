// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Transactions;

namespace System.Data.ProviderBase
{
    sealed internal partial class DbConnectionPool
    {
        private enum State
        {
            Initializing,
            Running,
            ShuttingDown,
        }

        // This class is a way to stash our cloned Tx key for later disposal when it's no longer needed.
        // We can't get at the key in the dictionary without enumerating entries, so we stash an extra
        // copy as part of the value.
        sealed private class TransactedConnectionList : List<DbConnectionInternal>
        {
            private Transaction _transaction;
            internal TransactedConnectionList(int initialAllocation, Transaction tx) : base(initialAllocation)
            {
                _transaction = tx;
            }

            internal void Dispose()
            {
                if (null != _transaction)
                {
                    _transaction.Dispose();
                }
            }
        }

        private sealed class PendingGetConnection
        {
            public PendingGetConnection(long dueTime, DbConnection owner, TaskCompletionSource<DbConnectionInternal> completion, DbConnectionOptions userOptions)
            {
                DueTime = dueTime;
                Owner = owner;
                Completion = completion;
            }
            public long DueTime { get; private set; }
            public DbConnection Owner { get; private set; }
            public TaskCompletionSource<DbConnectionInternal> Completion { get; private set; }
            public DbConnectionOptions UserOptions { get; private set; }
        }

        sealed private class TransactedConnectionPool
        {
            Dictionary<Transaction, TransactedConnectionList> _transactedCxns;

            DbConnectionPool _pool;

            private static int _objectTypeCount; // Bid counter
            internal readonly int _objectID = System.Threading.Interlocked.Increment(ref _objectTypeCount);

            internal TransactedConnectionPool(DbConnectionPool pool)
            {
                Debug.Assert(null != pool, "null pool?");

                _pool = pool;
                _transactedCxns = new Dictionary<Transaction, TransactedConnectionList>();
            }

            internal int ObjectID
            {
                get
                {
                    return _objectID;
                }
            }

            internal DbConnectionPool Pool
            {
                get
                {
                    return _pool;
                }
            }

            internal DbConnectionInternal GetTransactedObject(Transaction transaction)
            {
                Debug.Assert(null != transaction, "null transaction?");

                DbConnectionInternal transactedObject = null;

                TransactedConnectionList connections;
                bool txnFound = false;

                lock (_transactedCxns)
                {
                    txnFound = _transactedCxns.TryGetValue(transaction, out connections);
                }

                // NOTE: GetTransactedObject is only used when AutoEnlist = True and the ambient transaction 
                //   (Sys.Txns.Txn.Current) is still valid/non-null. This, in turn, means that we don't need 
                //   to worry about a pending asynchronous TransactionCompletedEvent to trigger processing in
                //   TransactionEnded below and potentially wipe out the connections list underneath us. It
                //   is similarly alright if a pending addition to the connections list in PutTransactedObject
                //   below is not completed prior to the lock on the connections object here...getting a new
                //   connection is probably better than unnecessarily locking
                if (txnFound)
                {
                    Debug.Assert(connections != null);

                    // synchronize multi-threaded access with PutTransactedObject (TransactionEnded should
                    //   not be a concern, see comments above)
                    lock (connections)
                    {
                        int i = connections.Count - 1;
                        if (0 <= i)
                        {
                            transactedObject = connections[i];
                            connections.RemoveAt(i);
                        }
                    }
                }
                
                return transactedObject;
            }

            internal void PutTransactedObject(Transaction transaction, DbConnectionInternal transactedObject)
            {
                Debug.Assert(null != transaction, "null transaction?");
                Debug.Assert(null != transactedObject, "null transactedObject?");

                TransactedConnectionList connections;
                bool txnFound = false;

                // NOTE: because TransactionEnded is an asynchronous notification, there's no guarantee
                //   around the order in which PutTransactionObject and TransactionEnded are called. 

                lock (_transactedCxns)
                {
                    // Check if a transacted pool has been created for this transaction
                    if (txnFound = _transactedCxns.TryGetValue(transaction, out connections))
                    {
                        Debug.Assert(connections != null);

                        // synchronize multi-threaded access with GetTransactedObject
                        lock (connections)
                        {
                            Debug.Assert(0 > connections.IndexOf(transactedObject), "adding to pool a second time?");
                            connections.Add(transactedObject);
                        }
                    }
                }

                // CONSIDER: the following code is more complicated than it needs to be to avoid cloning the 
                //   transaction and allocating memory within a lock. Is that complexity really necessary?
                if (!txnFound)
                {
                    // create the transacted pool, making sure to clone the associated transaction
                    //   for use as a key in our internal dictionary of transactions and connections
                    Transaction transactionClone = null;
                    TransactedConnectionList newConnections = null;

                    try
                    {
                        transactionClone = transaction.Clone();
                        newConnections = new TransactedConnectionList(2, transactionClone); // start with only two connections in the list; most times we won't need that many.

                        lock (_transactedCxns)
                        {
                            // NOTE: in the interim between the locks on the transacted pool (this) during 
                            //   execution of this method, another thread (threadB) may have attempted to 
                            //   add a different connection to the transacted pool under the same 
                            //   transaction. As a result, threadB may have completed creating the
                            //   transacted pool while threadA was processing the above instructions.
                            if (txnFound = _transactedCxns.TryGetValue(transaction, out connections))
                            {
                                Debug.Assert(connections != null);

                                // synchronize multi-threaded access with GetTransactedObject
                                lock (connections)
                                {
                                    Debug.Assert(0 > connections.IndexOf(transactedObject), "adding to pool a second time?");
                                    connections.Add(transactedObject);
                                }
                            }
                            else
                            {
                                // add the connection/transacted object to the list
                                newConnections.Add(transactedObject);

                                _transactedCxns.Add(transactionClone, newConnections);
                                transactionClone = null; // we've used it -- don't throw it or the TransactedConnectionList that references it away.                                
                            }
                        }
                    }
                    finally
                    {
                        if (null != transactionClone)
                        {
                            if (newConnections != null)
                            {
                                // another thread created the transaction pool and thus the new 
                                //   TransactedConnectionList was not used, so dispose of it and
                                //   the transaction clone that it incorporates.
                                newConnections.Dispose();
                            }
                            else
                            {
                                // memory allocation for newConnections failed...clean up unused transactionClone
                                transactionClone.Dispose();
                            }
                        }
                    }
                }
            }

            internal void TransactionEnded(Transaction transaction, DbConnectionInternal transactedObject)
            {
                TransactedConnectionList connections;
                int entry = -1;

                // NOTE: because TransactionEnded is an asynchronous notification, there's no guarantee
                //   around the order in which PutTransactionObject and TransactionEnded are called. As
                //   such, it is possible that the transaction does not yet have a pool created.

                // TODO: is this a plausible and/or likely scenario? Do we need to have a mechanism to ensure
                // TODO:   that the pending creation of a transacted pool for this transaction is aborted when
                // TODO:   PutTransactedObject finally gets some CPU time?

                lock (_transactedCxns)
                {
                    if (_transactedCxns.TryGetValue(transaction, out connections))
                    {
                        Debug.Assert(connections != null);

                        bool shouldDisposeConnections = false;

                        // Lock connections to avoid conflict with GetTransactionObject
                        lock (connections)
                        {
                            entry = connections.IndexOf(transactedObject);

                            if (entry >= 0)
                            {
                                connections.RemoveAt(entry);
                            }

                            // Once we've completed all the ended notifications, we can
                            // safely remove the list from the transacted pool.
                            if (0 >= connections.Count)
                            {
                                _transactedCxns.Remove(transaction);

                                // we really need to dispose our connection list; it may have 
                                // native resources via the tx and GC may not happen soon enough.
                                shouldDisposeConnections = true;
                            }
                        }

                        if (shouldDisposeConnections)
                        {
                            connections.Dispose();
                        }
                    }
                }

                // If (and only if) we found the connection in the list of
                // connections, we'll put it back...
                if (0 <= entry)
                {
                    Pool.PutObjectFromTransactedPool(transactedObject);
                }
            }

        }

        private sealed class PoolWaitHandles
        {
            private readonly Semaphore _poolSemaphore;
            private readonly ManualResetEvent _errorEvent;

            // Using a Mutex requires ThreadAffinity because SQL CLR can swap
            // the underlying Win32 thread associated with a managed thread in preemptive mode.
            // Using an AutoResetEvent does not have that complication.
            private readonly Semaphore _creationSemaphore;

            private readonly WaitHandle[] _handlesWithCreate;
            private readonly WaitHandle[] _handlesWithoutCreate;

            internal PoolWaitHandles()
            {
                _poolSemaphore = new Semaphore(0, MAX_Q_SIZE);
                _errorEvent = new ManualResetEvent(false);
                _creationSemaphore = new Semaphore(1, 1);

                _handlesWithCreate = new WaitHandle[] { _poolSemaphore, _errorEvent, _creationSemaphore };
                _handlesWithoutCreate = new WaitHandle[] { _poolSemaphore, _errorEvent };
            }


            internal Semaphore CreationSemaphore
            {
                get { return _creationSemaphore; }
            }

            internal ManualResetEvent ErrorEvent
            {
                get { return _errorEvent; }
            }

            internal Semaphore PoolSemaphore
            {
                get { return _poolSemaphore; }
            }

            internal WaitHandle[] GetHandles(bool withCreate)
            {
                return withCreate ? _handlesWithCreate : _handlesWithoutCreate;
            }
        }

        private const int MAX_Q_SIZE = (int)0x00100000;

        // The order of these is important; we want the WaitAny call to be signaled
        // for a free object before a creation signal.  Only the index first signaled
        // object is returned from the WaitAny call.
        private const int SEMAPHORE_HANDLE = (int)0x0;
        private const int ERROR_HANDLE = (int)0x1;
        private const int CREATION_HANDLE = (int)0x2;
        private const int BOGUS_HANDLE = (int)0x3;


        private const int ERROR_WAIT_DEFAULT = 5 * 1000; // 5 seconds

        // we do want a testable, repeatable set of generated random numbers
        private static readonly Random s_random = new Random(5101977); // Value obtained from Dave Driver

        private readonly int _cleanupWait;
        private readonly DbConnectionPoolIdentity _identity;

        private readonly DbConnectionFactory _connectionFactory;
        private readonly DbConnectionPoolGroup _connectionPoolGroup;
        private readonly DbConnectionPoolGroupOptions _connectionPoolGroupOptions;
        private DbConnectionPoolProviderInfo _connectionPoolProviderInfo;

        private State _state;

        private readonly ConcurrentStack<DbConnectionInternal> _stackOld = new ConcurrentStack<DbConnectionInternal>();
        private readonly ConcurrentStack<DbConnectionInternal> _stackNew = new ConcurrentStack<DbConnectionInternal>();

        private readonly ConcurrentQueue<PendingGetConnection> _pendingOpens = new ConcurrentQueue<PendingGetConnection>();
        private int _pendingOpensWaiting = 0;

        private readonly WaitCallback _poolCreateRequest;

        private int _waitCount;
        private readonly PoolWaitHandles _waitHandles;

        private Exception _resError;
        private volatile bool _errorOccurred;

        private int _errorWait;
        private Timer _errorTimer;

        private Timer _cleanupTimer;

        private readonly TransactedConnectionPool _transactedConnectionPool;

        private readonly List<DbConnectionInternal> _objectList;
        private int _totalObjects;


        // only created by DbConnectionPoolGroup.GetConnectionPool
        internal DbConnectionPool(
                            DbConnectionFactory connectionFactory,
                            DbConnectionPoolGroup connectionPoolGroup,
                            DbConnectionPoolIdentity identity,
                            DbConnectionPoolProviderInfo connectionPoolProviderInfo)
        {
            Debug.Assert(null != connectionPoolGroup, "null connectionPoolGroup");

            if ((null != identity) && identity.IsRestricted)
            {
                throw ADP.InternalError(ADP.InternalErrorCode.AttemptingToPoolOnRestrictedToken);
            }

            _state = State.Initializing;

            lock (s_random)
            { // Random.Next is not thread-safe
                _cleanupWait = s_random.Next(12, 24) * 10 * 1000; // 2-4 minutes in 10 sec intervals
            }

            _connectionFactory = connectionFactory;
            _connectionPoolGroup = connectionPoolGroup;
            _connectionPoolGroupOptions = connectionPoolGroup.PoolGroupOptions;
            _connectionPoolProviderInfo = connectionPoolProviderInfo;
            _identity = identity;

            _waitHandles = new PoolWaitHandles();

            _errorWait = ERROR_WAIT_DEFAULT;
            _errorTimer = null;  // No error yet.

            _objectList = new List<DbConnectionInternal>(MaxPoolSize);

            _transactedConnectionPool = new TransactedConnectionPool(this); // initialize irrespective of platform

            _poolCreateRequest = new WaitCallback(PoolCreateRequest); // used by CleanupCallback
            _state = State.Running;

            //_cleanupTimer & QueuePoolCreateRequest is delayed until DbConnectionPoolGroup calls
            // StartBackgroundCallbacks after pool is actually in the collection
        }

        private int CreationTimeout
        {
            get { return PoolGroupOptions.CreationTimeout; }
        }

        internal int Count
        {
            get { return _totalObjects; }
        }

        internal DbConnectionFactory ConnectionFactory
        {
            get { return _connectionFactory; }
        }

        internal bool ErrorOccurred
        {
            get { return _errorOccurred; }
        }

        private bool HasTransactionAffinity
        {
            get { return PoolGroupOptions.HasTransactionAffinity; }
        }

        internal TimeSpan LoadBalanceTimeout
        {
            get { return PoolGroupOptions.LoadBalanceTimeout; }
        }

        private bool NeedToReplenish
        {
            get
            {
                if (State.Running != _state) // Don't allow connection create when not running.
                    return false;

                int totalObjects = Count;

                if (totalObjects >= MaxPoolSize)
                    return false;

                if (totalObjects < MinPoolSize)
                    return true;

                int freeObjects = (_stackNew.Count + _stackOld.Count);
                int waitingRequests = _waitCount;
                bool needToReplenish = (freeObjects < waitingRequests) || ((freeObjects == waitingRequests) && (totalObjects > 1));

                return needToReplenish;
            }
        }

        internal DbConnectionPoolIdentity Identity
        {
            get { return _identity; }
        }

        internal bool IsRunning
        {
            get { return State.Running == _state; }
        }

        private int MaxPoolSize
        {
            get { return PoolGroupOptions.MaxPoolSize; }
        }

        private int MinPoolSize
        {
            get { return PoolGroupOptions.MinPoolSize; }
        }

        internal DbConnectionPoolGroup PoolGroup
        {
            get { return _connectionPoolGroup; }
        }

        internal DbConnectionPoolGroupOptions PoolGroupOptions
        {
            get { return _connectionPoolGroupOptions; }
        }

        internal DbConnectionPoolProviderInfo ProviderInfo
        {
            get { return _connectionPoolProviderInfo; }
        }

        internal bool UseLoadBalancing
        {
            get { return PoolGroupOptions.UseLoadBalancing; }
        }

        private bool UsingIntegrateSecurity
        {
            get { return (null != _identity && DbConnectionPoolIdentity.NoIdentity != _identity); }
        }

        private void CleanupCallback(object state)
        {
            // Called when the cleanup-timer ticks over.

            // This is the automatic pruning method.  Every period, we will
            // perform a two-step process:
            //
            // First, for each free object above MinPoolSize, we will obtain a
            // semaphore representing one object and destroy one from old stack.
            // We will continue this until we either reach MinPoolSize, we are
            // unable to obtain a free object, or we have exhausted all the
            // objects on the old stack.
            //
            // Second we move all free objects on the new stack to the old stack.
            // So, every period the objects on the old stack are destroyed and
            // the objects on the new stack are pushed to the old stack.  All
            // objects that are currently out and in use are not on either stack.
            //
            // With this logic, objects are pruned from the pool if unused for
            // at least one period but not more than two periods.


            // Destroy free objects that put us above MinPoolSize from old stack.
            while (Count > MinPoolSize)
            { // While above MinPoolSize...
                if (_waitHandles.PoolSemaphore.WaitOne(0))
                {
                    // We obtained a objects from the semaphore.
                    DbConnectionInternal obj;

                    if (_stackOld.TryPop(out obj))
                    {
                        Debug.Assert(obj != null, "null connection is not expected");
                        // If we obtained one from the old stack, destroy it.

                        // Transaction roots must survive even aging out (TxEnd event will clean them up).
                        bool shouldDestroy = true;
                        lock (obj)
                        {    // Lock to prevent race condition window between IsTransactionRoot and shouldDestroy assignment
                            if (obj.IsTransactionRoot)
                            {
                                shouldDestroy = false;
                            }
                        }

                        // !!!!!!!!!! WARNING !!!!!!!!!!!!!
                        //   ONLY touch obj after lock release if shouldDestroy is false!!!  Otherwise, it may be destroyed
                        //   by transaction-end thread!

                        // Note that there is a minor race condition between this task and the transaction end event, if the latter runs 
                        //  between the lock above and the SetInStasis call below. The reslult is that the stasis counter may be
                        //  incremented without a corresponding decrement (the transaction end task is normally expected
                        //  to decrement, but will only do so if the stasis flag is set when it runs). I've minimized the size
                        //  of the window, but we aren't totally eliminating it due to SetInStasis needing to do bid tracing, which
                        //  we don't want to do under this lock, if possible. It should be possible to eliminate this race condition with
                        //  more substantial re-architecture of the pool, but we don't have the time to do that work for the current release.

                        if (shouldDestroy)
                        {
                            DestroyObject(obj);
                        }
                        else
                        {
                            obj.SetInStasis();
                        }
                    }
                    else
                    {
                        // Else we exhausted the old stack (the object the
                        // semaphore represents is on the new stack), so break.
                        _waitHandles.PoolSemaphore.Release(1);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // Push to the old-stack.  For each free object, move object from
            // new stack to old stack.
            if (_waitHandles.PoolSemaphore.WaitOne(0))
            {
                for (; ;)
                {
                    DbConnectionInternal obj;

                    if (!_stackNew.TryPop(out obj))
                        break;

                    Debug.Assert(obj != null, "null connection is not expected");
                    Debug.Assert(!obj.IsEmancipated, "pooled object not in pool");
                    Debug.Assert(obj.CanBePooled, "pooled object is not poolable");

                    _stackOld.Push(obj);
                }
                _waitHandles.PoolSemaphore.Release(1);
            }

            // Queue up a request to bring us up to MinPoolSize
            QueuePoolCreateRequest();
        }

        internal void Clear()
        {
            DbConnectionInternal obj;

            // First, quickly doom everything.
            lock (_objectList)
            {
                int count = _objectList.Count;

                for (int i = 0; i < count; ++i)
                {
                    obj = _objectList[i];

                    if (null != obj)
                    {
                        obj.DoNotPoolThisConnection();
                    }
                }
            }

            // Second, dispose of all the free connections.
            while (_stackNew.TryPop(out obj))
            {
                Debug.Assert(obj != null, "null connection is not expected");
                DestroyObject(obj);
            }
            while (_stackOld.TryPop(out obj))
            {
                Debug.Assert(obj != null, "null connection is not expected");
                DestroyObject(obj);
            }

            // Finally, reclaim everything that's emancipated (which, because
            // it's been doomed, will cause it to be disposed of as well)
            ReclaimEmancipatedObjects();
        }

        private Timer CreateCleanupTimer() =>
            ADP.UnsafeCreateTimer(
                new TimerCallback(CleanupCallback),
                null,
                _cleanupWait,
                _cleanupWait);

        private DbConnectionInternal CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
        {
            DbConnectionInternal newObj = null;

            try
            {
                newObj = _connectionFactory.CreatePooledConnection(this, owningObject, _connectionPoolGroup.ConnectionOptions, _connectionPoolGroup.PoolKey, userOptions);
                if (null == newObj)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.CreateObjectReturnedNull);    // CreateObject succeeded, but null object
                }
                if (!newObj.CanBePooled)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.NewObjectCannotBePooled);        // CreateObject succeeded, but non-poolable object
                }
                newObj.PrePush(null);

                lock (_objectList)
                {
                    if ((oldConnection != null) && (oldConnection.Pool == this))
                    {
                        _objectList.Remove(oldConnection);
                    }
                    _objectList.Add(newObj);
                    _totalObjects = _objectList.Count;
                }

                // If the old connection belonged to another pool, we need to remove it from that
                if (oldConnection != null)
                {
                    var oldConnectionPool = oldConnection.Pool;
                    if (oldConnectionPool != null && oldConnectionPool != this)
                    {
                        Debug.Assert(oldConnectionPool._state == State.ShuttingDown, "Old connections pool should be shutting down");
                        lock (oldConnectionPool._objectList)
                        {
                            oldConnectionPool._objectList.Remove(oldConnection);
                            oldConnectionPool._totalObjects = oldConnectionPool._objectList.Count;
                        }
                    }
                }

                // Reset the error wait:
                _errorWait = ERROR_WAIT_DEFAULT;
            }
            catch (Exception e)
            {
                if (!ADP.IsCatchableExceptionType(e))
                {
                    throw;
                }

                CheckPoolBlockingPeriod(e);

                newObj = null; // set to null, so we do not return bad new object
                // Failed to create instance
                _resError = e;

                // Make sure the timer starts even if ThreadAbort occurs after setting the ErrorEvent.

                // timer allocation has to be done out of CER block
                Timer t = new Timer(new TimerCallback(this.ErrorCallback), null, Timeout.Infinite, Timeout.Infinite);

                bool timerIsNotDisposed;
                try { }
                finally
                {
                    _waitHandles.ErrorEvent.Set();
                    _errorOccurred = true;

                    // Enable the timer.
                    // Note that the timer is created to allow periodic invocation. If ThreadAbort occurs in the middle of ErrorCallback,
                    // the timer will restart. Otherwise, the timer callback (ErrorCallback) destroys the timer after resetting the error to avoid second callback.
                    _errorTimer = t;
                    timerIsNotDisposed = t.Change(_errorWait, _errorWait);
                }

                Debug.Assert(timerIsNotDisposed, "ErrorCallback timer has been disposed");

                if (30000 < _errorWait)
                {
                    _errorWait = 60000;
                }
                else
                {
                    _errorWait *= 2;
                }
                throw;
            }
            return newObj;
        }

        //This method is implemented in DbConnectionPool.NetCoreApp 
        partial void CheckPoolBlockingPeriod(Exception e);

        private void DeactivateObject(DbConnectionInternal obj)
        {
            obj.DeactivateConnection();

            bool returnToGeneralPool = false;
            bool destroyObject = false;
            bool rootTxn = false;

            if (obj.IsConnectionDoomed)
            {
                // the object is not fit for reuse -- just dispose of it.
                destroyObject = true;
            }
            else
            {
                // NOTE: constructor should ensure that current state cannot be State.Initializing, so it can only
                //   be State.Running or State.ShuttingDown
                Debug.Assert(_state == State.Running || _state == State.ShuttingDown);

                lock (obj)
                {
                    // A connection with a delegated transaction cannot currently
                    // be returned to a different customer until the transaction
                    // actually completes, so we send it into Stasis -- the SysTx
                    // transaction object will ensure that it is owned (not lost),
                    // and it will be certain to put it back into the pool.                    

                    if (_state == State.ShuttingDown)
                    {
                        if (obj.IsTransactionRoot)
                        {
                            // SQLHotfix# 50003503 - connections that are affiliated with a 
                            //   root transaction and that also happen to be in a connection 
                            //   pool that is being shutdown need to be put in stasis so that 
                            //   the root transaction isn't effectively orphaned with no 
                            //   means to promote itself to a full delegated transaction or
                            //   Commit or Rollback
                            obj.SetInStasis();
                            rootTxn = true;
                        }
                        else
                        {
                            // connection is being closed and the pool has been marked as shutting
                            //   down, so destroy this object.
                            destroyObject = true;
                        }
                    }
                    else
                    {
                        if (obj.IsNonPoolableTransactionRoot)
                        {
                            obj.SetInStasis();
                            rootTxn = true;
                        }
                        else if (obj.CanBePooled)
                        {
                            // We must put this connection into the transacted pool
                            // while inside a lock to prevent a race condition with
                            // the transaction asynchronously completing on a second
                            // thread.

                            Transaction transaction = obj.EnlistedTransaction;
                            if (null != transaction)
                            {
                                // NOTE: we're not locking on _state, so it's possible that its
                                //   value could change between the conditional check and here.
                                //   Although perhaps not ideal, this is OK because the 
                                //   DelegatedTransactionEnded event will clean up the
                                //   connection appropriately regardless of the pool state.
                                Debug.Assert(_transactedConnectionPool != null, "Transacted connection pool was not expected to be null.");
                                _transactedConnectionPool.PutTransactedObject(transaction, obj);
                                rootTxn = true;
                            }
                            else
                            {
                                // return to general pool
                                returnToGeneralPool = true;
                            }
                        }
                        else
                        {
                            if (obj.IsTransactionRoot && !obj.IsConnectionDoomed)
                            {
                                // SQLHotfix# 50003503 - if the object cannot be pooled but is a transaction
                                //   root, then we must have hit one of two race conditions:
                                //       1) PruneConnectionPoolGroups shutdown the pool and marked this connection 
                                //          as non-poolable while we were processing within this lock
                                //       2) The LoadBalancingTimeout expired on this connection and marked this
                                //          connection as DoNotPool.
                                //
                                //   This connection needs to be put in stasis so that the root transaction isn't
                                //   effectively orphaned with no means to promote itself to a full delegated 
                                //   transaction or Commit or Rollback
                                obj.SetInStasis();
                                rootTxn = true;
                            }
                            else
                            {
                                // object is not fit for reuse -- just dispose of it
                                destroyObject = true;
                            }
                        }
                    }
                }
            }

            if (returnToGeneralPool)
            {
                // Only push the connection into the general pool if we didn't
                //   already push it onto the transacted pool, put it into stasis,
                //   or want to destroy it.
                Debug.Assert(destroyObject == false);
                PutNewObject(obj);
            }
            else if (destroyObject)
            {
                DestroyObject(obj);
                QueuePoolCreateRequest();
            }

            //-------------------------------------------------------------------------------------
            // postcondition

            // ensure that the connection was processed
            Debug.Assert(rootTxn == true || returnToGeneralPool == true || destroyObject == true);
        }

        internal void DestroyObject(DbConnectionInternal obj)
        {
            // A connection with a delegated transaction cannot be disposed of
            // until the delegated transaction has actually completed.  Instead,
            // we simply leave it alone; when the transaction completes, it will
            // come back through PutObjectFromTransactedPool, which will call us
            // again.
            if (!obj.IsTxRootWaitingForTxEnd)
            {
                bool removed = false;
                lock (_objectList)
                {
                    removed = _objectList.Remove(obj);
                    Debug.Assert(removed, "attempt to DestroyObject not in list");
                    _totalObjects = _objectList.Count;
                }

                obj.Dispose();
            }
        }

        private void ErrorCallback(object state)
        {
            _errorOccurred = false;
            _waitHandles.ErrorEvent.Reset();

            // the error state is cleaned, destroy the timer to avoid periodic invocation
            Timer t = _errorTimer;
            _errorTimer = null;
            if (t != null)
            {
                t.Dispose(); // Cancel timer request.
            }
        }


        private Exception TryCloneCachedException()
        // Cached exception can be of any type, so is not always cloneable.
        // This functions clones SqlException 
        // OleDb and Odbc connections are not passing throw this code
        {
            if (_resError == null)
                return null;

            var sqlError = _resError as SqlClient.SqlException;
            if (sqlError != null)
                return sqlError.InternalClone();

            return _resError;
        }

        private void WaitForPendingOpen()
        {
            PendingGetConnection next;

            do
            {
                bool started = false;

                try
                {
                    try { }
                    finally
                    {
                        started = Interlocked.CompareExchange(ref _pendingOpensWaiting, 1, 0) == 0;
                    }

                    if (!started)
                    {
                        return;
                    }

                    while (_pendingOpens.TryDequeue(out next))
                    {
                        if (next.Completion.Task.IsCompleted)
                        {
                            continue;
                        }

                        uint delay;
                        if (next.DueTime == Timeout.Infinite)
                        {
                            delay = unchecked((uint)Timeout.Infinite);
                        }
                        else
                        {
                            delay = (uint)Math.Max(ADP.TimerRemainingMilliseconds(next.DueTime), 0);
                        }

                        DbConnectionInternal connection = null;
                        bool timeout = false;
                        Exception caughtException = null;

                        try
                        {
                            bool allowCreate = true;
                            bool onlyOneCheckConnection = false;
                            ADP.SetCurrentTransaction(next.Completion.Task.AsyncState as Transactions.Transaction);
                            timeout = !TryGetConnection(next.Owner, delay, allowCreate, onlyOneCheckConnection, next.UserOptions, out connection);
                        }
                        catch (Exception e)
                        {
                            caughtException = e;
                        }

                        if (caughtException != null)
                        {
                            next.Completion.TrySetException(caughtException);
                        }
                        else if (timeout)
                        {
                            next.Completion.TrySetException(ADP.ExceptionWithStackTrace(ADP.PooledOpenTimeout()));
                        }
                        else
                        {
                            Debug.Assert(connection != null, "connection should never be null in success case");
                            if (!next.Completion.TrySetResult(connection))
                            {
                                // if the completion was cancelled, lets try and get this connection back for the next try
                                PutObject(connection, next.Owner);
                            }
                        }
                    }
                }
                finally
                {
                    if (started)
                    {
                        Interlocked.Exchange(ref _pendingOpensWaiting, 0);
                    }
                }
            } while (_pendingOpens.TryPeek(out next));
        }

        internal bool TryGetConnection(DbConnection owningObject, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions, out DbConnectionInternal connection)
        {
            uint waitForMultipleObjectsTimeout = 0;
            bool allowCreate = false;

            if (retry == null)
            {
                waitForMultipleObjectsTimeout = (uint)CreationTimeout;

                // Set the wait timeout to INFINITE (-1) if the SQL connection timeout is 0 (== infinite)
                if (waitForMultipleObjectsTimeout == 0)
                    waitForMultipleObjectsTimeout = unchecked((uint)Timeout.Infinite);

                allowCreate = true;
            }

            if (_state != State.Running)
            {
                connection = null;
                return true;
            }

            bool onlyOneCheckConnection = true;
            if (TryGetConnection(owningObject, waitForMultipleObjectsTimeout, allowCreate, onlyOneCheckConnection, userOptions, out connection))
            {
                return true;
            }
            else if (retry == null)
            {
                // timed out on a sync call
                return true;
            }

            var pendingGetConnection =
                new PendingGetConnection(
                    CreationTimeout == 0 ? Timeout.Infinite : ADP.TimerCurrent() + ADP.TimerFromSeconds(CreationTimeout / 1000),
                    owningObject,
                    retry,
                    userOptions);
            _pendingOpens.Enqueue(pendingGetConnection);

            // it is better to StartNew too many times than not enough
            if (_pendingOpensWaiting == 0)
            {
                Thread waitOpenThread = new Thread(WaitForPendingOpen);
                waitOpenThread.IsBackground = true;
                waitOpenThread.Start();
            }

            connection = null;
            return false;
        }

        private bool TryGetConnection(DbConnection owningObject, uint waitForMultipleObjectsTimeout, bool allowCreate, bool onlyOneCheckConnection, DbConnectionOptions userOptions, out DbConnectionInternal connection)
        {
            DbConnectionInternal obj = null;
            Transaction transaction = null;

            // If automatic transaction enlistment is required, then we try to
            // get the connection from the transacted connection pool first.
            if (HasTransactionAffinity)
            {
                obj = GetFromTransactedPool(out transaction);
            }

            if (null == obj)
            {
                Interlocked.Increment(ref _waitCount);

                do
                {
                    int waitResult = BOGUS_HANDLE;
                    try
                    {
                        try
                        {
                        }
                        finally
                        {
                            waitResult = WaitHandle.WaitAny(_waitHandles.GetHandles(allowCreate), unchecked((int)waitForMultipleObjectsTimeout));
                        }

                        // From the WaitAny docs: "If more than one object became signaled during
                        // the call, this is the array index of the signaled object with the
                        // smallest index value of all the signaled objects."  This is important
                        // so that the free object signal will be returned before a creation
                        // signal.

                        switch (waitResult)
                        {
                            case WaitHandle.WaitTimeout:
                                Interlocked.Decrement(ref _waitCount);
                                connection = null;
                                return false;

                            case ERROR_HANDLE:
                                // Throw the error that PoolCreateRequest stashed.
                                Interlocked.Decrement(ref _waitCount);
                                throw TryCloneCachedException();

                            case CREATION_HANDLE:
                                try
                                {
                                    obj = UserCreateRequest(owningObject, userOptions);
                                }
                                catch
                                {
                                    if (null == obj)
                                    {
                                        Interlocked.Decrement(ref _waitCount);
                                    }
                                    throw;
                                }
                                finally
                                {
                                    // Ensure that we release this waiter, regardless
                                    // of any exceptions that may be thrown.
                                    if (null != obj)
                                    {
                                        Interlocked.Decrement(ref _waitCount);
                                    }
                                }

                                if (null == obj)
                                {
                                    // If we were not able to create an object, check to see if
                                    // we reached MaxPoolSize.  If so, we will no longer wait on
                                    // the CreationHandle, but instead wait for a free object or
                                    // the timeout.
                                    if (Count >= MaxPoolSize && 0 != MaxPoolSize)
                                    {
                                        if (!ReclaimEmancipatedObjects())
                                        {
                                            // modify handle array not to wait on creation mutex anymore
                                            Debug.Assert(2 == CREATION_HANDLE, "creation handle changed value");
                                            allowCreate = false;
                                        }
                                    }
                                }
                                break;

                            case SEMAPHORE_HANDLE:
                                //
                                //    guaranteed available inventory
                                //
                                Interlocked.Decrement(ref _waitCount);
                                obj = GetFromGeneralPool();

                                if ((obj != null) && (!obj.IsConnectionAlive()))
                                {
                                    DestroyObject(obj);
                                    obj = null;     // Setting to null in case creating a new object fails

                                    if (onlyOneCheckConnection)
                                    {
                                        if (_waitHandles.CreationSemaphore.WaitOne(unchecked((int)waitForMultipleObjectsTimeout)))
                                        {
                                            try
                                            {
                                                obj = UserCreateRequest(owningObject, userOptions);
                                            }
                                            finally
                                            {
                                                _waitHandles.CreationSemaphore.Release(1);
                                            }
                                        }
                                        else
                                        {
                                            // Timeout waiting for creation semaphore - return null
                                            connection = null;
                                            return false;
                                        }
                                    }
                                }
                                break;
                            default:
                                Interlocked.Decrement(ref _waitCount);
                                throw ADP.InternalError(ADP.InternalErrorCode.UnexpectedWaitAnyResult);
                        }
                    }
                    finally
                    {
                        if (CREATION_HANDLE == waitResult)
                        {
                            _waitHandles.CreationSemaphore.Release(1);
                        }
                    }
                } while (null == obj);
            }

            if (null != obj)
            {
                PrepareConnection(owningObject, obj, transaction);
            }

            connection = obj;
            return true;
        }

        private void PrepareConnection(DbConnection owningObject, DbConnectionInternal obj, Transaction transaction)
        {
            lock (obj)
            {   // Protect against Clear and ReclaimEmancipatedObjects, which call IsEmancipated, which is affected by PrePush and PostPop
                obj.PostPop(owningObject);
            }
            try
            {
                obj.ActivateConnection(transaction);
            }
            catch
            {
                // if Activate throws an exception
                // put it back in the pool or have it properly disposed of
                this.PutObject(obj, owningObject);
                throw;
            }
        }

        /// <summary>
        /// Creates a new connection to replace an existing connection
        /// </summary>
        /// <param name="owningObject">Outer connection that currently owns <paramref name="oldConnection"/></param>
        /// <param name="userOptions">Options used to create the new connection</param>
        /// <param name="oldConnection">Inner connection that will be replaced</param>
        /// <returns>A new inner connection that is attached to the <paramref name="owningObject"/></returns>
        internal DbConnectionInternal ReplaceConnection(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
        {
            DbConnectionInternal newConnection = UserCreateRequest(owningObject, userOptions, oldConnection);

            if (newConnection != null)
            {
                PrepareConnection(owningObject, newConnection, oldConnection.EnlistedTransaction);
                oldConnection.PrepareForReplaceConnection();
                oldConnection.DeactivateConnection();
                oldConnection.Dispose();
            }

            return newConnection;
        }

        private DbConnectionInternal GetFromGeneralPool()
        {
            DbConnectionInternal obj = null;

            if (!_stackNew.TryPop(out obj))
            {
                if (!_stackOld.TryPop(out obj))
                {
                    obj = null;
                }
                else
                {
                    Debug.Assert(obj != null, "null connection is not expected");
                }
            }
            else
            {
                Debug.Assert(obj != null, "null connection is not expected");
            }

            // When another thread is clearing this pool,  
            // it will remove all connections in this pool which causes the 
            // following assert to fire, which really mucks up stress against
            //  checked bits.

            if (null != obj)
            {
            }
            return (obj);
        }

        private DbConnectionInternal GetFromTransactedPool(out Transaction transaction)
        {
            transaction = ADP.GetCurrentTransaction();
            DbConnectionInternal obj = null;

            if (null != transaction && null != _transactedConnectionPool)
            {
                obj = _transactedConnectionPool.GetTransactedObject(transaction);

                if (null != obj)
                {
                    if (obj.IsTransactionRoot)
                    {
                        try
                        {
                            obj.IsConnectionAlive(true);
                        }
                        catch
                        {
                            DestroyObject(obj);
                            throw;
                        }
                    }
                    else if (!obj.IsConnectionAlive())
                    {
                        DestroyObject(obj);
                        obj = null;
                    }
                }
            }
            return obj;
        }

        private void PoolCreateRequest(object state)
        {
            // called by pooler to ensure pool requests are currently being satisfied -
            // creation mutex has not been obtained

            if (State.Running == _state)
            {
                // in case WaitForPendingOpen ever failed with no subsequent OpenAsync calls,
                // start it back up again
                if (!_pendingOpens.IsEmpty && _pendingOpensWaiting == 0)
                {
                    Thread waitOpenThread = new Thread(WaitForPendingOpen);
                    waitOpenThread.IsBackground = true;
                    waitOpenThread.Start();
                }

                // Before creating any new objects, reclaim any released objects that were
                // not closed.
                ReclaimEmancipatedObjects();

                if (!ErrorOccurred)
                {
                    if (NeedToReplenish)
                    {
                        // Check to see if pool was created using integrated security and if so, make
                        // sure the identity of current user matches that of user that created pool.
                        // If it doesn't match, do not create any objects on the ThreadPool thread,
                        // since either Open will fail or we will open a object for this pool that does
                        // not belong in this pool.  The side effect of this is that if using integrated
                        // security min pool size cannot be guaranteed.
                        if (UsingIntegrateSecurity && !_identity.Equals(DbConnectionPoolIdentity.GetCurrent()))
                        {
                            return;
                        }
                        int waitResult = BOGUS_HANDLE;
                        try
                        {
                            try { }
                            finally
                            {
                                waitResult = WaitHandle.WaitAny(_waitHandles.GetHandles(withCreate: true), CreationTimeout);
                            }
                            if (CREATION_HANDLE == waitResult)
                            {
                                DbConnectionInternal newObj;

                                // Check ErrorOccurred again after obtaining mutex
                                if (!ErrorOccurred)
                                {
                                    while (NeedToReplenish)
                                    {
                                        try
                                        {
                                            // Don't specify any user options because there is no outer connection associated with the new connection
                                            newObj = CreateObject(owningObject: null, userOptions: null, oldConnection: null);
                                        }
                                        catch
                                        {
                                            // Catch all the exceptions occurring during CreateObject so that they 
                                            // don't emerge as unhandled on the thread pool and don't crash applications
                                            // The error is handled in CreateObject and surfaced to the caller of the Connection Pool
                                            // using the ErrorEvent. Hence it is OK to swallow all exceptions here.
                                            break;
                                        }
                                        // We do not need to check error flag here, since we know if
                                        // CreateObject returned null, we are in error case.
                                        if (null != newObj)
                                        {
                                            PutNewObject(newObj);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (WaitHandle.WaitTimeout == waitResult)
                            {
                                // do not wait forever and potential block this worker thread
                                // instead wait for a period of time and just requeue to try again
                                QueuePoolCreateRequest();
                            }
                        }
                        finally
                        {
                            if (CREATION_HANDLE == waitResult)
                            {
                                // reuse waitResult and ignore its value
                                _waitHandles.CreationSemaphore.Release(1);
                            }
                        }
                    }
                }
            }
        }

        internal void PutNewObject(DbConnectionInternal obj)
        {
            Debug.Assert(null != obj, "why are we adding a null object to the pool?");
            // Debug.Assert(obj.CanBePooled,    "non-poolable object in pool");


            _stackNew.Push(obj);
            _waitHandles.PoolSemaphore.Release(1);
        }

        internal void PutObject(DbConnectionInternal obj, object owningObject)
        {
            Debug.Assert(null != obj, "null obj?");


            // Once a connection is closing (which is the state that we're in at
            // this point in time) you cannot delegate a transaction to or enlist
            // a transaction in it, so we can correctly presume that if there was
            // not a delegated or enlisted transaction to start with, that there
            // will not be a delegated or enlisted transaction once we leave the
            // lock.

            lock (obj)
            {
                // Calling PrePush prevents the object from being reclaimed
                // once we leave the lock, because it sets _pooledCount such
                // that it won't appear to be out of the pool.  What that
                // means, is that we're now responsible for this connection:
                // it won't get reclaimed if we drop the ball somewhere.
                obj.PrePush(owningObject);
            }

            DeactivateObject(obj);
        }

        internal void PutObjectFromTransactedPool(DbConnectionInternal obj)
        {
            Debug.Assert(null != obj, "null pooledObject?");
            Debug.Assert(obj.EnlistedTransaction == null, "pooledObject is still enlisted?");

            // called by the transacted connection pool , once it's removed the
            // connection from it's list.  We put the connection back in general
            // circulation.

            // NOTE: there is no locking required here because if we're in this
            // method, we can safely presume that the caller is the only person
            // that is using the connection, and that all pre-push logic has been
            // done and all transactions are ended.

            if (_state == State.Running && obj.CanBePooled)
            {
                PutNewObject(obj);
            }
            else
            {
                DestroyObject(obj);
                QueuePoolCreateRequest();
            }
        }

        private void QueuePoolCreateRequest()
        {
            if (State.Running == _state)
            {
                // Make sure we're at quota by posting a callback to the threadpool.
                ThreadPool.QueueUserWorkItem(_poolCreateRequest);
            }
        }

        private bool ReclaimEmancipatedObjects()
        {
            bool emancipatedObjectFound = false;

            List<DbConnectionInternal> reclaimedObjects = new List<DbConnectionInternal>();
            int count;

            lock (_objectList)
            {
                count = _objectList.Count;

                for (int i = 0; i < count; ++i)
                {
                    DbConnectionInternal obj = _objectList[i];

                    if (null != obj)
                    {
                        bool locked = false;

                        try
                        {
                            Monitor.TryEnter(obj, ref locked);

                            if (locked)
                            { // avoid race condition with PrePush/PostPop and IsEmancipated
                                if (obj.IsEmancipated)
                                {
                                    // Inside the lock, we want to do as little
                                    // as possible, so we simply mark the object
                                    // as being in the pool, but hand it off to
                                    // an out of pool list to be deactivated,
                                    // etc.
                                    obj.PrePush(null);
                                    reclaimedObjects.Add(obj);
                                }
                            }
                        }
                        finally
                        {
                            if (locked)
                                Monitor.Exit(obj);
                        }
                    }
                }
            }

            // NOTE: we don't want to call DeactivateObject while we're locked,
            // because it can make roundtrips to the server and this will block
            // object creation in the pooler.  Instead, we queue things we need
            // to do up, and process them outside the lock.
            count = reclaimedObjects.Count;

            for (int i = 0; i < count; ++i)
            {
                DbConnectionInternal obj = reclaimedObjects[i];

                emancipatedObjectFound = true;

                obj.DetachCurrentTransactionIfEnded();
                DeactivateObject(obj);
            }
            return emancipatedObjectFound;
        }

        internal void Startup()
        {
            _cleanupTimer = CreateCleanupTimer();
            if (NeedToReplenish)
            {
                QueuePoolCreateRequest();
            }
        }

        internal void Shutdown()
        {
            _state = State.ShuttingDown;

            // deactivate timer callbacks
            Timer t = _cleanupTimer;
            _cleanupTimer = null;
            if (null != t)
            {
                t.Dispose();
            }
        }

        // TransactionEnded merely provides the plumbing for DbConnectionInternal to access the transacted pool
        //   that is implemented inside DbConnectionPool. This method's counterpart (PutTransactedObject) should
        //   only be called from DbConnectionPool.DeactivateObject and thus the plumbing to provide access to 
        //   other objects is unnecessary (hence the asymmetry of Ended but no Begin)
        internal void TransactionEnded(Transaction transaction, DbConnectionInternal transactedObject)
        {
            Debug.Assert(null != transaction, "null transaction?");
            Debug.Assert(null != transactedObject, "null transactedObject?");
            // Note: connection may still be associated with transaction due to Explicit Unbinding requirement.

            // called by the internal connection when it get's told that the
            // transaction is completed.  We tell the transacted pool to remove
            // the connection from it's list, then we put the connection back in
            // general circulation.

            TransactedConnectionPool transactedConnectionPool = _transactedConnectionPool;
            if (null != transactedConnectionPool)
            {
                transactedConnectionPool.TransactionEnded(transaction, transactedObject);
            }
        }

        private DbConnectionInternal UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection = null)
        {
            // called by user when they were not able to obtain a free object but
            // instead obtained creation mutex

            DbConnectionInternal obj = null;
            if (ErrorOccurred)
            {
                throw TryCloneCachedException();
            }
            else
            {
                if ((oldConnection != null) || (Count < MaxPoolSize) || (0 == MaxPoolSize))
                {
                    // If we have an odd number of total objects, reclaim any dead objects.
                    // If we did not find any objects to reclaim, create a new one.
                    if ((oldConnection != null) || (Count & 0x1) == 0x1 || !ReclaimEmancipatedObjects())
                        obj = CreateObject(owningObject, userOptions, oldConnection);
                }
                return obj;
            }
        }
    }
}
