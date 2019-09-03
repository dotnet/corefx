// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Transactions.Configuration;

namespace System.Transactions
{
    /// <summary> Summary description for TransactionException. </summary>
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class TransactionException : SystemException
    {
        internal static bool IncludeDistributedTxId(Guid distributedTxId)
        {
            return (distributedTxId != Guid.Empty && AppSettings.IncludeDistributedTxIdInExceptionMessage);
        }

        internal static TransactionException Create(string message, Exception innerException)
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(TransactionExceptionType.TransactionException, message, innerException==null?string.Empty:innerException.ToString());
            }

            return new TransactionException(message, innerException);
        }

        internal static TransactionException Create(TraceSourceType traceSource, string message, Exception innerException)
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(TransactionExceptionType.TransactionException, message, innerException==null?string.Empty:innerException.ToString());
            }

            return new TransactionException(message, innerException);
        }
        internal static TransactionException CreateTransactionStateException(Exception innerException)
        {
            return Create(SR.TransactionStateException, innerException);
        }

        internal static Exception CreateEnlistmentStateException(Exception innerException, Guid distributedTxId)
        {
            string messagewithTxId = SR.EnlistmentStateException;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(TransactionExceptionType.InvalidOperationException, messagewithTxId, innerException==null?string.Empty:innerException.ToString());
            }
            return new InvalidOperationException(messagewithTxId, innerException);
        }

        internal static Exception CreateInvalidOperationException(TraceSourceType traceSource, string message, Exception innerException)
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(traceSource, TransactionExceptionType.InvalidOperationException, message, innerException==null?string.Empty:innerException.ToString());
            }

            return new InvalidOperationException(message, innerException);
        }

        public TransactionException()
        {
        }

        public TransactionException(string message) : base(message)
        {
        }

        public TransactionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal static TransactionException Create(string message, Guid distributedTxId)
        {
            if (IncludeDistributedTxId(distributedTxId))
            {
                return new TransactionException(SR.Format(SR.DistributedTxIDInTransactionException, message, distributedTxId));
            }
            return new TransactionException(message);
        }

        internal static TransactionException Create(string message, Exception innerException, Guid distributedTxId)
        {
            string messagewithTxId = message;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            return Create(messagewithTxId, innerException);
        }

        internal static TransactionException Create(TraceSourceType traceSource, string message, Exception innerException, Guid distributedTxId)
        {
            string messagewithTxId = message;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            return Create(traceSource, messagewithTxId, innerException);
        }

        internal static TransactionException Create(TraceSourceType traceSource, string message, Guid distributedTxId)
        {
            if (IncludeDistributedTxId(distributedTxId))
            {
                return new TransactionException(SR.Format(SR.DistributedTxIDInTransactionException, message, distributedTxId));
            }
            return new TransactionException(message);
        }

        internal static TransactionException CreateTransactionStateException(Exception innerException, Guid distributedTxId)
        {
            return Create(SR.TransactionStateException, innerException, distributedTxId);
        }

        internal static Exception CreateTransactionCompletedException(Guid distributedTxId)
        {
            string messagewithTxId = SR.TransactionAlreadyCompleted;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(TransactionExceptionType.InvalidOperationException, messagewithTxId, string.Empty);
            }

            return new InvalidOperationException(messagewithTxId);
        }

        internal static Exception CreateInvalidOperationException(TraceSourceType traceSource, string message, Exception innerException, Guid distributedTxId)
        {
            string messagewithTxId = message;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            return CreateInvalidOperationException(traceSource, messagewithTxId, innerException);
        }
    }


    /// <summary> Summary description for TransactionAbortedException. </summary>
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class TransactionAbortedException : TransactionException
    {
        internal static new TransactionAbortedException Create(string message, Exception innerException, Guid distributedTxId)
        {
            string messagewithTxId = message;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            return TransactionAbortedException.Create(messagewithTxId, innerException);
        }

        internal static new TransactionAbortedException Create(string message, Exception innerException)
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(TransactionExceptionType.TransactionAbortedException, message, innerException==null?string.Empty:innerException.ToString());
            }

            return new TransactionAbortedException(message, innerException);
        }
        public TransactionAbortedException() : base(SR.TransactionAborted)
        {
        }

        public TransactionAbortedException(string message) : base(message)
        {
        }

        public TransactionAbortedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal TransactionAbortedException(Exception innerException) : base(SR.TransactionAborted, innerException)
        {
        }

        internal TransactionAbortedException(Exception innerException, Guid distributedTxId) :
            base(IncludeDistributedTxId(distributedTxId) ?
                SR.Format(SR.DistributedTxIDInTransactionException, SR.TransactionAborted, distributedTxId)
                : SR.TransactionAborted, innerException)
        {
        }

        protected TransactionAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary> Summary description for TransactionInDoubtException. </summary>
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class TransactionInDoubtException : TransactionException
    {
        internal static new TransactionInDoubtException Create(TraceSourceType traceSource, string message, Exception innerException, Guid distributedTxId)
        {
            string messagewithTxId = message;
            if (IncludeDistributedTxId(distributedTxId))
                messagewithTxId = SR.Format(SR.DistributedTxIDInTransactionException, messagewithTxId, distributedTxId);

            return TransactionInDoubtException.Create(traceSource, messagewithTxId, innerException);
        }

        internal static new TransactionInDoubtException Create(TraceSourceType traceSource, string message, Exception innerException)
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(traceSource, TransactionExceptionType.TransactionInDoubtException, message, innerException==null?string.Empty:innerException.ToString());
            }

            return new TransactionInDoubtException(message, innerException);
        }

        public TransactionInDoubtException() : base(SR.TransactionIndoubt)
        {
        }

        public TransactionInDoubtException(string message) : base(message)
        {
        }

        public TransactionInDoubtException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransactionInDoubtException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary> Summary description for TransactionManagerCommunicationException. </summary>
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class TransactionManagerCommunicationException : TransactionException
    {
        internal static new TransactionManagerCommunicationException Create(string message, Exception innerException)
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.TransactionExceptionTrace(TransactionExceptionType.TransactionManagerCommunicationException, message, innerException==null?string.Empty:innerException.ToString());
            }

            return new TransactionManagerCommunicationException(message, innerException);
        }

        internal static TransactionManagerCommunicationException Create(Exception innerException)
        {
            return Create(SR.TransactionManagerCommunicationException, innerException);
        }

        public TransactionManagerCommunicationException() : base(SR.TransactionManagerCommunicationException)
        {
        }

        public TransactionManagerCommunicationException(string message) : base(message)
        {
        }

        public TransactionManagerCommunicationException(
            string message,
            Exception innerException
            ) : base(message, innerException)
        {
        }

        protected TransactionManagerCommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class TransactionPromotionException : TransactionException
    {
        public TransactionPromotionException() : this(SR.PromotionFailed)
        {
        }

        public TransactionPromotionException(string message) : base(message)
        {
        }

        public TransactionPromotionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransactionPromotionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
