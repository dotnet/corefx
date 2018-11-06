// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    /// <summary>
    /// Provides a view into, or slice of, a backing Tensor object. TensorView is used to provide
    /// a projection of a subset of data from the backing Tensor.
    /// </summary>
    internal class TensorView<T> : Tensor<T>
    {
        private readonly Tensor<T> _backingTensor;
        private readonly int[] _stridesIntoBackingTensor;
        private readonly int _offset;

        internal TensorView(Tensor<T> backingTensor, ReadOnlySpan<int> dimensions, ReadOnlySpan<int> stridesIntoBackingTensor, int offset)
            : base(dimensions, backingTensor.IsReversedStride)
        {
            if (stridesIntoBackingTensor.Length == 0)
            {
                throw new ArgumentException("Strides must contain elements.", nameof(stridesIntoBackingTensor));
            }
            if (dimensions.Length != stridesIntoBackingTensor.Length)
            {
                throw new ArgumentException($"dimensions.Length '{dimensions.Length}' must match backingTensorStrides.Length '{stridesIntoBackingTensor.Length}'");
            }

            _backingTensor = backingTensor;
            _offset = offset;
            _stridesIntoBackingTensor = new int[stridesIntoBackingTensor.Length];
            for (int i = 0; i < stridesIntoBackingTensor.Length; i++)
            {
                if (stridesIntoBackingTensor[i] < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(stridesIntoBackingTensor), "Strides must be positive and non-zero");
                }
                _stridesIntoBackingTensor[i] = stridesIntoBackingTensor[i];
            }
        }

        public override T this[ReadOnlySpan<int> indices]
        {
            get
            {
                EnsureValidIndices(indices);

                return _backingTensor.GetValue(_offset + ArrayUtilities.GetIndex(_stridesIntoBackingTensor, indices));
            }
            set
            {
                EnsureValidIndices(indices);

                _backingTensor.SetValue(_offset + ArrayUtilities.GetIndex(_stridesIntoBackingTensor, indices), value);
            }
        }

        public override T GetValue(int index)
        {
            EnsureValidIndex(index);

            Span<int> indices = Rank < ArrayUtilities.StackallocMax ? stackalloc int[Rank] : new int[Rank];
            ArrayUtilities.GetIndices(Strides, IsReversedStride, index, indices);

            return this[indices];
        }

        public override void SetValue(int index, T value)
        {
            EnsureValidIndex(index);

            Span<int> indices = Rank < ArrayUtilities.StackallocMax ? stackalloc int[Rank] : new int[Rank];
            ArrayUtilities.GetIndices(Strides, IsReversedStride, index, indices);

            this[indices] = value;
        }

        public override Tensor<T> Clone()
        {
            Tensor<T> result = _backingTensor.CloneEmpty(Dimensions);

            // TODO: don't use the linearized index here since we always need to re-calculate the indices.
            // Instead, we should have an enumerator that works on indices.
            for (int i = 0; i < Length; i++)
            {
                result.SetValue(i, GetValue(i));
            }

            return result;
        }

        public override Tensor<TResult> CloneEmpty<TResult>(ReadOnlySpan<int> dimensions)
        {
            return _backingTensor.CloneEmpty<TResult>(dimensions);
        }

        public override Tensor<T> Reshape(ReadOnlySpan<int> dimensions)
        {
            if (dimensions.Length == 0)
            {
                throw new ArgumentException("Dimensions must contain elements.", nameof(dimensions));
            }

            var newSize = ArrayUtilities.GetProduct(dimensions);

            if (newSize != Length)
            {
                throw new ArgumentException($"Cannot reshape array due to mismatch in lengths, currently {Length} would become {newSize}.", nameof(dimensions));
            }

            Tensor<T> result = _backingTensor.CloneEmpty(dimensions);

            // TODO: don't use the linearized index here since we always need to re-calculate the indices.
            // Instead, we should have an enumerator that works on indices.
            for (int i = 0; i < Length; i++)
            {
                result.SetValue(i, GetValue(i));
            }

            return result;
        }

        private void EnsureValidIndices(ReadOnlySpan<int> indices)
        {
            if (indices.Length != Rank)
            {
                throw new ArgumentException($"{nameof(indices)} Length '{indices.Length}' must match Rank '{Rank}' of the current Tensor.", nameof(indices));
            }

            for (int i = 0; i < indices.Length; i++)
            {
                if ((uint)indices[i] >= (uint)Dimensions[i])
                {
                    throw new ArgumentOutOfRangeException($"Index at position '{i}' must be non-negative and less than the Dimension '{Dimensions[i]}' at that position. The index value is '{indices[i]}'.", nameof(indices));
                }
            }
        }

        private void EnsureValidIndex(int index)
        {
            if ((uint)index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index with value '{index}' must be non-negative and less than Length '{Length}'.");
            }
        }
    }
}
