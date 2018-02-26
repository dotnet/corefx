// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers
{
    /// <summary>
    /// Represents a linked list of <see cref="Memory{T}"/> nodes.
    /// </summary>
    public interface IMemoryList<T>
    {
        /// <summary>
        /// The <see cref="Memory{T}"/> value for current node.
        /// </summary>
        Memory<T> Memory { get; }

        /// <summary>
        /// The next node.
        /// </summary>
        IMemoryList<T> Next { get; }

        /// <summary>
        /// Returns <see cref="IMemoryList{T}"/> that contains <typeparamref name="T"/> item offset from beginning of current <see cref="IMemoryList{T}"/> by <paramref name="offset"/>.
        /// <paramref name="localIndex"/> would contain index inside returned <see cref="IMemoryList{T}"/>
        /// </summary>
        IMemoryList<T> GetNext(long offset, out int localIndex);

        /// <summary>
        /// Returns count of items between start of this <see cref="IMemoryList{T}"/> to <paramref name="memoryList"/>.
        /// </summary>
        long GetLength(IMemoryList<T> memoryList);
    }
}
