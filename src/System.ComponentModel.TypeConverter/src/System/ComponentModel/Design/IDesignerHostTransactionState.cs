// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    /// Methods for the Designer host to report on the state of transactions.
    /// </summary>
    public interface IDesignerHostTransactionState
    {
        bool IsClosingTransaction { get; }
    }
}
