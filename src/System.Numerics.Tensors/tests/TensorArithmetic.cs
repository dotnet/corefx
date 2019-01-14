// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Numerics.Tensors
{
    internal interface ITensorArithmetic<T>
    {
        T One { get; }
        T Zero { get; }
        void Add(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Add(ITensor<T> tensor, T scalar, ITensor<T> result);
        void And(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void And(ITensor<T> tensor, T scalar, ITensor<T> result);
        void Contract(ITensor<T> left, ITensor<T> right, int[] leftAxes, int[] rightAxes, ITensor<T> result);
        void Decrement(ITensor<T> tensor, ITensor<T> result);
        void Divide(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Divide(ITensor<T> tensor, T scalar, ITensor<T> result);
        void Equals(ITensor<T> left, ITensor<T> right, ITensor<bool> result);
        void GreaterThan(ITensor<T> left, ITensor<T> right, ITensor<bool> result);
        void GreaterThanOrEqual(ITensor<T> left, ITensor<T> right, ITensor<bool> result);
        void Increment(ITensor<T> tensor, ITensor<T> result);
        void LeftShift(ITensor<T> tensor, int value, ITensor<T> result);
        void LessThan(ITensor<T> left, ITensor<T> right, ITensor<bool> result);
        void LessThanOrEqual(ITensor<T> left, ITensor<T> right, ITensor<bool> result);
        void Modulo(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Modulo(ITensor<T> tensor, T scalar, ITensor<T> result);
        void Multiply(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Multiply(ITensor<T> tensor, T scalar, ITensor<T> result);
        void NotEquals(ITensor<T> left, ITensor<T> right, ITensor<bool> result);
        void Or(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Or(ITensor<T> tensor, T scalar, ITensor<T> result);
        void RightShift(ITensor<T> tensor, int value, ITensor<T> result);
        void Subtract(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Subtract(ITensor<T> tensor, T scalar, ITensor<T> result);
        void UnaryMinus(ITensor<T> tensor, ITensor<T> result);
        void UnaryPlus(ITensor<T> tensor, ITensor<T> result);
        void Xor(ITensor<T> left, ITensor<T> right, ITensor<T> result);
        void Xor(ITensor<T> tensor, T scalar, ITensor<T> result);
    }

    internal static class TensorArithmetic<T>
    {
        public static ITensorArithmetic<T> Instance => TensorArithmetic.GetArithmetic<T>();
    }

    internal static class TensorArithmetic
    { 
        public static ITensorArithmetic<T> GetArithmetic<T>()
        {
            if (typeof(T) == typeof(bool))
            {
                return (ITensorArithmetic<T>)new BoolArithmetic();
            }
            else if (typeof(T) == typeof(byte))
            {
                return (ITensorArithmetic<T>)new ByteArithmetic();
            }
            else if (typeof(T) == typeof(char))
            {
                return (ITensorArithmetic<T>)new CharArithmetic();
            }
            else if (typeof(T) == typeof(decimal))
            {
                return (ITensorArithmetic<T>)new DecimalArithmetic();
            }
            else if (typeof(T) == typeof(double))
            {
                return (ITensorArithmetic<T>)new DoubleArithmetic();
            }
            else if (typeof(T) == typeof(float))
            {
                return (ITensorArithmetic<T>)new FloatArithmetic();
            }
            else if (typeof(T) == typeof(int))
            {
                return (ITensorArithmetic<T>)new IntArithmetic();
            }
            else if (typeof(T) == typeof(long))
            {
                return (ITensorArithmetic<T>)new LongArithmetic();
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return (ITensorArithmetic<T>)new SByteArithmetic();
            }
            else if (typeof(T) == typeof(short))
            {
                return (ITensorArithmetic<T>)new ShortArithmetic();
            }
            else if (typeof(T) == typeof(uint))
            {
                return (ITensorArithmetic<T>)new UIntArithmetic();
            }
            else if (typeof(T) == typeof(ulong))
            {
                return (ITensorArithmetic<T>)new ULongArithmetic();
            }
            else if (typeof(T) == typeof(ushort))
            {
                return (ITensorArithmetic<T>)new UShortArithmetic();
            }
            return null;
        }
    }
    
    internal class BoolArithmetic : ITensorArithmetic<bool>
    {
        public bool One => true;
        public bool Zero => false;

        public void Add(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Add(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void And(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (bool)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (bool)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<bool> left, ITensor<bool> right, int[] leftAxes, int[] rightAxes, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Decrement(ITensor<bool> tensor, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Divide(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Divide(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Equals(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void GreaterThanOrEqual(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Increment(ITensor<bool> tensor, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void LeftShift(ITensor<bool> tensor, int value, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void LessThanOrEqual(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Modulo(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Modulo(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Multiply(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Multiply(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void NotEquals(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (bool)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (bool)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<bool> tensor, int value, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryMinus(ITensor<bool> tensor, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(ITensor<bool> tensor, ITensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(ITensor<bool> left, ITensor<bool> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (bool)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<bool> tensor, bool scalar, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (bool)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Add(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void And(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (bool)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (bool)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (bool)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (bool)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<bool> left, DenseTensor<bool> right, int[] leftAxes, int[] rightAxes, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Decrement(DenseTensor<bool> tensor, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Divide(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Divide(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Equals(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void GreaterThanOrEqual(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Increment(DenseTensor<bool> tensor, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void LeftShift(DenseTensor<bool> tensor, int value, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void LessThanOrEqual(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Modulo(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Modulo(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Multiply(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Multiply(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void NotEquals(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (bool)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (bool)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (bool)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (bool)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<bool> tensor, int value, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryMinus(DenseTensor<bool> tensor, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(DenseTensor<bool> tensor, DenseTensor<bool> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(DenseTensor<bool> left, DenseTensor<bool> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (bool)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (bool)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<bool> tensor, bool scalar, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (bool)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (bool)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class ByteArithmetic : ITensorArithmetic<byte>
    {
        public byte One => 1;
        public byte Zero => 0;

        public void Add(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<byte> left, ITensor<byte> right, int[] leftAxes, int[] rightAxes, ITensor<byte> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                byte sum = (byte)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (byte)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<byte> tensor, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<byte> left, ITensor<byte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<byte> left, ITensor<byte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<byte> left, ITensor<byte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<byte> tensor, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<byte> tensor, int value, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<byte> left, ITensor<byte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<byte> left, ITensor<byte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<byte> left, ITensor<byte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<byte> tensor, int value, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<byte> tensor, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<byte> tensor, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<byte> left, ITensor<byte> right, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<byte> tensor, byte scalar, ITensor<byte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (byte)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<byte> left, DenseTensor<byte> right, int[] leftAxes, int[] rightAxes, DenseTensor<byte> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                byte sum = (byte)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (byte)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<byte> tensor, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<byte> tensor, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<byte> tensor, int value, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<byte> tensor, int value, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<byte> tensor, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<byte> tensor, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<byte> left, DenseTensor<byte> right, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<byte> tensor, byte scalar, DenseTensor<byte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (byte)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (byte)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class CharArithmetic : ITensorArithmetic<char>
    {
        public char One => (char)1;
        public char Zero => (char)0;

        public void Add(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<char> left, ITensor<char> right, int[] leftAxes, int[] rightAxes, ITensor<char> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                char sum = (char)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (char)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<char> tensor, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<char> left, ITensor<char> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<char> left, ITensor<char> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<char> left, ITensor<char> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<char> tensor, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<char> tensor, int value, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<char> left, ITensor<char> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<char> left, ITensor<char> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<char> left, ITensor<char> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<char> tensor, int value, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<char> tensor, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<char> tensor, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<char> left, ITensor<char> right, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<char> tensor, char scalar, ITensor<char> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (char)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<char> left, DenseTensor<char> right, int[] leftAxes, int[] rightAxes, DenseTensor<char> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                char sum = (char)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (char)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<char> tensor, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<char> tensor, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<char> tensor, int value, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<char> tensor, int value, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<char> tensor, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<char> tensor, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<char> left, DenseTensor<char> right, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<char> tensor, char scalar, DenseTensor<char> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (char)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (char)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class DecimalArithmetic : ITensorArithmetic<decimal>
    {
        public decimal One => 1;
        public decimal Zero => 0;

        public void Add(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void And(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Contract(ITensor<decimal> left, ITensor<decimal> right, int[] leftAxes, int[] rightAxes, ITensor<decimal> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                decimal sum = (decimal)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (decimal)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<decimal> tensor, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<decimal> left, ITensor<decimal> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<decimal> left, ITensor<decimal> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<decimal> left, ITensor<decimal> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<decimal> tensor, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<decimal> tensor, int value, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(ITensor<decimal> left, ITensor<decimal> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<decimal> left, ITensor<decimal> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<decimal> left, ITensor<decimal> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Or(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void RightShift(ITensor<decimal> tensor, int value, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<decimal> tensor, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<decimal> tensor, ITensor<decimal> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (decimal)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<decimal> left, ITensor<decimal> right, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(ITensor<decimal> tensor, decimal scalar, ITensor<decimal> result)
        {
            throw new NotSupportedException();
        }

        public void Add(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void And(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Contract(DenseTensor<decimal> left, DenseTensor<decimal> right, int[] leftAxes, int[] rightAxes, DenseTensor<decimal> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                decimal sum = (decimal)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (decimal)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<decimal> tensor, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<decimal> tensor, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<decimal> tensor, int value, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Or(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void RightShift(DenseTensor<decimal> tensor, int value, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<decimal> tensor, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<decimal> tensor, DenseTensor<decimal> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (decimal)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (decimal)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<decimal> left, DenseTensor<decimal> right, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(DenseTensor<decimal> tensor, decimal scalar, DenseTensor<decimal> result)
        {
            throw new NotSupportedException();
        }
    }
    internal class DoubleArithmetic : ITensorArithmetic<double>
    {
        public double One => 1.0;
        public double Zero => 0;

        public void Add(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<double> tensor, double scalar, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void And(ITensor<double> tensor, double scalar, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Contract(ITensor<double> left, ITensor<double> right, int[] leftAxes, int[] rightAxes, ITensor<double> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                double sum = (double)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (double)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<double> tensor, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<double> tensor, double scalar, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<double> left, ITensor<double> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<double> left, ITensor<double> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<double> left, ITensor<double> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<double> tensor, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<double> tensor, int value, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(ITensor<double> left, ITensor<double> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<double> left, ITensor<double> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<double> tensor, double scalar, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<double> tensor, double scalar, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<double> left, ITensor<double> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Or(ITensor<double> tensor, double scalar, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void RightShift(ITensor<double> tensor, int value, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<double> tensor, double scalar, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<double> tensor, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<double> tensor, ITensor<double> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (double)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<double> left, ITensor<double> right, ITensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(ITensor<double> tensor, double scalar, ITensor<double> result)
        {
            throw new NotSupportedException();
        }

        public void Add(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void And(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Contract(DenseTensor<double> left, DenseTensor<double> right, int[] leftAxes, int[] rightAxes, DenseTensor<double> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                double sum = (double)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (double)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<double> tensor, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<double> tensor, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<double> tensor, int value, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Or(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void RightShift(DenseTensor<double> tensor, int value, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<double> tensor, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<double> tensor, DenseTensor<double> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (double)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (double)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<double> left, DenseTensor<double> right, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(DenseTensor<double> tensor, double scalar, DenseTensor<double> result)
        {
            throw new NotSupportedException();
        }
    }
    internal class FloatArithmetic : ITensorArithmetic<float>
    {
        public float One => 1.0f;
        public float Zero => 0;

        public void Add(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<float> tensor, float scalar, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void And(ITensor<float> tensor, float scalar, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Contract(ITensor<float> left, ITensor<float> right, int[] leftAxes, int[] rightAxes, ITensor<float> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                float sum = (float)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (float)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<float> tensor, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<float> tensor, float scalar, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<float> left, ITensor<float> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<float> left, ITensor<float> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<float> left, ITensor<float> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<float> tensor, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<float> tensor, int value, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(ITensor<float> left, ITensor<float> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<float> left, ITensor<float> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<float> tensor, float scalar, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<float> tensor, float scalar, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<float> left, ITensor<float> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Or(ITensor<float> tensor, float scalar, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void RightShift(ITensor<float> tensor, int value, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<float> tensor, float scalar, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<float> tensor, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<float> tensor, ITensor<float> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (float)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<float> left, ITensor<float> right, ITensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(ITensor<float> tensor, float scalar, ITensor<float> result)
        {
            throw new NotSupportedException();
        }

        public void Add(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void And(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Contract(DenseTensor<float> left, DenseTensor<float> right, int[] leftAxes, int[] rightAxes, DenseTensor<float> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                float sum = (float)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (float)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<float> tensor, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<float> tensor, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<float> tensor, int value, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void LessThan(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Or(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void RightShift(DenseTensor<float> tensor, int value, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Subtract(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<float> tensor, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<float> tensor, DenseTensor<float> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (float)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (float)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<float> left, DenseTensor<float> right, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
        public void Xor(DenseTensor<float> tensor, float scalar, DenseTensor<float> result)
        {
            throw new NotSupportedException();
        }
    }
    internal class IntArithmetic : ITensorArithmetic<int>
    {
        public int One => 1;
        public int Zero => 0;

        public void Add(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<int> left, ITensor<int> right, int[] leftAxes, int[] rightAxes, ITensor<int> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                int sum = (int)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (int)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<int> tensor, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<int> left, ITensor<int> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<int> left, ITensor<int> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<int> left, ITensor<int> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<int> tensor, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<int> tensor, int value, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<int> left, ITensor<int> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<int> left, ITensor<int> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<int> left, ITensor<int> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<int> tensor, int value, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<int> tensor, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<int> tensor, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<int> left, ITensor<int> right, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<int> tensor, int scalar, ITensor<int> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (int)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<int> left, DenseTensor<int> right, int[] leftAxes, int[] rightAxes, DenseTensor<int> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                int sum = (int)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (int)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<int> tensor, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<int> tensor, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<int> tensor, int value, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<int> tensor, int value, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<int> tensor, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<int> tensor, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<int> left, DenseTensor<int> right, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<int> tensor, int scalar, DenseTensor<int> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (int)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (int)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class LongArithmetic : ITensorArithmetic<long>
    {
        public long One => 1;
        public long Zero => 0;

        public void Add(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<long> left, ITensor<long> right, int[] leftAxes, int[] rightAxes, ITensor<long> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                long sum = (long)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (long)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<long> tensor, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<long> left, ITensor<long> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<long> left, ITensor<long> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<long> left, ITensor<long> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<long> tensor, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<long> tensor, int value, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<long> left, ITensor<long> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<long> left, ITensor<long> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<long> left, ITensor<long> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<long> tensor, int value, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<long> tensor, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<long> tensor, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<long> left, ITensor<long> right, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<long> tensor, long scalar, ITensor<long> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (long)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<long> left, DenseTensor<long> right, int[] leftAxes, int[] rightAxes, DenseTensor<long> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                long sum = (long)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (long)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<long> tensor, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<long> tensor, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<long> tensor, int value, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<long> tensor, int value, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<long> tensor, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<long> tensor, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<long> left, DenseTensor<long> right, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<long> tensor, long scalar, DenseTensor<long> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (long)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (long)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class SByteArithmetic : ITensorArithmetic<sbyte>
    {
        public sbyte One => 1;
        public sbyte Zero => 0;

        public void Add(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<sbyte> left, ITensor<sbyte> right, int[] leftAxes, int[] rightAxes, ITensor<sbyte> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                sbyte sum = (sbyte)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (sbyte)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<sbyte> tensor, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<sbyte> tensor, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<sbyte> tensor, int value, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<sbyte> tensor, int value, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<sbyte> tensor, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<sbyte> tensor, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<sbyte> left, ITensor<sbyte> right, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<sbyte> tensor, sbyte scalar, ITensor<sbyte> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (sbyte)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<sbyte> left, DenseTensor<sbyte> right, int[] leftAxes, int[] rightAxes, DenseTensor<sbyte> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                sbyte sum = (sbyte)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (sbyte)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<sbyte> tensor, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<sbyte> tensor, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<sbyte> tensor, int value, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<sbyte> tensor, int value, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<sbyte> tensor, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<sbyte> tensor, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<sbyte> left, DenseTensor<sbyte> right, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<sbyte> tensor, sbyte scalar, DenseTensor<sbyte> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (sbyte)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (sbyte)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class ShortArithmetic : ITensorArithmetic<short>
    {
        public short One => 1;
        public short Zero => 0;

        public void Add(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<short> left, ITensor<short> right, int[] leftAxes, int[] rightAxes, ITensor<short> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                short sum = (short)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (short)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<short> tensor, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<short> left, ITensor<short> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<short> left, ITensor<short> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<short> left, ITensor<short> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<short> tensor, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<short> tensor, int value, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<short> left, ITensor<short> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<short> left, ITensor<short> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<short> left, ITensor<short> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<short> tensor, int value, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<short> tensor, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)-tensor[indices];
            }
            
        }
        public void UnaryPlus(ITensor<short> tensor, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<short> left, ITensor<short> right, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<short> tensor, short scalar, ITensor<short> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (short)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<short> left, DenseTensor<short> right, int[] leftAxes, int[] rightAxes, DenseTensor<short> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                short sum = (short)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (short)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<short> tensor, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<short> tensor, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<short> tensor, int value, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<short> tensor, int value, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<short> tensor, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)-tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)-tensorSpan[op1Index];

                }
            }
        }
        public void UnaryPlus(DenseTensor<short> tensor, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<short> left, DenseTensor<short> right, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<short> tensor, short scalar, DenseTensor<short> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (short)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (short)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class UIntArithmetic : ITensorArithmetic<uint>
    {
        public uint One => 1;
        public uint Zero => 0;

        public void Add(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<uint> left, ITensor<uint> right, int[] leftAxes, int[] rightAxes, ITensor<uint> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                uint sum = (uint)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (uint)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<uint> tensor, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<uint> left, ITensor<uint> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<uint> left, ITensor<uint> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<uint> left, ITensor<uint> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<uint> tensor, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<uint> tensor, int value, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<uint> left, ITensor<uint> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<uint> left, ITensor<uint> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<uint> left, ITensor<uint> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<uint> tensor, int value, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<uint> tensor, ITensor<uint> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(ITensor<uint> tensor, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<uint> left, ITensor<uint> right, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<uint> tensor, uint scalar, ITensor<uint> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (uint)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<uint> left, DenseTensor<uint> right, int[] leftAxes, int[] rightAxes, DenseTensor<uint> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                uint sum = (uint)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (uint)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<uint> tensor, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<uint> tensor, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<uint> tensor, int value, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<uint> tensor, int value, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<uint> tensor, DenseTensor<uint> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(DenseTensor<uint> tensor, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<uint> left, DenseTensor<uint> right, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<uint> tensor, uint scalar, DenseTensor<uint> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (uint)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (uint)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class ULongArithmetic : ITensorArithmetic<ulong>
    {
        public ulong One => 1;
        public ulong Zero => 0;

        public void Add(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<ulong> left, ITensor<ulong> right, int[] leftAxes, int[] rightAxes, ITensor<ulong> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                ulong sum = (ulong)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (ulong)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<ulong> tensor, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<ulong> left, ITensor<ulong> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<ulong> left, ITensor<ulong> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<ulong> left, ITensor<ulong> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<ulong> tensor, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<ulong> tensor, int value, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<ulong> left, ITensor<ulong> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<ulong> left, ITensor<ulong> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<ulong> left, ITensor<ulong> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<ulong> tensor, int value, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<ulong> tensor, ITensor<ulong> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(ITensor<ulong> tensor, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<ulong> left, ITensor<ulong> right, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<ulong> tensor, ulong scalar, ITensor<ulong> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ulong)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<ulong> left, DenseTensor<ulong> right, int[] leftAxes, int[] rightAxes, DenseTensor<ulong> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                ulong sum = (ulong)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (ulong)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<ulong> tensor, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<ulong> tensor, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<ulong> tensor, int value, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<ulong> tensor, int value, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<ulong> tensor, DenseTensor<ulong> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(DenseTensor<ulong> tensor, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<ulong> left, DenseTensor<ulong> right, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<ulong> tensor, ulong scalar, DenseTensor<ulong> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ulong)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ulong)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
    internal class UShortArithmetic : ITensorArithmetic<ushort>
    {
        public ushort One => 1;
        public ushort Zero => 0;

        public void Add(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] + right[indices]);
            }
            
        }
        public void Add(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] + scalar);
            }
            
        }
        public void And(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] & right[indices]);
            }
            
        }
        public void And(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] & scalar);
            }
            
        }
        public void Contract(ITensor<ushort> left, ITensor<ushort> right, int[] leftAxes, int[] rightAxes, ITensor<ushort> result)
        {
            var leftIndices = new int[left.Rank];
            var rightIndices = new int[right.Rank];
            var resultIndices = new int[result.Rank];

            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);

            for (int resultIndex = 0; resultIndex < result.Length; resultIndex++)
            {
                ushort sum = (ushort)0;
                
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, resultIndex, resultIndices);

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    // todo, make this more efficient
                    ArrayUtilities.GetIndices(left.Strides, left.IsReversedStride, leftIndex, leftIndices);
                    ArrayUtilities.GetIndices(right.Strides, right.IsReversedStride, rightIndex, rightIndices);

                    sum += (ushort)(left[leftIndices] * right[rightIndices]);
                }
                
                result[resultIndices] = sum;
            }
        }
        public void Decrement(ITensor<ushort> tensor, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]--;
            }
            
        }
        public void Divide(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] / right[indices]);
            }
            
        }
        public void Divide(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] / scalar);
            }
            
        }
        public void Equals(ITensor<ushort> left, ITensor<ushort> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] == right[indices];
            }
            
        }
        public void GreaterThan(ITensor<ushort> left, ITensor<ushort> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] > right[indices];
            }
            
        }
        public void GreaterThanOrEqual(ITensor<ushort> left, ITensor<ushort> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] >= right[indices];
            }
            
        }
        public void Increment(ITensor<ushort> tensor, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices]++;
            }
            
        }
        public void LeftShift(ITensor<ushort> tensor, int value, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] << value);
            }
            
        }
        public void LessThan(ITensor<ushort> left, ITensor<ushort> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] < right[indices];
            }
            
        }
        public void LessThanOrEqual(ITensor<ushort> left, ITensor<ushort> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] <= right[indices];
            }
            
        }
        public void Modulo(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] % right[indices]);
            }
            
        }
        public void Modulo(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] % scalar);
            }
            
        }
        public void Multiply(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] * right[indices]);
            }
            
        }
        public void Multiply(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] * scalar);
            }
            
        }
        public void NotEquals(ITensor<ushort> left, ITensor<ushort> right, ITensor<bool> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = left[indices] != right[indices];
            }
            
        }
        public void Or(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] | right[indices]);
            }
            
        }
        public void Or(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] | scalar);
            }
            
        }
        public void RightShift(ITensor<ushort> tensor, int value, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] >> value);
            }
            
        }
        public void Subtract(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] - right[indices]);
            }
            
        }
        public void Subtract(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] - scalar);
            }
            
        }
        public void UnaryMinus(ITensor<ushort> tensor, ITensor<ushort> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(ITensor<ushort> tensor, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)+tensor[indices];
            }
            
        }
        public void Xor(ITensor<ushort> left, ITensor<ushort> right, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(left[indices] ^ right[indices]);
            }
            
        }
        public void Xor(ITensor<ushort> tensor, ushort scalar, ITensor<ushort> result)
        {

            Span<int> indices = new Span<int>(new int[result.Rank]);
            for(int i = 0; i < result.Length; i++)
            {
                ArrayUtilities.GetIndices(result.Strides, result.IsReversedStride, i, indices);
                result[indices] = (ushort)(tensor[indices] ^ scalar);
            }
            
        }

        public void Add(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] + rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] + rightSpan[op2Index]);

                }
            }
        }
        public void Add(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] + scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] + scalar);

                }
            }
        }
        public void And(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] & rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] & rightSpan[op2Index]);

                }
            }
        }
        public void And(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] & scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] & scalar);

                }
            }
        }
        public void Contract(DenseTensor<ushort> left, DenseTensor<ushort> right, int[] leftAxes, int[] rightAxes, DenseTensor<ushort> result)
        {
            var summingDimensions = new int[leftAxes.Length];
            for(int i = 0; i < leftAxes.Length; i++)
            {
                summingDimensions[i] = left.Dimensions[leftAxes[i]];
            }

            var summingStrides = ArrayUtilities.GetStrides(summingDimensions);
            int summingLength = (int)ArrayUtilities.GetProduct(summingDimensions);

            var resultStrides = result.Strides;

            // translates from result index to left non-summing dimensions' index portion
            // since left non-summing dimensions are given precedence in result, the end is zero-padded
            int[] leftNonSummingStrides = new int[result.Rank];

            // translates from summing index to left summing dimensions' index portion
            int[] leftSummingStrides = new int[leftAxes.Length];
            ArrayUtilities.SplitStrides(left.Strides, leftAxes, leftNonSummingStrides, leftSummingStrides);

            // translates from result index to right non-summing dimensions' index portion
            int[] rightNonSummingStrides = new int[result.Rank];
            //  right non-summing dimensions appear after left non-summing dimensions.
            int rightNonSummingStridesOffset = (left.Rank - leftAxes.Length);

            // translates from summing index to right summing dimensions' index portion
            int[] rightSummingStrides = new int[rightAxes.Length];
            ArrayUtilities.SplitStrides(right.Strides, rightAxes, rightNonSummingStrides.AsSpan(rightNonSummingStridesOffset), rightSummingStrides);
            
            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;

            for (int resultIndex = 0; resultIndex < resultSpan.Length; resultIndex++)
            {
                ushort sum = (ushort)0;

                int leftIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, leftNonSummingStrides);
                int rightIndexNonSumming = ArrayUtilities.TransformIndexByStrides(resultIndex, resultStrides, result.IsReversedStride, rightNonSummingStrides);

                for (int summingIndex = 0; summingIndex < summingLength; summingIndex++)
                {
                    int leftIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, leftSummingStrides);
                    int rightIndexSumming = ArrayUtilities.TransformIndexByStrides(summingIndex, summingStrides, false, rightSummingStrides);

                    int leftIndex = leftIndexNonSumming + leftIndexSumming;
                    int rightIndex = rightIndexNonSumming + rightIndexSumming;

                    sum += (ushort)(leftSpan[leftIndex] * rightSpan[rightIndex]);
                }

                resultSpan[resultIndex] = sum;
            }
        }
        public void Decrement(DenseTensor<ushort> tensor, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]--;
            }
        }
        public void Divide(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] / rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] / rightSpan[op2Index]);

                }
            }
        }
        public void Divide(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] / scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] / scalar);

                }
            }
        }
        public void Equals(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] == rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] == rightSpan[op2Index];

                }
            }
        }
        public void GreaterThan(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] > rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] > rightSpan[op2Index];

                }
            }
        }
        public void GreaterThanOrEqual(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] >= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] >= rightSpan[op2Index];

                }
            }
        }
        public void Increment(DenseTensor<ushort> tensor, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            for(int i = 0; i < resultSpan.Length; i++)
            {
                resultSpan[i]++;
            }
        }
        public void LeftShift(DenseTensor<ushort> tensor, int value, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] << value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] << value);

                }
            }
        }
        public void LessThan(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] < rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] < rightSpan[op2Index];

                }
            }
        }
        public void LessThanOrEqual(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] <= rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] <= rightSpan[op2Index];

                }
            }
        }
        public void Modulo(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] % rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] % rightSpan[op2Index]);

                }
            }
        }
        public void Modulo(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] % scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] % scalar);

                }
            }
        }
        public void Multiply(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] * rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] * rightSpan[op2Index]);

                }
            }
        }
        public void Multiply(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] * scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] * scalar);

                }
            }
        }
        public void NotEquals(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<bool> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = leftSpan[i] != rightSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = leftSpan[op1Index] != rightSpan[op2Index];

                }
            }
        }
        public void Or(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] | rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] | rightSpan[op2Index]);

                }
            }
        }
        public void Or(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] | scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] | scalar);

                }
            }
        }
        public void RightShift(DenseTensor<ushort> tensor, int value, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] >> value);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] >> value);

                }
            }
        }
        public void Subtract(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] - rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] - rightSpan[op2Index]);

                }
            }
        }
        public void Subtract(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] - scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] - scalar);

                }
            }
        }
        public void UnaryMinus(DenseTensor<ushort> tensor, DenseTensor<ushort> result)
        {
            throw new NotSupportedException();
        }
        public void UnaryPlus(DenseTensor<ushort> tensor, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)+tensorSpan[i];
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)+tensorSpan[op1Index];

                }
            }
        }
        public void Xor(DenseTensor<ushort> left, DenseTensor<ushort> right, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var leftSpan = left.Buffer.Span;
            var rightSpan = right.Buffer.Span;
            if  ((result.IsReversedStride == left.IsReversedStride) && (result.IsReversedStride == right.IsReversedStride))
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(leftSpan[i] ^ rightSpan[i]);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref left.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                ref int op2Index = ref right.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;

                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      !left.IsReversedStride ? left.Strides : 
                                      right.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         left.IsReversedStride ? left.Strides : 
                                         right.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(leftSpan[op1Index] ^ rightSpan[op2Index]);

                }
            }
        }
        public void Xor(DenseTensor<ushort> tensor, ushort scalar, DenseTensor<ushort> result)
        {

            var resultSpan = result.Buffer.Span;
            var tensorSpan = tensor.Buffer.Span;
            if  (result.IsReversedStride == tensor.IsReversedStride)
            {
                for(int i = 0; i < resultSpan.Length; i++)
                {
                    resultSpan[i] = (ushort)(tensorSpan[i] ^ scalar);
                }
            }
            else
            {
                int rowMajorIndex = 0;
                int colMajorIndex = 0;
                
                ref int resultIndex = ref result.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                ref int op1Index = ref tensor.IsReversedStride ? ref colMajorIndex : ref rowMajorIndex;
                
                var rowMajorStrides = !result.IsReversedStride ? result.Strides :
                                      tensor.Strides;
                var columnMajorStrides = result.IsReversedStride ? result.Strides :
                                         tensor.Strides;
                for(;rowMajorIndex < resultSpan.Length; rowMajorIndex++)
                {
                    colMajorIndex = ArrayUtilities.TransformIndexByStrides(rowMajorIndex, rowMajorStrides, false, columnMajorStrides);
                    
                    resultSpan[resultIndex] = (ushort)(tensorSpan[op1Index] ^ scalar);

                }
            }
        }
    }
}
