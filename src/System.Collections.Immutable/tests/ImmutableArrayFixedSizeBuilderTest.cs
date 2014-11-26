// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Immutable.Test
{
    public class ImmutableArrayFixedLengthBuilderTest 
    {
        [Fact]
        public void ToImmutableAndClearDoesClear()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<short>(1);
            builder[0] = 42;
            var array = builder.ToImmutableAndClear();
            Assert.Equal(0, builder.Length);
        }

        [Fact]
        public void ToImmutableAndClearManyTimesIllegal()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(1);
            Assert.Equal(1, builder.ToImmutableAndClear().Length);
            Assert.Throws(typeof(InvalidOperationException), () => builder.ToImmutableAndClear());
        }

        [Fact]
        public void ToImmutableAndClearZeroLengthIsEmpty()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(0);
            var array = builder.ToImmutableAndClear();
            Assert.True(array.IsEmpty);
            Assert.Equal(0, array.Length);
        }

        [Fact]
        public void ToArrayAndClearDoesClear()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<short>(1);
            builder[0] = 42;
            var array = builder.ToArrayAndClear();
            Assert.Equal(0, builder.Length);
        }

        [Fact]
        public void ToArrayAndClearManyTimesIllegal()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(1);
            Assert.Equal(1, builder.ToArrayAndClear().Length);
            Assert.Throws(typeof(InvalidOperationException), () => builder.ToArrayAndClear());
        }

        [Fact]
        public void ToArrayAndClearZeroLengthIsEmpty()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(0);
            var array = builder.ToArrayAndClear();
            Assert.Equal(0, array.Length);
        }

        [Fact]
        public void IsInitialized()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(0);
            Assert.True(builder.IsInitialized);
            builder.ToImmutableAndClear();
            Assert.False(builder.IsInitialized);
            builder.Reset(42);
            Assert.True(builder.IsInitialized);
        }

        [Fact]
        public void NormalConstruction()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(3);
            builder[0] = "cat";
            builder[1] = "dog";
            builder[2] = "fish";

            var array = builder.ToImmutableAndClear();
            Assert.Equal(new[] { "cat", "dog", "fish" }, array);
        }

        [Fact]
        public void ReuseIndexerSet()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(3);
            for (int i = 0; i < 5; i++)
            {
                builder[0] = "cat";
                builder[1] = "dog";
                builder[2] = "fish";
            }

            var array = builder.ToImmutableAndClear();
            Assert.Equal(new[] { "cat", "dog", "fish" }, array);
        }

        [Fact]
        public void IndexerGet()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(2);
            builder[0] = 42;
            Assert.Equal(42, builder[0]);
            builder[1] = 13;
            Assert.Equal(13, builder[1]);
        }

        [Fact]
        public void IndexerBadIndex()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(2);
            Assert.Throws(typeof(IndexOutOfRangeException), () => { int temp = builder[4]; });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { builder[4] = 42; });
        }

        [Fact]
        public void IndexerEmptyBuilder()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(10);
            builder.ToImmutableAndClear();
            Assert.Throws(typeof(InvalidOperationException), () => { int temp = builder[4]; });
            Assert.Throws(typeof(InvalidOperationException), () => { builder[4] = 42; });
        }

        [Fact]
        public void LengthBadRange()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var b = ImmutableArray.CreateFixedLengthBuilder<int>(-1); });
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(0);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => builder.Reset(-1));
        }

        [Fact]
        public void ResetNormalUse()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(0);
            builder.ToImmutableAndClear();
            builder.Reset(2);
            builder[0] = 42;
            builder[1] = 13;
            var array = builder.ToImmutableAndClear();
            Assert.Equal(new[] { 42, 13 }, array);
        }

        [Fact]
        public void ResetWithoutToImmutable()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(0);
            builder.Reset(2);
            builder[0] = 42;
            builder[1] = 13;
            var array = builder.ToImmutableAndClear();
            Assert.Equal(new[] { 42, 13 }, array);
        }

        [Fact]
        public void ResetMultiple()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(1);
            builder.Reset(1);
            Assert.Equal(1, builder.Length);
            builder.Reset(2);
            Assert.Equal(2, builder.Length);
        }

        [Fact]
        public void ResetClearsEntries()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(2);
            builder[0] = "a";
            builder[1] = "b";
            builder.Reset(3);
            for (int i = 0; i < builder.Length; i++)
            {
                Assert.Null(builder[i]);
            }
        }

        [Fact]
        public void ResetSameSizeClearsEntries()
        {
            var size = 2;
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(size);
            builder[0] = "a";
            builder[1] = "b";
            builder.Reset(size);
            for (int i = 0; i < builder.Length; i++)
            {
                Assert.Null(builder[i]);
            }
        }

        [Fact]
        public void ResetZeroIsValid()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<string>(42);
            builder.Reset(0);
            Assert.True(builder.IsInitialized);
            Assert.Equal(0, builder.Length);
        }

        [Fact]
        public void Length()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(10);
            Assert.Equal(10, builder.Length);
            builder.ToImmutableAndClear();
            Assert.Equal(0, builder.Length);
            builder.Reset(5);
            Assert.Equal(5, builder.Length);
        }

        [Fact]
        public void EnumerationOnUninitialized()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(2);
            builder.ToImmutableAndClear();
            Assert.Throws(typeof(InvalidOperationException), () => builder.GetEnumerator());
            Assert.Throws(typeof(InvalidOperationException), () => ((IEnumerable)builder).GetEnumerator());
        }

        [Fact]
        public void EnumerationGeneric()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(2);
            builder[0] = 13;
            builder[1] = 14;

            var e = builder.GetEnumerator();
            Assert.True(e.MoveNext());
            Assert.Equal(13, e.Current);
            Assert.True(e.MoveNext());
            Assert.Equal(14, e.Current);
            Assert.False(e.MoveNext());
        }

        [Fact]
        public void EnumerationNonGeneric()
        {
            var builder = ImmutableArray.CreateFixedLengthBuilder<int>(2);
            builder[0] = 13;
            builder[1] = 14;

            var e = ((IEnumerable)builder).GetEnumerator();
            Assert.True(e.MoveNext());
            Assert.Equal(13, (int)e.Current);
            Assert.True(e.MoveNext());
            Assert.Equal(14, (int)e.Current);
            Assert.False(e.MoveNext());
        }
    }
}
