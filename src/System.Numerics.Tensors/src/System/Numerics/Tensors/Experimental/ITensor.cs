// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    public interface ITensor<T> : IReadOnlyTensor<T>
    {
        new T this[params long[] indices] { get; set; }
        new T this[ReadOnlySpan<long> indices] { get; set; }
    }
}
