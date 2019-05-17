// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Transactions.Distributed;

namespace System.Transactions
{
    [ComImport]
    [Guid("0fb15084-af41-11ce-bd2b-204c4f4f5020")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDtcTransaction
    {
        void Commit(int retaining, [MarshalAs(UnmanagedType.I4)] int commitType, int reserved);

        void Abort(IntPtr reason, int retaining, int async);

        void GetTransactionInfo(IntPtr transactionInformation);
    }

    public static class TransactionInterop
    {
        internal static DistributedTransaction ConvertToDistributedTransaction(Transaction transaction)
        {
            if (null == transaction)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (transaction.Disposed)
            {
                throw new ObjectDisposedException(nameof(Transaction));
            }

            if (transaction._complete)
            {
                throw TransactionException.CreateTransactionCompletedException(transaction.DistributedTxId);
            }

            DistributedTransaction distributedTx = transaction.Promote();
            if (distributedTx == null)
            {
                throw DistributedTransaction.NotSupported();
            }
            return distributedTx;
        }

        /// <summary>
        /// This is the PromoterType value that indicates that the transaction is promoting to MSDTC.
        /// 
        /// If using the variation of Transaction.EnlistPromotableSinglePhase that takes a PromoterType and the
        /// ITransactionPromoter being used promotes to MSDTC, then this is the value that should be 
        /// specified for the PromoterType parameter to EnlistPromotableSinglePhase.
        /// 
        /// If using the variation of Transaction.EnlistPromotableSinglePhase that assumes promotion to MSDTC and
        /// it that returns false, the caller can compare this value with Transaction.PromoterType to
        /// verify that the transaction promoted, or will promote, to MSDTC. If the Transaction.PromoterType
        /// matches this value, then the caller can continue with its enlistment with MSDTC. But if it
        /// does not match, the caller will not be able to enlist with MSDTC.
        /// </summary>
        public static readonly Guid PromoterTypeDtc = new Guid("14229753-FFE1-428D-82B7-DF73045CB8DA");

        public static byte[] GetExportCookie(Transaction transaction, byte[] whereabouts)
        {
            if (null == transaction)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (null == whereabouts)
            {
                throw new ArgumentNullException(nameof(whereabouts));
            }

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetExportCookie");
            }

            // Copy the whereabouts so that it cannot be modified later.
            var whereaboutsCopy = new byte[whereabouts.Length];
            Buffer.BlockCopy(whereabouts, 0, whereaboutsCopy, 0, whereabouts.Length);

            DistributedTransaction dTx = ConvertToDistributedTransaction(transaction);
            byte[] cookie = dTx.GetExportCookie(whereaboutsCopy);

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetExportCookie");
            }

            return cookie;
        }

        public static Transaction GetTransactionFromExportCookie(byte[] cookie)
        {
            if (null == cookie)
            {
                throw new ArgumentNullException(nameof(cookie));
            }

            if (cookie.Length < 32)
            {
                throw new ArgumentException(SR.InvalidArgument, nameof(cookie));
            }

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromExportCookie");
            }

            var cookieCopy = new byte[cookie.Length];
            Buffer.BlockCopy(cookie, 0, cookieCopy, 0, cookie.Length);
            cookie = cookieCopy;

            // Extract the transaction guid from the propagation token to see if we already have a
            // transaction object for the transaction.
            byte[] guidByteArray = new byte[16];
            for (int i = 0; i < guidByteArray.Length; i++)
            {
                // In a cookie, the transaction guid is preceeded by a signature guid.
                guidByteArray[i] = cookie[i + 16];
            }
            Guid txId = new Guid(guidByteArray);

            // First check to see if there is a promoted LTM transaction with the same ID.  If there
            // is, just return that.
            Transaction transaction = TransactionManager.FindPromotedTransaction(txId);
            if (transaction != null)
            {
                if (etwLog.IsEnabled())
                {
                    etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromExportCookie");
                }

                return transaction;
            }

            // Find or create the promoted transaction.
            DistributedTransaction dTx = DistributedTransactionManager.GetTransactionFromExportCookie(cookieCopy, txId);
            transaction = TransactionManager.FindOrCreatePromotedTransaction(txId, dTx);

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromExportCookie");
            }

            return transaction;
        }

        public static byte[] GetTransmitterPropagationToken(Transaction transaction)
        {
            if (null == transaction)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransmitterPropagationToken");
            }

            DistributedTransaction dTx = ConvertToDistributedTransaction(transaction);
            byte[] token = dTx.GetTransmitterPropagationToken();

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransmitterPropagationToken");
            }

            return token;
        }

        public static Transaction GetTransactionFromTransmitterPropagationToken(byte[] propagationToken)
        {
            if (null == propagationToken)
            {
                throw new ArgumentNullException(nameof(propagationToken));
            }

            if (propagationToken.Length < 24)
            {
                throw new ArgumentException(SR.InvalidArgument, nameof(propagationToken));
            }

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromTransmitterPropagationToken");
            }

            // Extract the transaction guid from the propagation token to see if we already have a
            // transaction object for the transaction.
            byte[] guidByteArray = new byte[16];
            for (int i = 0; i < guidByteArray.Length; i++)
            {
                // In a propagation token, the transaction guid is preceeded by two version DWORDs.
                guidByteArray[i] = propagationToken[i + 8];
            }

            var txId = new Guid(guidByteArray);

            // First check to see if there is a promoted LTM transaction with the same ID.  If there is, just return that.
            Transaction tx = TransactionManager.FindPromotedTransaction(txId);
            if (null != tx)
            {
                if (etwLog.IsEnabled())
                {
                    etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromTransmitterPropagationToken");
                }

                return tx;
            }

            DistributedTransaction dTx = GetDistributedTransactionFromTransmitterPropagationToken(propagationToken);
            
            // If a transaction is found then FindOrCreate will Dispose the distributed transaction created.
            Transaction returnValue = TransactionManager.FindOrCreatePromotedTransaction(txId, dTx);

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromTransmitterPropagationToken");
            }
            return returnValue;
        }

        public static IDtcTransaction GetDtcTransaction(Transaction transaction)
        {
            if (null == transaction)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetDtcTransaction");
            }

            DistributedTransaction dTx = ConvertToDistributedTransaction(transaction);
            IDtcTransaction transactionNative = dTx.GetDtcTransaction();

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetDtcTransaction");
            }

            return transactionNative;
        }

        public static Transaction GetTransactionFromDtcTransaction(IDtcTransaction transactionNative)
        {
            if (null == transactionNative)
            {
                throw new ArgumentNullException(nameof(transactionNative));
            }

            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromDtcTransaction");
            }

            Transaction transaction = DistributedTransactionManager.GetTransactionFromDtcTransaction(transactionNative);

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetTransactionFromDtcTransaction");
            }
            return transaction;
        }

        public static byte[] GetWhereabouts()
        {
            TransactionsEtwProvider etwLog = TransactionsEtwProvider.Log;
            if (etwLog.IsEnabled())
            {
                etwLog.MethodEnter(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetWhereabouts");
            }

            DistributedTransactionManager dTm = TransactionManager.DistributedTransactionManager;
            byte[] returnValue = dTm.GetWhereabouts();

            if (etwLog.IsEnabled())
            {
                etwLog.MethodExit(TraceSourceType.TraceSourceDistributed, "TransactionInterop.GetWhereabouts");
            }
            return returnValue;
        }

        internal static DistributedTransaction GetDistributedTransactionFromTransmitterPropagationToken(byte[] propagationToken)
        {
            if (null == propagationToken)
            {
                throw new ArgumentNullException(nameof(propagationToken));
            }

            if (propagationToken.Length < 24)
            {
                throw new ArgumentException(SR.InvalidArgument, nameof(propagationToken));
            }

            byte[] propagationTokenCopy = new byte[propagationToken.Length];
            Array.Copy(propagationToken, 0, propagationTokenCopy, 0, propagationToken.Length);

            return DistributedTransactionManager.GetDistributedTransactionFromTransmitterPropagationToken(propagationTokenCopy);
        }
    }
}
