// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Buffers.ArrayPool.Tests
{
    public partial class ArrayPoolUnitTests : ArrayPoolTest
    {
        private const int MaxEventWaitTimeoutInMs = 200;

        private struct TestStruct
        {
            internal string InternalRef;
        }

        /*
            NOTE - due to test parallelism and sharing, use an instance pool for testing unless necessary
        */
        [Fact]
        public static void SharedInstanceCreatesAnInstanceOnFirstCall()
        {
            Assert.NotNull(ArrayPool<byte>.Shared);
        }

        [Fact]
        public static void SharedInstanceOnlyCreatesOneInstanceOfOneTypep()
        {
            ArrayPool<byte> instance = ArrayPool<byte>.Shared;
            Assert.Same(instance, ArrayPool<byte>.Shared);
        }

        [Fact]
        public static void CreateWillCreateMultipleInstancesOfTheSameType()
        {
            Assert.NotSame(ArrayPool<byte>.Create(), ArrayPool<byte>.Create());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public static void CreatingAPoolWithInvalidArrayCountThrows(int length)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxArraysPerBucket", () => ArrayPool<byte>.Create(maxArraysPerBucket: length, maxArrayLength: 16));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public static void CreatingAPoolWithInvalidMaximumArraySizeThrows(int length)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxArrayLength", () => ArrayPool<byte>.Create(maxArrayLength: length, maxArraysPerBucket: 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(16)]
        [InlineData(0x40000000)]
        [InlineData(0x7FFFFFFF)]
        public static void CreatingAPoolWithValidMaximumArraySizeSucceeds(int length)
        {
            var pool = ArrayPool<byte>.Create(maxArrayLength: length, maxArraysPerBucket: 1);
            Assert.NotNull(pool);
            Assert.NotNull(pool.Rent(1));
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void RentingWithInvalidLengthThrows(ArrayPool<byte> pool)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("minimumLength", () => pool.Rent(-1));
        }

        [Fact]
        public static void RentingGiganticArraySucceedsOrOOMs()
        {
            try
            {
                int len = 0x70000000;
                byte[] buffer = ArrayPool<byte>.Shared.Rent(len);
                Assert.NotNull(buffer);
                Assert.True(buffer.Length >= len);
            }
            catch (OutOfMemoryException) { }
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void Renting0LengthArrayReturnsSingleton(ArrayPool<byte> pool)
        {
            byte[] zero0 = pool.Rent(0);
            byte[] zero1 = pool.Rent(0);
            byte[] zero2 = pool.Rent(0);
            byte[] one = pool.Rent(1);

            Assert.Same(zero0, zero1);
            Assert.Same(zero1, zero2);
            Assert.NotSame(zero2, one);

            pool.Return(zero0);
            pool.Return(zero1);
            pool.Return(zero2);
            pool.Return(one);

            Assert.Same(zero0, pool.Rent(0));
        }

        [Fact]
        public static void RentingMultipleArraysGivesBackDifferentInstances()
        {
            ArrayPool<byte> instance = ArrayPool<byte>.Create(maxArraysPerBucket: 2, maxArrayLength: 16);
            Assert.NotSame(instance.Rent(100), instance.Rent(100));
        }

        [Fact]
        public static void RentingMoreArraysThanSpecifiedInCreateWillStillSucceed()
        {
            ArrayPool<byte> instance = ArrayPool<byte>.Create(maxArraysPerBucket: 1, maxArrayLength: 16);
            Assert.NotNull(instance.Rent(100));
            Assert.NotNull(instance.Rent(100));
        }

        [Fact]
        public static void RentCanReturnBiggerArraySizeThanRequested()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArraysPerBucket: 1, maxArrayLength: 32);
            byte[] rented = pool.Rent(27);
            Assert.NotNull(rented);
            Assert.Equal(rented.Length, 32);
        }

        [Fact]
        public static void RentingAnArrayWithLengthGreaterThanSpecifiedInCreateStillSucceeds()
        {
            Assert.NotNull(ArrayPool<byte>.Create(maxArrayLength: 100, maxArraysPerBucket: 1).Rent(200));
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void CallingReturnBufferWithNullBufferThrows(ArrayPool<byte> pool)
        {
            AssertExtensions.Throws<ArgumentNullException>("array", () => pool.Return(null));
        }

        private static void FillArray(byte[] buffer)
        {
            for (byte i = 0; i < buffer.Length; i++)
                buffer[i] = i;
        }

        private static void CheckFilledArray(byte[] buffer, Action<byte, byte> assert)
        {
            for (byte i = 0; i < buffer.Length; i++)
            {
                assert(buffer[i], i);
            }
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void CallingReturnWithoutClearingDoesNotClearTheBuffer(ArrayPool<byte> pool)
        {
            byte[] buffer = pool.Rent(4);
            FillArray(buffer);
            pool.Return(buffer, clearArray: false);
            CheckFilledArray(buffer, (byte b1, byte b2) => Assert.Equal(b1, b2));
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void CallingReturnWithClearingDoesClearTheBuffer(ArrayPool<byte> pool)
        {
            byte[] buffer = pool.Rent(4);
            FillArray(buffer);

            // Note - yes this is bad to hold on to the old instance but we need to validate the contract
            pool.Return(buffer, clearArray: true);
            CheckFilledArray(buffer, (byte b1, byte b2) => Assert.Equal(b1, default(byte)));
        }

        [Fact]
        public static void CallingReturnOnReferenceTypeArrayDoesNotClearTheArray()
        {
            ArrayPool<string> pool = ArrayPool<string>.Create();
            string[] array = pool.Rent(2);
            array[0] = "foo";
            array[1] = "bar";
            pool.Return(array, clearArray: false);
            Assert.NotNull(array[0]);
            Assert.NotNull(array[1]);
        }

        [Fact]
        public static void CallingReturnOnReferenceTypeArrayAndClearingSetsTypesToNull()
        {
            ArrayPool<string> pool = ArrayPool<string>.Create();
            string[] array = pool.Rent(2);
            array[0] = "foo";
            array[1] = "bar";
            pool.Return(array, clearArray: true);
            Assert.Null(array[0]);
            Assert.Null(array[1]);
        }

        [Fact]
        public static void CallingReturnOnValueTypeWithInternalReferenceTypesAndClearingSetsValueTypeToDefault()
        {
            ArrayPool<TestStruct> pool = ArrayPool<TestStruct>.Create();
            TestStruct[] array = pool.Rent(2);
            array[0].InternalRef = "foo";
            array[1].InternalRef = "bar";
            pool.Return(array, clearArray: true);
            Assert.Equal(array[0], default(TestStruct));
            Assert.Equal(array[1], default(TestStruct));
        }

        [Fact]
        public static void TakingAllBuffersFromABucketPlusAnAllocatedOneShouldAllowReturningAllBuffers()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
            byte[] rented = pool.Rent(16);
            byte[] allocated = pool.Rent(16);
            pool.Return(rented);
            pool.Return(allocated);
        }

        [Fact]
        public static void NewDefaultArrayPoolWithSmallBufferSizeRoundsToOurSmallestSupportedSize()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 8, maxArraysPerBucket: 1);
            byte[] rented = pool.Rent(8);
            Assert.True(rented.Length == 16);
        }

        [Fact]
        public static void ReturningABufferGreaterThanMaxSizeDoesNotThrow()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
            byte[] rented = pool.Rent(32);
            pool.Return(rented);

            ArrayPool<byte>.Shared.Return(new byte[3 * 1024 * 1024]);
            ArrayPool<char>.Shared.Return(new char[3 * 1024 * 1024]);
            ArrayPool<string>.Shared.Return(new string[3 * 1024 * 1024]);
        }

        [Fact]
        public static void RentingAllBuffersAndCallingRentAgainWillAllocateBufferAndReturnIt()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
            byte[] rented1 = pool.Rent(16);
            byte[] rented2 = pool.Rent(16);
            Assert.NotNull(rented1);
            Assert.NotNull(rented2);
        }

        [Fact]
        public static void RentingReturningThenRentingABufferShouldNotAllocate()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
            byte[] bt = pool.Rent(16);
            int id = bt.GetHashCode();
            pool.Return(bt);
            bt = pool.Rent(16);
            Assert.Equal(id, bt.GetHashCode());
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void CanRentManySizedBuffers(ArrayPool<byte> pool)
        {
            for (int i = 1; i < 10000; i++)
            {
                byte[] buffer = pool.Rent(i);
                Assert.Equal(i <= 16 ? 16 : RoundUpToPowerOf2(i), buffer.Length);
                pool.Return(buffer);
            }
        }

        private static int RoundUpToPowerOf2(int i)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --i;
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i |= i >> 16;
            return i + 1;
        }

        [Theory]
        [InlineData(1, 16)]
        [InlineData(15, 16)]
        [InlineData(16, 16)]
        [InlineData(1023, 1024)]
        [InlineData(1024, 1024)]
        [InlineData(4096, 4096)]
        [InlineData(1024 * 1024, 1024 * 1024)]
        [InlineData(1024 * 1024 + 1, 1024 * 1024 + 1)]
        [InlineData(1024 * 1024 * 2, 1024 * 1024 * 2)]
        public static void RentingSpecificLengthsYieldsExpectedLengths(int requestedMinimum, int expectedLength)
        {
            foreach (ArrayPool<byte> pool in new[] { ArrayPool<byte>.Create(), ArrayPool<byte>.Shared })
            {
                byte[] buffer1 = pool.Rent(requestedMinimum);
                byte[] buffer2 = pool.Rent(requestedMinimum);

                Assert.NotNull(buffer1);
                Assert.Equal(expectedLength, buffer1.Length);

                Assert.NotNull(buffer2);
                Assert.Equal(expectedLength, buffer2.Length);

                Assert.NotSame(buffer1, buffer2);

                pool.Return(buffer2);
                pool.Return(buffer1);
            }

            foreach (ArrayPool<char> pool in new[] { ArrayPool<char>.Create(), ArrayPool<char>.Shared })
            {
                char[] buffer1 = pool.Rent(requestedMinimum);
                char[] buffer2 = pool.Rent(requestedMinimum);

                Assert.NotNull(buffer1);
                Assert.Equal(expectedLength, buffer1.Length);

                Assert.NotNull(buffer2);
                Assert.Equal(expectedLength, buffer2.Length);

                Assert.NotSame(buffer1, buffer2);

                pool.Return(buffer2);
                pool.Return(buffer1);
            }

            foreach (ArrayPool<string> pool in new[] { ArrayPool<string>.Create(), ArrayPool<string>.Shared })
            {
                string[] buffer1 = pool.Rent(requestedMinimum);
                string[] buffer2 = pool.Rent(requestedMinimum);

                Assert.NotNull(buffer1);
                Assert.Equal(expectedLength, buffer1.Length);

                Assert.NotNull(buffer2);
                Assert.Equal(expectedLength, buffer2.Length);

                Assert.NotSame(buffer1, buffer2);

                pool.Return(buffer2);
                pool.Return(buffer1);
            }
        }

        [Fact]
        public static void RentingAfterPoolExhaustionReturnsSizeForCorrespondingBucket_SmallerThanLimit()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 64, maxArraysPerBucket: 2);

            Assert.Equal(16, pool.Rent(15).Length); // try initial bucket
            Assert.Equal(16, pool.Rent(15).Length);

            Assert.Equal(32, pool.Rent(15).Length); // try one more level
            Assert.Equal(32, pool.Rent(15).Length);

            Assert.Equal(16, pool.Rent(15).Length); // fall back to original size
        }

        [Fact]
        public static void RentingAfterPoolExhaustionReturnsSizeForCorrespondingBucket_JustBelowLimit()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 64, maxArraysPerBucket: 2);

            Assert.Equal(32, pool.Rent(31).Length); // try initial bucket
            Assert.Equal(32, pool.Rent(31).Length);

            Assert.Equal(64, pool.Rent(31).Length); // try one more level
            Assert.Equal(64, pool.Rent(31).Length);

            Assert.Equal(32, pool.Rent(31).Length); // fall back to original size
        }

        [Fact]
        public static void RentingAfterPoolExhaustionReturnsSizeForCorrespondingBucket_AtLimit()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 64, maxArraysPerBucket: 2);

            Assert.Equal(64, pool.Rent(63).Length); // try initial bucket
            Assert.Equal(64, pool.Rent(63).Length);

            Assert.Equal(64, pool.Rent(63).Length); // still get original size
        }

        [Fact]
        public static void RentBufferFiresRentedDiagnosticEvent()
        {
            RemoteInvokeWithTrimming(() =>
            {
                ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);

                byte[] buffer = pool.Rent(16);
                pool.Return(buffer);

                Assert.Equal(1, RunWithListener(() => pool.Rent(16), EventLevel.Verbose, e =>
                {
                    Assert.Equal(1, e.EventId);
                    Assert.Equal(buffer.GetHashCode(), e.Payload[0]);
                    Assert.Equal(buffer.Length, e.Payload[1]);
                    Assert.Equal(pool.GetHashCode(), e.Payload[2]);
                }));
            });
        }

        [Fact]
        public static void ReturnBufferFiresDiagnosticEvent()
        {
            RemoteInvokeWithTrimming(() =>
            {
                ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
                byte[] buffer = pool.Rent(16);
                Assert.Equal(1, RunWithListener(() => pool.Return(buffer), EventLevel.Verbose, e =>
                {
                    Assert.Equal(3, e.EventId);
                    Assert.Equal(buffer.GetHashCode(), e.Payload[0]);
                    Assert.Equal(buffer.Length, e.Payload[1]);
                    Assert.Equal(pool.GetHashCode(), e.Payload[2]);
                }));
            });
        }

        [Fact]
        public static void RentingNonExistentBufferFiresAllocatedDiagnosticEvent()
        {
            RemoteInvokeWithTrimming(() =>
            {
                ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
                Assert.Equal(1, RunWithListener(() => pool.Rent(16), EventLevel.Informational, e => Assert.Equal(2, e.EventId)));
            });
        }

        [Fact]
        public static void RentingBufferOverConfiguredMaximumSizeFiresDiagnosticEvent()
        {
            RemoteInvokeWithTrimming(() =>
            {
                ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 1);
                Assert.Equal(1, RunWithListener(() => pool.Rent(64), EventLevel.Informational, e => Assert.Equal(2, e.EventId)));
            });
        }

        [Fact]
        public static void RentingManyBuffersFiresExpectedDiagnosticEvents()
        {
            RemoteInvokeWithTrimming(() =>
            {
                ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, maxArraysPerBucket: 10);
                var list = new List<EventWrittenEventArgs>();

                Assert.Equal(60, RunWithListener(() =>
                {
                    for (int i = 0; i < 10; i++)
                        pool.Return(pool.Rent(16)); // 10 rents + 10 allocations, 10 returns
                    for (int i = 0; i < 10; i++)
                        pool.Return(pool.Rent(0)); // 0 events for empty arrays
                    for (int i = 0; i < 10; i++)
                        pool.Rent(16); // 10 rents
                    for (int i = 0; i < 10; i++)
                        pool.Rent(16); // 10 rents + 10 allocations
                }, EventLevel.Verbose, list.Add));

                Assert.Equal(30, list.Where(e => e.EventId == 1).Count()); // rents
                Assert.Equal(20, list.Where(e => e.EventId == 2).Count()); // allocations
                Assert.Equal(10, list.Where(e => e.EventId == 3).Count()); // returns
            });
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void ReturningANonPooledBufferOfDifferentSizeToThePoolThrows(ArrayPool<byte> pool)
        {
            AssertExtensions.Throws<ArgumentException>("array", () => pool.Return(new byte[1]));
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void RentAndReturnManyOfTheSameSize_NoneAreSame(ArrayPool<byte> pool)
        {
            foreach (int length in new[] { 1, 16, 32, 64, 127, 4096, 4097 })
            {
                for (int iter = 0; iter < 2; iter++)
                {
                    var buffers = new HashSet<byte[]>();
                    for (int i = 0; i < 100; i++)
                    {
                        buffers.Add(pool.Rent(length));
                    }

                    Assert.Equal(100, buffers.Count);

                    foreach (byte[] buffer in buffers)
                    {
                        pool.Return(buffer);
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(BytePoolInstances))]
        public static void UsePoolInParallel(ArrayPool<byte> pool)
        {
            int[] sizes = new int[] { 16, 32, 64, 128 };
            Parallel.For(0, 250000, i =>
            {
                foreach (int size in sizes)
                {
                    byte[] array = pool.Rent(size);
                    Assert.NotNull(array);
                    Assert.InRange(array.Length, size, int.MaxValue);
                    pool.Return(array);
                }
            });
        }

        public static IEnumerable<object[]> BytePoolInstances()
        {
            yield return new object[] { ArrayPool<byte>.Create() };
            yield return new object[] { ArrayPool<byte>.Create(1024*1024, 50) };
            yield return new object[] { ArrayPool<byte>.Create(1024*1024, 1) };
            yield return new object[] { ArrayPool<byte>.Shared };
        }
    }
}
