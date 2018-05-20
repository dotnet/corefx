﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class FileStream_CopyTo : FileSystemTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InvalidArgs_Throws(bool useAsync)
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            {
                AssertExtensions.Throws<ArgumentNullException>("destination", () => { fs.CopyTo(null); });
                AssertExtensions.Throws<ArgumentOutOfRangeException>("bufferSize", () => { fs.CopyTo(new MemoryStream(), 0); });
                Assert.Throws<NotSupportedException>(() => { fs.CopyTo(new MemoryStream(new byte[1], writable: false)); });
                fs.Dispose();
                Assert.Throws<ObjectDisposedException>(() => { fs.CopyTo(new MemoryStream()); });
            }

            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.Write))
            {
                Assert.Throws<NotSupportedException>(() => { fs.CopyTo(new MemoryStream()); });
            }

            using (FileStream src = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            using (FileStream dst = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            {
                dst.Dispose();
                Assert.Throws<ObjectDisposedException>(() => { src.CopyTo(dst); });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        // SkipOnTargetFramework matches CopyToAsync tests
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The Stream CopyTo fails on netcoreapp because it calls Length which checks the validity of the underlying handle. On NetFX the operation no-ops for no input or delays failure to execution for input. See /dotnet/coreclr/pull/4540.")]
        public void DisposeHandleThenUseFileStream_CopyTo(bool useAsync)
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            {
                fs.SafeFileHandle.Dispose();
                Assert.Throws<ObjectDisposedException>(() => { fs.CopyTo(new MemoryStream()); });
            }

            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            {
                fs.Write(TestBuffer, 0, TestBuffer.Length);
                fs.SafeFileHandle.Dispose();
                Assert.Throws<ObjectDisposedException>(() => { fs.CopyTo(new MemoryStream()); });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        // SkipOnTargetFramework matches CopyToAsync tests 
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "The Stream CopyTo fails on netcoreapp because it calls Length which checks the validity of the underlying handle. On NetFX the operation no-ops for no input or delays failure to execution for input. See /dotnet/coreclr/pull/4540.")]
        public void DisposeHandleThenUseFileStream_CopyTo_netfx(bool useAsync)
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            {
                fs.SafeFileHandle.Dispose();
                fs.CopyTo(new MemoryStream());
            }

            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x100, useAsync))
            {
                fs.Write(TestBuffer, 0, TestBuffer.Length);
                fs.SafeFileHandle.Dispose();
                try
                {
                    fs.CopyTo(new MemoryStream());
                }
                catch (AggregateException e)
                {
                    Assert.Equal(typeof(ObjectDisposedException), e.InnerException.GetType());
                }
            }
        }

        [Theory] // inner loop, just a few cases
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void File_AllDataCopied_InnerLoop(bool useAsync, bool preWrite)
        {
            File_AllDataCopied(
                _ => new MemoryStream(), useAsync, preRead: false, preWrite: preWrite, exposeHandle: false,
                bufferSize: 4096, writeSize: 1024, numWrites: 10);
        }

        [Theory] // outer loop, many combinations
        [OuterLoop]
        [MemberData(nameof(File_AllDataCopied_MemberData))]
        public void File_AllDataCopied(
            Func<string, Stream> createDestinationStream,
            bool useAsync, bool preRead, bool preWrite, bool exposeHandle,
            int bufferSize, int writeSize, int numWrites)
        {
            // Create the expected data
            long totalLength = writeSize * numWrites;
            var expectedData = new byte[totalLength];
            new Random(42).NextBytes(expectedData);

            // Write it out into the source file
            string srcPath = GetTestFilePath();
            File.WriteAllBytes(srcPath, expectedData);

            string dstPath = GetTestFilePath();
            using (FileStream src = new FileStream(srcPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, bufferSize, useAsync))
            using (Stream dst = createDestinationStream(dstPath))
            {
                // If configured to expose the handle, do so.  This influences the stream's need to ensure the position is in sync.
                if (exposeHandle)
                {
                    var ignored = src.SafeFileHandle;
                }

                // If configured to "preWrite", do a write before we start reading.
                if (preWrite)
                {
                    src.Write(new byte[] { 42 }, 0, 1);
                    dst.Write(new byte[] { 42 }, 0, 1);
                    expectedData[0] = 42;
                }

                // If configured to "preRead", read one byte from the source prior to the CopyTo.
                // This helps test what happens when there's already data in the buffer, when the position
                // isn't starting at zero, etc.
                if (preRead)
                {
                    int initialByte = src.ReadByte();
                    if (initialByte >= 0)
                    {
                        dst.WriteByte((byte)initialByte);
                    }
                }

                // Do the copy
                src.CopyTo(dst, writeSize);
                dst.Flush();

                // Make sure we're at the end of the source file
                Assert.Equal(src.Length, src.Position);

                // Verify the copied data
                dst.Position = 0;
                using (var result = new MemoryStream())
                {
                    dst.CopyTo(result);
                    byte[] actualData = result.ToArray();
                    Assert.Equal(expectedData.Length, actualData.Length);
                    Assert.Equal<byte>(expectedData, actualData);
                }
            }
        }

        private static IEnumerable<object[]> File_AllDataCopied_MemberData()
        {
            bool[] bools = new[] { true, false };
            foreach (bool useAsync in bools) // sync or async mode
            {
                foreach (bool preRead in bools) // whether to do a read before the CopyTo
                {
                    for (int streamType = 0; streamType < 2; streamType++) // kind of stream to use
                    {
                        Func<string, Stream> createDestinationStream;
                        switch (streamType)
                        {
                            case 0:
                                createDestinationStream = _ => new MemoryStream();
                                break;
                            default:
                                createDestinationStream = s => File.Create(s);
                                break;
                        }

                        // Various exposeHandle (whether the SafeFileHandle was publicly accessed), 
                        // preWrite, bufferSize, writeSize, and numWrites combinations
                        yield return new object[] { createDestinationStream, useAsync, preRead, false, false, 0x1000, 0x100, 100 };
                        yield return new object[] { createDestinationStream, useAsync, preRead, false, false, 0x1, 0x1, 1000 };
                        yield return new object[] { createDestinationStream, useAsync, preRead, false, true, 0x2, 0x100, 100 };
                        yield return new object[] { createDestinationStream, useAsync, preRead, false, false, 0x4000, 0x10, 100 };
                        yield return new object[] { createDestinationStream, useAsync, preRead, false, true, 0x1000, 99999, 10 };
                    }
                }
            }
        }

        [Theory]
        [InlineData(10, 1024)]
        public void AnonymousPipeViaFileStream_AllDataCopied(int writeSize, int numWrites)
        {
            long totalLength = writeSize * numWrites;
            var expectedData = new byte[totalLength];
            new Random(42).NextBytes(expectedData);

            var results = new MemoryStream();

            using (var server = new AnonymousPipeServerStream(PipeDirection.Out))
            {
                Task serverTask = Task.Run(() =>
                {
                    for (int i = 0; i < numWrites; i++)
                    {
                        // later test will write to a pipe asynchronously 
                        server.Write(expectedData, i * writeSize, writeSize);
                    }
                    server.Dispose();
                });

                using (var client = new FileStream(new SafeFileHandle(server.ClientSafePipeHandle.DangerousGetHandle(), false), FileAccess.Read, bufferSize: 3))
                {
                    Task copyTask = Task.Run(() => client.CopyTo(results, writeSize));
                    Task.WaitAll(serverTask, copyTask);
                }
            }

            byte[] actualData = results.ToArray();
            Assert.Equal(expectedData.Length, actualData.Length);
            Assert.Equal<byte>(expectedData, actualData);
        }

        [PlatformSpecific(TestPlatforms.Windows)] // Uses P/Invokes to create async pipe handle
        [Theory]
        [InlineData(false, 10, 1024)]
        [InlineData(true, 10, 1024)]
        [ActiveIssue(22271, TargetFrameworkMonikers.UapNotUapAot)]
        public void NamedPipeViaFileStream_AllDataCopied(bool useAsync, int writeSize, int numWrites)
        {
            long totalLength = writeSize * numWrites;
            var expectedData = new byte[totalLength];
            new Random(42).NextBytes(expectedData);

            var results = new MemoryStream();
            var pipeOptions = useAsync ? PipeOptions.Asynchronous : PipeOptions.None;

            string name = GetNamedPipeServerStreamName();
            using (var server = new NamedPipeServerStream(name, PipeDirection.Out, 1, PipeTransmissionMode.Byte, pipeOptions))
            {
                Task serverTask = Task.Run(async () =>
                {
                    await server.WaitForConnectionAsync();
                    for (int i = 0; i < numWrites; i++)
                    {
                        await server.WriteAsync(expectedData, i * writeSize, writeSize);
                    }
                    server.Dispose();
                });

                using (SafeFileHandle clientHandle = CreateFileW(@"\\.\pipe\" + name, GENERIC_READ, FileShare.None, IntPtr.Zero, FileMode.Open, (int)pipeOptions, IntPtr.Zero))
                using (var client = new FileStream(clientHandle, FileAccess.Read, bufferSize: 3, isAsync: useAsync))
                {
                    client.CopyTo(results, (int)totalLength);
                    serverTask.Wait();
                }
            }

            byte[] actualData = results.ToArray();
            Assert.Equal(expectedData.Length, actualData.Length);
            Assert.Equal<byte>(expectedData, actualData);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DerivedFileStream_ReadInvoked(bool useAsync)
        {
            var expectedData = new byte[100];
            new Random(42).NextBytes(expectedData);

            string srcPath = GetTestFilePath();
            File.WriteAllBytes(srcPath, expectedData);

            bool readInvoked = false;
            using (var fs = new FileStreamThatOverridesRead(srcPath, useAsync, () => readInvoked = true))
            {
                fs.CopyTo(new MemoryStream());
                Assert.True(readInvoked);
            }
        }

        private class FileStreamThatOverridesRead : FileStream
        {
            private readonly Action _readInvoked;

            internal FileStreamThatOverridesRead(string path, bool useAsync, Action readInvoked) :
                base(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, useAsync)
            {
                _readInvoked = readInvoked;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                _readInvoked();
                return base.Read(buffer, offset, count);
            }
        }

        #region Windows P/Invokes

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeFileHandle CreateFileW(
            string lpFileName, int dwDesiredAccess, FileShare dwShareMode,
            IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        internal const int GENERIC_READ = unchecked((int)0x80000000);
        #endregion
    }
}
