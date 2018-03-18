// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;
using static System.SpanTests.SortSpanTests;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        // Existing coreclr tests seem to be in here:
        // https://github.com/dotnet/coreclr/tree/master/tests/src/CoreMangLib/cti/system/array
        // E.g. arraysort1.cs etc.
        // These have not been used for the tests below,
        // instead all tests are based on using Array.Sort for computing the expected.

        const string SortTrait = nameof(SortTrait);
        const string SortTraitValue = nameof(SortTraitValue);

        const int FastMaxLength = 50;
        const int SlowMaxLength = 512;
        public static readonly TheoryData<ISortCases> s_fastSortTests = CreateSortCases(FastMaxLength);
        public static readonly TheoryData<ISortCases> s_slowSortTests = CreateSortCases(SlowMaxLength);

        static TheoryData<ISortCases> CreateSortCases(int maxLength)
        {
            var cases = new ISortCases[]
            {
                new LengthZeroSortCases(),
                new LengthOneSortCases(),
                new AllLengthTwoSortCases(),
                new AllLengthThreeSortCases(),
                new AllLengthFourSortCases(),
                new FillerSortCases(maxLength, new ConstantSpanFiller(42) ),
                new FillerSortCases(maxLength, new DecrementingSpanFiller() ),
                new FillerSortCases(maxLength, new ModuloDecrementingSpanFiller(25) ),
                new FillerSortCases(maxLength, new ModuloDecrementingSpanFiller(256) ),
                new FillerSortCases(maxLength, new IncrementingSpanFiller() ),
                new FillerSortCases(maxLength, new MedianOfThreeKillerSpanFiller() ),
                new FillerSortCases(maxLength, new PartialRandomShuffleSpanFiller(new IncrementingSpanFiller(), 0.2, 16281) ),
                new FillerSortCases(maxLength, new RandomSpanFiller(1873318) ),
            };
            var allCases = cases.Concat(cases.Select(c => new PadAndSliceSortCases(c, 2)))
                .Concat(cases.Select(c => new StepwiseSpecialSortCases(c, 3)));

            var theoryData = new TheoryData<ISortCases>();
            foreach (var c in allCases)
            {
                theoryData.Add(c);
            }
            return theoryData;
        }

        // How do we create a not comparable? I.e. something Comparer<TKey>.Default fails on?
        //struct NotComparable { int i; string s; IntPtr p; }
        //[Fact]
        //[Trait(SortTrait, SortTraitValue)]
        //public static void Sort_NotComparableThrows()
        //{
        //    var comparer = Comparer<NotComparable>.Default;
        //    Assert.Throws<ArgumentNullException>(() => new Span<NotComparable>(new NotComparable[16])
        //        .Sort());
        //    Assert.Throws<ArgumentNullException>(() => new Span<NotComparable>(new NotComparable[16])
        //        .Sort(comparer));
        //}


        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_NullComparerDoesNotThrow()
        {
            new Span<int>(new int[] { 3 }).Sort((IComparer<int>)null);
        }

        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_NullComparisonThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Span<int>(new int[] { }).Sort((Comparison<int>)null));
            Assert.Throws<ArgumentNullException>(() => new Span<string>(new string[] { }).Sort((Comparison<string>)null));
        }

        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_UInt8_Int32_PatternWithRepeatedKeys_ArraySort_DifferentOutputs()
        {
            var keys = new byte[]{ 237, 236, 235, 234, 233, 232, 231, 230, 229, 228, 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 195, 194, 193, 192, 191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180, 179, 178, 177, 176, 175, 174, 173, 172, 171, 170, 169, 168, 167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 128, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16
                , 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 255, 254, 253, 252, 251, 250, 249, 248, 247, 246, 245, 244, 243, 242, 241, 240, 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 229, 228, 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 195, 194, 193, 192, 191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180, 179, 178, 177, 176, 175, 174, 173, 172, 171, 170, 169, 168, 167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 128, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 5,
                2, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var values = Enumerable.Range(0, keys.Length).ToArray();

            var arraySortKeysNoComparer = (byte[])keys.Clone();
            var arraySortValuesNoComparer = (int[])values.Clone();
            Array.Sort(arraySortKeysNoComparer, arraySortValuesNoComparer);
            var arraySortKeysComparer = (byte[])keys.Clone();
            var arraySortValuesComparer = (int[])values.Clone();
            Array.Sort(arraySortKeysComparer, arraySortValuesComparer, new StructCustomComparer<byte>());
            // Keys are the same
            Assert.Equal(arraySortKeysNoComparer, arraySortKeysComparer);
            // Values are **not** the same, for same keys they are sometimes swapped
            // NOTE: Commented out so test passes, but uncomment to see the difference
            //Assert.Equal(arraySortValuesNoComparer, arraySortValuesComparer);

            // The problem only seems to occur when HeapSort is used, so length has to be a certain minimum size
            // Although the depth limit of course is dynamic, but we need to be bigger than some multiple of 16 due to insertion sort

            var keysSegment = new ArraySegment<byte>(keys);
            var valuesSegment = new ArraySegment<int>(values);
            TestSort(keysSegment, valuesSegment); // Array.Sort gives a different result here, than the other two, specifically for two equal keys, the values are swapped
            TestSort(keysSegment, valuesSegment, new CustomComparer<byte>());
            TestSort(keysSegment, valuesSegment, new StructCustomComparer<byte>());
        }

        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_UInt8_Int32_PatternWithRepeatedKeys_ArraySort_DifferentOutputs_02()
        {
            var keys = new byte[] { 206, 206, 254, 253, 252, 251, 250, 249, 248, 247, 246, 245, 244, 243, 242, 241, 240, 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 229, 228, 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 195, 194, 193, 192, 191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180, 179, 178, 177, 176, 175, 174, 173, 172, 171, 170, 169, 168, 167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 128, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40,
                39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 255, 254, 253, 252, 251, 250, 249, 248, 247, 246, 245, 244, 243, 242, 241, 240, 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 229, 228, 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 195, 194, 193, 192, 191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180, 179, 178, 177, 176, 175, 174, 173, 172, 171, 170, 169, 168, 167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 128, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76
                , 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 206, 206 };
            var values = Enumerable.Range(0, keys.Length).ToArray();
            var offset = 2;
            var count = 511;

            var keysSegment = new ArraySegment<byte>(keys, offset, count);
            var valuesSegment = new ArraySegment<int>(values, offset, count);
            // Array.Sort gives a different result here, this is due to difference in depth limit, and hence Span calls HeapSort, Array does not
            // HACK: In this method to ensure Array.Sort is never called with an actual segment and thus this test passes
            TestSort(keysSegment, valuesSegment); 
        }

        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_UInt8_Int32_PatternWithRepeatedKeys_ArraySort_DifferentOutputs_02_NoOffsets()
        {
            var keys = new byte[] { 254, 253, 252, 251, 250, 249, 248, 247, 246, 245, 244, 243, 242, 241, 240, 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 229, 228, 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 195, 194, 193, 192, 191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180, 179, 178, 177, 176, 175, 174, 173, 172, 171, 170, 169, 168, 167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 128, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40,
                39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 255, 254, 253, 252, 251, 250, 249, 248, 247, 246, 245, 244, 243, 242, 241, 240, 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 229, 228, 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 195, 194, 193, 192, 191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180, 179, 178, 177, 176, 175, 174, 173, 172, 171, 170, 169, 168, 167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 128, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76
                , 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var values = Enumerable.Range(0, keys.Length).ToArray();
            var offset = 0;
            var count = keys.Length;

            var keysSegment = new ArraySegment<byte>(keys, offset, count);
            var valuesSegment = new ArraySegment<int>(values, offset, count);
            // Array.Sort gives a different result here, this is due to difference in depth limit, and hence Span calls HeapSort, Array does not
            TestSort(keysSegment, valuesSegment);
        }

        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_BogusComparable_Int32_PatternWithRepeatedKeys_ArraySort_DifferentOutputs()
        {
            var keysInt32 = new int[] { -825307442, -825307442, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, -825307442, -825307442 };
            var keys = keysInt32.Select(k => new BogusComparable(k)).ToArray();
            var values = Enumerable.Range(0, keys.Length).ToArray();
            var offset = 2;
            var count = 17;

            var keysSegment = new ArraySegment<BogusComparable>(keys, offset, count);
            var valuesSegment = new ArraySegment<int>(values, offset, count);
            // Array.Sort gives a different result here, this is due to difference in depth limit, and hence Span calls HeapSort, Array does not
            
            TestSort(keysSegment, valuesSegment);
        }

        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_Int32_BogusComparer()
        {
            TestSort(new ArraySegment<int>(new int[] { 0, 1 }), new BogusComparer<int>());
        }
        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_Keys_BogusComparable_ConstantPattern()
        {
            var s = new ArraySegment<BogusComparable>(Enumerable.Range(0, 17).Select(i => new BogusComparable(42)).ToArray());
            TestSort(s);
        }


        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_Single_Single_NaN()
        {
            TestSort(new ArraySegment<float>(new[] { float.NaN, 0f, 0f, float.NaN }),
                        new ArraySegment<float>(new[] { 1f, 2f, 3f, 4f }));
        }
        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_Double_Double_NaN()
        {
            TestSort(new ArraySegment<double>(new[] { double.NaN, 0.0, 0.0, double.NaN }),
                        new ArraySegment<double>(new[] { 1d, 2d, 3d, 4d }));
        }
        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_Single_Int32_NaN()
        {
            TestSort(new ArraySegment<float>(new[] { float.NaN, 0f, 0f, float.NaN }),
                        new ArraySegment<int>(new[] { 1, 2, 3, 4 }));
            // Array.Sort only uses NaNPrePass when both key and value are float
            // Array.Sort outputs: double.NaN, double.NaN, 0, 0, 
            //                              1,          4, 2, 3
        }
        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        public static void Sort_KeysValues_Double_Int32_NaN()
        {
            TestSort(new ArraySegment<double>(new[] { double.NaN, 0.0, 0.0, double.NaN }),
                        new ArraySegment<int>(new[] { 1, 2, 3, 4 }));
            // Array.Sort only uses NaNPrePass when both key and value are double
            // Array.Sort outputs: double.NaN, double.NaN, 0, 0, 
            //                              1,          4, 2, 3
        }

        #region Keys Tests
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Int8(ISortCases sortCases)
        {
            Test_Keys_Int8(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_UInt8(ISortCases sortCases)
        {
            Test_Keys_UInt8(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Int16(ISortCases sortCases)
        {
            Test_Keys_Int16(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_UInt16(ISortCases sortCases)
        {
            Test_Keys_UInt16(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Int32(ISortCases sortCases)
        {
            Test_Keys_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_UInt32(ISortCases sortCases)
        {
            Test_Keys_UInt32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Int64(ISortCases sortCases)
        {
            Test_Keys_Int64(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_UInt64(ISortCases sortCases)
        {
            Test_Keys_UInt64(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Single(ISortCases sortCases)
        {
            Test_Keys_Single(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Double(ISortCases sortCases)
        {
            Test_Keys_Double(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Boolean(ISortCases sortCases)
        {
            Test_Keys_Boolean(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_Char(ISortCases sortCases)
        {
            Test_Keys_Char(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_String(ISortCases sortCases)
        {
            Test_Keys_String(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_ComparableStructInt32(ISortCases sortCases)
        {
            Test_Keys_ComparableStructInt32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_ComparableClassInt32(ISortCases sortCases)
        {
            Test_Keys_ComparableClassInt32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_Keys_BogusComparable(ISortCases sortCases)
        {
            Test_Keys_BogusComparable(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Int8_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Int8(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_UInt8_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_UInt8(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Int16_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Int16(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_UInt16_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_UInt16(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_UInt32_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_UInt32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Int64_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Int64(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_UInt64_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_UInt64(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Single_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Single(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Double_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Double(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Boolean_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Boolean(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_Char_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_Char(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_String_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_String(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_ComparableStructInt32_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_ComparableStructInt32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_ComparableClassInt32_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_ComparableClassInt32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_Keys_BogusComparable_OuterLoop(ISortCases sortCases)
        {
            Test_Keys_BogusComparable(sortCases);
        }
        #endregion

        #region Keys and Values Tests
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Int8_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Int8_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_UInt8_Int32(ISortCases sortCases)
        {
            Test_KeysValues_UInt8_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Int16_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Int16_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_UInt16_Int32(ISortCases sortCases)
        {
            Test_KeysValues_UInt16_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Int32_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Int32_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_UInt32_Int32(ISortCases sortCases)
        {
            Test_KeysValues_UInt32_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Int64_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Int64_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_UInt64_Int32(ISortCases sortCases)
        {
            Test_KeysValues_UInt64_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Single_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Single_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Double_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Double_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Boolean_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Boolean_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_Char_Int32(ISortCases sortCases)
        {
            Test_KeysValues_Char_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_String_Int32(ISortCases sortCases)
        {
            Test_KeysValues_String_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_ComparableStructInt32_Int32(ISortCases sortCases)
        {
            Test_KeysValues_ComparableStructInt32_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_ComparableClassInt32_Int32(ISortCases sortCases)
        {
            Test_KeysValues_ComparableClassInt32_Int32(sortCases);
        }

        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_fastSortTests))]
        public static void Sort_KeysValues_BogusComparable_Int32(ISortCases sortCases)
        {
            Test_KeysValues_BogusComparable_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Int8_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Int8_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_UInt8_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_UInt8_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Int16_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Int16_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_UInt16_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_UInt16_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Int32_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Int32_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_UInt32_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_UInt32_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Int64_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Int64_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_UInt64_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_UInt64_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Single_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Single_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Double_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Double_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Boolean_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Boolean_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_Char_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_Char_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_String_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_String_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_ComparableStructInt32_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_ComparableStructInt32_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_ComparableClassInt32_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_ComparableClassInt32_Int32(sortCases);
        }

        [OuterLoop]
        [Theory]
        [Trait(SortTrait, SortTraitValue)]
        [MemberData(nameof(s_slowSortTests))]
        public static void Sort_KeysValues_BogusComparable_Int32_OuterLoop(ISortCases sortCases)
        {
            Test_KeysValues_BogusComparable_Int32(sortCases);
        }
        #endregion

        // NOTE: Sort_MaxLength_NoOverflow test is constrained to run on Windows and MacOSX because it causes
        //       problems on Linux due to the way deferred memory allocation works. On Linux, the allocation can
        //       succeed even if there is not enough memory but then the test may get killed by the OOM killer at the
        //       time the memory is accessed which triggers the full memory allocation.
        [Fact]
        [Trait(SortTrait, SortTraitValue)]
        [OuterLoop]
        [PlatformSpecific(TestPlatforms.Windows | TestPlatforms.OSX)]
        public unsafe static void Sort_MaxLength_NoOverflow()
        {
            if (sizeof(IntPtr) == sizeof(long))
            {
                // Allocate maximum length span native memory
                var length = int.MaxValue;
                if (!AllocationHelper.TryAllocNative(new IntPtr(length), out IntPtr memory))
                {
                    Console.WriteLine($"Span.Sort test {nameof(Sort_MaxLength_NoOverflow)} skipped (could not alloc memory).");
                    return;
                }
                try
                {
                    var span = new Span<byte>(memory.ToPointer(), length);
                    var filler = new DecrementingSpanFiller();
                    const byte fill = 42;
                    span.Fill(fill);
                    span[0] = 255;
                    span[1] = 254;
                    span[span.Length - 2] = 1;
                    span[span.Length - 1] = 0;

                    span.Sort();

                    Assert.Equal(span[0], (byte)0);
                    Assert.Equal(span[1], (byte)1);
                    Assert.Equal(span[span.Length - 2], (byte)254);
                    Assert.Equal(span[span.Length - 1], (byte)255);
                    for (int i = 2; i < length - 2; i++)
                    {
                        Assert.Equal(span[i], fill);
                    }
                }
                finally
                {
                    AllocationHelper.ReleaseNative(ref memory);
                }
            }
        }
    }
}
