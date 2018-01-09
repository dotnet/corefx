// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        private sealed class CopyToLargeSizeTestFileCleanup : FileCleanupTestBase
        {
            public string TestFilePath => GetTestFilePath(0, "SpanTests_CopyToLargeSizeTest");
        }

        // This test case tests the Span.CopyTo method for large buffers of size 4GB or more. In the fast path,
        // the CopyTo method performs copy in chunks of size 4GB (uint.MaxValue) with final iteration copying
        // the residual chunk of size (bufferSize % 4GB). The inputs sizes to this method, 4GB and 4GB+256B,
        // test the two size selection paths in CoptyTo method - memory size that is multiple of 4GB or,
        // a multiple of 4GB + some more size.
        [Theory]
        [PlatformSpecific(TestPlatforms.Windows | TestPlatforms.OSX)]
        [InlineData(4L * 1024L * 1024L * 1024L)]
        [InlineData((4L * 1024L * 1024L * 1024L) + 256)]
        public static void CopyToLargeSizeTest(long bufferSize)
        {
            // If this test is run in a 32-bit process, the large allocation will fail.
            if (Unsafe.SizeOf<IntPtr>() != sizeof(long))
            {
                return;
            }

            if (AllocationHelper.TryAllocNative((IntPtr)1, out IntPtr pMemory))  // Synchronize with other big-memory tests to reduce stress on machine. The memory itself isn't used.
            {
                try
                {
                    int GuidCount = (int)(bufferSize / Unsafe.SizeOf<Guid>());

                    using (CopyToLargeSizeTestFileCleanup fileCleanup = new CopyToLargeSizeTestFileCleanup())
                    {
                        string testFilePath = fileCleanup.TestFilePath;
                        using (FileStream fs = File.Create(testFilePath))
                        {
                            try
                            {
                                fs.SetLength(checked(bufferSize * 2));
                            }
                            catch (IOException)
                            {
                                return; // Test machine does not have enough space for a 8gb file.
                            }
                        }

                        using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(testFilePath))
                        using (MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor())
                        using (SafeHandle viewAccessorSafeHandle = viewAccessor.SafeMemoryMappedViewHandle)
                        {
                            unsafe
                            {
                                byte* memBlockFirst = (byte*)viewAccessorSafeHandle.DangerousGetHandle();
                                byte* memBlockSecond = memBlockFirst + bufferSize;

                                ref Guid memoryFirst = ref Unsafe.AsRef<Guid>(memBlockFirst);
                                var spanFirst = new Span<Guid>(memBlockFirst, GuidCount);

                                ref Guid memorySecond = ref Unsafe.AsRef<Guid>(memBlockSecond);
                                var spanSecond = new Span<Guid>(memBlockSecond, GuidCount);

                                Guid theGuid = Guid.Parse("900DBAD9-00DB-AD90-00DB-AD900DBADBAD");
                                for (int count = 0; count < GuidCount; ++count)
                                {
                                    Unsafe.Add(ref memoryFirst, count) = theGuid;
                                }

                                spanFirst.CopyTo(spanSecond);

                                for (int count = 0; count < GuidCount; ++count)
                                {
                                    Guid guidfirst = Unsafe.Add(ref memoryFirst, count);
                                    Guid guidSecond = Unsafe.Add(ref memorySecond, count);
                                    Assert.Equal(guidfirst, guidSecond);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    AllocationHelper.ReleaseNative(ref pMemory);
                }
            }
        }
    }
}
