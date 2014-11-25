// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Immutable.Test
{
    public class ImmutableArrayFixedSizeBuilderTest 
    {
        [Fact]
        public void FreezeClears()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<short>(1);
            builder[0] = 42;
            var array = builder.Freeze();
            Assert.Equal(0, builder.Capacity);
        }

        [Fact]
        public void FreezeManyTimesIllegal()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(1);
            Assert.Equal(1, builder.Freeze().Length);
            Assert.Throws(typeof(InvalidOperationException), () => builder.Freeze());
        }

        [Fact]
        public void FreezeZeroLengthIsEmpty()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(0);
            var array = builder.Freeze();
            Assert.True(array.IsEmpty);
            Assert.Equal(0, array.Length);
        }

        [Fact]
        public void IsInitialized()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(0);
            Assert.True(builder.IsInitialized);
            builder.Freeze();
            Assert.False(builder.IsInitialized);
            builder.Reset(42);
            Assert.True(builder.IsInitialized);
        }

        [Fact]
        public void NormalConstruction()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(3);
            builder[0] = "cat";
            builder[1] = "dog";
            builder[2] = "fish";

            var array = builder.Freeze();
            Assert.Equal(new[] { "cat", "dog", "fish" }, array);
        }

        [Fact]
        public void ReuseIndexerSet()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(3);
            for (int i = 0; i < 5; i++)
            {
                builder[0] = "cat";
                builder[1] = "dog";
                builder[2] = "fish";
            }

            var array = builder.Freeze();
            Assert.Equal(new[] { "cat", "dog", "fish" }, array);
        }

        [Fact]
        public void IndexerGet()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(2);
            builder[0] = 42;
            Assert.Equal(42, builder[0]);
            builder[1] = 13;
            Assert.Equal(13, builder[1]);
        }

        [Fact]
        public void IndexerBadIndex()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(2);
            Assert.Throws(typeof(IndexOutOfRangeException), () => { int temp = builder[4]; });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { builder[4] = 42; });
        }

        [Fact]
        public void IndexerEmptyBuilder()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(10);
            builder.Freeze();
            Assert.Throws(typeof(InvalidOperationException), () => { int temp = builder[4]; });
            Assert.Throws(typeof(InvalidOperationException), () => { builder[4] = 42; });
        }

        [Fact]
        public void CapacityBadRange()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var b = ImmutableArray.CreateFixedSizeBuilder<int>(-1); });
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(0);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => builder.Reset(-1));
        }

        [Fact]
        public void ResetNormalUse()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(0);
            builder.Freeze();
            builder.Reset(2);
            builder[0] = 42;
            builder[1] = 13;
            var array = builder.Freeze();
            Assert.Equal(new[] { 42, 13 }, array);
        }

        [Fact]
        public void ResetWithoutFreeze()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(0);
            builder.Reset(2);
            builder[0] = 42;
            builder[1] = 13;
            var array = builder.Freeze();
            Assert.Equal(new[] { 42, 13 }, array);
        }

        [Fact]
        public void ResetMultiple()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(1);
            builder.Reset(1);
            Assert.Equal(1, builder.Capacity);
            builder.Reset(2);
            Assert.Equal(2, builder.Capacity);
        }

        [Fact]
        public void ResetClearsEntries()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(2);
            builder[0] = "a";
            builder[1] = "b";
            builder.Reset(3);
            for (int i = 0; i < builder.Capacity; i++)
            {
                Assert.Null(builder[i]);
            }
        }

        [Fact]
        public void ResetSameSizeClearsEntries()
        {
            var size = 2;
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(size);
            builder[0] = "a";
            builder[1] = "b";
            builder.Reset(size);
            for (int i = 0; i < builder.Capacity; i++)
            {
                Assert.Null(builder[i]);
            }
        }

        [Fact]
        public void ResetZeroIsValid()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<string>(42);
            builder.Reset(0);
            Assert.True(builder.IsInitialized);
            Assert.Equal(0, builder.Capacity);
        }

        [Fact]
        public void Capacity()
        {
            var builder = ImmutableArray.CreateFixedSizeBuilder<int>(10);
            Assert.Equal(10, builder.Capacity);
            builder.Freeze();
            Assert.Equal(0, builder.Capacity);
            builder.Reset(5);
            Assert.Equal(5, builder.Capacity);
        }
    }
}
