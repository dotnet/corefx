// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    public interface ITensor<T>
    {
        ReadOnlySpan<int> Dimensions { get; }
        bool IsReversedStride { get; }
        long Length { get; }
        int Rank { get; }
        ReadOnlySpan<int> Strides { get; }

        T this[params int[] indices] { get; set; }
        T this[ReadOnlySpan<int> indices] { get; set; }

        T GetValue(int index);
        void SetValue(int index, T value);

        ITensor<T> Clone();
        ITensor<T> CloneEmpty();
        ITensor<T> CloneEmpty(ReadOnlySpan<int> dimensions);
        ITensor<TResult> CloneEmpty<TResult>();
        ITensor<TResult> CloneEmpty<TResult>(ReadOnlySpan<int> dimensions);
        void Fill(T value);
        ITensor<T> GetDiagonal();
        ITensor<T> GetDiagonal(int offset);
        ITensor<T> GetTriangle();
        ITensor<T> GetTriangle(int offset);
        ITensor<T> GetUpperTriangle();
        ITensor<T> GetUpperTriangle(int offset);
        ITensor<T> Reshape(ReadOnlySpan<int> dimensions);

        IDenseTensor<T> ToDenseTensor();
        ISparseTensor<T> ToSparseTensor();
        ICompressedSparseTensor<T> ToCompressedSparseTensor();

        string GetArrayString(bool includeWhitespace = true);
    }
}
