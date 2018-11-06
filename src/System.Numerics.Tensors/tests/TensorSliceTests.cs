// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Linq;
using Xunit;

namespace System.Numerics.Tensors.Tests
{
    public class TensorSliceTests : TensorTestsBase
    {
        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceTest(TensorConstructor constructor)
        {
            var arr = Enumerable.Range(0, 24).ToArray();
            var tensor = constructor.CreateFromArray<int>(arr).Reshape(new[] { 2, 3, 4 });

            // default              reverse stride
            //{{{  0, 1, 2, 3},         {{{ 0, 6,12,18},
            //  {  4, 5, 6, 7},           { 2, 8,14,20},
            //  {  8, 9,10,11}},          { 4,10,16,22}},
            //
            // {{ 12,13,14*,15*},        {{ 1, 7,13*,19*},
            //  { 16,17,18*,19*},         { 3, 9,15*,21*},
            //  { 20,21,22*,23*}}}        { 5,11,17*,23*}}}

            // get the *'d elements above
            var slice = tensor.Slice(new Range(1, 1), new Range(0, 3), new Range(2, 2));

            Assert.Equal(2, slice.Rank);
            Assert.Equal(new[] { 3, 2 }, slice.Dimensions.ToArray());

            int[] expectedCollection = constructor.IsReversedStride ?
                new[] { 13, 15, 17, 19, 21, 23 } :
                new[] { 14, 15, 18, 19, 22, 23 };

            if (constructor.IsReversedStride)
            {
                Assert.Equal(expectedCollection[0], slice[0, 0]);
                Assert.Equal(expectedCollection[1], slice[1, 0]);
                Assert.Equal(expectedCollection[2], slice[2, 0]);
                Assert.Equal(expectedCollection[3], slice[0, 1]);
                Assert.Equal(expectedCollection[4], slice[1, 1]);
                Assert.Equal(expectedCollection[5], slice[2, 1]);
            }
            else
            {
                Assert.Equal(expectedCollection[0], slice[0, 0]);
                Assert.Equal(expectedCollection[1], slice[0, 1]);
                Assert.Equal(expectedCollection[2], slice[1, 0]);
                Assert.Equal(expectedCollection[3], slice[1, 1]);
                Assert.Equal(expectedCollection[4], slice[2, 0]);
                Assert.Equal(expectedCollection[5], slice[2, 1]);
            }

            Assert.Equal(expectedCollection[0], slice.GetValue(0));
            Assert.Equal(expectedCollection[1], slice.GetValue(1));
            Assert.Equal(expectedCollection[2], slice.GetValue(2));
            Assert.Equal(expectedCollection[3], slice.GetValue(3));
            Assert.Equal(expectedCollection[4], slice.GetValue(4));
            Assert.Equal(expectedCollection[5], slice.GetValue(5));

            // Test the enumerator works correctly
            Assert.Equal(expectedCollection, slice);

            // ensure the underlying tensor is mutated correctly
            slice[1, 0] = 100;
            Assert.Equal(100, tensor[1, 1, 2]);

            slice.SetValue(5, 100);
            Assert.Equal(100, tensor[1, 2, 3]);
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceMatrixByColumnTest(TensorConstructor constructor)
        {
            var arr = Enumerable.Range(0, 12).ToArray();
            var tensor = constructor.CreateFromArray<int>(arr).Reshape(new[] { 3, 4 });

            // default              reverse stride
            //{{  0, 1, 2, 3*},         {{ 0, 3, 6, 9*},
            // {  4, 5, 6, 7*},          { 1, 4, 7,10*},
            // {  8, 9,10,11*}}          { 2, 5, 8,11*}}

            // get the *'d elements above
            var slice = tensor.Slice(new Range(0, 3), new Range(3, 1));

            Assert.Equal(1, slice.Rank);
            Assert.Equal(new[] { 3 }, slice.Dimensions.ToArray());

            int[] expectedCollection = constructor.IsReversedStride ?
                 new[] { 9, 10, 11 } :
                 new[] { 3, 7, 11 };

            Assert.Equal(expectedCollection[0], slice[0]);
            Assert.Equal(expectedCollection[1], slice[1]);
            Assert.Equal(expectedCollection[2], slice[2]);

            Assert.Equal(expectedCollection[0], slice.GetValue(0));
            Assert.Equal(expectedCollection[1], slice.GetValue(1));
            Assert.Equal(expectedCollection[2], slice.GetValue(2));

            // Test the enumerator works correctly
            Assert.Equal(expectedCollection, slice);
        }

        private static Tensor<int> Get3DSlice(TensorConstructor constructor)
        {
            var arr = new[, ,]
            {
                {
                   { 0, 1,  2,  3 },
                   { 4, 5,  6,  7 },
                   { 8, 9, 10, 11 },
                },
                {
                   { 12, 13, 14, 15 },
                   { 16, 17, 18, 19 },
                   { 20, 21, 22, 23 },
                },

            };
            var tensor = constructor.CreateFromArray<int>(arr).Reshape(new[] { 2, 3, 4 });

            //{{{  0, 1, 2*, 3*},
            //  {  4, 5, 6*, 7*},
            //  {  8, 9,10*,11*}},
            //
            // {{ 12,13,14*,15*},
            //  { 16,17,18*,19*},
            //  { 20,21,22*,23*}}}

            // get the *'d elements above
            return tensor.Slice(new Range(0, 2), new Range(0, 3), new Range(2, 2));
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceGetDiagonal(TensorConstructor constructor)
        {
            var slice = Get3DSlice(constructor);
            var diag = slice.GetDiagonal();

            var expected = new[,]
            {
                { 2, 3 },
                { 18, 19 },
            };
            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(diag, expected));
            Assert.Equal(constructor.IsReversedStride, diag.IsReversedStride);
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceIncrement(TensorConstructor constructor)
        {
            var slice = Get3DSlice(constructor);
            var incremented = TensorOperations.Increment(slice);

            var expected = new[, ,]
            {
                {
                   {  3,  4 },
                   {  7,  8 },
                   { 11, 12 },
                },
                {
                   { 15, 16 },
                   { 19, 20 },
                   { 23, 24 },
                },

            };
            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(incremented, expected));
            Assert.Equal(constructor.IsReversedStride, incremented.IsReversedStride);
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceReshape(TensorConstructor constructor)
        {
            var slice = Get3DSlice(constructor);
            var reshaped = slice.Reshape(new[] { 2, 2, 3 });

            int[,,] expected;
            if (constructor.IsReversedStride)
            {
                expected = new[, ,]
                {
                    {
                       {  2, 10, 7 },
                       {  6, 3, 11 },
                    },
                    {
                       { 14, 22, 19 },
                       { 18, 15, 23 },
                    },
                };

            }
            else
            {
                expected = new[, ,]
                {
                    {
                        {  2,  3,  6 },
                        {  7, 10, 11 },
                    },
                    {
                        { 14, 15, 18 },
                        { 19, 22, 23 },
                    },
                };
            }

            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(reshaped, expected));
            Assert.Equal(constructor.IsReversedStride, reshaped.IsReversedStride);
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void Slice1DSlice(TensorConstructor constructor)
        {
            var slice = Get3DSlice(constructor);
            var resliced = slice.Slice(new Range(0, 1), new Range(0, 3), new Range(1, 1));

            Assert.Equal(1, resliced.Rank);
            Assert.Equal(new[] { 3 }, resliced.Dimensions.ToArray());

            int[] expectedCollection = new[] { 3, 7, 11 };

            Assert.Equal(expectedCollection[0], resliced[0]);
            Assert.Equal(expectedCollection[1], resliced[1]);
            Assert.Equal(expectedCollection[2], resliced[2]);

            Assert.Equal(expectedCollection[0], resliced.GetValue(0));
            Assert.Equal(expectedCollection[1], resliced.GetValue(1));
            Assert.Equal(expectedCollection[2], resliced.GetValue(2));

            // Test the enumerator works correctly
            Assert.Equal(expectedCollection, resliced);
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void Slice2DSlice(TensorConstructor constructor)
        {
            var slice = Get3DSlice(constructor);
            var resliced = slice.Slice(new Range(0, 1), new Range(0, 2), new Range(0, 2));

            Assert.Equal(2, resliced.Rank);
            Assert.Equal(new[] { 2, 2 }, resliced.Dimensions.ToArray());

            int[] expectedCollection = constructor.IsReversedStride ?
                new[] { 2, 6, 3, 7 } :
                new[] { 2, 3, 6, 7 };

            if (constructor.IsReversedStride)
            {
                Assert.Equal(expectedCollection[0], resliced[0, 0]);
                Assert.Equal(expectedCollection[1], resliced[1, 0]);
                Assert.Equal(expectedCollection[2], resliced[0, 1]);
                Assert.Equal(expectedCollection[3], resliced[1, 1]);
            }
            else
            {
                Assert.Equal(expectedCollection[0], resliced[0, 0]);
                Assert.Equal(expectedCollection[1], resliced[0, 1]);
                Assert.Equal(expectedCollection[2], resliced[1, 0]);
                Assert.Equal(expectedCollection[3], resliced[1, 1]);
            }

            Assert.Equal(expectedCollection[0], resliced.GetValue(0));
            Assert.Equal(expectedCollection[1], resliced.GetValue(1));
            Assert.Equal(expectedCollection[2], resliced.GetValue(2));
            Assert.Equal(expectedCollection[3], resliced.GetValue(3));

            // Test the enumerator works correctly
            Assert.Equal(expectedCollection, resliced);
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceOutOfRange(TensorConstructor constructor)
        {
            var arr = new[, ,]
            {
                {
                   { 0, 1,  2,  3 },
                   { 4, 5,  6,  7 },
                   { 8, 9, 10, 11 },
                },
                {
                   { 12, 13, 14, 15 },
                   { 16, 17, 18, 19 },
                   { 20, 21, 22, 23 },
                },

            };
            var tensor = constructor.CreateFromArray<int>(arr).Reshape(new[] { 2, 3, 4 });

            // first dimension is invalid
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 3), new Range(0, 1), new Range(0, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 10), new Range(0, 1), new Range(0, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(1, 2), new Range(0, 1), new Range(0, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(2, 1), new Range(0, 1), new Range(0, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(-1, 1), new Range(0, 1), new Range(0, 1)));

            // third dimension is invalid
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 1), new Range(0, 1), new Range(0, 5)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 1), new Range(0, 1), new Range(0, 10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 1), new Range(0, 1), new Range(3, 2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 1), new Range(0, 1), new Range(4, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => tensor.Slice(new Range(0, 1), new Range(0, 1), new Range(-1, 1)));
        }

        [Theory]
        [MemberData(nameof(GetSingleTensorConstructors))]
        public void SliceIndexOutOfRange(TensorConstructor constructor)
        {
            var slice = Get3DSlice(constructor);

            Assert.Throws<ArgumentNullException>(() => slice[(int[])null]);
            Assert.Throws<ArgumentException>(() => slice[new int[] { }]);
            Assert.Throws<ArgumentException>(() => slice[default(ReadOnlySpan<int>)]);
            Assert.Throws<ArgumentException>(() => slice[2, 0]);
            Assert.Throws<ArgumentException>(() => slice[2, 0, 0, 4]);

            Assert.Throws<ArgumentOutOfRangeException>(() => slice[2, 0, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => slice[1, 3, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => slice[1, 0, -1]);

            Assert.Throws<ArgumentOutOfRangeException>(() => slice[2, 0, 0] = 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => slice.GetValue(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => slice.GetValue(12));
            Assert.Throws<ArgumentOutOfRangeException>(() => slice.GetValue(13));
            Assert.Throws<ArgumentOutOfRangeException>(() => slice.GetValue(100));

            Assert.Throws<ArgumentOutOfRangeException>(() => slice.SetValue(12, 10));
        }
    }
}
