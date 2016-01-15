// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Diagnostics.Tracing;
using System.Threading;
using Xunit;

namespace System.Buffers.ArrayPool.Tests
{
    public partial class ArrayPoolUnitTests
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
            Assert.Throws<ArgumentOutOfRangeException>("arraysPerBucket", () => ArrayPool<byte>.Create(numberOfArrays: length, maxArrayLength: 16));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public static void CreatingAPoolWithInvalidMaximumArraySizeThrows(int length)
        {
            Assert.Throws<ArgumentOutOfRangeException>("maxLength", () => ArrayPool<byte>.Create(maxArrayLength: length, numberOfArrays: 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public static void RentingWithInvalidLengthThrows(int length)
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create();
            Assert.Throws<ArgumentOutOfRangeException>("minimumLength", () => pool.Rent(length));
        }

        [Fact]
        public static void RentingAValidSizeArraySucceeds()
        {
            Assert.NotNull(ArrayPool<byte>.Create().Rent(100));
        }

        [Fact]
        public static void RentingMultipleArraysGivesBackDifferentInstances()
        {
            ArrayPool<byte> instance = ArrayPool<byte>.Create(numberOfArrays: 2, maxArrayLength: 16);
            Assert.NotSame(instance.Rent(100), instance.Rent(100));
        }

        [Fact]
        public static void RentingMoreArraysThanSpecifiedInCreateWillStillSucceed()
        {
            ArrayPool<byte> instance = ArrayPool<byte>.Create(numberOfArrays: 1, maxArrayLength: 16);
            Assert.NotNull(instance.Rent(100));
            Assert.NotNull(instance.Rent(100));
        }

        [Fact]
        public static void RentCanReturnBiggerArraySizeThanRequested()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(numberOfArrays: 1, maxArrayLength: 32);
            byte[] rented = pool.Rent(27);
            Assert.NotNull(rented);
            Assert.Equal(rented.Length, 32);
        }

        [Fact]
        public static void RentingAnArrayWithLengthGreaterThanSpecifiedInCreateSillSucceeds()
        {
            Assert.NotNull(ArrayPool<byte>.Create(maxArrayLength: 100, numberOfArrays: 1).Rent(200));
        }

        [Fact]
        public static void CallingReturnBufferWithNullBufferThrows()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create();
            Assert.Throws<ArgumentNullException>("buffer", () => pool.Return(null));
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

        [Fact]
        public static void CallingReturnWithoutClearingDoesNotClearTheBuffer()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create();
            byte[] buffer = pool.Rent(4);
            FillArray(buffer);
            pool.Return(buffer, clearArray: false);
            CheckFilledArray(buffer, (byte b1, byte b2) => Assert.Equal(b1, b2));
        }

        [Fact]
        public static void CallingReturnWithClearingDoesClearTheBuffer()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create();
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
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            byte[] rented = pool.Rent(16);
            byte[] allocated = pool.Rent(16);
            pool.Return(rented);
            pool.Return(allocated);
        }

        [Fact]
        public static void NewDefaultArrayPoolWithSmallBufferSizeRoundsToOurSmallestSupportedSize()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 8, numberOfArrays: 1);
            byte[] rented = pool.Rent(8);
            Assert.True(rented.Length == 16);
        }

        [Fact]
        public static void ReturningABufferGreaterThanMaxSizeDoesNotThrow()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            byte[] rented = pool.Rent(32);
            pool.Return(rented);
        }

        [Fact]
        public static void RentingAllBuffersAndCallingRentAgainWillAllocateBufferAndReturnIt()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            byte[] rented1 = pool.Rent(16);
            byte[] rented2 = pool.Rent(16);
            Assert.NotNull(rented1);
            Assert.NotNull(rented2);
        }

        [Fact]
        public static void RentingReturningThenRentingABufferShouldNotAllocate()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            byte[] bt = pool.Rent(16);
            int id = bt.GetHashCode();
            pool.Return(bt);
            bt = pool.Rent(16);
            Assert.Equal(id, bt.GetHashCode());
        }

        private static void ActionFiresSpecificEvent(Action body, int eventId, AutoResetEvent are)
        {
            using (TestEventListener listener = new TestEventListener("System.Buffers.BufferPoolEventSource", EventLevel.Verbose))
            {
                listener.RunWithCallback((EventWrittenEventArgs e) =>
                {
                    if (e.EventId == eventId)
                        are.Set();
                }, body);
            }
        }

        [Fact]
        public static void RentBufferFiresRentedDiagnosticEvent()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            AutoResetEvent are = new AutoResetEvent(false);

            ActionFiresSpecificEvent(() =>
            {
                byte[] bt = pool.Rent(16);
                Assert.True(are.WaitOne(MaxEventWaitTimeoutInMs));
            }, 1, are);
        }

        [Fact]
        public static void ReturnBufferFiresDiagnosticEvent()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            AutoResetEvent are = new AutoResetEvent(false);

            ActionFiresSpecificEvent(() => 
            {
                byte[] bt = pool.Rent(16);
                pool.Return(bt);
                Assert.True(are.WaitOne(MaxEventWaitTimeoutInMs));
            }, 3, are);
        }

        [Fact]
        public static void FirstCallToRentBufferFiresCreatedDiagnosticEvent()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            AutoResetEvent are = new AutoResetEvent(false);

            ActionFiresSpecificEvent(() => 
            {
                byte[] bt = pool.Rent(16);
                Assert.True(are.WaitOne(MaxEventWaitTimeoutInMs));
            }, 2, are);
        }

        [Fact]
        public static void AllocatingABufferDueToBucketExhaustionFiresDiagnosticEvent()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            AutoResetEvent are = new AutoResetEvent(false);

            ActionFiresSpecificEvent(() => 
            {
                byte[] bt = pool.Rent(16);
                byte[] bt2 = pool.Rent(16);
                Assert.True(are.WaitOne(MaxEventWaitTimeoutInMs));
            }, 2, are);
        }

        [Fact]
        public static void RentingBufferOverConfiguredMaximumSizeFiresDiagnosticEvent()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            AutoResetEvent are = new AutoResetEvent(false);

            ActionFiresSpecificEvent(() => 
            {
                byte[] bt = pool.Rent(64);
                Assert.True(are.WaitOne(MaxEventWaitTimeoutInMs));
            }, 2, are);
        }
        
        [Fact]
        public static void ExhaustingBufferBucketFiresDiagnosticEvent()
        {
            ArrayPool<byte> pool = ArrayPool<byte>.Create(maxArrayLength: 16, numberOfArrays: 1);
            AutoResetEvent are = new AutoResetEvent(false);

            ActionFiresSpecificEvent(() => 
            {
                byte[] bt = pool.Rent(16);
                byte[] bt2 = pool.Rent(16);
                Assert.True(are.WaitOne(MaxEventWaitTimeoutInMs));
            }, 4, are);
        }
    }
}
