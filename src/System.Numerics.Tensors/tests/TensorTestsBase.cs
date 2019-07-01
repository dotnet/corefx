﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Numerics.Tensors.Tests
{
    public class TensorTestsBase
    {
        public enum TensorType
        {
            Dense,
            Sparse,
            CompressedSparse
        };

        public class TensorConstructor
        {
            public TensorType TensorType { get; set; }

            public bool IsReversedStride { get; set; }

            public Tensor<T> CreateFromArray<T>(Array array)
            {
                switch (TensorType)
                {
                    case TensorType.Dense:
                        return array.ToTensor<T>(IsReversedStride);
                    case TensorType.Sparse:
                        return array.ToSparseTensor<T>(IsReversedStride);
                    case TensorType.CompressedSparse:
                        return array.ToCompressedSparseTensor<T>(IsReversedStride);
                }

                throw new ArgumentException(nameof(TensorType));
            }
            public Tensor<T> CreateFromDimensions<T>(ReadOnlySpan<int> dimensions)
            {
                switch (TensorType)
                {
                    case TensorType.Dense:
                        return new DenseTensor<T>(dimensions, IsReversedStride);
                    case TensorType.Sparse:
                        return new SparseTensor<T>(dimensions, IsReversedStride);
                    case TensorType.CompressedSparse:
                        return new CompressedSparseTensor<T>(dimensions, IsReversedStride);
                }

                throw new ArgumentException(nameof(TensorType));
            }

            public override string ToString()
            {
                return $"{TensorType}, {nameof(IsReversedStride)} = {IsReversedStride}";
            }
        }

        private static TensorType[] s_tensorTypes = new[]
        {
            TensorType.Dense,
            TensorType.Sparse,
            TensorType.CompressedSparse
        };

        private static bool[] s_reverseStrideValues = new[]
        {
            false,
            true
        };

        public static IEnumerable<object[]> GetSingleTensorConstructors()
        {
            foreach (TensorType tensorType in s_tensorTypes)
            {
                foreach (bool isReversedStride in s_reverseStrideValues)
                {
                    yield return new[]
                    {
                        new TensorConstructor()
                        {
                            TensorType = tensorType,
                            IsReversedStride = isReversedStride
                        }
                    };
                }
            }
        }

        public static IEnumerable<object[]> GetDualTensorConstructors()
        {
            foreach (TensorType leftTensorType in s_tensorTypes)
            {
                foreach (TensorType rightTensorType in s_tensorTypes)
                {
                    foreach (bool isLeftReversedStride in s_reverseStrideValues)
                    {
                        foreach (bool isRightReversedStride in s_reverseStrideValues)
                        {
                            yield return new[]
                            {
                                new TensorConstructor()
                                {
                                    TensorType = leftTensorType,
                                    IsReversedStride = isLeftReversedStride
                                },
                                new TensorConstructor()
                                {
                                    TensorType = rightTensorType,
                                    IsReversedStride = isRightReversedStride
                                }
                            };
                        }
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetTensorAndResultConstructor()
        {
            foreach (TensorType leftTensorType in s_tensorTypes)
            {
                foreach (TensorType rightTensorType in s_tensorTypes)
                {
                    foreach (bool isReversedStride in s_reverseStrideValues)
                    {
                        yield return new[]
                        {
                            new TensorConstructor()
                            {
                                TensorType = leftTensorType,
                                IsReversedStride = isReversedStride
                            },
                            new TensorConstructor()
                            {
                                TensorType = rightTensorType,
                                IsReversedStride = isReversedStride
                            }
                        };
                    }
                }
            }
        }

        public static NativeMemory<T> NativeMemoryFromArray<T>(T[] array)
        {
            return NativeMemoryFromArray<T>((Array)array);
        }

        public static NativeMemory<T> NativeMemoryFromArray<T>(Array array)
        {
            // this silly method takes a managed array and copies it over to unmanaged memory,
            // **only for test purposes**

            var memory = NativeMemory<T>.Allocate(array.Length);
            var span = memory.GetSpan();
            int index = 0;
            foreach (T item in array)
            {
                span[index++] = item;
            }

            return memory;
        }
    }
}
