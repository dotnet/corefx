// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using Xunit;

namespace System.IO.Pipes.Tests
{
    /// <summary>
    /// Tests for the constructors for NamedPipeServerStream
    /// </summary>
    public class NamedPipeTest_CreateServer : NamedPipeTestBase
    {
        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void NullPipeName_Throws_ArgumentNullException(PipeDirection direction)
        {
            AssertExtensions.Throws<ArgumentNullException>("pipeName", () => new NamedPipeServerStream(null));
            AssertExtensions.Throws<ArgumentNullException>("pipeName", () => new NamedPipeServerStream(null, direction));
            AssertExtensions.Throws<ArgumentNullException>("pipeName", () => new NamedPipeServerStream(null, direction, 2));
            AssertExtensions.Throws<ArgumentNullException>("pipeName", () => new NamedPipeServerStream(null, direction, 3, PipeTransmissionMode.Byte));
            AssertExtensions.Throws<ArgumentNullException>("pipeName", () => new NamedPipeServerStream(null, direction, 3, PipeTransmissionMode.Byte, PipeOptions.None));
            AssertExtensions.Throws<ArgumentNullException>("pipeName", () => new NamedPipeServerStream(null, direction, 3, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void ZeroLengthPipeName_Throws_ArgumentException(PipeDirection direction)
        {
            Assert.Throws<ArgumentException>(() => new NamedPipeServerStream(""));
            Assert.Throws<ArgumentException>(() => new NamedPipeServerStream("", direction));
            Assert.Throws<ArgumentException>(() => new NamedPipeServerStream("", direction, 2));
            Assert.Throws<ArgumentException>(() => new NamedPipeServerStream("", direction, 3, PipeTransmissionMode.Byte));
            Assert.Throws<ArgumentException>(() => new NamedPipeServerStream("", direction, 3, PipeTransmissionMode.Byte, PipeOptions.None));
            Assert.Throws<ArgumentException>(() => new NamedPipeServerStream("", direction, 3, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        [PlatformSpecific(TestPlatforms.Windows)] // "anonymous" only reserved on Windows
        public static void ReservedPipeName_Throws_ArgumentOutOfRangeException(PipeDirection direction)
        {
            const string reservedName = "anonymous";
            AssertExtensions.Throws<ArgumentOutOfRangeException>("pipeName", () => new NamedPipeServerStream(reservedName));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("pipeName", () => new NamedPipeServerStream(reservedName, direction));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("pipeName", () => new NamedPipeServerStream(reservedName, direction, 1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("pipeName", () => new NamedPipeServerStream(reservedName, direction, 1, PipeTransmissionMode.Byte));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("pipeName", () => new NamedPipeServerStream(reservedName, direction, 1, PipeTransmissionMode.Byte, PipeOptions.None));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("pipeName", () => new NamedPipeServerStream(reservedName, direction, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0));}

        [Fact]
        public static void Create_PipeName()
        {
            new NamedPipeServerStream(GetUniquePipeName()).Dispose();
        }

        [Fact]
        public static void Create_PipeName_Direction_MaxInstances()
        {
            new NamedPipeServerStream(GetUniquePipeName(), PipeDirection.Out, 1).Dispose();
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)] // can't access SafePipeHandle on Unix until after connection created
        public static void CreateWithNegativeOneServerInstances_DefaultsToMaxServerInstances()
        {
            // When passed -1 as the maxnumberofserverisntances, the NamedPipeServerStream.Windows class
            // will translate that to the platform specific maximum number (255)
            using (var server = new NamedPipeServerStream(GetUniquePipeName(), PipeDirection.InOut, -1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            using (var server2 = new NamedPipeServerStream(PipeDirection.InOut, false, true, server.SafePipeHandle))
            using (var server3 = new NamedPipeServerStream(PipeDirection.InOut, false, true, server.SafePipeHandle))
            {
            }
        }

        [Fact]
        public static void InvalidPipeDirection_Throws_ArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("direction", () => new NamedPipeServerStream("temp1", (PipeDirection)123));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("direction", () => new NamedPipeServerStream("temp1", (PipeDirection)123, 1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("direction", () => new NamedPipeServerStream("temp1", (PipeDirection)123, 1, PipeTransmissionMode.Byte));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("direction", () => new NamedPipeServerStream("temp1", (PipeDirection)123, 1, PipeTransmissionMode.Byte, PipeOptions.None));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("direction", () => new NamedPipeServerStream("tempx", (PipeDirection)123, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public static void InvalidServerInstances_Throws_ArgumentOutOfRangeException(int numberOfServerInstances)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", PipeDirection.In, numberOfServerInstances));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", PipeDirection.In, numberOfServerInstances, PipeTransmissionMode.Byte));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", PipeDirection.In, numberOfServerInstances, PipeTransmissionMode.Byte, PipeOptions.None));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", PipeDirection.In, numberOfServerInstances, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void ServerInstancesOver254_Throws_ArgumentOutOfRangeException(PipeDirection direction)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", direction, 255));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", direction, 255, PipeTransmissionMode.Byte));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", direction, 255, PipeTransmissionMode.Byte, PipeOptions.None));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxNumberOfServerInstances", () => new NamedPipeServerStream("temp3", direction, 255, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void InvalidTransmissionMode_Throws_ArgumentOutOfRangeException(PipeDirection direction)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("transmissionMode", () => new NamedPipeServerStream("temp1", direction, 1, (PipeTransmissionMode)123));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("transmissionMode", () => new NamedPipeServerStream("temp1", direction, 1, (PipeTransmissionMode)123, PipeOptions.None));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("transmissionMode", () => new NamedPipeServerStream("tempx", direction, 1, (PipeTransmissionMode)123, PipeOptions.None, 0, 0));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void InvalidPipeOptions_Throws_ArgumentOutOfRangeException(PipeDirection direction)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("options", () => new NamedPipeServerStream("temp1", direction, 1, PipeTransmissionMode.Byte, (PipeOptions)255));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("options", () => new NamedPipeServerStream("tempx", direction, 1, PipeTransmissionMode.Byte, (PipeOptions)255, 0, 0));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void InvalidBufferSize_Throws_ArgumentOutOfRangeException(PipeDirection direction)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("inBufferSize", () => new NamedPipeServerStream("temp2", direction, 1, PipeTransmissionMode.Byte, PipeOptions.None, -1, 0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("outBufferSize", () => new NamedPipeServerStream("temp2", direction, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, -123));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void NullPipeHandle_Throws_ArgumentNullException(PipeDirection direction)
        {
            AssertExtensions.Throws<ArgumentNullException>("safePipeHandle", () => new NamedPipeServerStream(direction, false, true, null));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void InvalidPipeHandle_Throws_ArgumentException(PipeDirection direction)
        {
            SafePipeHandle pipeHandle = new SafePipeHandle(new IntPtr(-1), true);
            AssertExtensions.Throws<ArgumentException>("safePipeHandle", () => new NamedPipeServerStream(direction, false, true, pipeHandle));
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        public static void BadHandleKind_Throws_IOException(PipeDirection direction)
        {
            using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "_BadHandleKind_Throws_IOException_" + Path.GetRandomFileName()), FileMode.Create, FileAccess.Write, FileShare.None, 8, FileOptions.DeleteOnClose))
            {
                SafeFileHandle safeHandle = fs.SafeFileHandle;

                bool gotRef = false;
                try
                {
                    safeHandle.DangerousAddRef(ref gotRef);
                    IntPtr handle = safeHandle.DangerousGetHandle();

                    SafePipeHandle fakePipeHandle = new SafePipeHandle(handle, ownsHandle: false);
                    Assert.Throws<IOException>(() => new NamedPipeServerStream(direction, false, true, fakePipeHandle));
                }
                finally
                {
                    if (gotRef)
                        safeHandle.DangerousRelease();
                }
            }
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "This scenario is not handled with System.ArgumentException on Full Framework")]
        [PlatformSpecific(TestPlatforms.Windows)] // accessing SafePipeHandle on Unix fails for a non-connected stream
        public static void Windows_CreateFromDisposedServerHandle_Throws_ObjectDisposedException(PipeDirection direction)
        {
            // The pipe is closed when we try to make a new Stream with it
            var pipe = new NamedPipeServerStream(GetUniquePipeName(), direction, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            SafePipeHandle handle = pipe.SafePipeHandle;
            pipe.Dispose();
            Assert.Throws<ObjectDisposedException>(() => new NamedPipeServerStream(direction, true, true, pipe.SafePipeHandle).Dispose());
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, ".NET Core handles this scenario by throwing ArgumentException instead")]
        [PlatformSpecific(TestPlatforms.Windows)] // accessing SafePipeHandle on Unix fails for a non-connected stream
        public static void Windows_CreateFromAlreadyBoundHandle_Throws_ApplicationException(PipeDirection direction)
        {
            // The pipe is already bound
            using (var pipe = new NamedPipeServerStream(GetUniquePipeName(), direction, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                SafePipeHandle handle = pipe.SafePipeHandle;
                Assert.Throws<ApplicationException>(() => new NamedPipeServerStream(direction, true, true, handle));
             }
         }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)] // accessing SafePipeHandle on Unix fails for a non-connected stream
        public static void Unix_GetHandleOfNewServerStream_Throws_InvalidOperationException()
        {
            using (var pipe = new NamedPipeServerStream(GetUniquePipeName(), PipeDirection.Out, 1, PipeTransmissionMode.Byte))
            {
                Assert.Throws<InvalidOperationException>(() => pipe.SafePipeHandle);
            }
        }

        [Theory]
        [InlineData(PipeDirection.In)]
        [InlineData(PipeDirection.InOut)]
        [InlineData(PipeDirection.Out)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, ".NET framework handles this scenario by throwing ApplicationException instead")]
        [PlatformSpecific(TestPlatforms.Windows)] // accessing SafePipeHandle on Unix fails for a non-connected stream
        public static void Windows_CreateFromAlreadyBoundHandle_Throws_ArgumentException(PipeDirection direction)
        {
            // The pipe is already bound
            using (var pipe = new NamedPipeServerStream(GetUniquePipeName(), direction, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Assert.Throws<ArgumentException>(() => new NamedPipeServerStream(direction, true, true, pipe.SafePipeHandle));
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)] // NumberOfServerInstances > 1 isn't supported and has undefined behavior on Unix
        public static void ServerCountOverMaxServerInstances_Throws_IOException()
        {
            string uniqueServerName = GetUniquePipeName();
            using (NamedPipeServerStream server = new NamedPipeServerStream(uniqueServerName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Assert.Throws<IOException>(() => new NamedPipeServerStream(uniqueServerName));
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)] // NumberOfServerInstances > 1 isn't supported and has undefined behavior on Unix
        public static void Windows_ServerCloneWithDifferentDirection_Throws_UnauthorizedAccessException()
        {
            string uniqueServerName = GetUniquePipeName();
            using (NamedPipeServerStream server = new NamedPipeServerStream(uniqueServerName, PipeDirection.In, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Assert.Throws<UnauthorizedAccessException>(() => new NamedPipeServerStream(uniqueServerName, PipeDirection.Out));
            }
        }
    }
}
