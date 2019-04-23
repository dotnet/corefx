// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System;
using System.Diagnostics;
using System.Threading;

namespace System.Data.ProviderBase
{
    /// <summary>
    /// Represents the context of an authentication attempt when using the new active directory based authentication mechanisms.
    /// All data members, except_isUpdateInProgressCounter, should be immutable.
    /// </summary>
    sealed internal class DbConnectionPoolAuthenticationContext
    {
        /// <summary>
        /// The value expected in _isUpdateInProgress if a thread has taken a lock on this context,
        /// to perform the update on the context.
        /// </summary>
        private const int STATUS_LOCKED = 1;

        /// <summary>
        /// The value expected in _isUpdateInProgress if no thread has taken a lock on this context.
        /// </summary>
        private const int STATUS_UNLOCKED = 0;

        /// <summary>
        /// Access Token, which is obtained from Active Directory Authentication Library for SQL Server, and needs to be sent to SQL Server
        /// as part of TDS Token type Federated Authentication Token.
        /// </summary>
        private readonly byte[] _accessToken;

        /// <summary>
        /// Expiration time of the above access token.
        /// </summary>
        private readonly DateTime _expirationTime;

        /// <summary>
        /// A member which is used to achieve a lock to control refresh attempt on this context.
        /// </summary>
        private int _isUpdateInProgress;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="accessToken">Access Token that will be used to connect to SQL Server. Carries identity information about a user.</param>
        /// <param name="expirationTime">The expiration time in UTC for the above accessToken.</param>
        internal DbConnectionPoolAuthenticationContext(byte[] accessToken, DateTime expirationTime)
        {

            Debug.Assert(accessToken != null && accessToken.Length > 0);
            Debug.Assert(expirationTime > DateTime.MinValue && expirationTime < DateTime.MaxValue);

            _accessToken = accessToken;
            _expirationTime = expirationTime;
            _isUpdateInProgress = STATUS_UNLOCKED;
        }

        /// <summary>
        /// Static Method.
        /// Given two contexts, choose one to update in the cache. Chooses based on expiration time.
        /// </summary>
        /// <param name="context1">Context1</param>
        /// <param name="context2">Context2</param>
        internal static DbConnectionPoolAuthenticationContext ChooseAuthenticationContextToUpdate(DbConnectionPoolAuthenticationContext context1, DbConnectionPoolAuthenticationContext context2)
        {

            Debug.Assert(context1 != null, "context1 should not be null.");
            Debug.Assert(context2 != null, "context2 should not be null.");

            return context1.ExpirationTime > context2.ExpirationTime ? context1 : context2;
        }

        internal byte[] AccessToken
        {
            get
            {
                return _accessToken;
            }
        }

        internal DateTime ExpirationTime
        {
            get
            {
                return _expirationTime;
            }
        }

        /// <summary>
        /// Try locking the variable _isUpdateInProgressCounter and return if this thread got the lock to update.
        /// Whichever thread got the chance to update this variable to 1 wins the lock.
        /// </summary>
        internal bool LockToUpdate()
        {
            int oldValue = Interlocked.CompareExchange(ref _isUpdateInProgress, STATUS_LOCKED, STATUS_UNLOCKED);
            return (oldValue == STATUS_UNLOCKED);
        }

        /// <summary>
        /// Release the lock which was obtained through LockToUpdate.
        /// </summary>
        internal void ReleaseLockToUpdate()
        {
            int oldValue = Interlocked.CompareExchange(ref _isUpdateInProgress, STATUS_UNLOCKED, STATUS_LOCKED);
            Debug.Assert(oldValue == STATUS_LOCKED);
        }
    }
}
