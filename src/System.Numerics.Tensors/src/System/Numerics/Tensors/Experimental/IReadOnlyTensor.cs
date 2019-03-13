// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    public interface IReadOnlyTensor<T>
    {
        long Rank { get; }

        T this[params long[] indices] { get; }
        T this[ReadOnlySpan<long> indices] { get; }

        long GetDimensionLength(long dimension);
        IReadOnlyTensor<T> Slice(params (long start, long length)[] ranges);
        IReadOnlyTensor<T> Slice(ReadOnlySpan<(long start, long length)> ranges);
        IReadOnlyTensor<T> Reshape(params long[] dimensions);
        IReadOnlyTensor<T> Reshape(ReadOnlySpan<long> dimensions);
    }
}
