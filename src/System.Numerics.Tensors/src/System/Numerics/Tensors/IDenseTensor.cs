// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    public interface IDenseTensor<T> : ITensor<T>
    {
        Memory<T> Buffer { get; }
    }
}
