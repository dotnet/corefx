// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Transactions
{
    [Serializable]
    public sealed class SubordinateTransaction : Transaction
    {
        // Create a transaction with the given settings
        //
        public SubordinateTransaction(IsolationLevel isoLevel, ISimpleTransactionSuperior superior) : base(isoLevel, superior)
        {
        }
    }
}
