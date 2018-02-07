// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an interface that may be implemented by an awaiter.</summary>
    public interface IAwaiter : ICriticalNotifyCompletion
    {
        /// <summary>Gets whether the awaiter is completed.</summary>
        bool IsCompleted { get; }
        /// <summary>Gets the result of the completed, awaited operation.</summary>
        void GetResult();
    }

    /// <summary>Provides an interface that may be implemented by an awaiter.</summary>
    /// <typeparam name="T">Specifies the result type of the await operation using this awaiter.</typeparam>
    public interface IAwaiter<T> : ICriticalNotifyCompletion
    {
        /// <summary>Gets whether the awaiter is completed.</summary>
        bool IsCompleted { get; }
        /// <summary>Gets the result of the completed, awaited operation.</summary>
        T GetResult();
    }
}
