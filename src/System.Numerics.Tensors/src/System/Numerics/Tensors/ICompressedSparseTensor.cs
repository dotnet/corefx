// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    public interface ICompressedSparseTensor<T> : ITensor<T>
    {
        int Capacity { get; }
        Memory<int> CompressedCounts { get; }
        Memory<int> Indices { get; }
        int NonZeroCount { get; }
        Memory<T> Values { get; }
    }
}
