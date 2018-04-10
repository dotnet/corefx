// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Transactions
{
    // The volatile Demultiplexer is a fanout point for promoted volatile enlistments.
    // When a transaction is promoted a single volatile enlistment is created in the new
    // transaction for all volitile enlistments on the transaction.  When the VolatileDemux
    // receives a preprepare it will fan that notification out to all of the enlistments
    // on the transaction.  When it has gathered all of the responses it will send a 
    // single vote back to the DistributedTransactionManager.
    internal abstract class VolatileDemultiplexer
    {
        // Reference the transactions so that we have access to it's enlistments
        protected InternalTransaction _transaction;
        
        // Store the IVolatileEnlistment interface to call back to the Distributed TM
        internal IPromotedEnlistment _promotedEnlistment;

        public VolatileDemultiplexer(InternalTransaction transaction)
        {
            _transaction = transaction;
        }

        internal void BroadcastCommitted(ref VolatileEnlistmentSet volatiles)
        {
            // Broadcast preprepare to the volatile subordinates
            for (int i = 0; i < volatiles._volatileEnlistmentCount; i++)
            {
                volatiles._volatileEnlistments[i]._twoPhaseState.InternalCommitted(
                    volatiles._volatileEnlistments[i]);
            }
        }

        // This broadcast is used by the state machines and therefore must be internal.
        internal void BroadcastRollback(ref VolatileEnlistmentSet volatiles)
        {
            // Broadcast preprepare to the volatile subordinates
            for (int i = 0; i < volatiles._volatileEnlistmentCount; i++)
            {
                volatiles._volatileEnlistments[i]._twoPhaseState.InternalAborted(
                    volatiles._volatileEnlistments[i]);
            }
        }

        internal void BroadcastInDoubt(ref VolatileEnlistmentSet volatiles)
        {
            // Broadcast preprepare to the volatile subordinates
            for (int i = 0; i < volatiles._volatileEnlistmentCount; i++)
            {
                volatiles._volatileEnlistments[i]._twoPhaseState.InternalIndoubt(
                    volatiles._volatileEnlistments[i]);
            }
        }
    }


    // This class implements the phase 0 version of a volatile demux.
    internal class Phase0VolatileDemultiplexer : VolatileDemultiplexer
    {
        public Phase0VolatileDemultiplexer(InternalTransaction transaction) : base(transaction) { }
    }

    // This class implements the phase 1 version of a volatile demux.
    internal class Phase1VolatileDemultiplexer : VolatileDemultiplexer
    {
        public Phase1VolatileDemultiplexer(InternalTransaction transaction) : base(transaction) { }
    }

    internal struct VolatileEnlistmentSet
    {
        internal InternalEnlistment[] _volatileEnlistments;
        internal int _volatileEnlistmentCount;
        internal int _volatileEnlistmentSize;
        internal int _dependentClones;

        // Track the number of volatile enlistments that have prepared.
        internal int _preparedVolatileEnlistments;

        // This is a single pinpoint enlistment to represent all volatile enlistments that
        // may exist on a promoted transaction.  This member should only be initialized if
        // a transaction is promoted.
        private VolatileDemultiplexer _volatileDemux;
        internal VolatileDemultiplexer VolatileDemux
        {
            get { return _volatileDemux; }
            set
            {
                Debug.Assert(_volatileDemux == null, "volatileDemux can only be set once.");
                _volatileDemux = value;
            }
        }
    }
}
