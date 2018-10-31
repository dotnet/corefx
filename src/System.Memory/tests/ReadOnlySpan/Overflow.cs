// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsRobustUnderMemoryPressure))]
        [OuterLoop]
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
                        var span = new ReadOnlySpan<Guid>(memBlock.ToPointer(), s_guidThreeGiBLimit);

                        int bigIndex = checked(s_guidTwoGiBLimit + 1);
                        uint byteOffset = checked((uint)bigIndex * (uint)sizeof(Guid));
                        Assert.True(byteOffset > int.MaxValue);  // Make sure byteOffset actually overflows 2Gb, or this test is pointless.
                        Guid expectedGuid = Guid.NewGuid();
                        ref Guid expected = ref Unsafe.Add<Guid>(ref memory, bigIndex);
                        expected = expectedGuid;
                        Guid actualGuid = span[bigIndex];
                        Assert.Equal(expectedGuid, actualGuid);

                        ReadOnlySpan<Guid> slice = span.Slice(bigIndex);
                        Assert.True(Unsafe.AreSame<Guid>(ref expected, ref Unsafe.AsRef(in MemoryMarshal.GetReference(slice))));

                        slice = span.Slice(bigIndex, 1);
                        Assert.True(Unsafe.AreSame<Guid>(ref expected, ref Unsafe.AsRef(in MemoryMarshal.GetReference(slice))));
                    }
                    finally
                    {
                        AllocationHelper.ReleaseNative(ref memBlock);
                    }
                }
            }
        }

        private const long ThreeGiB = 3L * 1024L * 1024L * 1024L;
        private const long TwoGiB = 2L * 1024L * 1024L * 1024L;
        private const long OneGiB = 1L * 1024L * 1024L * 1024L;

        private static readonly int s_guidThreeGiBLimit = (int)(ThreeGiB / Unsafe.SizeOf<Guid>());  // sizeof(Guid) requires unsafe keyword and I don't want to mark the entire class unsafe.
        private static readonly int s_guidTwoGiBLimit = (int)(TwoGiB / Unsafe.SizeOf<Guid>());
    }
}
