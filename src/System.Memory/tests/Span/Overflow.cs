// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        // NOTE: IndexOverflow test is constrained to run on Windows and MacOSX because it
        //       causes problems on Linux due to the way deferred memory allocation works.
        //       On Linux, the allocation can succeed even if there is not enough memory
        //       but then the test may get killed by the OOM killer at the time the memory
        //       is accessed which triggers the full memory allocation.

        [Fact]
        [OuterLoop]
        [PlatformSpecific(TestPlatforms.Windows | TestPlatforms.OSX)]
        public static void IndexOverflow()
        {
            // If this test is run in a 32-bit process, the 3GB allocation will fail.
            if (Unsafe.SizeOf<IntPtr>() == sizeof(long))
            {
                //
                // Although Span constrains indexes to 0..2Gb, it does not similarly constrain index * sizeof(T).
                // Make sure that internal offset calculcations handle the >2Gb case properly.
                //

                unsafe
                {
                    if (!AllocationHelper.TryAllocNative((IntPtr)ThreeGiB, out IntPtr memBlock))
                        return; // It's not implausible to believe that a 3gb allocation will fail - if so, skip this test to avoid unnecessary test flakiness.

                    try
                    {
                        ref Guid memory = ref Unsafe.AsRef<Guid>(memBlock.ToPointer());
                        var span = new Span<Guid>(memBlock.ToPointer(), s_guidThreeGiBLimit);

                        int bigIndex = checked(s_guidTwoGiBLimit + 1);
                        uint byteOffset = checked((uint)bigIndex * (uint)sizeof(Guid));
                        Assert.True(byteOffset > int.MaxValue);  // Make sure byteOffset actually overflows 2Gb, or this test is pointless.
                        ref Guid expected = ref Unsafe.Add<Guid>(ref memory, bigIndex);

                        Assert.True(Unsafe.AreSame<Guid>(ref expected, ref span[bigIndex]));

                        Span<Guid> slice = span.Slice(bigIndex);
                        Assert.True(Unsafe.AreSame<Guid>(ref expected, ref MemoryMarshal.GetReference(slice)));

                        slice = span.Slice(bigIndex, 1);
                        Assert.True(Unsafe.AreSame<Guid>(ref expected, ref MemoryMarshal.GetReference(slice)));
                    }
                    finally
                    {
                        AllocationHelper.ReleaseNative(ref memBlock);
                    }
                }
            }
        }


        [Fact]
        public static void SliceStartInt32Overflow()
        {
            IntPtr huge;
            try
            {
                huge = Marshal.AllocHGlobal(new IntPtr(ThreeGiB));
            }
            catch (Exception)
            {
                return;  // It's not implausible to believe that a 3gb allocation will fail - if so, skip this test to avoid unnecessary test flakiness.
            }

            try
            {
                unsafe
                {
                    int
                        GuidThreeGiBLimit = (int)(ThreeGiB / sizeof(Guid)),
                        GuidTwoGiBLimit = (int)(TwoGiB / sizeof(Guid)),
                        GuidOneGiBLimit = (int)(OneGiB / sizeof(Guid));


                    var span = new Span<Guid>((void*)huge, GuidThreeGiBLimit);
                    Guid guid = Guid.NewGuid();
                    var slice = span.Slice(GuidTwoGiBLimit + 1);
                    slice[0] = guid;

                    slice = span.Slice(GuidOneGiBLimit).Slice(1).Slice(GuidOneGiBLimit);
                    Assert.Equal(guid, slice[0]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(huge);
            }
        }

        [Fact]
        public static void SliceStartUInt32Overflow()
        {
            IntPtr huge;
            try
            {
                huge = Marshal.AllocHGlobal(new IntPtr(ThreeGiB));
            }
            catch (Exception)
            {
                return;  // It's not implausible to believe that a 3gb allocation will fail - if so, skip this test to avoid unnecessary test flakiness.
            }

            try
            {
                unsafe
                {
                    int
                        GuidThreeGiBLimit = (int)(ThreeGiB / sizeof(Guid)),
                        GuidTwoGiBLimit = (int)(TwoGiB / sizeof(Guid)),
                        GuidOneGiBLimit = (int)(OneGiB / sizeof(Guid));
                    var span = new Span<Guid>((void*)huge, GuidThreeGiBLimit);
                    Guid guid = Guid.NewGuid();
                    var slice = span.Slice((GuidTwoGiBLimit + 1));
                    slice[0] = guid;

                    slice = span.Slice(GuidOneGiBLimit).Slice(1).Slice(GuidOneGiBLimit);
                    Assert.Equal(guid, slice[0]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(huge);
            }
        }

        [Fact]
        public static void ArrayCtorStartOverflow()
        {
            var arr = new Guid[20];

            var slice = arr.AsSpan().Slice(2);
            Guid guid = Guid.NewGuid();
            slice[1] = guid;

            Assert.Equal(guid, arr[3]);
        }

        [Fact]
        public static void ReadOnlySliceStartInt32Overflow()
        {
            IntPtr huge;
            try
            {
                huge = Marshal.AllocHGlobal(new IntPtr(ThreeGiB));
            }
            catch (Exception)
            {
                return;  // It's not implausible to believe that a 3gb allocation will fail - if so, skip this test to avoid unnecessary test flakiness.
            }

            try
            {
                unsafe
                {
                    int
                        GuidThreeGiBLimit = (int)(ThreeGiB / sizeof(Guid)),
                        GuidTwoGiBLimit = (int)(TwoGiB / sizeof(Guid)),
                        GuidOneGiBLimit = (int)(OneGiB / sizeof(Guid));
                    var mutable = new Span<Guid>((void*)huge, GuidThreeGiBLimit);
                    var span = new ReadOnlySpan<Guid>((void*)huge, GuidThreeGiBLimit);
                    Guid guid = Guid.NewGuid();
                    var slice = span.Slice(GuidTwoGiBLimit + 1);
                    mutable[GuidTwoGiBLimit + 1] = guid;
                    Assert.Equal(guid, slice[0]);

                    slice = span.Slice(GuidOneGiBLimit).Slice(1).Slice(GuidOneGiBLimit);
                    Assert.Equal(guid, slice[0]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(huge);
            }
        }

        [Fact]
        public static void ReadOnlySliceStartUInt32Overflow()
        {
            IntPtr huge;
            try
            {
                huge = Marshal.AllocHGlobal(new IntPtr(ThreeGiB));
            }
            catch (Exception)
            {
                return;  // It's not implausible to believe that a 3gb allocation will fail - if so, skip this test to avoid unnecessary test flakiness.
            }

            try
            {
                unsafe
                {
                    int
                        GuidThreeGiBLimit = (int)(ThreeGiB / sizeof(Guid)),
                        GuidTwoGiBLimit = (int)(TwoGiB / sizeof(Guid)),
                        GuidOneGiBLimit = (int)(OneGiB / sizeof(Guid));

                    var mutable = new Span<Guid>((void*)huge, GuidThreeGiBLimit);
                    var span = new ReadOnlySpan<Guid>((void*)huge, GuidThreeGiBLimit);
                    Guid guid = Guid.NewGuid();
                    var slice = span.Slice((GuidTwoGiBLimit + 1));
                    mutable[GuidTwoGiBLimit + 1] = guid;
                    Assert.Equal(guid, slice[0]);

                    slice = span.Slice(GuidOneGiBLimit).Slice(1).Slice(GuidOneGiBLimit);
                    Assert.Equal(guid, slice[0]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(huge);
            }
        }

        [Fact]
        public static void ReadOnlySliceStartLengthInt32Overflow()
        {
            IntPtr huge;
            try
            {
                huge = Marshal.AllocHGlobal(new IntPtr(ThreeGiB));
            }
            catch (Exception)
            {
                return;  // It's not implausible to believe that a 3gb allocation will fail - if so, skip this test to avoid unnecessary test flakiness.
            }

            try
            {
                unsafe
                {
                    int
                        GuidThreeGiBLimit = (int)(ThreeGiB / sizeof(Guid)),
                        GuidTwoGiBLimit = (int)(TwoGiB / sizeof(Guid)),
                        GuidOneGiBLimit = (int)(OneGiB / sizeof(Guid));

                    var mutable = new Span<Guid>((void*)huge, GuidThreeGiBLimit);
                    var span = new ReadOnlySpan<Guid>((void*)huge, GuidThreeGiBLimit);
                    Guid guid = Guid.NewGuid();
                    var slice = span.Slice(GuidTwoGiBLimit + 1, 20);
                    mutable[GuidTwoGiBLimit + 1] = guid;
                    Assert.Equal(guid, slice[0]);

                    slice = span.Slice(GuidOneGiBLimit).Slice(1).Slice(GuidOneGiBLimit);
                    Assert.Equal(guid, slice[0]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(huge);
            }
        }

        [Fact]
        public static void ReadOnlyArrayCtorStartLengthOverflow()
        {
            var arr = new Guid[20];

            var slice = new ReadOnlySpan<Guid>(arr, 2, 2);
            Guid guid = Guid.NewGuid();
            arr[3] = guid;

            Assert.Equal(guid, slice[1]);
        }

        private const long ThreeGiB = 3L * 1024L * 1024L * 1024L;
        private const long TwoGiB = 2L * 1024L * 1024L * 1024L;
        private const long OneGiB = 1L * 1024L * 1024L * 1024L;

        

        private static readonly int s_guidThreeGiBLimit = (int)(ThreeGiB / Unsafe.SizeOf<Guid>());  // sizeof(Guid) requires unsafe keyword and I don't want to mark the entire class unsafe.
        private static readonly int s_guidTwoGiBLimit = (int)(TwoGiB / Unsafe.SizeOf<Guid>());
    }

}
